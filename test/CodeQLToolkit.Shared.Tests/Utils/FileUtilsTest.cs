using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Tests.Utils
{
    public class FileUtilsTest
    {
        [SetUp]
        public void Setup()
        {
        }
 
        [Test]
        public void TestCreateTempDirectory()
        {
            var dir = FileUtils.CreateTempDirectory();           
            Assert.IsTrue(Directory.Exists(dir));
            Assert.IsTrue(dir.StartsWith(Path.GetTempPath()));
        }

        [Test]
        public void TestCreateTempDirectoryWithPath()
        {
            var dir = FileUtils.CreateTempDirectory(Path.GetTempPath());
            Assert.IsTrue(Directory.Exists(dir));
            Assert.IsTrue(dir.StartsWith(Path.GetTempPath()));

        }


        [Test]
        public void TestSanitizeFilename()
        {
            string[] paths = new string[]{ 
                "invalid:#!/\\/path",
                "codeql/cli-1.1.2"
            };

            string[] expected = new string[]{
                "invalid_#!___path",
                "codeql_cli-1.1.2"
            };

            for (int i= 0; i < paths.Length; i++)
            {
                Console.WriteLine(i + "Actual: " + FileUtils.SanitizeFilename(paths[i]));
                Console.WriteLine(i + "Expected: " + expected[i]);

                Assert.IsTrue(FileUtils.SanitizeFilename(paths[i]) == expected[i]);
            }

        }
    }
}
