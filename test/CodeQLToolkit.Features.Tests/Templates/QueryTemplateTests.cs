using CodeQLToolkit.Shared.Template;
using CodeQLToolkit.Shared.Utils;
using System.Text.RegularExpressions;

namespace CodeQLToolkit.Features.Tests.Templates
{
    [TestFixture]
    public class QueryTemplateTests
    {
        private TemplateUtil templateUtil;
        private readonly List<LanguageType> allSupportedLanguages = new()
        {
            LanguageType.C,
            LanguageType.CPP,
            LanguageType.CSHARP,
            LanguageType.GO,
            LanguageType.JAVA,
            LanguageType.JAVASCRIPT,
            LanguageType.PYTHON,
            LanguageType.RUBY
        };

        [SetUp]
        public void Setup()
        {
            templateUtil = new TemplateUtil();
        }

        [Test]
        public void AllLanguagesHaveQueryPackTemplates()
        {
            foreach (var language in allSupportedLanguages)
            {
                var templatePath = Path.Combine("Templates", "Query", language.ToDirectory(), "qlpack-query.liquid");

                Assert.DoesNotThrow(() => templateUtil.RawTemplateFromFile(templatePath),
                    $"qlpack-query.liquid template should exist for language {language}");
            }
        }

        [Test]
        public void AllLanguagesHaveNewQueryTemplates()
        {
            foreach (var language in allSupportedLanguages)
            {
                var templatePath = Path.Combine("Templates", "Query", language.ToDirectory(), "new-query.liquid");

                Assert.DoesNotThrow(() => templateUtil.RawTemplateFromFile(templatePath),
                    $"new-query.liquid template should exist for language {language}");
            }
        }

        [Test]
        public void AllLanguagesHaveTestTemplates()
        {
            foreach (var language in allSupportedLanguages)
            {
                var templatePath = Path.Combine("Templates", "Query", language.ToDirectory(), "test.liquid");

                Assert.DoesNotThrow(() => templateUtil.RawTemplateFromFile(templatePath),
                    $"test.liquid template should exist for language {language}");
            }
        }

        [Test]
        public void QueryPackTemplatesGenerateCorrectDependencies()
        {
            foreach (var language in allSupportedLanguages)
            {
                var templatePath = Path.Combine("Templates", "Query", language.ToDirectory(), "qlpack-query.liquid");
                var template = templateUtil.TemplateFromFile(templatePath);

                // Test data matching what NewQueryScaffoldTarget passes
                var testData = new
                {
                    queryPackScope = "testscope",
                    queryPackName = "testpack",
                    ql_language = language.ToDirectory()
                };

                var rendered = template.Render(testData);

                // Check that the dependency is correctly formatted
                var expectedDependency = $"codeql/{language.ToDirectory()}-all";
                Assert.That(rendered, Does.Contain(expectedDependency),
                    $"Template for {language} should generate dependency '{expectedDependency}' but rendered:\n{rendered}");

                // Ensure no empty variables (like "codeql/-all")
                Assert.That(rendered, Does.Not.Match(@"codeql/[^a-z]"),
                    $"Template for {language} should not contain malformed dependencies with empty language part");

                // Ensure the template variables are properly substituted
                Assert.That(rendered, Does.Not.Contain("{{"),
                    $"Template for {language} should not contain unresolved template variables");
                Assert.That(rendered, Does.Not.Contain("}}"),
                    $"Template for {language} should not contain unresolved template variables");
            }
        }

        [Test]
        public void NewQueryTemplatesGenerateValidQL()
        {
            foreach (var language in allSupportedLanguages)
            {
                var templatePath = Path.Combine("Templates", "Query", language.ToDirectory(), "new-query.liquid");
                var template = templateUtil.TemplateFromFile(templatePath);

                // Test data matching EXACTLY what NewQueryScaffoldTarget passes
                var testData = new
                {
                    language = language.ToDirectory(),
                    queryPackName = "testpack",
                    query_pack_name = "testpack",
                    queryName = "testquery",
                    query_name = "testquery",
                    description = "Test query description",
                    ql_language_import = language.ToImport()
                };

                var rendered = template.Render(testData);

                // Basic validation that it's a valid QL structure
                Assert.That(rendered, Does.Contain("import"),
                    $"Generated query for {language} should contain import statement");
                Assert.That(rendered, Does.Contain("from"),
                    $"Generated query for {language} should contain 'from' clause");
                Assert.That(rendered, Does.Contain("select"),
                    $"Generated query for {language} should contain 'select' clause");

                // Validate that the import statement is correctly rendered
                var expectedImport = $"import {language.ToImport()}";
                Assert.That(rendered, Does.Contain(expectedImport),
                    $"Generated query for {language} should contain correct import statement '{expectedImport}'");

                // Ensure no unresolved template variables
                Assert.That(rendered, Does.Not.Contain("{{"),
                    $"Template for {language} should not contain unresolved template variables");
                Assert.That(rendered, Does.Not.Contain("}}"),
                    $"Template for {language} should not contain unresolved template variables");
            }
        }

