using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Target
{
    public abstract class CommandTarget : ITarget
    {
        public string Language { get; set; }
    }
}
