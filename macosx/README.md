# SmartTicker for macOS

This tracked placeholder is reserved for future macOS application-bundle, signing, notarization, and DMG packaging resources. Nothing is built here yet.

The application in [windows/src](../windows/src) is portable Avalonia and is already shared with the Linux `.deb`, so the outstanding work is packaging and notarization rather than a port. One functional gap is known: `StartupRegistrationFactory` wires up autostart for Windows and Linux only, so start-at-login would report as unsupported on macOS until a launch-agent implementation is added.
