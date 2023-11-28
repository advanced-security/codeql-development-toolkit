using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Test.Commands
{
    public abstract class BaseGetMatrixCommandTarget : CommandTarget
    {
        public string[] OSVersions { get; set; }    
    }
}