        [Test]
        public void LanguageToDirectoryMappingIsConsistent()
        {
            // Test that Language.ToDirectory() method returns expected values
            var expectedMappings = new Dictionary<LanguageType, string>
            {
                { LanguageType.C, "cpp" },
                { LanguageType.CPP, "cpp" },
                { LanguageType.CSHARP, "csharp" },
                { LanguageType.GO, "go" },
                { LanguageType.JAVA, "java" },
                { LanguageType.JAVASCRIPT, "javascript" },
                { LanguageType.PYTHON, "python" },
                { LanguageType.RUBY, "ruby" }
            };

            foreach (var kvp in expectedMappings)
            {
                Assert.That(kvp.Key.ToDirectory(), Is.EqualTo(kvp.Value),
                    $"Language {kvp.Key} should map to directory '{kvp.Value}'");
            }
        }

        [Test]
        public void AllSupportedLanguagesHaveCompleteTemplateSets()
        {
            var requiredTemplates = new[]
            {
                "qlpack-query.liquid",
                "new-query.liquid",
                "test.liquid",
                "expected.liquid",
                "qlpack-test.liquid"
            };

            foreach (var language in allSupportedLanguages)
            {
                foreach (var templateName in requiredTemplates)
                {
                    var templatePath = Path.Combine("Templates", "Query", language.ToDirectory(), templateName);

                    Assert.DoesNotThrow(() => templateUtil.RawTemplateFromFile(templatePath),
                        $"Template '{templateName}' should exist for language {language}");
                }
            }

            // Check that the shared testref.liquid template exists
            var sharedTestRefPath = Path.Combine("Templates", "Query", "all", "testref.liquid");
            Assert.DoesNotThrow(() => templateUtil.RawTemplateFromFile(sharedTestRefPath),
                "Shared testref.liquid template should exist in Templates/Query/all/");
        }

        [Test]
        [TestCase(LanguageType.PYTHON, "codeql/python-all")]
        [TestCase(LanguageType.JAVA, "codeql/java-all")]
        [TestCase(LanguageType.GO, "codeql/go-all")]
        [TestCase(LanguageType.RUBY, "codeql/ruby-all")]
        [TestCase(LanguageType.CSHARP, "codeql/csharp-all")]
        [TestCase(LanguageType.JAVASCRIPT, "codeql/javascript-all")]
        [TestCase(LanguageType.CPP, "codeql/cpp-all")]
        [TestCase(LanguageType.C, "codeql/cpp-all")]
        public void SpecificLanguageDependenciesAreCorrect(LanguageType language, string expectedDependency)
        {
            var templatePath = Path.Combine("Templates", "Query", language.ToDirectory(), "qlpack-query.liquid");
            var template = templateUtil.TemplateFromFile(templatePath);

            var testData = new
            {
                queryPackScope = "testscope",
                queryPackName = "testpack",
                ql_language = language.ToDirectory()
            };

            var rendered = template.Render(testData);

            Assert.That(rendered, Does.Contain($"{expectedDependency}:"),
                $"Template for {language} should generate dependency '{expectedDependency}' but got:\n{rendered}");
        }

        [Test]
        public void TestPackTemplatesGenerateCorrectContent()
        {
            foreach (var language in allSupportedLanguages)
            {
                var templatePath = Path.Combine("Templates", "Query", language.ToDirectory(), "qlpack-test.liquid");
                var template = templateUtil.TemplateFromFile(templatePath);

                // Test data matching what NewQueryScaffoldTarget passes
                var testData = new
                {
                    queryPackDependency = $"testscope/testpack",
                    queryPackScope = "testscope",
                    queryPackName = $"testpack-tests",  // This already includes "-tests"
                    query_pack_full_name = $"testscope/testpack-tests",
                    ql_language = language.ToDirectory()
                };

                var rendered = template.Render(testData);

                // Check that the name does not have double "-tests"
                Assert.That(rendered, Does.Not.Contain("testpack-tests-tests"),
                    $"Template for {language} should not have double '-tests' suffix but got:\n{rendered}");

                // Ensure no unresolved template variables
                Assert.That(rendered, Does.Not.Contain("{{"),
                    $"Template for {language} should not contain unresolved template variables");
                Assert.That(rendered, Does.Not.Contain("}}"),
                    $"Template for {language} should not contain unresolved template variables");

                // Should contain the correct dependency
                Assert.That(rendered, Does.Contain("testscope/testpack:"),
                    $"Template for {language} should contain correct dependency reference");
            }
        }

