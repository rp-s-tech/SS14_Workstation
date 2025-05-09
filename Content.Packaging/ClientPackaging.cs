﻿using System.Diagnostics;
using System.IO.Compression;
using Robust.Packaging;
using Robust.Packaging.AssetProcessing;
using Robust.Packaging.AssetProcessing.Passes;
using Robust.Packaging.Utility;
using Robust.Shared.Timing;

namespace Content.Packaging;

public static class ClientPackaging
{
    // RPSX-Secrets-Start
    private static readonly string RPSXClientPath = Path.Combine("RPSX", "Content.RPSX.Client", "Content.RPSX.Client.csproj");
    private static readonly bool UseRPSX = File.Exists(RPSXClientPath);
    // RPSX-Secrets-End


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

            // RPSX-Secrets-Start
            if (UseRPSX)
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
            // RPSX-Secrets-End
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

        // RPSX-Secrets-Start
        if (UseRPSX)
            assemblies.AddRange(["Content.RPSX.Shared", "Content.RPSX.Client"]);
        // RPSX-Secrets-End

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
