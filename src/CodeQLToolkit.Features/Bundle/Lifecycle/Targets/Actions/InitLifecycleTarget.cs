using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Bundle.Lifecycle.Targets.Actions
{
    [AutomationType(AutomationType.ACTIONS)]
    public class InitLifecycleTarget : BaseLifecycleTarget
    {

        public InitLifecycleTarget()
        {
            AutomationType = AutomationType.ACTIONS;
        }

        public override void Run()
        {
            Log<InitLifecycleTarget>.G().LogInformation("Running init command...");

            // temporarily disable the language resolution 
            var tmpLanguage = Language;
            Language = null;
           
            WriteTemplateIfOverwriteOrNotExists("install-qlt", Path.Combine(Base, ".github", "actions", "install-qlt", "action.yml"), "install-qlt action");
            WriteTemplateIfOverwriteOrNotExists("run-bundle-integration-tests", Path.Combine(Base, ".github", "workflows", $"run-bundle-integration-tests-{tmpLanguage}.yml"), $"Run CodeQL Unit Tests ({Language})", new
            {
                useRunner = UseRunner,
                language = tmpLanguage,
                devMode = DevMode,
            });

            Language = tmpLanguage;

            var message = @"------------------------------------------
Your repository now has the Bundle Creation and Integration Test Runner installed in `.github/workflows/`. Additionally, 
QLT has installed necessary actions for keeping your version of QLT and CodeQL current in `.github/actions/install-qlt`. 

Note that for integration testing to work, you MUST create a directory `integration-test` in the root of your repository. Please
consult the QLT documentation for details on how to structure this directory. 

In addition to using QLT to generate your files you can also directly edit this file to fine tune its settings. 
            
(Hint: If you'd like to regenerate your files, you can use the `--overwrite-existing` option to overwrite the files that are in place now.)";

            Log<InitLifecycleTarget>.G().LogInformation(message);
        }
    }
}
