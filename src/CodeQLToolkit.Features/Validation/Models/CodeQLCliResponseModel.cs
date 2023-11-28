using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Validation.Models
{
    public class CodeQLCliResponseModel
    {
        public string query { get; set; }
        public string relativeName { get; set; }
        public bool success { get; set; }
        public Message[] messages { get; set; }
    }

    public class Message
    {
        public string severity { get; set; }
        public string message { get; set; }
        public Position position { get; set; }
    }

    public class Position
    {
        public string fileName { get; set; }
        public int line { get; set; }
        public int column { get; set; }
        public int endLine { get; set; }
        public int endColumn { get; set; }
    }

}
