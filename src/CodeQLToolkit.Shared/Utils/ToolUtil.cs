using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Utils
{
    public class ToolUtil
    {
        public static string ToolRoot => Path.Combine(Utils.FileUtils.GetExecutingDirectory().FullName, "tools");

        public static string ExecutableExtensionForPlatform
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return ".exe";
                }

                return "";
            }
        }

        public static string GetTool(string toolName)
        {
            return Path.Combine(ToolRoot, toolName + ExecutableExtensionForPlatform);
        }

        public static string GetCommand(string toolName)
        {
            return Path.Combine(toolName + ExecutableExtensionForPlatform);
        }
    }
}
