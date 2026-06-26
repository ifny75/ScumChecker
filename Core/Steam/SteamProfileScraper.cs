using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScumChecker.Core.Steam
{
    public static class SteamProfileScraper
    {
        private static readonly HttpClient _http;

        static SteamProfileScraper()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.All
            };

            _http = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            _http.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ScumChecker/1.0");
        }

        public sealed record SteamProfileLite(string SteamId64, string PersonaName, string? AvatarUrl);

        public static async Task<SteamProfileLite?> GetProfileLiteAsync(string steamId64)
        {
            if (string.IsNullOrWhiteSpace(steamId64)) return null;

            var url = $"https://steamcommunity.com/profiles/{steamId64}/?l=english";
            var html = await _http.GetStringAsync(url).ConfigureAwait(false);

            // persona name
            var name = Extract(html, @"<span class=""actual_persona_name"">([^<]+)</span>")
                       ?? Extract(html, @"<meta property=""og:title"" content=""([^""]+)""")
                       ?? steamId64;

            // avatar: обычно есть og:image
            var avatar = Extract(html, @"<meta property=""og:image"" content=""([^""]+)""");

            return new SteamProfileLite(steamId64, WebUtility.HtmlDecode(name).Trim(), avatar);
        }

        private static string? Extract(string html, string pattern)
        {
            var m = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (!m.Success) return null;
            return m.Groups[1].Value;
        }
    }
}
