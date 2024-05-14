using CodeQLToolkit.Features.Pack.Commands;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Pack
{
    public class PackFeatureMain : IToolkitFeature
    {

        readonly static PackFeatureMain instance;
        readonly PackCommandFeature commandFeature;

        static PackFeatureMain()
        {
            instance = new PackFeatureMain();
        }

        private PackFeatureMain()
        {
            commandFeature = new PackCommandFeature();
        }
        public static PackFeatureMain Instance => instance;

        public void Register(Command parentCommand)
        {
            var packCommand = new Command("pack", "Features CodeQL pack management and publication.");
            parentCommand.Add(packCommand);

            Log<PackFeatureMain>.G().LogInformation("Registering scaffolding submodule.");
            commandFeature.Register(packCommand);
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
