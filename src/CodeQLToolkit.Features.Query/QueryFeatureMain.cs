using CodeQLToolkit.Shared;
using CodeQLToolkit.Features.Query.Scaffolding;
using CommandLine;

namespace CodeQLDevelopmentLifecycleToolkit.Features.Query
{

    [Verb("query", HelpText = "Functions for manipulating queries within your repository.")]
    public class QueryFeatureOptions : BaseOptions
    {
       

    }
    public class QueryFeatureMain : IToolkitFeature<QueryFeatureOptions>
    {
        readonly QueryScaffoldFeature scaffoldFeature;
        readonly static QueryFeatureMain instance;

        static QueryFeatureMain() { 
            instance = new QueryFeatureMain();
        }
        private QueryFeatureMain()
        {
            scaffoldFeature = new QueryScaffoldFeature();
        }

        public static QueryFeatureMain Instance { get { return instance; } }

        public int Run(QueryFeatureOptions opts, string[] args)
        {
            Console.WriteLine("[Query] RUNNING QUERY FEATURE");

            string[] subArgs = args.Skip(1).ToArray();

            return Parser.Default.ParseArguments<QueryScaffoldingOptions>(args)
             .MapResult(  
                 // Register the query scaffolding 
                 // Note that within each subcommand we should dispatach to the correct orchestration layer. 
                 (QueryScaffoldingOptions opts) => scaffoldFeature.Run(opts, subArgs),
             errs => 1);
        }
    }
}