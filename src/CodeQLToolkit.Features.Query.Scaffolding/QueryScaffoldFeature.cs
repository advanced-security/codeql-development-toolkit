using CodeQLToolkit.Features.Query.Scaffolding.Targets;
using CodeQLToolkit.Shared.Feature;
using CodeQLToolkit.Shared.Options;
using System.CommandLine;

namespace CodeQLToolkit.Features.Query.Scaffolding
{
    public class QueryScaffoldFeature : FeatureBase, IToolkitScaffoldingFeature 
    {
        public override string[] SupportedLangauges { get => new string[] { "c", "cpp" }; }

        public void Register(Command parentCommand)
        {
            var scaffoldCommand = new Command("generate", "Functions pertaining to generating query-related artifacts.");
            parentCommand.Add(scaffoldCommand);

            // add the types of things we can scaffold. 
            var newQueryCommand = new Command("new-query", "Generates a new query and associated tests. Optionally will generate a new query pack if required.");

            var createQueryPackOption = new Option<bool>("--create-query-pack", () => true, "Create a new query pack if none exists.");
            var createTestsOption = new Option<bool>("--create-tests", ()=> true, "Create a new unit test for this query if it doesn't already exist.");
            var queryNameOption = new Option<string>("--query-name", "Name of the query. Note: Do not specify the `.ql` extension in naming your query.") { IsRequired = true };
            var queryLanguageOption = new Option<string>("--language", $"The language to generate a query for.") { IsRequired = true}.FromAmong(SupportedLangauges);

            newQueryCommand.Add(createQueryPackOption);
            newQueryCommand.Add(createTestsOption);
            newQueryCommand.Add(queryNameOption);
            newQueryCommand.Add(queryLanguageOption);

            scaffoldCommand.Add(newQueryCommand);

            {
                newQueryCommand.SetHandler((createQueryPack, createTests, queryName, queryLangauge, basePath) =>
                {

                    if (!IsSupportedLangauge(queryLangauge))
                    {
                        DieWithError($"Unsupported langauge `{queryLangauge}`");
                    }

                    var target = new NewQueryScaffoldTarget()
                    {
                        Name = queryName,
                        Langauge = queryLangauge,
                        Base = basePath
                    }.Run();

                }, createQueryPackOption, createTestsOption, queryNameOption, queryLanguageOption, Globals.BasePathOption);
            }
        }

        public int Run()
        {
            return 0;
        }
    }
}