using CodeQLToolkit.Features.Test.Lifecycle;
using CodeQLToolkit.Shared.Types;
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
        public override string[] SupportedLangauges { get => new string[] { "c", "cpp", "javascript", "go", "python", "ql", "java", "ruby" }; }

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
            var languageOption = new Option<string>("--language", $"The language to run tests for.") { IsRequired = true }.FromAmong(SupportedLangauges);
            var runnerOSOption = new Option<string>("--runner-os", $"Label for the operating system running these tests.") { IsRequired = true };
            var cliVersionOption = new Option<string>("--cli-version", $"The version of the cli running the tests.") { IsRequired = true };
            var stdLibIdentOption = new Option<string>("--stdlib-ident", $"A string identifying the standard library used.") { IsRequired = true };

            unitTestsCommand.Add(numThreadsOption); 
            unitTestsCommand.Add(workDirectoryOption);
            unitTestsCommand.Add(languageOption);
            unitTestsCommand.Add(runnerOSOption);
            unitTestsCommand.Add(cliVersionOption);
            unitTestsCommand.Add(stdLibIdentOption);

            // a command validates the tests 
            var validateUnitTestsCommand = new Command("validate-unit-tests", "Validates a unit test run in a fashion suitable for use in CI/CD systems.");

            var resultsDirectoryOption = new Option<string>("--results-directory", "Where to find the intermediate execution output files.") { IsRequired = true };

            validateUnitTestsCommand.Add(resultsDirectoryOption);

            runCommand.Add(getMatrixTestCommand);
            runCommand.Add(unitTestsCommand);
            runCommand.Add(validateUnitTestsCommand);

            getMatrixTestCommand.SetHandler((basePath, automationType, osVersions) => {

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


            unitTestsCommand.SetHandler((basePath, automationType, numThreads, workDirectory, language, runnerOS, cliVersion, stdLibIdent) => {

                Log<TestCommandFeature>.G().LogInformation("Executing execute-unit-tests command...");

                // dispatch at runtime to the correct automation type
                var featureTarget = AutomationFeatureFinder
                    .FindTargetForAutomationType<BaseExecuteUnitTestsCommandTarget>(
                    AutomationTypeHelper.AutomationTypeFromString(automationType)
                    );

                featureTarget.Base = basePath;
                featureTarget.NumThreads = numThreads;
                featureTarget.WorkDirectory = workDirectory;
                featureTarget.Language = language;
                featureTarget.RunnerOS = runnerOS;
                featureTarget.CLIVersion = cliVersion;
                featureTarget.STDLibIdent = stdLibIdent;

                featureTarget.Run();

            }, Globals.BasePathOption, Globals.AutomationTypeOption, numThreadsOption, workDirectoryOption, languageOption, runnerOSOption, cliVersionOption, stdLibIdentOption);
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
