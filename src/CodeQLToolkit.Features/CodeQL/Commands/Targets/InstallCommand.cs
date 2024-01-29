using CodeQLToolkit.Shared.CodeQL;
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
            }


            Log<InstallCommand>.G().LogInformation($"Done.");




        }
    }
}
