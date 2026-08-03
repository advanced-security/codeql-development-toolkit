using CodeQLToolkit.Shared.CodeQL;
using System.Net;
using System.Text;

namespace CodeQLToolkit.Shared.Tests.CodeQL
{
    public class GitHubReleaseResolverTests
    {
        // Fake HttpMessageHandler that returns a fixed response.
        private class StubHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _statusCode;
            private readonly string _body;

            public StubHandler(HttpStatusCode statusCode, string body)
            {
                _statusCode = statusCode;
                _body = body;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(_statusCode)
                {
                    Content = new StringContent(_body, Encoding.UTF8, "application/json")
                };
                return Task.FromResult(response);
            }
        }

        private static HttpClient MakeClient(HttpStatusCode status, string body)
            => GitHubReleaseResolver.CreateTestClient(new StubHandler(status, body));

        private const string CliResponse = "{\"tag_name\":\"v2.99.0\",\"name\":\"v2.99.0\"}";
        private const string BundleResponse = "{\"tag_name\":\"codeql-bundle-v3.28.5\",\"name\":\"codeql-bundle-v3.28.5\"}";

        // ── GetLatestCLIVersion ──────────────────────────────────────────────

        [Test]
        public void GetLatestCLIVersion_ParsesTagAndStripsV()
        {
            var client = MakeClient(HttpStatusCode.OK, CliResponse);
            var result = GitHubReleaseResolver.GetLatestCLIVersion(client);
            Assert.That(result, Is.EqualTo("2.99.0"));
        }

        [Test]
        public void GetLatestCLIVersion_ReturnsFallback_OnNetworkError()
        {
            var client = MakeClient(HttpStatusCode.ServiceUnavailable, "");
            var result = GitHubReleaseResolver.GetLatestCLIVersion(client);
            Assert.That(result, Is.EqualTo(GitHubReleaseResolver.FallbackCLIVersion));
        }

        [Test]
        public void GetLatestCLIVersion_ReturnsFallback_OnMalformedJson()
        {
            var client = MakeClient(HttpStatusCode.OK, "not-json");
            var result = GitHubReleaseResolver.GetLatestCLIVersion(client);
            Assert.That(result, Is.EqualTo(GitHubReleaseResolver.FallbackCLIVersion));
        }

        [Test]
        public void GetLatestCLIVersion_ReturnsFallback_OnMissingTagName()
        {
            var client = MakeClient(HttpStatusCode.OK, "{\"name\":\"v2.99.0\"}");
            var result = GitHubReleaseResolver.GetLatestCLIVersion(client);
            Assert.That(result, Is.EqualTo(GitHubReleaseResolver.FallbackCLIVersion));
        }

        // ── GetLatestStandardLibraryVersion ─────────────────────────────────

        [Test]
        public void GetLatestStandardLibraryVersion_FormatsCorrectly()
        {
            var client = MakeClient(HttpStatusCode.OK, CliResponse);
            var result = GitHubReleaseResolver.GetLatestStandardLibraryVersion(client);
            Assert.That(result, Is.EqualTo("codeql-cli/v2.99.0"));
        }

        [Test]
        public void GetLatestStandardLibraryVersion_ReturnsFallback_OnNetworkError()
        {
            var client = MakeClient(HttpStatusCode.InternalServerError, "");
            var result = GitHubReleaseResolver.GetLatestStandardLibraryVersion(client);
            Assert.That(result, Is.EqualTo(GitHubReleaseResolver.FallbackStandardLibraryVersion));
        }

        [Test]
        public void GetLatestStandardLibraryVersion_ReturnsFallback_OnMalformedJson()
        {
            var client = MakeClient(HttpStatusCode.OK, "not-json");
            var result = GitHubReleaseResolver.GetLatestStandardLibraryVersion(client);
            Assert.That(result, Is.EqualTo(GitHubReleaseResolver.FallbackStandardLibraryVersion));
        }

        // ── GetLatestBundleVersion ───────────────────────────────────────────

        [Test]
        public void GetLatestBundleVersion_FormatsCorrectly()
        {
            var client = MakeClient(HttpStatusCode.OK, BundleResponse);
            var result = GitHubReleaseResolver.GetLatestBundleVersion(client);
            Assert.That(result, Is.EqualTo("codeql-bundle-v3.28.5"));
        }

        [Test]
        public void GetLatestBundleVersion_ReturnsFallback_OnNetworkError()
        {
            var client = MakeClient(HttpStatusCode.ServiceUnavailable, "");
            var result = GitHubReleaseResolver.GetLatestBundleVersion(client);
            Assert.That(result, Is.EqualTo(GitHubReleaseResolver.FallbackBundleVersion));
        }

        [Test]
        public void GetLatestBundleVersion_ReturnsFallback_OnMalformedJson()
        {
            var client = MakeClient(HttpStatusCode.OK, "not-json");
            var result = GitHubReleaseResolver.GetLatestBundleVersion(client);
            Assert.That(result, Is.EqualTo(GitHubReleaseResolver.FallbackBundleVersion));
        }

        [Test]
        public void GetLatestBundleVersion_ReturnsFallback_OnMissingTagName()
        {
            var client = MakeClient(HttpStatusCode.OK, "{\"name\":\"codeql-bundle-v3.28.5\"}");
            var result = GitHubReleaseResolver.GetLatestBundleVersion(client);
            Assert.That(result, Is.EqualTo(GitHubReleaseResolver.FallbackBundleVersion));
        }

        // ── Fallback value format sanity checks ──────────────────────────────

        [Test]
        public void FallbackCLIVersion_HasCorrectFormat()
        {
            // Should be X.Y.Z with no leading 'v'
            var parts = GitHubReleaseResolver.FallbackCLIVersion.Split('.');
            Assert.That(parts.Length, Is.EqualTo(3));
            Assert.That(GitHubReleaseResolver.FallbackCLIVersion, Does.Not.StartWith("v"));
        }

        [Test]
        public void FallbackStandardLibraryVersion_HasCorrectFormat()
        {
            Assert.That(GitHubReleaseResolver.FallbackStandardLibraryVersion, Does.StartWith("codeql-cli/v"));
        }

        [Test]
        public void FallbackBundleVersion_HasCorrectFormat()
        {
            Assert.That(GitHubReleaseResolver.FallbackBundleVersion, Does.StartWith("codeql-bundle-v"));
        }
    }
}
