using CodeQLToolkit.Features.Test.Lifecycle;
using CodeQLToolkit.Shared.Utils;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Validation.Lifecycle
{
    internal class ValidationLifecycleFeature : FeatureBase, IToolkitLifecycleFeature
    {
        public ValidationLifecycleFeature()
        {
            FeatureName = "Validation";
        }

        public override LanguageType[] SupportedLangauges => new LanguageType[] {
            LanguageType.C,
            LanguageType.CPP,
            LanguageType.CSHARP,
            LanguageType.JAVA,
            LanguageType.JAVASCRIPT,
            LanguageType.GO,
            LanguageType.RUBY,
            LanguageType.PYTHON
        };

        public void Register(Command parentCommand)
        {
            Log<ValidationLifecycleFeature>.G().LogInformation("Registering lifecycle submodule.");

            var initCommand = new Command("init", "Initialize validation checking in this repository.");

            var overwriteExistingOption = new Option<bool>("--overwrite-existing", () => false, "Overwrite exiting files (if they exist).");
            var languageOption = new Option<string>("--language", $"The language to generate automation for.") { IsRequired = true }.FromAmong(SupportedLangauges.Select(x => x.ToOptionString()).ToArray());
            var useRunnerOption = new Option<string>("--use-runner", () => "ubuntu-latest", "The runner(s) to use. Should be a comma-seperated list of actions runners.");
            var branchOption = new Option<string>("--branch", () => "main", "Branch to use as the target for triggering automation.");

            initCommand.AddOption(overwriteExistingOption);
            initCommand.AddOption(languageOption);
            initCommand.AddOption(useRunnerOption);
            initCommand.AddOption(branchOption);

            parentCommand.Add(initCommand);

            initCommand.SetHandler((devMode, basePath, automationType, overwriteExisting, language, useRunner, branch) =>
            {
                Log<TestLifecycleFeature>.G().LogInformation("Executing init command...");

                //
                // dispatch at runtime to the correct automation type
                //
                var featureTarget = AutomationFeatureFinder.FindTargetForAutomationType<BaseLifecycleTarget>(AutomationTypeHelper.AutomationTypeFromString(automationType));

                // setup common params 
                featureTarget.FeatureName = FeatureName;
                featureTarget.Base = basePath;
                featureTarget.UseRunner = useRunner;
                featureTarget.OverwriteExisting = overwriteExisting;
                featureTarget.Language = language;
                featureTarget.DevMode = devMode;
                featureTarget.Branch = branch;
                featureTarget.Run();

            }, Globals.Development, Globals.BasePathOption, Globals.AutomationTypeOption, overwriteExistingOption, languageOption, useRunnerOption, branchOption);

        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
