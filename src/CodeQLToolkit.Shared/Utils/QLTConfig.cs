using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Utils
{
    public class QLTConfig
    {
        public string CodeQLCLI { get; set; }
        public string CodeQLStandardLibrary { get; set; }
        public string CodeQLCLIBundle { get; set; }

        public bool EnableCustomCodeQLBundles { get; set; }
        
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
    }
}
