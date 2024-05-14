using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Utils
{
    public class Query
    {
        public string Base { get; set; }
        public LanguageType Language { get; set; }
        public string QueryPackName { get; set; }
        public string Scope { get; set; }
        public string Name { get; set; }

        public string QuerySrcDir => Path.Combine(Base, Language.ToDirectory(), QueryPackName, "src");

        public string QueryFileDir => Path.Combine(QuerySrcDir, Name);

        public string QueryFilePath => Path.Combine(QueryFileDir, $"{Name}.ql");

        public string QueryPackPath => Path.Combine(QuerySrcDir, "qlpack.yml");

        //
        public string QueryTestDir => Path.Combine(Base, Language.ToDirectory(), QueryPackName, "test");

        public string QueryFileTestDir => Path.Combine(QueryTestDir, Name);

        public string QueryFileTestPath => Path.Combine(QueryFileTestDir, $"{Name}.{Language.ToExtension()}");

        public string QueryFileQLRefPath => Path.Combine(QueryFileTestDir, $"{Name}.qlref");

        public string QueryPackTestPath => Path.Combine(QueryTestDir, "qlpack.yml");

        public string QueryTestExpectedFile => Path.Combine(QueryFileTestDir, $"{Name}.expected");

        public string QueryTestPackName => $"{QueryPackName}-tests";

        public string GetLanguageImportForLangauge()
        {
            return Language.ToImport();
        }
    }
}
