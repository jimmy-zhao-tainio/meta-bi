# Releases

Release packages are Windows x64 offline installer zips.

## Build Local Zips

From `../meta`:

```powershell
cmd /c Meta\Installer\package-offline.cmd
```

Produces:

```text
Meta\Installer\bin\publish\meta-offline-win-x64-yyyy-MM-dd.zip
```

From `meta-bi`:

```powershell
cmd /c MetaInstaller\Installer\package-offline.cmd
```

The default `meta-bi` package is the small release path: framework-dependent Windows x64 executables with a shared DLL payload, without ReadyToRun precompilation. This requires the .NET 8 runtime on the target machine and keeps common dependencies in `payload\meta-bi\bin` only once.

To build a self-contained package for machines without .NET 8 installed, pass:

```powershell
cmd /c MetaInstaller\Installer\package-offline.cmd -SelfContained
```

To build the older compatibility shape with self-contained single-file executables, pass:

```powershell
cmd /c MetaInstaller\Installer\package-offline.cmd -SelfContained -SingleFile
```

To build a slower, larger package optimized for CLI startup time, add:

```powershell
cmd /c MetaInstaller\Installer\package-offline.cmd -ReadyToRun
```

The `meta-bi` package script expects the sibling upstream repo at `../meta` and packs fresh local foundation packages for the release restore. Use `-MetaRepo <path>` if the upstream repo is elsewhere.

Produces:

```text
MetaInstaller\Installer\bin\publish\meta-bi-offline-win-x64-yyyy-MM-dd-framework-dependent-shared.zip
```

Each zip contains the installer executable at the archive root and the offline payload under `payload\...\bin`.
The package scripts remove stale local zips for the same package family before writing the new zip, and use the built-in .NET zip API with fast compression instead of `Compress-Archive`.

## Sanity Check

Before uploading, extract each zip to a temporary directory and verify at least:

```powershell
.\payload\meta\bin\meta.exe help
.\payload\meta-bi\bin\meta-transform-script.exe help
```

The `meta-bi` zip includes the sanctioned model workspaces needed by the BI CLIs.
