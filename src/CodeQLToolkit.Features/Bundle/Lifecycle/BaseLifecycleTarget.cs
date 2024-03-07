using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Bundle.Lifecycle
{
    abstract public class BaseLifecycleTarget : ILifecycleTarget
    {
        public string UseRunner { get; set; }   

    }
}
