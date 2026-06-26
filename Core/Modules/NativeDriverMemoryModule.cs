using System.Collections.Generic;
using System.Threading;
using ScumChecker.Core.Native;

namespace ScumChecker.Core.Modules
{
    public sealed class NativeDriverMemoryModule : IScanModule
    {
        public string Name => "Native Fast Scanner (C++)";

        public IEnumerable<ScanItem> Run(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var native = NativeScannerBridge.RunDriverMemoryScan();
            if (!native.Success)
            {
                yield return new ScanItem
                {
                    Severity = Severity.Low,
                    Group = FindingGroup.SystemInfo,
                    Category = "Native Scanner",
                    What = "Native driver/memory checks unavailable",
                    Reason = native.Error ?? "Unknown native scanner error",
                    Recommendation = "Build and place scum_native_scanner.dll near ScumChecker.exe",
                    Details = "Run build_native_scanner_dll.bat and then restart the app."
                };
                yield break;
            }

            yield return new ScanItem
            {
                Severity = Severity.Info,
                Group = FindingGroup.SystemInfo,
                Category = "Native Scanner",
                What = "Native fast scan summary",
                Reason = "File/process/driver/registry/steam checks completed",
                Recommendation = native.Report.RanAsAdmin == 1
                    ? "Reviewed with elevated privileges."
                    : "For fuller driver visibility, run scanner as Administrator.",
                Details = $"Files={native.Report.FilesChecked}, Processes={native.Report.ProcessesChecked}, Drivers={native.Report.DriversChecked}, Services={native.Report.ServicesChecked}, Registry={native.Report.RegistryValuesChecked}, SteamAccounts={native.Report.SteamAccountsChecked}, Time={native.Report.ElapsedMs}ms"
            };

            foreach (var f in native.Findings)
            {
                ct.ThrowIfCancellationRequested();

                var sev = MapSeverity(f.Severity);
                yield return new ScanItem
                {
                    Severity = sev,
                    Group = sev == Severity.High ? FindingGroup.HighRisk : FindingGroup.Suspicious,
                    Category = string.IsNullOrWhiteSpace(f.Category) ? "Native Scanner" : f.Category,
                    What = string.IsNullOrWhiteSpace(f.Title) ? "Native detection" : f.Title,
                    Reason = string.IsNullOrWhiteSpace(f.Reason) ? "Native engine raised a risk signal" : f.Reason,
                    Recommendation = sev == Severity.High
                        ? "Manual review recommended. Confirm before enforcement."
                        : "Use as context with other evidence.",
                    Details = string.IsNullOrWhiteSpace(f.Details)
                        ? $"Score={f.Score}"
                        : $"{f.Details} | Score={f.Score}",
                    EvidencePath = string.IsNullOrWhiteSpace(f.EvidencePath) ? null : f.EvidencePath
                };
            }
        }

        private static Severity MapSeverity(int nativeSeverity) => nativeSeverity switch
        {
            3 => Severity.High,
            2 => Severity.Medium,
            1 => Severity.Low,
            _ => Severity.Info
        };
    }
}
