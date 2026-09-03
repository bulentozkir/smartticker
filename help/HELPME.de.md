# SmartTicker-Hilfe

Dieser Leitfaden gilt für SmartTicker 1.0.3. Er erläutert den Hauptticker, die
App-Einstellungen, Kurse, Warnregeln, Websiteberechtigungen, Sicherungen und häufige
Probleme.

SmartTicker liest öffentliches statisches HTML von Webseiten, die Sie konfigurieren.
Die Anwendung stellt keinen Marktdatenfeed bereit; extrahierte Informationen können
verzögert, unvollständig oder falsch sein. Prüfen Sie wichtige Finanzinformationen
anhand einer maßgeblichen Quelle.

## Schnellnavigation

| Bereich | Direkt zu |
| --- | --- |
| Erste Schritte | [Hilfe- und Konfigurationsfenster öffnen](#hilfe-und-konfigurationsfenster-öffnen) |
| Hauptticker | [Steuerelemente](#hauptticker-steuerelemente) · [Laufende oder statische Ansicht](#laufende-oder-statische-kursansicht-auswählen) · [Verschieben](#ticker-verschieben) · [Größe ändern](#tickergröße-ändern) · [Anhalten](#anhalten-und-fortsetzen) · [Menüreferenz](#hauptmenüreferenz) |
| Kurse und Nachrichten | [Kurse](#kurse) · [Eintrag hinzufügen](#kurs--oder-nachrichteneintrag-hinzufügen) · [Kurse gruppieren](#kurse-gruppieren) · [Quell-URLs](#quellvorgaben-und-urls) · [Selektoren](#referenz-der-selektorfelder) · [Ermittlung](#selektoren-ermitteln) · [Validierung](#quelle-validieren) |
| Anwendungseinstellungen | [App-Einstellungen](#app-einstellungen) · [Zeilen und Geschwindigkeit](#tickerzeilen-und-geschwindigkeit) · [Autostart](#smartticker-bei-der-anmeldung-starten) · [Websitezugriff](#websitezugriff) · [Darstellung](#darstellung) · [Sichern und Wiederherstellen](#sichern-und-wiederherstellen) · [Konfigurationsdateien bearbeiten](#konfigurationsdateien-direkt-bearbeiten) |
| Kurswarnungen | [Warnregeln](#warnregeln) · [Regel erstellen](#regel-erstellen) · [Auslöseverhalten](#wenn-eine-regel-auslöst) · [Warnausgabe](#einstellungen-für-warnausgaben) · [Regeln verwalten](#konfigurierte-regeln-verwalten) |
| Daten und Support | [Lokale Dateien und Datenschutz](#lokale-dateien-und-datenschutz) · [Problembehandlung](#problembehandlung) · [Unterstützung](#unterstützung) |

## Hilfe und Konfigurationsfenster öffnen

Klicken Sie mit der rechten Maustaste auf den Ticker, um sein Menü zu öffnen. Die
wichtigsten Konfigurationsbefehle sind:

- **Kurse...** (**Quotes...**): Kurs- oder Nachrichtenquellen hinzufügen, testen,
	bearbeiten, anordnen und entfernen.
- **Kursgruppen...** (**Quote groups...**): Gruppen erstellen, aktualisieren oder
	löschen und ihnen Kurse zuordnen.
- **Warnungen** (**Alerts**): Kurswarnregeln erstellen und verwalten.
- **App-Einstellungen...** (**App Settings...**): Zeilen, Geschwindigkeiten,
	Aktualisierungsintervalle, Autostart, Websitezugriff, Farben, Transparenz und
	Sicherungen konfigurieren.
- **Ansicht** (**View**): eine von vier sich gegenseitig ausschließenden Kombinationen
	auswählen: laufend oder statisch, nur mit Kursen oder mit Kursen und Nachrichten.
- **Hilfe** (**Help**): diesen Leitfaden in SmartTicker öffnen.
- **Über SmartTicker** (**About SmartTicker**): installierte Version und Lizenzhinweis
	anzeigen.
- **Beenden** (**Exit**): SmartTicker vollständig schließen.

Das Hilfefenster formatiert sofort den integrierten Leitfaden für die ausgewählte
Anwendungssprache und zeigt ihn an. Anschließend prüft es beim Öffnen der Hilfe oder
beim Ändern der **Sprache** (**Language**) den passenden Onlineleitfaden. Der deutsche
Onlineleitfaden ist:

<https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/help/HELPME.de.md>

Schlägt die Onlineanfrage fehl, zeigt SmartTicker weiterhin die passende, in der
installierten Anwendung eingebettete Übersetzung an. Beim Ändern der **Sprache**
(**Language**) werden Titel, Status, Navigation und der vollständige Leitfaden eines
geöffneten Hilfefensters sofort aktualisiert. Schließen Sie die Hilfe über die normale
Schließen-Schaltfläche der Titelleiste.

## Hauptticker-Steuerelemente

### Laufende oder statische Kursansicht auswählen

SmartTicker bietet vier sich gegenseitig ausschließende Anzeigemodi. Klicken Sie mit
der rechten Maustaste auf den Ticker, öffnen Sie **Ansicht** (**View**) und wählen Sie
einen Modus aus. Das Layout ändert sich sofort und Ihre Auswahl wird gespeichert.

| Ansichtsoption | Ergebnis |
| --- | --- |
| **Von links nach rechts laufend: nur Kurse** (**Left-to-right scroll: Prices only**) | Kurslaufband im Hauptticker; keine Nachrichtenanzeige. Dies ist die Standardeinstellung. |
| **Von links nach rechts laufend: Kurse mit Nachrichten** (**Left-to-right scroll: Prices with News**) | Kurs- und Nachrichtenlaufband im Hauptticker. |
| **Statische Ansicht: nur Kurse** (**Static view: Prices only**) | Responsive Kurskacheln im Hauptfenster; kein Nachrichtenfenster. |
| **Statische Ansicht: Kurse mit Nachrichten** (**Static view: Prices with News**) | Responsive Kurskacheln sowie ein separates statisches Fenster **SmartTicker News**. |

Einstellungsdateien, die vor Einführung dieser Auswahlmöglichkeiten erstellt wurden,
werden entsprechend ihren gespeicherten Einstellungen für laufende/statische Anzeige
und Nachrichten der passenden Kombination zugeordnet. Der Anzeigemodus wird
ausschließlich über das Kontextmenü **Ansicht** (**View**) des Tickers verwaltet.

- In beiden laufenden Modi verwenden Kurse das horizontale Laufband sowie die
	konfigurierte Anzahl der Kurszeilen und die Laufgeschwindigkeit.
- In beiden statischen Modi erscheinen Gruppen als responsive Kacheln von links nach
	rechts. Kacheln werden nur dann in eine weitere Zeile umgebrochen, wenn das Fenster zu
	schmal ist. Kurse bewegen sich nicht automatisch.
- Jede Kurskachel besitzt eigene ausgerichtete Spalten **Symbol**, **Letzter Kurs**
	(**Last**), **Änd.** (**Chg**) und **Änd.%** (**Chg%**). **Änd.** wird aus dem letzten
	Kurs und Änd.% abgeleitet, da Quellseiten einen Prozentselektor statt eines separaten
	Selektors für die absolute Änderung bereitstellen. Wenn einer der Werte nicht
	verfügbar ist, wird `—` angezeigt.
- Wählen Sie eine Gruppenüberschrift aus, um sie ein- oder auszuklappen. Die Reihenfolge
	der Gruppen richtet sich nach dem ersten Auftreten ihrer Kurse in der Reihenfolge der
	konfigurierten Einträge; die Zeilen innerhalb einer Gruppe behalten diese Reihenfolge.
- Einträge ohne Gruppe erscheinen unter **Nicht gruppiert** (**Ungrouped**).
- Zeigen Sie auf den letzten Kurs, um verfügbare vorbörsliche und nachbörsliche Werte
	anzusehen. Doppelklicken Sie auf eine Kurszeile, um deren Quellseite zu öffnen.
- Das Blinken von Warnungen sowie die Farben für steigende/fallende Werte funktionieren
	in beiden Kursmodi.
- Nachrichten werden automatisch in einem separaten Fenster **SmartTicker News** mit
	statischen Gruppenkacheln **Symbol / Schlagzeile** (**Symbol / Headline**) geöffnet.
	Im statischen Modus gibt es kein Laufband. Das Nachrichtenfenster besitzt eine normale
	Titelleiste und einen Größenänderungsrahmen, sodass Kurs- und Nachrichtenfenster
	unabhängig auf verschiedene Monitore verschoben werden können. Doppelklicken Sie auf
	eine Schlagzeile, um deren Quelle zu öffnen.
- Beim ersten Start verwendet das Nachrichtenfenster die kompakte Größe 680×340.
	SmartTicker platziert es nach Möglichkeit auf einem anderen Monitor. Bei nur einem
	Monitor versucht die Anwendung zunächst einen freien Bereich unterhalb, rechts,
	oberhalb oder links vom Kursfenster. Danach können Sie es normal verschieben und seine
	Größe ändern.
- Innerhalb jeder Nachrichtengruppe werden Schlagzeilen nach Kurs verschachtelt: zuerst
	eine Schlagzeile des ersten Kurses, dann eine des nächsten Kurses und so fort in
	Runden. Ein Kurs mit vielen Schlagzeilen kann daher nicht den gesamten oberen Bereich
	seiner Gruppe belegen.
- Öffnen Sie die einzeilige Auswahlliste **Nachrichten anzeigen für** (**Show news for**)
	und aktivieren oder deaktivieren Sie jeden Kurs einzeln. Jede Kombination von Kursen
	kann sichtbar sein, einschließlich aller oder keiner. Die Schaltfläche fasst die
	aktuelle Auswahl zusammen; die Einträge enthalten Kurs und Quelle, damit doppelte
	Symbole unabhängig bleiben. Deaktivierte Kurse werden in Ihrer Einstellungsdatei als
	`hiddenNewsQuotes` gespeichert und bleiben daher nach einem Neustart erhalten und
	werden mit einer Einstellungssicherung übertragen.
- Ziehen Sie den gepunkteten Griff neben einer Kurs- oder Nachrichtenkachelüberschrift
	und legen Sie ihn auf der linken oder rechten Hälfte einer anderen Kachel ab. Die
	Reihenfolge ändert sich in beiden Fenstern und wird durch Neuanordnung der zugrunde
	liegenden konfigurierten Einträge gespeichert.
- Eine Gruppe mit vielen Zeilen kann innerhalb ihrer begrenzten Kachel gescrollt werden.
	Die Gesamtansicht wird nur dann vertikal gescrollt, wenn umgebrochene Kachelzeilen
	nicht in die aktuelle Fensterhöhe passen.

Durch Schließen von **SmartTicker News** wird die Nachrichtenerfassung nicht
deaktiviert. Um das Fenster erneut zu öffnen, klicken Sie mit der rechten Maustaste auf
das Kursfenster und wählen **Ansicht > Statisches Nachrichtenfenster öffnen** (**View >
Open static news window**). Mit **Statische Ansicht: nur Kurse** wird es geschlossen;
mit **Statische Ansicht: Kurse mit Nachrichten** wird es wieder geöffnet. Beide
laufenden Optionen schließen das separate Nachrichtenfenster; die laufende Option mit
Kursen und Nachrichten stellt das Nachrichtenlaufband im Hauptticker wieder her.

Beim Wechseln des Modus wird die für diese Ansicht gespeicherte Größe angewendet. Der
laufende Ticker, das statische Kursfenster und das statische Nachrichtenfenster behalten
jeweils eine unabhängige Breite und Höhe.

### Ticker verschieben

Halten Sie den Griff mit vertikalen Punkten oben im schmalen linken Streifen gedrückt,
ziehen Sie den Ticker und lassen Sie die Maustaste los. Tickertext ist keine
Ziehfläche. Durch Auswählen oder Anklicken von Inhalten kann daher nicht versehentlich
das Fenster verschoben werden.

### Tickergröße ändern

Bewegen Sie den Zeiger auf eine Kante oder Ecke, bis ein Größenänderungszeiger
erscheint, halten Sie dann die Maustaste gedrückt und ziehen Sie. Die untere rechte Ecke
besitzt eine kleine sichtbare Größenmarkierung. Die minimale Fensterbreite beträgt 420
Pixel. Die Höhe der laufenden Ansicht reicht von 50 bis 900 Pixel, die Höhe des
statischen Kursfensters von 420 bis 4320 Pixel und die Höhe des statischen
Nachrichtenfensters von 240 bis 4320 Pixel.

Eine manuelle Größenänderung aktualisiert die gespeicherten Abmessungen der aktiven
Ansicht, sobald der Ziehvorgang beendet ist. Alle drei Größenpaare sind in einer
Einstellungssicherung enthalten. Fensterpositionen werden nicht gespeichert. Ist eine
laufende Ansicht für die ausgewählten Kurs-/Nachrichtenzeilen und die Schriftgröße der
laufenden Ansicht zu niedrig, vergrößert SmartTicker die gespeicherte Höhe automatisch.
Mit **Von links nach rechts laufend: Kurse mit Nachrichten** wird daher stets Platz für
die Nachrichtenzeilen geschaffen, statt sie unbemerkt auszublenden.
Wenn ein Fenster geöffnet oder verschoben wird, hält SmartTicker mindestens seine 32
Pixel große obere linke Ecke innerhalb eines Bildschirmarbeitsbereichs und begrenzt die
globalen X- und Y-Werte auf mindestens 1. Dadurch bleibt der Verschiebegriff oder die
Titelecke selbst nach dem Trennen eines Monitors mit der Maus erreichbar.

### Anhalten und fortsetzen

Wählen Sie die Statusschaltfläche unter dem Verschiebegriff aus oder klicken Sie mit
der rechten Maustaste und wählen Sie **Anhalten / Fortsetzen** (**Pause / Resume**).
Beim Anhalten werden automatische Kurs- und Nachrichtenaktualisierungen gestoppt und
das Laufband eingefroren. Außerdem können beide manuellen Aktualisierungsbefehle keine
neue Arbeit starten. Eine bereits laufende Quellanfrage wird nicht allein aufgrund des
Anhaltens zwangsweise abgebrochen und kann beendet werden, bevor sämtliche Aktivität
zur Ruhe kommt. Beim Fortsetzen werden die automatischen Zeitgeber neu gestartet.

Unter Windows setzt SmartTicker seine Betriebssystem-Prozesspriorität vor dem Start der
Benutzeroberfläche automatisch auf **Niedrig** (**Low**) und aktiviert den Windows-
**Effizienzmodus** (**Efficiency mode**, EcoQoS). Für dieses Verhalten gibt es keine
App-Einstellung. Die Anwendung verwendet außerdem einen ressourcenschonenden
Software-Renderingpfad. Die Laufbandzeitsteuerung passt sich an die konfigurierte
Geschwindigkeit an; bei angehaltenem, leerem oder getrenntem Laufband stoppt der
Animationszeitgeber. Unveränderte Zeilen unterdrücken redundante visuelle
Benachrichtigungen. Das Blinken von Warnungen und die drei Sekunden lange braune
Änderungshervorhebung sind beabsichtigt und halten das Laufband nicht an. Unter Linux
bleibt die Prozessplanung dem Betriebssystem überlassen. Lehnt Windows eine der beiden
Prozesseinstellungen ab, meldet SmartTicker den Fehler in der Diagnoseablaufverfolgung
und setzt den Start fort.

### Links öffnen

Doppelklicken Sie auf verknüpften Tickertext, einschließlich einer Schlagzeile, um die
Quelle in Ihrem Standardbrowser zu öffnen. SmartTicker öffnet Links nicht mit einem
einfachen Klick.

### Hervorhebungen von Änderungen

Nach jeder Aktualisierung markiert SmartTicker drei Sekunden lang auf braunem
Hintergrund, was sich geändert hat:

- Einen Kurs, dessen Preis vom vorherigen Abgleich abweicht.
- Jede Schlagzeile, die beim vorherigen Abgleich für diesen Kurs nicht vorhanden war.

Beim ersten Abgleich nach dem Start wird nichts hervorgehoben, da kein früherer Wert
zum Vergleich vorliegt. Eine ausgelöste Warnung behält ihre eigene Warnblinkfarbe und
hat Vorrang.

### Hauptmenüreferenz

| Befehl | Wirkung |
| --- | --- |
| **Kurse jetzt aktualisieren** (**Refresh prices now**) | Gestaffelten Kurszyklus neu starten und dessen erstes Zeitfenster anfordern, sofern SmartTicker nicht angehalten ist. |
| **Nachrichten jetzt aktualisieren** (**Refresh news now**) | Gestaffelten Nachrichtenzyklus neu starten und dessen erstes Zeitfenster anfordern, sofern SmartTicker nicht angehalten ist. |
| **Anhalten / Fortsetzen** (**Pause / Resume**) | Aktualisierung und Laufbandbewegung umschalten. |
| **Ansicht > Von links nach rechts laufend: nur Kurse** | Nur das horizontale Kurslaufband verwenden. Dies ist die Standardeinstellung. |
| **Ansicht > Von links nach rechts laufend: Kurse mit Nachrichten** | Beide horizontalen Laufbänder verwenden. |
| **Ansicht > Statische Ansicht: nur Kurse** | Nur responsive statische Kurskacheln verwenden. |
| **Ansicht > Statische Ansicht: Kurse mit Nachrichten** | Kurskacheln sowie das separate statische Nachrichtenfenster verwenden. |
| **Ansicht > Statisches Nachrichtenfenster öffnen** | Das separate Nachrichtenfenster nach dem Schließen wieder öffnen. Verfügbar im statischen Modus, wenn Nachrichten aktiviert sind. |
| **Sprache** (**Language**) | Eine von 16 Sprachen für Menüs, Statustext und den vollständigen Hilfeleitfaden auswählen. Ein geöffnetes Hilfefenster wird sofort aktualisiert. |

Zeilensichtbarkeit, Sprache und die übrigen Konfigurationswerte werden automatisch
gespeichert.

## Kurse

Öffnen Sie **Kurse...** (**Quotes...**) über das Kontextmenü. Jeder konfigurierte
Eintrag steht für ein Symbol und eine Webseite. Doppelte Symbole sind zulässig und
bleiben unabhängig, da jeder Eintrag eigene Quelle, Selektoren, Erfassungsoptionen und
Warnungen besitzt.

### Schnellstart mit veröffentlichtem Beispiel

Sind keine Einträge vorhanden, bietet das Kursfenster **Beispielkurse von GitHub
importieren** (**Import sample quotes from GitHub**) an. Dadurch wird das
Repositorybeispiel heruntergeladen und die aktuelle Anwendungskonfiguration ersetzt.
Prüfen Sie jede importierte URL und die aktuellen Bedingungen der jeweiligen Website,
bevor Sie sie verwenden. Anschließend können Sie jeden Beispieleintrag bearbeiten oder
entfernen.

**Beispiel-Kurskonfiguration importieren** (**Import Sample Quotes Config**) oben im
Kursfenster und in den App-Einstellungen führt jederzeit denselben Vorgang nach einer
Bestätigung aus:

- SmartTicker fragt **Sind Sie sicher?** (**Are you sure?**) und warnt, dass der
	Download Ihre vorhandenen Kurse, Kursgruppen, Quellgenehmigungen, Ansicht,
	Darstellung und sonstigen App-Einstellungen ersetzt. Warnregeln befinden sich in
	einer eigenen Datei und werden nicht gelöscht.
- **Vorhandene Konfiguration exportieren...** (**Export existing config...**) ist
	optional. Damit wird Ihre aktuelle Konfiguration in einer lokalen JSON-Datei
	gespeichert und anschließend dieselbe Bestätigung erneut angezeigt.
- **Beispiel-Kurskonfiguration importieren** lädt das Beispiel aus dem Internet
	herunter und ersetzt Ihre Konfiguration.
- **Abbrechen** (**Cancel**) ändert nichts.

### Kurs- oder Nachrichteneintrag hinzufügen

1. Geben Sie die Bezeichnung **Ticker** ein, beispielsweise `MSFT`. SmartTicker
	 entfernt Leerzeichen am Anfang und Ende und speichert sie in Großbuchstaben.
2. Wählen Sie optional eine vorhandene **Gruppe** (**Group**) aus der Auswahlliste oder
	 geben Sie einen neuen Namen wie `Nasdaq`, `Precious Metals` oder `Mag 7` ein. Lassen
	 Sie das Feld für **Nicht gruppiert** (**Ungrouped**) leer.
3. Wählen Sie eine **Quelle** (**Source**) aus den Vorgaben.
4. Geben Sie das **URL-Suffix** (**URL suffix**) oder bei **Benutzerdefinierte URL**
	 (**Custom URL**) eine vollständige URL ein.
5. Wählen Sie unter **Erfassen** (**Collect**) **Kurs** (**Price**), **Nachrichten**
	 (**News**) oder beides aus. Mindestens eine Option ist erforderlich.
6. Geben Sie Selektoren manuell ein, verwenden Sie die Ermittlungsschaltflächen oder
	 lassen Sie optionale Selektoren leer, um die integrierte Erkennung zu verwenden.
7. Wählen Sie **URL validieren** (**Validate URL**), um den regulären Kurs und/oder die
	 Schlagzeilen zu testen.
8. Wenn SmartTicker eine Quellgenehmigung anfordert, prüfen Sie die Website und
	 bestätigen Sie nur, wenn Sie Daten von ihr erfassen dürfen.
9. Wählen Sie **Unabhängigen Eintrag hinzufügen** (**Add independent entry**).
	 SmartTicker speichert den Eintrag und aktualisiert seine aktivierten Daten sofort.

### Kurse gruppieren

Eine Gruppe ist eine von Ihnen definierte benannte Sammlung. Sie ist weder an eine
Börse noch an eine integrierte Kategorie gebunden. Sie können Einträge daher nach
Markt, Anlageart, Strategie, Portfolio, Region oder einem anderen Schema organisieren.
Namen werden am Anfang und Ende bereinigt, dürfen Unicode verwenden und höchstens 80
Zeichen enthalten. Jeder Kurs kann höchstens einer Gruppe angehören.

Verwenden Sie **Gruppen verwalten** (**Manage groups**) neben dem Gruppenfeld oder
wählen Sie **Kursgruppen...** (**Quote groups...**) im Kontextmenü des Tickers. Das
Fenster besitzt drei Arbeitsbereiche:

- Geben Sie links einen **Gruppennamen** (**Group name**) ein und wählen Sie
	**Erstellen** (**Create**). Wählen Sie eine vorhandene Gruppe aus, bearbeiten Sie
	ihren Namen und wählen Sie **Aktualisieren** (**Update**) oder **Löschen** (**Delete**).
	Leere Gruppen bleiben erhalten.
- Wählen Sie rechts einen Kurs aus. Seine aktuelle Gruppe wird in der Spalte
	**Aktuelle Gruppe** (**Current group**) angezeigt; **Nicht gruppiert** bedeutet, dass
	keine Zuordnung besteht.
- Wählen Sie in der Mitte **Zuordnen** (**Associate**), nachdem Sie eine Gruppe und
	einen Kurs ausgewählt haben. Gehört der Kurs bereits einer anderen Gruppe an,
	verschiebt SmartTicker ihn in die ausgewählte Gruppe.
- Wählen Sie **Zuordnung entfernen** (**Remove association**), um nur den ausgewählten
	Kurs wieder unter **Nicht gruppiert** einzuordnen.
- Beim Löschen einer Gruppe werden alle ihre Kurse wieder unter **Nicht gruppiert**
	eingeordnet. Kurse, Quellen, aktuelle Daten und Warnungen werden nicht gelöscht.
- Beim Hinzufügen oder Bearbeiten eines Kurses können Sie auch eine vorhandene Gruppe
	aus der Auswahlliste wählen oder dort einen neuen Gruppennamen eingeben.
- Verwenden Sie die Aufwärts-/Abwärts-Steuerelemente unter „Konfigurierte Einträge“, um
	die Gruppen- und Zeilenreihenfolge in der statischen Tabelle festzulegen.
- Ziehen Sie im statischen Modus eine Kachelüberschrift, um vollständige Gruppen direkt
	neu anzuordnen. Das separate Kurs- und Nachrichtenfenster verwendet dieselbe
	Reihenfolge.

Das veröffentlichte Beispiel enthält sechs Beispielgruppen, lässt den statischen Modus
aber standardmäßig deaktiviert. Aktivieren Sie nach dem Import die statische Ansicht,
um diese Gruppen als Tabelle anzuzeigen.

### Quellvorgaben und URLs

| Quelle | Eingabe | Von SmartTicker angezeigte Richtlinie |
| --- | --- | --- |
| **Yahoo Finance** | Ein Suffix nach `https://finance.yahoo.com/`, zum Beispiel `quote/MSFT/`. | Schriftliche Genehmigung erforderlich. Die Bedingungen von Yahoo untersagen automatisierte Erfassung ohne vorherige Genehmigung. |
| **CNBC** | Ein Suffix nach `https://www.cnbc.com/`. | Aktuelle Richtlinie und robots-Anweisungen der Website prüfen. |
| **Trading Economics** | Ein Suffix nach `https://tradingeconomics.com/`. | Eine dokumentierte API oder einen autorisierten Feed bevorzugen und die aktuelle Richtlinie der Website prüfen. |
| **Benutzerdefinierte URL** (**Custom URL**) | Eine vollständige öffentliche Seiten-URL mit `http://` oder `https://`. | Bedingungen, Datenschutzrichtlinie und Regeln für automatisierten Zugriff der Website prüfen. |

Es werden ausschließlich absolute HTTP- und HTTPS-URLs akzeptiert. URLs mit
eingebetteten Benutzernamen oder Kennwörtern werden abgelehnt. Eine Browseranmeldung
berechtigt SmartTicker nicht zur Erfassung einer Seite, und SmartTicker verwendet keine
authentifizierten Browsersitzungen.

Die Zeile **Vollständige URL** (**Full URL**) zeigt die endgültige Adresse, die sich aus
dem Präfix der Vorgabe und Ihrem Suffix ergibt. Prüfen Sie sie vor der Validierung oder
Ermittlung.

### Erfassungsoptionen

- **Kurs** (**Price**) fordert den regulären Kurs an. Optionale Selektoren für Änderung,
	vorbörsliche und nachbörsliche Werte werden auf derselben heruntergeladenen Seite
	ausgewertet.
- **Nachrichten** (**News**) fordert Schlagzeilenlinks von der Seite an.
- Wenn Sie beides auswählen, kann ein Eintrag zu beiden Tickerbereichen beitragen.
- Das Deaktivieren beider Optionen ist ungültig.

### Referenz der Selektorfelder

Ein CSS-Selektor bezeichnet ein Element im statischen HTML einer Webseite. Selektoren
sind optional, sofern die automatische Erkennung den gewünschten Wert findet.

| Feld | Von SmartTicker extrahierter Wert |
| --- | --- |
| **Kursselektor** (**Price selector**) | Regulärer oder Schlusskurs. |
| **Kursänderung** (**Price change**) | Prozentuale Änderung der regulären Sitzung. Ist das Feld leer, wird die integrierte Änderungserkennung versucht. |
| **Vorbörslicher Selektor** (**Pre-market selector**) | Vorbörslicher Kurs, wenn diese Sitzung auf der Seite vorhanden ist. |
| **Vorbörsliche Änderung** (**Pre-market change**) | Prozentuale vorbörsliche Änderung. |
| **Nachbörslicher Selektor** (**After-hours selector**) | Nachbörslicher Kurs. |
| **Nachbörsliche Änderung** (**After-hours change**) | Prozentuale nachbörsliche Änderung. |
| **Nachrichtenselektor** (**News selector**) | Schlagzeilenlinks. Wählen Sie einen Anker oder einen Container aus, dessen Ergebnisse Links enthalten. |

Vor- und nachbörsliche Werte ergänzen den regulären Kurs; sie ersetzen ihn nicht. Eine
Seite kann diese Elemente außerhalb der jeweiligen Marktsitzung auslassen.

Vom veröffentlichten Beispiel verwendete Yahoo-Finance-Selektoren sind:

```text
Price:                  [data-testid="qsp-price"]
Price change:           section.primary span[data-testid="qsp-price-change-percent"]
Pre-market price:       section.secondary span[data-testid="qsp-pre-price"]
Pre-market change:      section.secondary span[data-testid="qsp-pre-price-change-percent"]
After-hours price:      section.secondary span[data-testid="qsp-post-price"]
After-hours change:     section.secondary span[data-testid="qsp-post-price-change-percent"]
```

Das Markup von Websites ändert sich im Lauf der Zeit. Betrachten Sie Beispiele als
Ausgangspunkt, nicht als dauerhafte Verträge.

### Selektoren ermitteln

Jedes Selektorfeld besitzt eine passende Schaltfläche **Ermitteln** (**Discover**).

1. Vervollständigen Sie die Quell-URL und genehmigen Sie die Website, falls eine
	 Genehmigung erforderlich ist.
2. Wählen Sie die Ermittlungsschaltfläche für den exakten Werttyp aus.
3. SmartTicker lädt öffentliches statisches HTML herunter und führt mögliche
	 Selektoren mit einem Beispielwert, Konfidenzprozentsatz und einer Begründung in der
	 QuickInfo auf.
4. Wählen Sie neben einem Vorschlag **Verwenden** (**Use**), um ihn in das passende
	 Feld zu kopieren.
5. Validieren oder beobachten Sie das Ergebnis, bevor Sie sich darauf verlassen.

Die Ermittlung führt kein JavaScript aus, meldet sich nicht an, umgeht keine
Zugriffskontrollen und untersucht Ihren Browser nicht. Für einen ausschließlich durch
JavaScript erzeugten Wert ist möglicherweise kein Selektor ermittelbar. Separate
Ermittlungstypen verhindern absichtlich eine Vermischung vor- und nachbörslicher Werte.

### Quelle validieren

**URL validieren** (**Validate URL**) fordert die Seite an und meldet den regulären
Kurs und/oder die Anzahl der lesbaren Schlagzeilen. Die Funktion kann gefahrlos vor
Eingabe eines Tickers verwendet werden, da SmartTicker für den Test eine temporäre
Bezeichnung nutzt.

Diese Validierung prüft derzeit nicht die vier Selektorfelder für vor- und
nachbörsliche Werte. Verwenden Sie deren Beispielwerte aus der Ermittlung und prüfen
Sie anschließend die angezeigten Sitzungsdaten.

Typische Fehler sind HTTP-Fehler, Zeitüberschreitungen, fehlende Werte, null
Schlagzeilen, eine nicht genehmigte Quelle, ausschließlich durch JavaScript erzeugte
Inhalte oder ein veralteter Selektor.

### Wiederholungslimit für Nachrichten

**Höchstens _N_-mal anzeigen** (**Show max _N_ times**) akzeptiert Werte von 1 bis 100
und ist standardmäßig auf 5 eingestellt. SmartTicker zählt jede abgeschlossene
Nachrichtenaktualisierung, bei der derselbe Schlagzeilentitel zurückgegeben wird, als
eine Anzeige. Sobald der Titel in der konfigurierten Anzahl von Zyklen erschienen ist,
wird er für den Rest der aktuellen Anwendungssitzung nicht mehr angezeigt. Durch
Bearbeiten oder Entfernen des Eintrags wird sein Wiederholungsverlauf gelöscht.

### Einträge bearbeiten, anordnen und entfernen

Die Liste **Konfigurierte Einträge** (**Configured entries**) zeigt Symbol, Gruppe,
Quelle, URL, Erfassungskennzeichen, Selektor für den regulären Kurs,
Nachrichtenselektor und Wiederholungslimit für Nachrichten.

- **Bearbeiten** (**Edit**) lädt den Eintrag in das Formular. Wählen Sie **Änderungen
	speichern** (**Save changes**), um sie anzuwenden, oder **Bearbeitung abbrechen**
	(**Cancel edit**), um Formularänderungen zu verwerfen.
- Die Aufwärts- und Abwärtspfeile ändern die Tickerreihenfolge und speichern sie sofort.
- **Entfernen** (**Remove**) löscht den Eintrag und seine derzeit angezeigten Daten.
- Wenn Warnregeln auf den Eintrag verweisen, fragt SmartTicker, ob diese Regeln
	gelöscht werden sollen. Eine Warnung ohne passenden konfigurierten Kurs kann nicht
	auslösen.
- Beim Umbenennen eines Eintrags werden die in Warnregeln angezeigten Symbole für die
	diesem Eintrag zugeordneten Regeln aktualisiert.

## App-Einstellungen

Öffnen Sie **App-Einstellungen...** (**App Settings...**) über das Kontextmenü.
Änderungen werden sofort wirksam und automatisch gespeichert; es gibt keine
Übernehmen-Schaltfläche.

### Tickerzeilen und Geschwindigkeit

| Einstellung | Optionen | Standard | Wirkung |
| --- | --- | --- | --- |
| Kurszeilen | 1 bis 8 | 1 | Anzahl paralleler Kurslaufbandzeilen. |
| Kurslaufgeschwindigkeit | 20, 30, 40, 50, 65, 80, 100 oder 120 px/sec | 50 | Geschwindigkeit des Kurslaufbands. |
| Nachrichtenzeilen | 1 bis 8 | 1 | Anzahl paralleler Schlagzeilenlaufbandzeilen. |
| Nachrichtenlaufgeschwindigkeit | 20, 30, 40, 50, 65, 80, 100 oder 120 px/sec | 40 | Geschwindigkeit des Nachrichtenlaufbands. |
| Schriftgröße der laufenden Ansicht | 9 bis 24 pt | 14 pt | Kurs- und Nachrichtentext in laufenden Zeilen. |
| Schriftgröße der statischen Ansicht | 9 bis 24 pt | 13 pt | Kurs- und Schlagzeilentext in statischen Zeilen. |
| Kursaktualisierung | 30 bis 300 Sekunden, in 15-Sekunden-Schritten | 60 Sekunden | Zeitraum, in dem jeder zulässige Kurseintrag eine geplante Aktualisierung erhält. |
| Nachrichtenaktualisierung | 30 bis 300 Sekunden, in 15-Sekunden-Schritten | 300 Sekunden | Zeitraum, in dem jeder zulässige Nachrichteneintrag eine geplante Aktualisierung erhält. |

Kurszeilen und Kurslaufgeschwindigkeit sind deaktiviert, während statische gruppierte
Tabellen aktiv sind, da dieser Modus alle Kurseinträge anzeigt und keines der beiden
Fenster automatisch scrollt. Einstellungen für Nachrichtenzeilen und -geschwindigkeit
bleiben für die laufende Ansicht erhalten.

Kurs- und Nachrichtenanfragen werden unabhängig voneinander über Ein-Sekunden-
Zeitfenster ihrer gesamten Intervalle verteilt, statt gleichzeitig zu starten. Bei 60
Einträgen über 30 Sekunden werden beispielsweise zwei Einträge pro Sekunde geplant;
bei fünf Einträgen über 30 Sekunden ungefähr alle sechs Sekunden einer. Es werden
höchstens vier Quellanfragen gleichzeitig ausgeführt, doppelte Arbeit für denselben
Eintrag und Datenstrom wird übersprungen und verpasste Zeitfenster werden nicht in
einem Schub nachgeholt. **Kurse jetzt aktualisieren** oder **Nachrichten jetzt
aktualisieren** startet nur den jeweiligen Datenstrom neu und fordert dessen erstes
Zeitfenster an. Vorhandene erfolgreiche Kurse und Schlagzeilen bleiben sichtbar,
während Ersatzdaten gelesen werden.

Jede HTTP-Anfrage besitzt eine feste Zeitüberschreitung von 20 Sekunden. Eine langsame
Quelle blockiert weder den UI-Dispatcher noch verhindert sie, dass spätere Zeitfenster
die verbleibende Anfragekapazität nutzen. SmartTicker meldet Fehler wie HTTP 403 und
429 und umgeht keine Einschränkungen. Die Anwendung analysiert oder erzwingt
robots-Anweisungen, crawl-delay-Werte oder Serveranweisungen zum Backoff nicht
automatisch. Wählen Sie daher regelkonforme Quellen und vermeiden Sie unnötig häufige
Anfragen.

### Fenstergrößen

Die App-Einstellungen speichern drei unabhängige Größenpaare:

| Fenster | Breite | Höhe | Standard |
| --- | --- | --- | --- |
| Laufende Ansicht | 420–7680 px | 50–900 px | 980 × 64 px |
| Statische Kursansicht | 420–7680 px | 420–4320 px | 980 × 420 px |
| Statische Nachrichtenansicht | 420–7680 px | 240–4320 px | 680 × 340 px |

Eine Änderung wird sofort angewendet, wenn das betreffende Fenster oder die betreffende
Ansicht aktiv ist. Das veröffentlichte Beispiel zeigt 1200 × 96 für die laufende
Ansicht, 1200 × 720 für statische Kurse und 760 × 480 für statische Nachrichten, mit
15-Punkt-Text in der laufenden und 14-Punkt-Text in der statischen Ansicht. Eine Höhe
der laufenden Ansicht, die kleiner als der für die aktivierten Zeilen benötigte Platz
ist, wird automatisch auf das erforderliche Minimum erhöht.

Legen Sie mit den vier Optionen unter **Ansicht** fest, ob Nachrichten angezeigt werden
und ob das Layout läuft oder statisch bleibt. Ein Ansichtswechsel löscht niemals
konfigurierte Einträge.

### SmartTicker bei der Anmeldung starten

Aktivieren Sie **SmartTicker bei meiner Anmeldung starten** (**Start SmartTicker when I
sign in**), um die installierte ausführbare Datei nur für den aktuellen Benutzer zu
registrieren.

- Unter Windows verwendet SmartTicker den Registrierungsschlüssel `Run` des aktuellen
	Benutzers.
- Auf Linux-Desktops, die die freedesktop-Autostartkonvention unterstützen, schreibt
	SmartTicker `smartticker.desktop` in das Autostartverzeichnis des Benutzers.
- Auf Plattformen ohne unterstützten Registrierungsmechanismus von SmartTicker ist die
	Option deaktiviert.

Das Betriebssystem ist maßgeblich. Wird der Autostart außerhalb von SmartTicker
geändert, zeigt das Kontrollkästchen beim nächsten Laden der Einstellungen den Zustand
des Betriebssystems an.

### Websitezugriff

**Website-Cookies und hostübergreifende Weiterleitungen zulassen** (**Allow website
cookies and cross-host redirects**) ist standardmäßig deaktiviert.

Bei deaktivierter Option:

- SmartTicker verlangt vor einer Anfrage eine ausdrückliche Genehmigung für jeden
	Websitehost.
- Website-Cookies werden nicht angenommen.
- Weiterleitungen zu einem anderen Host werden blockiert.
- Genehmigte Hosts werden in den lokalen Einstellungen gespeichert.

Bei aktivierter Option:

- SmartTicker überspringt den Genehmigungsschritt für einzelne Hosts.
- Von angeforderten Websites gesetzte Cookies werden nur in einem isolierten
	Arbeitsspeichercontainer gehalten und beim Beenden von SmartTicker gelöscht.
- Weiterleitungen zu anderen Hosts dürfen verfolgt werden.
- SmartTicker liest weiterhin keine Browser-Cookies, übermittelt keine Anmeldedaten und
	sendet keine Anmeldeformulare.

Wenn diese Option deaktiviert wird, werden derzeit angezeigte Daten von nicht
genehmigten Quellen entfernt, bis diese Hosts genehmigt und aktualisiert wurden.

#### Datenschutzoptionen der Website

Wird eine Antwort als Datenschutz-/Cookieformular mit einer positiven und einer
negativen Auswahl erkannt, hält SmartTicker an und zeigt Seitentitel, angeforderte URL,
Einwilligungs-URL, Formularzusammenfassung sowie die Beschriftungen für Akzeptieren und
Ablehnen der Website an.

- **Akzeptieren** (**Accept**) sendet die vom Formular bereitgestellten ausgeblendeten
	Felder zusammen mit genau dem von Ihnen ausgewählten Akzeptieren-Steuerelement.
- **Ablehnen** (**Reject**) sendet diese ausgeblendeten Felder zusammen mit genau dem
	von Ihnen ausgewählten Ablehnen-Steuerelement.
- **Abbrechen** (**Cancel**) sendet nichts.

Dies ist eine Datenschutzentscheidung für eine Website, nicht die SmartTicker-
Genehmigung für eine einzelne Quelle.

#### Alle Quellen validieren

Wählen Sie **Alle Quellen validieren** (**Validate all sources**), um jeden
konfigurierten Eintrag zu prüfen und zu testen.

1. Ist der Websitezugriff eingeschränkt, gruppiert SmartTicker nicht genehmigte
	 Einträge nach Hostname und zeigt für jeden Host einen Dialog zur Quellprüfung an.
2. Prüfen Sie Host, Richtlinienzusammenfassung, Hinweise, Quellnamen und Symbole.
3. Aktivieren Sie die Bestätigung nur, wenn Sie die Website geprüft haben und zu ihrer
	 Nutzung berechtigt sind.
4. Wählen Sie **Diese Quelle genehmigen** (**Approve this source**), **Diese Quelle
	 überspringen** (**Skip this source**) oder **Validierung abbrechen** (**Cancel
	 validation**).
5. SmartTicker testet jeden zulässigen Eintrag und meldet die Gesamtzahl erfolgreicher,
	 fehlgeschlagener und übersprungener Tests. Einzelne Probleme erscheinen unter der
	 Statuszeile.

Genehmigungsdatensätze dokumentieren eine Berechtigung innerhalb von SmartTicker; sie
gewähren keine gesetzlichen Rechte und setzen die Bedingungen der Website nicht außer
Kraft.

### Darstellung

**Fenstertransparenz** (**Window transparency**) ändert nur den Tickerhintergrund. Text
bleibt undurchsichtig. Der Bereich reicht in 5-%-Schritten von 20% bis 100%, der
Standardwert beträgt 100%.

Farbfelder akzeptieren hexadezimale Werte im Format `#RRGGBB` und bieten außerdem eine
Farbauswahl.

| Farbe | Standard | Verwendung |
| --- | --- | --- |
| Hintergrund | `#10151D` | Tickerhintergrund vor Anwendung der Transparenz. |
| Kursname | `#79C0FF` | Symbol-/Quellbezeichnung. |
| Schlusskurs | `#FFA657` | Regulärer Kurs. |
| Außerbörslich | `#00E5FF` | Vor- und nachbörsliche Kurse. |
| Nachricht 1 | `#FFFFFF` | Schlagzeilen 1, 5, 9 usw. |
| Nachricht 2 | `#00E5FF` | Schlagzeilen 2, 6, 10 usw. |
| Nachricht 3 | `#A3E635` | Schlagzeilen 3, 7, 11 usw. |
| Nachricht 4 | `#79C0FF` | Schlagzeilen 4, 8, 12 usw. |
| Änderung aufwärts | `#3FB950` | Positive prozentuale Änderungen. |
| Änderung abwärts | `#F85149` | Negative prozentuale Änderungen. |
| Warnblinken | `#FF00FF` | Ausgelöste Kurswarnungen, abwechselnd mit Schwarz. |

**Auf Standardwerte zurücksetzen** (**Reset to defaults**) stellt alle oben genannten
Farben und 100% Hintergrunddeckkraft wieder her. Zeilen, Geschwindigkeiten,
Schriftgrößen, Fenstergrößen, Quellen, Aktualisierungsintervalle, Warnungen und Sprache
werden nicht zurückgesetzt.

### Sichern und Wiederherstellen

SmartTicker speichert Anwendungseinstellungen und Warnregeln in getrennten JSON-Dateien
und stellt für jeden Sicherungstyp eigene Schaltflächen bereit.

#### Einstellungen exportieren und importieren

- **Einstellungen exportieren...** (**Export settings...**) schreibt konfigurierte
	Einträge, Gruppenzuordnungen, Gruppendefinitionen, ausgeblendete Nachrichtenkurse,
	Eintragsreihenfolge, Selektoren, die Auswahl der laufenden/statischen Kursansicht,
	genehmigte Hosts, Zeilensichtbarkeit, Zeilen, Geschwindigkeiten, Schriftgrößen der
	laufenden/statischen Ansicht, alle drei Fenstergrößenpaare,
	Aktualisierungsintervalle, Autostartpräferenz, Websitezugriffsoption, Farben
	einschließlich der Warnblinkfarbe, Transparenz und Sprache.
- **Einstellungen importieren...** (**Import settings...**) validiert die gesamte Datei,
	bevor Änderungen vorgenommen werden. Bei einer abgelehnten Datei bleiben die
	aktuellen Einstellungen unverändert.
- Ein erfolgreicher Import ersetzt alle konfigurierten Einträge und
	Anwendungseinstellungen. Die separate Warnregeldatei wird nicht ersetzt.
- Gruppen sind sowohl als Kurszuordnungen als auch als Gruppendefinitionen selbst in
	der Einstellungsdatei enthalten, sodass auch eine Gruppe ohne Kurse eine Sicherung
	übersteht. Es gibt keine separate Datei nur für den Export oder Import von Gruppen.
- Die Autostartpräferenz ist in einer Einstellungssicherung enthalten, doch ihr Import
	ändert die Autostartregistrierung des Betriebssystems nicht unbemerkt. Das
	Betriebssystem bleibt maßgeblich; verwenden Sie das Autostart-Kontrollkästchen, um die
	Registrierung auf dem aktuellen Computer zu ändern.
- Importdateien sind auf 1 MiB, Schemaversion 1 und höchstens 200 Abonnements begrenzt.
	Unbekannte Eigenschaften, doppelte IDs, fehlerhafte URLs, ungültige Farben,
	ungültige Bereiche oder nicht unterstützte Sprachcodes werden abgelehnt statt
	unbemerkt ignoriert.

#### Warnregeln exportieren und importieren

- **Warnregeln exportieren...** (**Export alert rules...**) schreibt alle Regeln sowie
	Buzz, Buzz-Anzahl und Blinkdauer.
- **Warnregeln importieren...** (**Import alert rules...**) validiert die gesamte Datei
	und ersetzt anschließend alle aktuellen Regeln und Einstellungen für das Auslösen von
	Warnungen.
- Regeln werden zunächst über die Abonnement-ID wieder verknüpft. Unterscheiden sich
	die IDs, versucht SmartTicker eine Übereinstimmung des Symbols ohne Beachtung der
	Groß-/Kleinschreibung.
- Eine importierte Regel ohne passenden Kurs bleibt erhalten, kann aber nicht auslösen.
	Der Importstatus meldet, wie viele Regeln erneut verknüpft wurden oder ohne Zuordnung
	bleiben.
- Importdateien für Warnungen sind auf 1 MiB begrenzt.

Für die Übertragung auf einen anderen Computer importieren Sie zuerst die
Anwendungseinstellungen und anschließend die Warnregeln. Werden Warnungen als Zweites
importiert, können die Regeln anhand des Symbols wieder mit den neuen Abonnement-IDs
verknüpft werden.

### Konfigurationsdateien direkt bearbeiten

**Aktuelle App-Konfiguration bearbeiten** (**Edit Current App Config**) und **Aktuelle
Warnregeln bearbeiten** (**Edit Current Alert Rules**) in den App-Einstellungen öffnen
die aktive JSON-Datei in dem Texteditor, den Ihr System mit `.json` verknüpft. Diese
Funktion richtet sich an fortgeschrittene Benutzer; die Fenster in SmartTicker decken
dieselben Einstellungen ohne dieses Risiko ab.

Beide Schaltflächen zeigen zunächst eine Bestätigung an, die Sie zum Export der
aktuellen Datei auffordert. Führen Sie diesen Export aus: Eine manuelle Bearbeitung
kann die Datei unbrauchbar machen und lässt sich nicht rückgängig machen.

- **Vorhandene Konfiguration exportieren...** (**Export existing config...**) speichert
	die aktuelle Datei und kehrt anschließend zur selben Aufforderung zurück.
- **Im Texteditor öffnen** (**Open in text editor**) öffnet die aktive Datei.
- **Abbrechen** (**Cancel**) ändert nichts.

SmartTicker überwacht die Datei und lädt sie neu, sobald Ihr Editor sie speichert:

- Eine gültige Datei wird sofort angewendet und der Ticker ohne Neustart aktualisiert.
- Fehlerhaftes JSON, ein Schemaverstoß oder ein anderer Validierungsfehler wird
	abgelehnt. Ihre laufende Konfiguration bleibt unangetastet und das Fenster
	„App-Einstellungen“ meldet das Problem.
- Korrigieren Sie nach einer abgelehnten Bearbeitung die Datei oder stellen Sie einen
	gültigen Export mit **Einstellungen importieren...** oder **Warnregeln
	importieren...** wieder her.
- Eine Datei, die durch ein anderes Programm gesperrt bleibt, wird kurzzeitig erneut
	versucht und anschließend als Problem gemeldet.

Die Bearbeitung der Warnregeldatei folgt denselben Regeln und wirkt sich nicht auf die
Anwendungseinstellungen aus, da beide Dateien getrennt sind.

## Warnregeln

Öffnen Sie **Warnungen** (**Alerts**) über das Kontextmenü. Regeln werden nach jeder
erfolgreichen Kursaktualisierung ausgewertet und überwachen nur den regulären Kurs,
nicht vor- oder nachbörsliche Werte.

### Regel erstellen

1. Wählen Sie einen konfigurierten **Kurs** (**Quote**). Einträge mit demselben Symbol
	 bleiben voneinander getrennt.
2. Wählen Sie eine **Bedingung** (**Condition**) und geben Sie einen numerischen
	 Schwellenwert mit einem kulturunabhängigen Dezimaltrennzeichen ein, zum Beispiel
	 `250.50`.
3. Wählen Sie optional **Aktiv ab** (**Active from**). Lassen Sie das Feld leer, um die
	 Regel sofort zu aktivieren.
4. Lassen Sie **Läuft nie ab** (**Never expires**) aktiviert oder deaktivieren Sie es
	 und wählen Sie ein Ablaufdatum.
5. Wählen Sie **Regel hinzufügen** (**Add rule**).

Folgende Vergleiche sind verfügbar:

| Auswahl | Bedeutung |
| --- | --- |
| `LessThan` | Kurs `<` Schwellenwert. |
| `LessThanOrEqual` | Kurs `<=` Schwellenwert. |
| `GreaterThan` | Kurs `>` Schwellenwert. |
| `GreaterThanOrEqual` | Kurs `>=` Schwellenwert. |
| `EqualTo` | Kurs entspricht exakt dem Schwellenwert. |
| `NotEqualTo` | Kurs weicht vom Schwellenwert ab. |

Die Startgrenze ist einschließlich. Auch die Ablaufgrenze ist einschließlich; nach
ihrem Überschreiten löst die Regel nicht mehr aus. SmartTicker lehnt ein Ablaufdatum
ab, das vor dem Startdatum liegt.

### Wenn eine Regel auslöst

Eine aktivierte, zeitlich gültige Regel löst einmal aus, wenn ihre Bedingung von falsch
zu wahr wechselt. Solange die Bedingung wahr bleibt, erfolgt nicht bei jeder
Aktualisierung eine Benachrichtigung. Nachdem der Kurs die Bedingung verlassen hat,
wird die Regel erneut scharfgeschaltet und kann beim nächsten Eintritt des Kurses
wieder auslösen.

Auch das Bearbeiten einer Regel oder ihr Deaktivieren und erneutes Aktivieren schaltet
sie wieder scharf. Eine aktivierte Regel kann daher sofort auslösen, wenn der jüngste
reguläre Kurs ihre Bedingung bereits erfüllt. Ein fehlgeschlagener oder fehlender Kurs
kann keine Regel auslösen.

Wenn eine oder mehrere Regeln auslösen:

- Der betroffene Kurseintrag wechselt für die konfigurierte Dauer zwischen der
	eingestellten Warnblinkfarbe und Schwarz. Die Standard-Blinkfarbe ist Magenta
	(`#FF00FF`).
- Wenn **Buzz** aktiviert ist, spielt SmartTicker die konfigurierte Buzz-Sequenz ab.
- Die Warnmeldung bezeichnet eine einzelne Regel oder meldet die Anzahl der gemeinsam
	ausgelösten Regeln.
- Das Tickerlaufband läuft weiter, während die Warnhervorhebung aktiv ist.

### Einstellungen für Warnausgaben

| Einstellung | Bereich | Standard |
| --- | --- | --- |
| **Buzz** | Ein oder Aus | Ein |
| Buzz-Anzahl | 1 bis 20 | 15 |
| **Blinken für** (**Blink for**) | 5 bis 900 Sekunden, in 15-Sekunden-Schritten | 60 Sekunden |

Durch Deaktivieren von Buzz bleibt die visuelle Warnung aktiv. Wenn bei derselben
Auswertung mehrere Regeln auslösen, startet SmartTicker für diese Auswertung eine
konfigurierte Buzz-Sequenz. Ändern Sie **Warnblinken** (**Alert blink**) unter
**App-Einstellungen > Darstellung**. Es handelt sich um eine Einstellung der
Anwendungsdarstellung; sie ist daher im Export/Import der Einstellungen und nicht in
der separaten Warnregeldatei enthalten.

### Konfigurierte Regeln verwalten

- **Bearbeiten** (**Edit**) lädt eine Regel in das Formular. Wählen Sie **Regel
	aktualisieren** (**Update rule**) zum Speichern oder **Abbrechen** (**Cancel**), um sie
	unverändert zu lassen.
- **Deaktivieren** (**Disable**) behält die Regel bei, verhindert aber Treffer.
	**Aktivieren** (**Enable**) schaltet sie wieder scharf und wertet sie anhand des
	neuesten regulären Kurses aus.
- **Entfernen** (**Remove**) löscht die Regel.
- Die Liste zeigt Aktivierungszustand, Symbol, Zusammenfassung der Bedingung und
	Zeitplan.

Änderungen an Warnregeln und Einstellungen für die Warnausgabe werden automatisch
gespeichert.

## Lokale Dateien und Datenschutz

SmartTicker speichert die Konfiguration lokal und synchronisiert sie nicht mit einem
Entwicklerdienst.

Unter Windows lauten die Standarddateien:

```text
%LocalAppData%\SmartTicker\settings.json
%LocalAppData%\SmartTicker\alerts.json
```

Unter Linux verwendet .NET das lokale Anwendungsdatenverzeichnis des aktuellen
Benutzers, normalerweise:

```text
~/.local/share/SmartTicker/settings.json
~/.local/share/SmartTicker/alerts.json
```

### Isoliertes Datenverzeichnis verwenden

Für erweiterte Diagnosen und Testläufe kann vor dem Start von SmartTicker
`SMARTTICKER_DATA_DIRECTORY` gesetzt werden. Ist der Wert nicht leer, werden beide
Dateien direkt in diesem aufgelösten Verzeichnis als `settings.json` und `alerts.json`
abgelegt; die oben genannten Plattformstandards werden für diesen Prozess nicht
verwendet. Verwenden Sie vorzugsweise einen absoluten Pfad und stellen Sie sicher, dass
er beschreibbar ist.

PowerShell-Beispiel:

```powershell
$env:SMARTTICKER_DATA_DIRECTORY = 'D:\SmartTicker-Profile'
& 'C:\Program Files\SmartTicker\SmartTicker.Desktop.exe'
```

Linux-Shell-Beispiel:

```bash
SMARTTICKER_DATA_DIRECTORY="$HOME/.local/share/SmartTicker-Test" smartticker
```

Setzen Sie die Variable vor dem Prozessstart. SmartTicker kopiert das Standardprofil
nicht in das ausgewählte Verzeichnis, sodass ein leeres Verzeichnis mit einer leeren
Konfiguration beginnt. Instanzen, die auf dasselbe Verzeichnis verweisen, können die
gespeicherten Änderungen der jeweils anderen Instanz erkennen. Verwenden Sie die
normalen Befehle zum Exportieren/Importieren von Einstellungen und Warnregeln für
Sicherungen und Profilübertragungen.

Das Warnungsfenster zeigt den exakten Pfad der verwendeten Warnungsdatei an. Beim
Schreiben wird zunächst eine temporäre Datei angelegt und anschließend ersetzt, damit
eine teilweise geschriebene Datei nicht als aktuelle Konfiguration behandelt wird.

SmartTicker besitzt weder Konto noch Telemetrie, Analysefunktionen, Werbung oder
Cloudsynchronisierung. Eine Quellwebsite empfängt normale Netzwerkinformationen wie
Ihre IP-Adresse, wenn SmartTicker diese Quelle anfordert. Beim Öffnen der Hilfe wird der
unbearbeitete Leitfaden von GitHub angefordert. Vollständige Informationen finden Sie
in `PRIVACY.md` im Repository.

Sie sind dafür verantwortlich, dass jede Quell-URL und jeder Selektor im Einklang mit
den Bedingungen, der Lizenz, den robots-Anweisungen und dem anwendbaren Recht der
Website verwendet wird.

## Problembehandlung

### Ein Kurs zeigt Nicht verfügbar oder keinen Preis

Eine Quellanfrage läuft nach 20 Sekunden ab. Besitzt dieser Kurs bereits einen früheren
erfolgreichen Snapshot, bleibt er nach einer fehlgeschlagenen Aktualisierung sichtbar;
andernfalls zeigt der Kurs bis zu einer späteren erfolgreichen Aktualisierung **Nicht
verfügbar** (**Unavailable**) an. Lesen Sie vor dem Ändern von Selektoren den genauen
Validierungs- oder Aktualisierungsfehler.

1. Öffnen Sie **Kurse...**, bearbeiten Sie den Eintrag und prüfen Sie die vollständige
	 URL.
2. Vergewissern Sie sich, dass **Kurs** ausgewählt ist.
3. Genehmigen Sie die Website, wenn Sie dazu aufgefordert werden.
4. Wählen Sie **URL validieren** und lesen Sie das genaue Ergebnis.
5. Führen Sie **Kurs ermitteln** (**Discover price**) aus oder untersuchen Sie das
	 statische HTML der Seite und aktualisieren Sie den Selektor.
6. Prüfen Sie, ob die Seite JavaScript, Authentifizierung oder eine Einwilligung
	 erfordert, die SmartTicker nicht sicher verarbeiten kann.
7. Beachten Sie HTTP 403, 429, robots-Einschränkungen und die Richtlinie der Website für
	 automatisierten Zugriff.

### Vor- oder nachbörsliche Daten fehlen

- Die entsprechende Marktsitzung ist möglicherweise nicht aktiv.
- Die Seite kann das Sitzungselement auslassen, wenn kein Sitzungswert vorhanden ist.
- Prüfen Sie, dass vorbörsliche Selektoren vorbörsliche Elemente und nachbörsliche
	Selektoren nachbörsliche Elemente adressieren.
- Führen Sie den passenden Ermittlungsbefehl erneut aus, da sich das Website-Markup
	geändert haben könnte.

### Nachrichten sind leer

- Vergewissern Sie sich, dass **Nachrichten** ausgewählt ist.
- Validieren Sie die Quelle und führen Sie **Nachrichten ermitteln** (**Discover news**)
	aus.
- Stellen Sie sicher, dass der Selektor Links mit sichtbarem Schlagzeilentext liefert.
- Eine fehlgeschlagene oder abgelaufene Nachrichtenanfrage behält frühere erfolgreiche
	Schlagzeilen bei, wenn diese verfügbar sind. Eine Quelle ohne erfolgreiches Ergebnis
	bleibt bis zu einem späteren erfolgreichen Zeitfenster leer.
- Eine Schlagzeile verschwindet, nachdem sie das konfigurierte Wiederholungslimit für
	diese Sitzung erreicht hat.
- Prüfen Sie in den statischen Nachrichten, ob der gewünschte Kurs unter **Nachrichten
	anzeigen für** aktiviert ist.

### Selektorermittlung findet nichts

Die Ermittlung liest nur das heruntergeladene statische HTML. Werte, die erst später
durch JavaScript der Seite erzeugt werden, sind für sie unsichtbar. Geben Sie einen
geprüften Selektor manuell ein, wählen Sie eine statische Seite/einen statischen Feed
oder verwenden Sie eine autorisierte dokumentierte API über eine kompatible öffentliche
Seite.

### Eine Warnung löst nicht aus

- Prüfen Sie, dass der zugeordnete Kurs noch vorhanden ist, Kurse erfasst und einen
	erfolgreichen regulären Kurs besitzt.
- Prüfen Sie, dass die Regel aktiviert ist und innerhalb ihres Start-/Ablaufzeitplans
	liegt.
- Prüfen Sie Vergleich und Schwellenwert. `EqualTo` erfordert exakte Dezimalgleichheit.
- Denken Sie daran, dass eine dauerhaft wahre Bedingung einmal auslöst. Sie muss falsch
	werden, bevor sie erneut auslösen kann, sofern Sie die Regel nicht bearbeiten oder
	erneut aktivieren.
- Vor- und nachbörsliche Kurse steuern keine Warnregeln.

### SmartTicker lässt sich nicht verschieben oder in der Größe ändern

- Verschieben Sie das Fenster ausschließlich am Griff mit vertikalen Punkten im linken
	Streifen.
- Ändern Sie die Größe an einer Kante oder Ecke; verwenden Sie die sichtbare Markierung
	unten rechts, wenn eine Kante schwer zu treffen ist.
- Tickerinhalte sind absichtlich keine Verschiebefläche.

### Statische Gruppen oder Werte entsprechen nicht den Erwartungen

- Öffnen Sie **Kurse...** und prüfen Sie den Gruppenwert jedes Eintrags.
- Öffnen Sie **Kursgruppen...**, um Gruppendefinitionen zu verwalten und die aktuelle
	Zuordnung jedes Kurses zu prüfen.
- Einträge mit leerer Gruppe erscheinen unter **Nicht gruppiert**.
- **Änd.** wird aus dem letzten Kurs und Änd.% berechnet; der Wert wird nicht unabhängig
	aus der Seite extrahiert. Wenn der Prozentwert nicht verfügbar ist, bleibt `—` stehen.
- Ordnen Sie Einträge mit den Aufwärts-/Abwärts-Steuerelementen neu an, um die Gruppen-
	und Zeilenreihenfolge zu ändern.
- Ziehen Sie den gepunkteten Griff an einer Kachelüberschrift, um die gesamte Gruppe zu
	verschieben. Legen Sie sie auf der linken Hälfte einer anderen Kachel ab, um sie davor
	zu platzieren, oder auf der rechten Hälfte, um sie danach zu platzieren.
- Wählen Sie **Kurse jetzt aktualisieren**, während SmartTicker nicht angehalten ist,
	um die Tabelle zu aktualisieren.

### Hilfetext ist nicht formatiert oder Navigation bewegt sich nicht

- Das Hilfefenster sollte formatierte Überschriften, Absätze, Listen, Tabellen, Links
	und Codeblöcke statt Markdown-Satzzeichen anzeigen.
- Verwenden Sie links **Auf dieser Seite** (**On this page**), um zu einem Hauptabschnitt
	zu springen. Auch Links in der Schnellnavigationstabelle scrollen innerhalb des
	Dokuments.
- Schließen und öffnen Sie die Hilfe erneut oder ändern Sie die **Sprache** (**Language**),
	um den passenden veröffentlichten Onlineleitfaden anzufordern. Bis er eintrifft, zeigt
	SmartTicker den passenden formatierten Leitfaden an, der in der installierten Anwendung
	eingebettet ist.

### Onlinehilfe ist nicht verfügbar oder veraltet

- Schließen und öffnen Sie die Hilfe erneut, um den veröffentlichten Leitfaden erneut
	anzufordern.
- Öffnen Sie die am Anfang dieses Leitfadens angezeigte unbearbeitete GitHub-Adresse in
	einem Browser, um die veröffentlichte Datei direkt zu prüfen.
- SmartTicker verwendet den eingebetteten Leitfaden, wenn die Anfrage fehlschlägt oder
	eine leere Datei zurückgibt.
- Onlineänderungen werden erst angezeigt, nachdem `HELPME.md` oder die passende
	lokalisierte Datei `help/HELPME.de.md` im `main`-Branch des Repositorys
	veröffentlicht wurde.

## Unterstützung

Melden Sie reproduzierbare Probleme unter:

<https://github.com/bulentozkir/smartticker/issues>

Geben Sie SmartTicker-Version, Betriebssystem, Quellhostname, Validierungsstatus und
den genauen Fehlertext an. Entfernen Sie vor dem Veröffentlichen private URLs und
andere vertrauliche Informationen.