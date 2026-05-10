# Typedown Development Setup

This document describes the baseline development environment for building this repository reliably on a new machine.

## Required Tools

- Visual Studio 2022
- Git for Windows
- Node.js LTS
- Yarn
- .NET Core 3.1 SDK

## Required Visual Studio Components

Install Visual Studio 2022 with the Windows desktop and packaging toolchain needed by this repository.

Recommended components:

- .NET desktop development
- Universal Windows Platform development
- Windows Application Packaging Project / Desktop Bridge related components
- Windows 10/11 SDK `10.0.22621.0`

## Windows SDK Baseline

The repository currently targets Windows SDK `10.0.22621.0`.

Projects in this repository use:

- `TargetPlatformVersion = 10.0.22621.0`
- `TargetPlatformMinVersion = 10.0.19041.0`

For team consistency, every developer machine should install Windows SDK `10.0.22621.0`.

## Frontend Prerequisites

The editor frontend is located at `Dev/Typedown.Editor`.

Install Yarn globally if it is not already available:

```powershell
npm install -g yarn
```

Build the frontend before opening the desktop app in `Debug_Local` or `Release`:

```powershell
cd Dev\Typedown.Editor
yarn
yarn build
```

For `Debug` configuration, also run:

```powershell
yarn start
```

This serves the frontend from `http://localhost:3000`.

## NuGet Setup

The repository includes a root `NuGet.Config` that points to the local package source:

- `nupkgs`

Do not remove this source. Some builds depend on packages provided there.

## Solution Configurations

- `Debug`
  Uses the frontend dev server at `http://localhost:3000`
- `Debug_Local`
  Uses the built frontend assets from `Dev/Typedown/Resources/Statics`
- `Release`
  Used for release packaging

## Packaging Project

The repository contains a packaging project:

- `Tools/Typedown.Package/Typedown.Package.wapproj`

This project depends on UWP/Desktop Bridge packaging support being installed in Visual Studio. If packaging builds fail with missing `UAP.props`, missing SDK, or missing packaging targets, verify that:

- Windows SDK `10.0.22621.0` is installed
- UWP development support is installed
- Desktop Bridge / packaging components are installed

## Recommended First-Time Build Steps

1. Clone the repository.
2. Open a terminal in the repository root.
3. Build the frontend in `Dev/Typedown.Editor`.
4. Open `Typedown.sln` in Visual Studio 2022.
5. Select `Debug_Local|x64` for the first local build.
6. Build the solution.

## Troubleshooting

If Visual Studio still reports an older SDK version after project files are updated, clear local caches before rebuilding:

1. Close Visual Studio.
2. Delete the repository `.vs` directory.
3. Delete affected project `bin` and `obj` directories.
4. Reopen the solution and rebuild.
