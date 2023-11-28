using CodeQLToolkit.Features.Pack.Commands.Targets;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Features.Pack.Commands
{
    public class PackCommandFeature : FeatureBase, IToolkitCommandFeature
    {
        public void Register(Command parentCommand)
        {
            Log<PackCommandFeature>.G().LogInformation("Registering command submodule.");

            var runCommand = new Command("run", "Functions pertaining to running pack-related commands.");
            parentCommand.Add(runCommand);

            // a command that installs query packs
            var sayHello = new Command("hello-jeongsoo", "Says hello!");
            var howManyTimesHello = new Option<int>("--times", "how many times to say it") { IsRequired = true };
            sayHello.Add(howManyTimesHello);

            var sayGoodbye = new Command("goodbye-jeongsoo", "Says goodbye!");

            var howManyTimes = new Option<int>("--times", "how many times to say it") { IsRequired = true };
            sayGoodbye.Add(howManyTimes);


            runCommand.Add(sayHello);
            runCommand.Add(sayGoodbye);

            sayHello.SetHandler((basePath, times) => {

                new HelloJeongsooCommandTarget() { 
                    Base = basePath,
                    Times = times 
                
                }.Run();

            }, Globals.BasePathOption, howManyTimesHello);

            sayGoodbye.SetHandler((basePath, times) => {

                Console.WriteLine($"Saying goodbye {times} number of times");

                for (int i = 0; i < times; i++)
                {
                    Console.WriteLine("Goodbye!");
                }


            }, Globals.BasePathOption, howManyTimes);

        }

        public int Run()
        {
            throw new NotImplementedException();
        }
    }
}
