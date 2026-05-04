#pragma once

#include <Windows.h>

extern "C" {

enum NsStatus : int {
    NS_STATUS_OK = 0,
    NS_STATUS_INVALID_ARGUMENT = 1,
    NS_STATUS_INTERNAL_ERROR = 2
};

enum NsSeverity : int {
    NS_SEVERITY_INFO = 0,
    NS_SEVERITY_LOW = 1,
    NS_SEVERITY_MEDIUM = 2,
    NS_SEVERITY_HIGH = 3
};

struct NsFinding {
    wchar_t category[64];
    wchar_t title[128];
    wchar_t reason[256];
    wchar_t details[512];
    wchar_t evidence_path[520];
    int severity;
    int score;
};

struct NsReport {
    int status;
    int findings_returned;
    int files_checked;
    int drivers_checked;
    int services_checked;
    int processes_checked;
    int registry_values_checked;
    int steam_accounts_checked;
    int errors;
    int ran_as_admin;
    int elapsed_ms;
};

__declspec(dllexport) BOOL __stdcall Ns_IsAdministrator();
__declspec(dllexport) int __stdcall Ns_RunDriverMemoryScan(
    NsFinding* findings_buffer,
    int findings_capacity,
    NsReport* out_report);

}
