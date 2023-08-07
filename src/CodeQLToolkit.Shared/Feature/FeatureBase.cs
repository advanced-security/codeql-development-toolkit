using CodeQLToolkit.Shared.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Feature
{
    abstract public class FeatureBase
    {
        public string FeatureName { get; set; }

        public virtual string[] SupportedLangauges { get; } = { };
        public bool IsSupportedLangauge(string language)
        {
            return SupportedLangauges.Contains(language);
        }
        public string GetSupportedLangaugeString()
        {
            return String.Join(", ", SupportedLangauges);
        }

        public void DieWithError(string message)
        {
            Console.Error.WriteLine(message);
            Environment.Exit(1);
        }
    }
}
