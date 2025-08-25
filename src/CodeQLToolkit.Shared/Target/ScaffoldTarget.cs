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

            // First check if there's a shared template in the "all" directory
            var sharedTemplatePath = Path.Combine("Templates", FeatureName, "all", templateName + ".liquid");
            var templateUtil = new TemplateUtil();

            try
            {
                // Try to load the shared template first
                templateUtil.RawTemplateFromFile(sharedTemplatePath);
                return sharedTemplatePath;
            }
            catch
            {
                // If shared template doesn't exist, fall back to language-specific template
                return Path.Combine("Templates", FeatureName, Language.ToDirectory(), templateName + ".liquid");
            }
        }



        public string GetTemplatePath(string templateName)
        {
            return Path.Combine("Templates", FeatureName, "all", templateName + ".liquid");
        }

        public void WriteTemplateIfOverwriteOrNotExists(string template, string path, string description, object model)
        {
            if (!File.Exists(path) || OverwriteExisting)
            {
                Log<ScaffoldTarget>.G().LogInformation($"Writing {description} in {path}.");

                var templateUtil = new TemplateUtil();
                var t = templateUtil.TemplateFromFile(GetTemplatePathForLanguage(template));

                var rendered = templateUtil.RenderTemplateStrictly(t, model);

                File.WriteAllText(path, rendered);
            }
            else
            {
                Log<ScaffoldTarget>.G().LogInformation($"Refusing to overwrite existing {description} in {path}");
            }
        }

    }
}
