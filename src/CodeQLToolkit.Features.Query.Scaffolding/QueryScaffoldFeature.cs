using CodeQLToolkit.Shared;
using CommandLine.Text;
using CommandLine;

namespace CodeQLToolkit.Features.Query.Scaffolding
{
    [Verb("generate", HelpText = "Specifies to generate items related to queries.")]

    public class QueryScaffoldingOptions : BaseOptions
    {
    }

    [Verb("new-query", HelpText = "Generates a new query.")]

    public class QueryScaffoldingNewQueryOptions : BaseOptions
    {
    }


    public class QueryScaffoldFeature : IToolkitScaffoldingFeature<QueryScaffoldingOptions>
    {

        public int Run(QueryScaffoldingOptions opts, string[] args)
        {
            string[] subArgs = args.Skip(1).ToArray();

            return Parser.Default.ParseArguments<QueryScaffoldingNewQueryOptions>(args)
             .MapResult(
                 // Register the query scaffolding 
                 // Note that within each subcommand we should dispatach to the correct orchestration layer. 
                 (QueryScaffoldingNewQueryOptions opts) => { 
                     Console.WriteLine("Creating a new query!");
                     Console.WriteLine($"Base Path={opts.GetBaseDirectory()}");
                     
                     return 0; 
                 
                 },
             errs => 1);
        }
    }
}