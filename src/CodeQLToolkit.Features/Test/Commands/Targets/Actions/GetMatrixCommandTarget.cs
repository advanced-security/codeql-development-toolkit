﻿using CodeQLToolkit.Shared.Utils;
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

            if (!File.Exists(c.QLTConfigFilePath))
            {
                ProcessUtils.DieWithError($"Cannot read values from missing file {c.QLTConfigFilePath}");
            }

            var config = c.FromFile();

            List<object> configs = new List<object>();

            foreach (var os in OSVersions)
            {
                Log<TestCommandFeature>.G().LogInformation($"Creating matrix for {os}");

                configs.Add(new
                {
                    codeql_cli = config.CodeQLCLI,
                    codeql_standard_library = config.CodeQLStandardLibrary,
                    codeql_cli_bundle = config.CodeQLCLIBundle,
                    os = os,
                    codeql_standard_library_ident = config.CodeQLStandardLibraryIdent
                });
            }

            var data = new
            {
                include = configs
            };

            var json = JsonSerializer.Serialize(data);

            var matrixVariable = $"matrix={json}";

            string envFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");

            Log<TestCommandFeature>.G().LogInformation($"Writing matrix output {matrixVariable} to {envFile}");

            File.AppendAllText(envFile, matrixVariable);

            Log<TestCommandFeature>.G().LogInformation($"Done.");

        }
    }
}
