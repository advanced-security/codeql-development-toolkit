using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Utils
{
    public class CodeQLPackConfiguration
    {
        public string Name { get; set; }
        public bool Bundle { get; set; }
        public bool Publish { get; set; }
        public bool ReferencesBundle { get; set; }

    }

    public class QLTConfig
    {
        public string CodeQLCLI { get; set; }
        public string CodeQLStandardLibrary { get; set; }
        public string CodeQLCLIBundle { get; set; }
        public string CodeQLConfiguration { get; set; }

        public CodeQLPackConfiguration[] CodeQLPackConfiguration { get; set; }

        public string CodeQLStandardLibraryIdent
        {
            get
            {
                if (CodeQLStandardLibrary != null)
                {
                    return CodeQLStandardLibrary.Replace("/", "_");
                }
                return CodeQLStandardLibrary;
            }
        }

        [JsonIgnore]
        public string CodeQLConfigurationPath => Path.Combine(Base, CodeQLConfiguration);

        [JsonIgnore]
        public string Base { get; set; }

        [JsonIgnore]
        public string QLTConfigFilePath => Path.Combine(Base, "qlt.conf.json");

        public QLTConfig FromFile()
        {
            var data = File.ReadAllText(QLTConfigFilePath);
            QLTConfig c = JsonConvert.DeserializeObject<QLTConfig>(data);
            c.Base = Base;
            return c;
        }

        public void ToFile()
        {
            var data = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(QLTConfigFilePath, data);
        }

        public static QLTConfig? LoadFromFile(string baseDir)
        {
            var config = new QLTConfig()
            {
                Base = baseDir
            };


            if (File.Exists(config.QLTConfigFilePath))
            {
                return config.FromFile();
            }

            return null;
        }
    }
}
