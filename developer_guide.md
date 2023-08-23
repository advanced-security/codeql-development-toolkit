# Developer Guide 

# Feature Details

## 

## Test Feature




# Mapping Targets to Different Automation Types

QLT is designed to work on a variety of different automation systems. For example, 
Actions and Jenkins. At runtime, QLT is able to dispatch to the correct modules via 
special attributes which can be annoated to special classes.

Currently, to achieve this a class implementing a `ITarget` interface should be annotated as thus:

```C#
namespace CodeQLToolkit.Features.Test.Lifecycle.Targets.Actions
{
    [AutomationType(AutomationType.ACTIONS)]
    public class InitLifecycleTarget : ILifecycleTarget
    {
        public override void Run()
        {
            Log<InitLifecycleTarget>.G().LogInformation("Running init command...");
        }
    }
}
```