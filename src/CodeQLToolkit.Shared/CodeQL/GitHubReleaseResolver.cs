using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace CodeQLToolkit.Shared.CodeQL
{
    /// <summary>
    /// Resolves the latest release versions for CodeQL CLI, standard library, and bundle
    /// from GitHub's public API. Falls back to hardcoded values if the request fails.
    /// </summary>
    public static class GitHubReleaseResolver
    {
        public const string FallbackCLIVersion = "2.25.1";
        public const string FallbackStandardLibraryVersion = "codeql-cli/v2.25.1";
        public const string FallbackBundleVersion = "codeql-bundle-v2.25.1";

        private static readonly HttpClient _client = CreateClient(null);

        private static HttpClient CreateClient(HttpMessageHandler? handler)
        {
            var client = handler != null ? new HttpClient(handler) : new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("qlt", "1.0"));
            client.Timeout = TimeSpan.FromSeconds(5);
            return client;
        }

        /// <summary>
        /// Fetches the latest CodeQL CLI version string (e.g. "2.25.1").
        /// Falls back to <see cref="FallbackCLIVersion"/> on any error.
        /// </summary>
        public static string GetLatestCLIVersion() => GetLatestCLIVersion(_client);

        /// <summary>
        /// Fetches the latest standard library version string (e.g. "codeql-cli/v2.25.1").
        /// Falls back to <see cref="FallbackStandardLibraryVersion"/> on any error.
        /// </summary>
        public static string GetLatestStandardLibraryVersion() => GetLatestStandardLibraryVersion(_client);

        /// <summary>
        /// Fetches the latest bundle version string (e.g. "codeql-bundle-v2.25.1").
        /// Falls back to <see cref="FallbackBundleVersion"/> on any error.
        /// </summary>
        public static string GetLatestBundleVersion() => GetLatestBundleVersion(_client);

        // Internal overloads for testing — accept a custom HttpClient.

        internal static string GetLatestCLIVersion(HttpClient client)
        {
            try
            {
                var tag = GetLatestTagName(client, "https://api.github.com/repos/github/codeql-cli-binaries/releases/latest");
                return tag.TrimStart('v');
            }
            catch
            {
                return FallbackCLIVersion;
            }
        }

        internal static string GetLatestStandardLibraryVersion(HttpClient client)
        {
            try
            {
                var tag = GetLatestTagName(client, "https://api.github.com/repos/github/codeql-cli-binaries/releases/latest");
                return $"codeql-cli/v{tag.TrimStart('v')}";
            }
            catch
            {
                return FallbackStandardLibraryVersion;
            }
        }

        internal static string GetLatestBundleVersion(HttpClient client)
        {
            try
            {
                // codeql-action release tags are already in the form "codeql-bundle-vX.Y.Z"
                return GetLatestTagName(client, "https://api.github.com/repos/github/codeql-action/releases/latest");
            }
            catch
            {
                return FallbackBundleVersion;
            }
        }

        internal static HttpClient CreateTestClient(HttpMessageHandler handler) => CreateClient(handler);

        private static string GetLatestTagName(HttpClient client, string url)
        {
            var response = client.GetAsync(url).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var obj = JObject.Parse(json);
            return obj["tag_name"]!.Value<string>()!;
        }
    }
}
