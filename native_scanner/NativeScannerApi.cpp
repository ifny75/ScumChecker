#ifndef NOMINMAX
#define NOMINMAX
#endif

#include "NativeScannerApi.h"

#include "DriverScanner.h"
#include "FileScanner.h"
#include "ProcessScanner.h"
#include "RegistryScanner.h"
#include "RiskAnalyzer.h"
#include "ScannerShared.h"
#include "SteamScanner.h"

#include <Windows.h>

#include <chrono>
#include <filesystem>
#include <future>
#include <vector>

namespace {

bool IsAdmin() {
    SID_IDENTIFIER_AUTHORITY ntAuthority = SECURITY_NT_AUTHORITY;
    PSID adminGroup = nullptr;
    if (!AllocateAndInitializeSid(
            &ntAuthority,
            2,
            SECURITY_BUILTIN_DOMAIN_RID,
            DOMAIN_ALIAS_RID_ADMINS,
            0, 0, 0, 0, 0, 0,
            &adminGroup)) {
        return false;
    }

    BOOL isMember = FALSE;
    CheckTokenMembership(nullptr, adminGroup, &isMember);
    FreeSid(adminGroup);
    return isMember == TRUE;
}

void MergeOutput(const ns::ModuleOutput& mo, NsReport* report, std::vector<NsFinding>* findings) {
    report->files_checked += mo.files_checked;
    report->drivers_checked += mo.drivers_checked;
    report->services_checked += mo.services_checked;
    report->processes_checked += mo.processes_checked;
    report->registry_values_checked += mo.registry_values_checked;
    report->steam_accounts_checked += mo.steam_accounts_checked;
    report->errors += mo.errors;

    findings->insert(findings->end(), mo.findings.begin(), mo.findings.end());
}

} // namespace

extern "C" __declspec(dllexport) BOOL __stdcall Ns_IsAdministrator() {
    return IsAdmin() ? TRUE : FALSE;
}

extern "C" __declspec(dllexport) int __stdcall Ns_RunDriverMemoryScan(
    NsFinding* findings_buffer,
    int findings_capacity,
    NsReport* out_report) {
    if (!out_report || findings_capacity < 0) {
        return NS_STATUS_INVALID_ARGUMENT;
    }

    *out_report = {};
    out_report->status = NS_STATUS_OK;
    out_report->ran_as_admin = IsAdmin() ? 1 : 0;

    const auto started = std::chrono::steady_clock::now();
    std::vector<NsFinding> findings;
    findings.reserve(static_cast<size_t>(findings_capacity > 0 ? findings_capacity : 512));

    try {
        const auto rulesPath = (std::filesystem::path(ns::GetModuleDirectory()) / L"rules.json").wstring();
        const auto rules = ns::LoadRulesFromJson(rulesPath);

        std::vector<std::future<ns::ModuleOutput>> jobs;
        jobs.emplace_back(std::async(std::launch::async, [&rules]() { return ns::RunFileScanner(rules); }));
        jobs.emplace_back(std::async(std::launch::async, [&rules]() { return ns::RunProcessScanner(rules); }));
        jobs.emplace_back(std::async(std::launch::async, [&rules, out_report]() { return ns::RunDriverScanner(rules, out_report->ran_as_admin == 1); }));
        jobs.emplace_back(std::async(std::launch::async, [&rules]() { return ns::RunRegistryScanner(rules); }));
        jobs.emplace_back(std::async(std::launch::async, [&rules]() { return ns::RunSteamScanner(rules); }));

        for (auto& job : jobs) {
            try {
                auto mo = job.get();
                MergeOutput(mo, out_report, &findings);
            } catch (...) {
                out_report->errors++;
            }
        }

        ns::ApplyRiskAnalysis(findings);
    } catch (...) {
        out_report->errors++;
        out_report->status = NS_STATUS_INTERNAL_ERROR;
    }

    if (findings_buffer && findings_capacity > 0 && !findings.empty()) {
        const int copyCount = std::min<int>(findings_capacity, static_cast<int>(findings.size()));
        for (int i = 0; i < copyCount; ++i) {
            findings_buffer[i] = findings[static_cast<size_t>(i)];
        }
        out_report->findings_returned = copyCount;
    } else {
        out_report->findings_returned = 0;
    }

    const auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(
        std::chrono::steady_clock::now() - started);
    out_report->elapsed_ms = static_cast<int>(elapsed.count());

    return out_report->status;
}
