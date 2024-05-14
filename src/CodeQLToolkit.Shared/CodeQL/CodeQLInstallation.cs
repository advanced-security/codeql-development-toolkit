using System.Net;
using CodeQLToolkit.Shared.Utils;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using CodeQLToolkit.Shared.Logging;
using LibGit2Sharp;
using System.IO.Compression;
using System.Diagnostics;
using System;

namespace CodeQLToolkit.Shared.CodeQL
{
    public class CodeQLInstallation
    {
        public string CLIVersion { get; set; }
        public string StandardLibraryVersion { get; set; }

        public string CodeScanningConfig { get; set; }
        public string QLTConfigFilePath { get; private set; }

        public string CLIBundle { get; set; }
        public string StandardLibraryIdent { get; set; }
        public bool EnableCustomCodeQLBundles { get; set; }
        public CodeQLPackConfiguration[] CodeQLPackConfiguration { get; set; }
        public bool QuickBundle { get; set; }
        public string Base { get; set; }

        public static CodeQLInstallation LoadFromConfig(string Base)
        {
            var c = new QLTConfig()
            {
                Base = Base
            };

            return LoadFromConfig(c.FromFile());
        }

        public static CodeQLInstallation LoadFromConfig(QLTConfig c)
        {
            var config = c.FromFile();

            return new CodeQLInstallation
            {
                CLIVersion = config.CodeQLCLI,
                CLIBundle = config.CodeQLCLIBundle,
                StandardLibraryIdent = config.CodeQLStandardLibraryIdent,
                StandardLibraryVersion = config.CodeQLStandardLibrary,
                CodeQLPackConfiguration = config.CodeQLPackConfiguration,
                Base = config.Base,
                CodeScanningConfig = config.CodeQLConfiguration,
                QLTConfigFilePath = config.QLTConfigFilePath
            };
        }

        public void LogPacksToBeBuilt()
        {
            if (CodeQLPackConfiguration != null)
            {
                foreach (var p in CodeQLPackConfiguration)
                {
                    Log<CodeQLInstallation>.G().LogInformation($"Pack: {p}");
                }
            }
        }

        public ArtifactKind Kind
        {
            get
            {
                if (EnableCustomCodeQLBundles)
                {
                    return ArtifactKind.CUSTOM_BUNDLE;
                }

                // TODO - here we only support the kind 
                // we package together however this can be extended 
                // to support the precompiled packages. 

                return ArtifactKind.PACKAGE;
            }
        }

        public string PlatformID
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return "win64";
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return "linux64";
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return "osx64";
                }

