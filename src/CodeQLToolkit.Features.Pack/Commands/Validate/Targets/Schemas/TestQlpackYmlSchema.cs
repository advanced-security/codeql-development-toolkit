namespace CodeQLToolkit.Features.Pack.Commands.Validate.Schemas
{
    public class TestQlpackYmlFileSchema : IYmlSchema
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Extractor { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }

        override public string ToString()
        {
            string DependenciesString = "\n";
            if (Dependencies is not null)
                foreach (KeyValuePair<string, string> pair in Dependencies)
                    DependenciesString += $"{pair.Key}: {pair.Value}\n";

            return $@"Test qlpack.yml file:
                Name: {Name},
                Version: {Version},
                Extractor: {Extractor},
                Dependencies: {DependenciesString}";
        }
    }
}