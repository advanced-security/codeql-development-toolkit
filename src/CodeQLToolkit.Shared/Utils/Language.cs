using CodeQLToolkit.Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Utils
{
    public enum LanguageType
    {
        C,
        CPP,
        JAVASCRIPT,
        GO,
        RUBY,
        PYTHON,
        JAVA,
        CSHARP
    }

    public static class LanguageTypeMethods
    {
        public static LanguageType FromOptionString(this LanguageType LanguageType, string value)
        {
            if (value.ToLower().Equals("c"))
            {
                return LanguageType.C;
            }

            if (value.ToLower().Equals("cpp"))
            {
                return LanguageType.CPP;
            }

            if (value.ToLower().Equals("javascript"))
            {
                return LanguageType.JAVASCRIPT;
            }

            if (value.ToLower().Equals("go"))
            {
                return LanguageType.GO;
            }

            if (value.ToLower().Equals("ruby"))
            {
                return LanguageType.RUBY;
            }

            if (value.ToLower().Equals("java"))
            {
                return LanguageType.JAVA;
            }

            if (value.ToLower().Equals("csharp"))
            {
                return LanguageType.CSHARP;
            }

            if (value.ToLower().Equals("python"))
            {
                return LanguageType.PYTHON;
            }

            throw new NotImplementedException();
        }


        public static string ToOptionString(this LanguageType LanguageType)
        {
            if (LanguageType == LanguageType.C)
            {
                return "c";
            }

            if (LanguageType == LanguageType.CPP)
            {
                return "cpp";
            }

            if (LanguageType == LanguageType.JAVASCRIPT)
            {
                return "javascript";
            }
            if (LanguageType == LanguageType.GO)
            {
                return "go";
            }
            if (LanguageType == LanguageType.RUBY)
            {
                return "ruby";
            }
            if (LanguageType == LanguageType.PYTHON)
            {
                return "python";
            }
            if (LanguageType == LanguageType.JAVA)
            {
                return "java";
            }
            if (LanguageType == LanguageType.CSHARP)
            {
                return "csharp";
            }

            throw new NotImplementedException();
        }


        public static string ToDirectory(this LanguageType LanguageType)
        {
            if (LanguageType == LanguageType.C)
            {
                return "cpp";
            }

            if (LanguageType == LanguageType.CPP)
            {
                return "cpp";
            }

            if (LanguageType == LanguageType.JAVASCRIPT)
            {
                return "javascript";
            }
            if (LanguageType == LanguageType.GO)
            {
                return "go";
            }
            if (LanguageType == LanguageType.RUBY)
            {
                return "ruby";
            }
            if (LanguageType == LanguageType.PYTHON)
            {
                return "python";
            }
            if (LanguageType == LanguageType.JAVA)
            {
                return "java";
            }
            if (LanguageType == LanguageType.CSHARP)
            {
                return "csharp";
            }

            throw new NotImplementedException();
        }

        public static string ToImport(this LanguageType LanguageType)
        {
            if (LanguageType == LanguageType.C)
            {
                return "cpp";
            }

            if (LanguageType == LanguageType.CPP)
            {
                return "cpp";
            }

            if (LanguageType == LanguageType.JAVASCRIPT)
            {
                return "javascript";
            }
            if (LanguageType == LanguageType.GO)
            {
                return "go";
            }
            if (LanguageType == LanguageType.RUBY)
            {
                return "ruby";
            }
            if (LanguageType == LanguageType.PYTHON)
            {
                return "python";
            }
            if (LanguageType == LanguageType.JAVA)
            {
                return "java";
            }
            if (LanguageType == LanguageType.CSHARP)
            {
                return "csharp";
            }

            throw new NotImplementedException();
        }


        public static string ToExtension(this LanguageType LanguageType)
        {
            if (LanguageType == LanguageType.C)
            {
                return "c";
            }

            if (LanguageType == LanguageType.CPP)
            {
                return "cpp";
            }

            if (LanguageType == LanguageType.JAVASCRIPT)
            {
                return "js";
            }
            if (LanguageType == LanguageType.GO)
            {
                return "go";
            }
            if (LanguageType == LanguageType.RUBY)
            {
                return "rb";
            }
            if (LanguageType == LanguageType.PYTHON)
            {
                return "py";
            }
            if (LanguageType == LanguageType.JAVA)
            {
                return "java";
            }
            if (LanguageType == LanguageType.CSHARP)
            {
                return "cs";
            }


            throw new NotImplementedException();
        }

        /// <summary>
        /// Sanitizes a name to be safe for use as a test directory/module name for the specific language.
        /// Different languages have different naming restrictions for modules, packages, and classes.
        /// </summary>
        /// <param name="languageType">The language type</param>
        /// <param name="name">The original name to sanitize</param>
        /// <returns>A sanitized name safe for the language</returns>
        public static string ToSafeTestName(this LanguageType languageType, string name)
        {
            // For most languages that use module/package systems, hyphens are problematic
            // and should be replaced with underscores
            switch (languageType)
            {
                case LanguageType.PYTHON:
                case LanguageType.RUBY:
                case LanguageType.GO:
                    // These languages have strict module naming rules
                    return name.Replace("-", "_");

                case LanguageType.JAVA:
                case LanguageType.CSHARP:
                    // Java and C# package/namespace names can't contain hyphens
                    return name.Replace("-", "_");

                case LanguageType.JAVASCRIPT:
                    // JavaScript module names in some contexts can't contain hyphens
                    return name.Replace("-", "_");

                case LanguageType.C:
                case LanguageType.CPP:
                    // C/C++ are more flexible with file names, but underscores are safer
                    return name.Replace("-", "_");

                default:
                    throw new NotImplementedException();
            }
        }

    }


    public class LanguageTypeHelper
    {
        public static LanguageType LanguageTypeFromOptionString(string LanguageType)
        {
            return new LanguageType().FromOptionString(LanguageType);
        }
    }

}
