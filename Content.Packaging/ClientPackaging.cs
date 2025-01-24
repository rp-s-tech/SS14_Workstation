using System.Diagnostics;
using System.IO.Compression;
using Robust.Packaging;
using Robust.Packaging.AssetProcessing;
using Robust.Packaging.AssetProcessing.Passes;
using Robust.Packaging.Utility;
using Robust.Shared.Timing;

namespace Content.Packaging;

public static class ClientPackaging
{
    private static readonly bool UseExodus = File.Exists(Path.Combine("Exodus", "ExodusSecrets.sln")); // Exodus-Secrets
    private static readonly string RPSXClientPath = Path.Combine("RPSX", "Content.RPSXClient", "Content.RPSXClient.csproj");
    // private static readonly string SecretSharedPath = Path.Combine("RPSX", "Content.RPSXShared", "Content.RPSXShared.csproj");

    private static readonly bool UseRPSXClient = File.Exists(RPSXClientPath);
    /// <summary>
    /// Be advised this can be called from server packaging during a HybridACZ build.
    /// </summary>
    public static async Task PackageClient(bool skipBuild, string configuration, IPackageLogger logger)
    {
        logger.Info("Building client...");

        if (!skipBuild)
        {
            await ProcessHelpers.RunCheck(new ProcessStartInfo
            {
                FileName = "dotnet",
                ArgumentList =
                {
                    "build",
                    Path.Combine("Content.Client", "Content.Client.csproj"),
                    "-c", configuration,
                    "--nologo",
                    "/v:m",
                    "/t:Rebuild",
                    "/p:FullRelease=true",
                    "/m"
                }
            });

            if (UseRPSXClient)
            {
                await ProcessHelpers.RunCheck(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    ArgumentList =
                    {
                        "build",
                        RPSXClientPath,
                        "-c", "Release",
                        "--nologo",
                        "/v:m",
                        "/t:Rebuild",
                        "/p:FullRelease=true",
                        "/m"
                    }
                });
            }
            // Exodus-Secrets-Start
            if (UseExodus)
            {
                await ProcessHelpers.RunCheck(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    ArgumentList =
                    {
                        "build",
                        Path.Combine("Exodus", "Content.Exodus.Client", "Content.Exodus.Client.csproj"),
                        "-c", configuration,
                        "--nologo",
                        "/v:m",
                        "/t:Rebuild",
                        "/p:FullRelease=true",
                        "/m"
                    }
                });
            }
            // Exodus-Secrets-End
        }

        logger.Info("Packaging client...");

        var sw = RStopwatch.StartNew();
        {
            await using var zipFile =
                File.Open(Path.Combine("release", "SS14.Client.zip"), FileMode.Create, FileAccess.ReadWrite);
            using var zip = new ZipArchive(zipFile, ZipArchiveMode.Update);
            var writer = new AssetPassZipWriter(zip);

            await WriteResources("", writer, logger, default);
            await writer.FinishedTask;
        }

        logger.Info($"Finished packaging client in {sw.Elapsed}");
    }

    public static async Task WriteResources(
        string contentDir,
        AssetPass pass,
        IPackageLogger logger,
        CancellationToken cancel)
    {
        var graph = new RobustClientAssetGraph();
        pass.Dependencies.Add(new AssetPassDependency(graph.Output.Name));

        var dropSvgPass = new AssetPassFilterDrop(f => f.Path.EndsWith(".svg"))
        {
            Name = "DropSvgPass",
        };
        dropSvgPass.AddDependency(graph.Input).AddBefore(graph.PresetPasses);

        AssetGraph.CalculateGraph([pass, dropSvgPass, ..graph.AllPasses], logger);

        var inputPass = graph.Input;

        var assemblies = new List<string> { "Content.Client", "Content.Shared", "Content.Shared.Database" };
        if (UseRPSXClient)
        {
            assemblies.Add("Content.RPSXClient");
            // assemblies.Add("Content.RPSXShared");
        }
        // Exodus-Secrets-Start: Add Corvax interfaces to Magic ACZ
        if (UseExodus)
            assemblies.AddRange(["Content.Exodus.Shared", "Content.Exodus.Client"]);
        // Exodus-Secrets-End

        await RobustSharedPackaging.WriteContentAssemblies(
            inputPass,
            contentDir,
            "Content.Client",
            assemblies,
            cancel: cancel);

        await RobustClientPackaging.WriteClientResources(contentDir, inputPass, cancel);

        inputPass.InjectFinished();
    }
}
