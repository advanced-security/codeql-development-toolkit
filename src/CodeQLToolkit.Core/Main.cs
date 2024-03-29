﻿
using CodeQLDevelopmentLifecycleToolkit.Features.Query;
using System.CommandLine;
using CodeQLToolkit.Shared.Logging;
using Microsoft.Extensions.Logging;
using System.IO;
using CodeQLToolkit.Shared.Options;
using CodeQLToolkit.Shared.Utils;
using CodeQLToolkit.Features.CodeQL;
using CodeQLToolkit.Features.Test;
using CodeQLToolkit.Features.Pack;
using CodeQLToolkit.Features.Validation;
using CodeQLToolkit.Features.Bundle;

namespace CodeQLDevelopmentLifecycleToolkit.Core
{

    internal class QLT
    {
        public static async Task<int> Main(string[] args)
        {
            Log<QLT>.G().LogInformation("QLT Startup...");

            Console.OutputEncoding = System.Text.Encoding.UTF8; 

            var rootCommand = new RootCommand();

            // Add global option for the root directory          
            rootCommand.AddGlobalOption(Globals.BasePathOption);
            rootCommand.AddGlobalOption(Globals.AutomationTypeOption);
            rootCommand.AddGlobalOption(Globals.Development);
            rootCommand.AddGlobalOption(Globals.UseBundle);

            var versionCommand = new Command("version", "Get the current tool version.");
            rootCommand.Add(versionCommand);

            versionCommand.SetHandler(() =>
            {
                var version = File.ReadAllText(Path.Combine(FileUtils.GetExecutingDirectory().FullName, "ver.txt"));

                Console.Write($"QLT Version: {version}");
            });

            // Register the `Query` feature
            QueryFeatureMain.Instance.Register(rootCommand);
            // Register the `CodeQL` feature
            CodeQLFeatureMain.Instance.Register(rootCommand);
            // Register the `Test` feature
            TestFeatureMain.Instance.Register(rootCommand);
            // Register the `Pack` feature
            PackFeatureMain.Instance.Register(rootCommand);
            // Register the `Validation` feature
            ValidationFeatureMain.Instance.Register(rootCommand);
            // Register the `Bundle` feature
            BundleFeatureMain.Instance.Register(rootCommand);

            return await rootCommand.InvokeAsync(args);
        }
    }
}