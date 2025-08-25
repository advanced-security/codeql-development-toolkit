using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scriban;
using Scriban.Runtime;

namespace CodeQLToolkit.Shared.Template
{
    public class TemplateUtil
    {
        public string TemplatePath { get; set; } = Utils.FileUtils.GetExecutingDirectory().FullName;

        public Scriban.Template TemplateFromFile(string templateFile)
        {
            var templateFilePath = Path.Combine(TemplatePath, templateFile);

            var data = File.ReadAllText(templateFilePath);

            var template = Scriban.Template.ParseLiquid(data);

            return template;
        }

        public Scriban.Template TemplateFromFileStrict(string templateFile)
        {
            var templateFilePath = Path.Combine(TemplatePath, templateFile);

            var data = File.ReadAllText(templateFilePath);

            var template = Scriban.Template.ParseLiquid(data);

            return template;
        }

        public string RenderTemplateStrictly(Scriban.Template template, object model)
        {
            // Create a template context with default built-ins and strict variables
            var context = new TemplateContext();
            context.StrictVariables = true;

            // Import all the default built-in functions
            context.BuiltinObject.Import(new Scriban.Functions.StringFunctions());

            var scriptObject = new ScriptObject();
            scriptObject.Import(model);
            context.PushGlobal(scriptObject);

            return template.Render(context);
        }

        public string RawTemplateFromFile(string templateFile)
        {
            var templateFilePath = Path.Combine(TemplatePath, templateFile);

            var data = File.ReadAllText(templateFilePath);

            return data;
        }
    }
}
