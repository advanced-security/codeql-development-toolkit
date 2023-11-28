using CodeQLToolkit.Shared.Feature;
using System.CommandLine;
using CodeQLToolkit.Shared.Logging;
using Microsoft.Extensions.Logging;
using CodeQLToolkit.Features.CodeQL.Lifecycle;

namespace CodeQLToolkit.Features.CodeQL
{
        public class CodeQLFeatureMain : IToolkitFeature
    {
        readonly CodeQLLifecycleFeature lifecycleFeature;
        readonly static CodeQLFeatureMain instance;

        static CodeQLFeatureMain()
        {
            instance = new CodeQLFeatureMain();
        }

        private CodeQLFeatureMain()
        {
            lifecycleFeature = new CodeQLLifecycleFeature();
        }

        public static CodeQLFeatureMain Instance { get { return instance; } }

        public int Run()
        {

            return 0;
        }

        public void Register(Command parentCommand)
        {
            var queryCommand = new Command("codeql", "Use the features related to managing the version of CodeQL used by this repository.");
            parentCommand.Add(queryCommand);
            Log<CodeQLFeatureMain>.G().LogInformation("Registering scaffolding submodule.");
            lifecycleFeature.Register(queryCommand);
        }


    }
}