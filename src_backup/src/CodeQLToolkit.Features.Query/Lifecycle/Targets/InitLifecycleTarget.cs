using CodeQLToolkit.Features.Query.Scaffolding.Targets;
using CodeQLToolkit.Shared.Feature;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Query.Lifecycle.Targets
{
    public class InitLifecycleTarget : ILifecycleTarget
    {
        public override void Run()
        {
            Log<InitLifecycleTarget>.G().LogInformation("Initializing new query development workspace...");

            Directory.CreateDirectory(Base);

            var workspaceLocation = Path.Combine(Base, "codeql-workspace.yml");

            WriteTemplateIfOverwriteOrNotExists("codeql-workspace", workspaceLocation, "CodeQL Workspace");
        }
    }
}
