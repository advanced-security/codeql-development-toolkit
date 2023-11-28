using CodeQLToolkit.Features.Query.Lifecycle.Targets;
using CodeQLToolkit.Shared.Feature;
using CodeQLToolkit.Shared.Options;
using CodeQLToolkit.Shared.Types;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Query.Lifecycle
{
    public class QueryLifecycleFeature : FeatureBase, IToolkitLifecycleFeature
    {
        public QueryLifecycleFeature()
        {
            FeatureName = "Query";
        }

        public void Register(Command parentCommand)
        {
            Log<QueryLifecycleFeature>.G().LogInformation("Registering lifecycle submodule.");

            var initCommand = new Command("init", "Initialize query support in this repository.");

            var overwriteExistingOption = new Option<bool>("--overwrite-existing", () => false, "Overwrite exiting files (if they exist).");

            initCommand.AddOption(overwriteExistingOption);

            parentCommand.Add(initCommand);

            initCommand.SetHandler((basePath, overwriteExisting) =>
            {
                Log<QueryLifecycleFeature>.G().LogInformation("Executing init command...");

               
                new InitLifecycleTarget()
                {
                    FeatureName = FeatureName,
                    Base = basePath,
                    OverwriteExisting = overwriteExisting

                }.Run();

                
            }, Globals.BasePathOption, overwriteExistingOption);

        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
