using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeQLToolkit.Shared.CodeQL
{
    public enum ArtifactKind { BUNDLE, CUSTOM_BUNDLE, PACKAGE };

    public class InstallationRepository
    {
        readonly static string PAKCAGE_DIRECTORY = "packages";
        readonly static string BUNDLE_DIRECTORY = "bundle";
        readonly static string CUSTOM_BUNDLE_DIRECTORY = "custom-bundle";

        public static string GetLocation => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".qlt");

        public static string PackageLocation => Path.Combine(GetLocation, PAKCAGE_DIRECTORY);

        public static string BundleLocation => Path.Combine(GetLocation, BUNDLE_DIRECTORY);

        public static string CustomBundleLocation => Path.Combine(GetLocation, CUSTOM_BUNDLE_DIRECTORY);


        public static string DirectoryForVersion(ArtifactKind kind, string version)
        {
            if (kind == ArtifactKind.PACKAGE)
            {
                return Path.Combine(PackageLocation, version);
            }
            else if (kind == ArtifactKind.BUNDLE)
            {
                return Path.Combine(BundleLocation, version);
            }
            else
            {
                return Path.Combine(CustomBundleLocation, version);
            }
        }
    }
}
