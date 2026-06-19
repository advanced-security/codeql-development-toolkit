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
        public string QueryKind { get; set; }

        public override void Run()
        {
            Log<NewQueryScaffoldTarget>.G().LogInformation("Creating new query...");

            // Use the provided query name exactly as given
            var query = new Shared.Utils.Query()
            {
                Language = Language,
                QueryPackName = QueryPack,
                Name = Name,
                Scope = QueryPackScope,
                Base = Base
            };

            Directory.CreateDirectory(query.QueryFileDir);

            // Select template based on query kind
            string templateName = QueryKind.ToLowerInvariant() switch
            {
                "path-problem" => "new-dataflow-query",
                "problem" => "new-query",
                _ => "new-query"
            };

            WriteTemplateIfOverwriteOrNotExists(templateName, query.QueryFilePath, "new query", new
            {
                language = query.Language.ToDirectory(), // Use directory name instead of enum value
                queryPackName = query.QueryPackName,
                query_pack_name = query.QueryPackName, // Add snake_case version for template compatibility
                queryName = query.Name,
                query_name = query.Name, // Add snake_case version for template compatibility
                description = "Replace this text with a description of your query.",
                ql_language_import = query.GetLanguageImportForLanguage()
            });

            if (CreateQueryPack)
            {
                WriteTemplateIfOverwriteOrNotExists("qlpack-query", query.QueryPackPath, "new query pack", new
                {
                    queryPackScope = query.Scope,
                    queryPackName = query.QueryPackName,
                    query_pack_full_name = string.IsNullOrEmpty(query.Scope) ? query.QueryPackName : $"{query.Scope}/{query.QueryPackName}",
                    ql_language = query.Language.ToDirectory()
                });
            }

            if (CreateTests)
            {
                Directory.CreateDirectory(query.QueryFileTestDir);

                // the source file to use 
                WriteTemplateIfOverwriteOrNotExists("test", query.QueryFileTestPath, "new query test file", new
                {
                    queryName = query.Name,
                    query_name = query.Name,
                    query_kind = QueryKind,
                    test_file_prefix = query.Name
                });

                // the expected file 
                WriteTemplateIfOverwriteOrNotExists("expected", query.QueryTestExpectedFile, "new query test expected file", new
                {
                    query_kind = QueryKind,
                    test_file_prefix = query.Name
                });

                // the the qlref file 
                WriteTemplateIfOverwriteOrNotExists("testref", query.QueryFileQLRefPath, "new query test ref", new
                {
                    queryName = query.Name
                });

                // the qlpack file
                WriteTemplateIfOverwriteOrNotExists("qlpack-test", query.QueryPackTestPath, "new query test pack", new
                {
                    queryPackDependency = string.IsNullOrEmpty(query.Scope) ? query.QueryPackName : $"{query.Scope}/{query.QueryPackName}",
                    queryPackScope = query.Scope,
                    queryPackName = query.QueryTestPackName,
                    query_pack_full_name = string.IsNullOrEmpty(query.Scope) ? query.QueryTestPackName : $"{query.Scope}/{query.QueryTestPackName}",
                    ql_language = query.Language.ToDirectory()
                });

            }
            else
            {
                Log<NewQueryScaffoldTarget>.G().LogInformation($"Not creating tests because test creation was disabled.");
            }
        }
    }
}
