using CodeQLToolkit.Shared.Logging;
using CodeQLToolkit.Shared.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Semver;
using CodeQLToolkit.Features.Pack.Commands.Validate.Schemas;

namespace CodeQLToolkit.Features.Pack.Commands.Validate.Targets
{
    public class YamlParseException : Exception
    {
        public string message { get; set; }
        public YamlParseException(string failMessage)
        {
            message = failMessage;
        }
    }
    public class ValidateVersionTarget : CommandTarget
    {
        public string[] QlpackYmlFiles { get; set; }
        private static Deserializer YamlDeserializer = (Deserializer)new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

        public override void Run()
        {
            Console.WriteLine("Hello world! I got: ");
            foreach (string qlpackYmlFile in QlpackYmlFiles)
            {
                Console.WriteLine(qlpackYmlFile);
            }
            var range = SemVersionRange.Parse("^1.0.0");
        }
    }
}
