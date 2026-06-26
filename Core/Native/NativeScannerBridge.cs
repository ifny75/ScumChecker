using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ScumChecker.Core.Native
{
    internal static class NativeScannerBridge
    {
        private const string DllName = "scum_native_scanner.dll";

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct NsFinding
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string Category;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string Title;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string Reason;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)] public string Details;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 520)] public string EvidencePath;
            public int Severity;
            public int Score;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NsReport
        {
            public int Status;
            public int FindingsReturned;
            public int FilesChecked;
            public int DriversChecked;
            public int ServicesChecked;
            public int ProcessesChecked;
            public int RegistryValuesChecked;
            public int SteamAccountsChecked;
            public int Errors;
            public int RanAsAdmin;
            public int ElapsedMs;
        }

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool Ns_IsAdministrator();

        [DllImport(DllName, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int Ns_RunDriverMemoryScan(
            [Out] NsFinding[] findingsBuffer,
            int findingsCapacity,
            out NsReport report);

        internal sealed class NativeScanResult
        {
            public bool Success { get; init; }
            public string? Error { get; init; }
            public NsReport Report { get; init; }
            public IReadOnlyList<NsFinding> Findings { get; init; } = Array.Empty<NsFinding>();
        }

        internal static NativeScanResult RunDriverMemoryScan(int maxFindings = 512)
        {
            try
            {
                var buffer = new NsFinding[Math.Max(1, maxFindings)];
                int status = Ns_RunDriverMemoryScan(buffer, buffer.Length, out var report);
                if (status != 0)
                {
                    return new NativeScanResult
                    {
                        Success = false,
                        Error = $"Native scanner returned status {status}",
                        Report = report
                    };
                }

                int safeCount = Math.Max(0, Math.Min(report.FindingsReturned, buffer.Length));
                var findings = buffer
                    .Take(safeCount)
                    .Where(f => !string.IsNullOrWhiteSpace(f.Title))
                    .OrderByDescending(f => f.Severity)
                    .ThenByDescending(f => f.Score)
                    .ToArray();

                return new NativeScanResult
                {
                    Success = true,
                    Report = report,
                    Findings = findings
                };
            }
            catch (DllNotFoundException ex)
            {
                return new NativeScanResult
                {
                    Success = false,
                    Error = $"Native DLL not found: {ex.Message}"
                };
            }
            catch (EntryPointNotFoundException ex)
            {
                return new NativeScanResult
                {
                    Success = false,
                    Error = $"Native API mismatch: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new NativeScanResult
                {
                    Success = false,
                    Error = $"Native scan failed: {ex.Message}"
                };
            }
        }
    }
}
