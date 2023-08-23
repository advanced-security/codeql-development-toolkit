using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Test.Lifecycle.Targets.Actions
{
    [AutomationType(AutomationType.ACTIONS)]
    public class InitLifecycleTarget : BaseLifecycleTarget
    {
        public override void Run()
        {
            Log<InitLifecycleTarget>.G().LogInformation("Running init command...");
        }
    }
}
