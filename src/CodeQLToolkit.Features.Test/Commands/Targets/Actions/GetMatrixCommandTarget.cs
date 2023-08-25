using CodeQLToolkit.Shared.Utils;
using System.Text.Json;

namespace CodeQLToolkit.Features.Test.Commands.Targets.Actions
{
    [AutomationType(AutomationType.ACTIONS)]
    public class GetMatrixCommandTarget : BaseGetMatrixCommandTarget
    {
        public override void Run()
        {

            // based on the current configuration of the repository, generate a matrix 
            // for actions, it looks like this:

            // {
            //     "include": [
            //         {
            //             "codeql_cli": "2.12.6",
            //             "codeql_standard_library": "codeql-cli/v2.12.6",
            //             "codeql_cli_bundle": "codeql-bundle-20230403",
            //             "os": "ubuntu-latest",
            //             "codeql_standard_library_ident": "codeql-cli_v2.12.6"
            //         }
            //     ]
            // }

            // For now, we only support a single version but this is easy to extend. The options for runners are what create different matrix types.
            var c = new QLTConfig()
            {
                Base = Base
            };

            if (!File.Exists(c.CodeQLConfigFilePath))
            {
                ProcessUtils.DieWithError($"Cannot read values from missing file {c.CodeQLConfigFilePath}");
            }

            var config = c.FromFile();

            List<object> configs = new List<object>();

            foreach(var os in OSVersions)
            {
                Log<TestCommandFeature>.G().LogInformation($"Creating matrix for {os}");

                configs.Add(new
                {
                    codeql_cli = config.CodeQLCLI,
                    codeql_standard_library = config.CodeQLStandardLibrary,
                    codeql_cli_bundle = config.CodeQLCLIBundle,
                    os = os,
                    codeql_standard_library_ident = $"codeql-cli_v{config.CodeQLCLI}"
                });
            }

            var data = new
            {
                include = configs
            };

            var json = JsonSerializer.Serialize(data);
            Console.WriteLine(json);

        }
    }
}
