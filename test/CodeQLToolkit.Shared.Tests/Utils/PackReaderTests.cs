using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Tests.Utils
{
    public class PackReaderTests
    {
        public string TestFile { get; set; }
        [SetUp]
        public void Setup()
        {
            var doc = @"
---
library: true
name: qlt2/stuff2
version: 0.0.1
description: Default description
suites: 
license: 
dependencies:
  codeql/cpp-all: ""0.12.2""
";

            TestFile = Path.Combine(Path.GetTempPath(), "qlpack.yml");

            File.WriteAllText(TestFile, doc);
        }

        [Test]
        public void TestReadPackName()
        {
            Assert.AreEqual("qlt2/stuff2", CodeQLPackReader.read(TestFile).Name);
        }
    }
}
