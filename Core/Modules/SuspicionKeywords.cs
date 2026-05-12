using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;

namespace ScumChecker.Core.Modules
{
    public static class SuspicionKeywords
    {
        // ========== Ключевые слова (более строгие, меньше ложных)
        public static readonly string[] Generic =
        [
            // injection / drivers / kernel
            "injector", "injection", "dll injector", "manual map", "manualmap",
            "mapper", "kdmapper", "drvmap", "driver mapper", "kernel driver",
            "ring0", "ring 0", "kernelmode", "kernel mode",
            "vulnerable driver", "rtcore64", "gdrv", "asrdrv", "capcom",

            // bypass / spoof / hwid
            "bypass", "anti-cheat bypass", "anticheat bypass",
            "spoofer", "spoofing", "hwid spoofer", "hwid spoof",
            "serial spoof", "disk serial", "volumeid", "smbios", "mac spoof",

            // hooks / overlays
            "hooking", "setwindowshookex", "detour", "detours", "minhook",
            "present hook", "d3d hook", "dxgi hook", "swapchain",
            "imgui", "dear imgui",

            // cheat features
            "aimbot", "triggerbot", "silentaim", "silent aim",
            "wallhack", "wh", "esp", "radarhack", "radar hack",
            "chams", "norecoil", "no recoil", "nospread", "no spread",
            "bhop", "bunnyhop", "speedhack", "flyhack",
            "noclip", "freecam", "hitbox", "magic bullet",
            "aim assist", "aimassist", "softaim", "ragebot", "spinbot",

            // macros
            "autoclicker", "auto clicker", "autofire", "auto fire",
            "recoil macro", "no recoil macro", "rapidfire", "rapid fire",
            "macros", "macro", "macro script",

            // unban / unlock
            "unban", "unlocker", "unlock all", "vac bypass", "vac bypasser",
            "cheat", "hack", "trainer"
        ];

        // SCUM бренды/триггеры
        public static readonly string[] ScumNames =
        [
            "scum [ingram]",
            "scum [shack]",
            "pheonix [scum]",
            "scum [aj]",
            "scum [arcane]",
            "scum [baunti]",
            "scum [ca]",
            "scum [hc]",
            "scum [mason]",

            "scum aimbot",
            "scum esp",
            "scum wallhack",
            "scum radar",
            "scum loot esp",
            "scum item esp",
            "scum triggerbot",
            "scum silent aim",
            "scum norecoil",
            "scum no recoil",
            "scum speedhack",
            "scum flyhack",
            "scum teleport",
            "scum hwid spoofer",
            "scum unban",
            "scum undetected",
            "scum cheat",
            "scum hack",
            "scum external",
            "scum internal",
            "scum dma"
        ];

        // dev/reverse инструменты
        public static readonly string[] DevTools =
        [
            "dnspy", "ilspy", "x64dbg", "x32dbg", "ollydbg",
            "ghidra", "ida", "ida64", "ida pro",
            "cheat engine", "cheatengine", "process hacker", "procexp",
            "decompiler", "debugger", "debug",
            "api monitor", "fiddler", "wireshark"
        ];

        // ====== Критичные (красные) слова/бренды -> для High
        public static readonly string[] Critical =
        [
            "ingram", "shack", "pheonix", "aj", "arcane", "baunt", "mason",
            "hyper", "dma", "external", "xone", "interium", "midnight", "loader",
            "ragebot", "spinbot", "aimbot"
        ];

        private static readonly string[] ConfigTokens =
        [
            "cfg", "config"
        ];

        // матчим аккуратно: токены/слова/части имени + расширение .cfg
        private static readonly Regex CriticalRegex = new Regex(
            $@"(?i)(?<!\w)({string.Join("|", Critical.Select(Regex.Escape))})(?!\w)",
            RegexOptions.Compiled
        );

        private static readonly Regex ConfigRegex = new Regex(
            $@"(?i)(?<!\w)({string.Join("|", ConfigTokens.Select(Regex.Escape))})(?!\w)|(\.cfg\b)",
            RegexOptions.Compiled
        );

        public static bool ContainsCritical(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (CriticalRegex.IsMatch(text)) return true;

            if (ConfigRegex.IsMatch(text))
                return ContainsAny(text, Generic, ScumNames);

            return false;
        }

        public static bool ContainsConfigHint(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            return ConfigRegex.IsMatch(text);
        }

