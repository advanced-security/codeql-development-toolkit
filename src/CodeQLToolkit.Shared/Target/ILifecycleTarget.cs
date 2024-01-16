using CodeQLToolkit.Shared.Logging;
using CodeQLToolkit.Shared.Template;
using CodeQLToolkit.Shared.Types;
using Microsoft.Extensions.Logging;


namespace CodeQLToolkit.Shared.Target
{
    public abstract class ILifecycleTarget : ITarget
    {
        public string Name { get; set; }
        public string Language { get; set; }
        public bool OverwriteExisting { get; set; }
        public string FeatureName { get; set; }
        public bool DevMode { get; set; }
        public AutomationType AutomationType { get; set; } = AutomationType.ANY;


        public string GetTemplatePath(string templateName)
        {
            var languagePath = Language;

            List<string> pathElements = new List<string>();

            pathElements.Add("Templates");
            pathElements.Add(FeatureName);


            if (AutomationType != AutomationType.ANY)
            {
                pathElements.Add(AutomationType.ToDirectory());
            }

            if (languagePath != null)
            {
                if (languagePath == "c")
                {
                    languagePath = "cpp";
                }

                pathElements.Add(languagePath);
            }

            pathElements.Add(templateName + ".liquid");

            return Path.Combine(pathElements.ToArray());
        }

        public void WriteTemplateIfOverwriteOrNotExists(string template, string path, string description)
        {
            WriteTemplateIfOverwriteOrNotExists(template, path, description, null);
        }


        public void WriteTemplateIfOverwriteOrNotExists(string template, string path, string description, object model)
        {
            if (!File.Exists(path) || OverwriteExisting)
            {

                Directory.CreateDirectory(Path.GetDirectoryName(path));

                Log<ScaffoldTarget>.G().LogInformation($"Writing new {description} in {path}.");

                var templatePath = GetTemplatePath(template);


                if (model == null)
                {
                    var rendered = new TemplateUtil().RawTemplateFromFile(templatePath);
                    Log<ScaffoldTarget>.G().LogInformation($"Loaded raw template {templatePath}");

                    File.WriteAllText(path, rendered);
                }
                else
                {
                    var t = new TemplateUtil().TemplateFromFile(templatePath);

                    Log<ScaffoldTarget>.G().LogInformation($"Loaded template {templatePath}");

                    var rendered = t.Render(model);
                    File.WriteAllText(path, rendered);
                }
                
            }
            else
            {
                Log<ScaffoldTarget>.G().LogInformation($"Refusing to overwrite existing {description} in {path}");
            }
        }
}
}