        [Test]
        public void TestPackTemplatesHandleEmptyScope()
        {
            foreach (var language in allSupportedLanguages)
            {
                var templatePath = Path.Combine("Templates", "Query", language.ToDirectory(), "qlpack-test.liquid");
                var template = templateUtil.TemplateFromFile(templatePath);

                // Test data with empty scope
                var testData = new
                {
                    queryPackDependency = $"testpack",  // No scope prefix
                    queryPackScope = "",
                    queryPackName = $"testpack-tests",
                    query_pack_full_name = $"testpack-tests",  // No scope prefix
                    ql_language = language.ToDirectory()
                };

                var rendered = template.Render(testData);

                // Should not contain invalid pack names with leading slash
                Assert.That(rendered, Does.Not.Contain("/testpack"),
                    $"Template for {language} should not contain invalid pack names with leading slash but got:\n{rendered}");

                // Should contain the correct dependency without scope
                Assert.That(rendered, Does.Contain("testpack:"),
                    $"Template for {language} should contain correct dependency reference without scope");

                // Ensure no unresolved template variables
                Assert.That(rendered, Does.Not.Contain("{{"),
                    $"Template for {language} should not contain unresolved template variables");
                Assert.That(rendered, Does.Not.Contain("}}"),
                    $"Template for {language} should not contain unresolved template variables");
            }
        }

        [Test]
        public void QueryTemplatesGenerateCorrectSyntax()
        {
            foreach (var language in allSupportedLanguages)
            {
                var templatePath = Path.Combine("Templates", "Query", language.ToDirectory(), "new-query.liquid");
                var template = templateUtil.TemplateFromFile(templatePath);

                // Test data with a mixed-case query name
                var testData = new
                {
                    language = language.ToDirectory(),
                    query_pack_name = "test-queries",
                    query_name = "TestQuery123",
                    description = "Test description",
                    ql_language_import = language.ToImport()
                };

                var rendered = template.Render(testData);

                // Check that @id uses lowercase query name
                Assert.That(rendered, Does.Contain($"@id {language.ToDirectory()}/test-queries/testquery123"),
                    $"Template for {language} should use lowercase query name in @id");

                // Check that @name preserves original case
                Assert.That(rendered, Does.Contain("@name TestQuery123"),
                    $"Template for {language} should preserve original case in @name");

                // Check that there are no leading spaces on import, from, select lines
                var lines = rendered.Split('\n');
                foreach (var line in lines)
                {
                    if (line.TrimStart().StartsWith("import ") ||
                        line.TrimStart().StartsWith("from ") ||
                        line.TrimStart().StartsWith("select "))
                    {
                        Assert.That(line, Does.Not.StartWith(" "),
                            $"Template for {language} should not have leading spaces on CodeQL statements. Found: '{line}'");
                    }
                }

                // Ensure no unresolved template variables
                Assert.That(rendered, Does.Not.Contain("{{"),
                    $"Template for {language} should not contain unresolved template variables");
                Assert.That(rendered, Does.Not.Contain("}}"),
                    $"Template for {language} should not contain unresolved template variables");
            }
        }

        [Test]
        public void DataflowQueryTemplatesGenerateValidQL()
        {
            foreach (var language in allSupportedLanguages)
            {
                var templatePath = Path.Combine("Templates", "Query", language.ToDirectory(), "new-dataflow-query.liquid");
                var template = templateUtil.TemplateFromFile(templatePath);

                // Test data matching EXACTLY what NewQueryScaffoldTarget passes for dataflow queries
                var testData = new
                {
                    language = language.ToDirectory(),
                    queryPackName = "testpack",
                    query_pack_name = "testpack",
                    queryName = "testquery",
                    query_name = "testquery",
                    description = "Test dataflow query description",
                    ql_language_import = language.ToImport()
                };

                var rendered = template.Render(testData);

                // Validate that it's a valid dataflow QL structure
                Assert.That(rendered, Does.Contain("@kind path-problem"),
                    $"Generated dataflow query for {language} should contain '@kind path-problem'");
                Assert.That(rendered, Does.Contain("import"),
                    $"Generated dataflow query for {language} should contain import statement");

                // Validate that the import statement is correctly rendered
                var expectedImport = $"import {language.ToImport()}";
                Assert.That(rendered, Does.Contain(expectedImport),
                    $"Generated dataflow query for {language} should contain correct import statement '{expectedImport}'");

                // Ensure no unresolved template variables
                Assert.That(rendered, Does.Not.Contain("{{"),
                    $"Dataflow template for {language} should not contain unresolved template variables");
                Assert.That(rendered, Does.Not.Contain("}}"),
                    $"Dataflow template for {language} should not contain unresolved template variables");
            }
        }

