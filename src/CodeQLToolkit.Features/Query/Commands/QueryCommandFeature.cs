using CodeQLToolkit.Features.Query.Commands.Targets;
using CodeQLToolkit.Features.Query.Scaffolding;
using CodeQLToolkit.Shared.Feature;
using CodeQLToolkit.Shared.Options;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Query.Commands
{
    public class QueryCommandFeature : FeatureBase, IToolkitCommandFeature
    {
        public QueryCommandFeature()
        {
            FeatureName = "Query"; 
        }

        public void Register(Command parentCommand)
        {
            Log<QueryCommandFeature>.G().LogInformation("Registering command submodule.");

            var runCommand = new Command("run", "Functions pertaining to running query-related commands.");
            parentCommand.Add(runCommand);

            // a command that installs query packs
            var installPacksQueryCommand = new Command("install-packs", "Installs CodeQL packs within the repository.");

            runCommand.Add(installPacksQueryCommand);

            installPacksQueryCommand.SetHandler(
                (basePath, useBundle) => new InstallQueryPacksCommandTarget() { Base = basePath, UseBundle = useBundle}.Run(), Globals.BasePathOption, Globals.UseBundle);

        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
