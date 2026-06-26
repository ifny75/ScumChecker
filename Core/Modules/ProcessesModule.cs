using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using ScumChecker.Core;

namespace ScumChecker.Core.Modules
{
    public sealed class ProcessesModule : IScanModule
    {
        public string Name => "Processes (name + path + cmdline analysis)";

        public IEnumerable<ScanItem> Run(CancellationToken ct)
        {
            Process[] processes;
            var commandLines = ReadCommandLinesByPid();

            try
            {
                processes = Process.GetProcesses();
            }
            catch
            {
                yield break;
            }

            foreach (var p in processes)
            {
                ct.ThrowIfCancellationRequested();

                string name;
                int pid;
                string? path = null;
                bool reportedUnsigned = false;
                bool reportedByKeyword = false;
                bool hasPath = false;
                bool isUnsigned = false;
                bool inUserSpace = false;

                try
                {
                    name = p.ProcessName;
                    pid = p.Id;
                }
                catch
                {
                    continue;
                }

                try
                {
                    path = p.MainModule?.FileName;
                }
                catch
                {
                    path = null;
                }

                string commandLine = commandLines.TryGetValue(pid, out var cmd) ? cmd : "";
                string pathFile = string.IsNullOrWhiteSpace(path) ? "" : Path.GetFileName(path);
                string haystack = $"{name} {pathFile} {path ?? ""} {commandLine}";

                // Агрессивный детект dev/reverse тулзов (IDA/Ghidra/x64dbg и т.д.).
                if (SuspicionKeywords.ContainsAny(haystack, SuspicionKeywords.DevTools))
                {
                    yield return new ScanItem
                    {
                        Severity = Severity.Low,
                        Group = FindingGroup.DevTools,
                        Category = "Processes",
                        Title = "Dev / reverse engineering tool",
                        Reason = "Process indicator matches reverse engineering or debugging tool",
                        Recommendation = "Often legitimate. Use as context only.",
                        Details = $"{name} (PID {pid}) | {path ?? "Path unavailable"}"
                    };

                    reportedByKeyword = true;
                }

                // Critical/brand hit (xone/loader/scum-brand markers).
                if (SuspicionKeywords.ContainsCritical(haystack) ||
                    SuspicionKeywords.ContainsAny(haystack, SuspicionKeywords.ScumNames))
                {
                    yield return new ScanItem
                    {
                        Severity = Severity.High,
                        Group = FindingGroup.HighRisk,
                        Category = "Processes",
                        Title = "Process matched high-risk brand/keyword",
                        Reason = "Matched critical SCUM/loader-related keyword in process indicators",
                        Recommendation = "Manual review recommended. Confirm with additional evidence.",
                        Details = $"{name} (PID {pid}) | {path ?? "Path unavailable"}"
                    };

                    reportedByKeyword = true;
                }

                if (!string.IsNullOrWhiteSpace(path))
                {
                    hasPath = true;
                    isUnsigned = !SuspicionKeywords.HasValidDigitalSignature(path);
                    inUserSpace = SuspicionKeywords.IsUserSpacePath(path);

                    if (isUnsigned && inUserSpace)
                    {
                        var sev = SuspicionKeywords.IsTempPath(path) ? Severity.High : Severity.Medium;
                        yield return new ScanItem
                        {
                            Severity = sev,
                            Group = sev == Severity.High ? FindingGroup.HighRisk : FindingGroup.Suspicious,
                            Category = "Processes",
                            Title = "Unsigned process running from user-space",
                            Reason = "Executable without valid signature is running from user profile or temp location",
                            Recommendation = "Manual review. Confirm source and intent before action.",
                            Details = $"{name} (PID {pid}) - {path}"
                        };

                        reportedUnsigned = true;
                    }
                }

                // Generic keyword rules over name + path + cmdline.
                if (SuspicionKeywords.ContainsAny(haystack, SuspicionKeywords.Generic))
                {
                    if (reportedUnsigned || reportedByKeyword) continue;

                    Severity sev = Severity.Medium;
                    string reason = "Process indicators contain suspicious keyword";

                    if (hasPath)
                    {
                        if (isUnsigned || inUserSpace)
                        {
                            sev = Severity.Medium;
                            reason = "Suspicious keyword with unsigned or user-space process path";
                        }
                        else
                        {
                            sev = Severity.Low;
                            reason = "Suspicious keyword but signed and not in user-space";
                        }
                    }

                    yield return new ScanItem
                    {
                        Severity = sev,
                        Group = sev == Severity.High ? FindingGroup.HighRisk : FindingGroup.Suspicious,
                        Category = "Processes",
                        Title = "Suspicious process keyword",
                        Reason = reason,
                        Recommendation = "Manual review. Do not ban by this alone.",
                        Details = $"{name} (PID {pid}) | {path ?? "Path unavailable"}"
                    };
                }
            }
        }

        private static Dictionary<int, string> ReadCommandLinesByPid()
        {
            var map = new Dictionary<int, string>();
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT ProcessId, CommandLine FROM Win32_Process");
                foreach (var obj in searcher.Get())
                {
                    try
                    {
                        var pidObj = obj["ProcessId"];
                        var cmdObj = obj["CommandLine"];
                        if (pidObj == null || cmdObj == null) continue;

                        int pid = Convert.ToInt32(pidObj);
                        var cmd = cmdObj.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(cmd))
                            map[pid] = cmd;
                    }
                    catch
                    {
                        // skip invalid row
                    }
                }
            }
            catch
            {
                // WMI can be disabled
            }

            return map;
        }
    }
}
