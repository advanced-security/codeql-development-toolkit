using System.Runtime.InteropServices;
using CodeQLToolkit.Shared.CodeQL;

namespace CodeQLToolkit.Shared.Tests.CodeQL
{
    public class CodeQLInstallationTests
    {
        [Test]
        public void UsesCurrentPlatformBundle()
        {
            var expectedPlatform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "win64"
                : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                    ? "linux64"
                    : "osx64";
            var installation = new CodeQLInstallation
            {
                CLIBundle = "test",
                EnableCustomCodeQLBundles = true
            };

            Assert.That(
                Path.GetFileName(installation.CustomBundleOutputBundleCurrentPlatform),
                Is.EqualTo($"codeql-bundle-{expectedPlatform}.tar.gz"));
        }
    }
}
