
using CodeQLDevelopmentLifecycleToolkit.Features.Query;
using CommandLine;
using CommandLine.Text;

namespace CodeQLDevelopmentLifecycleToolkit.Core
{

    // TODO - remove this temporary command once another base command is added.
    // The way the library works is that it will omit printing of help text if there
    // is only one command. 
    [Verb("add", HelpText = "Add file contents to the index.")]
    class AddOptions
    {
        [Option(Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }
    }

    internal class QLT
    {       

        static int Main(string[] args)
        {
            // each command gets a slice of the arguments, scoped below the current command.
            string[] subArgs = args.Skip(1).ToArray();

            // add help option 
            return Parser.Default.ParseArguments<AddOptions, QueryFeatureOptions>(args)
            .MapResult(
                (QueryFeatureOptions opts) => QueryFeatureMain.Instance.Run(opts, subArgs),
            errs => 1);
        }
    }
}