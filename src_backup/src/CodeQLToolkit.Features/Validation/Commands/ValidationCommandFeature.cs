using CodeQLToolkit.Shared.Utils;
using System.CommandLine;

namespace CodeQLToolkit.Features.Test.Commands
{
    public class ValidationCommandFeature : FeatureBase, IToolkitLifecycleFeature
    {
        public override LanguageType[] SupportedLangauges { get => new LanguageType[] { 
            LanguageType.C,
            LanguageType.CPP,
            LanguageType.CSHARP,
            LanguageType.JAVA,
            LanguageType.JAVASCRIPT,
            LanguageType.GO,
            LanguageType.RUBY,
            LanguageType.PYTHON            
        }; }

        public ValidationCommandFeature()
        {
            FeatureName = "Validation";
        }

        public void Register(Command parentCommand)
        {
            Log<ValidationCommandFeature>.G().LogInformation("Registering command submodule.");


            var runCommand = new Command("run", "Functions pertaining running validation commands.");
            parentCommand.Add(runCommand);

            var getMatrixTestCommand = new Command("check-metadsata", "Checks the query metadata for the specified queries.");

            var languageOption = new Option<string>("--language", $"The language to run tests for.") { IsRequired = true }.FromAmong(SupportedLangauges.Select(x => x.ToOptionString()).ToArray());
            var matrixOSVersion = new Option<string>("--os-version", () => "ubuntu-latest", "A comma-seperated list of operating systems to use. Example: `ubuntu-latest`.") { IsRequired = true };
            getMatrixTestCommand.Add(matrixOSVersion);
      
            runCommand.Add(getMatrixTestCommand);


            getMatrixTestCommand.SetHandler(() =>
            {
                Log<ValidationCommandFeature>.G().LogInformation("Executing validate-unit-tests command...");

                //new ValidateUnitTestsCommand()
                //{
                //    ResultsDirectory = resultsDirectory,
                //    PrettyPrint = prettyPrint
                //}.Run();


            });
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
