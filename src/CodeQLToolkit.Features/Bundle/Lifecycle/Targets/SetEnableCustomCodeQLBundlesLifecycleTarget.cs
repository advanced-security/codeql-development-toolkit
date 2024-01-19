using CodeQLToolkit.Shared.Utils;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Bundle.Lifecycle.Targets
{
    public class SetEnableCustomCodeQLBundlesLifecycleTarget : ILifecycleTarget 
    {       
        override public void Run()
        {
            Log<SetEnableCustomCodeQLBundlesLifecycleTarget>.G().LogInformation("Running set command...");

            var c = new QLTConfig()
            {               
                Base = Base
            };

            var config = c.FromFile();

            config.EnableCustomCodeQLBundles = true;

            config.ToFile();

            Log<SetEnableCustomCodeQLBundlesLifecycleTarget>.G().LogInformation("Wrote to file...");

        }
    }
}
