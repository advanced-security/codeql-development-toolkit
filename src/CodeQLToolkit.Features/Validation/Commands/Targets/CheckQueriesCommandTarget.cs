using CodeQLToolkit.Features.Query.Commands.Targets;
using CodeQLToolkit.Features.Validation.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Validation.Commands.Targets
{
    public class CheckQueriesCommandTarget : CommandTarget
    {

        public bool PrettyPrint { get; set; }


        public override void Run()
        {
            Log<CheckQueriesCommandTarget>.G().LogInformation($"Validating query metadata for {Language}...");

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "codeql";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = Base;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = false;
                process.StartInfo.Arguments = $"query compile --format json  -n {Language}";
                process.Start();

                var output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    DieWithError($"Fatal error. Please check error output.");
                }

                var results = JsonConvert.DeserializeObject<List<CodeQLCliResponseModel>>(output);

                bool shouldFail = false;

                if (results != null)
                {
                    foreach (var r in results)
                    {
                        foreach (var message in r.messages)
                        {
                            if (message.severity == "WARNING" || message.severity == "ERROR")
                            {
                                shouldFail = true;

                                if (PrettyPrint)
                                {
                                    Console.WriteLine($"❌ [{message.severity}] {message.message}: {message.position.fileName}:{message.position.line},{message.position.column}-{message.position.endColumn},{message.position.endColumn}");
                                }
                                else
                                {
                                    Log<CheckQueriesCommandTarget>.G().LogWarning($"[{message.severity}] {message.message}: {message.position.fileName}:{message.position.line},{message.position.column}-{message.position.endColumn},{message.position.endColumn}");
                                }

                            }
                        }
                    }
                }

                if(shouldFail && !PrettyPrint )
                {
                    DieWithError("One or more validation errors found.");
                }



            }

        }
    }
}
