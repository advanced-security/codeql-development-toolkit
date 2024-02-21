using CodeQLToolkit.Shared.Utils;
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

        public bool UseBundle { get; set; }
        public abstract void Run();

        public void DieWithError(string message)
        {
            ProcessUtils.DieWithError(message);
        }
    }
}
