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
                QueryPackName = QueryPack,
                Name = Name,
                Scope = QueryPackScope,
                Base = Base
            };
            
            Directory.CreateDirectory(query.QueryFileDir);

            WriteTemplateIfOverwriteOrNotExists("new-query", query.QueryFilePath, "new query", new
            {
                language = query.Language,
                queryPackName = query.QueryPackName,
                queryName = query.Name,
                description = "Replace this text with a description of your query.",
                qlLanguageImport = query.GetLanguageImportForLangauge()
            });

            if (CreateQueryPack)
            {
                WriteTemplateIfOverwriteOrNotExists("qlpack-query", query.QueryPackPath, "new query pack", new
                {
                    queryPackScope = query.Scope,
                    queryPackName = query.QueryPackName
                });
            }
      
            if (CreateTests)
            {                
                Directory.CreateDirectory(query.QueryFileTestDir);

                // the source file to use 
                WriteTemplateIfOverwriteOrNotExists("test", query.QueryFileTestPath, "new query test file", new {});

                // the expected file 
                WriteTemplateIfOverwriteOrNotExists("expected", query.QueryTestExpectedFile, "new query test expected file", new { });

                // the the qlref file 
                WriteTemplateIfOverwriteOrNotExists("testref", query.QueryFileQLRefPath, "new query test ref", new {
                    queryName = query.Name
                });

                // the qlpack file
                WriteTemplateIfOverwriteOrNotExists("qlpack-test", query.QueryPackTestPath, "new query test pack", new {
                    queryPackDependency = $"{query.Scope}/{query.QueryPackName}",
                    queryPackScope = query.Scope,
                    queryPackName  = query.QueryTestPackName
                });

            }
            else
            {
                Log<NewQueryScaffoldTarget>.G().LogInformation($"Not creating tests because test creation was disabled.");
            }
        }
    }
}
