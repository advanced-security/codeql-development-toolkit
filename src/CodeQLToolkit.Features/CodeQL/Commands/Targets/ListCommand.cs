using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.CodeQL.Commands.Targets
{
    public class ListCommand : CommandTarget
    {    
        public override void Run()
        {
            Log<ListCommand>.G().LogInformation($"Running List Command");           
        }        
    }
}
