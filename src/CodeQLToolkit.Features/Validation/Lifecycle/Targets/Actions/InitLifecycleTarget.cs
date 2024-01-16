using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Validation.Lifecycle.Targets.Actions
{
    [AutomationType(AutomationType.ACTIONS)]
    internal class InitLifecycleTarget : BaseLifecycleTarget
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
           
            WriteTemplateIfOverwriteOrNotExists("validate-query-metadata", Path.Combine(Base, ".github", "workflows", $"validate-codeql-queries-{tmpLanguage}.yml"), $"Validate CodeQL Queries ({Language})", new
            {
                useRunner = UseRunner,
                language = tmpLanguage,
                devMode = DevMode
            });

            Language = tmpLanguage;

            var message = @"------------------------------------------
Your repository now has CodeQL Query Validation installed in `.github/workflows/`. Please ensure to initialize CodeQL 
testing before using this workflow with `qlt test init`. 

Note that, by default, your runner the `ubuntu-latest` runner. 

You can modify default runner by adjusting the `--use-runner` argument. 

In addition to using QLT to generate your files you can also directly edit this file to fine tune its settings. 
            
(Hint: If you'd like to regenerate your files, you can use the `--overwrite-existing` option to overwrite the files that are in place now.)";

            Log<InitLifecycleTarget>.G().LogInformation(message);
        }
    }
}
