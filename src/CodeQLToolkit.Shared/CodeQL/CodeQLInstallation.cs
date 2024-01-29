using System.Net;
using CodeQLToolkit.Shared.Utils;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using CodeQLToolkit.Shared.Logging;
using LibGit2Sharp;
using System.IO.Compression;
using System.Runtime.CompilerServices;


namespace CodeQLToolkit.Shared.CodeQL
{
    public class CodeQLInstallation
    {
        public string CLIVersion { get; set; }
        public string StandardLibraryVersion { get; set; }
        public string CLIBundle { get; set; }
        public string StandardLibraryIdent { get; set; }
        public bool EnableCustomCodeQLBundles { get; set; }
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
                EnableCustomCodeQLBundles = config.EnableCustomCodeQLBundles,
                CLIVersion = config.CodeQLCLI,
                CLIBundle = config.CodeQLCLIBundle,
                StandardLibraryIdent = config.CodeQLStandardLibraryIdent,
                StandardLibraryVersion = config.CodeQLStandardLibrary,
                Base = config.Base                
            };


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

        public string PlatformExtension
        {
            get
            {
                // for now they are all zips,
                return "zip";
            }
        }

        public void Install()
        {
            // each time we download the file; however, 
            // .qlt/repo/packages/ident
            // .qlt/repo/custom-bundle/ident
            // .qlt/repo/bundle/ident

            // https://github.com/github/codeql-cli-binaries/releases/download/v2.16.0/codeql-linux64.zip
            
            // workout a destination directory for this 

            if(Kind == ArtifactKind.CUSTOM_BUNDLE)
            {
                CustomBundleInstall();
            }else if(Kind == ArtifactKind.BUNDLE)
            {
                BundleInstall();                
            }else
            {
                PackageInstall();
            }




        }

        private void CustomBundleInstall()
        {
            throw new NotImplementedException();
        }

        private void BundleInstall()
        {
            throw new NotImplementedException();
        }

        public string StdLibDirectory
        {
            get
            {
                return Path.Combine(InstallationDirectory, "codeql-stdlib");

            }
        }

        public string CodeQLDirectory
        {
            get
            {
                return Path.Combine(InstallationDirectory, "codeql");

            }
        }

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

        

        private string GetIdentForPackage(ArtifactKind k)
        {
            if (k == ArtifactKind.PACKAGE)
            {
                var ident =  String.Join("", "codeql-cli-" + CLIVersion, "#standard-library-ident-" ,StandardLibraryIdent);
                return StringUtils.CreateMD5(FileUtils.SanitizeFilename(ident)).ToLower();
            }

            throw new NotImplementedException();
        }

        public bool IsInstalled()
        {
            Log<CodeQLInstallation>.G().LogInformation($"Checking if codeql is installed...");
            Log<CodeQLInstallation>.G().LogInformation($"Requested CLI Version {CLIVersion}, Standard Library Ident: {StandardLibraryIdent}");

            // if custom bundles are enabled 
            // they are doing local development or running
            // unit tests. In either case, we want to make sure
            // to reassemble the bundle. 
            if (EnableCustomCodeQLBundles)
            {
                Log<CodeQLInstallation>.G().LogInformation($"Custom bundle mode.");
                return false;
            }

            Log<CodeQLInstallation>.G().LogInformation($"CodeQL Package Mode");
            Log<CodeQLInstallation>.G().LogInformation($"Checking for existance of directory {InstallationDirectory}");

            if (Directory.Exists(InstallationDirectory) && Directory.Exists(CodeQLDirectory) && Directory.Exists(StdLibDirectory))
            {
                Log<CodeQLInstallation>.G().LogInformation($"CodeQL Installation exists. CLI Version {CLIVersion}, Standard Library Ident: {StandardLibraryIdent}");
                return true;
            }

            return false;
        }

        public string InstallationDirectory
        {
            get {
                return GetInstallationDirectory(Kind);
            }
        }

        public string GetInstallationDirectory(ArtifactKind k)
        {
            return InstallationRepository.DirectoryForVersion(k, GetIdentForPackage(k));

        }

        public string CodeQLHome { get {
                return "";
            } 
        }

        public string CodeQLToolBinary { get {
                return Path.Combine("", "");
            } 
        }

    }
}
