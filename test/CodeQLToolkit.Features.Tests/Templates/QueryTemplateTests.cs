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

                // Test data matching what NewQueryScaffoldTarget passes
                var testData = new
                {
                    language = language,
                    queryPackName = "testpack",
                    queryName = "testquery",
                    description = "Test query description",
                    qlLanguageImport = $"import {language.ToDirectory()}"
                };

                var rendered = template.Render(testData);

                // Basic validation that it's a valid QL structure
                Assert.That(rendered, Does.Contain("import"),
                    $"Generated query for {language} should contain import statement");
                Assert.That(rendered, Does.Contain("from"),
                    $"Generated query for {language} should contain 'from' clause");
                Assert.That(rendered, Does.Contain("select"),
                    $"Generated query for {language} should contain 'select' clause");

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
                "testref.liquid",
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
    }
}
