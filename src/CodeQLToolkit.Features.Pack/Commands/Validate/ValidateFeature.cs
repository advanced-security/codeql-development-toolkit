using CodeQLToolkit.Shared.Logging;
using CodeQLToolkit.Shared.Utils;
using CodeQLToolkit.Shared.Feature;
using CodeQLToolkit.Features.Pack.Commands.Validate.Targets;
using System.CommandLine;
using System.IO;

namespace CodeQLToolkit.Features.Pack.Commands.Validate
{
    public class ValidateFeature : FeatureBase, IToolkitCommandFeature
    {
        public void Register(Command parentCommand)
        {
            Log<ValidateFeature>.G().LogInformation("Registering Validate submodule.");
            var validateCommand = new Command("validate", "Checking correctness of various files");
            parentCommand.Add(validateCommand);

            var validateSyntaxCommand = new Command("syntax", "Check if the extensions designators are in correct syntax");
            var extFilePath = new Option<string>("--ext", "path to the extensions' definition file in yml format") { IsRequired = true, Arity = ArgumentArity.ExactlyOne };
            validateSyntaxCommand.Add(extFilePath);

            var validateVersionCommand = new Command("version", "Check if the versions match up across the qlpack.yml files");
            var qlpackYmlFiles = new Option<string[]>("--yml", "qlpack.yml files to check against") { IsRequired = true, Arity = ArgumentArity.OneOrMore, AllowMultipleArgumentsPerToken = true };
            validateVersionCommand.Add(qlpackYmlFiles);

            validateCommand.Add(validateSyntaxCommand);
            validateCommand.Add(validateVersionCommand);

            validateSyntaxCommand.SetHandler((basePath, extFilePath) =>
            {
                var fileExists = File.Exists(extFilePath);
                var isYamlFile = Path.GetExtension(extFilePath) == ".yml" || Path.GetExtension(extFilePath) == ".yaml";
                if (!fileExists)
                    DieWithError($"{extFilePath} does not exist.");
                if (!isYamlFile)
                    DieWithError($"{extFilePath} is not a yaml file.");
                new ValidateSyntaxTarget()
                {
                    Base = basePath,
                    ExtFilePath = extFilePath
                }.Run();
            }, Globals.BasePathOption, extFilePath);
            validateVersionCommand.SetHandler(() => { throw new NotImplementedException(); });

            validateVersionCommand.SetHandler((basePath, qlpackYmlFiles) =>
            {
                foreach (var qlpackYmlFile in qlpackYmlFiles)
                {
                    var fileExists = File.Exists(qlpackYmlFile);
                    var isYamlFile = Path.GetExtension(qlpackYmlFile) == ".yml" || Path.GetExtension(qlpackYmlFile) == ".yaml";
                    if (!fileExists)
                        DieWithError($"{qlpackYmlFile} does not exist.");
                    if (!isYamlFile)
                        DieWithError($"{qlpackYmlFile} is not a yaml file.");
                }
                new ValidateVersionTarget()
                {
                    Base = basePath,
                    QlpackYmlFiles = qlpackYmlFiles
                }.Run();
            }, Globals.BasePathOption, qlpackYmlFiles
            );
        }
        public int Run()
        {
            throw new NotImplementedException();
        }
    }

}
