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
        public override LanguageType[] SupportedLangauges { get => new LanguageType[] { 
            LanguageType.C,
            LanguageType.CPP,
            LanguageType.CSHARP,
            LanguageType.JAVA,
            LanguageType.JAVASCRIPT,
            LanguageType.GO,
            LanguageType.RUBY,
            LanguageType.PYTHON            
        }; }

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
            //var useCommand = new Command("use", "Switches tooling to use a different CodeQL version and updates the paths accordingly.");
            //var listCommand = new Command("list", "Lists versions of CodeQL available locally.");

            runCommand.Add(installCommand);
            //runCommand.Add(useCommand);
            //runCommand.Add(listCommand);

           
            installCommand.SetHandler((basePath, automationType) =>
            {
                Log<CodeQLCommandFeature>.G().LogInformation("Executing install command...");

                new InstallCommand()
                {
                    Base = basePath,
                    AutomationTarget = automationType,
                }.Run();


            }, Globals.BasePathOption, Globals.AutomationTypeOption);
        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
