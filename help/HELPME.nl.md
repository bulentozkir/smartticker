# SmartTicker-help

Deze handleiding is van toepassing op SmartTicker 1.0.3. Hierin worden de hoofdticker, App-instellingen,
koersen, waarschuwingsregels, websitebevoegdheden, back-ups en veelvoorkomende problemen uitgelegd.

SmartTicker leest openbare statische HTML van webpagina's die u configureert. Het biedt geen
marktgegevensfeed en geëxtraheerde informatie kan vertraagd, onvolledig of onjuist zijn.
Controleer belangrijke financiële informatie bij een gezaghebbende bron.

## Snelle navigatie

| Onderdeel | Ga naar |
| --- | --- |
| Aan de slag | [Help- en configuratievensters openen](#help--en-configuratievensters-openen) |
| Hoofdticker | [Bediening](#bediening-van-de-hoofdticker) · [Scrollende of statische weergave](#kiezen-tussen-scrollende-of-statische-koersenweergave) · [Verplaatsen](#de-ticker-verplaatsen) · [Formaat wijzigen](#het-formaat-van-de-ticker-wijzigen) · [Pauzeren](#pauzeren-en-hervatten) · [Menuoverzicht](#overzicht-van-het-hoofdmenu) |
| Koersen en nieuws | [Koersen](#koersen) · [Item toevoegen](#een-koers--of-nieuwsitem-toevoegen) · [Koersen groeperen](#koersen-groeperen) · [Bron-URL's](#bronvoorinstellingen-en-urls) · [Selectors](#naslag-voor-selectorvelden) · [Ontdekken](#selectors-ontdekken) · [Validatie](#een-bron-valideren) |
| Toepassingsvoorkeuren | [App-instellingen](#app-instellingen) · [Rijen en snelheid](#tickerrijen-en-snelheid) · [Opstarten](#smartticker-starten-bij-aanmelden) · [Websitetoegang](#websitetoegang) · [Uiterlijk](#uiterlijk) · [Back-up en herstel](#back-up-maken-en-herstellen) · [Configuratiebestanden bewerken](#de-configuratiebestanden-ter-plaatse-bewerken) |
| Prijswaarschuwingen | [Waarschuwingsregels](#waarschuwingsregels) · [Regel maken](#een-regel-maken) · [Activeringsgedrag](#wanneer-een-regel-wordt-geactiveerd) · [Waarschuwingsuitvoer](#instellingen-voor-waarschuwingsuitvoer) · [Regels beheren](#geconfigureerde-regels-beheren) |
| Gegevens en ondersteuning | [Lokale bestanden en privacy](#lokale-bestanden-en-privacy) · [Problemen oplossen](#problemen-oplossen) · [Ondersteuning](#ondersteuning) |

## Help- en configuratievensters openen

Klik met de rechtermuisknop op de ticker om het menu te openen. De belangrijkste configuratieopdrachten zijn:

- **Quotes...** (*Koersen...*): koers- of nieuwsbronnen toevoegen, testen, bewerken, ordenen en verwijderen.
- **Quote groups...** (*Koersgroepen...*): groepen maken, bijwerken of verwijderen en koersen eraan koppelen.
- **Alerts** (*Waarschuwingen*): prijswaarschuwingsregels maken en beheren.
- **App Settings...** (*App-instellingen...*): rijen, snelheden, vernieuwingsintervallen, opstarten, website-
	toegang, kleuren, transparantie en back-ups configureren.
- **View** (*Weergave*): een van vier elkaar uitsluitende combinaties selecteren: scrollend of statisch,
	met alleen Prices (*Prijzen*) of Prices with News (*Prijzen met nieuws*).
- **Help** (*Help*): deze handleiding in SmartTicker openen.
- **About SmartTicker** (*Over SmartTicker*): de geïnstalleerde versie en licentievermelding tonen.
- **Exit** (*Afsluiten*): SmartTicker volledig sluiten.

Het Help-venster maakt de ingebouwde handleiding voor de geselecteerde app-taal
onmiddellijk op en geeft deze weer. Daarna controleert het de bijbehorende onlinehandleiding
telkens wanneer u Help opent of **Language** (*Taal*) wijzigt. De Nederlandse onlinehandleiding is:

<https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/help/HELPME.nl.md>

Als het online document niet kan worden gedownload, blijft SmartTicker de bij de geselecteerde
taal behorende ingebouwde vertaling weergeven. Als u **Language** (*Taal*) wijzigt, worden de titel,
status, navigatie en volledige handleiding in een geopend Help-venster onmiddellijk bijgewerkt.
Sluit Help met de normale sluitknop in de titelbalk.

## Bediening van de hoofdticker

### Kiezen tussen scrollende of statische koersenweergave

SmartTicker biedt vier elkaar uitsluitende weergavemodi. Klik met de rechtermuisknop op de ticker, open
**View** (*Weergave*) en selecteer er een. De indeling verandert onmiddellijk en uw keuze wordt opgeslagen.

| Weergaveoptie | Resultaat |
| --- | --- |
| **Left-to-right scroll: Prices only** (*Van links naar rechts scrollen: alleen prijzen*) | Prijzenmarquee in de hoofdticker; geen nieuwsweergave. Dit is de standaardinstelling. |
| **Left-to-right scroll: Prices with News** (*Van links naar rechts scrollen: prijzen met nieuws*) | Prijzen- en nieuwsmarquees in de hoofdticker. |
| **Static view: Prices only** (*Statische weergave: alleen prijzen*) | Responsieve prijstegels in het hoofdvenster; geen News-venster (*Nieuws*). |
| **Static view: Prices with News** (*Statische weergave: prijzen met nieuws*) | Responsieve prijstegels plus een afzonderlijk statisch venster **SmartTicker News**. |

Instellingenbestanden die zijn gemaakt voordat deze keuzes werden toegevoegd, worden toegewezen aan de overeenkomstige combinatie
van hun opgeslagen instellingen voor scrollend/statisch en nieuws. De weergavemodus wordt uitsluitend beheerd via
het **View**-menu (*Weergave*) dat u opent door met de rechtermuisknop op de ticker te klikken.

- In beide scrollmodi gebruiken prijzen de horizontale marquee en het geconfigureerde aantal prijsrijen
	en de scrollsnelheid.
- In beide statische modi verschijnen groepen als responsieve tegels die van links naar rechts zijn ingedeeld. Tegels
  lopen alleen door op een volgende rij wanneer het venster te smal is. Prijzen bewegen niet
  automatisch.
- Elke koerstegel heeft eigen uitgelijnde kolommen **Symbol** (*Symbool*), **Last** (*Laatste*), **Chg** (*Wijz.*) en **Chg%** (*Wijz.%*).
  **Chg** wordt afgeleid
	van Last en Chg%, omdat bronpagina's een percentageselector bieden in plaats van een
	afzonderlijke selector voor absolute verandering. Er wordt `—` weergegeven wanneer een van beide waarden niet beschikbaar is.
- Selecteer de kop van een groep om deze in of uit te klappen. Groepen volgen het eerste voorkomen
	van hun koersen in de volgorde van de geconfigureerde items; rijen binnen een groep behouden die volgorde.
- Items zonder groep verschijnen onder **Ungrouped** (*Niet gegroepeerd*).
- Beweeg de aanwijzer over Last om beschikbare pre-market- en after-hourswaarden te zien. Dubbelklik op een
	koersrij om de bronpagina te openen.
- Knipperende waarschuwingen en kleuren voor stijgingen/dalingen werken in beide prijsmodi.
- Nieuws wordt automatisch geopend in een afzonderlijk venster **SmartTicker News** met statische
	groepstegels **Symbol / Headline** (*Symbool / Kop*). In de statische modus wordt het niet als marquee weergegeven. Het News-
	venster heeft een normale titelbalk en rand voor formaatwijziging, zodat de vensters Quotes en News
	onafhankelijk naar verschillende beeldschermen kunnen worden verplaatst. Dubbelklik op een kopregel om
	de bron te openen.
- Bij de eerste start gebruikt News een compact formaat van 680×340. SmartTicker plaatst het op een ander
	beeldscherm wanneer dat beschikbaar is; op één beeldscherm probeert het eerst een vrije ruimte onder,
	rechts, boven of links van Prices. Daarna kunt u het normaal verplaatsen en het formaat wijzigen.
- Binnen elke News-groep worden koppen per koers afgewisseld: één kop van de
	eerste koers, vervolgens één van de volgende koers, en zo in rondes verder. Een koers met veel
	koppen kan daardoor niet de hele bovenkant van zijn groep innemen.
- Open de eenregelige vervolgkeuzelijst **Show news for** (*Nieuws tonen voor*) en schakel elke koers
	afhankelijk van de andere in of uit. Elke combinatie van koersen kan zichtbaar zijn, inclusief alle of geen. De
	knop vat de huidige keuze samen en items bevatten de koers en bron, zodat
	dubbele symbolen onafhankelijk blijven. Uitgeschakelde koersen worden in uw instellingenbestand opgeslagen
	als `hiddenNewsQuotes`, zodat ze na opnieuw starten behouden blijven en meegaan met een instellingenback-up.
- Sleep de gestippelde greep naast een koers- of nieuwstegelkoptitel en zet deze neer op de linker-
	of rechterhelft van een andere tegel. De volgorde verandert in beide vensters en wordt opgeslagen door
	de onderliggende geconfigureerde items opnieuw te ordenen.
- Een groep met veel rijen scrolt binnen zijn eigen begrensde tegel. De volledige weergave scrolt
	alleen verticaal wanneer de doorlopende tegelrijen niet binnen de huidige vensterhoogte passen.

Als u **SmartTicker News** sluit, wordt het verzamelen van nieuws niet uitgeschakeld. U opent het opnieuw door met de rechtermuisknop
op het Prices-venster te klikken en **View > Open static news window** (*Weergave > Statisch nieuwsvenster openen*) te selecteren. Als u **Static
view: Prices only** selecteert, wordt het gesloten; met **Static view: Prices with News** wordt het
opnieuw geopend. Beide scrollkeuzes sluiten het afzonderlijke News-venster; de scrollkeuze
Prices-with-News herstelt de nieuwsmarquee in de hoofdticker.

Bij het wisselen van modus wordt het opgeslagen formaat voor die weergave toegepast. De scrollende ticker, het statische Prices-
venster en het statische News-venster bewaren elk een eigen breedte en hoogte.

### De ticker verplaatsen

Houd de greep met verticale stippen boven aan de smalle linkerstrook ingedrukt, sleep de
ticker en laat de muisknop los. Tickerttekst is geen sleepvlak, zodat het selecteren
of aanklikken van inhoud niet per ongeluk een vensterverplaatsing start.

### Het formaat van de ticker wijzigen

Beweeg de aanwijzer naar een rand of hoek totdat een cursor voor formaatwijziging verschijnt, druk vervolgens en
sleep. Rechtsonder staat een kleine zichtbare markering voor formaatwijziging. De minimale vensterbreedte
is 420 pixels. De scrollhoogte is 50 tot en met 900 pixels, de hoogte van statische Prices is 420
tot en met 4320 pixels en de hoogte van statische News is 240 tot en met 4320 pixels.

Bij handmatig wijzigen worden de opgeslagen afmetingen van de actieve weergave bijgewerkt nadat het slepen is gestopt.
Alle drie formaatparen zijn opgenomen in een instellingenback-up. Vensterposities worden niet opgeslagen.
Als een scrollformaat te laag is voor de geselecteerde Price/News-rijen en de scrollende tekengrootte,
verhoogt SmartTicker die opgeslagen hoogte automatisch. Door **Left-to-right
scroll: Prices with News** te selecteren, is er daarom altijd ruimte voor de News-rijen in plaats van dat
ze ongemerkt worden verborgen.
Wanneer een venster wordt geopend of verplaatst, houdt SmartTicker ten minste de linkerbovenhoek van 32 pixels
binnen het werkgebied van een beeldscherm en begrenst het globale X en Y tot minimaal 1. Daardoor blijft de verplaatsingsgreep
of titelhoek met de muis bereikbaar, zelfs nadat een beeldscherm is losgekoppeld.

### Pauzeren en hervatten

Selecteer de statusknop onder de verplaatsingsgreep, of klik met de rechtermuisknop en selecteer
**Pause / Resume** (*Pauzeren / Hervatten*). Pauzeren stopt automatische prijs- en nieuwsvernieuwingen en bevriest de
marquee. Het voorkomt ook dat een van de handmatige vernieuwingsopdrachten nieuw werk start. Een bronaanvraag
die al bezig was, wordt niet uitsluitend vanwege Pause geforceerd geannuleerd en kan
worden voltooid voordat alle activiteit volledig tot rust komt. Resume start de automatische timers opnieuw.

In Windows stelt SmartTicker de OS-procesprioriteit automatisch in op **Low** (*Laag*) en schakelt het
Windows **Efficiency mode** (*Efficiëntiemodus*, EcoQoS) in voordat de gebruikersinterface wordt gestart. Er is geen app-instelling voor
dit gedrag. Het gebruikt ook een software-renderingpad met lage overhead. De marquee-timing past zich
aan de geconfigureerde snelheid aan en een gepauzeerde, lege of losgekoppelde marquee stopt zijn animatie-
timer. Ongewijzigde rijen onderdrukken overbodige visuele meldingen. Knipperende waarschuwingen en de
bruine wijzigingsmarkering van drie seconden zijn opzettelijk en pauzeren het scrollen niet. Linux-
procesplanning wordt aan het besturingssysteem overgelaten. Als Windows een van beide procesinstellingen
weigert, meldt SmartTicker de fout in de diagnostische tracering en gaat het verder met opstarten.

### Links openen

Dubbelklik op gekoppelde tickerttekst, waaronder een nieuwskop, om de bron in uw
standaardbrowser te openen. SmartTicker opent links niet met één klik.

### Wijzigingen markeren

Na elke vernieuwing markeert SmartTicker gedurende drie seconden kort op een bruine achtergrond wat is gewijzigd:

- Een koers waarvan de prijs afwijkt van de vorige synchronisatie.
- Elke kop die bij de vorige synchronisatie voor die koers niet aanwezig was.

De eerste synchronisatie na het opstarten markeert niets, omdat er geen eerdere waarde is om mee
te vergelijken. Een geactiveerde waarschuwing behoudt de eigen knipperkleur en heeft voorrang.

### Overzicht van het hoofdmenu

| Opdracht | Effect |
| --- | --- |
| **Refresh prices now** (*Prijzen nu vernieuwen*) | De gespreide prijscyclus opnieuw starten en het eerste tijdslot aanvragen wanneer SmartTicker niet is gepauzeerd. |
| **Refresh news now** (*Nieuws nu vernieuwen*) | De gespreide nieuwscyclus opnieuw starten en het eerste tijdslot aanvragen wanneer SmartTicker niet is gepauzeerd. |
| **Pause / Resume** (*Pauzeren / Hervatten*) | Vernieuwen en marquee-beweging in- of uitschakelen. |
| **View > Left-to-right scroll: Prices only** (*Weergave > Van links naar rechts scrollen: alleen prijzen*) | Alleen de horizontale prijzenmarquee gebruiken. Dit is de standaardinstelling. |
| **View > Left-to-right scroll: Prices with News** (*Weergave > Van links naar rechts scrollen: prijzen met nieuws*) | Beide horizontale marquees gebruiken. |
| **View > Static view: Prices only** (*Weergave > Statische weergave: alleen prijzen*) | Alleen responsieve statische koerstegels gebruiken. |
| **View > Static view: Prices with News** (*Weergave > Statische weergave: prijzen met nieuws*) | Koerstegels plus het afzonderlijke statische News-venster gebruiken. |
| **View > Open static news window** (*Weergave > Statisch nieuwsvenster openen*) | Het afzonderlijke News-venster opnieuw openen nadat het is gesloten. Beschikbaar in de statische modus wanneer nieuws is ingeschakeld. |
| **Language** (*Taal*) | Een van de 16 talen kiezen voor menu's, statustekst en de volledige Help-handleiding. Een geopend Help-venster wordt onmiddellijk bijgewerkt. |

Zichtbaarheid van regels, taal en de overige configuratiewaarden worden automatisch opgeslagen.

## Koersen

Open **Quotes...** (*Koersen...*) vanuit het snelmenu. Elk geconfigureerd item vertegenwoordigt één
symbool en één webpagina. Dubbele symbolen zijn toegestaan en blijven onafhankelijk, omdat
elk item een eigen bron, selectors, verzamelopties en waarschuwingen heeft.

### Snel starten met het gepubliceerde voorbeeld

Als er geen items bestaan, biedt het Quotes-venster **Import sample quotes from GitHub** (*Voorbeeldkoersen importeren uit GitHub*) aan.
Hiermee wordt het voorbeeld uit de opslagplaats gedownload en worden de huidige toepassingsinstellingen vervangen.
Controleer elke geïmporteerde URL en de actuele voorwaarden van elke website voordat u deze gebruikt. U kunt
elk voorbeelditem daarna bewerken of verwijderen.

**Import Sample Quotes Config** (*Configuratie met voorbeeldkoersen importeren*) bovenaan de vensters Quotes en App Settings
doet op elk moment hetzelfde, na een bevestiging:

- SmartTicker vraagt **Are you sure?** (*Weet u het zeker?*) en waarschuwt dat de download uw bestaande
	koersen, koersgroepen, brongoedkeuringen, weergave, uiterlijk en overige app-instellingen vervangt.
	Waarschuwingsregels staan in een eigen bestand en worden niet verwijderd.
- **Export existing config...** (*Bestaande configuratie exporteren...*) is optioneel. Hiermee wordt uw huidige configuratie in een
	lokaal JSON-bestand opgeslagen en keert u vervolgens terug naar dezelfde bevestiging.
- **Import Sample Quotes Config** (*Configuratie met voorbeeldkoersen importeren*) downloadt het voorbeeld van internet en vervangt
	uw configuratie.
- **Cancel** (*Annuleren*) verandert niets.

### Een koers- of nieuwsitem toevoegen

1. Voer het label **Ticker** in, bijvoorbeeld `MSFT`. SmartTicker verwijdert omringende spaties en slaat het
	 op in hoofdletters.
2. Kies eventueel een bestaande **Group** (*Groep*) in de keuzelijst of typ een nieuwe naam, zoals
	 `Nasdaq`, `Precious Metals` of `Mag 7`. Laat het veld leeg voor **Ungrouped** (*Niet gegroepeerd*).
3. Selecteer een **Source**-voorinstelling (*Bron*).
4. Voer het **URL suffix** (*URL-achtervoegsel*) in, of een volledige URL wanneer u **Custom URL** (*Aangepaste URL*) gebruikt.
5. Selecteer **Price** (*Prijs*), **News** (*Nieuws*) of beide onder **Collect** (*Verzamelen*). Er is ten minste één keuze vereist.
6. Voer selectors handmatig in, gebruik de ontdekkingsknoppen of laat optionele selectors
	 leeg om ingebouwde detectie te gebruiken.
7. Selecteer **Validate URL** (*URL valideren*) om de normale prijs en/of koppen te testen.
8. Als SmartTicker om brongoedkeuring vraagt, controleert u de website en bevestigt u alleen wanneer
	 u gegevens ervan mag verzamelen.
9. Selecteer **Add independent entry** (*Onafhankelijk item toevoegen*). SmartTicker slaat het item op en vernieuwt de
	 ingeschakelde gegevens onmiddellijk.

### Koersen groeperen

Een groep is een benoemde verzameling die u definieert. Deze is niet aan een beurs of ingebouwde
categorie gekoppeld, zodat u items kunt indelen op markt, activatype, strategie, portefeuille,
regio of een ander schema. Namen worden bijgesneden, mogen Unicode gebruiken en mogen maximaal
80 tekens bevatten. Elke koers kan bij ten hoogste één groep horen.

Gebruik **Manage groups** (*Groepen beheren*) naast het veld Group of selecteer **Quote groups...** (*Koersgroepen...*) in het
snelmenu van de ticker. Het venster heeft drie werkgebieden:

- Voer links een **Group name** (*Groepsnaam*) in en kies vervolgens **Create** (*Maken*). Selecteer een bestaande groep,
	bewerk de naam en kies **Update** (*Bijwerken*), of kies **Delete** (*Verwijderen*). Lege groepen blijven behouden.
- Selecteer rechts een koers. De huidige groep wordt in de kolom **Current group** (*Huidige groep*)
	weergegeven; **Ungrouped** betekent dat deze geen koppeling heeft.
- Kies in het midden **Associate** (*Koppelen*) nadat u één groep en één koers hebt geselecteerd. Als die
	koers al bij een andere groep hoort, verplaatst SmartTicker deze naar de geselecteerde groep.
- Kies **Remove association** (*Koppeling verwijderen*) om alleen de geselecteerde koers terug te zetten naar **Ungrouped**.
- Als u een groep verwijdert, keren alle koersen ervan terug naar **Ungrouped**. Koersen, bronnen, huidige
	gegevens en waarschuwingen worden niet verwijderd.
- U kunt tijdens het toevoegen of bewerken van een koers ook een bestaande groep in de keuzelijst kiezen,
	of daar een nieuwe groepsnaam typen.
- Gebruik de knoppen omhoog/omlaag in Configured entries (*Geconfigureerde items*) om de groeps- en rijvolgorde in
	de statische tabel te bepalen.
- Sleep in de statische modus een tegelkop om volledige groepen rechtstreeks opnieuw te ordenen. Dezelfde
	volgorde wordt gebruikt door de afzonderlijke Quotes- en News-vensters.

Het gepubliceerde voorbeeld bevat zes voorbeeldgroepen, terwijl de statische modus standaard uitgeschakeld blijft.
Schakel na het importeren de statische weergave in om deze groepen als tabel te zien.

### Bronvoorinstellingen en URL's

| Bron | Wat u moet invoeren | Beleid dat SmartTicker toont |
| --- | --- | --- |
| **Yahoo Finance** | Een achtervoegsel na `https://finance.yahoo.com/`, bijvoorbeeld `quote/MSFT/`. | Schriftelijke toestemming vereist. De voorwaarden van Yahoo verbieden geautomatiseerde verzameling zonder voorafgaande toestemming. |
| **CNBC** | Een achtervoegsel na `https://www.cnbc.com/`. | Controleer het actuele beleid en de robots-richtlijnen van de website. |
| **Trading Economics** | Een achtervoegsel na `https://tradingeconomics.com/`. | Geef de voorkeur aan een gedocumenteerde API of geautoriseerde feed en controleer het actuele beleid van de website. |
| **Custom URL** (*Aangepaste URL*) | Een volledige openbare `http://`- of `https://`-pagina-URL. | Controleer de voorwaarden, het privacybeleid en de regels voor geautomatiseerde toegang van de website. |

Alleen absolute HTTP- en HTTPS-URL's worden geaccepteerd. URL's met ingesloten gebruikersnamen of
wachtwoorden worden geweigerd. Aanmelden in een browser geeft SmartTicker geen toestemming om een
pagina te verzamelen en SmartTicker gebruikt geen geverifieerde browsersessies.

De regel **Full URL** (*Volledige URL*) toont het uiteindelijke adres dat wordt samengesteld uit het vooringestelde voorvoegsel en uw
achtervoegsel. Controleer dit vóór validatie of ontdekking.

### Verzamelingopties

- **Price** (*Prijs*) vraagt de normale prijs op. Optionele selectors voor verandering, pre-market en after-hours
	worden op dezelfde gedownloade pagina geëvalueerd.
- **News** (*Nieuws*) vraagt koppelingen naar koppen op de pagina op.
- Als u beide selecteert, kan één item aan beide tickergebieden bijdragen.
- Beide uitschakelen is ongeldig.

### Naslag voor selectorvelden

Een CSS-selector identificeert een element in de statische HTML van een webpagina. Selectors zijn
optioneel, tenzij automatische detectie de gewenste waarde niet kan vinden.

| Veld | Waarde die SmartTicker extraheert |
| --- | --- |
| **Price selector** (*Prijsselector*) | Normale prijs of slotkoers. |
| **Price change** (*Prijswijziging*) | Procentuele wijziging tijdens de normale sessie. Als dit leeg is, wordt ingebouwde wijzigingsdetectie geprobeerd. |
| **Pre-market selector** (*Pre-marketselector*) | Pre-marketprijs, wanneer die sessie op de pagina bestaat. |
| **Pre-market change** (*Pre-marketwijziging*) | Procentuele pre-marketwijziging. |
| **After-hours selector** (*After-hoursselector*) | Post-market- of after-hoursprijs. |
| **After-hours change** (*After-hourswijziging*) | Procentuele post-market- of after-hourswijziging. |
| **News selector** (*Nieuwsselector*) | Koppelingen naar koppen. Selecteer een anker of een container waarvan de resultaten koppelingen bevatten. |

Pre-market- en after-hourswaarden vullen de normale prijs aan; ze vervangen deze niet.
Een pagina kan deze elementen buiten de bijbehorende marktsessie weglaten.

Voorbeelden van Yahoo Finance-selectors die door het gepubliceerde voorbeeld worden gebruikt:

```text
Price:                  [data-testid="qsp-price"]
Price change:           section.primary span[data-testid="qsp-price-change-percent"]
Pre-market price:       section.secondary span[data-testid="qsp-pre-price"]
Pre-market change:      section.secondary span[data-testid="qsp-pre-price-change-percent"]
After-hours price:      section.secondary span[data-testid="qsp-post-price"]
After-hours change:     section.secondary span[data-testid="qsp-post-price-change-percent"]
```

Websitemarkup verandert in de loop van de tijd. Beschouw voorbeelden als uitgangspunten, niet als permanente
contracten.

### Selectors ontdekken

Elk selectorveld heeft een bijbehorende knop **Discover** (*Ontdekken*).

1. Vul de bron-URL in en keur de website goed als goedkeuring vereist is.
2. Selecteer de ontdekkingsknop voor het exacte waardetype.
3. SmartTicker downloadt openbare statische HTML en toont mogelijke selectors met een voorbeeld-
	 waarde, betrouwbaarheidspercentage en reden in de knopinfo.
4. Selecteer **Use** (*Gebruiken*) naast een suggestie om deze naar het bijbehorende veld te kopiëren.
5. Valideer of observeer het resultaat voordat u erop vertrouwt.

Discovery voert geen JavaScript uit, meldt zich niet aan, omzeilt geen toegangscontroles en inspecteert uw
browser niet. Een waarde die alleen via JavaScript beschikbaar is, heeft mogelijk geen vindbare selector. Afzonderlijke ontdekkings-
typen voorkomen bewust dat pre-market- en after-hourswaarden worden gemengd.

### Een bron valideren

**Validate URL** (*URL valideren*) vraagt de pagina op en rapporteert de normale prijs en/of het aantal
koppen dat kan worden gelezen. U kunt dit veilig gebruiken voordat u een ticker invoert, omdat SmartTicker
een tijdelijk label voor de test gebruikt.

Deze validatie controleert momenteel niet de vier selectorvelden voor pre-market en after-hours.
Gebruik de voorbeeldwaarden uit Discovery en bevestig daarna de weergegeven sessiegegevens.

Veelvoorkomende fouten zijn een HTTP-fout, time-out, ontbrekende waarde, nul koppen, niet-goedgekeurde bron-
toestemming, inhoud die alleen via JavaScript beschikbaar is of een verouderde selector.

### Limiet voor nieuwsherhaling

**Show max _N_ times** (*Maximaal _N_ keer tonen*) accepteert 1 tot en met 100 en is standaard 5. SmartTicker telt één
weergave voor elke voltooide News-vernieuwingscyclus waarin dezelfde koptitel wordt
geretourneerd. Zodra de titel in het geconfigureerde aantal cycli is verschenen, wordt deze voor
de rest van de huidige toepassingssessie verwijderd. Door dat item te bewerken of verwijderen wordt
de herhalingsgeschiedenis gewist.

### Items bewerken, ordenen en verwijderen

De lijst **Configured entries** (*Geconfigureerde items*) toont symbool, groep, bron, URL, verzamelbadges,
selector voor de normale prijs, nieuwsselector en limiet voor nieuwsherhaling.

- **Edit** (*Bewerken*) laadt het item in het formulier. Selecteer **Save changes** (*Wijzigingen opslaan*) om het toe te passen of
	**Cancel edit** (*Bewerken annuleren*) om formulierwijzigingen te negeren.
- De pijlknoppen omhoog en omlaag wijzigen de tickervolgorde en slaan deze onmiddellijk op.
- **Remove** (*Verwijderen*) verwijdert het item en de momenteel weergegeven gegevens.
- Als waarschuwingsregels op het item zijn gericht, vraagt SmartTicker of die regels moeten worden verwijderd. Een
	waarschuwing zonder overeenkomende geconfigureerde koers kan niet worden geactiveerd.
- Als u een item hernoemt, worden de weergegeven symbolen van waarschuwingsregels die aan dat item zijn gekoppeld bijgewerkt.

## App-instellingen

Open **App Settings...** (*App-instellingen...*) vanuit het snelmenu. Wijzigingen worden van kracht en automatisch
opgeslagen; er is geen knop Apply (*Toepassen*).

### Tickerrijen en snelheid

| Instelling | Keuzes | Standaard | Effect |
| --- | --- | --- | --- |
| Prijsrijen | 1 tot en met 8 | 1 | Aantal parallelle prijzenmarqueerijen. |
| Scrollsnelheid prijzen | 20, 30, 40, 50, 65, 80, 100 of 120 px/sec | 50 | Snelheid van de prijzenmarquee. |
| Nieuwsrijen | 1 tot en met 8 | 1 | Aantal parallelle koppenmarqueerijen. |
| Scrollsnelheid nieuws | 20, 30, 40, 50, 65, 80, 100 of 120 px/sec | 40 | Snelheid van de nieuwsmarquee. |
| Scrollende tekengrootte | 9 tot en met 24 pt | 14 pt | Prijs- en News-tekst in scrollende rijen. |
| Statische tekengrootte | 9 tot en met 24 pt | 13 pt | Koers- en koptekst in statische rijen. |
| Prijzen vernieuwen | 30 tot en met 300 seconden, in stappen van 15 seconden | 60 seconden | Tijd waarin elk toegestaan prijsitem één geplande vernieuwing ontvangt. |
| Nieuws vernieuwen | 30 tot en met 300 seconden, in stappen van 15 seconden | 300 seconden | Tijd waarin elk toegestaan News-item één geplande vernieuwing ontvangt. |

Prijsrijen en de scrollsnelheid voor prijzen zijn uitgeschakeld wanneer statische gegroepeerde tabellen actief zijn,
omdat die modus alle prijsitems weergeeft en geen van beide vensters automatisch scrolt.
Instellingen voor nieuwsrijen en -snelheid blijven bewaard voor de scrollende weergave.

Prijs- en News-aanvragen worden onafhankelijk over tijdsloten van één seconde verdeeld voor hun
volledige intervallen, in plaats van tegelijk te starten. Zo worden 60 items over 30 seconden
gepland als twee items per seconde; vijf items over 30 seconden als ongeveer één item
per zes seconden. Er worden maximaal vier bronaanvragen tegelijk uitgevoerd, dubbel werk voor hetzelfde
item en dezelfde stream wordt overgeslagen en gemiste tijdsloten worden niet in een piek ingehaald. **Refresh
prices now** (*Prijzen nu vernieuwen*) of **Refresh news now** (*Nieuws nu vernieuwen*) start alleen die stream opnieuw en vraagt het eerste tijdslot aan.
Bestaande geslaagde prijzen en koppen blijven zichtbaar terwijl vervangende gegevens worden gelezen.

Elke HTTP-aanvraag heeft een vaste time-out van 20 seconden. Een trage bron houdt de UI-
dispatcher niet vast en verhindert niet dat latere tijdsloten de resterende aanvraagcapaciteit gebruiken. SmartTicker
rapporteert fouten zoals HTTP 403 en 429 en omzeilt beperkingen niet. Het parseert of handhaaft robots-richtlijnen,
crawl-delay-waarden of back-offinstructies van de server niet automatisch. Kies daarom conforme bronnen en vermijd
onnodig frequente aanvragen.

### Vensterformaten

App Settings bewaart drie onafhankelijke formaatparen:

| Venster | Breedte | Hoogte | Standaard |
| --- | --- | --- | --- |
| Scrollende weergave | 420–7680 px | 50–900 px | 980 × 64 px |
| Statische Prices-weergave | 420–7680 px | 420–4320 px | 980 × 420 px |
| Statische News-weergave | 420–7680 px | 240–4320 px | 680 × 340 px |

Een gewijzigde waarde wordt onmiddellijk toegepast wanneer dat venster of die weergave actief is. Het gepubliceerde
voorbeeld demonstreert 1200 × 96 voor scrollen, 1200 × 720 voor statische Prices en 760 × 480 voor statische
News, met scrollende tekst van 15 punten en statische tekst van 14 punten. Een scrollhoogte onder de
ruimte die de ingeschakelde rijen vereisen, wordt automatisch tot het vereiste minimum verhoogd.

Gebruik de vier keuzes onder **View** (*Weergave*) om te kiezen of News wordt weergegeven en of de
indeling scrolt of statisch blijft. Als u de weergave wijzigt, worden geconfigureerde items nooit verwijderd.

### SmartTicker starten bij aanmelden

Schakel **Start SmartTicker when I sign in** (*SmartTicker starten wanneer ik me aanmeld*) in om het geïnstalleerde uitvoerbare bestand alleen voor
de huidige gebruiker te registreren.

- In Windows gebruikt SmartTicker de registersleutel `Run` van de huidige gebruiker.
- Op Linux-desktops die de freedesktop-autostartconventie ondersteunen, schrijft SmartTicker
	`smartticker.desktop` in de autostartmap van de gebruiker.
- De optie is uitgeschakeld op platforms waarvoor SmartTicker geen ondersteund registratie-
	mechanisme heeft.

Het besturingssysteem is leidend. Als het opstartgedrag buiten SmartTicker wordt gewijzigd, geeft het
selectievakje de OS-status weer wanneer de instellingen de volgende keer worden geladen.

### Websitetoegang

**Allow website cookies and cross-host redirects** (*Websitecookies en omleidingen tussen hosts toestaan*) is standaard uitgeschakeld.

Wanneer uitgeschakeld:

- SmartTicker vereist één expliciete goedkeuring voor elke websitehost voordat deze wordt aangevraagd.
- Websitecookies worden niet geaccepteerd.
- Omleidingen naar een andere host worden geblokkeerd.
- Goedgekeurde hosts worden in lokale instellingen onthouden.

Wanneer ingeschakeld:

- SmartTicker slaat de goedkeuringsstap per host over.
- Cookies die door opgevraagde websites worden ingesteld, worden alleen in een geïsoleerde container in het geheugen
	bewaard en verdwijnen wanneer SmartTicker wordt afgesloten.
- Omleidingen naar andere hosts mogen worden gevolgd.
- SmartTicker leest nog steeds geen browsercookies, verzendt geen referenties en verzendt geen
	aanmeldingsformulieren.

Als u deze optie uitschakelt, worden de momenteel weergegeven gegevens van niet-goedgekeurde bronnen verwijderd
totdat die hosts zijn goedgekeurd en vernieuwd.

#### Privacykeuzes van websites

Als een antwoord wordt herkend als een privacy-/cookieformulier met zowel positieve als
negatieve keuzes, pauzeert SmartTicker en toont het de paginatitel, aangevraagde URL,
toestemmings-URL, samenvatting van het formulier en de Accept/Reject-labels van de website.

- **Accept** (*Accepteren*) verzendt de verborgen velden die door dat formulier zijn geleverd plus de exacte Accept-
	bediening die u hebt geselecteerd.
- **Reject** (*Weigeren*) verzendt die verborgen velden plus de exacte Reject-bediening die u hebt geselecteerd.
- **Cancel** (*Annuleren*) verzendt niets.

Dit is een privacykeuze van een website, niet de brontoestemmingsgoedkeuring van SmartTicker.

#### Alle bronnen valideren

Selecteer **Validate all sources** (*Alle bronnen valideren*) om elk geconfigureerd item te controleren en testen.

1. Als websitetoegang beperkt is, groepeert SmartTicker niet-goedgekeurde items op hostnaam
	 en toont het één dialoogvenster voor broncontrole per host.
2. Controleer de host, beleidssamenvatting, richtlijnen, bronnamen en symbolen.
3. Schakel de bevestiging alleen in als u de website hebt gecontroleerd en deze mag gebruiken.
4. Kies **Approve this source** (*Deze bron goedkeuren*), **Skip this source** (*Deze bron overslaan*) of **Cancel validation** (*Validatie annuleren*).
5. SmartTicker test elk toegestaan item en rapporteert de totalen geslaagd, mislukt en overgeslagen.
	 Afzonderlijke problemen verschijnen onder de statusregel.

Goedkeuringsrecords leggen toestemming vast binnen SmartTicker; ze verlenen geen wettelijke rechten en
stellen de voorwaarden van de website niet buiten werking.

### Uiterlijk

**Window transparency** (*Venstertransparantie*) wijzigt alleen de achtergrond van de ticker. Tekst blijft ondoorzichtig. Het
bereik is 20% tot en met 100%, in stappen van 5%, en de standaardwaarde is 100%.

Kleurvelden accepteren hexadecimale waarden in de vorm `#RRGGBB` en bieden ook een kleurkiezer.

| Kleur | Standaard | Gebruikt voor |
| --- | --- | --- |
| Achtergrond | `#10151D` | Tickerachtergrond voordat transparantie wordt toegepast. |
| Koersnaam | `#79C0FF` | Symbool-/bronlabel. |
| Slotkoers | `#FFA657` | Normale prijs. |
| Buiten handelsuren | `#00E5FF` | Pre-market- en after-hoursprijzen. |
| Nieuws 1e | `#FFFFFF` | Koppen 1, 5, 9 enzovoort. |
| Nieuws 2e | `#00E5FF` | Koppen 2, 6, 10 enzovoort. |
| Nieuws 3e | `#A3E635` | Koppen 3, 7, 11 enzovoort. |
| Nieuws 4e | `#79C0FF` | Koppen 4, 8, 12 enzovoort. |
| Stijging | `#3FB950` | Positieve procentuele wijzigingen. |
| Daling | `#F85149` | Negatieve procentuele wijzigingen. |
| Waarschuwing knipperen | `#FF00FF` | Geactiveerde prijswaarschuwingen, afgewisseld met zwart. |

**Reset to defaults** (*Standaardwaarden herstellen*) herstelt elke bovenstaande kleur en een achtergronddekking van 100%. Hiermee worden
rijen, snelheden, tekengrootten, vensterformaten, bronnen, vernieuwingsintervallen, waarschuwingen of
taal niet opnieuw ingesteld.

### Back-up maken en herstellen

SmartTicker bewaart toepassingsinstellingen en waarschuwingsregels in afzonderlijke JSON-bestanden en
biedt afzonderlijke knoppen voor elk back-uptype.

#### Instellingen exporteren en importeren

- **Export settings...** (*Instellingen exporteren...*) schrijft geconfigureerde items, groepstoewijzingen, groepsdefinities,
	verborgen nieuwskoersen, itemvolgorde, selectors, de keuze voor scrollende/statische koersenweergave,
	goedgekeurde hosts, regelzichtbaarheid, rijen, snelheden, scrollende/statische tekengrootten, alle drie
	vensterformaatparen, vernieuwingsintervallen, opstartvoorkeur,
	optie voor websitetoegang, kleuren inclusief de knipperkleur voor waarschuwingen, transparantie en
	taal.
- **Import settings...** (*Instellingen importeren...*) valideert het volledige bestand voordat iets wordt gewijzigd. Bij een geweigerd
	bestand blijven de huidige instellingen ongewijzigd.
- Een geslaagde import vervangt elk geconfigureerd item en elke toepassingsvoorkeur. De
	afzonderlijke waarschuwingsregels worden niet vervangen.
- Groepen zijn in het instellingenbestand opgenomen als koerskoppelingen, naast de groeps-
	definities zelf, zodat een groep zonder koersen ook in een back-up blijft bestaan. Er is geen
	afzonderlijk export- of importbestand dat alleen groepen bevat.
- De opstartvoorkeur staat in een instellingenback-up, maar importeren verandert de OS-
	opstartregistratie niet ongemerkt. Het besturingssysteem blijft leidend;
	gebruik het selectievakje Startup (*Opstarten*) om de registratie op de huidige computer te wijzigen.
- Importbestanden zijn beperkt tot 1 MiB, schemaversie 1 en maximaal 200 abonnementen.
	Onbekende eigenschappen, dubbele ID's, onjuist gevormde URL's, ongeldige kleuren, ongeldige bereiken
	of niet-ondersteunde taalcodes worden geweigerd in plaats van stilzwijgend genegeerd.

#### Waarschuwingsregels exporteren en importeren- **Export alert rules...** (*Waarschuwingsregels exporteren...*) schrijft alle regels plus Buzz, het aantal buzzes en de knipperduur.
- **Import alert rules...** (*Waarschuwingsregels importeren...*) valideert het volledige bestand en vervangt vervolgens alle huidige regels
	en instellingen voor het activeren van waarschuwingen.
- Regels worden eerst opnieuw gekoppeld op abonnements-ID. Wanneer ID's verschillen, probeert SmartTicker een
	hoofdletterongevoelige symboolovereenkomst.
- Een geïmporteerde regel zonder overeenkomende koers blijft behouden, maar kan niet worden geactiveerd. De import-
	status rapporteert hoeveel regels opnieuw zijn gekoppeld of niet gekoppeld blijven.
- Importbestanden voor waarschuwingen zijn beperkt tot 1 MiB.

Voor overdracht naar een andere computer importeert u eerst de toepassingsinstellingen en daarna de waarschuwingsregels.
Door waarschuwingen als tweede te importeren, kunnen regels op symbool opnieuw aan de nieuwe abonnements-ID's worden gekoppeld.

### De configuratiebestanden ter plaatse bewerken

**Edit Current App Config** (*Huidige app-configuratie bewerken*) en **Edit Current Alert Rules** (*Huidige waarschuwingsregels bewerken*) in App Settings openen het
actieve JSON-bestand in de teksteditor die uw systeem aan `.json` heeft gekoppeld. Dit is voor
gevorderde gebruikers; de vensters in SmartTicker omvatten dezelfde instellingen zonder dit risico.

Beide knoppen tonen eerst een bevestiging waarin u wordt gevraagd het huidige bestand te exporteren. Maak
die export: handmatig bewerken kan het bestand beschadigen en er is geen functie voor ongedaan maken.

- **Export existing config...** (*Bestaande configuratie exporteren...*) slaat het huidige bestand op en keert vervolgens terug naar dezelfde vraag.
- **Open in text editor** (*In teksteditor openen*) opent het actieve bestand.
- **Cancel** (*Annuleren*) verandert niets.

SmartTicker bewaakt het bestand en laadt het opnieuw zodra uw editor het opslaat:

- Een geldig bestand wordt onmiddellijk toegepast en de ticker wordt zonder herstart bijgewerkt.
- Onjuist gevormde JSON, een schemaovertreding of een andere validatiefout wordt geweigerd. Uw
	actieve configuratie blijft onaangetast en het venster App Settings rapporteert het
	probleem.
- Corrigeer na een geweigerde bewerking het bestand of herstel een geldige export met
	**Import settings...** (*Instellingen importeren...*) of **Import alert rules...** (*Waarschuwingsregels importeren...*).
- Een bestand dat door een ander programma vergrendeld blijft, wordt kort opnieuw geprobeerd en daarna gerapporteerd.

Het bewerken van het waarschuwingsregelsbestand volgt dezelfde regels en heeft geen invloed op toepassings-
instellingen, omdat de twee bestanden gescheiden zijn.

## Waarschuwingsregels

Open **Alerts** (*Waarschuwingen*) vanuit het snelmenu. Regels worden na elke geslaagde
prijsvernieuwing geëvalueerd en bewaken alleen de normale prijs, niet pre-market- of after-hourswaarden.

### Een regel maken

1. Selecteer een geconfigureerde **Quote** (*Koers*). Items met hetzelfde symbool blijven afzonderlijk.
2. Selecteer een **Condition** (*Voorwaarde*) en voer een numerieke drempel in met een invariant decimaalteken, zoals
	 `250.50`.
3. Kies eventueel **Active from** (*Actief vanaf*). Laat dit leeg om onmiddellijk te activeren.
4. Laat **Never expires** (*Verloopt nooit*) ingeschakeld, of schakel het uit en kies een vervaldatum.
5. Selecteer **Add rule** (*Regel toevoegen*).

De beschikbare vergelijkingen zijn:

| Keuze | Betekenis |
| --- | --- |
| `LessThan` | Prijs `<` drempel. |
| `LessThanOrEqual` | Prijs `<=` drempel. |
| `GreaterThan` | Prijs `>` drempel. |
| `GreaterThanOrEqual` | Prijs `>=` drempel. |
| `EqualTo` | Prijs is exact gelijk aan de drempel. |
| `NotEqualTo` | Prijs wijkt af van de drempel. |

De startgrens is inclusief. De vervalgrens is eveneens inclusief; nadat deze is
verstreken, wordt de regel niet meer geactiveerd. SmartTicker weigert een vervaldatum vóór de startdatum.

### Wanneer een regel wordt geactiveerd

Een ingeschakelde, geplande regel wordt één keer geactiveerd wanneer de voorwaarde van onwaar naar waar verandert.
De regel meldt niet bij elke vernieuwing terwijl de voorwaarde waar blijft. Nadat de prijs
de voorwaarde verlaat, wordt de regel opnieuw geactiveerd en kan deze afgaan wanneer de prijs er opnieuw aan voldoet.

Door een regel te bewerken of uit en weer in te schakelen, wordt deze ook opnieuw geactiveerd. Daardoor kan een ingeschakelde
regel onmiddellijk afgaan als de meest recente normale prijs al aan de
voorwaarde voldoet. Een mislukte of ontbrekende prijs kan geen regel activeren.

Wanneer een of meer regels worden geactiveerd:

- Het betrokken prijsitem wisselt gedurende de geconfigureerde duur tussen de geconfigureerde knipperkleur voor waarschuwingen en zwart.
	De standaardknipperkleur is magenta (`#FF00FF`).
- Als **Buzz** is ingeschakeld, speelt SmartTicker de geconfigureerde buzzreeks af.
- Het waarschuwingsbericht identificeert één regel of meldt het aantal regels dat tegelijk is geactiveerd.
- De ticker blijft scrollen terwijl de waarschuwingsmarkering actief is.

### Instellingen voor waarschuwingsuitvoer

| Instelling | Bereik | Standaard |
| --- | --- | --- |
| **Buzz** | Aan of uit | Aan |
| Aantal buzzes | 1 tot en met 20 | 15 |
| **Blink for** (*Knipperen gedurende*) | 5 tot en met 900 seconden, in stappen van 15 seconden | 60 seconden |

Als u Buzz uitschakelt, blijft de visuele waarschuwing actief. Als meerdere regels in dezelfde
evaluatie worden geactiveerd, start SmartTicker één geconfigureerde buzzreeks voor die evaluatie.
Wijzig **Alert blink** (*Waarschuwing knipperen*) onder **App Settings > Appearance** (*App-instellingen > Uiterlijk*). Dit is een toepassings-
voorkeur voor het uiterlijk, zodat export/import van Settings deze bevat in plaats van het afzonderlijke
waarschuwingsregelsbestand.

### Geconfigureerde regels beheren

- **Edit** (*Bewerken*) laadt een regel in het formulier. Selecteer **Update rule** (*Regel bijwerken*) om op te slaan of **Cancel** (*Annuleren*) om
	de regel ongewijzigd te laten.
- **Disable** (*Uitschakelen*) behoudt de regel, maar voorkomt dat deze overeenkomt. **Enable** (*Inschakelen*) activeert de regel opnieuw en
	evalueert deze tegen de meest recente normale prijs.
- **Remove** (*Verwijderen*) verwijdert de regel.
- De lijst toont de ingeschakelde status, het symbool, een samenvatting van de voorwaarde en het schema.

Wijzigingen in waarschuwingsregels en instellingen voor waarschuwingsuitvoer worden automatisch opgeslagen.

## Lokale bestanden en privacy

SmartTicker slaat de configuratie lokaal op en synchroniseert deze niet met een ontwikkelaars-
service.

In Windows zijn de standaardbestanden:

```text
%LocalAppData%\SmartTicker\settings.json
%LocalAppData%\SmartTicker\alerts.json
```

In Linux gebruikt .NET de lokale map met toepassingsgegevens van de huidige gebruiker, normaal:

```text
~/.local/share/SmartTicker/settings.json
~/.local/share/SmartTicker/alerts.json
```

### Een geïsoleerde gegevensmap gebruiken

Bij geavanceerde diagnostiek en testruns kan `SMARTTICKER_DATA_DIRECTORY` vóór het starten van
SmartTicker worden ingesteld. Wanneer de waarde niet leeg is, worden beide bestanden rechtstreeks in die opgeloste
map geplaatst als `settings.json` en `alerts.json`; de bovenstaande platformstandaarden worden voor dat proces niet
gebruikt. Geef de voorkeur aan een absoluut pad en zorg dat het beschrijfbaar is.

PowerShell-voorbeeld:

```powershell
$env:SMARTTICKER_DATA_DIRECTORY = 'D:\SmartTicker-Profile'
& 'C:\Program Files\SmartTicker\SmartTicker.Desktop.exe'
```

Voorbeeld voor Linux-shell:

```bash
SMARTTICKER_DATA_DIRECTORY="$HOME/.local/share/SmartTicker-Test" smartticker
```

Stel de variabele vóór het starten van het proces in. SmartTicker kopieert het standaardprofiel niet
naar de geselecteerde map, zodat een lege map met een lege configuratie begint.
Instanties die naar dezelfde map verwijzen, kunnen elkaars opgeslagen bewerkingen waarnemen. Gebruik de
normale export-/importopdrachten voor Settings en Alert Rules voor back-ups en profieloverdracht.

Het Alerts-venster toont het exacte pad van het gebruikte waarschuwingsbestand. Bij schrijven wordt een tijdelijk
bestand gebruikt, gevolgd door vervanging, zodat een gedeeltelijk geschreven bestand niet als huidige
configuratie wordt behandeld.

SmartTicker heeft geen account, telemetrie, analyse, reclame of cloudsynchronisatie. Een bron-
website ontvangt normale netwerkinformatie, zoals uw IP-adres, wanneer SmartTicker
die bron opvraagt. Bij het openen van Help wordt de onbewerkte handleiding van GitHub opgevraagd. Lees voor volledige
details `PRIVACY.md` in de opslagplaats.

U bent er verantwoordelijk voor dat elke bron-URL en selector wordt gebruikt in
overeenstemming met de voorwaarden, licentie, robots-richtlijnen en toepasselijke wetgeving van de website.

## Problemen oplossen

### Een koers toont niet beschikbaar of geen prijs

Een bronaanvraag verloopt na 20 seconden. Als die koers een eerdere geslaagde
momentopname heeft, blijft deze bij een mislukte vernieuwing zichtbaar; anders toont de koers **Unavailable** (*Niet beschikbaar*)
totdat een latere vernieuwing slaagt. Lees de validatie- of vernieuwingsfout voordat u
selectors wijzigt.

1. Open **Quotes...** (*Koersen...*), bewerk het item en controleer Full URL (*Volledige URL*).
2. Controleer of **Price** (*Prijs*) is geselecteerd.
3. Keur de website goed als daarom wordt gevraagd.
4. Selecteer **Validate URL** (*URL valideren*) en lees het exacte resultaat.
5. Voer **Discover price** (*Prijs ontdekken*) uit of inspecteer de statische HTML van de pagina en werk de selector bij.
6. Controleer of de pagina JavaScript, verificatie of toestemming vereist die
	 SmartTicker niet veilig kan afhandelen.
7. Respecteer HTTP 403, 429, robots-beperkingen en het beleid voor geautomatiseerde toegang van de website.

### Pre-market- of after-hoursgegevens ontbreken

- De bijbehorende marktsessie is mogelijk niet actief.
- De pagina kan het sessie-element weglaten wanneer er geen sessiewaarde bestaat.
- Controleer of pre-marketselectors op pre-marketelementen zijn gericht en after-hoursselectors
	op post-marketelementen.
- Voer de bijbehorende ontdekkingsopdracht opnieuw uit, omdat de websitemarkup mogelijk is gewijzigd.

### Nieuws is leeg

- Controleer of **News** (*Nieuws*) is geselecteerd.
- Valideer de bron en voer **Discover news** (*Nieuws ontdekken*) uit.
- Zorg dat de selector koppelingen met zichtbare koptekst retourneert.
- Bij een mislukte of verlopen News-aanvraag blijven eerdere geslaagde koppen waar mogelijk behouden.
	Een bron zonder geslaagd resultaat blijft leeg totdat een later tijdslot slaagt.
- Een kop verdwijnt nadat deze de geconfigureerde herhalingslimiet voor deze sessie heeft bereikt.
- Controleer in statische News of de bedoelde koers is ingeschakeld onder **Show news for** (*Nieuws tonen voor*).

### Selectorontdekking vindt niets

Discovery leest alleen de gedownloade statische HTML. Het kan geen waarden zien die later door
JavaScript op de pagina worden gemaakt. Voer handmatig een geverifieerde selector in, kies een statische pagina/feed of gebruik
een geautoriseerde gedocumenteerde API via een compatibele openbare pagina.

### Een waarschuwing wordt niet geactiveerd

- Controleer of de gekoppelde koers nog bestaat, Price verzamelt en een geslaagde normale
	prijs heeft.
- Controleer of de regel Enabled (*Ingeschakeld*) is en binnen het start-/vervalschema valt.
- Controleer de vergelijking en drempel. `EqualTo` vereist exacte decimale gelijkheid.
- Onthoud dat een voortdurend ware voorwaarde één keer wordt geactiveerd; deze moet onwaar worden voordat
	de regel opnieuw kan worden geactiveerd, tenzij u de regel bewerkt of opnieuw inschakelt.
- Pre-market- en after-hoursprijzen sturen waarschuwingsregels niet aan.

### SmartTicker kan niet worden verplaatst of vergroot/verkleind

- Verplaats alleen met de greep met verticale stippen in de linkerstrook.
- Wijzig het formaat vanaf een rand of hoek; gebruik de zichtbare markering rechtsonder als een rand moeilijk
	te vinden is.
- Tickerinhoud is opzettelijk geen verplaatsingsvlak.

### Statische groepen of waarden zijn niet zoals verwacht

- Open **Quotes...** (*Koersen...*) en controleer de Group-waarde van elk item.
- Open **Quote groups...** (*Koersgroepen...*) om groepsdefinities te beheren en de huidige
	koppeling van elke koers te controleren.
- Items met een lege Group verschijnen onder **Ungrouped** (*Niet gegroepeerd*).
- **Chg** wordt berekend uit Last en Chg%; het wordt niet onafhankelijk van de
	pagina geëxtraheerd. Het blijft `—` wanneer het percentage niet beschikbaar is.
- Orden items opnieuw met de knoppen omhoog/omlaag om de groeps- en rijvolgorde te wijzigen.
- Sleep de gestippelde greep op een tegelkop om de hele groep te verplaatsen. Zet deze op de linker-
  helft van een andere tegel neer om de groep ervoor te plaatsen, of op de rechterhelft om deze erna te plaatsen.
- Selecteer **Refresh prices now** (*Prijzen nu vernieuwen*) terwijl SmartTicker niet is gepauzeerd om de tabel bij te werken.

### Help-tekst is niet opgemaakt of navigatie werkt niet

- Het Help-venster hoort opgemaakte koppen, alinea's, lijsten, tabellen, koppelingen
	en codeblokken te tonen in plaats van Markdown-leestekens.
- Gebruik **On this page** (*Op deze pagina*) links om naar een hoofdsectie te springen. Koppelingen in de tabel voor snelle
	navigatie scrollen ook binnen het document.
- Sluit Help en open het opnieuw, of wijzig **Language** (*Taal*), om de bijbehorende gepubliceerde
	handleiding aan te vragen. Totdat deze is opgehaald, toont SmartTicker de opgemaakte handleiding
	die in de geïnstalleerde toepassing is ingebouwd.

### Online Help is niet beschikbaar of verouderd

- Sluit Help en open het opnieuw om de gepubliceerde handleiding opnieuw aan te vragen.
- Open het onbewerkte GitHub-adres dat aan het begin van deze handleiding staat in een browser om
	het gepubliceerde bestand rechtstreeks te inspecteren.
- SmartTicker gebruikt de opgenomen handleiding wanneer de aanvraag mislukt of een leeg bestand retourneert.
- Online wijzigingen verschijnen pas nadat `HELPME.md` of het bijbehorende gelokaliseerde bestand
  `help/HELPME.<language-code>.md` op de `main`-tak van de opslagplaats is gepubliceerd.

## Ondersteuning

Meld reproduceerbare problemen op:

<https://github.com/bulentozkir/smartticker/issues>

Vermeld de SmartTicker-versie, het besturingssysteem, de hostnaam van de bron, de validatiestatus
en de exacte fouttekst. Verwijder privé-URL's of andere gevoelige informatie voordat u iets plaatst.