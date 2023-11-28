using CodeQLToolkit.Features.Test.Commands;
using System.CommandLine;

namespace CodeQLToolkit.Features.Validation
{
    public class ValidationFeatureMain : IToolkitFeature

    {
        readonly ValidationCommandFeature commandFeature;
        readonly static ValidationFeatureMain instance;

        static ValidationFeatureMain()
        {
            instance = new ValidationFeatureMain();
        }

        private ValidationFeatureMain()
        {
            commandFeature = new ValidationCommandFeature();  
        }
        public static ValidationFeatureMain Instance { get { return instance; } }
        
        public void Register(Command parentCommand)
        {
            var validationCommand = new Command("validation", "Features related to the validation of CodeQL Development Repositories.");
            parentCommand.Add(validationCommand);            

            Log<ValidationFeatureMain>.G().LogInformation("Registering command submodule.");
            commandFeature.Register(validationCommand);

        }

        public int Run()
        {
            return 0;
        }
    }
}