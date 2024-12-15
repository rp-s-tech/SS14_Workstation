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
    private static readonly string SecretClientPath = Path.Combine("RPSX", "Content.RPSXClient", "Content.RPSXClient.csproj");
    // private static readonly string SecretSharedPath = Path.Combine("RPSX", "Content.RPSXShared", "Content.RPSXShared.csproj");

    private static readonly bool UseSecretsClient = File.Exists(SecretClientPath);
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

            if (UseSecretsClient)
            {
                await ProcessHelpers.RunCheck(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    ArgumentList =
                    {
                        "build",
                        SecretClientPath,
                        "-c", "Release",
                        "--nologo",
                        "/v:m",
                        "/t:Rebuild",
                        "/p:FullRelease=true",
                        "/m"
                    }
                });
            }
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

        AssetGraph.CalculateGraph(graph.AllPasses.Append(pass).ToArray(), logger);

        var inputPass = graph.Input;

        var assemblies = new List<string>{ "Content.Client", "Content.Shared", "Content.Shared.Database" };
        if (UseSecretsClient)
        {
            assemblies.Add("Content.RPSXClient");
            // assemblies.Add("Content.RPSXShared");
        }

        await RobustSharedPackaging.WriteContentAssemblies(
            inputPass,
            contentDir,
            "Content.Client",
            assemblies,
            cancel: cancel);

        await RobustClientPackaging.WriteClientResources(contentDir, pass, cancel);

        inputPass.InjectFinished();
    }
}
