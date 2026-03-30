using CodeQLToolkit.Features.Query.Scaffolding.Targets;
using CodeQLToolkit.Shared.Feature;
using CodeQLToolkit.Shared.Options;
using CodeQLToolkit.Shared.Utils;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace CodeQLToolkit.Features.Query.Scaffolding
{
    public class QueryScaffoldFeature : FeatureBase, IToolkitScaffoldingFeature
    {
        public QueryScaffoldFeature()
        {
            FeatureName = "Query";
        }
        public override LanguageType[] SupportedLangauges => new LanguageType[] {
            LanguageType.C,
            LanguageType.CPP,
            LanguageType.CSHARP,
            LanguageType.GO,
            LanguageType.JAVA,
            LanguageType.JAVASCRIPT,
            LanguageType.PYTHON,
            LanguageType.RUBY
        };

        public void Register(Command parentCommand)
        {
            Log<QueryScaffoldFeature>.G().LogInformation("Registering scaffolding submodule.");

            var scaffoldCommand = new Command("generate", "Functions pertaining to generating query-related artifacts.");
            parentCommand.Add(scaffoldCommand);

            // add the types of things we can scaffold. 
            var newQueryCommand = new Command("new-query", "Generates a new query and associated tests. Optionally will generate a new query pack if required.");

            var createQueryPackOption = new Option<bool>("--create-query-pack", () => true, "Create a new query pack if none exists.");
            var overwriteExistingOption = new Option<bool>("--overwrite-existing", () => false, "Overwrite exiting files (if they exist).");

            var createTestsOption = new Option<bool>("--create-tests", () => true, "Create a new unit test for this query if it doesn't already exist.");
            var queryNameOption = new Option<string>("--query-name", "Name of the query. Note: Do not specify the `.ql` extension in naming your query.") { IsRequired = true };
            var queryLanguageOption = new Option<string>("--language", $"The language to generate a query for.") { IsRequired = true }
            .FromAmong(SupportedLangauges.Select(x => x.ToOptionString()).ToArray());
            var queryPackOption = new Option<string>("--pack", "The name of the query pack to place this query in.") { IsRequired = true };
            var queryPackScopeOption = new Option<string>("--scope", "The scope to use (optional)") { IsRequired = false };
            var queryKindOption = new Option<string>("--query-kind", () => "problem", "The kind of query to generate (problem or path-problem).") { IsRequired = false }
            .FromAmong(new[] { "problem", "path-problem" });

            newQueryCommand.Add(createQueryPackOption);
            newQueryCommand.Add(createTestsOption);
            newQueryCommand.Add(queryNameOption);
            newQueryCommand.Add(queryLanguageOption);
            newQueryCommand.Add(queryPackOption);
            newQueryCommand.Add(overwriteExistingOption);
            newQueryCommand.Add(queryPackScopeOption);
            newQueryCommand.Add(queryKindOption);

            scaffoldCommand.Add(newQueryCommand);

            {
                newQueryCommand.SetHandler((InvocationContext context) =>
                {
                    var createQueryPack = context.ParseResult.GetValueForOption(createQueryPackOption);
                    var createTests = context.ParseResult.GetValueForOption(createTestsOption);
                    var queryName = context.ParseResult.GetValueForOption(queryNameOption);
                    var queryLangauge = context.ParseResult.GetValueForOption(queryLanguageOption);
                    var queryPack = context.ParseResult.GetValueForOption(queryPackOption);
                    var basePath = context.ParseResult.GetValueForOption(Globals.BasePathOption);
                    var overwriteExisting = context.ParseResult.GetValueForOption(overwriteExistingOption);
                    var queryPackScope = context.ParseResult.GetValueForOption(queryPackScopeOption);
                    var queryKind = context.ParseResult.GetValueForOption(queryKindOption);

                    if (!IsSupportedLangauge(queryLangauge))
                    {
                        DieWithError($"Unsupported langauge `{queryLangauge}`");
                    }

                    new NewQueryScaffoldTarget()
                    {
                        Name = queryName,
                        Language = LanguageTypeHelper.LanguageTypeFromOptionString(queryLangauge),
                        Base = basePath,
                        QueryPack = queryPack,
                        QueryPackScope = queryPackScope,
                        CreateTests = createTests,
                        CreateQueryPack = createQueryPack,
                        OverwriteExisting = overwriteExisting,
                        FeatureName = FeatureName,
                        QueryKind = queryKind
                    }.Run();

                });
            }
        }

        public int Run()
        {
            return 0;
        }
    }
}