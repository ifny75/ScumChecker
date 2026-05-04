#ifndef NOMINMAX
#define NOMINMAX
#endif

#include "FileScanner.h"

#include <Windows.h>

#include <array>
#include <chrono>
#include <future>
#include <filesystem>
#include <mutex>
#include <algorithm>
#include <unordered_map>
#include <unordered_set>

namespace {

std::mutex g_cacheMutex;
std::unordered_map<std::wstring, unsigned long long> g_fileFingerprintCache;

unsigned long long ReadFingerprint(const std::wstring& path, bool* ok = nullptr) {
    WIN32_FILE_ATTRIBUTE_DATA fad{};
    if (!GetFileAttributesExW(path.c_str(), GetFileExInfoStandard, &fad)) {
        if (ok) *ok = false;
        return 0;
    }

    ULARGE_INTEGER sz{};
    sz.HighPart = fad.nFileSizeHigh;
    sz.LowPart = fad.nFileSizeLow;

    ULARGE_INTEGER lw{};
    lw.HighPart = fad.ftLastWriteTime.dwHighDateTime;
    lw.LowPart = fad.ftLastWriteTime.dwLowDateTime;

    if (ok) *ok = true;
    return (sz.QuadPart * 1315423911ull) ^ lw.QuadPart;
}

int DaysSinceCreation(const std::wstring& path) {
    WIN32_FILE_ATTRIBUTE_DATA fad{};
    if (!GetFileAttributesExW(path.c_str(), GetFileExInfoStandard, &fad)) {
        return 9999;
    }

    FILETIME nowFt{};
    GetSystemTimeAsFileTime(&nowFt);

    ULARGE_INTEGER created{};
    created.HighPart = fad.ftCreationTime.dwHighDateTime;
    created.LowPart = fad.ftCreationTime.dwLowDateTime;

    ULARGE_INTEGER now{};
    now.HighPart = nowFt.dwHighDateTime;
    now.LowPart = nowFt.dwLowDateTime;

    if (now.QuadPart <= created.QuadPart) return 0;
    const auto diff100ns = now.QuadPart - created.QuadPart;
    const auto days = static_cast<int>(diff100ns / (10ull * 1000ull * 1000ull * 60ull * 60ull * 24ull));
    return days;
}

bool IsInterestingExtension(const std::wstring& extLower) {
    return extLower == L".exe" || extLower == L".dll" || extLower == L".sys";
}

bool IsMasqueradingSystemName(const std::wstring& exeLower) {
    static const std::unordered_set<std::wstring> names = {
        L"svchost.exe", L"lsass.exe", L"csrss.exe", L"winlogon.exe", L"services.exe", L"smss.exe"
    };
    return names.find(exeLower) != names.end();
}

std::vector<std::wstring> BuildRoots() {
    std::vector<std::wstring> roots;
    std::array<wchar_t, 4096> buf{};

    if (GetEnvironmentVariableW(L"APPDATA", buf.data(), static_cast<DWORD>(buf.size())) > 0) {
        roots.emplace_back(buf.data());
    }
    if (GetEnvironmentVariableW(L"LOCALAPPDATA", buf.data(), static_cast<DWORD>(buf.size())) > 0) {
        roots.emplace_back(buf.data());
    }
    if (GetEnvironmentVariableW(L"TEMP", buf.data(), static_cast<DWORD>(buf.size())) > 0) {
        roots.emplace_back(buf.data());
    }

    std::vector<std::wstring> unique;
    std::unordered_set<std::wstring> seen;
    for (auto& r : roots) {
        if (r.empty()) continue;
        auto low = ns::ToLowerCopy(r);
        if (seen.insert(low).second && std::filesystem::exists(std::filesystem::path(r))) {
            unique.push_back(r);
        }
    }

    return unique;
}

void ScanRoot(const std::wstring& root, const ns::RuleSet& rules, ns::ModuleOutput& out) {
    std::error_code ec;
    std::filesystem::recursive_directory_iterator it(
        std::filesystem::path(root),
        std::filesystem::directory_options::skip_permission_denied,
        ec);
    std::filesystem::recursive_directory_iterator end;

    for (; it != end; it.increment(ec)) {
        if (ec) {
            out.errors++;
            ec.clear();
            continue;
        }

        if (!it->is_regular_file(ec) || ec) {
            ec.clear();
            continue;
        }

        const auto path = it->path().wstring();
        const auto pathLower = ns::ToLowerCopy(path);
        const auto extLower = ns::ToLowerCopy(it->path().extension().wstring());

        if (!IsInterestingExtension(extLower)) continue;

        out.files_checked++;

        bool fpOk = false;
        const auto fingerprint = ReadFingerprint(path, &fpOk);
        if (fpOk) {
            std::lock_guard<std::mutex> lock(g_cacheMutex);
            auto found = g_fileFingerprintCache.find(pathLower);
            if (found != g_fileFingerprintCache.end() && found->second == fingerprint) {
                continue;
            }
            g_fileFingerprintCache[pathLower] = fingerprint;
        }

        const auto baseLower = ns::ToLowerCopy(ns::Basename(path));
        const bool signedOk = ns::VerifyFileSignature(path);
        const int daysOld = DaysSinceCreation(path);
        const bool recentlyCreated = daysOld <= 3;

        bool rulesPathHit = ns::ContainsAnyNeedle(pathLower, rules.paths);
        bool rulesStringHit = ns::ContainsAnyNeedle(pathLower, rules.strings);

        if (!rules.hashes.empty()) {
            auto hash = ns::ComputeSha256Hex(path, 2 * 1024 * 1024);
            if (!hash.empty()) {
                auto hashLower = ns::ToLowerCopy(hash);
                if (std::find(rules.hashes.begin(), rules.hashes.end(), hashLower) != rules.hashes.end()) {
                    ns::PushFinding(
                        out.findings,
                        L"File",
                        L"File hash matched rule",
                        L"SHA256 hash is present in rules.json",
                        hashLower,
                        path,
                        NS_SEVERITY_HIGH,
                        98);
                }
            }
        }

        if (!signedOk && ns::IsUserSpacePath(pathLower)) {
            ns::PushFinding(
                out.findings,
                L"File",
                L"Unsigned executable in user-space",
                L"Executable/DLL/SYS in temp/appdata without valid signature",
                ns::Basename(path),
                path,
                extLower == L".sys" ? NS_SEVERITY_HIGH : NS_SEVERITY_MEDIUM,
                extLower == L".sys" ? 90 : 78);
        }

        if (recentlyCreated && (extLower == L".exe" || extLower == L".dll")) {
            ns::PushFinding(
                out.findings,
                L"File",
                L"Recently created executable artifact",
                L"Binary artifact created in the last 3 days",
                L"DaysOld=" + std::to_wstring(daysOld),
                path,
                NS_SEVERITY_LOW,
                52);
        }

        if (IsMasqueradingSystemName(baseLower) &&
            pathLower.find(L"\\windows\\system32\\") == std::wstring::npos) {
            ns::PushFinding(
                out.findings,
                L"File",
                L"System-process name outside trusted directory",
                L"Masquerading executable name not located in System32",
                ns::Basename(path),
                path,
                NS_SEVERITY_HIGH,
                92);
        }

        if (rulesPathHit || rulesStringHit) {
            ns::PushFinding(
                out.findings,
                L"File",
                L"File path/name matched signature rules",
                L"Matched string/path pattern from rules.json",
                ns::Basename(path),
                path,
                NS_SEVERITY_MEDIUM,
                74);
        }
    }
}

} // namespace

namespace ns {

ModuleOutput RunFileScanner(const RuleSet& rules) {
    ModuleOutput merged;
    auto roots = BuildRoots();
    if (roots.empty()) {
        return merged;
    }

    std::vector<std::future<ModuleOutput>> tasks;
    tasks.reserve(roots.size());

    for (const auto& r : roots) {
        tasks.push_back(std::async(std::launch::async, [r, &rules]() {
            ModuleOutput mo;
            try {
                ScanRoot(r, rules, mo);
            } catch (...) {
                mo.errors++;
            }
            return mo;
        }));
    }

    for (auto& t : tasks) {
        auto mo = t.get();
        merged.files_checked += mo.files_checked;
        merged.errors += mo.errors;
        merged.findings.insert(merged.findings.end(), mo.findings.begin(), mo.findings.end());
    }

    return merged;
}

} // namespace ns
