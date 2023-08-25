using CodeQLToolkit.Features.Test.Lifecycle;
using CodeQLToolkit.Shared.Types;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Test.Commands
{
    public class TestCommandFeature : FeatureBase, IToolkitLifecycleFeature
    {
        public TestCommandFeature()
        {
            FeatureName = "Test";
        }

        public void Register(Command parentCommand)
        {
            Log<TestCommandFeature>.G().LogInformation("Registering command submodule.");


            var runCommand = new Command("run", "Functions pertaining to running test-related commands.");
            parentCommand.Add(runCommand);

            // a command that installs query packs
            var getMatrixTestCommand = new Command("get-matrix", "Gets a CI/CD matrix based on the current configuration.");

            var matrixOSVersion = new Option<string>("--os-version", () => "ubuntu-latest", "A comma-seperated list of operating systems to use. Example: `ubuntu-latest`.") { IsRequired = true };
            getMatrixTestCommand.Add(matrixOSVersion);

            runCommand.Add(getMatrixTestCommand);

            getMatrixTestCommand.SetHandler((basePath, automationType, osVersions) => {

                Log<TestCommandFeature>.G().LogInformation("Executing get-matrix command...");

                // dispatch at runtime to the correct automation type
                var featureTarget = AutomationFeatureFinder.FindTargetForAutomationType<BaseGetMatrixCommandTarget>(AutomationTypeHelper.AutomationTypeFromString(automationType));

                featureTarget.Base = basePath;
                featureTarget.OSVersions = osVersions.Split(",");

                featureTarget.Run();

            }, Globals.BasePathOption, Globals.AutomationTypeOption, matrixOSVersion);            
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
