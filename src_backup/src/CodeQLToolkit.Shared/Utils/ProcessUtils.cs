using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Utils
{
    public class ProcessUtils
    {
        public static void DieWithError(string message)
        {
            Console.Error.WriteLine(message);
            Environment.Exit(1);
        }
    }
}
