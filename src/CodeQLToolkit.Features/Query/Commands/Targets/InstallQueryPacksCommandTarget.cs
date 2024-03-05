using CodeQLToolkit.Shared.CodeQL;
using CodeQLToolkit.Shared.Utils;
using Microsoft.VisualBasic;
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


            installation.EnableCustomCodeQLBundles = UseBundle;

            //
            installation.IsInstalledOrDie();
            //


            // filter the packs that are part of a custom bundle if we are using bundles.
            if(UseBundle)
            {
                // load the config
                var config = QLTConfig.LoadFromFile(Base);

                Log<InstallQueryPacksCommandTarget>.G().LogInformation("In bundle mode so filtering bundled packs...");

                
                foreach (var pack in config.ExportedCustomizationPacks)
                {
                    Log<InstallQueryPacksCommandTarget>.G().LogInformation($"Pack {pack} will NOT installed because it is part of the bundle...");
                }

                files = files.Where(f => !config.ExportedCustomizationPacks.Any(p => CodeQLPackReader.read(f).Name == p)).ToArray();

                Log<InstallQueryPacksCommandTarget>.G().LogInformation($"Got {files.Length} packs after filtering...");

                foreach (var file in files)
                {
                    Log<InstallQueryPacksCommandTarget>.G().LogInformation($"Pack {CodeQLPackReader.read(file).Name} in {file} will installed because it is not part of the bundle...");
                }
            }


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
