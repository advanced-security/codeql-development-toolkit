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

        public string QuerySrcDir
        {
            get
            {
                return Path.Combine(Base, Language.ToDirectory(), QueryPackName, "src");
            }
        }

        public string QueryFileDir
        { 
            get
            {
                return Path.Combine(QuerySrcDir, Name);
            }
        }

        public string QueryFilePath
        {
            get
            {
                return Path.Combine(QueryFileDir, $"{Name}.ql");
            }
        }

        public string QueryPackPath
        {
            get
            {
                return Path.Combine(QuerySrcDir, "qlpack.yml");
                
            }
        }

        //
        public string QueryTestDir
        {
            get
            {
                return Path.Combine(Base, Language.ToDirectory(), QueryPackName, "test");
            }
        }

        public string QueryFileTestDir
        {
            get
            {
                return Path.Combine(QueryTestDir, Name);
            }
        }

        public string QueryFileTestPath
        {
            get
            {
                return Path.Combine(QueryFileTestDir, $"{Name}.{Language.ToExtension()}");
            }
        }

        public string QueryFileQLRefPath
        {
            get
            {
                return Path.Combine(QueryFileTestDir, $"{Name}.qlref");
            }
        }

        public string QueryPackTestPath
        {
            get
            {
                return Path.Combine(QueryTestDir, "qlpack.yml");

            }
        }

        public string QueryTestExpectedFile
        {
            get
            {
                return Path.Combine(QueryFileTestDir, $"{Name}.expected");

            }
        }

        public string QueryTestPackName
        {
            get
            {
                return $"{QueryPackName}-tests";
            }
        }

        public string GetLanguageImportForLangauge()
        {
            return Language.ToImport();
        }
    }
}
