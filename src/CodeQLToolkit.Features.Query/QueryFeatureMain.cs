using CodeQLToolkit.Features.Query.Commands;
using CodeQLToolkit.Features.Query.Scaffolding;
using CodeQLToolkit.Shared.Feature;
using CodeQLToolkit.Shared.Logging;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace CodeQLDevelopmentLifecycleToolkit.Features.Query
{

    public class QueryFeatureMain : IToolkitFeature
    {
        readonly QueryScaffoldFeature scaffoldFeature;
        readonly QueryCommandFeature commandFeature;

        readonly static QueryFeatureMain instance;

        static QueryFeatureMain() { 
            instance = new QueryFeatureMain();
        }
        private QueryFeatureMain()
        {
            scaffoldFeature = new QueryScaffoldFeature();
            commandFeature = new QueryCommandFeature();
        }

        public static QueryFeatureMain Instance { get { return instance; } }

        public int Run()
        {

            Console.WriteLine("[Query] RUNNING QUERY FEATURE");

            return 0;
        }

        public void Register(Command parentCommand)
        {
            var queryCommand = new Command("query", "Use the features related to query creation and execution.");
            parentCommand.Add(queryCommand);

            scaffoldFeature.Register(queryCommand);
            commandFeature.Register(queryCommand);
            
        }
    }
}