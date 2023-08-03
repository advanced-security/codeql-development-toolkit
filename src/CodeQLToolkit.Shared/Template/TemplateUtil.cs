using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scriban;

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
    }
}
