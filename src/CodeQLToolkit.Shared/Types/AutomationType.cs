using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Types
{
    public enum AutomationType
    {
        ACTIONS
    }

    public static class AutomationTypeMethods
    {
        public static AutomationType FromString(this AutomationType automationType, string value)
        {
            if (value.ToLower().Equals("actions"))
            {
                return AutomationType.ACTIONS;
            }

            throw new NotImplementedException();
        }
    }


    public class AutomationTypeHelper
    {
        public static AutomationType AutomationTypeFromString(string automationType)
        {
            return new AutomationType().FromString(automationType);
        }
    }
}
