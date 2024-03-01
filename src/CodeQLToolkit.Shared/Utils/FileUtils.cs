using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.Utils
{
    public class FileUtils
    {
        public static DirectoryInfo GetExecutingDirectory()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory;
        }

        public static string CreateTempDirectory()
        {
            return CreateTempDirectory(Path.GetTempPath());
        }

        public static string CreateTempDirectory(string baseDir)
        {
            string tempDirectory = Path.Combine(baseDir, Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        public static string SanitizeFilename(string filename)
        {
            string f = filename;

            foreach(var c in Path.GetInvalidFileNameChars())
            {
                f = f.Replace(c, '_');
            }

            return f;
        }

    }
}
