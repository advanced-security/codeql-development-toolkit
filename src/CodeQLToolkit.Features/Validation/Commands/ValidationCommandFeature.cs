using CodeQLToolkit.Features.Validation.Commands.Targets;
using CodeQLToolkit.Shared.Utils;
using System.CommandLine;

namespace CodeQLToolkit.Features.Test.Commands
{
    public class ValidationCommandFeature : FeatureBase, IToolkitLifecycleFeature
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

        public ValidationCommandFeature()
        {
            FeatureName = "Validation";
        }

        public void Register(Command parentCommand)
        {
            Log<ValidationCommandFeature>.G().LogInformation("Registering command submodule.");


            var runCommand = new Command("run", "Functions pertaining running validation commands.");
            parentCommand.Add(runCommand);

            var checkQueryQueriesCommand = new Command("check-queries", "Checks the query metadata for the specified language.");

            var languageOption = new Option<string>("--language", $"The language to run tests for.") { IsRequired = true }.FromAmong(SupportedLangauges.Select(x => x.ToOptionString()).ToArray());
            var prettyPrintOption = new Option<bool>("--pretty-print", () => false, "Pretty prints error output in a pretty compact format.") { IsRequired = true };

            checkQueryQueriesCommand.Add(languageOption);
            checkQueryQueriesCommand.Add(prettyPrintOption);

            runCommand.Add(checkQueryQueriesCommand);


            checkQueryQueriesCommand.SetHandler((language, basePath, prettyPrint, useBundle) =>
            {
                Log<ValidationCommandFeature>.G().LogInformation("Executing check-query-metadata command...");

                new CheckQueriesCommandTarget()
                {
                    Base = basePath,
                    Language = language,
                    PrettyPrint = prettyPrint,
                    UseBundle = useBundle
                }.Run();

            }, languageOption, Globals.BasePathOption, prettyPrintOption, Globals.UseBundle);
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
