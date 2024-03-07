using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Utils
{
    public class QLTCustomizationPack
    {
        public string Name { get; set; }
        public bool Export { get; set; }
    }   

    public class QLTConfig
    {
        public string CodeQLCLI { get; set; }
        public string CodeQLStandardLibrary { get; set; }
        public string CodeQLCLIBundle { get; set; }

        public string CodeQLConfiguration { get; set; }

        public QLTCustomizationPack[] CustomizationPacks { get; set; }
        
        public string CodeQLStandardLibraryIdent { 
            get  {
                if (CodeQLStandardLibrary != null)
                {
                    return CodeQLStandardLibrary.Replace("/", "_");
                }
                return CodeQLStandardLibrary;
            } 
        }

        [JsonIgnore]
        public string CodeQLConfigurationPath { get { return Path.Combine(Base, CodeQLConfiguration); } }

        [JsonIgnore]
        public string Base { get; set; }

        [JsonIgnore]
        public string CodeQLConfigFilePath
        {
            get
            {
                return Path.Combine(Base, "qlt.conf.json");
            }
        }

        public QLTConfig FromFile() {
            var data = File.ReadAllText(CodeQLConfigFilePath);
            QLTConfig c = JsonConvert.DeserializeObject<QLTConfig>(data);


            c.Base = Base;
            return c;
        }

        public void ToFile()
        {
            var data = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(CodeQLConfigFilePath, data);
        }

        public static QLTConfig? LoadFromFile(string baseDir)
        {
            var config = new QLTConfig()
            {
                Base = baseDir
            };
            

            if (File.Exists(config.CodeQLConfigFilePath))
            {
                return config.FromFile();
            }

            return null;
        }
    }
}
