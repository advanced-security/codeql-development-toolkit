using CodeQLToolkit.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.CodeQL.Lifecycle.Targets
{
    public class GetVersionLifecycleTarget : ILifecycleTarget
    {
       
        override public void Run()
        {
            Log<GetVersionLifecycleTarget>.G().LogInformation("Running get command...");

            var c = new QLTConfig()
            {
                Base = Base
            };

            if (!File.Exists(c.CodeQLConfigFilePath))
            {
                ProcessUtils.DieWithError($"Cannot read values from missing file {c.CodeQLConfigFilePath}");
            }
           
            var config = c.FromFile();

            // This should be updated so that we can pretty print all the various options:
            Console.WriteLine($"---------current settings---------");
            Console.WriteLine($"CodeQL CLI Version: {config.CodeQLCLI}");
            Console.WriteLine($"CodeQL Standard Library Version: {config.CodeQLStandardLibrary}");
            Console.WriteLine($"CodeQL CLI Bundle Version: {config.CodeQLCLIBundle}");
            Console.WriteLine($"----------------------------------");
            Console.WriteLine("(hint: use `qlt codeql set` to modify these values.)");


        }
    }
}