using CodeQLToolkit.Shared.Utils;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Bundle.Lifecycle.Targets
{
    public class GetEnabledCustomCodeQLBundlesLifecycleTarget : ILifecycleTarget 
    {       
        override public void Run()
        {
            Log<GetEnabledCustomCodeQLBundlesLifecycleTarget>.G().LogInformation("Running get enabled command...");

            var c = new QLTConfig()
            {               
                Base = Base
            };

            var config = c.FromFile();

            Console.WriteLine($"---------current settings---------");
            Console.WriteLine($"CodeQL Custom Bundles Enabled: {config.EnableCustomCodeQLBundles}");
            Console.WriteLine($"----------------------------------");
            Console.WriteLine("(hint: use `qlt bundle set` to modify these values.)");

        }
    }
}
