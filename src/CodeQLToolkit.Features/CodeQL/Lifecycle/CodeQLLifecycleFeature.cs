using CodeQLToolkit.Features.CodeQL.Lifecycle.Targets;
using CodeQLToolkit.Shared.CodeQL;
using System.CommandLine;

namespace CodeQLToolkit.Features.CodeQL.Lifecycle
{
    public class CodeQLLifecycleFeature : FeatureBase, IToolkitLifecycleFeature
    {
        public CodeQLLifecycleFeature()
        {
            FeatureName = "CodeQL";
        }

        public void Register(Command parentCommand)
        {
            Log<CodeQLLifecycleFeature>.G().LogInformation("Registering lifecycle submodule.");

            var setCommand = new Command("set", "Functions pertaining to setting variables related to CodeQL.");
            parentCommand.Add(setCommand);

            var setVersionCommand = new Command("version", "Sets the version of CodeQL used.");

            var cliVersionOption = new Option<string>("--cli-version", GitHubReleaseResolver.GetLatestCLIVersion, "The version of the cli to use. Example: `2.25.1`.") { IsRequired = true };
            var standardLibraryVersionOption = new Option<string>("--standard-library-version", GitHubReleaseResolver.GetLatestStandardLibraryVersion, "The version of the standard library to use. Example: `codeql-cli/v2.25.1`.") { IsRequired = true };
            var bundleVersionOption = new Option<string>("--bundle-version", GitHubReleaseResolver.GetLatestBundleVersion, "The bundle version to use. Example: `codeql-bundle-v2.25.1`.") { IsRequired = true };

            setVersionCommand.Add(cliVersionOption);
            setVersionCommand.Add(standardLibraryVersionOption);
            setVersionCommand.Add(bundleVersionOption);

            setCommand.Add(setVersionCommand);

            var getCommand = new Command("get", "Functions pertaining to getting variables related to CodeQL.");
            parentCommand.Add(getCommand);

            var getVersionCommand = new Command("version", "Gets the version of CodeQL used.");
            getCommand.Add(getVersionCommand);

            {
                setVersionCommand.SetHandler((cliVersion, standardLibraryVersion, bundleVersion, basePath) =>
                {
                    Log<CodeQLLifecycleFeature>.G().LogInformation("Executing set command...");

                    new SetVersionLifecycleTarget()
                    {
                        CodeQLCLI = cliVersion,
                        CodeQLStandardLibrary = standardLibraryVersion,
                        CodeQLCLIBundle = bundleVersion,
                        Base = basePath
                    }.Run();

                }, cliVersionOption, standardLibraryVersionOption, bundleVersionOption, Globals.BasePathOption);
            }


            {
                getVersionCommand.SetHandler((basePath) => 
                {
                    Log<CodeQLLifecycleFeature>.G().LogInformation("Executing get command...");

                    new GetVersionLifecycleTarget()
                    {
                        Base = basePath
                    }.Run();

                }, Globals.BasePathOption);
            }


        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}