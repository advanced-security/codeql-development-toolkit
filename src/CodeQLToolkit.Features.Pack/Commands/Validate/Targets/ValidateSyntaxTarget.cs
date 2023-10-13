using CodeQLToolkit.Shared.Logging;
using CodeQLToolkit.Shared.Utils;
using YamlDotNet.Serialization;

namespace CodeQLToolkit.Features.Pack.Commands.Validate.Targets
{
    public class ValidateSyntaxTarget : CommandTarget
    {
        public string ExtFilePath { get; set; }

        public override void Run()
        {
            Console.WriteLine("Hello world!");
        }
    }
}