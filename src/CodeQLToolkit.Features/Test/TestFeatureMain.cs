using CodeQLToolkit.Features.Test.Commands;
using CodeQLToolkit.Features.Test.Lifecycle;
using CodeQLToolkit.Shared.Feature;
using System.CommandLine;

namespace CodeQLToolkit.Features.Test
{ 
    public class TestFeatureMain : IToolkitFeature

    {
        readonly TestLifecycleFeature lifecycleFeature;
        readonly TestCommandFeature commandFeature;
        readonly static TestFeatureMain instance;

        static TestFeatureMain()
        {
            instance = new TestFeatureMain();
        }

        private TestFeatureMain()
        {
            lifecycleFeature = new TestLifecycleFeature();
            commandFeature = new TestCommandFeature();  
        }
        public static TestFeatureMain Instance { get { return instance; } }
        
        public void Register(Command parentCommand)
        {
            var testCommand = new Command("test", "Features related to the running and processing of CodeQL Unit Tests.");
            parentCommand.Add(testCommand);

            Log<TestFeatureMain>.G().LogInformation("Registering scaffolding submodule.");
            lifecycleFeature.Register(testCommand);

            Log<TestFeatureMain>.G().LogInformation("Registering command submodule.");
            commandFeature.Register(testCommand);

        }

        public int Run()
        {
            return 0;
        }
    }
}