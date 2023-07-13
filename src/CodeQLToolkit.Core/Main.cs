
using CodeQLDevelopmentLifecycleToolkit.Features.Query;
using System.CommandLine;
using CodeQLToolkit.Shared.Logging;
using Microsoft.Extensions.Logging;

namespace CodeQLDevelopmentLifecycleToolkit.Core
{

    internal class QLT
    {       

        static int Main(string[] args)
        {
            Log<QLT>.G().LogInformation("QLT Startup...");
            
            RootCommand rootCommand = new RootCommand();

            // Register the `Query` feature
            QueryFeatureMain.Instance.Register(rootCommand);

            return 0;            
        }
    }
}