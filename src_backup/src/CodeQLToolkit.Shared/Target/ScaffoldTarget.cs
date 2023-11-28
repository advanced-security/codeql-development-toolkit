using CodeQLToolkit.Shared.Logging;
using CodeQLToolkit.Shared.Template;
using CodeQLToolkit.Shared.Utils;
using Microsoft.Extensions.Logging;

namespace CodeQLToolkit.Shared.Target
{
    public abstract class ScaffoldTarget : ITarget
    {
       
        public string Name { get; set; }
        public LanguageType Language { get; set; }
        public bool OverwriteExisting { get; set; }
        public string FeatureName { get; set; }

       

        public string GetTemplatePathForLanguage(string templateName)
        {
            var languagePath = Language;

            return Path.Combine("Templates", FeatureName, Language.ToDirectory(), templateName + ".liquid");
        }

        

        public string GetTemplatePath(string templateName)
        {
            return Path.Combine("Templates", FeatureName, "all", templateName + ".liquid");
        }

        public void WriteTemplateIfOverwriteOrNotExists(string template, string path, string description, object model)
        {
            if (!File.Exists(path) || OverwriteExisting)
            {
                Log<ScaffoldTarget>.G().LogInformation($"Writing new {description} in {path}.");

                var t = new TemplateUtil().TemplateFromFile(GetTemplatePathForLanguage(template));

                var rendered = t.Render(model);

                File.WriteAllText(path, rendered);
            }
            else
            {
                Log<ScaffoldTarget>.G().LogInformation($"Refusing to overwrite existing {description} in {path}");
            }
        }

    }
}
