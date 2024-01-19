using CodeQLToolkit.Features.CodeQL.Lifecycle.Targets;
using CodeQLToolkit.Features.CodeQL.Lifecycle;
using CodeQLToolkit.Features.Test.Lifecycle.Targets;
using CodeQLToolkit.Features.Test.Lifecycle.Targets.Actions;
using CodeQLToolkit.Shared.Utils;
using System.CommandLine;
using System.Reflection;
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

            var setCommand = new Command("set", "Functions pertaining to setting variables related to custom CodeQL bundles.");
            parentCommand.Add(setCommand);

            var enableCommand = new Command("enable-custom-bundles", "Enables custom CodeQL Bundles.");            
            setCommand.Add(enableCommand);

            var disableCommand = new Command("disable-custom-bundles", "Disables custom CodeQL Bundles.");
            setCommand.Add(disableCommand);

            var getCommand = new Command("get", "Functions pertaining to getting variables related to CodeQL Bundles.");
            parentCommand.Add(getCommand);

            var getEnabledCommand = new Command("enabled", "Determines if custom CodeQL Bundles are enabled.");
            getCommand.Add(getEnabledCommand);

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


        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
