using CodeQLToolkit.Features.Bundle.Commands.Targets;
using CodeQLToolkit.Shared.Utils;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Bundle.Commands
{
    public class BundleCommandFeature : FeatureBase, IToolkitLifecycleFeature
    {
        public override LanguageType[] SupportedLangauges
        {
            get => new LanguageType[] {
            LanguageType.C,
            LanguageType.CPP,
            LanguageType.CSHARP,
            LanguageType.JAVA,
            LanguageType.JAVASCRIPT,
            LanguageType.GO,
            LanguageType.RUBY,
            LanguageType.PYTHON
        };
        }

        public BundleCommandFeature()
        {
            FeatureName = "Bundle";
        }

        public void Register(Command parentCommand)
        {
            Log<BundleCommandFeature>.G().LogInformation("Registering command submodule.");


            var runCommand = new Command("run", "Functions pertaining running bundle commands.");


            var validateIntegrationTestsCommand = new Command("validate-integration-tests", "Validates the results of an integration test using a semantic diff.");
            var expectedOption = new Option<string>("--expected", "The SARIF file containing the expected results.") { IsRequired = true };
            var actualOption = new Option<string>("--actual", "The SARIF file containing the actual results.") { IsRequired = true };

            validateIntegrationTestsCommand.Add(expectedOption);
            validateIntegrationTestsCommand.Add(actualOption);

            runCommand.Add(validateIntegrationTestsCommand);

            parentCommand.Add(runCommand);


            validateIntegrationTestsCommand.SetHandler((basePath, expected, actual) =>
            {
                Log<BundleCommandFeature>.G().LogInformation("Executing validate-integration-tests command...");

                new ValidateIntegrationTestResults()
                {
                    Base = basePath,
                    Expected = expected,
                    Actual = actual

                }.Run();

            },Globals.BasePathOption, expectedOption, actualOption);
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
