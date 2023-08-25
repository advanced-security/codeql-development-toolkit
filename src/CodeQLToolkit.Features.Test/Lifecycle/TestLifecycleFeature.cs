using CodeQLToolkit.Features.Test.Lifecycle.Targets;
using CodeQLToolkit.Features.Test.Lifecycle.Targets.Actions;
using System.CommandLine;
using System.Reflection;

namespace CodeQLToolkit.Features.Test.Lifecycle
{
    public class TestLifecycleFeature : FeatureBase, IToolkitLifecycleFeature
    {
        public TestLifecycleFeature() 
        {
            FeatureName = "Test";
        }

        //DispatchCommandToOrchestrationTarget(Globals.OrchestrationType, this)

        public void Register(Command parentCommand)
        {
            Log<TestLifecycleFeature>.G().LogInformation("Registering lifecycle submodule.");

            var initCommand = new Command("init", "Initialize testing in this repository.");
            parentCommand.Add(initCommand);
            
            initCommand.SetHandler((basePath, automationType) =>
            {
                Log<TestLifecycleFeature>.G().LogInformation("Executing init command...");

                // dispatch at runtime to the correct automation type

                var featureTarget = AutomationFeatureFinder.FindTargetForAutomationType<BaseLifecycleTarget>(AutomationTypeHelper.AutomationTypeFromString(automationType));

                featureTarget.Base = basePath;

                featureTarget.Run();

            }, Globals.BasePathOption, Globals.AutomationTypeOption);

        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
