using CodeQLToolkit.Shared.CodeQL;
using CodeQLToolkit.Shared.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.CodeQL.Commands.Targets
{
    public class InstallCommand : CommandTarget
    {
        public bool CustomBundles { get; set; }
        public bool QuickBundles { get; set; }
        public string[] Packs { get; set; }
        public override void Run()
        {
            Log<InstallCommand>.G().LogInformation($"Running Install command");

            // First, check if CodeQL is installed.
            var installation = CodeQLInstallation.LoadFromConfig(Base);
            if(CustomBundles || QuickBundles)
            {
                installation.EnableCustomCodeQLBundles = true;
                if (Packs!=null && Packs.Length > 0)
                {
                    Log<InstallCommand>.G().LogInformation($"Overriding Packs on the command line. The following Packs will be packaged:");
                    installation.ExportedCustomizationPacks = Packs;
                }
                else
                {
                    Log<InstallCommand>.G().LogInformation($"Using `qlt.conf.json` to build list of packs to bundle. The following Packs will be packaged:");
                }

                installation.LogPacksToBeBuilt();


                installation.QuickBundle = QuickBundles;
            }

            Log<InstallCommand>.G().LogInformation($"Checking for installation...");

            // if it is the case that it is installed but we are in custom bundle mode we RE install it. 

            if (installation.IsInstalled() && !installation.EnableCustomCodeQLBundles)
            {
                Log<InstallCommand>.G().LogInformation($"CodeQL is already installed at that version. Please delete the installation directory to reinstall.");
            }
            else
            {
                Log<InstallCommand>.G().LogInformation($"Installing CodeQL...");
                installation.Install();

                // set the environment variable
                Log<InstallCommand>.G().LogInformation($"Setting QLT_CODEQL_HOME to {installation.CodeQLHome}...");
                Log<InstallCommand>.G().LogInformation($"Setting QLT_CODEQL_PATH to {installation.CodeQLToolBinary}...");

                Environment.SetEnvironmentVariable("QLT_CODEQL_HOME", installation.CodeQLHome);
                Environment.SetEnvironmentVariable("QLT_CODEQL_PATH", installation.CodeQLToolBinary);
                if (CustomBundles || QuickBundles)
                {
                    Environment.SetEnvironmentVariable("QLT_CODEQL_BUNDLE_PATH", installation.CustomBundleOutputBundle);
                }

                if (AutomationTypeHelper.AutomationTypeFromString(AutomationTarget) == AutomationType.ACTIONS)
                {
                    if (Environment.GetEnvironmentVariable("GITHUB_ENV") != null && File.Exists(Environment.GetEnvironmentVariable("GITHUB_ENV")))
                    {
                        
                        File.AppendAllText(Environment.GetEnvironmentVariable("GITHUB_ENV"), $"QLT_CODEQL_HOME={installation.CodeQLHome}" + "\n");
                        File.AppendAllText(Environment.GetEnvironmentVariable("GITHUB_ENV"), $"QLT_CODEQL_PATH={installation.CodeQLToolBinary}" + "\n");
                        if (CustomBundles || QuickBundles)
                        {
                            File.AppendAllText(Environment.GetEnvironmentVariable("GITHUB_ENV"), $"QLT_CODEQL_BUNDLE_PATH={installation.CustomBundleOutputBundle}" + "\n");
                        }
                    }
                }

            }


            Log<InstallCommand>.G().LogInformation($"Done.");




        }
    }
}
