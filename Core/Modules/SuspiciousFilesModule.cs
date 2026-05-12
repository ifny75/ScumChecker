using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ScumChecker.Core;

namespace ScumChecker.Core.Modules
{
    public sealed class SuspiciousFilesModule : IScanModule
    {
        public string Name => "Filesystem (scan)";

        private const int DefaultMaxDepth = 5;
        private const int MaxReportedHits = 500;

        private sealed class RootScan
        {
            public RootScan(string path, int maxDepth)
            {
                Path = path;
                MaxDepth = maxDepth;
            }

            public string Path { get; }
            public int MaxDepth { get; }
        }

        public IEnumerable<ScanItem> Run(CancellationToken ct)
        {
            int hits = 0;

            foreach (var root in GetRoots())
            {
                ct.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(root.Path)) continue;
                if (!Directory.Exists(root.Path)) continue;

                foreach (var found in ScanDir(root.Path, 0, ct, root.MaxDepth))
                {
                    if (hits < MaxReportedHits)
                    {
                        yield return found;
                    }

                    hits++;
                }
            }
        }

        private static IEnumerable<RootScan> GetRoots()
        {
            yield return new RootScan(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), DefaultMaxDepth);
            yield return new RootScan(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), DefaultMaxDepth);
            yield return new RootScan(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), DefaultMaxDepth);
            yield return new RootScan(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), DefaultMaxDepth);
            yield return new RootScan(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 2);

            // Downloads (нет SpecialFolder.Downloads)
            yield return new RootScan(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"),
                DefaultMaxDepth
            );
            yield return new RootScan(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games"),
                DefaultMaxDepth
            );
            yield return new RootScan(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games"),
                DefaultMaxDepth
            );

            yield return new RootScan(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games"),
                DefaultMaxDepth
            );
            yield return new RootScan(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games"),
                DefaultMaxDepth
            );

            yield return new RootScan(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DefaultMaxDepth);
            yield return new RootScan(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DefaultMaxDepth);

            yield return new RootScan(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs"), 3);
            yield return new RootScan(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp"), 2);
            yield return new RootScan(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft", "Windows", "Start Menu", "Programs", "Startup"),
                DefaultMaxDepth
            );

            yield return new RootScan(Path.GetTempPath(), 2);

            yield return new RootScan(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), 2);
            yield return new RootScan(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), 2);
            yield return new RootScan(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), 2);
        }

        private static IEnumerable<ScanItem> ScanDir(string dir, int depth, CancellationToken ct, int maxDepth)
        {
            if (depth > maxDepth) yield break;
            if (ShouldSkipDirectory(dir)) yield break;

            IEnumerable<string> subDirs;
            IEnumerable<string> files;

            try { subDirs = Directory.EnumerateDirectories(dir); }
            catch { yield break; }

            try { files = Directory.EnumerateFiles(dir); }
            catch { files = Array.Empty<string>(); }

            foreach (var f in files)
            {
                ct.ThrowIfCancellationRequested();

                var name = Path.GetFileName(f);
                var ext = Path.GetExtension(f).ToLowerInvariant();
                var haystack = $"{name} {f}";

                bool isExec = ext is ".exe" or ".dll" or ".sys";
                bool inUserSpace = SuspicionKeywords.IsUserSpacePath(f);
                bool hasSignature = SuspicionKeywords.HasValidDigitalSignature(f);
                bool hasConfigHint = SuspicionKeywords.ContainsConfigHint(haystack);
                bool hasKeywords = SuspicionKeywords.ContainsAny(haystack, SuspicionKeywords.Generic, SuspicionKeywords.ScumNames);
                bool isCritical = SuspicionKeywords.ContainsCritical(haystack);

                bool nameSuspicious =
                    !hasSignature &&
                    (isCritical ||
                     hasKeywords ||
                     (hasConfigHint && hasKeywords));

                // devtools (dnSpy и т.п.)
                if (SuspicionKeywords.ContainsAny(name, SuspicionKeywords.DevTools))
                {
                    yield return new ScanItem
                    {
                        Severity = Severity.Low,
                        Group = FindingGroup.DevTools,
                        Category = "Filesystem",
                        What = "Dev / reverse tool file",
                        Reason = "Reverse engineering/debug tooling detected (often legitimate)",
                        Recommendation = "No ban. Use as context only.",
                        Details = name,
                        EvidencePath = f
                    };
                    continue;
                }

                // suspicious by keyword (только если реально suspicious по правилам выше)
                if (nameSuspicious)
                {
                    bool inHotUserSpace =
                        f.Contains(@"\AppData\", StringComparison.OrdinalIgnoreCase) ||
                        f.Contains(@"\Temp\", StringComparison.OrdinalIgnoreCase);

                    Severity sev;
                    FindingGroup grp;
                    string reason;

                    if (isCritical)
                    {
                        sev = Severity.High;
                        reason = "Critical keyword detected in filename/path";
                    }
                    else if (isExec && inHotUserSpace)
                    {
                        sev = Severity.High;
                        reason = "Suspicious executable/driver-like file in AppData/Temp";
                    }
                    else if (hasConfigHint)
                    {
                        sev = (isExec || inUserSpace) ? Severity.Medium : Severity.Low;
                        reason = "Config-like file with suspicious keyword";
                    }
                    else
                    {
                        sev = (isExec || inUserSpace) ? Severity.Medium : Severity.Low;
                        reason = "Name contains suspicious keyword (needs manual review)";
                    }

                    grp = (sev == Severity.High) ? FindingGroup.HighRisk : FindingGroup.Suspicious;

                    yield return new ScanItem
                    {
                        Severity = sev,
                        Group = grp,
                        Category = "Filesystem",
                        What = isExec ? "Suspicious executable name" : "Suspicious filename",
                        Reason = reason,
                        Recommendation = sev == Severity.High
                            ? "Manual review + ask user. Consider action if confirmed."
                            : (sev == Severity.Medium
                                ? "Manual review. Do not ban by this alone."
                                : "Use as context only."),
                        Details = name,
                        EvidencePath = f
                    };

                    continue;
                }

                // unsigned executable in user-space (отдельное правило)
                if (isExec && inUserSpace && !hasSignature)
                {
                    var sev = SuspicionKeywords.IsTempPath(f) ? Severity.High : Severity.Medium;

                    yield return new ScanItem
                    {
                        Severity = sev,
                        Group = sev == Severity.High ? FindingGroup.HighRisk : FindingGroup.Suspicious,
                        Category = "Filesystem",
                        What = "Unsigned executable in user-space",
                        Reason = "Unsigned executable/driver-like file running from user profile or temp location",
                        Recommendation = "Manual review. Confirm origin and intent before action.",
                        Details = name,
                        EvidencePath = f
                    };
                }
            }

            foreach (var d in subDirs)
            {
                ct.ThrowIfCancellationRequested();

                if (SuspicionKeywords.IsDirectorySuspicious(d))
                {
                    var name = Path.GetFileName(d);
                    bool isCritical = SuspicionKeywords.ContainsCritical($"{name} {d}");
                    var sev = isCritical ? Severity.High : Severity.Medium;

                    yield return new ScanItem
                    {
                        Severity = sev,
                        Group = sev == Severity.High ? FindingGroup.HighRisk : FindingGroup.Suspicious,
                        Category = "Filesystem",
                        What = "Suspicious folder",
                        Reason = isCritical
                            ? "Critical keyword detected in folder name/path"
                            : "Folder name/path contains suspicious keyword",
                        Recommendation = sev == Severity.High
                            ? "Manual review + ask user. Consider action if confirmed."
                            : "Manual review. Do not ban by this alone.",
                        Details = name,
                        EvidencePath = d
                    };
                }

                foreach (var nested in ScanDir(d, depth + 1, ct, maxDepth))
                    yield return nested;
            }
        }

        private static bool ShouldSkipDirectory(string dir)
        {
            try
            {
                var info = new DirectoryInfo(dir);

                // не лезем в reparse points (симлинки/джанкшены)
                if (info.Attributes.HasFlag(FileAttributes.ReparsePoint)) return true;

                // системные папки часто закрыты правами + не нужны
                if (info.Attributes.HasFlag(FileAttributes.System)) return true;
            }
            catch
            {
                return true;
            }

            var name = Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar));

            if (string.Equals(name, "$Recycle.Bin", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(name, "System Volume Information", StringComparison.OrdinalIgnoreCase)) return true;

            // Windows/WinSxS обычно бесполезно и шумит
            if (string.Equals(name, "Windows", StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(name, "WinSxS", StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }
    }
}
