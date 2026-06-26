cmake_minimum_required(VERSION 3.21)
project(scum_native_scanner LANGUAGES CXX)

add_library(scum_native_scanner SHARED
    NativeScannerApi.cpp
    NativeScannerApi.h
    ScannerShared.cpp
    ScannerShared.h
    FileScanner.cpp
    FileScanner.h
    ProcessScanner.cpp
    ProcessScanner.h
    DriverScanner.cpp
    DriverScanner.h
    RegistryScanner.cpp
    RegistryScanner.h
    SteamScanner.cpp
    SteamScanner.h
    RiskAnalyzer.cpp
    RiskAnalyzer.h
)

target_compile_features(scum_native_scanner PRIVATE cxx_std_20)
target_compile_definitions(scum_native_scanner PRIVATE
    UNICODE
    _UNICODE
    WIN32_LEAN_AND_MEAN
)
target_compile_options(scum_native_scanner PRIVATE /EHsc /utf-8)
target_link_libraries(scum_native_scanner PRIVATE psapi wintrust shlwapi bcrypt)
