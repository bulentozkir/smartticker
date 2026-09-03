# SmartTicker for Debian Linux

Debian packaging for SmartTicker. The application itself is not here: it lives in
[windows/src](../windows/src) and is shared across platforms. This folder only holds the
scripts that publish that project for `linux-x64` and pack the result into a `.deb`.

## Building from Windows

The publish step runs on Windows and `dpkg-deb` runs inside WSL, so a WSL distribution
with `dpkg-deb` available is required.

```powershell
./Build-Deb.ps1 -Version 1.0.3
```

Pass `-Distribution` if your WSL distribution is not named `Debian`. The finished package
and a SHA-256 checksum are written to `releases/linux/<version>/`.

## Building on Linux

`Build-Deb.ps1` is only a wrapper. Publish the project yourself and call the packing
script directly:

```bash
dotnet publish ../windows/src/SmartTicker.Desktop/SmartTicker.Desktop.csproj \
  --configuration Release --framework net10.0 --runtime linux-x64 --self-contained true \
  -p:Version=1.0.3 --output ./publish

bash build-deb.sh 1.0.3 "$PWD/publish" "$PWD/out" ../windows/packaging/Assets/AppIcon256.png
```

## What the package installs

| Path | Contents |
| --- | --- |
| `/opt/smartticker/` | The self-contained application, including the .NET runtime |
| `/usr/bin/smartticker` | Symlink to the executable |
| `/usr/share/applications/smartticker.desktop` | Desktop entry, categorised under Office and Finance |
| `/usr/share/pixmaps/smartticker.png` | Application icon |

Declared dependencies are only the shared libraries Avalonia needs: `libc6`, `libgcc-s1`,
`libstdc++6`, `zlib1g`, `libx11-6`, `libice6`, `libsm6`, `libfontconfig1` and
`libglib2.0-0`. The .NET runtime is deliberately absent from that list because the build
is self-contained.

Installing:

```bash
sudo apt install ./smartticker_1.0.3_amd64.deb
```

## Known limits

- Only `amd64` is produced. An `arm64` package needs a second publish runtime identifier
  and a matching `Architecture` in the control file.
- Start-at-login writes to `~/.config/autostart`, so it depends on the desktop environment
  honouring the XDG autostart specification.
