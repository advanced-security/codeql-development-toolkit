using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Types
{

    [AttributeUsage(AttributeTargets.Class |
                           AttributeTargets.Struct)
    ]
    public class AutomationTypeAttribute : Attribute
    {
        public AutomationType AutomationType { get; set; }
        public AutomationTypeAttribute(AutomationType automationType)
        {
            AutomationType = automationType;
        }
    }
}
