using CodeQLToolkit.Features.Test.Commands;
using CodeQLToolkit.Features.Validation.Lifecycle;
using System.CommandLine;

namespace CodeQLToolkit.Features.Validation
{
    public class ValidationFeatureMain : IToolkitFeature

    {
        readonly ValidationCommandFeature commandFeature;
        readonly ValidationLifecycleFeature validationLifecycleFeature;
        readonly static ValidationFeatureMain instance;

        static ValidationFeatureMain()
        {
            instance = new ValidationFeatureMain();
        }

        private ValidationFeatureMain()
        {
            commandFeature = new ValidationCommandFeature();
            validationLifecycleFeature = new ValidationLifecycleFeature();
        }
        public static ValidationFeatureMain Instance => instance;

        public void Register(Command parentCommand)
        {
            var validationCommand = new Command("validation", "Features related to the validation of CodeQL Development Repositories.");
            parentCommand.Add(validationCommand);

            Log<ValidationFeatureMain>.G().LogInformation("Registering command submodule.");
            commandFeature.Register(validationCommand);

            Log<ValidationFeatureMain>.G().LogInformation("Registering lifecycle submodule.");
            validationLifecycleFeature.Register(validationCommand);

        }

        public int Run()
        {
            return 0;
        }
    }
}