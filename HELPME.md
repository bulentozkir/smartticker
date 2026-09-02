# SmartTicker Help

This guide applies to SmartTicker 1.0.3. It explains the main ticker, App Settings,
Quotes, alert rules, website permissions, backups, and common problems.

SmartTicker reads public static HTML from webpages that you configure. It does not
provide a market-data feed, and extracted information can be delayed, incomplete, or
wrong. Verify important financial information with an authoritative source.

## Quick navigation

| Area | Jump to |
| --- | --- |
| Getting started | [Open Help and configuration windows](#open-help-and-configuration-windows) |
| Main ticker | [Controls](#main-ticker-controls) · [Scrolling or static view](#choose-scrolling-or-static-quote-view) · [Move](#move-the-ticker) · [Resize](#resize-the-ticker) · [Pause](#pause-and-resume) · [Menu reference](#main-menu-reference) |
| Quotes and news | [Quotes](#quotes) · [Add an entry](#add-a-quote-or-news-entry) · [Group quotes](#group-quotes) · [Source URLs](#source-presets-and-urls) · [Selectors](#selector-field-reference) · [Discovery](#discover-selectors) · [Validation](#validate-a-source) |
| Application preferences | [App Settings](#app-settings) · [Rows and speed](#ticker-rows-and-speed) · [Startup](#start-smartticker-when-signing-in) · [Website access](#website-access) · [Appearance](#appearance) · [Backup and restore](#backup-and-restore) · [Edit config files](#edit-the-configuration-files-in-place) |
| Price alerts | [Alert rules](#alert-rules) · [Create a rule](#create-a-rule) · [Firing behavior](#when-a-rule-fires) · [Alert output](#alert-output-settings) · [Manage rules](#manage-configured-rules) |
| Data and support | [Local files and privacy](#local-files-and-privacy) · [Troubleshooting](#troubleshooting) · [Support](#support) |

## Open Help and configuration windows

Right-click the ticker to open its menu. The main configuration commands are:

- **Quotes...**: add, test, edit, order, and remove quote or news sources.
- **Quote groups...**: create, update, or delete groups and associate quotes with them.
- **Alerts**: create and manage price alert rules.
- **App Settings...**: configure rows, speeds, refresh intervals, startup, website
	access, colors, transparency, and backups.
- **View**: select one of four mutually exclusive combinations: scrolling or static,
	with Prices only or Prices with News.
- **Help**: open this guide inside SmartTicker.
- **About SmartTicker**: show the installed version and license notice.
- **Exit**: close SmartTicker completely.

The Help window checks the following online document whenever you open it:

<https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/HELPME.md>

If the online document cannot be downloaded, SmartTicker formats and displays the copy
embedded in your installed application. Close Help with its normal title-bar close
control.

## Main ticker controls

### Choose scrolling or static quote view

SmartTicker offers four mutually exclusive display modes. Right-click the ticker, open
**View**, and select one. The layout changes immediately and your choice is saved.

| View option | Result |
| --- | --- |
| **Left-to-right scroll: Prices only** | Price marquee in the main ticker; no news display. This is the default. |
| **Left-to-right scroll: Prices with News** | Price and news marquees in the main ticker. |
| **Static view: Prices only** | Responsive price tiles in the main window; no News window. |
| **Static view: Prices with News** | Responsive price tiles plus a separate static **SmartTicker News** window. |

Settings files created before these choices were added map to the matching combination
of their saved scrolling/static and news settings. Display mode is managed only from
the ticker's right-click **View** menu.

- In either scrolling mode, prices use the horizontal marquee and the configured price row
	count and scroll speed.
- In either static mode, groups appear as responsive tiles laid out from left to right. Tiles
  wrap onto another row only when the window is too narrow. Prices do not move
  automatically.
- Every quote tile has its own aligned **Symbol**, **Last**, **Chg**, and **Chg%**
  columns. **Chg** is derived
	from Last and Chg% because source pages provide a percentage selector rather than a
	separate absolute-change selector. It displays `—` when either value is unavailable.
- Select a group header to collapse or expand it. Groups follow the first occurrence
	of their quotes in the configured-entry order; rows within a group keep that order.
- Entries without a group appear under **Ungrouped**.
- Hover over Last to see available pre-market and after-hours values. Double-click a
	quote row to open its source page.
- Alert blinking and up/down colors work in both price modes.
- News opens automatically in a separate **SmartTicker News** window containing static
	**Symbol / Headline** group tiles. It does not marquee in static mode. The News
	window has a normal title bar and resize border, so the Quotes and News windows can
	be moved independently to different monitors. Double-click a headline row to open
	its source.
- On initial launch, News uses a compact 680×340 size. SmartTicker places it on another
	monitor when one is available; on a single monitor it first tries a free area below,
	right, above, or left of Prices. You can then move and resize it normally.
- Within each News group, headlines are interleaved by quote: one headline from the
	first quote, then one from the next quote, continuing in rounds. A quote with many
	headlines therefore cannot occupy the whole top of its group.
- Open the one-line **Show news for** dropdown and check or clear each quote
	independently. Any combination of quotes can be visible, including all or none. The
	button summarises the current choice, and entries include the quote and source so
	duplicate symbols remain independent. Cleared quotes are saved in your settings file
	as `hiddenNewsQuotes`, so they survive a restart and travel with a settings backup.
- Drag the dotted handle beside any quote or news tile heading and drop it on the left
	or right half of another tile. The order changes in both windows and is saved by
	reordering the underlying configured entries.
- A group with many rows scrolls inside its own bounded tile. The overall view scrolls
	vertically only when wrapped tile rows do not fit in the current window height.

Closing **SmartTicker News** does not disable news collection. To reopen it, right-click
the Prices window and select **View > Open static news window**. Selecting **Static
view: Prices only** closes it; selecting **Static view: Prices with News** opens it
again. Either scrolling choice closes the separate News window; the scrolling
Prices-with-News choice restores the news marquee in the main ticker.

Switching to static mode expands a short ticker window to a usable table height. You
can then resize it from any edge or corner.

### Move the ticker

Press and hold the vertical-dot grip at the top of the narrow left strip, drag the
ticker, and release the mouse button. Ticker text is not a drag surface, so selecting
or clicking content cannot accidentally start a window move.

### Resize the ticker

Move the pointer onto any edge or corner until a resize cursor appears, then press and
drag. The lower-right corner has a small visible resize mark. The minimum window width
is 420 pixels. The height is limited to 50 through 900 pixels.

The configured price and news row counts determine the normal height. A manual window
size or position is not part of the settings backup and can reset when row or line
visibility settings change.

### Pause and resume

Select the status button below the move grip, or right-click and select
**Pause / Resume**. Pausing stops automatic price and news refreshes and freezes the
marquee. Resume to restart normal activity.

### Open links

Double-click linked ticker text, including a news headline, to open its source in your
default browser. SmartTicker does not open links on a single click.

### Change highlights

After each refresh, SmartTicker briefly marks what moved on a brown background for three
seconds:

- A quote whose price differs from the previous sync.
- Each headline that was not present in the previous sync for that quote.

The first sync after startup highlights nothing because there is no earlier value to
compare against. A fired alert keeps its own alert blink colour and takes precedence.

### Main menu reference

| Command | Effect |
| --- | --- |
| **Refresh prices now** | Restart the staggered price cycle and request its first time slot when SmartTicker is not paused. |
| **Refresh news now** | Request all permitted entries that collect news immediately when SmartTicker is not paused. |
| **Pause / Resume** | Toggle refreshing and marquee movement. |
| **View > Left-to-right scroll: Prices only** | Use only the horizontal price marquee. This is the default. |
| **View > Left-to-right scroll: Prices with News** | Use both horizontal marquees. |
| **View > Static view: Prices only** | Use only responsive static quote tiles. |
| **View > Static view: Prices with News** | Use quote tiles plus the separate static News window. |
| **View > Open static news window** | Reopen the separate News window after closing it. Available in static mode when news is enabled. |
| **Language** | Choose one of the 16 supported menu/status languages. |

Line visibility, language, and the other configuration values are saved automatically.

## Quotes

Open **Quotes...** from the right-click menu. Each configured entry represents one
symbol and one webpage. Duplicate symbols are allowed and remain independent because
each entry has its own source, selectors, collection options, and alerts.

### Quick start with the published sample

When no entries exist, the Quotes window offers **Import sample quotes from GitHub**.
This downloads the repository sample and replaces the current application settings.
Review every imported URL and each website's current terms before using it. You can
edit or remove any sample entry afterward.

**Import Sample Quotes Config** at the top of both the Quotes and App Settings windows
does the same thing at any time, behind a confirmation:

- SmartTicker asks **Are you sure?** and warns that the download replaces your existing
	quotes, quote groups, source approvals, view, appearance, and other app settings.
	Alert rules live in their own file and are not deleted.
- **Export existing config...** is optional. It saves your current configuration to a
	local JSON file, then returns to the same confirmation.
- **Import Sample Quotes Config** downloads the sample from the internet and replaces
	your configuration.
- **Cancel** changes nothing.

### Add a quote or news entry

1. Enter the **Ticker** label, such as `MSFT`. SmartTicker trims it and stores it in
	 uppercase.
2. Optionally choose an existing **Group** from the lookup, or type a new name such as
	 `Nasdaq`, `Precious Metals`, or `Mag 7`. Leave it blank for **Ungrouped**.
3. Select a **Source** preset.
4. Enter the **URL suffix**, or a complete URL when using **Custom URL**.
5. Select **Price**, **News**, or both under **Collect**. At least one is required.
6. Enter selectors manually, use the discovery buttons, or leave optional selectors
	 blank to use built-in detection.
7. Select **Validate URL** to test the regular price and/or headlines.
8. If SmartTicker requests source approval, review the website and confirm only when
	 you are permitted to collect from it.
9. Select **Add independent entry**. SmartTicker saves the entry and refreshes its
	 enabled data immediately.

### Group quotes

A group is a named collection you define. It is not tied to an exchange or a built-in
category, so you can organize entries by market, asset type, strategy, portfolio,
region, or any other scheme. Names are trimmed, may use Unicode, and may contain up to
80 characters. Each quote can belong to at most one group.

Use **Manage groups** beside the Group field, or select **Quote groups...** from the
ticker's right-click menu. The window has three working areas:

- On the left, enter a **Group name**, then choose **Create**. Select an existing group,
	edit its name, and choose **Update**, or choose **Delete**. Empty groups are retained.
- On the right, select a quote. Its current group is shown in the **Current group**
	column; **Ungrouped** means it has no association.
- In the middle, choose **Associate** after selecting one group and one quote. If that
	quote already belongs to another group, SmartTicker moves it to the selected group.
- Choose **Remove association** to return only the selected quote to **Ungrouped**.
- Deleting a group returns all of its quotes to **Ungrouped**. Quotes, sources, current
	data, and alerts are not deleted.
- You can also choose an existing group from the lookup while adding or editing a quote,
	or type a new group name there.
- Use the up/down controls in Configured entries to determine group and row order in
	the static table.
- In static mode, drag a tile heading to reorder complete groups directly. The same
	order is used by the separate Quotes and News windows.

The published sample contains six example groups while leaving static mode off by
default. Enable the static view after importing it to see those groups as a table.

### Source presets and URLs

| Source | What to enter | Policy shown by SmartTicker |
| --- | --- | --- |
| **Yahoo Finance** | A suffix after `https://finance.yahoo.com/`, for example `quote/MSFT/`. | Written permission required. Yahoo's terms prohibit automated collection without prior permission. |
| **CNBC** | A suffix after `https://www.cnbc.com/`. | Check the site's current policy and robots directives. |
| **Trading Economics** | A suffix after `https://tradingeconomics.com/`. | Prefer a documented API or authorized feed and check the site's current policy. |
| **Custom URL** | A complete public `http://` or `https://` page URL. | Review the site's terms, privacy policy, and automated-access rules. |

Only absolute HTTP and HTTPS URLs are accepted. URLs containing embedded usernames or
passwords are rejected. A browser login does not authorize SmartTicker to collect a
page, and SmartTicker does not use authenticated browser sessions.

The **Full URL** line shows the final address produced from the preset prefix and your
suffix. Check it before validation or discovery.

### Collect options

- **Price** requests the regular price. Optional change, pre-market, and after-hours
	selectors are evaluated from the same downloaded page.
- **News** requests headline links from the page.
- Selecting both lets one entry contribute to both ticker areas.
- Clearing both is invalid.

### Selector field reference

A CSS selector identifies an element in a webpage's static HTML. Selectors are
optional unless automatic detection cannot find the value you need.

| Field | Value SmartTicker extracts |
| --- | --- |
| **Price selector** | Regular or closing price. |
| **Price change** | Regular-session percentage change. When blank, built-in change detection is attempted. |
| **Pre-market selector** | Pre-market price, when that session exists on the page. |
| **Pre-market change** | Pre-market percentage change. |
| **After-hours selector** | Post-market or after-hours price. |
| **After-hours change** | Post-market or after-hours percentage change. |
| **News selector** | Headline links. Select an anchor or a container whose results include links. |

Pre-market and after-hours values supplement the regular price; they do not replace
it. A page may omit those elements outside the corresponding market session.

Example Yahoo Finance selectors used by the published sample are:

```text
Price:                  [data-testid="qsp-price"]
Price change:           section.primary span[data-testid="qsp-price-change-percent"]
Pre-market price:       section.secondary span[data-testid="qsp-pre-price"]
Pre-market change:      section.secondary span[data-testid="qsp-pre-price-change-percent"]
After-hours price:      section.secondary span[data-testid="qsp-post-price"]
After-hours change:     section.secondary span[data-testid="qsp-post-price-change-percent"]
```

Website markup changes over time. Treat examples as starting points, not permanent
contracts.

### Discover selectors

Each selector field has a matching **Discover** button.

1. Complete the source URL and approve the website if approval is required.
2. Select the discovery button for the exact value type.
3. SmartTicker downloads public static HTML and lists possible selectors with a sample
	 value, confidence percentage, and reason in the tooltip.
4. Select **Use** beside a suggestion to copy it into the matching field.
5. Validate or observe the result before relying on it.

Discovery does not run JavaScript, sign in, bypass access controls, or inspect your
browser. A JavaScript-only value may have no discoverable selector. Separate discovery
types deliberately avoid mixing pre-market and after-hours values.

### Validate a source

**Validate URL** requests the page and reports the regular price and/or number of
headlines it can read. It is safe to use before entering a ticker because SmartTicker
uses a temporary label for the test.

This validation does not currently verify the four pre-market and after-hours selector
fields. Use their discovery sample values and then confirm the displayed session data.

Typical failures include an HTTP error, timeout, missing value, zero headlines, source
permission not approved, JavaScript-only content, or a stale selector.

### News repeat limit

**Show max _N_ times** accepts 1 through 100 and defaults to 5. SmartTicker counts one
showing for each news refresh in which the same headline title appears. Once the title
has appeared in the configured number of refreshes, it is retired for the rest of the
current application session. Editing or removing that entry clears its repeat history.

### Edit, order, and remove entries

The **Configured entries** list shows symbol, group, source, URL, collection badges,
regular price selector, news selector, and news repeat limit.

- **Edit** loads the entry into the form. Select **Save changes** to apply it or
	**Cancel edit** to discard form changes.
- The up and down arrow buttons change ticker order and save it immediately.
- **Remove** deletes the entry and its current displayed data.
- If alert rules target the entry, SmartTicker asks about deleting those rules. An
	alert with no matching configured quote cannot fire.
- Renaming an entry updates alert rule display symbols for rules attached to that entry.

## App Settings

Open **App Settings...** from the right-click menu. Changes take effect and save
automatically; there is no Apply button.

### Ticker rows and speed

| Setting | Choices | Default | Effect |
| --- | --- | --- | --- |
| Price rows | 1 through 8 | 1 | Number of parallel price marquee rows. |
| Price scroll speed | 20, 30, 40, 50, 65, 80, 100, or 120 px/sec | 50 | Price marquee speed. |
| News rows | 1 through 8 | 1 | Number of parallel headline marquee rows. |
| News scroll speed | 20, 30, 40, 50, 65, 80, 100, or 120 px/sec | 40 | News marquee speed. |
| Price refresh | 30 through 300 seconds, in 15-second steps | 60 seconds | Time in which every permitted price entry receives one scheduled refresh. |
| News refresh | 30 through 300 seconds, in 15-second steps | 300 seconds | Automatic headline refresh interval. |

Price rows and price scroll speed are disabled while static grouped tables are active
because that mode displays all price entries and never auto-scrolls either window.
News row and speed settings are retained for the scrolling view.

Price requests are distributed across one-second slots for the whole interval instead of
starting together. For example, 60 quotes over 30 seconds schedules two quotes per second;
five quotes over 30 seconds schedules one roughly every six seconds. If a source is slow,
SmartTicker waits rather than launching missed slots together. Existing prices remain on
screen until a complete replacement batch is ready.

The website's policy, robots directives, server throttling, and requested backoff take
priority over a configured interval. Avoid unnecessarily frequent requests.

Use the four choices under **View** to choose whether News is displayed and whether the
layout scrolls or remains static. Changing the view never deletes configured entries.

### Start SmartTicker when signing in

Enable **Start SmartTicker when I sign in** to register the installed executable for
the current user only.

- On Windows, SmartTicker uses the current user's `Run` registry key.
- On Linux desktops that support the freedesktop autostart convention, SmartTicker
	writes `smartticker.desktop` in the user's autostart directory.
- The option is disabled on platforms where SmartTicker has no supported registration
	mechanism.

The operating system is authoritative. If startup is changed outside SmartTicker, the
checkbox reflects the OS state the next time settings are loaded.

### Website access

**Allow website cookies and cross-host redirects** is disabled by default.

When disabled:

- SmartTicker requires one explicit approval for each website host before requesting
	it.
- Website cookies are not accepted.
- Redirects to a different host are blocked.
- Approved hosts are remembered in local settings.

When enabled:

- SmartTicker skips its per-host approval step.
- Cookies set by requested websites are held only in an isolated in-memory container
	and disappear when SmartTicker exits.
- Redirects to other hosts may be followed.
- SmartTicker still does not read browser cookies, submit credentials, or submit
	sign-in forms.

Changing this option off removes currently displayed data from unapproved sources
until those hosts are approved and refreshed.

#### Website privacy choices

If a response is recognized as a privacy/cookie form containing both positive and
negative choices, SmartTicker pauses and displays the page title, requested URL,
consent URL, form summary, and the website's Accept/Reject labels.

- **Accept** submits the hidden fields supplied by that form plus the exact Accept
	control you selected.
- **Reject** submits those hidden fields plus the exact Reject control you selected.
- **Cancel** submits nothing.

This is a website's privacy choice, not SmartTicker's per-source permission approval.

#### Validate all sources

Select **Validate all sources** to review and test every configured entry.

1. If website access is restricted, SmartTicker groups unapproved entries by hostname
	 and displays one source-review dialog per host.
2. Review the host, policy summary, guidance, source names, and symbols.
3. Check the confirmation only if you reviewed the website and are permitted to use it.
4. Choose **Approve this source**, **Skip this source**, or **Cancel validation**.
5. SmartTicker tests each permitted entry and reports passed, failed, and skipped
	 totals. Individual problems appear below the status line.

Approval records permission inside SmartTicker; it does not grant legal rights or
override the website's terms.

### Appearance

**Window transparency** changes only the ticker background. Text remains opaque. The
range is 20% through 100%, in 5% steps, and the default is 100%.

Color fields accept `#RRGGBB` hexadecimal values and also provide a color picker.

| Color | Default | Used for |
| --- | --- | --- |
| Background | `#10151D` | Ticker background before transparency is applied. |
| Quote name | `#79C0FF` | Symbol/source label. |
| Close price | `#FFA657` | Regular price. |
| After hours | `#00E5FF` | Pre-market and after-hours prices. |
| News 1st | `#FFFFFF` | Headlines 1, 5, 9, and so on. |
| News 2nd | `#00E5FF` | Headlines 2, 6, 10, and so on. |
| News 3rd | `#A3E635` | Headlines 3, 7, 11, and so on. |
| News 4th | `#79C0FF` | Headlines 4, 8, 12, and so on. |
| Change up | `#3FB950` | Positive percentage changes. |
| Change down | `#F85149` | Negative percentage changes. |
| Alert blink | `#FF00FF` | Triggered price alerts, alternating with black. |

**Reset to defaults** restores every color above and 100% background opacity. It does
not reset rows, speeds, sources, refresh intervals, alerts, or language.

### Backup and restore

SmartTicker keeps application settings and alert rules in separate JSON files and
provides separate buttons for each backup type.

#### Export and import settings

- **Export settings...** writes configured entries, group assignments, group definitions,
	hidden news quotes, entry order, selectors, the scrolling/static quote-view choice,
	approved hosts, line visibility, rows, speeds, refresh intervals, startup preference,
	website access option, colors including the alert blink colour, transparency, and
	language.
- **Import settings...** validates the entire file before changing anything. A rejected
	file leaves current settings unchanged.
- A successful import replaces every configured entry and application preference. It
	does not replace the separate alert-rules file.
- Groups are included as quote assignments in the settings file, alongside the group
	definitions themselves, so a group with no quotes also survives a backup. There is no
	separate group-only export or import file.
- The startup preference is present in a settings backup, but importing it does not
	silently change OS startup registration. The operating system remains authoritative;
	use the Startup checkbox to change registration on the current computer.
- Import files are limited to 1 MiB, schema version 1, and at most 200 subscriptions.
	Unknown properties, duplicate IDs, malformed URLs, invalid colors, invalid ranges,
	or unsupported language codes are rejected rather than silently ignored.

#### Export and import alert rules- **Export alert rules...** writes all rules plus Buzz, buzz count, and blink duration.
- **Import alert rules...** validates the whole file, then replaces all current rules
	and alert-fire settings.
- Rules first reconnect by subscription ID. When IDs differ, SmartTicker tries a
	case-insensitive symbol match.
- An imported rule with no matching quote is retained but cannot fire. The import
	status reports how many rules were re-linked or remain unmatched.
- Alert import files are limited to 1 MiB.

For a transfer to another computer, import application settings first and alert rules
second. Importing alerts second allows rules to reconnect to the new subscription IDs
by symbol.

### Edit the configuration files in place

**Edit Current App Config** and **Edit Current Alert Rules** in App Settings open the
live JSON file in whatever text editor your system associates with `.json`. This is for
advanced users; the windows in SmartTicker cover the same settings without risk.

Both buttons first show a confirmation that asks you to export the current file. Take
that export: hand-editing can break the file, and there is no undo.

- **Export existing config...** saves the current file, then returns to the same prompt.
- **Open in text editor** opens the live file.
- **Cancel** changes nothing.

SmartTicker watches the file and reloads it as soon as your editor saves:

- A valid file is applied immediately, and the ticker updates without a restart.
- Malformed JSON, a schema violation, or any other validation error is rejected. Your
	running configuration is left untouched and the App Settings window reports the
	problem.
- After a rejected edit, correct the file, or restore a valid export with
	**Import settings...** or **Import alert rules...**.
- A file that stays locked by another program is retried briefly and then reported.

Editing the alert-rules file follows the same rules and does not affect application
settings, because the two files are separate.

## Alert rules

Open **Alerts** from the right-click menu. Rules are evaluated after each successful
price refresh and watch the regular price only, not pre-market or after-hours values.

### Create a rule

1. Select a configured **Quote**. Entries with the same symbol remain distinct.
2. Select a **Condition** and enter a numeric threshold using an invariant decimal such
	 as `250.50`.
3. Optionally choose **Active from**. Leave it empty to activate immediately.
4. Leave **Never expires** checked, or clear it and choose an expiry date.
5. Select **Add rule**.

The available comparisons are:

| Choice | Meaning |
| --- | --- |
| `LessThan` | Price `<` threshold. |
| `LessThanOrEqual` | Price `<=` threshold. |
| `GreaterThan` | Price `>` threshold. |
| `GreaterThanOrEqual` | Price `>=` threshold. |
| `EqualTo` | Price equals the threshold exactly. |
| `NotEqualTo` | Price differs from the threshold. |

The start boundary is inclusive. The expiry boundary is also inclusive; after it has
passed, the rule no longer fires. SmartTicker rejects an expiry earlier than the start.

### When a rule fires

An enabled, scheduled rule fires once when its condition changes from false to true.
It does not notify on every refresh while the condition remains true. After the price
leaves the condition, the rule re-arms and can fire when the price enters it again.

Editing a rule or disabling and re-enabling it also re-arms it. Therefore, an enabled
rule can fire immediately if the most recent regular price already satisfies its
condition. A failed or missing price cannot trigger a rule.

When one or more rules fire:

- The affected price entry alternates the configured alert blink color and black for
	the configured duration. The default blink color is magenta (`#FF00FF`).
- If **Buzz** is enabled, SmartTicker plays the configured buzz sequence.
- The alert message identifies one rule or reports the number of rules fired together.
- Ticker scrolling continues while the alert highlight is active.

### Alert output settings

| Setting | Range | Default |
| --- | --- | --- |
| **Buzz** | On or off | On |
| Buzz count | 1 through 20 | 15 |
| **Blink for** | 5 through 900 seconds, in 15-second steps | 60 seconds |

Disabling Buzz leaves the visual alert active. If several rules fire in the same
evaluation, SmartTicker starts one configured buzz sequence for that evaluation.
Change **Alert blink** under **App Settings > Appearance**. It is an application
appearance preference, so Settings export/import includes it rather than the separate
alert-rules file.

### Manage configured rules

- **Edit** loads a rule into the form. Select **Update rule** to save or **Cancel** to
	leave it unchanged.
- **Disable** keeps the rule but stops it from matching. **Enable** re-arms it and
	evaluates it against the latest regular price.
- **Remove** deletes the rule.
- The list shows enabled state, symbol, condition summary, and schedule.

Alert rule changes and alert output settings save automatically.

## Local files and privacy

SmartTicker stores configuration locally and does not synchronize it to a developer
service.

On Windows, the default files are:

```text
%LocalAppData%\SmartTicker\settings.json
%LocalAppData%\SmartTicker\alerts.json
```

On Linux, .NET uses the current user's local application-data directory, normally:

```text
~/.local/share/SmartTicker/settings.json
~/.local/share/SmartTicker/alerts.json
```

The Alerts window displays the exact alert file path in use. Writes use a temporary
file followed by replacement so a partially written file is not treated as current
configuration.

SmartTicker has no account, telemetry, analytics, advertising, or cloud sync. A source
website receives normal network information such as your IP address when SmartTicker
requests that source. Opening Help requests the raw guide from GitHub. For complete
details, read `PRIVACY.md` in the repository.

You are responsible for ensuring that each source URL and selector is used in
accordance with the website's terms, license, robots directives, and applicable law.

## Troubleshooting

### A quote shows unavailable or no price

1. Open **Quotes...**, edit the entry, and check the Full URL.
2. Confirm **Price** is selected.
3. Approve the website if prompted.
4. Select **Validate URL** and read its exact result.
5. Run **Discover price**, or inspect the page's static HTML and update the selector.
6. Check whether the page requires JavaScript, authentication, or consent that
	 SmartTicker cannot safely handle.
7. Respect HTTP 403, 429, robots restrictions, and the site's automated-access policy.

### Pre-market or after-hours data is missing

- The corresponding market session may not be active.
- The page may omit the session element when no session value exists.
- Verify that pre-market selectors target pre-market elements and after-hours selectors
	target post-market elements.
- Run the matching discovery command again because website markup may have changed.

### News is empty

- Confirm **News** is selected.
- Validate the source and run **Discover news**.
- Ensure the selector returns links with visible headline text.
- A headline disappears after reaching its configured repeat limit for this session.
- In static News, confirm the intended quote is checked under **Show news for**.

### Selector discovery finds nothing

Discovery reads only the downloaded static HTML. It cannot see values created later by
page JavaScript. Enter a verified selector manually, choose a static page/feed, or use
an authorized documented API through a compatible public page.

### An alert does not fire

- Confirm the attached quote still exists, collects Price, and has a successful regular
	price.
- Confirm the rule is Enabled and within its start/expiry schedule.
- Check the comparison and threshold. `EqualTo` requires exact decimal equality.
- Remember that a continuously true condition fires once; it must become false before
	it can fire again, unless you edit or re-enable the rule.
- Pre-market and after-hours prices do not drive alert rules.

### SmartTicker cannot move or resize

- Move only from the vertical-dot grip in the left strip.
- Resize from an edge or corner; use the visible lower-right mark if an edge is hard to
	locate.
- Ticker content is intentionally not a move surface.

### Static groups or values are not what I expect

- Open **Quotes...** and confirm each entry's Group value.
- Open **Quote groups...** to manage group definitions and review every quote's current
	association.
- Entries with a blank Group appear under **Ungrouped**.
- **Chg** is calculated from Last and Chg%; it is not independently extracted from the
	page. It stays `—` when the percentage is unavailable.
- Reorder entries with the up/down controls to change group and row order.
- Drag the dotted handle on a tile heading to move the whole group. Drop on the left
  half of another tile to place it before, or the right half to place it after.
- Select **Refresh prices now** while SmartTicker is not paused to update the table.

### Help text is not formatted or navigation does not move

- The Help window should show formatted headings, paragraphs, lists, tables, links,
	and code blocks rather than Markdown punctuation.
- Use **On this page** on the left to jump to a major section. Links in the Quick
	navigation table also scroll within the document.
- Select **Reload** to fetch the current published guide. If that fails, SmartTicker
	formats the guide embedded in the installed application.

### Online Help is unavailable or out of date

- Close and reopen Help to request the published guide again.
- Open the raw GitHub address shown near the beginning of this guide in a browser to
	inspect the published file directly.
- SmartTicker uses the embedded guide when the request fails or returns an empty file.
- Online changes appear only after the updated `HELPME.md` is published on the
	repository's `main` branch.

## Support

Report reproducible problems at:

<https://github.com/bulentozkir/smartticker/issues>

Include the SmartTicker version, operating system, source hostname, validation status,
and exact error text. Remove private URLs or other sensitive information before posting.
