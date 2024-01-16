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
    }
}
