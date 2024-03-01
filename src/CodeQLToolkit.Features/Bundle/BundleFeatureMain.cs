using CodeQLToolkit.Features.Test.Commands;
using CodeQLToolkit.Features.Validation.Lifecycle;
using CodeQLToolkit.Features.Validation;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeQLToolkit.Features.Bundle.Commands;
using CodeQLToolkit.Features.Bundle.Lifecycle;

namespace CodeQLToolkit.Features.Bundle
{
    public class BundleFeatureMain : IToolkitFeature

    {
        readonly BundleCommandFeature commandFeature;
        readonly BundleLifecycleFeature lifecycleFeature;
        readonly static BundleFeatureMain instance;

        static BundleFeatureMain()
        {
            instance = new BundleFeatureMain();
        }

        private BundleFeatureMain()
        {
            commandFeature = new BundleCommandFeature();
            lifecycleFeature = new BundleLifecycleFeature();
        }
        public static BundleFeatureMain Instance { get { return instance; } }

        public void Register(Command parentCommand)
        {
            var bundleFeatureCommand = new Command("bundle", "Features related creation and usage of custom CodeQL bundles.");
            parentCommand.Add(bundleFeatureCommand);

            Log<BundleFeatureMain>.G().LogInformation("Registering command submodule.");
            commandFeature.Register(bundleFeatureCommand);

            Log<BundleFeatureMain>.G().LogInformation("Registering lifecycle submodule.");
            lifecycleFeature.Register(bundleFeatureCommand);

        }

        public int Run()
        {
            return 0;
        }
    }
}
