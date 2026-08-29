# SmartTicker for Windows

This directory contains the Windows application, tests, Microsoft Store-compatible packaging, and Windows release tooling.

The application uses C# with .NET 10 and Avalonia. Generated packages are written to the repository-root `releases/windows/` directory and are not committed to Git.

## Planned outputs

- Self-contained Windows x64 and ARM64 portable builds
- Architecture-specific MSIX packages
- An x64/ARM64 MSIX bundle for Microsoft Partner Center

Microsoft Store identity values remain development placeholders until the app name is reserved in Partner Center. Signing certificates and passwords must never be committed.
