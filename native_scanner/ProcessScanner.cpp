#ifndef NOMINMAX
#define NOMINMAX
#endif

#include "ProcessScanner.h"

#include <Windows.h>
#include <Psapi.h>

#include <algorithm>
#include <array>
#include <unordered_set>
#include <vector>

#pragma comment(lib, "Psapi.lib")

namespace {

struct OverlayWindowCollector {
    std::unordered_set<DWORD> pids;
};

BOOL CALLBACK EnumWindowsProc(HWND hwnd, LPARAM lParam) {
    if (!IsWindowVisible(hwnd)) return TRUE;

    const LONG exStyle = GetWindowLongW(hwnd, GWL_EXSTYLE);
    const bool topMost = (exStyle & WS_EX_TOPMOST) != 0;
    const bool layered = (exStyle & WS_EX_LAYERED) != 0;
    const bool transparent = (exStyle & WS_EX_TRANSPARENT) != 0;

    if (!(topMost && (layered || transparent))) {
        return TRUE;
    }

    RECT rc{};
    if (!GetWindowRect(hwnd, &rc)) return TRUE;
    if ((rc.right - rc.left) < 250 || (rc.bottom - rc.top) < 150) return TRUE;

    DWORD pid = 0;
    GetWindowThreadProcessId(hwnd, &pid);
    if (pid == 0) return TRUE;

    auto* collector = reinterpret_cast<OverlayWindowCollector*>(lParam);
    collector->pids.insert(pid);
    return TRUE;
}

std::unordered_set<DWORD> CollectOverlayProcessIds() {
    OverlayWindowCollector collector;
    EnumWindows(EnumWindowsProc, reinterpret_cast<LPARAM>(&collector));
    return collector.pids;
}

bool IsTrustedProcessName(const std::wstring& exeLower) {
    static const std::unordered_set<std::wstring> trusted = {
        L"system", L"csrss.exe", L"wininit.exe", L"winlogon.exe", L"services.exe", L"svchost.exe",
        L"lsass.exe", L"explorer.exe", L"dwm.exe", L"taskmgr.exe", L"steam.exe", L"discord.exe"
    };
    return trusted.find(exeLower) != trusted.end();
}

bool IsMasqueradingName(const std::wstring& exeLower) {
    static const std::unordered_set<std::wstring> names = {
        L"svchost.exe", L"lsass.exe", L"csrss.exe", L"winlogon.exe", L"services.exe", L"smss.exe"
    };
    return names.find(exeLower) != names.end();
}

bool IsGameProcessName(const std::wstring& exeLower) {
    return exeLower == L"scum.exe";
}

bool ProcessHasGraphicsModules(HANDLE hProc) {
    HMODULE modules[1024]{};
    DWORD cbNeeded = 0;
    if (!EnumProcessModulesEx(hProc, modules, sizeof(modules), &cbNeeded, LIST_MODULES_ALL)) {
        return false;
    }

    const size_t count = cbNeeded / sizeof(HMODULE);
    for (size_t i = 0; i < count; ++i) {
        wchar_t modName[MAX_PATH]{};
        if (!GetModuleBaseNameW(hProc, modules[i], modName, static_cast<DWORD>(std::size(modName)))) {
            continue;
        }

        auto lower = ns::ToLowerCopy(modName);
        if (lower == L"d3d9.dll" || lower == L"d3d11.dll" || lower == L"dxgi.dll" || lower == L"opengl32.dll") {
            return true;
        }
    }

    return false;
}

void DetectSuspiciousModules(HANDLE hProc, DWORD pid, const ns::RuleSet& rules, std::vector<NsFinding>& findings) {
    HMODULE modules[2048]{};
    DWORD cbNeeded = 0;
    if (!EnumProcessModulesEx(hProc, modules, sizeof(modules), &cbNeeded, LIST_MODULES_ALL)) {
        return;
    }

    const size_t count = cbNeeded / sizeof(HMODULE);
    for (size_t i = 0; i < count; ++i) {
        wchar_t modPath[2048]{};
        if (!GetModuleFileNameExW(hProc, modules[i], modPath, static_cast<DWORD>(std::size(modPath)))) {
            continue;
        }

        std::wstring path = modPath;
        auto lower = ns::ToLowerCopy(path);
        if (!ns::IsUserSpacePath(lower)) continue;

        const bool hitRule = ns::ContainsAnyNeedle(lower, rules.strings) || ns::ContainsAnyNeedle(lower, rules.paths);
        const bool signedOk = ns::VerifyFileSignature(path);

        if (!signedOk || hitRule) {
            ns::PushFinding(
                findings,
                L"Process",
                L"Possible injected user-space module",
                L"Process loaded module from user-space path (temp/appdata)",
                L"PID " + std::to_wstring(pid) + L" | " + ns::Basename(path),
                path,
                (!signedOk ? NS_SEVERITY_HIGH : NS_SEVERITY_MEDIUM),
                (!signedOk ? 89 : 73));
        }
    }
}

} // namespace

