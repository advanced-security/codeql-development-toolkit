using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Options
{
    public class Globals
    {
        public static Option<string> BasePathOption { get; } = new Option<string>("--base", () => {
            return Directory.GetCurrentDirectory();
        }, "The base path to find the query repository.");
    }
}
