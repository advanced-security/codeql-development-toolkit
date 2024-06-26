﻿using CodeQLToolkit.Features.Test.Lifecycle.Targets;
using CodeQLToolkit.Features.Test.Lifecycle.Targets.Actions;
using CodeQLToolkit.Shared.Utils;
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
            Log<TestLifecycleFeature>.G().LogInformation("Registering lifecycle submodule.");

            var initCommand = new Command("init", "Initialize testing in this repository.");

            var overwriteExistingOption = new Option<bool>("--overwrite-existing", () => false, "Overwrite exiting files (if they exist).");
            var numThreadsOption = new Option<int>("--num-threads", () => 4, "Number of threads to use during test execution.");
            var useRunnerOption = new Option<string>("--use-runner", () => "ubuntu-latest", "The runner(s) to use. Should be a comma-seperated list of actions runners.");
            var languageOption = new Option<string>("--language", $"The language to generate automation for.") { IsRequired = true }.FromAmong(SupportedLangauges.Select(x => x.ToOptionString()).ToArray());
            //var extraCodeQLOptions = new Option<string>("--codeql-args", $"Extra arguments to pass to CodeQL.") { IsRequired = false };
            var branchOption = new Option<string>("--branch", () => "main", "Branch to use as the target for triggering automation.");

            initCommand.AddOption(overwriteExistingOption);
            initCommand.AddOption(numThreadsOption);
            initCommand.AddOption(useRunnerOption);
            initCommand.AddOption(languageOption);
            //initCommand.AddOption(extraCodeQLOptions);
            initCommand.AddOption(branchOption);

            parentCommand.Add(initCommand);



            initCommand.SetHandler((devMode, basePath, automationType, overwriteExisting, numThreads, useRunner, language, branch) =>
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
                //featureTarget.ExtraArgs = extraArgs;
                featureTarget.DevMode = devMode;
                featureTarget.Branch = branch;
                featureTarget.Run();

            }, Globals.Development, Globals.BasePathOption, Globals.AutomationTypeOption, overwriteExistingOption, numThreadsOption, useRunnerOption, languageOption, branchOption);

        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
