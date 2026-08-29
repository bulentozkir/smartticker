# SmartTicker release artifacts

Build and packaging scripts write generated artifacts beneath `releases/windows/<version>/`.

Generate them with:

```powershell
pwsh -NoProfile -File windows/packaging/Build-Release.ps1 -Version 1.0.0
```

Expected layout:

- `portable/` — self-contained x64 and ARM64 ZIP archives
- `msix/layout/<runtime>/` — staged MSIX payload plus generated `AppxManifest.xml`
- `msix/` — architecture-specific signed or unsigned development MSIX packages
- `store/` — Microsoft Store submission bundles
- `checksums/SHA256SUMS.txt` — SHA-256 hashes
- `symbols/` — optional debugging symbols

Packing `.msix` files requires `makeappx.exe` from the Windows SDK. When the SDK is absent the
script still stages the layout and reports the remaining command, so packing can be finished on a
machine that has the SDK installed.

Store identity (`Name`, `Publisher`, `PublisherDisplayName`) in
`windows/packaging/Package.appxmanifest` is a development placeholder and must be replaced with the
values reserved in Microsoft Partner Center before submission.

Generated binaries are intentionally excluded from Git history. Publish them through CI artifacts, repository releases, or Microsoft Partner Center. Keep this README tracked so the output contract remains documented.
