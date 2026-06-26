using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScumChecker.Core.Steam
{
    public static class SteamApi
    {
        private static readonly HttpClient _http = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        })
        {
            Timeout = TimeSpan.FromSeconds(12)
        };

        public readonly record struct BanLite(
            bool Unknown,
            bool VacBanned,
            int VacBans,
            int GameBans,
            int? DaysSinceLastBan
        )
        {
            public static BanLite UnknownResult() => new(true, false, 0, 0, null);
        }

        /// <summary>
        /// Без API key:
        /// 1) Пробуем XML: https://steamcommunity.com/profiles/{id}/?xml=1
        /// 2) Если нужно — добираем GameBan/Days из HTML по блоку profile_ban_status
        /// </summary>
        public static async Task<BanLite> GetBansNoKeyAsync(string steamId64)
        {
            if (string.IsNullOrWhiteSpace(steamId64))
                return BanLite.UnknownResult();

            EnsureUserAgent();

            // 1) XML
            var xmlUrl = $"https://steamcommunity.com/profiles/{steamId64}/?xml=1";
            string xml;
            try { xml = await _http.GetStringAsync(xmlUrl).ConfigureAwait(false); }
            catch { return BanLite.UnknownResult(); }

            if (string.IsNullOrWhiteSpace(xml))
                return BanLite.UnknownResult();

            var vacBanned = TryTagBool(xml, "vacBanned");
            if (vacBanned is null)
            {
                // XML не дал вообще ничего (private/blocked)
                // попробуем хотя бы HTML (может дать game ban/days)
                var htmlOnly = await TryParseGameBanFromHtmlAsync(steamId64).ConfigureAwait(false);
                return htmlOnly.Unknown ? BanLite.UnknownResult() : htmlOnly;
            }

            var vacCount = TryTagInt(xml, "numberOfVACBans") ?? 0;
            var gameBans = TryTagInt(xml, "numberOfGameBans") ?? 0;
            var days = TryTagInt(xml, "daysSinceLastBan");

            var result = new BanLite(
                Unknown: false,
                VacBanned: vacBanned.Value,
                VacBans: vacCount,
                GameBans: gameBans,
                DaysSinceLastBan: days
            );

            // 2) Если game bans / days пустые — доберём из HTML блока profile_ban_status
            if (result.GameBans == 0 && result.DaysSinceLastBan is null)
            {
                var htmlExtra = await TryParseGameBanFromHtmlAsync(steamId64).ConfigureAwait(false);
                if (!htmlExtra.Unknown)
                {
                    result = result with
                    {
                        GameBans = htmlExtra.GameBans != 0 ? htmlExtra.GameBans : result.GameBans,
                        DaysSinceLastBan = htmlExtra.DaysSinceLastBan ?? result.DaysSinceLastBan
                    };
                }
            }

            return result;
        }

        private static void EnsureUserAgent()
        {
            if (_http.DefaultRequestHeaders.UserAgent.Count == 0)
            {
                // Steam иногда режет без нормального UA
                _http.DefaultRequestHeaders.UserAgent.TryParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ScumChecker/1.0"
                );
            }
        }

        // =======================
        // HTML парс блока банов
        // =======================

        // Ищем именно блок profile_ban_status (как ты скинул)
        private static readonly Regex RxBanStatusBlock = new Regex(
            @"<div\s+class=""profile_ban_status"">(?<body>[\s\S]*?)</div>\s*</div>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // RU: "1 игровая блокировка"
        private static readonly Regex RxGameBanRu = new Regex(
            @"(?<!\d)(?<n>\d+)\s+игров\w*\s+блокировк\w*",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // EN: "1 game ban" / "2 game bans"
        private static readonly Regex RxGameBanEn = new Regex(
            @"(?<!\d)(?<n>\d+)\s+game\s+ban",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // RU: "Дней с последней блокировки: 1814"
        private static readonly Regex RxDaysRu = new Regex(
            @"Дней\s+с\s+последн\w*\s+блокировк\w*:\s*(?<n>\d+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // EN: "Days since last ban: 1814"
        private static readonly Regex RxDaysEn = new Regex(
            @"Days\s+since\s+last\s+ban:\s*(?<n>\d+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        private static async Task<BanLite> TryParseGameBanFromHtmlAsync(string steamId64)
        {
            EnsureUserAgent();

            var url = $"https://steamcommunity.com/profiles/{steamId64}/";
            string html;
            try { html = await _http.GetStringAsync(url).ConfigureAwait(false); }
            catch { return BanLite.UnknownResult(); }

            if (string.IsNullOrWhiteSpace(html))
                return BanLite.UnknownResult();

            // decode html entities (например &amp;)
            html = WebUtility.HtmlDecode(html);

            var m = RxBanStatusBlock.Match(html);

            // если блока вообще нет — значит game bans нет (обычно)
            if (!m.Success)
                return new BanLite(Unknown: false, VacBanned: false, VacBans: 0, GameBans: 0, DaysSinceLastBan: null);

            var body = m.Groups["body"].Value;

            int gameBans = 0;
            int? days = null;

            var gRu = RxGameBanRu.Match(body);
            var gEn = RxGameBanEn.Match(body);

            if (gRu.Success && int.TryParse(gRu.Groups["n"].Value, out var n1)) gameBans = n1;
            else if (gEn.Success && int.TryParse(gEn.Groups["n"].Value, out var n2)) gameBans = n2;

            var dRu = RxDaysRu.Match(body);
            var dEn = RxDaysEn.Match(body);

            if (dRu.Success && int.TryParse(dRu.Groups["n"].Value, out var d1)) days = d1;
            else if (dEn.Success && int.TryParse(dEn.Groups["n"].Value, out var d2)) days = d2;

            return new BanLite(
                Unknown: false,
                VacBanned: false, // VAC мы тут НЕ выводим, это отдельная часть
                VacBans: 0,
                GameBans: gameBans,
                DaysSinceLastBan: days
            );
        }

        // =======================
        // XML helpers
        // =======================

        private static bool? TryTagBool(string xml, string tag)
        {
            var v = TryTagString(xml, tag);
            if (v == null) return null;
            v = v.Trim();
            if (v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase)) return true;
            if (v == "0" || v.Equals("false", StringComparison.OrdinalIgnoreCase)) return false;
            return null;
        }

        private static int? TryTagInt(string xml, string tag)
        {
            var v = TryTagString(xml, tag);
            if (v == null) return null;
            if (int.TryParse(v.Trim(), out var n)) return n;
            return null;
        }

        private static string? TryTagString(string xml, string tag)
        {
            var m = Regex.Match(xml, $@"\<{tag}\>\s*(.*?)\s*\</{tag}\>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (!m.Success) return null;
            return m.Groups[1].Value;
        }
    }
}
