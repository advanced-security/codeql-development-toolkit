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
        // Suffix used to temporarily hide the sources of packs that are shipped inside
        // the custom bundle. See HideBundledPackSources for why this is necessary.
        private const string BundledPackSuffix = ".qlt-bundled";

        public override void Run()
        {
            Log<InstallQueryPacksCommandTarget>.G().LogInformation("Finding all qlpacks...");

            // starting at the base path, find all qlpacks and install them
            RestoreBundledPackSources(Directory.GetFiles(Base, "qlpack.yml" + BundledPackSuffix, SearchOption.AllDirectories));

            string[] files = Directory.GetFiles(Base, "qlpack.yml", SearchOption.AllDirectories);
            string[] allFiles = files;

            Log<InstallQueryPacksCommandTarget>.G().LogInformation($"Got {files.Length} packs...");


            var installation = CodeQLInstallation.LoadFromConfig(Base);


            installation.EnableCustomCodeQLBundles = UseBundle;

            //
            installation.IsInstalledOrDie();
            //


            // filter the packs that are part of a custom bundle if we are using bundles.
            string[] bundledPackFiles = Array.Empty<string>();

            if(UseBundle)
            {
                // load the config
                var config = QLTConfig.LoadFromFile(Base);

                Log<InstallQueryPacksCommandTarget>.G().LogInformation("In bundle mode so filtering bundled packs...");

                
                foreach (var pack in config.CodeQLPackConfiguration)
                {
                    Log<InstallQueryPacksCommandTarget>.G().LogInformation($"Pack {pack.Name} will NOT installed because it is part of the bundle...");
                }

                files = files.Where(f => 
                    // all things that are part of the customization pack must be excluded. 
                    // if it is exported is not relevant here.
                    !config.CodeQLPackConfiguration.Any(p => CodeQLPackReader.read(f).Name == p.Name && (p.Bundle==true || p.ReferencesBundle==true))
                ).ToArray();

                Log<InstallQueryPacksCommandTarget>.G().LogInformation($"Got {files.Length} packs after filtering...");

                foreach (var file in files)
                {
                    Log<InstallQueryPacksCommandTarget>.G().LogInformation($"Pack {CodeQLPackReader.read(file).Name} in {file} will installed because it is not part of the bundle...");
                }

                // Packs that were bundled are shipped inside the custom bundle with their
                // dependencies stripped, which is how the bundle tool avoids a cycle between
                // the standard library pack and the customization pack. Their sources are still
                // discoverable through the CodeQL workspace though, and workspace packs take
                // precedence over the ones in the distribution. Left in place they shadow the
                // bundled copies and reintroduce that cycle, so dependency resolution fails for
                // every pack that depends on the customized standard library.
                bundledPackFiles = allFiles.Where(f =>
                    config.CodeQLPackConfiguration.Any(p => CodeQLPackReader.read(f).Name == p.Name && p.Bundle == true)
                ).ToArray();
            }


            string failedPack = null;
            var hiddenPacks = new List<string>();

            try
            {
                hiddenPacks = HideBundledPackSources(bundledPackFiles);

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
                            // Do not exit here. DieWithError terminates the process, which would
                            // leave the hidden pack sources behind in the user's working tree.
                            failedPack = file;
                            break;
                        }
                    }

                }
            }
            finally
            {
                RestoreBundledPackSources(hiddenPacks.ToArray());
            }

            if (failedPack != null)
            {
                DieWithError($"Failed to install query pack {failedPack}.");
            }

            Log<InstallQueryPacksCommandTarget>.G().LogInformation($"Installed {files.Length} packs.");

        }

        // Moves the given 'qlpack.yml' files aside so that CodeQL's workspace discovery no longer
        // sees them, and returns the paths that were hidden so they can be restored afterwards.
        private static List<string> HideBundledPackSources(string[] packFiles)
        {
            var hidden = new List<string>();

            foreach (var packFile in packFiles)
            {
                var hiddenPath = packFile + BundledPackSuffix;

                Log<InstallQueryPacksCommandTarget>.G().LogInformation($"Temporarily hiding {packFile} because the pack is provided by the custom bundle...");

                File.Move(packFile, hiddenPath);
                hidden.Add(hiddenPath);
            }

            return hidden;
        }

        // Restores files previously moved aside by HideBundledPackSources. Also used on start-up to
        // recover from an interrupted run that never got to restore them.
        private static void RestoreBundledPackSources(string[] hiddenPaths)
        {
            foreach (var hiddenPath in hiddenPaths)
            {
                var packFile = hiddenPath.Substring(0, hiddenPath.Length - BundledPackSuffix.Length);

                if (File.Exists(hiddenPath) && !File.Exists(packFile))
                {
                    File.Move(hiddenPath, packFile);
                }
            }
        }
    }
}
