using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ScumChecker.Core.Tools
{
    public static class ToolsBundler
    {
        // Куда распаковываем (совпадает с твоей логикой ToolsDetector)
        public static string GetProgramsDir()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ScumChecker",
                "programms"
            );
        }

        // Маппинг: "имя ресурса" -> "имя файла на диске"
        // ВАЖНО: имя ресурса обычно = <DefaultNamespace>.<папки>.<файл>
        private static readonly (string resourceEndsWith, string outFile)[] Tools =
        [
            ("Everything.exe", "Everything.exe"),
    ("CachedProgramsList.exe", "CachedProgramsList.exe"),
    ("ExecutedProgramsList.exe", "ExecutedProgramsList.exe"),
    ("LastActivityView.exe", "LastActivityView.exe"),
    ("USBDeview.exe", "USBDeview.exe"),
    ("USBDriveLog.exe", "USBDriveLog.exe"),
    ("ShellBagsExplorer64.exe", "ShellBagsExplorer64.exe"), // нахуя оно надо но ладно типо похуй типо окак пон смихуятина хаахаххахаха

    // СУКА Я ЗАБЫЛ ЭТИ ДВА ФАЙЛА ЕБАННЫХ ДОБАВИТЬ И ДОЛБИЛСЯ 2 ЧАСА
    ("JournalTrace.exe", "JournalTrace.exe"),
    ("shellbag_analyzer_cleaner.exe", "shellbag_analyzer_cleaner.exe"),
];


        public static void EnsureExtracted()
        {
            var dir = GetProgramsDir();
            Directory.CreateDirectory(dir);

            var asm = Assembly.GetExecutingAssembly();
            var names = asm.GetManifestResourceNames();

            foreach (var (endsWith, outFile) in Tools)
            {
                var targetPath = Path.Combine(dir, outFile);
                if (File.Exists(targetPath)) continue; // уже есть

                var resName = FindResource(names, endsWith);
                if (resName == null) continue;

                using var s = asm.GetManifestResourceStream(resName);
                if (s == null) continue;

                using var fs = File.Create(targetPath);
                s.CopyTo(fs);
            }
        }

        private static string? FindResource(string[] names, string endsWith)
        {
            foreach (var n in names)
                if (n.EndsWith(endsWith, StringComparison.OrdinalIgnoreCase))
                    return n;
            return null;
        }
    }
}
