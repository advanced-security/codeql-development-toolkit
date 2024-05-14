using CodeQLToolkit.Features.Test.Commands.Targets;
using CodeQLToolkit.Features.Test.Lifecycle;
using CodeQLToolkit.Shared.Types;
using CodeQLToolkit.Shared.Utils;
using Microsoft.VisualBasic;
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

        public TestCommandFeature()
        {
            FeatureName = "Test";
        }

        public void Register(Command parentCommand)
        {
            Log<TestCommandFeature>.G().LogInformation("Registering command submodule.");


            var runCommand = new Command("run", "Functions pertaining to running test-related commands.");
            parentCommand.Add(runCommand);

            // a command that gets the matrix configuration 
            var getMatrixTestCommand = new Command("get-matrix", "Gets a CI/CD matrix based on the current configuration.");

            var matrixOSVersion = new Option<string>("--os-version", () => "ubuntu-latest", "A comma-seperated list of operating systems to use. Example: `ubuntu-latest`.") { IsRequired = true };
            getMatrixTestCommand.Add(matrixOSVersion);

            // a command that runs the actual tests 
            var unitTestsCommand = new Command("execute-unit-tests", "Runs unit tests within a repository based on the current configuration.");

            var numThreadsOption = new Option<int>("--num-threads", () => 4, "The number of threads to use for runner. For best performance, do not exceed the number of physical cores on your system.") { IsRequired = true };
            var workDirectoryOption = new Option<string>("--work-dir", () => Path.GetTempPath(), "Where to place intermediate execution output files.") { IsRequired = true };
            var languageOption = new Option<string>("--language", $"The language to run tests for.") { IsRequired = true }.FromAmong(SupportedLangauges.Select(x => x.ToOptionString()).ToArray());
            var runnerOSOption = new Option<string>("--runner-os", $"Label for the operating system running these tests.") { IsRequired = true };
            //var cliVersionOption = new Option<string>("--cli-version", $"The version of the cli running the tests.") { IsRequired = true };
            //var stdLibIdentOption = new Option<string>("--stdlib-ident", $"A string identifying the standard library used.") { IsRequired = true };
            var extraCodeQLOptions = new Option<string>("--codeql-args", $"Extra arguments to pass to CodeQL.") { IsRequired = false };

            unitTestsCommand.Add(numThreadsOption);
            unitTestsCommand.Add(workDirectoryOption);
            unitTestsCommand.Add(languageOption);
            unitTestsCommand.Add(runnerOSOption);
            //unitTestsCommand.Add(cliVersionOption);
            //unitTestsCommand.Add(stdLibIdentOption);
            unitTestsCommand.Add(extraCodeQLOptions);

            // a command validates the tests 
            var validateUnitTestsCommand = new Command("validate-unit-tests", "Validates a unit test run in a fashion suitable for use in CI/CD systems.");

            var resultsDirectoryOption = new Option<string>("--results-directory", "Where to find the intermediate execution output files.") { IsRequired = true };
            var prettyPrintOption = new Option<bool>("--pretty-print", () => false, "Pretty print test output in a compact format. Note this will not exit with a failure code if tests fail.") { IsRequired = true };

            validateUnitTestsCommand.Add(resultsDirectoryOption);
            validateUnitTestsCommand.Add(prettyPrintOption);

            runCommand.Add(getMatrixTestCommand);
            runCommand.Add(unitTestsCommand);
            runCommand.Add(validateUnitTestsCommand);

            getMatrixTestCommand.SetHandler((basePath, automationType, osVersions) =>
            {

                Log<TestCommandFeature>.G().LogInformation("Executing get-matrix command...");

                // dispatch at runtime to the correct automation type
                var featureTarget = AutomationFeatureFinder
                    .FindTargetForAutomationType<BaseGetMatrixCommandTarget>(
                    AutomationTypeHelper.AutomationTypeFromString(automationType)
                    );

                featureTarget.Base = basePath;
                featureTarget.OSVersions = osVersions.Split(",");

                featureTarget.Run();

            }, Globals.BasePathOption, Globals.AutomationTypeOption, matrixOSVersion);

            //stdLibIdent
            unitTestsCommand.SetHandler((basePath, automationType, numThreads, workDirectory, language, runnerOS, extraArgs, useBundle) =>
            {

                Log<TestCommandFeature>.G().LogInformation("Executing execute-unit-tests command...");

                // dispatch at runtime to the correct automation type
                var featureTarget = AutomationFeatureFinder
                    .FindTargetForAutomationType<BaseExecuteUnitTestsCommandTarget>(
                    AutomationTypeHelper.AutomationTypeFromString(automationType)
                    );

                // lookup cliVersion and stdLibIdent 
                var c = new QLTConfig()
                {
                    Base = basePath
                };

                if (!File.Exists(c.QLTConfigFilePath))
                {
                    ProcessUtils.DieWithError($"Cannot read values from missing file {c.QLTConfigFilePath}");
                }

                var config = c.FromFile();

                featureTarget.Base = basePath;
                featureTarget.NumThreads = numThreads;
                featureTarget.WorkDirectory = workDirectory;
                featureTarget.Language = language;
                featureTarget.RunnerOS = runnerOS;
                featureTarget.CLIVersion = config.CodeQLCLI;
                featureTarget.STDLibIdent = config.CodeQLStandardLibraryIdent;
                featureTarget.ExtraCodeQLArgs = extraArgs;
                featureTarget.UseBundle = useBundle;

                featureTarget.Run();

            }, Globals.BasePathOption,
               Globals.AutomationTypeOption,
               numThreadsOption,
               workDirectoryOption,
               languageOption,
               runnerOSOption,
               extraCodeQLOptions,
               Globals.UseBundle
            );


            validateUnitTestsCommand.SetHandler((resultsDirectory, prettyPrint) =>
            {
                Log<TestCommandFeature>.G().LogInformation("Executing validate-unit-tests command...");

                new ValidateUnitTestsCommand()
                {
                    ResultsDirectory = resultsDirectory,
                    PrettyPrint = prettyPrint
                }.Run();


            }, resultsDirectoryOption, prettyPrintOption);
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
