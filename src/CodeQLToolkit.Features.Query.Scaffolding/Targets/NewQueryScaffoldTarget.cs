using CodeQLToolkit.Shared.Template;
using CodeQLToolkit.Shared.Utils;
using System.Reflection;

namespace CodeQLToolkit.Features.Query.Scaffolding.Targets
{
    public class NewQueryScaffoldTarget : ScaffoldTarget
    {
        public string QueryPack { get; set; }
        public bool CreateTests { get; set; }
        public bool CreateQueryPack { get; set; }
        public string QueryPackScope { get; set; }

        public override void Run()
        {
            Log<NewQueryScaffoldTarget>.G().LogInformation("Creating new query...");

            var query = new Shared.Utils.Query()
            {
                Language = Language,
                QueryPack = QueryPack,
                Name = Name,
                Scope = QueryPackScope,
                Base = Base
            };
            
            Directory.CreateDirectory(query.QueryFileDir);

            WriteTemplateIfOverwriteOrNotExists("new-query", query.QueryFilePath, "new query", new
            {
                language = query.Language,
                package = query.QueryPack,
                queryName = query.Name,
                description = "Replace this text with a description of your query.",
                qlLanguageImport = query.GetLanguageImportForLangauge()
            });

            if (CreateQueryPack)
            {
                WriteTemplateIfOverwriteOrNotExists("qlpack-query", query.QueryPackPath, "new query pack", new
                {
                    packName = $"{query.Scope}/{query.QueryPack}"
                });
            }
      
            if (CreateTests)
            {                
                Directory.CreateDirectory(query.QueryTestDir);

                // the source file to use 
                WriteTemplateIfOverwriteOrNotExists("test", query.QueryFileTestPath, "new query test file", new {});

                // the expected file 
                WriteTemplateIfOverwriteOrNotExists("expected", query.QueryTestExpectedFile, "new query test expected file", new { });

                // the the qlref file 
                WriteTemplateIfOverwriteOrNotExists("testref", query.QueryFileQLRefPath, "new query test ref", new { });

                // the qlpack file
                WriteTemplateIfOverwriteOrNotExists("qlpack-test", query.QueryPackTestPath, "new query test pack", new { });

            }
            else
            {
                Log<NewQueryScaffoldTarget>.G().LogInformation($"Not creating tests because test creation was disabled.");
            }
        }
    }
}
