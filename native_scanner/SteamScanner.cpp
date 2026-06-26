#ifndef NOMINMAX
#define NOMINMAX
#endif

#include "SteamScanner.h"

#include <Windows.h>

#include <filesystem>
#include <fstream>
#include <sstream>
#include <unordered_set>

namespace {

std::wstring ReadRegistrySteamPath() {
    HKEY key = nullptr;
    std::wstring path;

    if (RegOpenKeyExW(HKEY_LOCAL_MACHINE, L"SOFTWARE\\WOW6432Node\\Valve\\Steam", 0, KEY_READ, &key) == ERROR_SUCCESS) {
        if (auto p = ns::QueryStringValue(key, L"InstallPath"); p.has_value()) {
            path = *p;
        }
        RegCloseKey(key);
    }

    if (path.empty() && RegOpenKeyExW(HKEY_CURRENT_USER, L"Software\\Valve\\Steam", 0, KEY_READ, &key) == ERROR_SUCCESS) {
        if (auto p = ns::QueryStringValue(key, L"SteamPath"); p.has_value()) {
            path = *p;
        }
        RegCloseKey(key);
    }

    return path;
}

std::vector<std::wstring> GetSteamRoots() {
    std::vector<std::wstring> roots;

    auto reg = ReadRegistrySteamPath();
    if (!reg.empty()) roots.push_back(reg);

    wchar_t pf86[MAX_PATH]{};
    if (GetEnvironmentVariableW(L"ProgramFiles(x86)", pf86, static_cast<DWORD>(std::size(pf86))) > 0) {
        roots.emplace_back(std::wstring(pf86) + L"\\Steam");
    }

    std::vector<std::wstring> unique;
    std::unordered_set<std::wstring> seen;
    for (auto& r : roots) {
        auto low = ns::ToLowerCopy(r);
        if (seen.insert(low).second && std::filesystem::exists(std::filesystem::path(r))) {
            unique.push_back(r);
        }
    }

    return unique;
}

std::string ReadTextFileUtf8(const std::filesystem::path& path) {
    std::ifstream in(path, std::ios::in | std::ios::binary);
    if (!in) return {};
    return std::string((std::istreambuf_iterator<char>(in)), std::istreambuf_iterator<char>());
}

int CountOccurrences(const std::string& hay, const std::string& needle) {
    if (needle.empty()) return 0;
    int count = 0;
    size_t pos = 0;
    while ((pos = hay.find(needle, pos)) != std::string::npos) {
        ++count;
        pos += needle.size();
    }
    return count;
}

void ScanSteamRoot(const std::wstring& root, const ns::RuleSet& rules, ns::ModuleOutput& out) {
    const auto base = std::filesystem::path(root);
    const auto loginUsers = base / "config" / "loginusers.vdf";
    if (std::filesystem::exists(loginUsers)) {
        auto txt = ReadTextFileUtf8(loginUsers);
        const int accountCount = CountOccurrences(txt, "\"AccountName\"");
        out.steam_accounts_checked += accountCount;

        if (accountCount > 0) {
            ns::PushFinding(
                out.findings,
                L"Steam",
                L"Steam accounts discovered",
                L"Detected local Steam account history",
                L"Accounts=" + std::to_wstring(accountCount),
                loginUsers.wstring(),
                NS_SEVERITY_INFO,
                10);
        }

        if (txt.find("\"MostRecent\"\t\t\"1\"") != std::string::npos || txt.find("\"MostRecent\"\t\"1\"") != std::string::npos) {
            ns::PushFinding(
                out.findings,
                L"Steam",
                L"MostRecent Steam account marker found",
                L"Steam loginusers.vdf contains active/recent account flag",
                L"MostRecent=1",
                loginUsers.wstring(),
                NS_SEVERITY_INFO,
                8);
        }
    }

    const auto userdata = base / "userdata";
    if (std::filesystem::exists(userdata)) {
        std::error_code ec;
        for (auto it = std::filesystem::recursive_directory_iterator(userdata, std::filesystem::directory_options::skip_permission_denied, ec);
             it != std::filesystem::recursive_directory_iterator(); it.increment(ec)) {
            if (ec) {
                ec.clear();
                continue;
            }

            if (!it->is_regular_file(ec) || ec) {
                ec.clear();
                continue;
            }

            if (ns::ToLowerCopy(it->path().filename().wstring()) != L"localconfig.vdf") continue;

            auto txt = ReadTextFileUtf8(it->path());
            if (txt.find("513710") != std::string::npos) {
                ns::PushFinding(
                    out.findings,
                    L"Steam",
                    L"SCUM app history in Steam config",
                    L"Found AppID 513710 in localconfig.vdf",
                    it->path().wstring(),
                    it->path().wstring(),
                    NS_SEVERITY_INFO,
                    12);
            }
        }
    }

    const auto scumDir = base / "steamapps" / "common" / "SCUM";
    if (std::filesystem::exists(scumDir)) {
        std::error_code ec;
        int suspiciousHits = 0;

        for (auto it = std::filesystem::recursive_directory_iterator(scumDir, std::filesystem::directory_options::skip_permission_denied, ec);
             it != std::filesystem::recursive_directory_iterator(); it.increment(ec)) {
            if (ec) {
                ec.clear();
                continue;
            }
            if (!it->is_regular_file(ec) || ec) {
                ec.clear();
                continue;
            }

            auto lowerPath = ns::ToLowerCopy(it->path().wstring());
            auto lowerName = ns::ToLowerCopy(it->path().filename().wstring());
            if (!ns::ContainsAnyNeedle(lowerName, rules.strings) && !ns::ContainsAnyNeedle(lowerPath, rules.paths)) {
                continue;
            }

            ++suspiciousHits;
            ns::PushFinding(
                out.findings,
                L"Steam",
                L"Suspicious artifact near game files",
                L"File near SCUM installation matched signature rules",
                it->path().filename().wstring(),
                it->path().wstring(),
                NS_SEVERITY_MEDIUM,
                68);
        }

        if (suspiciousHits == 0) {
            ns::PushFinding(
                out.findings,
                L"Steam",
                L"No suspicious near-game artifacts by rules",
                L"SCUM directory scan completed",
                scumDir.wstring(),
                scumDir.wstring(),
                NS_SEVERITY_INFO,
                5);
        }
    }
}

} // namespace

namespace ns {

ModuleOutput RunSteamScanner(const RuleSet& rules) {
    ModuleOutput out;
    auto roots = GetSteamRoots();

    for (const auto& r : roots) {
        try {
            ScanSteamRoot(r, rules, out);
        } catch (...) {
            out.errors++;
        }
    }

    return out;
}

} // namespace ns
