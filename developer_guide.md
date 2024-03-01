# Developer Guide 

## Building external tools

External tools must be placed in the `tools` directory in the final distribution. During local development you can build your own binaries, in addition to placing them in the correct location manually. Binaries must be placed in:

```
codeql-development-lifecycle-toolkit/src/CodeQLToolkit.Core/bin/Debug/net6.0/tools 
```

Assuming you are running the `Debug` configuration locally. 

Note that we keep recent copies of tools (for local debugging purposes) in the `tools` directory in the root of this repo. These are made available just to make local development easier and should not be used for production or distribution purposes. 

**CodeQL Bundle**

```
./scripts/build_codeql_bundle_dist.ps1 -Version 0.2.0 -WorkDirectory dist -DestinationDirectory ./src/CodeQLToolkit.Core/bin/Debug/net6.0/tools
```



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