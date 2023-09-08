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

        public string CodeQLStandardLibraryIdent { 
            get  {
                return CodeQLStandardLibrary.Replace("/", "_");
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
            return JsonConvert.DeserializeObject<QLTConfig>(data);
        }

        public void ToFile()
        {
            var data = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(CodeQLConfigFilePath, data);
        }
    }
}
