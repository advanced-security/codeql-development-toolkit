using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Test.Lifecycle.Models
{
    public class TestReport
    {
        public string RunnerOS { get; set; }
        public string CLIVersion { get; set; }
        public string STDLibIdent { get; set; }

        public string Language { get; set; }

        public int Slice { get; set; }
        public int NumSlices { get; set; }


        public string FileName { 
            
            get {

                var savePath = $"test_report_{RunnerOS}_{CLIVersion}_{STDLibIdent}_slice_{Slice}_of_{NumSlices}.json";

                return savePath;
            }
        }

        
    }
}
