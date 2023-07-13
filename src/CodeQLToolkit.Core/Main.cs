
using CodeQLDevelopmentLifecycleToolkit.Features.Query;
using System.CommandLine;
using CodeQLToolkit.Shared.Logging;
using Microsoft.Extensions.Logging;
using System.IO;

namespace CodeQLDevelopmentLifecycleToolkit.Core
{

    internal class QLT
    {
        public static async Task<int> Main(string[] args)
        {
            Log<QLT>.G().LogInformation("QLT Startup...");
            
            var rootCommand = new RootCommand();

            // Add global option for the root directory
            var basePathOption = new Option<string>("--base", () => {
                return Directory.GetCurrentDirectory();
            }, "The base path to find the query repository.");
            rootCommand.AddGlobalOption(basePathOption);


            // Register the `Query` feature
            QueryFeatureMain.Instance.Register(rootCommand);

            await rootCommand.InvokeAsync(args);

            return 0;            
        }
    }
}