using CommandLine;

namespace CodeQLToolkit.Shared
{
    public class BaseOptions
    {
        [Option(HelpText = "Sets the working directory for the tool.")]
        public string Base { get; set; }

        // note this should be called, instead of `Base`.
        public string GetBaseDirectory()
        {
            if(Base == null)
            {
                return Directory.GetCurrentDirectory();
            }

            return Base;
        }

        [Option(Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }
    }
}
