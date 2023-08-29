using CodeQLToolkit.Features.Test.Lifecycle.Targets;
using CodeQLToolkit.Features.Test.Lifecycle.Targets.Actions;
using System.CommandLine;
using System.Reflection;

namespace CodeQLToolkit.Features.Test.Lifecycle
{
    public class TestLifecycleFeature : FeatureBase, IToolkitLifecycleFeature
    {
        public TestLifecycleFeature() 
        {
            FeatureName = "Test";
        }

        public override string[] SupportedLangauges { get => new string[] { "c", "cpp", "javascript", "go", "python", "ql", "java", "ruby" }; }

        public void Register(Command parentCommand)
        {
            Log<TestLifecycleFeature>.G().LogInformation("Registering lifecycle submodule.");

            var initCommand = new Command("init", "Initialize testing in this repository.");

            var overwriteExistingOption = new Option<bool>("--overwrite-existing", () => false, "Overwrite exiting files (if they exist).");
            var numThreadsOption = new Option<int>("--num-threads", () => 4, "Number of threads to use during test execution.");
            var useRunnerOption = new Option<string>("--use-runner", () => "ubuntu-latest", "The runner(s) to use. Should be a comma-seperated list of actions runners.");
            var languageOption = new Option<string>("--language", $"The language to generate automation for.") { IsRequired = true }.FromAmong(SupportedLangauges);

            initCommand.AddOption(overwriteExistingOption);
            initCommand.AddOption(numThreadsOption);
            initCommand.AddOption(useRunnerOption);
            initCommand.AddOption(languageOption);



            parentCommand.Add(initCommand);


            
            initCommand.SetHandler((basePath, automationType, overwriteExisting, numThreads, useRunner, language) =>
            {
                Log<TestLifecycleFeature>.G().LogInformation("Executing init command...");

                //
                // dispatch at runtime to the correct automation type
                //
                var featureTarget = AutomationFeatureFinder.FindTargetForAutomationType<BaseLifecycleTarget>(AutomationTypeHelper.AutomationTypeFromString(automationType));

                // setup common params 
                featureTarget.FeatureName = FeatureName;
                featureTarget.Base = basePath;             
                featureTarget.OverwriteExisting = overwriteExisting;
                featureTarget.NumThreads = numThreads;
                featureTarget.UseRunner = useRunner;    
                featureTarget.Language = language;

                featureTarget.Run();

            }, Globals.BasePathOption, Globals.AutomationTypeOption, overwriteExistingOption, numThreadsOption, useRunnerOption, languageOption);

        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
