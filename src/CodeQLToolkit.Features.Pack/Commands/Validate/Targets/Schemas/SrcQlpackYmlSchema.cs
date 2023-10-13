using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeQLToolkit.Features.Pack.Commands.Validate.Schemas
{
    public class SrcQlpackYmlSchema : IYmlSchema
    {
        public bool Library { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Suites { get; set; }
        public string Extractor { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }

        [YamlMember(Alias = "default-suite-file", ApplyNamingConventions = false)]
        public string DefaultSuiteFile { get; set; }

        public override string ToString()
        {
            string DependenciesString = "\n";
            if (Dependencies is not null)
                foreach (KeyValuePair<string, string> pair in Dependencies)
                    DependenciesString += $"{pair.Key}: {pair.Value}\n";

            return $@"Src qlpack.yml file:
                Library: {Library},
                Name: {Name},
                Version: {Version},
                Suites: {Suites},
                Extractor: {Extractor},
                Dependencies: {DependenciesString},
                DefaultSuiteFile: {DefaultSuiteFile}";
        }
    }
}