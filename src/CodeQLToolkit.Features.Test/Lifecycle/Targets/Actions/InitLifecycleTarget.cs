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

            WriteTemplateIfOverwriteOrNotExists("install-codeql", Path.Combine(Base, ".github", "actions", "install-codeql", "action.yml"), "install-codeql action");
            WriteTemplateIfOverwriteOrNotExists("install-qlt", Path.Combine(Base, ".github", "actions", "install-qlt", "action.yml"), "install-qlt action");
            WriteTemplateIfOverwriteOrNotExists("run-unit-tests", Path.Combine(Base, ".github", "workflows", $"run-codeql-unit-tests-{tmpLanguage}.yml"), $"Run CodeQL Unit Tests ({Language})", new
            {
                numThreads = NumThreads,
                useRunner = UseRunner,
                language = tmpLanguage
            });

            Language = tmpLanguage; 

            var message = @"------------------------------------------
Your repository now has the CodeQL Unit Test Runner installed in `.github/workflows/`. Additionally, 
QLT has installed necessary actions for keeping your version of QLT and CodeQL current in `.github/actions/install-qlt` and 
`.github/actions/install-codeql`. 

Note that, by default, your runner will use 4 threads and defaults to the `ubuntu-latest` runner. 

You can modify the number of threads used by using the `--num-threads` argument and you can select a different
runner with the `--use-runner` argument. 

In addition to using QLT to generate your files you can also directly edit this file to fine tune its settings. 
            
(Hint: If you'd like to regenerate your files, you can use the `--overwrite-existing` option to overwrite the files that are in place now.)";



        }
    }
}
