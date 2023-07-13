using CodeQLToolkit.Shared.Feature;
using CodeQLToolkit.Shared.Template;
using System.CommandLine;

namespace CodeQLToolkit.Features.Query.Scaffolding
{

    public class QueryScaffoldFeature : IToolkitScaffoldingFeature
    {
        internal static string SupportedLanguages = "c, cpp";
        static TemplateUtil templateUtil { get; } = new TemplateUtil()
        {
            TemplatePath = ""
        };

        public void Register(Command parentCommand)
        {
            var scaffoldCommand = new Command("generate", "Functions pertaining to generating query-related artifacts.");
            parentCommand.Add(scaffoldCommand);

            // add the types of things we can scaffold. 
            var newQueryCommand = new Command("new-query", "Generates a new query and associated tests. Optionally will generate a new query pack if required.");

            var createQueryPackOption = new Option<bool>("--create-query-pack", () => true, "Create a new query pack if none exists.");
            var createTestsOption = new Option<bool>("--create-tests", ()=> true, "Create a new unit test for this query if it doesn't already exist.");
            var queryNameOption = new Option<string>("--query-name", "Name of the query.") { IsRequired = true };
            var queryLanguageOption = new Option<string>("--language", $"The language to generate a query for. One of ({SupportedLanguages})") { IsRequired = true };

            newQueryCommand.Add(createQueryPackOption);
            newQueryCommand.Add(createTestsOption);
            newQueryCommand.Add(queryNameOption);
            newQueryCommand.Add(queryLanguageOption);


            scaffoldCommand.Add(newQueryCommand);
        }

        public int Run()
        {
            return 0;
        }
    }
}