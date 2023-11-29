using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Validation.Lifecycle
{
    abstract internal class BaseLifecycleTarget : ILifecycleTarget
    {
        public string UseRunner { get; set; }

    }
}