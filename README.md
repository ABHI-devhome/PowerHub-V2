# PowerHub

<p align="center">
  <img src="logo.png" alt="PowerHub Logo" width="150" />
</p>

**PowerHub** is a native Windows desktop app that helps you manage **Windows power plans**, **Windows 11 power modes (overlay)**, **display refresh rates**, and optional **system tweaks**—without extra dependencies beyond the .NET Framework runtime used by the WPF UI.

## What's new (v2)

- **WPF + MVVM** shell with a task-first layout: **Home**, **Power plans**, **Display**, **Performance**, **Activity**, **About**.
- **Responsive navigation**: compact rail under ~900px width; PerMonitorV2 DPI awareness via `app.manifest`.
- **Activity log** for recent operations in the current session.
- Core logic lives in **`src/PowerHub.Core`**; the UI is **`src/PowerHub.Wpf`**.

## Requirements

- Windows 10 / 11 (x64 recommended)
- **.NET SDK** (to build from CLI), or **Visual Studio** with .NET desktop development workload  
  - Target: **.NET Framework 4.8** (`net48`) for the WPF app.

## Build

From the repo root:

```bat
build.bat
```

Or:

```bash
dotnet build PowerHub.sln -c Release
```

Output (default):

`src\PowerHub.Wpf\bin\Release\net48\PowerHub.exe`

## Run

1. Build as above.
2. Run `PowerHub.exe` **as Administrator** (UAC). The app will prompt to elevate if not already admin—required for `powercfg`, display changes, and many tweaks.

## Features

- **Duplicate plan prevention** patterns for unlocking built-in schemes without cluttering Control Panel.
- **Refresh rate** selection via native `ChangeDisplaySettings`.
- **Windows 11-style power modes** (overlay) from Home quick actions.
- **Optimization tools** with confirmations on higher-risk actions.

## Legacy WinForms

The previous WinForms single-file build has been **replaced** by this WPF solution. Use `PowerHub.sln` going forward.

## License / author

See repository owner; GitHub link in-app on the About page.
