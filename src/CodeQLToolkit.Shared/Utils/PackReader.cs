using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace CodeQLToolkit.Shared.Utils
{
    public class CodeQLPack
    {
        public string Name { get; set; }
    }
    public class CodeQLPackReader
    {
        public static CodeQLPack read(string path)
        {
            var pack = new CodeQLPack();

            using (var reader = new StreamReader(path))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);

                var root = (YamlMappingNode)yaml.Documents[0].RootNode;

                foreach (var e in root.Children)
                {
                    if(e.Key.ToString() == "name")
                    {
                        pack.Name = e.Value.ToString();
                    }
                }



            }

            return pack;
        }
    }
}
