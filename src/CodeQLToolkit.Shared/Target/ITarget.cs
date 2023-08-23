using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Target
{
    public abstract class ITarget
    {
        public string Base { get; set; }
        public abstract void Run();

        public static ITarget ForCurrentOrchestration()
        {

        }

        public static ITarget ForOrchestration(Orchestration orchestration) 
        
    }
}
