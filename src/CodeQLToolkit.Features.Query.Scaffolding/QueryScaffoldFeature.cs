using CodeQLToolkit.Shared.Feature;
using CodeQLToolkit.Shared.Template;
using System.CommandLine;

namespace CodeQLToolkit.Features.Query.Scaffolding
{

    public class QueryScaffoldFeature : IToolkitScaffoldingFeature
    {
        static TemplateUtil templateUtil { get; } = new TemplateUtil()
        {
            TemplatePath = ""
        };

        public void Register(Command parentCommand)
        {
            throw new NotImplementedException();
        }

        public int Run()
        {
            return 0;
        }
    }
}