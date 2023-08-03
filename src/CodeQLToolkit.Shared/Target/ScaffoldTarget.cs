using CodeQLToolkit.Shared.Logging;
using CodeQLToolkit.Shared.Template;
using Microsoft.Extensions.Logging;

namespace CodeQLToolkit.Shared.Target
{
    public abstract class ScaffoldTarget : ITarget
    {
        public string Base { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public bool OverwriteExisting { get; set; }
        public string FeatureName { get; set; }

        public abstract void Run();

        public string GetTemplatePathForLanguage(string templateName)
        {
            var languagePath = Language;

            if(languagePath == "c")
            {
                languagePath = "cpp";

            }
            return Path.Combine("Templates", FeatureName, languagePath, templateName + ".liquid");
        }

        

        public string GetTemplatePath(string templateName)
        {
            return Path.Combine("Templates", FeatureName, "all", templateName + ".liquid");
        }

        public void WriteTemplateIfOverwriteOrNotExists(string template, string path, string description, object model)
        {
            if (!File.Exists(template) || OverwriteExisting)
            {
                Log<ScaffoldTarget>.G().LogInformation($"Writing new {description} in {path}.");

                var t = new TemplateUtil().TemplateFromFile(GetTemplatePathForLanguage("new-query"));

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
