using CodeQLToolkit.Features.CodeQL.Lifecycle;
using CodeQLToolkit.Shared.Utils;
using System.CommandLine;
using CodeQLToolkit.Features.Bundle.Lifecycle.Targets;

namespace CodeQLToolkit.Features.Bundle.Lifecycle
{
    public class BundleLifecycleFeature : FeatureBase, IToolkitLifecycleFeature
    {
        public BundleLifecycleFeature() 
        {
            FeatureName = "Bundle";
        }

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

        public void Register(Command parentCommand)
        {
            Log<BundleLifecycleFeature>.G().LogInformation("Registering lifecycle submodule.");

            var initCommand = new Command("init", "Initialize bundle creation and integration testing features.");
            var overwriteExistingOption = new Option<bool>("--overwrite-existing", () => false, "Overwrite exiting files (if they exist).");
            var useRunnerOption = new Option<string>("--use-runner", () => "ubuntu-latest", "The runner(s) to use. Should be a comma-seperated list of actions runners.");
            var languageOption = new Option<string>("--language", $"The language to generate automation for.") { IsRequired = true }.FromAmong(SupportedLangauges.Select(x => x.ToOptionString()).ToArray());

            initCommand.AddOption(overwriteExistingOption);
            initCommand.AddOption(useRunnerOption);
            initCommand.AddOption(languageOption);

            parentCommand.Add(initCommand);

            var setCommand = new Command("set", "Functions pertaining to setting variables related to custom CodeQL bundles.");
            //parentCommand.Add(setCommand);

            var enableCommand = new Command("enable-custom-bundles", "Enables custom CodeQL Bundles.");            
            //setCommand.Add(enableCommand);

            var disableCommand = new Command("disable-custom-bundles", "Disables custom CodeQL Bundles.");
            //setCommand.Add(disableCommand);

            var getCommand = new Command("get", "Functions pertaining to getting variables related to CodeQL Bundles.");
            //parentCommand.Add(getCommand);

            var getEnabledCommand = new Command("enabled", "Determines if custom CodeQL Bundles are enabled.");
            //getCommand.Add(getEnabledCommand);

            {
                enableCommand.SetHandler((basePath) =>
                {
                    Log<CodeQLLifecycleFeature>.G().LogInformation("Executing enable command...");

                    new SetEnableCustomCodeQLBundlesLifecycleTarget()
                    {
                        Base = basePath
                    }.Run();

                }, Globals.BasePathOption);
            }

            {
                disableCommand.SetHandler((basePath) =>
                {
                    Log<CodeQLLifecycleFeature>.G().LogInformation("Executing get enabled command...");

                    new SetDisableCustomCodeQLBundlesLifecycleTarget()
                    {
                        Base = basePath
                    }.Run();

                }, Globals.BasePathOption);
            }


            {
                getEnabledCommand.SetHandler((basePath) =>
                {
                    Log<CodeQLLifecycleFeature>.G().LogInformation("Executing disable command...");

                    new GetEnabledCustomCodeQLBundlesLifecycleTarget()
                    {
                        Base = basePath
                    }.Run();

                }, Globals.BasePathOption);
            }


            initCommand.SetHandler((devMode, basePath, automationType, overwriteExisting, useRunner, language) =>
            {
                Log<BundleLifecycleFeature>.G().LogInformation("Executing init command...");

                //
                // dispatch at runtime to the correct automation type
                //
                var featureTarget = AutomationFeatureFinder.FindTargetForAutomationType<BaseLifecycleTarget>(AutomationTypeHelper.AutomationTypeFromString(automationType));

                // setup common params 
                featureTarget.FeatureName = FeatureName;
                featureTarget.Base = basePath;
                featureTarget.OverwriteExisting = overwriteExisting;
                featureTarget.UseRunner = useRunner;
                featureTarget.Language = language;
                featureTarget.DevMode = devMode;
                featureTarget.Run();

            }, Globals.Development, Globals.BasePathOption, Globals.AutomationTypeOption, overwriteExistingOption, useRunnerOption, languageOption);


        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
