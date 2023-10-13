using CodeQLToolkit.Features.Pack.Commands;
using CodeQLToolkit.Features.Pack.Commands.Validate;
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
        readonly ValidateFeature validateFeature;

        static PackFeatureMain()
        {
            instance = new PackFeatureMain();
        }

        private PackFeatureMain()
        {
            validateFeature = new ValidateFeature();
        }
        public static PackFeatureMain Instance { get { return instance; } }

        public void Register(Command parentCommand)
        {
            var packCommand = new Command("pack", "Features CodeQL pack management and publication.");
            parentCommand.Add(packCommand);

            Log<PackFeatureMain>.G().LogInformation("Registering scaffolding submodule.");
            validateFeature.Register(packCommand);
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