        [Test]
        public void DetectsMissingTemplateVariables()
        {
            // This test ensures that missing template variables are detected using strict rendering
            var templatePath = Path.Combine("Templates", "Query", "python", "new-query.liquid");
            var template = templateUtil.TemplateFromFile(templatePath);

            // Pass template data with a MISSING required variable (ql_language_import)
            var incompleteTemplateData = new
            {
                language = "python",
                queryPackName = "test-pack",
                query_pack_name = "test-pack",
                queryName = "TestQuery",
                query_name = "TestQuery",
                description = "Test description",
                // Missing: ql_language_import
            };

            // When rendering with strict variables, this should throw an exception for undefined variables
            try
            {
                var result = templateUtil.RenderTemplateStrictly(template, incompleteTemplateData);
                Assert.Fail($"Expected an exception when ql_language_import is missing, but got result: {result}");
            }
            catch (Exception ex)
            {
                // Verify the exception mentions the missing variable
                Assert.That(ex.Message, Does.Contain("ql_language_import"),
                    $"Exception should mention the missing ql_language_import variable. Got: {ex.Message}");

                // This is expected - the test should pass when an exception is thrown for missing variables
                Assert.Pass("Template correctly threw an exception for missing ql_language_import variable");
            }
        }

        [Test]
        public void StrictRenderingWorksWithCompleteTemplateData()
        {
            // This test ensures that strict rendering works correctly when all variables are provided
            var templatePath = Path.Combine("Templates", "Query", "python", "new-query.liquid");
            var template = templateUtil.TemplateFromFile(templatePath);

            // Pass complete template data with ALL required variables
            var completeTemplateData = new
            {
                language = "python",
                queryPackName = "test-pack",
                query_pack_name = "test-pack",
                queryName = "TestQuery",
                query_name = "TestQuery",
                description = "Test description",
                ql_language_import = "python" // This variable is present
            };

            // With all variables provided, strict rendering should work without throwing
            string result = null;
            Assert.DoesNotThrow(() =>
            {
                result = templateUtil.RenderTemplateStrictly(template, completeTemplateData);
            }, "Template should render successfully when all required variables are provided");

            // Verify the template rendered correctly
            Assert.That(result, Is.Not.Null.And.Not.Empty, "Template should produce output");
            Assert.That(result, Does.Contain("import python"), "Template should contain the correct import statement");
            Assert.That(result, Does.Contain("@name TestQuery"), "Template should contain the query name");
        }

        [Test]
        public void TemplateDataMatchesNewQueryScaffoldTargetExactly()
        {
            // This test ensures that NewQueryScaffoldTarget passes the correct template variables
            var mockQuery = new CodeQLToolkit.Shared.Utils.Query
            {
                Language = LanguageType.PYTHON,
                QueryPackName = "test-pack",
                Name = "TestQuery",
                Scope = "test-scope",
                Base = "/tmp/test"
            };

            // Use the exact data structure that NewQueryScaffoldTarget should pass
            var correctTemplateData = new
            {
                language = mockQuery.Language.ToDirectory(),
                queryPackName = mockQuery.QueryPackName,
                query_pack_name = mockQuery.QueryPackName,
                queryName = mockQuery.Name,
                query_name = mockQuery.Name,
                description = "Replace this text with a description of your query.",
                ql_language_import = mockQuery.GetLanguageImportForLanguage() // Correct variable name
            };

            var templatePath = Path.Combine("Templates", "Query", "python", "new-query.liquid");
            var template = templateUtil.TemplateFromFile(templatePath);
            var rendered = template.Render(correctTemplateData);

            // Verify that the template renders correctly with proper variable names
            Assert.That(rendered, Does.Contain("import python"),
                "Template should render correct import statement when using proper variable names");

            // Verify no empty import statements (which would indicate missing variables)
            Assert.That(rendered, Does.Not.Contain("import \n"),
                "Template should not have empty import statements");
            Assert.That(rendered, Does.Not.Contain("import \r"),
                "Template should not have empty import statements");
        }
    }
}
