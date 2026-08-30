#!/usr/bin/env bash
# Builds the SmartTicker .deb. Staging happens inside the WSL filesystem because
# DrvFs under /mnt/c does not reliably carry the executable permission bit.
set -euo pipefail

VERSION="$1"
PUBLISH_DIR="$2"   # /mnt/c/... path to the linux-x64 publish output
OUTPUT_DIR="$3"    # /mnt/c/... path for the finished .deb
ICON_SOURCE="${4:-}"

PKG="smartticker"
ARCH="amd64"
STAGE="$(mktemp -d)"
ROOT="${STAGE}/${PKG}_${VERSION}_${ARCH}"

trap 'rm -rf "${STAGE}"' EXIT

mkdir -p "${ROOT}/DEBIAN"
mkdir -p "${ROOT}/opt/${PKG}"
mkdir -p "${ROOT}/usr/bin"
mkdir -p "${ROOT}/usr/share/applications"
mkdir -p "${ROOT}/usr/share/pixmaps"

cp -r "${PUBLISH_DIR}/." "${ROOT}/opt/${PKG}/"

# Everything arrives from DrvFs as 777; reset to sane modes and mark the binaries.
find "${ROOT}/opt/${PKG}" -type f -exec chmod 644 {} +
find "${ROOT}/opt/${PKG}" -type d -exec chmod 755 {} +
chmod 755 "${ROOT}/opt/${PKG}/SmartTicker.Desktop"
find "${ROOT}/opt/${PKG}" -type f -name '*.so' -exec chmod 755 {} +

if [ -n "${ICON_SOURCE}" ] && [ -f "${ICON_SOURCE}" ]; then
  cp "${ICON_SOURCE}" "${ROOT}/usr/share/pixmaps/${PKG}.png"
  chmod 644 "${ROOT}/usr/share/pixmaps/${PKG}.png"
fi

ln -s "/opt/${PKG}/SmartTicker.Desktop" "${ROOT}/usr/bin/${PKG}"

INSTALLED_KB="$(du -sk "${ROOT}/opt" | cut -f1)"

cat > "${ROOT}/DEBIAN/control" <<EOF
Package: ${PKG}
Version: ${VERSION}
Section: utils
Priority: optional
Architecture: ${ARCH}
Maintainer: SmartTicker <noreply@smartticker.invalid>
Installed-Size: ${INSTALLED_KB}
Depends: libc6, libgcc-s1, libstdc++6, zlib1g, libx11-6, libice6, libsm6, libfontconfig1, libglib2.0-0
Description: Desktop price and news ticker
 SmartTicker shows configurable stock, ETF and commodity prices together with
 scrolling headlines collected from public static HTML pages.
 .
 The build is self-contained: the .NET runtime ships inside the package.
EOF
chmod 644 "${ROOT}/DEBIAN/control"

cat > "${ROOT}/usr/share/applications/${PKG}.desktop" <<EOF
[Desktop Entry]
Type=Application
Name=SmartTicker
Comment=Price and news ticker
Exec=/opt/${PKG}/SmartTicker.Desktop
Icon=${PKG}
Terminal=false
Categories=Office;Finance;
EOF
chmod 644 "${ROOT}/usr/share/applications/${PKG}.desktop"

mkdir -p "${OUTPUT_DIR}"
DEB="${OUTPUT_DIR}/${PKG}_${VERSION}_${ARCH}.deb"

# --root-owner-group gives every file root:root without needing fakeroot.
dpkg-deb --root-owner-group --build "${ROOT}" "${DEB}" >/dev/null

echo "BUILT ${DEB}"
