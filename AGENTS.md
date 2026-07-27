# AGENTS.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

This repository provides .NET MAUI bindings for the Facebook iOS SDK (v18.1.0), packaged as a NuGet package `Tonestro.Maui.FacebookSlim.iOS`. It wraps a subset of the Facebook iOS SDK functionality through a custom Swift wrapper (`MauiFacebook`) that exposes simplified APIs for CoreKit (analytics/events) and LoginKit (authentication).

## Architecture

The project has a three-layer architecture:

1. **Native Layer** (`native/`): Contains the XCode project `MauiFacebook.xcodeproj` that:
   - Links against Facebook iOS SDK XCFrameworks (downloaded from Facebook's GitHub releases)
   - Provides Swift wrapper classes (`CoreKitManagerSlim`, `LoginManagerSlim`) that simplify Facebook SDK APIs
   - Builds a framework that gets consumed by the binding project

2. **Binding Layer** (`Tonestro.Maui.FacebookSlim.iOS/`): A .NET iOS binding project that:
   - Uses `XCodeProjectReference` to build the native Swift framework
   - Contains hand-written binding definitions in `ApiDefinition.cs` (interface definitions with `[BaseType]`, `[Export]` attributes)
   - Contains enums in `StructsAndEnums.cs` that mirror Swift enums
   - Uses `CommunityToolkit.Maui.BindingExtensions` for binding generation

3. **Usage Checker** (`Tonestro.Maui.FacebookSlim.iOS.UsageChecker/`): A minimal iOS app that validates the bindings work correctly by:
   - Calling into the bound APIs
   - Enabling logging behaviors
   - Testing event logging and authentication flows

## Development Commands

### Prerequisites
- .NET SDK 8.0.408 (specified in `global.json`)
- XCode version specified in `.xcode-version` file
- Facebook iOS SDK dependencies must be downloaded first

### Download Facebook Dependencies
```bash
./native/deps/download-dependencies.sh
```
This downloads Facebook iOS SDK v18.1.0 static XCFrameworks from GitHub and extracts them to `native/deps/`.

### Build the Main Binding Project
```bash
cd Tonestro.Maui.FacebookSlim.iOS
dotnet workload restore
dotnet build -c Release
```

### Create NuGet Package
```bash
cd Tonestro.Maui.FacebookSlim.iOS
dotnet pack -c Release
```
The package is output to `nugetoutput/`.

### Build the Usage Checker
```bash
cd Tonestro.Maui.FacebookSlim.iOS.UsageChecker
dotnet build
```

### Regenerate Bindings with Sharpie
If the native Swift API changes, regenerate bindings:
```bash
cd Tonestro.Maui.FacebookSlim.iOS
./sharpie.sh
```
This builds the native framework and uses Objective Sharpie to generate new binding definitions. Manual adjustment of the generated files is typically required.

## CI/CD Pipelines

### Pull Request Pipeline (`azure-pipeline-pullrequests.yml`)
Runs on PRs to `main`:
1. Selects XCode version from `.xcode-version`
2. Restores .NET workloads
3. Downloads Facebook dependencies
4. Builds and packs the binding project
5. Copies package to `local-nugets/` (used as a local feed)
6. Builds the UsageChecker to validate bindings
7. Publishes artifacts

### Release Pipeline (`azure-pipeline-public-nuget-publish.yml`)
Triggered by tags matching `release-v*`:
1. Validates tag version matches `<Version>` in .csproj
2. Builds and packs the binding project
3. Publishes to nuget.org
4. Creates a GitHub release

## Key Files and Their Purposes

- `native/MauiFacebook/FacebookProxy.swift`: Swift wrapper classes that expose simplified Facebook SDK APIs
- `Tonestro.Maui.FacebookSlim.iOS/ApiDefinition.cs`: C# interface definitions with Objective-C binding attributes
- `Tonestro.Maui.FacebookSlim.iOS/StructsAndEnums.cs`: C# enums matching Swift enums
- `nuget.config`: Configures custom package sources (maui-binding-extensions from Azure DevOps, local-nugets)
- `.xcode-version`: Specifies required XCode version for builds

## Working with Bindings

When modifying the bindings:

1. **Native changes**: Edit `native/MauiFacebook/FacebookProxy.swift`, then rebuild the XCode project
2. **Binding changes**: Update `ApiDefinition.cs` or `StructsAndEnums.cs` to match native API changes
3. **Testing**: Always build the UsageChecker to validate bindings compile and link correctly
4. **Version bumping**: Update `<Version>` in `Tonestro.Maui.FacebookSlim.iOS.csproj` when making releases

The binding uses `[Export]` attributes to map C# method names to Objective-C selectors. The Swift code is exposed via `@objc` attributes which make it visible to the Objective-C runtime.

## Facebook SDK Dependencies

The project depends on these Facebook iOS SDK frameworks (all v18.1.0):
- FBAEMKit.xcframework
- FBSDKCoreKit.xcframework
- FBSDKCoreKit_Basics.xcframework
- FBSDKGamingServicesKit.xcframework
- FBSDKLoginKit.xcframework
- FBSDKShareKit.xcframework

These are downloaded as static XCFrameworks and linked into the native MauiFacebook framework.
