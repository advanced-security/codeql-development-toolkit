using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Feature
{
    public interface IToolkitFeature
    {
        public void Register(Command parentCommand);
        public int Run();
    }
}
