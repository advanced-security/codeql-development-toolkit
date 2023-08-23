using CodeQLToolkit.Shared.Feature;
using CodeQLToolkit.Shared.Target;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Types
{
    public class AutomationFeatureFinder
    {
        public static T FindTargetForAutomationType<T>(AutomationType automationType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {                
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    // restrict this to just things that are subtypes
                    // of targets 
                    if (!type.IsSubclassOf(typeof(ITarget)) || !type.IsSubclassOf(typeof(T)))
                    {
                       continue;
                    }

                    var attributes = type.GetCustomAttributes(typeof(AutomationTypeAttribute), true);

                    // match the first thing with the automation type attribute 
                    foreach (var attribute in attributes)
                    {
                        if (((AutomationTypeAttribute)attribute).AutomationType == automationType)
                        {
                            return (T)Activator.CreateInstance(type);
                        }
                    }
                }
            }
            throw new NotImplementedException();
        }
    }
}
