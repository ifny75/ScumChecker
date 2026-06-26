using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using Microsoft.Win32;

namespace ScumChecker.Core.Modules
{
    public sealed class HwidModule : IScanModule
    {
        public string Name => "HWID (extended + spoof heuristics)";

        public IEnumerable<ScumChecker.Core.ScanItem> Run(CancellationToken ct)
        {
            string cpu = "";
            string bios = "";
            string disk = "";
            string uuid = "";
            string baseBoard = "";
            string machineGuid = "";

            try
            {
                using (var s1 = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                    foreach (var o in s1.Get())
                    {
                        ct.ThrowIfCancellationRequested();
                        cpu = o["ProcessorId"]?.ToString() ?? "";
                        break;
                    }

                using (var s2 = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS"))
                    foreach (var o in s2.Get())
                    {
                        ct.ThrowIfCancellationRequested();
                        bios = o["SerialNumber"]?.ToString() ?? "";
                        break;
                    }

                using (var s3 = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_PhysicalMedia"))
                    foreach (var o in s3.Get())
                    {
                        ct.ThrowIfCancellationRequested();
                        disk = o["SerialNumber"]?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(disk)) break;
                    }

                using (var s4 = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct"))
                    foreach (var o in s4.Get())
                    {
                        ct.ThrowIfCancellationRequested();
                        uuid = o["UUID"]?.ToString() ?? "";
                        break;
                    }

                using (var s5 = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard"))
                    foreach (var o in s5.Get())
                    {
                        ct.ThrowIfCancellationRequested();
                        baseBoard = o["SerialNumber"]?.ToString() ?? "";
                        break;
                    }
            }
            catch
            {
                // WMI может быть недоступен.
            }

            try
            {
                using var hk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography", false);
                machineGuid = hk?.GetValue("MachineGuid")?.ToString() ?? "";
            }
            catch
            {
                machineGuid = "";
            }

            yield return new ScumChecker.Core.ScanItem
            {
                Severity = ScumChecker.Core.Severity.Info,
                Group = ScumChecker.Core.FindingGroup.SystemInfo,
                Category = "System",
                What = "HWID (fingerprint)",
                Reason = "Collected local hardware/software identifiers",
                Recommendation = "Use as baseline context.",
                Details =
                    $"CPU: {cpu} | BIOS: {bios} | DISK: {disk} | UUID: {uuid} | BASEBOARD: {baseBoard} | MachineGuid: {machineGuid}"
            };

            var signals = new List<string>();
            int score = 0;

            CheckIdentifier("CPU", cpu, signals, ref score);
            CheckIdentifier("BIOS", bios, signals, ref score);
            CheckIdentifier("DISK", disk, signals, ref score);
            CheckIdentifier("UUID", uuid, signals, ref score);
            CheckIdentifier("BASEBOARD", baseBoard, signals, ref score);
            CheckIdentifier("MachineGuid", machineGuid, signals, ref score);

            var suspiciousServiceHits = FindSuspiciousDriverServices(ct).ToList();
            if (suspiciousServiceHits.Count > 0)
            {
                score += 20 + Math.Min(20, suspiciousServiceHits.Count * 4);
                signals.Add("Suspicious driver services: " + string.Join(", ", suspiciousServiceHits.Take(6)));
            }

            var suspiciousProcessHits = FindSuspiciousProcesses(ct).ToList();
            if (suspiciousProcessHits.Count > 0)
            {
                score += 10 + Math.Min(20, suspiciousProcessHits.Count * 3);
                signals.Add("Suspicious processes: " + string.Join(", ", suspiciousProcessHits.Take(6)));
            }

            if (score >= 35)
            {
                var sev = score >= 70 ? ScumChecker.Core.Severity.High : ScumChecker.Core.Severity.Medium;

                yield return new ScumChecker.Core.ScanItem
                {
                    Severity = sev,
                    Group = sev == ScumChecker.Core.Severity.High
                        ? ScumChecker.Core.FindingGroup.HighRisk
                        : ScumChecker.Core.FindingGroup.Suspicious,
                    Category = "System",
                    What = "Possible HWID spoof indicators",
                    Reason = "Multiple spoof-related indicators detected (heuristic)",
                    Recommendation = sev == ScumChecker.Core.Severity.High
                        ? "Manual review recommended. Confirm with additional evidence before enforcement."
                        : "Treat as context signal. Do not ban based on this alone.",
                    Details = $"Score={score} | Signals: {string.Join(" | ", signals)}"
                };
            }
        }

        private static void CheckIdentifier(string name, string value, List<string> signals, ref int score)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                score += 8;
                signals.Add($"{name} is empty");
                return;
            }

            var v = value.Trim();
            if (LooksPlaceholder(v))
            {
                score += 18;
                signals.Add($"{name} looks placeholder/default");
            }

            if (LooksUniformOrRepeated(v))
            {
                score += 12;
                signals.Add($"{name} has repeated/uniform pattern");
            }
        }

        private static bool LooksPlaceholder(string value)
        {
            var v = value.ToLowerInvariant();
            if (v.Length < 4) return true;

            string[] bad =
            [
                "to be filled", "to be filled by o.e.m", "default", "default string",
                "system serial", "none", "unknown", "oem", "invalid",
                "123456", "abcdef", "ffffffff", "00000000"
            ];

            return bad.Any(x => v.Contains(x, StringComparison.Ordinal));
        }

        private static bool LooksUniformOrRepeated(string value)
        {
            var compact = new string(value.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
            if (compact.Length < 6) return false;

            if (compact.All(c => c == compact[0]))
                return true;

            int same = compact.Count(c => c == compact[0]);
            if (same >= compact.Length - 2)
                return true;

            return false;
        }

        private static IEnumerable<string> FindSuspiciousDriverServices(CancellationToken ct)
        {
            string[] needles =
            [
                "spoof", "spoofer", "mapper", "kdmapper", "amifldrv", "iqvw", "rtcore", "winring", "drvmap"
            ];

            RegistryKey? root = null;
            try { root = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services", false); }
            catch { }
            if (root == null) yield break;

            using (root)
            {
                string[] names;
                try { names = root.GetSubKeyNames(); }
                catch { yield break; }

                foreach (var n in names)
                {
                    ct.ThrowIfCancellationRequested();

                    var lower = n.ToLowerInvariant();
                    if (!needles.Any(x => lower.Contains(x, StringComparison.Ordinal)))
                        continue;

                    yield return n;
                }
            }
        }

        private static IEnumerable<string> FindSuspiciousProcesses(CancellationToken ct)
        {
            string[] needles =
            [
                "spoofer", "hwid", "mapper", "kdmapper", "serial", "amidewin", "volumeid"
            ];

            Process[] processes;
            try { processes = Process.GetProcesses(); }
            catch { yield break; }

            foreach (var p in processes)
            {
                ct.ThrowIfCancellationRequested();

                string name;
                try { name = p.ProcessName; }
                catch { continue; }

                var lower = name.ToLowerInvariant();
                if (needles.Any(x => lower.Contains(x, StringComparison.Ordinal)))
                    yield return name;
            }
        }
    }
}