namespace ns {

ModuleOutput RunProcessScanner(const RuleSet& rules) {
    ModuleOutput out;

    std::array<DWORD, 8192> pids{};
    DWORD needed = 0;
    if (!EnumProcesses(pids.data(), static_cast<DWORD>(pids.size() * sizeof(DWORD)), &needed)) {
        out.errors++;
        return out;
    }

    auto overlayPids = CollectOverlayProcessIds();

    const DWORD count = std::min<DWORD>(needed / sizeof(DWORD), static_cast<DWORD>(pids.size()));
    out.processes_checked = static_cast<int>(count);

    bool gameRunning = false;

    for (DWORD i = 0; i < count; ++i) {
        const DWORD pid = pids[i];
        if (pid == 0 || pid == 4 || pid == GetCurrentProcessId()) continue;

        HANDLE qh = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, FALSE, pid);
        if (!qh) continue;

        wchar_t pathBuf[2048]{};
        DWORD pathLen = static_cast<DWORD>(std::size(pathBuf));
        const bool gotPath = QueryFullProcessImageNameW(qh, 0, pathBuf, &pathLen) == TRUE;
        CloseHandle(qh);
        if (!gotPath || pathLen == 0) continue;

        std::wstring fullPath(pathBuf, pathLen);
        auto fullLower = ToLowerCopy(fullPath);
        auto exeLower = ToLowerCopy(Basename(fullPath));

        if (IsGameProcessName(exeLower)) {
            gameRunning = true;
        }

        const bool signedOk = VerifyFileSignature(fullPath);
        const bool userSpace = IsUserSpacePath(fullLower);
        const bool trusted = IsTrustedProcessName(exeLower);

        if (!signedOk && userSpace) {
            PushFinding(
                out.findings,
                L"Process",
                L"Unsigned process from user-space",
                L"Process executable is unsigned and launched from temp/appdata",
                L"PID " + std::to_wstring(pid),
                fullPath,
                NS_SEVERITY_HIGH,
                88);
        }

        if (IsMasqueradingName(exeLower) && fullLower.find(L"\\windows\\system32\\") == std::wstring::npos) {
            PushFinding(
                out.findings,
                L"Process",
                L"System process name masquerading",
                L"Process name mimics system binary but path is not System32",
                L"PID " + std::to_wstring(pid),
                fullPath,
                NS_SEVERITY_HIGH,
                93);
        }

        if (ContainsAnyNeedle(exeLower, rules.strings) || ContainsAnyNeedle(fullLower, rules.paths)) {
            PushFinding(
                out.findings,
                L"Process",
                L"Process name/path matched signature rules",
                L"Matched process indicator from rules.json",
                L"PID " + std::to_wstring(pid),
                fullPath,
                NS_SEVERITY_MEDIUM,
                76);
        }

        HANDLE ph = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, pid);
        if (!ph) {
            ph = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION | PROCESS_VM_READ, FALSE, pid);
        }

        bool hasGraphics = false;
        if (ph) {
            hasGraphics = ProcessHasGraphicsModules(ph);
            DetectSuspiciousModules(ph, pid, rules, out.findings);
            CloseHandle(ph);
        }

        if (overlayPids.find(pid) != overlayPids.end() && hasGraphics && !trusted) {
            PushFinding(
                out.findings,
                L"Process",
                L"Unknown overlay-like process",
                L"Top-most layered window + DirectX/OpenGL modules",
                L"PID " + std::to_wstring(pid),
                fullPath,
                NS_SEVERITY_MEDIUM,
                79);
        }
    }

    if (gameRunning) {
        PushFinding(
            out.findings,
            L"Process",
            L"Game process detected",
            L"SCUM process is running during scan",
            L"Process context for access/overlay checks is active",
            L"",
            NS_SEVERITY_INFO,
            5);
    }

    return out;
}

} // namespace ns
