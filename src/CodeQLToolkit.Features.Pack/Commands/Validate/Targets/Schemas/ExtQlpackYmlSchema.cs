namespace CodeQLToolkit.Features.Pack.Commands.Validate.Schemas
{
    public class ExtQlpackYmlFileSchema : IYmlSchema
    {
        public bool Library { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public Dictionary<string, string> ExtensionTargets { get; set; }
        public List<string> DataExtensions { get; set; }

        override public string ToString()
        {
            string ExtensionTargetsString = "\n";
            if (ExtensionTargets is not null)
                foreach (KeyValuePair<string, string> pair in ExtensionTargets)
                    ExtensionTargetsString += $"{pair.Key}: {pair.Value}\n";
            
            string DataExtensionString = "\n";
            if (DataExtensions is not null)
                foreach (string DataExtension in DataExtensions)
                    DataExtensionString += $"{DataExtension}, ";

            return $@"Ext qlpack.yml file:
                Library: {Library},
                Name: {Name},
                Version: {Version},
                ExtensionTargets: {ExtensionTargetsString}
                DataExtensions: {DataExtensionString}";
        }
    }
}
