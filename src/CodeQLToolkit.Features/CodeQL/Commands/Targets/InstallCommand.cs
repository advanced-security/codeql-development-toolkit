using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.CodeQL.Commands.Targets
{
    public class InstallCommand : CommandTarget
    {

        public override void Run()
        {
            Log<InstallCommand>.G().LogInformation($"Running Install command");
        }
    }
}