        // короткие токены лучше искать как отдельные слова
        private static readonly string[] ShortTokens =
        [
            "wh", "esp", "bhop", "vac", "hwid", "kdmapper"
        ];

        private static readonly Regex ShortTokenRegex = new Regex(
            $@"(?<!\w)({string.Join("|", ShortTokens.Select(Regex.Escape))})(?!\w)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        /// <summary>
        /// Проверка: содержит ли текст любые ключевики из массивов.
        /// </summary>
        public static bool ContainsAny(string text, params string[][] lists)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            var s = text.ToLowerInvariant();

            // 1) regex для коротких токенов
            if (ShortTokenRegex.IsMatch(s))
                return true;

            // 2) обычные contains для остальных
            foreach (var list in lists)
            {
                foreach (var k in list)
                {
                    if (string.IsNullOrWhiteSpace(k)) continue;
                    if (s.Contains(k.ToLowerInvariant()))
                        return true;
                }
            }

            return false;
        }

        // =========================================================
        // PATH / DIRECTORY / FILE checks
        // =========================================================

        /// <summary>
        /// Главная проверка пути: файл или папка.
        /// </summary>
        public static bool IsPathSuspicious(string path, bool treatDevToolsAsSuspicious = false)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            if (Directory.Exists(path))
                return IsDirectorySuspicious(path, treatDevToolsAsSuspicious);

            if (File.Exists(path))
                return IsFileSuspicious(path, treatDevToolsAsSuspicious);

            return false;
        }

        /// <summary>
        /// Проверка папки по названию/пути.
        /// </summary>
        public static bool IsDirectorySuspicious(string dirPath, bool treatDevToolsAsSuspicious = false)
        {
            if (string.IsNullOrWhiteSpace(dirPath) || !Directory.Exists(dirPath))
                return false;

            var haystack = $"{dirPath} {Path.GetFileName(dirPath)}";

            // 1) критичные (High)
            if (ContainsCritical(haystack))
                return true;

            // 2) обычные ключевики + SCUM
            if (ContainsAny(haystack, Generic, ScumNames))
                return true;

            // 3) DevTools опционально
            if (treatDevToolsAsSuspicious && ContainsAny(haystack, DevTools))
                return true;

            return false;
        }

        /// <summary>
        /// Главная проверка файла: НЕ триггериться на файлы с валидной цифровой подписью.
        /// </summary>
        public static bool IsFileSuspicious(string filePath, bool treatDevToolsAsSuspicious = false)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return false;

            // если подписан валидно — считаем не подозрительным
            if (HasValidDigitalSignature(filePath))
                return false;

            var haystack = $"{filePath} {Path.GetFileName(filePath)}";

            // критичные тоже считаем подозрительными
            if (ContainsCritical(haystack))
                return true;

            if (ContainsAny(haystack, Generic, ScumNames))
                return true;

            if (treatDevToolsAsSuspicious && ContainsAny(haystack, DevTools))
                return true;

            return false;
        }

        /// <summary>
        /// Валидная цифровая подпись (Authenticode)
        /// </summary>
        public static bool HasValidDigitalSignature(string filePath)
        {
            try
            {
                var cert = new X509Certificate2(X509Certificate.CreateFromSignedFile(filePath));

                using var chain = new X509Chain();
                chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;
                chain.ChainPolicy.VerificationTime = DateTime.UtcNow;

                return chain.Build(cert);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsUserSpacePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(path);
            }
            catch
            {
                return false;
            }

            foreach (var root in GetUserSpaceRoots())
            {
                if (string.IsNullOrWhiteSpace(root)) continue;
                if (fullPath.StartsWith(NormalizeRoot(root), StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public static bool IsTempPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            string fullPath;
            try
            {
                fullPath = Path.GetFullPath(path);
            }
            catch
            {
                return false;
            }

            var tempRoot = Path.GetTempPath();
            if (string.IsNullOrWhiteSpace(tempRoot)) return false;

            return fullPath.StartsWith(NormalizeRoot(tempRoot), StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeRoot(string path)
            => path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

        private static IEnumerable<string> GetUserSpaceRoots()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            yield return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            yield return localAppData;
            yield return Path.Combine(localAppData, "Programs");
            yield return Path.Combine(userProfile, "Downloads");
            yield return Path.GetTempPath();
        }
    }
}
