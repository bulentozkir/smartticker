# SmartTicker Privacy Policy

**Last updated: 30 August 2026**

## Summary

SmartTicker is a desktop application that displays stock, ETF, commodity, and news information from public web pages that **you** configure. It has no user accounts, collects no analytics, and sends no data about you or your usage to the developer or any third party. All of your configuration stays on your own computer.

## Information We Collect

**We collect nothing.** The developer of SmartTicker operates no servers, receives no data from the application, and has no ability to identify you or observe how you use it.

SmartTicker contains no telemetry, no usage analytics, no crash or error reporting, no advertising identifiers, and no automatic update or "phone home" checks.

## Information Stored on Your Device

SmartTicker saves your configuration locally, in these two files:

- `%LocalAppData%\SmartTicker\settings.json`
- `%LocalAppData%\SmartTicker\alerts.json`

These contain only:

- The web page addresses (URLs) and ticker symbols you choose to track
- Optional CSS selectors used to locate values on those pages
- Your price alert rules, such as threshold values and expiry dates
- Display preferences: colours, transparency, scroll speed, row counts, refresh intervals, and language

This data never leaves your computer. It is not uploaded, synchronised, backed up to any cloud service, or transmitted to the developer. You can view, edit, or delete these files at any time, and deleting them resets the application to its defaults.

## Network Connections

SmartTicker makes outbound network requests in only three situations:

**1. Fetching the sources you configure.** At the refresh interval you choose, SmartTicker requests the public web pages you have added. Source-page requests use HTTP GET. Website cookie storage and cross-host redirects are disabled by default, and each website must be approved inside SmartTicker before it is requested. If you enable "Allow website cookies and cross-host redirects," SmartTicker skips that per-source approval step, accepts cookies set by websites into an isolated in-memory cookie container until exit, and follows redirects to other hosts. If a response is positively identified as a privacy/cookie consent form with both Accept and Reject controls, SmartTicker pauses and displays its text and choices. It sends the form's hidden fields and the exact choice only after you click Accept or Reject; Cancel sends nothing. SmartTicker never reads browser cookies, persists website cookies, sends credentials, or submits sign-in forms. Requests identify the application with the user agent `SmartTicker/0.1 (+local desktop public HTML reader)`.

**2. Downloading the optional starter configuration.** If — and only if — you choose the "download starter quotes" option, SmartTicker retrieves a sample configuration file from `raw.githubusercontent.com`. This is a manual, one-off action that you initiate. No information about you is sent with this request. If you never use this option, this connection never occurs.

**3. Displaying online help.** When you open the Help window or click Reload, SmartTicker requests `HELPME.md` from `raw.githubusercontent.com` so it can display the latest published guide. If the request fails, it displays the copy embedded in the application. Clicking Open online sends the same address to your default browser. No application settings or usage data are included in the request.

## Third-Party Websites

The sources you add to SmartTicker are operated by third parties, not by us. When SmartTicker requests a page, that website receives the request in the normal way a web browser visit would — including your IP address — and handles it under **its own privacy policy**, not this one. You choose which sites to add, and you are responsible for reviewing their terms and privacy practices, and for respecting their access rules.

## Data Sharing and Sale

We do not share, sell, rent, or disclose any personal information, because we do not collect or receive any.

## Data Retention

Since no data reaches us, we retain nothing. Data stored on your device remains until you delete it or uninstall the application.

## Permissions

SmartTicker requests two Windows capabilities:

- **Internet (client)** — to fetch the pages you configure
- **Run full trust** — required for a standard Windows desktop application

It does not request access to your location, camera, microphone, contacts, documents, or any other personal data.

## Children's Privacy

SmartTicker is not directed at children and collects no personal information from anyone, including children under 13.

## Changes to This Policy

If this policy changes, the updated version will be published at this address with a revised "Last updated" date.

## Contact

Questions about this policy can be raised at:
<https://github.com/bulentozkir/smartticker/issues>
