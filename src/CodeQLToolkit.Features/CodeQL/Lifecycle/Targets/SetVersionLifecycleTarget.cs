using CodeQLToolkit.Shared.Utils;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.CodeQL.Lifecycle.Targets
{
    public class SetVersionLifecycleTarget : ILifecycleTarget 
    {
        public string CodeQLCLI { get; set; }
        public string CodeQLStandardLibrary { get; set; }
        public string CodeQLCLIBundle { get; set; }

        override public void Run()
        {
            Log<SetVersionLifecycleTarget>.G().LogInformation("Running set command...");

            var c = new QLTConfig()
            {
                CodeQLCLI = CodeQLCLI,
                CodeQLStandardLibrary = CodeQLStandardLibrary,
                CodeQLCLIBundle = CodeQLCLIBundle,
                Base = Base
            };

            c.ToFile();

            Log<SetVersionLifecycleTarget>.G().LogInformation("Wrote to file...");

        }
    }
}
