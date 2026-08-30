# SmartTicker

<img src="windows/packaging/Assets/AppIcon256.png" alt="SmartTicker logo" width="128" />

SmartTicker is a compact, always-on-top desktop ticker. It shows a scrolling line of prices and a second line of news headlines, drawn from public web pages that you choose yourself.

Unlike apps tied to a single data provider, SmartTicker lets you point each entry at any public page. Add a symbol, paste the page address, and SmartTicker reads the value from it. Stocks, ETFs, indices, commodities, currencies and crypto all work the same way, because you decide where each number comes from.

Built with C# on .NET 10 and Avalonia UI, so the Windows, Linux and macOS builds share the same portable application layers.

## Features

**Quotes and news**

- Always-on-top window with separate scrolling lines for prices and news
- Track any quote from a public web page you choose yourself
- Automatic value detection, or pick an exact element with a CSS selector
- After-hours prices and daily change percentages
- Clickable headlines that open in your browser
- Independent refresh intervals for prices and news, from 30 to 300 seconds

**Price alerts**

- Rules per quote using less than, greater than, equal to and other comparisons
- Optional start date, end date, or no expiry at all
- Flashing high-contrast highlight and an audible buzz when a rule fires, both for a duration you set
- Rules can be edited, disabled temporarily, or removed
- Stored in their own file, separate from the rest of your settings

**Appearance**

- One to eight rows per line, with adjustable scrolling speed
- Adjustable window transparency
- Configurable colours for the background, quote names, prices, after-hours values, rising and falling changes, and four separate news colours

**Everything else**

- Available in 16 languages, switchable from the right-click menu
- Optional automatic start when you sign in
- Export and import for both settings and alert rules
- No accounts, no sign-in, no cloud sync
- No telemetry, analytics or crash reporting; configuration is stored locally as ordinary JSON

See [PRIVACY.md](PRIVACY.md) for exactly what is stored and what leaves your computer.

## Screenshots

The ticker itself: two rows of prices above two rows of headlines. The highlighted quote is a fired price alert.

![SmartTicker ticker window](docs/screenshots/ticker.png)

Everything is reachable from the right-click menu, including the language picker.

![Right-click menu with the language submenu open](docs/screenshots/menu-language.png)

Quotes are added one symbol and source at a time, with optional CSS selectors and a discovery helper for finding the right element.

![Quotes window showing the entry form and configured entries](docs/screenshots/quotes.png)

App Settings covers rows, scrolling speed, refresh intervals, start at sign-in, window transparency and every colour.

![App Settings window](docs/screenshots/app-settings.png)

Alert rules are created per quote, with an optional schedule, a buzz count and a blink duration.

![Alerts window with a configured rule](docs/screenshots/alerts.png)

Settings and alert rules can be exported and imported independently.

![Backup section with export and import buttons](docs/screenshots/backup.png)

## Release 1.0.0

First public release, available as MSIX and MSI for Windows (x64 and arm64), a portable ZIP, and a `.deb` for Debian-based Linux. See [releases](releases/README.md).

Website policies, robots directives, crawl delays, and server-requested backoff take precedence over configured refresh intervals. JavaScript-only pages and pages that prohibit automated access may not be supported.

## Data and financial disclaimer

SmartTicker does not provide real-time market data, trading services, or investment advice. Extracted values can be delayed, stale, incomplete, or incorrect. Always verify financial information with an authoritative source before making a decision.

Users are responsible for ensuring that their use of each webpage or feed complies with its terms, licenses, robots directives, and applicable law.

## Repository layout

- [windows](windows/README.md) contains the application source, the tests, and the MSIX and MSI packaging. The Avalonia project here is shared, not Windows-only: the Linux build publishes the same project, so the folder name reflects the order the platforms were built rather than a split codebase.
- [linux-debian](linux-debian/README.md) holds the scripts that publish that project as `linux-x64` and pack it into a `.deb`.
- [macosx](macosx/README.md) is a placeholder. No macOS build exists yet.
- [releases](releases/README.md) is the local and CI output contract for generated packages.

## Development status

Windows is complete and packaged as MSIX and MSI for x64 and arm64. Debian-based Linux is packaged as an `amd64` `.deb`. macOS is not built. Build instructions are maintained in each platform directory.

## Ownership

Bulent Ozkir (bulentozkir@hotmail.com) is the patent and license owner.

No patent number, application status, or grant status is asserted by this notice.

## License

SmartTicker is available for non-commercial use under the PolyForm Noncommercial License 1.0.0. See [LICENSE](LICENSE) for the complete terms and [NOTICE.md](NOTICE.md) for the required ownership notice.

Commercial use is not permitted under this license. Contact the license owner to discuss separate commercial licensing.