                throw new Exception("Unknown platform.");
            }
        }

        public string PlatformExtension => "zip";


        public void Install()
        {
            // each time we download the file; however, 
            // .qlt/repo/packages/ident
            // .qlt/repo/custom-bundle/ident
            // .qlt/repo/bundle/ident

            // https://github.com/github/codeql-cli-binaries/releases/download/v2.16.0/codeql-linux64.zip

            // workout a destination directory for this 

            if (Kind == ArtifactKind.CUSTOM_BUNDLE)
            {
                CustomBundleInstall();
            }
            else if (Kind == ArtifactKind.BUNDLE)
            {
                BundleInstall();
            }
            else
            {
                PackageInstall();
            }
        }

        public string CustomBundleOutputBundleCurrentPlatform => Path.Combine(CustomBundleOutputDirectory, $"codeql-bundle-{PlatformID}.tar.gz");
        public string CustomBundleOutputBundleLinux => Path.Combine(CustomBundleOutputDirectory, "codeql-bundle-linux64.tar.gz");
        public string CustomBundleOutputBundleWindows => Path.Combine(CustomBundleOutputDirectory, "codeql-bundle-win64.tar.gz");
        public string CustomBundleOutputBundleOSX => Path.Combine(CustomBundleOutputDirectory, "codeql-bundle-osx64.tar.gz");

        public string CustomBundleOutputDirectory => Path.Combine(InstallationDirectory, "out");

        public string StdLibDirectory => Path.Combine(InstallationDirectory, "codeql-stdlib");
        public string CodeQLDirectory => Path.Combine(InstallationDirectory, "codeql");


        private void PackageInstall()
        {
            Log<CodeQLInstallation>.G().LogInformation($"Begin Installation ");
            Log<CodeQLInstallation>.G().LogInformation($"Requested CLI Version {CLIVersion}, Standard Library Ident: {StandardLibraryIdent}");

            Log<CodeQLInstallation>.G().LogInformation($"Create installation directory {InstallationDirectory}");
            Directory.CreateDirectory(InstallationDirectory);

            Log<CodeQLInstallation>.G().LogInformation($"Download CodeQL CLI...");

            var downloadFile = $"codeql-{PlatformID}.{PlatformExtension}";

            // first, download the cli. 
            using (var client = new WebClient())
            {
                string uri = $"https://github.com/github/codeql-cli-binaries/releases/download/v{CLIVersion}/{downloadFile}";
                Log<CodeQLInstallation>.G().LogInformation($"Remote URL: {uri}...");

                client.DownloadFile(uri, Path.Combine(InstallationDirectory, downloadFile));
            }

            // unpack 
            Log<CodeQLInstallation>.G().LogInformation($"Unpacking distribution...");
            ZipFile.ExtractToDirectory(Path.Combine(InstallationDirectory, downloadFile), InstallationDirectory);
            Log<CodeQLInstallation>.G().LogInformation($"Done.");

            Log<CodeQLInstallation>.G().LogInformation($"Checkout standard library into.. {StdLibDirectory}");

            var repoPath = Repository.Clone("https://github.com/github/codeql.git", StdLibDirectory);


            Log<CodeQLInstallation>.G().LogInformation($"Getting standard library version.. {StandardLibraryVersion}");

            using (var repo = new Repository(repoPath))
            {
                var tag = repo.Tags[$"refs/tags/{StandardLibraryVersion}"];

                if (tag == null)
                {
                    Log<CodeQLInstallation>.G().LogInformation($"Unknown standard library version: {StandardLibraryVersion}");
                    throw new Exception($"Unknown standard library version: {StandardLibraryVersion}");
                }

                Branch b = Commands.Checkout(repo, $"refs/tags/{StandardLibraryVersion}");
            }
        }

        private void CustomBundleInstall()
        {
            Log<CodeQLInstallation>.G().LogInformation($"Begin Installation");
            Log<CodeQLInstallation>.G().LogInformation($"Requested Base Bundle Version {CLIBundle}");
            Log<CodeQLInstallation>.G().LogInformation($"Create installation directory {InstallationDirectory}");

            if (!Directory.Exists(InstallationDirectory))
            {
                Directory.CreateDirectory(InstallationDirectory);
            }

            // Download the platform-independent bundle.
            Log<CodeQLInstallation>.G().LogInformation($"Downloading CodeQL base bundle...");

            var downloadFile = $"codeql-bundle.tar.gz";
            var customBundlePath = Path.Combine(InstallationDirectory, downloadFile);

            Log<CodeQLInstallation>.G().LogInformation($"Checking if existing source bundle {downloadFile} is present...");

            if (File.Exists(customBundlePath))
            {
                Log<CodeQLInstallation>.G().LogInformation($"Bundle exists, will skip download");
            }
            else
            {
                using var client = new WebClient();
                string uri = $"https://github.com/github/codeql-action/releases/download/{CLIBundle}/{downloadFile}";
                Log<CodeQLInstallation>.G().LogInformation($"Downloading platform-independent bundle from remote URL: {uri}...");
                client.DownloadFile(uri, customBundlePath);
            }

            // Create the output directory that will contain the custom bundles.
            // If it already exists, delete it and recreate it.
            Log<CodeQLInstallation>.G().LogInformation($"Checking for custom bundle output directory...");

            if (Directory.Exists(CustomBundleOutputDirectory))
            {
                Log<CodeQLInstallation>.G().LogInformation($"Exists. Removing...");
                Directory.Delete(CustomBundleOutputDirectory, true);
                Directory.CreateDirectory(CustomBundleOutputDirectory);
            }
            else
            {
                Directory.CreateDirectory(CustomBundleOutputDirectory);
            }

            var workingDirectory = Path.GetFullPath(Base);

            if (CodeQLPackConfiguration == null || CodeQLPackConfiguration.Length == 0)
            {
                throw new Exception("No packs are set to be exported. Please add at least one pack to export in your `qlt.conf.json` file under the property `ExportedCustomizationPacks`.");
            }

            Log<CodeQLInstallation>.G().LogInformation($"Building custom bundles. This may take a while...");

            var packsToExport = CodeQLPackConfiguration.Where(p => p.Bundle == true).Select(p => p.Name).ToArray();
            var packs = string.Join(" ", packsToExport);

            // Run the bundling tool to create the platform-specific custom bundles from the platform-independent bundle
            var bundleArgs = $"--log DEBUG -a qlt.conf.json -p win64 -p osx64 -p linux64 -b {customBundlePath} -o {CustomBundleOutputDirectory} -w {workingDirectory} {packs}";

            if (QuickBundle)
            {
                Log<CodeQLInstallation>.G().LogInformation($"Note: Quick Bundles enabled and pre-compilation will be disabled...");
                bundleArgs = $"-nc {bundleArgs}";
            }

            if (CodeScanningConfig != null && CodeScanningConfig.Length > 0)
            {
                Log<CodeQLInstallation>.G().LogInformation($"Note: Attempting to include default code scanning configuration ...");

                if (File.Exists(Path.Combine(Base, CodeScanningConfig)))
                {
                    bundleArgs = $"-c \"{Path.Combine(Base, CodeScanningConfig)}\" {bundleArgs}";
                }
            }

            Log<CodeQLInstallation>.G().LogInformation($"Executing Bundle Tool with Working Directory: `{workingDirectory}`");
            Log<CodeQLInstallation>.G().LogInformation($"Executing Bundle Tool with Arguments: `{bundleArgs}`");

            using (Process process = new Process())
            {
                process.StartInfo.FileName = ToolUtil.GetTool("codeql_bundle");
                process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;
                if (QuickBundle)
                {
                    Log<CodeQLInstallation>.G().LogInformation($"Note: Quick Bundles enabled and pre-compilation will be disabled...");
                    process.StartInfo.Arguments = bundleArgs;
                }
                else
                {
                    process.StartInfo.Arguments = bundleArgs;
                }

                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Log<CodeQLInstallation>.G().LogInformation($"Failure running bundle command.");
                    throw new Exception("Failure running bundle command.");
                }
            }

            // Expand the correct platform-specific archive that is generated by the bundle command to `InstallationDirectory`
            Log<CodeQLInstallation>.G().LogInformation($"Checking for existence of {CodeQLDirectory}");

            if (Directory.Exists(CodeQLDirectory))
            {
                Log<CodeQLInstallation>.G().LogInformation($"Exists. Will delete {CodeQLDirectory}");
                Directory.Delete(CodeQLDirectory, true);
            }

            Log<CodeQLInstallation>.G().LogInformation($"Extracting bundle to {InstallationDirectory}...");

            using (Process process = new Process())
            {
                process.StartInfo.FileName = ToolUtil.GetCommand("tar");
                process.StartInfo.WorkingDirectory = InstallationDirectory;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;
                process.StartInfo.Arguments = $"-zxf {CustomBundleOutputBundleCurrentPlatform} ";

                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Log<CodeQLInstallation>.G().LogInformation($"Failure running extract bundle command.");
                    throw new Exception("Failure running extract bundle command.");
                }
            }

            Log<CodeQLInstallation>.G().LogInformation($"Done.");
        }

        private void BundleInstall()
        {
            throw new NotImplementedException();
        }


        private string GetIdentForPackage(ArtifactKind k)
        {
            if (k == ArtifactKind.PACKAGE)
            {
                var ident = String.Join("", "codeql-cli-" + CLIVersion, "#standard-library-ident-", StandardLibraryIdent);
                return StringUtils.CreateMD5(FileUtils.SanitizeFilename(ident)).ToLower();
            }

            if (k == ArtifactKind.CUSTOM_BUNDLE)
            {
                var ident = String.Join("", "codeql-bundle-" + CLIBundle);
                return StringUtils.CreateMD5(FileUtils.SanitizeFilename(ident)).ToLower();
            }

            throw new NotImplementedException();
        }

        public bool IsInstalledOrDie()
        {
            var installed = IsInstalled();

            if (!installed)
            {
                ProcessUtils.DieWithError("CodeQL not installed. Please run `qlt codeql run install --help` for more info.");
            }

            return true;
        }

        public bool IsInstalled()
        {
            Log<CodeQLInstallation>.G().LogInformation($"Checking if codeql is installed...");

            if (EnableCustomCodeQLBundles)
            {
                Log<CodeQLInstallation>.G().LogInformation($"Requested Custom Bundle Based on Bundle {CLIBundle}");
            }
            else
            {
                Log<CodeQLInstallation>.G().LogInformation($"Requested CLI Version {CLIVersion} with Standard Library Ident: {StandardLibraryIdent}");
            }

            Log<CodeQLInstallation>.G().LogInformation($"Checking for existance of required directories...");

            if (!Directory.Exists(InstallationDirectory))
            {
                Log<CodeQLInstallation>.G().LogInformation($"Installation directory {InstallationDirectory} is missing.");
                return false;
            }

            if (!Directory.Exists(CodeQLDirectory))
            {
                Log<CodeQLInstallation>.G().LogInformation($"CodeQL Directory Missing: {CodeQLDirectory}");
                return false;
            }

            // custom bundles don't have a standard library directory. 
            if (!EnableCustomCodeQLBundles)
            {
                if (!Directory.Exists(StdLibDirectory))
                {
                    Log<CodeQLInstallation>.G().LogInformation($"Standard Library Directory Missing: {StdLibDirectory}");
                    return false;
                }

            }

            return true;
        }

        public string InstallationDirectory => GetInstallationDirectory(Kind);

        public string GetInstallationDirectory(ArtifactKind k)
        {
            return InstallationRepository.DirectoryForVersion(k, GetIdentForPackage(k));

        }

        public string CodeQLHome => CodeQLDirectory;

        public string CodeQLToolBinary
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return Path.Combine(CodeQLDirectory, "codeql.exe");
                }
                return Path.Combine(CodeQLDirectory, "codeql");
            }
        }
    }
}
