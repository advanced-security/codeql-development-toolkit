using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace CodeQLToolkit.Shared.Logging
{
    public class Log<T>
    {
        public static readonly Log<T> instance;
        private ILogger _logger { get; set; }

        static Log()
        {
            instance = new Log<T>();
        }
        private Log()
        {
            _logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<T>();
        }

        public static ILogger G()
        {
            return instance._logger;            
        }
    }
}
