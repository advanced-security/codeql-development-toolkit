using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared
{
    public interface IToolkitFeature<T>
    {
        public int Run(T opts, string[] args);
    }
}
