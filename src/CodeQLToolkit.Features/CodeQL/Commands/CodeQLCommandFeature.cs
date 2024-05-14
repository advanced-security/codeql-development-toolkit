using CodeQLToolkit.Features.CodeQL.Commands.Targets;
using CodeQLToolkit.Features.Test.Commands.Targets;
using CodeQLToolkit.Features.Test.Lifecycle;
using CodeQLToolkit.Shared.Types;
using CodeQLToolkit.Shared.Utils;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.CodeQL.Commands
{
    public class CodeQLCommandFeature : FeatureBase, IToolkitLifecycleFeature
    {
        public override LanguageType[] SupportedLangauges => new LanguageType[] {
            LanguageType.C,
            LanguageType.CPP,
            LanguageType.CSHARP,
            LanguageType.JAVA,
            LanguageType.JAVASCRIPT,
            LanguageType.GO,
            LanguageType.RUBY,
            LanguageType.PYTHON
        };

        public CodeQLCommandFeature()
        {
            FeatureName = "CodeQL";
        }

        public void Register(Command parentCommand)
        {
            Log<CodeQLCommandFeature>.G().LogInformation("Registering command submodule.");

            var runCommand = new Command("run", "Functions pertaining to running codeql-related commands.");
            parentCommand.Add(runCommand);

            var installCommand = new Command("install", "Installs CodeQL (bundle or release distribution) locally.");
            var customBundleOption = new Option<bool>("--custom-bundle", () => false, "Build a custom bundle and compile the bundle.") { IsRequired = true };
            var quickBundleOption = new Option<bool>("--quick-bundle", () => false, "Build a custom bundle and DO NOT compile the bundle.") { IsRequired = true };
            var packsOption = new Option<string[]>("--packs", "When creating bundles, this specifies the packs to include, Example `pack1 pack2 pack3`. You may specify also as `--pack pack1 --pack2 --pack3`") { IsRequired = false, AllowMultipleArgumentsPerToken = true };

            installCommand.Add(customBundleOption);
            installCommand.Add(quickBundleOption);
            installCommand.Add(packsOption);

            runCommand.Add(installCommand);

            installCommand.SetHandler((basePath, automationType, customBundleOption, quickBundleOption, packs) =>
            {
                Log<CodeQLCommandFeature>.G().LogInformation("Executing install command...");

                new InstallCommand()
                {
                    Base = basePath,
                    AutomationTarget = automationType,
                    CustomBundles = customBundleOption,
                    QuickBundles = quickBundleOption,
                    Packs = packs
                }.Run();


            }, Globals.BasePathOption, Globals.AutomationTypeOption, customBundleOption, quickBundleOption, packsOption);
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
