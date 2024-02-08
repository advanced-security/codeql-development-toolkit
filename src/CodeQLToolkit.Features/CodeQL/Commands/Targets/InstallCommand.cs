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

        public override void Run()
        {
            Log<InstallCommand>.G().LogInformation($"Running Install command");

            // First, check if CodeQL is installed.
            var installation = CodeQLInstallation.LoadFromConfig(Base);

            Log<InstallCommand>.G().LogInformation($"Checking for installation...");

            if (installation.IsInstalled())
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

                if (AutomationTypeHelper.AutomationTypeFromString(AutomationTarget) == AutomationType.ACTIONS)
                {
                    File.AppendAllText(Environment.GetEnvironmentVariable("GITHUB_ENV"), $"QLT_CODEQL_HOME={installation.CodeQLHome}" + "\n");
                    File.AppendAllText(Environment.GetEnvironmentVariable("GITHUB_ENV"), $"QLT_CODEQL_PATH={installation.CodeQLToolBinary}" + "\n");
                }

            }


            Log<InstallCommand>.G().LogInformation($"Done.");




        }
    }
}
