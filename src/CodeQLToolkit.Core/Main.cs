
using CodeQLDevelopmentLifecycleToolkit.Features.Query;
using System.CommandLine;
using CodeQLToolkit.Shared.Logging;
using Microsoft.Extensions.Logging;
using System.IO;
using CodeQLToolkit.Shared.Options;
using CodeQLToolkit.Shared.Utils;

namespace CodeQLDevelopmentLifecycleToolkit.Core
{

    internal class QLT
    {
        public static async Task<int> Main(string[] args)
        {
            Log<QLT>.G().LogInformation("QLT Startup...");
            
            var rootCommand = new RootCommand();

            // Add global option for the root directory
          
            rootCommand.AddGlobalOption(Globals.BasePathOption);

            var versionCommand = new Command("version", "Get the current tool version.");
            rootCommand.Add(versionCommand);

            versionCommand.SetHandler(() =>
            {
                var version = File.ReadAllText(Path.Combine(FileUtils.GetExecutingDirectory().FullName, "ver.txt"));

                Console.Write($"QLT Version: {version}");
            });


            // Register the `Query` feature
            QueryFeatureMain.Instance.Register(rootCommand);

            await rootCommand.InvokeAsync(args);

            return 0;            
        }
    }
}