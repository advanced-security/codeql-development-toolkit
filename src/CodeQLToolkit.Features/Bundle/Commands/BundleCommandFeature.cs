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
            parentCommand.Add(runCommand);

            //var checkQueryQueriesCommand = new Command("check-queries", "Checks the query metadata for the specified language.");

            //var languageOption = new Option<string>("--language", $"The language to run tests for.") { IsRequired = true }.FromAmong(SupportedLangauges.Select(x => x.ToOptionString()).ToArray());

            //checkQueryQueriesCommand.Add(languageOption);

            //runCommand.Add(checkQueryQueriesCommand);


            //checkQueryQueriesCommand.SetHandler((language, basePath, prettyPrint) =>
            //{
            //    Log<BundleCommandFeature>.G().LogInformation("Executing check-query-metadata command...");

            //    new CheckQueriesCommandTarget()
            //    {
            //        Base = basePath,
            //        Language = language,
            //        PrettyPrint = prettyPrint,
            //    }.Run();

            //}, languageOption, Globals.BasePathOption, prettyPrintOption);
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
