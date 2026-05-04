using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;
using ScumChecker.Core;

namespace ScumChecker.Core.Modules
{
    public sealed class SteamAccountsModule : IScanModule
    {
        public string Name => "Steam (loginusers + userdata history)";

        private const long SteamId64Base = 76561197960265728L;

        private sealed class SteamAccountInfo
        {
            public string SteamId64 { get; set; } = "";
            public string AccountName { get; set; } = "";
            public string PersonaName { get; set; } = "";
            public string MostRecentRaw { get; set; } = "";
            public string TimestampRaw { get; set; } = "";
            public DateTimeOffset? LoginTimeUtc { get; set; }
            public string LoginUsersPath { get; set; } = "";
            public string UserDataPath { get; set; } = "";
            public DateTimeOffset? UserDataWriteUtc { get; set; }
            public DateTimeOffset? ScumLastPlayedUtc { get; set; }
            public DateTimeOffset? LocalConfigWriteUtc { get; set; }
            public bool SeenInUserData { get; set; }
        }

        public IEnumerable<ScanItem> Run(CancellationToken ct)
        {
            var roots = DiscoverSteamRoots().ToList();
            if (roots.Count == 0)
                yield break;

            var bySteamId = new Dictionary<string, SteamAccountInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var root in roots)
            {
                ct.ThrowIfCancellationRequested();

                var loginUsersPath = Path.Combine(root, "config", "loginusers.vdf");
                if (File.Exists(loginUsersPath))
                {
                    string text;
                    ScanItem? readError = null;
                    try
                    {
                        text = File.ReadAllText(loginUsersPath);
                    }
                    catch (Exception ex)
                    {
                        text = "";
                        readError = new ScanItem
                        {
                            Severity = Severity.Low,
                            Group = FindingGroup.SystemInfo,
                            Category = "Steam",
                            What = "Read error",
                            Reason = "Could not read loginusers.vdf",
                            Recommendation = "Run as admin or check permissions.",
                            Details = ex.Message,
                            EvidencePath = loginUsersPath
                        };
                    }

                    if (readError != null)
                    {
                        yield return readError;
                        continue;
                    }

                    foreach (Match m in Regex.Matches(text, "\"(?<id>7656\\d{13,17})\"\\s*\\{(?<body>[\\s\\S]*?)\\}", RegexOptions.Multiline))
                    {
                        ct.ThrowIfCancellationRequested();

                        var id = m.Groups["id"].Value.Trim();
                        var body = m.Groups["body"].Value;
                        if (string.IsNullOrWhiteSpace(id)) continue;

                        if (!bySteamId.TryGetValue(id, out var info))
                        {
                            info = new SteamAccountInfo { SteamId64 = id };
                            bySteamId[id] = info;
                        }

                        info.AccountName = FirstNonEmpty(info.AccountName, GetVdfValue(body, "AccountName"));
                        info.PersonaName = FirstNonEmpty(info.PersonaName, GetVdfValue(body, "PersonaName"));
                        info.MostRecentRaw = FirstNonEmpty(info.MostRecentRaw, GetVdfValue(body, "MostRecent"));
                        info.TimestampRaw = FirstNonEmpty(info.TimestampRaw, GetVdfValue(body, "Timestamp"));
                        info.LoginUsersPath = FirstNonEmpty(info.LoginUsersPath, loginUsersPath);

                        if (TryParseUnix(info.TimestampRaw, out var ts))
                            info.LoginTimeUtc ??= ts;
                    }
                }

                var userDataRoot = Path.Combine(root, "userdata");
                if (!Directory.Exists(userDataRoot))
                    continue;

                IEnumerable<string> dirs;
                try { dirs = Directory.EnumerateDirectories(userDataRoot); }
                catch { continue; }

                foreach (var d in dirs)
                {
                    ct.ThrowIfCancellationRequested();

                    var name = Path.GetFileName(d);
                    if (!long.TryParse(name, out var accountId32) || accountId32 <= 0)
                        continue;

                    var steamId64 = (SteamId64Base + accountId32).ToString();
                    if (!bySteamId.TryGetValue(steamId64, out var info))
                    {
                        info = new SteamAccountInfo { SteamId64 = steamId64 };
                        bySteamId[steamId64] = info;
                    }

                    info.SeenInUserData = true;
                    info.UserDataPath = FirstNonEmpty(info.UserDataPath, d);

                    try
                    {
                        var w = Directory.GetLastWriteTimeUtc(d);
                        if (w.Year > 2000)
                        {
                            var wdt = new DateTimeOffset(w);
                            if (info.UserDataWriteUtc is null || wdt > info.UserDataWriteUtc.Value)
                                info.UserDataWriteUtc = wdt;
                        }
                    }
                    catch { }

                    var localConfig = Path.Combine(d, "config", "localconfig.vdf");
                    if (File.Exists(localConfig))
                    {
                        try
                        {
                            var cfgWrite = File.GetLastWriteTimeUtc(localConfig);
                            if (cfgWrite.Year > 2000)
                            {
                                var cfgDt = new DateTimeOffset(cfgWrite);
                                if (info.LocalConfigWriteUtc is null || cfgDt > info.LocalConfigWriteUtc.Value)
                                    info.LocalConfigWriteUtc = cfgDt;
                            }
                        }
                        catch { }

                        string cfgText = "";
                        try { cfgText = File.ReadAllText(localConfig); } catch { }

                        var lastPlayed = TryExtractScumLastPlayed(cfgText);
                        if (lastPlayed is not null &&
                            (info.ScumLastPlayedUtc is null || lastPlayed.Value > info.ScumLastPlayedUtc.Value))
                        {
                            info.ScumLastPlayedUtc = lastPlayed;
                        }
                    }
                }
            }

