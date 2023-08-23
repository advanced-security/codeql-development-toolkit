using CodeQLToolkit.Shared.Types;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeQLToolkit.Shared.Types;
 
namespace CodeQLToolkit.Shared.Options
{
    public class Globals
    {
        public static string[] SupportedAutomationTypes { get => new string[] { "actions" }; }

        public static Option<string> BasePathOption { get; } = new Option<string>("--base", () => {
            return Directory.GetCurrentDirectory();
        }, "The base path to find the query repository.");

        public static Option<string> AutomationTypeOption { get; } = new Option<string>("--automation-type", () => {
            return "actions";
        }, "The base path to find the query repository.") { IsRequired = true }.FromAmong(SupportedAutomationTypes);
    }
}
