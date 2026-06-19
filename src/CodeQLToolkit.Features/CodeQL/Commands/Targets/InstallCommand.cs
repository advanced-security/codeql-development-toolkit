using CodeQLToolkit.Shared.CodeQL;
using CodeQLToolkit.Shared.Types;
using CodeQLToolkit.Shared.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.CodeQL.Commands.Targets
{
    public class InstallCommand : CommandTarget
    {
        public bool CustomBundles { get; set; }
        public bool QuickBundles { get; set; }
        public string[] Packs { get; set; } = Array.Empty<string>();

        void SetEnvironmentVariableMultiTarget(string name, string value)
        {
            Log<InstallCommand>.G().LogInformation($"Setting {name} to {value}...");

            Environment.SetEnvironmentVariable(name, value);

            // Only attempt to write to GITHUB_ENV if we're running in GitHub Actions
            if (AutomationTypeHelper.AutomationTypeFromString(AutomationTarget) == AutomationType.ACTIONS)
            {
                string? githubEnvPath = Environment.GetEnvironmentVariable("GITHUB_ENV");
                try
                {
                    if (!string.IsNullOrEmpty(githubEnvPath) && File.Exists(githubEnvPath))
                    {
                        File.AppendAllText(githubEnvPath, $"{name}={value}\n");
                        Log<InstallCommand>.G().LogInformation($"Successfully wrote {name} to GITHUB_ENV file.");
                    }
                    else
                    {
                        Log<InstallCommand>.G().LogWarning($"GITHUB_ENV file not found. Environment variable {name} set locally only.");
                    }
                }
                catch (Exception ex)
                {
                    Log<InstallCommand>.G().LogWarning($"Could not write to GITHUB_ENV file: {ex.Message}. Environment variable {name} set locally only.");
                }
            }
        }

        public override void Run()
        {
            Log<InstallCommand>.G().LogInformation($"Running Install command");

            // First, check if CodeQL is installed.
            var installation = CodeQLInstallation.LoadFromConfig(Base);
            if (CustomBundles || QuickBundles)
            {
                installation.EnableCustomCodeQLBundles = true;
                if (Packs != null && Packs.Length > 0)
                {
                    Log<InstallCommand>.G().LogInformation($"Overriding Packs on the command line. The following Packs will be packaged:");
                    installation.CodeQLPackConfiguration = Packs.Select(p => new CodeQLPackConfiguration()
                    {
                        Name = p,
                        Bundle = true
                    }).ToArray();
                }
                else
                {
                    Log<InstallCommand>.G().LogInformation($"Using `qlt.conf.json` to build list of packs to bundle. The following Packs will be packaged:");
                }

                installation.LogPacksToBeBuilt();

                installation.QuickBundle = QuickBundles;
            }

            Log<InstallCommand>.G().LogInformation($"Checking for installation...");

            // If CodeQL is already installed, but custom bundles are enabled, reinstall CodeQL anyway to ensure use of the correct custom bundle.
            if (installation.IsInstalled() && !installation.EnableCustomCodeQLBundles)
            {
                Log<InstallCommand>.G().LogInformation($"CodeQL is already installed at that version. Please delete the installation directory to reinstall.");
            }
            else
            {
                Log<InstallCommand>.G().LogInformation($"Installing CodeQL...");
                installation.Install();

                SetEnvironmentVariableMultiTarget("QLT_CODEQL_HOME", installation.CodeQLHome);
                SetEnvironmentVariableMultiTarget("QLT_CODEQL_PATH", installation.CodeQLToolBinary);

                if (CustomBundles || QuickBundles)
                {
                    SetEnvironmentVariableMultiTarget("QLT_CODEQL_BUNDLE_PATH", installation.CustomBundleOutputBundleCurrentPlatform);
                    SetEnvironmentVariableMultiTarget("QLT_CODEQL_BUNDLE_PATH_WIN64", installation.CustomBundleOutputBundleWindows);
                    SetEnvironmentVariableMultiTarget("QLT_CODEQL_BUNDLE_PATH_OSX64", installation.CustomBundleOutputBundleOSX);
                    SetEnvironmentVariableMultiTarget("QLT_CODEQL_BUNDLE_PATH_LINUX64", installation.CustomBundleOutputBundleLinux);
                }
            }

            Log<InstallCommand>.G().LogInformation($"Done.");
        }
    }
}
