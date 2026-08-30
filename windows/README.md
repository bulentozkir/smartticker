# SmartTicker for Windows

This directory contains the Windows application, tests, Microsoft Store-compatible packaging, and Windows release tooling.

The application uses C# with .NET 10 and Avalonia. Generated packages are written to the repository-root `releases/windows/` directory and are not committed to Git.

## Outputs

- Architecture-specific MSIX packages for x64 and ARM64
- A combined x64/ARM64 `.msixbundle` for Microsoft Partner Center
- Architecture-specific MSI installers

The Store identity in `packaging/Package.appxmanifest` is the one reserved in Partner Center and must not be edited: the Store rejects packages whose identity does not match. Signing certificates and passwords must never be committed.
