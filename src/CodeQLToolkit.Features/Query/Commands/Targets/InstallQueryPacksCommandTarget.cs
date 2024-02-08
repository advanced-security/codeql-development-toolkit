using CodeQLToolkit.Shared.CodeQL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Query.Commands.Targets
{
    public class InstallQueryPacksCommandTarget : CommandTarget
    {
        public override void Run()
        {
            Log<InstallQueryPacksCommandTarget>.G().LogInformation("Finding all qlpacks...");

            // starting at the base path, find all qlpacks and install them
            string[] files = Directory.GetFiles(Base, "qlpack.yml", SearchOption.AllDirectories);

            Log<InstallQueryPacksCommandTarget>.G().LogInformation($"Got {files.Length} packs...");


            var installation = CodeQLInstallation.LoadFromConfig(Base);


            foreach ( string file in files )
            {
                Log<InstallQueryPacksCommandTarget>.G().LogInformation($"Installing qlpack {file}...");

                using(Process  process = new Process())
                {
                    process.StartInfo.FileName = installation.CodeQLToolBinary;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = false;
                    process.StartInfo.Arguments = $"pack install {file}";
                    process.Start();

                    process.WaitForExit();

                    if(process.ExitCode !=0)
                    {
                        DieWithError($"Failed to install query pack {file}.");
                    }
                }

            }

            Log<InstallQueryPacksCommandTarget>.G().LogInformation($"Installed {files.Length} packs.");

        }
    }
}
