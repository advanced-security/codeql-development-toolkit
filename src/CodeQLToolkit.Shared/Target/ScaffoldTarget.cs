using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CodeQLToolkit.Shared.Target
{
    public abstract class ScaffoldTarget : ITarget
    {
        public string Base { get; set; }
        public string Name { get; set; }
        public string Langauge { get; set; }

        public abstract void Run();
    }
}
