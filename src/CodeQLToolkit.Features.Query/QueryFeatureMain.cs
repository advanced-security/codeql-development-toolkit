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
        readonly static QueryFeatureMain instance;

        static QueryFeatureMain() { 
            instance = new QueryFeatureMain();
        }
        private QueryFeatureMain()
        {
            scaffoldFeature = new QueryScaffoldFeature();
        }

        public static QueryFeatureMain Instance { get { return instance; } }

        public int Run()
        {

            Console.WriteLine("[Query] RUNNING QUERY FEATURE");

            return 0;
        }

        public void Register(Command parentCommand)
        {
            Log<QueryFeatureMain>.G().LogInformation("Registering scaffolding submodule.");
            throw new NotImplementedException();
        }
    }
}