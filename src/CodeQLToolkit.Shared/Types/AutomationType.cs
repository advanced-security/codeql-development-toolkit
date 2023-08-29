using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Types
{
    public enum AutomationType
    {
        ANY,
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

        public static string ToDirectory(this AutomationType automationType)
        {
            if(automationType == AutomationType.ANY)
            {
                return "Any";
            }

            if(automationType == AutomationType.ACTIONS)
            {
                return "Actions";
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

        public static string AutomationTypeToString(AutomationType automationType)
        {
            return automationType.ToDirectory();
        }
    }
}
