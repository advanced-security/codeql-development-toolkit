using CodeQLToolkit.Shared.Logging;
using CodeQLToolkit.Shared.Utils;
using YamlDotNet.Serialization;
using Semver;

namespace CodeQLToolkit.Features.Pack.Commands.Validate.Targets
{
    public class ValidateVersionTarget : CommandTarget
    {
        public string[] QlpackYmlFiles { get; set; }
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
