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
        public string Language { get; set; }
        public string QueryPack { get; set; }
        public string Scope { get; set; }
        public string Name { get; set; }

        public string QuerySrcDir
        {
            get
            {
                return Path.Combine(Base, Language, QueryPack, "src");
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

        public String QueryPackPath
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
                return Path.Combine(Base, Language, QueryPack, "test");
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
                return Path.Combine(QueryFileTestDir, $"{Name}.{GetExtensionForLanguage()}");
            }
        }

        public string QueryFileQLRefPath
        {
            get
            {
                return Path.Combine(QueryFileTestDir, $"{Name}.qlref");
            }
        }

        public String QueryPackTestPath
        {
            get
            {
                return Path.Combine(QueryTestDir, "qlpack.yml");

            }
        }

        public String QueryTestExpectedFile
        {
            get
            {
                return Path.Combine(QueryFileTestDir, $"{Name}.expected");

            }
        }

        public string GetLanguageImportForLangauge()
        {
            if (Language == "cpp" || Language == "c")
            {
                return "cpp";
            }

            throw new NotImplementedException();
        }


        public string GetExtensionForLanguage()
        {
            // todo - refine this. 
            return Language;
        }


    }
}
