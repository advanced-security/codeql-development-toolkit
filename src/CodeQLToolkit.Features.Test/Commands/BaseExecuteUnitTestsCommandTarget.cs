using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Test.Commands
{
    public abstract class BaseExecuteUnitTestsCommandTarget : CommandTarget
    {
        public int NumThreads { get; set; } 
        public string WorkDirectory { get; set; }
        public string RunnerOS { get; set; }
        public string CLIVersion { get; set; }
        public string STDLibIdent { get; set; }
    }
}

