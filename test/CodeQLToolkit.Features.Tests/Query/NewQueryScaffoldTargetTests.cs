using CodeQLToolkit.Features.Query.Scaffolding.Targets;
using CodeQLToolkit.Shared.Utils;
using System.IO;

namespace CodeQLToolkit.Features.Tests.Query
{
    [TestFixture]
    public class NewQueryScaffoldTargetTests
    {
        private string tempDirectory;
        private NewQueryScaffoldTarget target;

        [SetUp]
        public void Setup()
        {
            // Create a temporary directory for test outputs
            tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            target = new NewQueryScaffoldTarget
            {
                Base = tempDirectory,
                Language = LanguageType.PYTHON,
                QueryPack = "test-queries",
                QueryPackScope = "test-scope",
                QueryKind = "problem",
                CreateTests = false,
                CreateQueryPack = false,
                OverwriteExisting = true,
                FeatureName = "Query"
            };
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up temporary directory
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Test]
        public void QueryNameParameterIsUsedExactlyAsProvided()
        {
            // Test that custom query names are used exactly as provided without modification
            var customQueryName = "MyCustomQueryName";
            target.Name = customQueryName;

            target.Run();

            // Verify the query file was created with the exact name
            var expectedQueryPath = Path.Combine(tempDirectory, "python", "test-queries", "src", customQueryName, $"{customQueryName}.ql");
            Assert.That(File.Exists(expectedQueryPath), Is.True,
                $"Query file should be created at {expectedQueryPath}");

            // Verify the content uses the exact query name
            var queryContent = File.ReadAllText(expectedQueryPath);
            Assert.That(queryContent, Does.Contain($"@name {customQueryName}"),
                "Query content should use the exact query name in @name annotation");
        }

        [Test]
        public void QueryNameWithSpecialCharactersIsPreserved()
        {
            // Test that query names with numbers, underscores, etc. are preserved
            var specialQueryName = "Test_Query_123";
            target.Name = specialQueryName;

            target.Run();

            var expectedQueryPath = Path.Combine(tempDirectory, "python", "test-queries", "src", specialQueryName, $"{specialQueryName}.ql");
            Assert.That(File.Exists(expectedQueryPath), Is.True,
                $"Query file should be created with special character name at {expectedQueryPath}");

            var queryContent = File.ReadAllText(expectedQueryPath);
            Assert.That(queryContent, Does.Contain($"@name {specialQueryName}"),
                "Query content should preserve special characters in query name");
        }

        [Test]
        public void QueryNameMatchingValidationScriptPattern()
        {
            // Test the exact pattern used by the validation script
            var validationQueryName = "TestQueryKindProblem";
            target.Name = validationQueryName;

            target.Run();

            var expectedQueryPath = Path.Combine(tempDirectory, "python", "test-queries", "src", validationQueryName, $"{validationQueryName}.ql");
            Assert.That(File.Exists(expectedQueryPath), Is.True,
                $"Query file should be created with validation script name at {expectedQueryPath}");

            var queryContent = File.ReadAllText(expectedQueryPath);
            Assert.That(queryContent, Does.Contain($"@name {validationQueryName}"),
                "Query content should use the exact validation script query name");
        }

        [Test]
        public void SimpleQueryNameIsPreserved()
        {
            // Test that simple query names work correctly
            var simpleQueryName = "SimpleQuery";
            target.Name = simpleQueryName;

            target.Run();

            var expectedQueryPath = Path.Combine(tempDirectory, "python", "test-queries", "src", simpleQueryName, $"{simpleQueryName}.ql");
            Assert.That(File.Exists(expectedQueryPath), Is.True,
                $"Query file should be created with simple name at {expectedQueryPath}");

            var queryContent = File.ReadAllText(expectedQueryPath);
            Assert.That(queryContent, Does.Contain($"@name {simpleQueryName}"),
                "Query content should use the exact simple query name");
        }

        [Test]
        public void QueryNameIsUsedInDirectoryStructure()
        {
            // Test that the query name is used to create the proper directory structure
            var queryName = "DirectoryTestQuery";
            target.Name = queryName;

            target.Run();

            // Verify directory structure matches the query name
            var expectedQueryDir = Path.Combine(tempDirectory, "python", "test-queries", "src", queryName);
            Assert.That(Directory.Exists(expectedQueryDir), Is.True,
                $"Query directory should be created at {expectedQueryDir}");

            var expectedQueryFile = Path.Combine(expectedQueryDir, $"{queryName}.ql");
            Assert.That(File.Exists(expectedQueryFile), Is.True,
                $"Query file should be created at {expectedQueryFile}");
        }

        [Test]
        public void QueryNameInTemplateDataIsUnmodified()
        {
            // Test that query name is passed to templates without modification
            var originalQueryName = "OriginalQueryName123";
            target.Name = originalQueryName;

            target.Run();

            var expectedQueryPath = Path.Combine(tempDirectory, "python", "test-queries", "src", originalQueryName, $"{originalQueryName}.ql");
            var queryContent = File.ReadAllText(expectedQueryPath);

            // Check both snake_case and camelCase template variables preserve the original name
            Assert.That(queryContent, Does.Contain($"@name {originalQueryName}"),
                "Template should preserve original query name in @name");

            // Verify @id uses lowercase version (this is template behavior, not target modification)
            Assert.That(queryContent, Does.Contain($"@id python/test-queries/{originalQueryName.ToLower()}"),
                "Template should use lowercase version in @id as expected");
        }

        [Test]
        [TestCase("TestQuery")]
        [TestCase("test_query_name")]
        [TestCase("TestQuery123")]
        [TestCase("ALLCAPS")]
        [TestCase("mixedCASE")]
        [TestCase("TestQueryKindProblem")]
        public void VariousQueryNamesArePreserved(string queryName)
        {
            // Test multiple query name formats to ensure all are preserved exactly
            target.Name = queryName;

            target.Run();

            var expectedQueryPath = Path.Combine(tempDirectory, "python", "test-queries", "src", queryName, $"{queryName}.ql");
            Assert.That(File.Exists(expectedQueryPath), Is.True,
                $"Query file should be created for name '{queryName}' at {expectedQueryPath}");

            var queryContent = File.ReadAllText(expectedQueryPath);
            Assert.That(queryContent, Does.Contain($"@name {queryName}"),
                $"Query content should preserve exact name '{queryName}' in @name annotation");
        }

        [Test]
        public void DifferentLanguagesPreserveQueryName()
        {
            // Test that query name preservation works across different languages
            var queryName = "CrossLanguageTest";
            var testLanguages = new[] { LanguageType.PYTHON, LanguageType.JAVA, LanguageType.CSHARP };

            foreach (var language in testLanguages)
            {
                // Reset target for each language
                target.Language = language;
                target.Name = queryName;

                target.Run();

                var expectedQueryPath = Path.Combine(tempDirectory, language.ToDirectory(), "test-queries", "src", queryName, $"{queryName}.ql");
                Assert.That(File.Exists(expectedQueryPath), Is.True,
                    $"Query file should be created for {language} at {expectedQueryPath}");

                var queryContent = File.ReadAllText(expectedQueryPath);
                Assert.That(queryContent, Does.Contain($"@name {queryName}"),
                    $"Query content for {language} should preserve exact name '{queryName}'");
            }
        }
    }
}
