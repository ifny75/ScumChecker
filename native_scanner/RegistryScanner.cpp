#ifndef NOMINMAX
#define NOMINMAX
#endif

#include "RegistryScanner.h"

#include <Windows.h>

#include <array>

namespace {

void ScanRunKey(HKEY root, const wchar_t* subKey, REGSAM sam, const ns::RuleSet& rules, ns::ModuleOutput& out) {
    HKEY key = nullptr;
    if (RegOpenKeyExW(root, subKey, 0, KEY_READ | sam, &key) != ERROR_SUCCESS) {
        return;
    }

    DWORD idx = 0;
    wchar_t valueName[512]{};
    BYTE valueData[4096]{};

    while (true) {
        DWORD valueNameLen = static_cast<DWORD>(std::size(valueName));
        DWORD valueDataSize = static_cast<DWORD>(std::size(valueData));
        DWORD type = 0;

        const LONG rc = RegEnumValueW(
            key,
            idx++,
            valueName,
            &valueNameLen,
            nullptr,
            &type,
            valueData,
            &valueDataSize);

        if (rc == ERROR_NO_MORE_ITEMS) break;
        if (rc != ERROR_SUCCESS) {
            out.errors++;
            break;
        }

        if (type != REG_SZ && type != REG_EXPAND_SZ) continue;

        ++out.registry_values_checked;

        std::wstring cmd(reinterpret_cast<wchar_t*>(valueData), valueDataSize / sizeof(wchar_t));
        if (!cmd.empty() && cmd.back() == L'\0') cmd.pop_back();

        auto expanded = ns::ExpandEnvironment(cmd);
        auto trimmed = ns::TrimQuotesAndArgs(expanded);
        auto lower = ns::ToLowerCopy(trimmed);

        const bool userSpace = ns::IsUserSpacePath(lower);
        const bool suspicious = ns::ContainsAnyNeedle(lower, rules.strings) || ns::ContainsAnyNeedle(lower, rules.paths);

        if (userSpace) {
            ns::PushFinding(
                out.findings,
                L"Registry",
                L"Autorun entry from user-space",
                L"Startup registry entry points to temp/appdata path",
                std::wstring(valueName),
                trimmed,
                NS_SEVERITY_MEDIUM,
                75);
        }

        if (suspicious) {
            ns::PushFinding(
                out.findings,
                L"Registry",
                L"Autorun entry matched suspicious rules",
                L"Startup command matched rule strings/paths",
                std::wstring(valueName),
                trimmed,
                NS_SEVERITY_MEDIUM,
                72);
        }
    }

    RegCloseKey(key);
}

} // namespace

namespace ns {

ModuleOutput RunRegistryScanner(const RuleSet& rules) {
    ModuleOutput out;

    try {
        ScanRunKey(HKEY_CURRENT_USER, L"Software\\Microsoft\\Windows\\CurrentVersion\\Run", 0, rules, out);
        ScanRunKey(HKEY_CURRENT_USER, L"Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce", 0, rules, out);

        ScanRunKey(HKEY_LOCAL_MACHINE, L"Software\\Microsoft\\Windows\\CurrentVersion\\Run", KEY_WOW64_64KEY, rules, out);
        ScanRunKey(HKEY_LOCAL_MACHINE, L"Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce", KEY_WOW64_64KEY, rules, out);
        ScanRunKey(HKEY_LOCAL_MACHINE, L"Software\\Microsoft\\Windows\\CurrentVersion\\Run", KEY_WOW64_32KEY, rules, out);
        ScanRunKey(HKEY_LOCAL_MACHINE, L"Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce", KEY_WOW64_32KEY, rules, out);
    } catch (...) {
        out.errors++;
    }

    return out;
}

} // namespace ns