            if (bySteamId.Count == 0)
                yield break;

            var ordered = bySteamId.Values
                .OrderByDescending(x => x.MostRecentRaw == "1" || x.MostRecentRaw.Equals("true", StringComparison.OrdinalIgnoreCase))
                .ThenByDescending(x => x.LoginTimeUtc ?? DateTimeOffset.MinValue)
                .ThenByDescending(x => x.UserDataWriteUtc ?? DateTimeOffset.MinValue)
                .ToList();

            foreach (var x in ordered)
            {
                ct.ThrowIfCancellationRequested();

                var persona = string.IsNullOrWhiteSpace(x.PersonaName) ? "Unknown persona" : x.PersonaName;
                var account = string.IsNullOrWhiteSpace(x.AccountName) ? "unknown" : x.AccountName;
                var tsRaw = string.IsNullOrWhiteSpace(x.TimestampRaw) ? "-" : x.TimestampRaw;

                var historyBits = new List<string>
                {
                    $"{persona} ({account})",
                    $"MostRecent={x.MostRecentRaw}",
                    $"Timestamp={tsRaw}"
                };

                if (x.LoginTimeUtc is not null)
                    historyBits.Add($"LoginTimeLocal={x.LoginTimeUtc.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss}");

                historyBits.Add($"UserDataSeen={(x.SeenInUserData ? "1" : "0")}");

                if (x.UserDataWriteUtc is not null)
                    historyBits.Add($"UserDataWriteLocal={x.UserDataWriteUtc.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss}");

                if (x.LocalConfigWriteUtc is not null)
                    historyBits.Add($"LocalConfigWriteLocal={x.LocalConfigWriteUtc.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss}");

                if (x.ScumLastPlayedUtc is not null)
                    historyBits.Add($"ScumLastPlayedLocal={x.ScumLastPlayedUtc.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss}");

                yield return new ScanItem
                {
                    Severity = Severity.Info,
                    Group = FindingGroup.SystemInfo,
                    Category = "Steam",
                    What = "Steam account",
                    Reason = "Found in Steam local artifacts (loginusers.vdf / userdata)",
                    Recommendation = "Open profile to review.",
                    Details = string.Join(" | ", historyBits),
                    EvidencePath = !string.IsNullOrWhiteSpace(x.LoginUsersPath) ? x.LoginUsersPath : x.UserDataPath,
                    Url = "https://steamcommunity.com/profiles/" + x.SteamId64
                };

                if (x.SeenInUserData || x.ScumLastPlayedUtc is not null)
                {
                    yield return new ScanItem
                    {
                        Severity = Severity.Info,
                        Group = FindingGroup.SystemInfo,
                        Category = "Steam",
                        What = "Steam account history",
                        Reason = "Additional local account history artifacts found",
                        Recommendation = "Use with loginusers data for timeline reconstruction.",
                        Details =
                            $"SteamID={x.SteamId64} | UserDataPath={x.UserDataPath} | " +
                            $"LocalConfigWrite={(x.LocalConfigWriteUtc is null ? "-" : x.LocalConfigWriteUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"))} | " +
                            $"SCUM LastPlayed={(x.ScumLastPlayedUtc is null ? "-" : x.ScumLastPlayedUtc.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"))}",
                        EvidencePath = x.UserDataPath
                    };
                }
            }
        }

        private static IEnumerable<string> DiscoverSteamRoots()
        {
            var roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void AddIfValid(string? p)
            {
                if (string.IsNullOrWhiteSpace(p)) return;
                try
                {
                    var full = Path.GetFullPath(p.Trim().Trim('"'));
                    if (Directory.Exists(full))
                        roots.Add(full);
                }
                catch { }
            }

            AddIfValid(@"C:\Program Files (x86)\Steam");
            AddIfValid(@"C:\Program Files\Steam");
            AddIfValid(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam"));

            try
            {
                using var hkcu = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam", false);
                if (hkcu != null)
                {
                    AddIfValid(hkcu.GetValue("SteamPath")?.ToString());
                    var steamExe = hkcu.GetValue("SteamExe")?.ToString();
                    if (!string.IsNullOrWhiteSpace(steamExe))
                    {
                        try { AddIfValid(Path.GetDirectoryName(steamExe)); } catch { }
                    }
                }
            }
            catch { }

            try
            {
                using var hklm = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam", false);
                if (hklm != null)
                    AddIfValid(hklm.GetValue("InstallPath")?.ToString());
            }
            catch { }

            return roots;
        }

        private static string GetVdfValue(string body, string key)
        {
            var mm = Regex.Match(body, $"\"{Regex.Escape(key)}\"\\s*\"(?<v>.*?)\"", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return mm.Success ? mm.Groups["v"].Value : "";
        }

        private static string FirstNonEmpty(string current, string next)
            => !string.IsNullOrWhiteSpace(current) ? current : (next ?? "");

        private static bool TryParseUnix(string raw, out DateTimeOffset ts)
        {
            ts = default;
            if (!long.TryParse(raw, out var n)) return false;
            if (n <= 0) return false;
            try
            {
                ts = DateTimeOffset.FromUnixTimeSeconds(n);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static DateTimeOffset? TryExtractScumLastPlayed(string localConfigText)
        {
            if (string.IsNullOrWhiteSpace(localConfigText))
                return null;

            var appBlock = Regex.Match(
                localConfigText,
                "\"513710\"\\s*\\{(?<body>[\\s\\S]*?)\\}",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (!appBlock.Success)
                return null;

            var body = appBlock.Groups["body"].Value;
            var m = Regex.Match(
                body,
                "\"LastPlayed\"\\s*\"(?<v>\\d{6,})\"",
                RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (!m.Success)
                return null;

            if (!TryParseUnix(m.Groups["v"].Value, out var ts))
                return null;

            return ts;
        }
    }
}
