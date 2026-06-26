#pragma once

#ifndef NOMINMAX
#define NOMINMAX
#endif

#include "NativeScannerApi.h"

#include <Windows.h>

#include <optional>
#include <string>
#include <vector>

namespace ns {

struct RuleSet {
    std::vector<std::wstring> strings;
    std::vector<std::wstring> paths;
    std::vector<std::wstring> hashes;
};

struct ModuleOutput {
    std::vector<NsFinding> findings;
    int files_checked = 0;
    int services_checked = 0;
    int processes_checked = 0;
    int drivers_checked = 0;
    int registry_values_checked = 0;
    int steam_accounts_checked = 0;
    int errors = 0;
};

std::wstring ToLowerCopy(const std::wstring& value);
void WriteBounded(wchar_t* out, size_t outCount, const std::wstring& text);
std::wstring Basename(const std::wstring& path);
bool ContainsAnyNeedle(const std::wstring& value, const std::vector<std::wstring>& needles);

void PushFinding(
    std::vector<NsFinding>& findings,
    const std::wstring& category,
    const std::wstring& title,
    const std::wstring& reason,
    const std::wstring& details,
    const std::wstring& evidence,
    int severity,
    int score);

bool VerifyFileSignature(const std::wstring& path);
bool IsUserSpacePath(const std::wstring& pathLower);
std::wstring TrimQuotesAndArgs(const std::wstring& src);
std::optional<std::wstring> QueryStringValue(HKEY hKey, const wchar_t* valueName);
std::wstring ExpandEnvironment(const std::wstring& src);
bool PathExistsFile(const std::wstring& path);

std::wstring GetModuleDirectory();
std::wstring ComputeSha256Hex(const std::wstring& path, size_t maxBytesToRead = 0);
RuleSet LoadRulesFromJson(const std::wstring& filePath);

} // namespace ns
