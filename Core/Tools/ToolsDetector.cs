using System;
using System.Collections.Generic;
using System.IO;

namespace ScumChecker.Core.Tools
{
    public static class ToolsDetector
    {
        private static string? _baseDir;

        public static void SetBaseDirectory(string dir)
        {
            _baseDir = dir;
        }




        private static string GetBaseDirectory()
        {
            // если задали — используем
            if (!string.IsNullOrWhiteSpace(_baseDir))
                return _baseDir;

            // fallback: папка рядом с exe
            return Path.Combine(
                AppContext.BaseDirectory,
                "programms"
            );
        }

        // ⬇️ ниже идёт СТАРЫЙ код ToolsDetector (Detect и т.д.)

        // Твоя папка с утилитами (как ты написал)

        private static string ProgramsDir => GetBaseDirectory();

        public static List<ToolEntry> Detect()
        {
            var list = new List<ToolEntry>();

            // === Основные ===
            list.Add(DetectFromProgramsDir(
                name: "Everything (Voidtools)",
                fileNames: new[] { "Everything.exe" },
                downloadUrl: "https://www.voidtools.com/downloads/"));


            // === Со скрина / portable ===
            list.Add(DetectFromProgramsDir(
                name: "CachedProgramsList (NirSoft)",
                fileNames: new[] { "CachedProgramsList.exe" },
                downloadUrl: "https://www.nirsoft.net/utils/cached_programs_list.html"));

            list.Add(DetectFromProgramsDir(
                name: "ExecutedProgramsList (NirSoft)",
                fileNames: new[] { "ExecutedProgramsList.exe" },
                downloadUrl: "https://www.nirsoft.net/utils/executed_programs_list.html"));

            list.Add(DetectFromProgramsDir(
                name: "JournalTrace",
                fileNames: new[] { "JournalTrace.exe" },
                downloadUrl: "https://github.com/ponei/JournalTrace/releases")); // если нет ссылки — оставь пусто

            list.Add(DetectFromProgramsDir(
                name: "LastActivityView (NirSoft)",
                fileNames: new[] { "LastActivityView.exe" },
                downloadUrl: "https://www.nirsoft.net/utils/lastactivityview.html"));

            list.Add(DetectFromProgramsDir(
                name: "USBDeview (NirSoft)",
                fileNames: new[] { "USBDeview.exe" },
                downloadUrl: "https://www.nirsoft.net/utils/usb_devices_view.html"));

            list.Add(DetectFromProgramsDir(
                name: "USBDriveLog (NirSoft)",
                fileNames: new[] { "USBDriveLog.exe" },
                downloadUrl: "https://www.nirsoft.net/utils/usb_drive_log.html"));

            list.Add(DetectFromProgramsDir(
                name: "Shellbag Analyzer/Cleaner",
                fileNames: new[] { "shellbag_analyzer_cleaner.exe" },
                downloadUrl: "https://privazer.com/ru/download-shellbag-analyzer-shellbag-cleaner.php")); // если это твой файл — тоже можно пусто

            return list;
        }

        /// <summary>
        /// Ищет утилиту в ProgramsDir. Возвращает ToolEntry со Status/Path.
        /// Если папки нет — Status = "Not found".
        /// </summary>
        private static ToolEntry DetectFromProgramsDir(string name, string[] fileNames, string downloadUrl)
        {
            var t = new ToolEntry
            {
                Name = name,
                DownloadUrl = downloadUrl,
                Status = "Not found",
                Path = ""
            };

            try
            {
                if (!Directory.Exists(ProgramsDir))
                    return t;

                foreach (var fn in fileNames)
                {
                    var full = Path.Combine(ProgramsDir, fn);
                    if (File.Exists(full))
                    {
                        t.Status = "Found";
                        t.Path = full;
                        return t;
                    }
                }

                // Дополнительно: если файл мог быть переименован, попробуем "по маске"
                // Например: JournalTrace (2).exe и т.п.
                // Ищем по началу имени (без расширения).
                foreach (var fn in fileNames)
                {
                    var baseName = Path.GetFileNameWithoutExtension(fn);

                    foreach (var f in Directory.EnumerateFiles(ProgramsDir, "*.exe", SearchOption.TopDirectoryOnly))
                    {
                        var fNameNoExt = Path.GetFileNameWithoutExtension(f);

                        if (fNameNoExt.StartsWith(baseName, StringComparison.OrdinalIgnoreCase))
                        {
                            t.Status = "Found";
                            t.Path = f;
                            return t;
                        }
                    }
                }
            }
            catch
            {
                // молча — это инструмент для модерации, не валим UI из-за детекта
            }

            return t;
        }
    }
}
