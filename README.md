# SmartTicker

SmartTicker is a compact, two-line desktop ticker for tracking delayed prices for stocks, ETFs, commodities, and market news.

The first release targets Windows. The application is planned in C# with .NET 10 and Avalonia UI so that Linux and macOS versions can follow without replacing the portable application layers.

## Planned Windows MVP

- Borderless, always-on-top two-line window
- Delayed price discovery from user-provided public webpages
- User-provided ticker and display names
- Automatic static-HTML price detection with an optional CSS selector
- Price refresh every 1 minute by default
- RSS/Atom headline refresh every 5 minutes by default
- Right-click menu for settings, refresh, pause/resume, always-on-top, About, and exit
- Local-only configuration and cached state

Website policies, robots directives, crawl delays, and server-requested backoff take precedence over configured refresh intervals. JavaScript-only pages and pages that prohibit automated access may not be supported.

## Data and financial disclaimer

SmartTicker does not provide real-time market data, trading services, or investment advice. Extracted values can be delayed, stale, incomplete, or incorrect. Always verify financial information with an authoritative source before making a decision.

Users are responsible for ensuring that their use of each webpage or feed complies with its terms, licenses, robots directives, and applicable law.

## Development status

The repository is in its initial planning and scaffolding stage. Build and contribution instructions will be added with the first implementation.

## Ownership

Bulent Ozkir (bulentozkir@hotmail.com) is the patent and license owner.

No patent number, application status, or grant status is asserted by this notice.

## License

SmartTicker is available for non-commercial use under the PolyForm Noncommercial License 1.0.0. See [LICENSE](LICENSE) for the complete terms and [NOTICE.md](NOTICE.md) for the required ownership notice.

Commercial use is not permitted under this license. Contact the license owner to discuss separate commercial licensing.
