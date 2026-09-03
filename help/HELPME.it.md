# Guida di SmartTicker

Questa guida si applica a SmartTicker 1.0.3. Illustra il ticker principale, le Impostazioni
dell'app (App Settings), le Quotazioni (Quotes), le regole di avviso, le autorizzazioni per i
siti web, i backup e i problemi comuni.

SmartTicker legge HTML statico pubblico dalle pagine web configurate dall'utente. Non
fornisce un feed di dati di mercato e le informazioni estratte possono essere in ritardo,
incomplete o errate. Verificare le informazioni finanziarie importanti con una fonte autorevole.

## Navigazione rapida

| Area | Vai a |
| --- | --- |
| Per iniziare | [Aprire la Guida e le finestre di configurazione](#aprire-la-guida-e-le-finestre-di-configurazione) |
| Ticker principale | [Controlli](#controlli-del-ticker-principale) · [Visualizzazione scorrevole o statica](#scegliere-la-visualizzazione-scorrevole-o-statica-delle-quotazioni) · [Spostare](#spostare-il-ticker) · [Ridimensionare](#ridimensionare-il-ticker) · [Pausa](#mettere-in-pausa-e-riprendere) · [Riferimento del menu](#riferimento-del-menu-principale) |
| Quotazioni e notizie | [Quotazioni](#quotazioni) · [Aggiungere una voce](#aggiungere-una-quotazione-o-una-notizia) · [Raggruppare le quotazioni](#raggruppare-le-quotazioni) · [URL delle fonti](#preimpostazioni-delle-fonti-e-url) · [Selettori](#riferimento-dei-campi-selettore) · [Individuazione](#individuare-i-selettori) · [Convalida](#convalidare-una-fonte) |
| Preferenze dell'applicazione | [Impostazioni dell'app](#impostazioni-dellapp) · [Righe e velocità](#righe-e-velocità-del-ticker) · [Avvio](#avviare-smartticker-allaccesso) · [Accesso ai siti web](#accesso-ai-siti-web) · [Aspetto](#aspetto) · [Backup e ripristino](#backup-e-ripristino) · [Modificare i file di configurazione](#modificare-direttamente-i-file-di-configurazione) |
| Avvisi di prezzo | [Regole di avviso](#regole-di-avviso) · [Creare una regola](#creare-una-regola) · [Comportamento all'attivazione](#quando-si-attiva-una-regola) · [Output degli avvisi](#impostazioni-di-output-degli-avvisi) · [Gestire le regole](#gestire-le-regole-configurate) |
| Dati e supporto | [File locali e privacy](#file-locali-e-privacy) · [Risoluzione dei problemi](#risoluzione-dei-problemi) · [Supporto](#supporto) |

## Aprire la Guida e le finestre di configurazione

Fare clic con il pulsante destro del mouse sul ticker per aprirne il menu. I principali
comandi di configurazione sono:

- **Quotazioni... (Quotes...)**: aggiungere, verificare, modificare, ordinare e rimuovere
  fonti di quotazioni o notizie.
- **Gruppi di quotazioni... (Quote groups...)**: creare, aggiornare o eliminare gruppi e
  associarvi le quotazioni.
- **Avvisi (Alerts)**: creare e gestire le regole di avviso sui prezzi.
- **Impostazioni dell'app... (App Settings...)**: configurare righe, velocità, intervalli di
	aggiornamento, avvio, accesso ai siti web, colori, trasparenza e backup.
- **Visualizza (View)**: selezionare una delle quattro combinazioni che si escludono a
  vicenda: scorrevole o statica, con soli Prezzi (Prices) o Prezzi con Notizie (News).
- **Guida (Help)**: aprire questa guida all'interno di SmartTicker.
- **Informazioni su SmartTicker (About SmartTicker)**: mostrare la versione installata e
  l'avviso di licenza.
- **Esci (Exit)**: chiudere completamente SmartTicker.

La finestra Guida formatta immediatamente la guida incorporata per la lingua dell'app
selezionata, quindi controlla la guida online corrispondente ogni volta che si apre la
Guida o si cambia **Lingua (Language)**. La guida online in italiano è:

<https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/help/HELPME.it.md>

Le guide tradotte usano `help/HELPME.<language-code>.md` nello stesso repository. Se non
è possibile scaricare il documento online, SmartTicker continua a visualizzare la
traduzione corrispondente incorporata nell'applicazione installata. Cambiando
**Lingua (Language)**, il titolo, lo stato, la navigazione e l'intera guida nella finestra
Guida già aperta si aggiornano immediatamente. Chiudere la Guida con il normale controllo
di chiusura della barra del titolo.

## Controlli del ticker principale

### Scegliere la visualizzazione scorrevole o statica delle quotazioni

SmartTicker offre quattro modalità di visualizzazione che si escludono a vicenda. Fare
clic con il pulsante destro del mouse sul ticker, aprire **Visualizza (View)** e selezionarne
una. Il layout cambia immediatamente e la scelta viene salvata.

| Opzione di visualizzazione | Risultato |
| --- | --- |
| **Scorrimento da sinistra a destra: solo Prezzi (Left-to-right scroll: Prices only)** | Testo scorrevole dei prezzi nel ticker principale; nessuna visualizzazione delle notizie. Questa è l'impostazione predefinita. |
| **Scorrimento da sinistra a destra: Prezzi con Notizie (Left-to-right scroll: Prices with News)** | Testi scorrevoli di prezzi e notizie nel ticker principale. |
| **Visualizzazione statica: solo Prezzi (Static view: Prices only)** | Riquadri reattivi dei prezzi nella finestra principale; nessuna finestra Notizie. |
| **Visualizzazione statica: Prezzi con Notizie (Static view: Prices with News)** | Riquadri reattivi dei prezzi più una finestra statica separata **SmartTicker News**. |

I file di impostazioni creati prima dell'aggiunta di queste opzioni vengono associati alla
combinazione corrispondente delle impostazioni scorrevole/statica e notizie salvate. La
modalità di visualizzazione si gestisce solo dal menu **Visualizza (View)** accessibile con
il pulsante destro del mouse sul ticker.

- In entrambe le modalità scorrevoli, i prezzi usano il testo scorrevole orizzontale e il
  numero di righe dei prezzi e la velocità di scorrimento configurati.
- In entrambe le modalità statiche, i gruppi appaiono come riquadri reattivi disposti da
  sinistra a destra. I riquadri vanno a capo su un'altra riga solo quando la finestra è
  troppo stretta. I prezzi non si spostano automaticamente.
- Ogni riquadro di quotazione ha colonne allineate dedicate a **Simbolo (Symbol)**,
  **Ultimo (Last)**, **Var. (Chg)** e **Var.% (Chg%)**. **Var. (Chg)** viene ricavata da
  Last e Chg% perché le pagine di origine forniscono un selettore percentuale anziché un
  selettore separato per la variazione assoluta. Visualizza `—` quando uno dei due valori
  non è disponibile.
- Selezionare l'intestazione di un gruppo per comprimerlo o espanderlo. I gruppi seguono
  la prima occorrenza delle rispettive quotazioni nell'ordine delle voci configurate; le
  righe di un gruppo mantengono tale ordine.
- Le voci prive di gruppo appaiono sotto **Senza gruppo (Ungrouped)**.
- Passare il puntatore su Ultimo (Last) per vedere i valori pre-market e after-hours
  disponibili. Fare doppio clic su una riga di quotazione per aprirne la pagina di origine.
- Il lampeggiamento degli avvisi e i colori di rialzo/ribasso funzionano in entrambe le
  modalità dei prezzi.
- Le notizie si aprono automaticamente in una finestra separata **SmartTicker News** che
  contiene riquadri di gruppo statici **Simbolo / Titolo (Symbol / Headline)**. In modalità
  statica non scorrono. La finestra Notizie ha una normale barra del titolo e un bordo di
  ridimensionamento, pertanto le finestre Quotazioni e Notizie possono essere spostate
  indipendentemente su monitor diversi. Fare doppio clic su una riga di titolo per aprirne
  la fonte.
- Al primo avvio, Notizie usa una dimensione compatta di 680×340. SmartTicker la posiziona
  su un altro monitor, se disponibile; con un solo monitor tenta prima un'area libera in
  basso, a destra, in alto o a sinistra di Prezzi. È quindi possibile spostarla e
  ridimensionarla normalmente.
- All'interno di ogni gruppo Notizie, i titoli vengono alternati per quotazione: un titolo
  della prima quotazione, poi uno della successiva, continuando a turni. Una quotazione con
  molti titoli non può quindi occupare tutta la parte superiore del gruppo.
- Aprire l'elenco a discesa su una riga **Mostra notizie per (Show news for)** e selezionare
  o deselezionare ogni quotazione in modo indipendente. Può essere visibile qualsiasi
  combinazione di quotazioni, incluse tutte o nessuna. Il pulsante riepiloga la scelta
  corrente e le voci includono quotazione e fonte, così i simboli duplicati restano
  indipendenti. Le quotazioni deselezionate vengono salvate nel file di impostazioni come
  `hiddenNewsQuotes`, quindi persistono dopo un riavvio e vengono incluse nel backup delle
  impostazioni.
- Trascinare la maniglia puntinata accanto all'intestazione di un riquadro di quotazione o
  notizia e rilasciarla sulla metà sinistra o destra di un altro riquadro. L'ordine cambia
  in entrambe le finestre e viene salvato riordinando le voci configurate sottostanti.
- Un gruppo con molte righe scorre all'interno del proprio riquadro delimitato. L'intera
  visualizzazione scorre verticalmente solo quando le righe di riquadri mandate a capo non
  rientrano nell'altezza corrente della finestra.

La chiusura di **SmartTicker News** non disattiva la raccolta delle notizie. Per riaprirla,
fare clic con il pulsante destro del mouse sulla finestra Prezzi e selezionare
**Visualizza > Apri finestra statica delle notizie (View > Open static news window)**.
Selezionando **Visualizzazione statica: solo Prezzi (Static view: Prices only)** la si
chiude; selezionando **Visualizzazione statica: Prezzi con Notizie (Static view: Prices with
News)** la si riapre. Entrambe le opzioni scorrevoli chiudono la finestra Notizie separata;
l'opzione scorrevole Prezzi con Notizie ripristina il testo scorrevole delle notizie nel
ticker principale.

Il passaggio da una modalità all'altra applica la dimensione salvata per quella vista. Il
ticker scorrevole, la finestra statica Prezzi e la finestra statica Notizie mantengono
ciascuno una larghezza e un'altezza indipendenti.

### Spostare il ticker

Tenere premuta la maniglia a punti verticali nella parte superiore della sottile fascia a
sinistra, trascinare il ticker e rilasciare il pulsante del mouse. Il testo del ticker non
è una superficie di trascinamento, quindi selezionare o fare clic sul contenuto non può
avviare accidentalmente lo spostamento della finestra.

### Ridimensionare il ticker

Spostare il puntatore su un bordo o un angolo finché non appare il cursore di
ridimensionamento, quindi premere e trascinare. Nell'angolo inferiore destro è presente un
piccolo indicatore visibile di ridimensionamento. La larghezza minima della finestra è 420
pixel. L'altezza in modalità scorrevole va da 50 a 900 pixel, quella di Prezzi statici da
420 a 4320 pixel e quella di Notizie statiche da 240 a 4320 pixel.

Il ridimensionamento manuale aggiorna le dimensioni salvate per la vista attiva al termine
del trascinamento. Tutte e tre le coppie di dimensioni sono incluse nel backup delle
impostazioni. Le posizioni delle finestre non vengono memorizzate. Se una dimensione
scorrevole è troppo bassa per le righe Prezzi/Notizie e la dimensione del carattere
scorrevole selezionate, SmartTicker aumenta automaticamente l'altezza salvata. Selezionando
**Scorrimento da sinistra a destra: Prezzi con Notizie (Left-to-right scroll: Prices with
News)** viene quindi sempre creato spazio per le righe Notizie, senza nasconderle.
Ogni volta che una finestra si apre o viene spostata, SmartTicker mantiene almeno il suo
angolo superiore sinistro di 32 pixel all'interno dell'area di lavoro di uno schermo e
limita X e Y globali ad almeno 1. In questo modo la maniglia di spostamento o l'angolo del
titolo restano raggiungibili con il mouse anche dopo la disconnessione di un monitor.

### Mettere in pausa e riprendere

Selezionare il pulsante di stato sotto la maniglia di spostamento oppure fare clic con il
pulsante destro e scegliere **Pausa / Riprendi (Pause / Resume)**. La pausa interrompe gli
aggiornamenti automatici di prezzi e notizie e blocca il testo scorrevole. Impedisce inoltre
a entrambi i comandi di aggiornamento manuale di avviare nuovo lavoro. Una richiesta a una
fonte già in corso non viene annullata forzatamente solo a causa della Pausa e può terminare
prima che l'attività si arresti del tutto. Riprendi riavvia i timer automatici.

In Windows, SmartTicker imposta automaticamente la priorità del proprio processo del
sistema operativo su **Bassa (Low)** e abilita la **Modalità efficienza (Efficiency mode)**
(EcoQoS) prima di avviare l'interfaccia utente. Non esiste un'impostazione dell'app per
questo comportamento. Usa inoltre un percorso di rendering software a basso sovraccarico.
La temporizzazione del testo scorrevole si adatta alla velocità configurata e un testo
scorrevole in pausa, vuoto o scollegato arresta il proprio timer di animazione. Le righe
invariate evitano notifiche visive ridondanti. Il lampeggiamento degli avvisi e
l'evidenziazione marrone delle modifiche per tre secondi sono intenzionali e non mettono in
pausa lo scorrimento. In Linux la pianificazione dei processi resta affidata al sistema
operativo. Se Windows rifiuta una delle impostazioni del processo, SmartTicker segnala
l'errore nella traccia diagnostica e prosegue l'avvio.

### Aprire i collegamenti

Fare doppio clic sul testo collegato del ticker, incluso il titolo di una notizia, per
aprirne la fonte nel browser predefinito. SmartTicker non apre i collegamenti con un singolo
clic.

### Evidenziazione delle modifiche

Dopo ogni aggiornamento, SmartTicker contrassegna brevemente ciò che è cambiato con uno
sfondo marrone per tre secondi:

- Una quotazione il cui prezzo è diverso da quello della sincronizzazione precedente.
- Ogni titolo che non era presente nella sincronizzazione precedente di quella quotazione.

La prima sincronizzazione dopo l'avvio non evidenzia nulla perché non esiste un valore
precedente con cui effettuare il confronto. Un avviso attivato mantiene il proprio colore
lampeggiante e ha la precedenza.

### Riferimento del menu principale

| Comando | Effetto |
| --- | --- |
| **Aggiorna i prezzi ora (Refresh prices now)** | Riavvia il ciclo scaglionato dei prezzi e richiede la sua prima fascia temporale quando SmartTicker non è in pausa. |
| **Aggiorna le notizie ora (Refresh news now)** | Riavvia il ciclo scaglionato delle notizie e richiede la sua prima fascia temporale quando SmartTicker non è in pausa. |
| **Pausa / Riprendi (Pause / Resume)** | Attiva o disattiva aggiornamento e movimento del testo scorrevole. |
| **Visualizza > Scorrimento da sinistra a destra: solo Prezzi (View > Left-to-right scroll: Prices only)** | Usa soltanto il testo scorrevole orizzontale dei prezzi. Questa è l'impostazione predefinita. |
| **Visualizza > Scorrimento da sinistra a destra: Prezzi con Notizie (View > Left-to-right scroll: Prices with News)** | Usa entrambi i testi scorrevoli orizzontali. |
| **Visualizza > Visualizzazione statica: solo Prezzi (View > Static view: Prices only)** | Usa soltanto riquadri statici reattivi delle quotazioni. |
| **Visualizza > Visualizzazione statica: Prezzi con Notizie (View > Static view: Prices with News)** | Usa i riquadri delle quotazioni più la finestra statica Notizie separata. |
| **Visualizza > Apri finestra statica delle notizie (View > Open static news window)** | Riapre la finestra Notizie separata dopo la chiusura. Disponibile in modalità statica quando le notizie sono abilitate. |
| **Lingua (Language)** | Consente di scegliere una delle 16 lingue per i menu, il testo di stato e la Guida completa. Una finestra Guida già aperta si aggiorna immediatamente. |

La visibilità delle righe, la lingua e gli altri valori di configurazione vengono salvati
automaticamente.

## Quotazioni

Aprire **Quotazioni... (Quotes...)** dal menu accessibile con il pulsante destro del mouse.
Ogni voce configurata rappresenta un simbolo e una pagina web. I simboli duplicati sono
consentiti e restano indipendenti perché ogni voce dispone di fonte, selettori, opzioni di
raccolta e avvisi propri.

### Avvio rapido con l'esempio pubblicato

Quando non esistono voci, la finestra Quotazioni propone **Importa quotazioni di esempio da
GitHub (Import sample quotes from GitHub)**. L'operazione scarica l'esempio del repository e
sostituisce le impostazioni correnti dell'applicazione. Prima di usarla, esaminare ogni URL
importato e le condizioni correnti di ciascun sito web. In seguito è possibile modificare o
rimuovere qualsiasi voce di esempio.

**Importa configurazione quotazioni di esempio (Import Sample Quotes Config)**, nella parte
superiore delle finestre Quotazioni e Impostazioni dell'app, esegue la stessa operazione in
qualsiasi momento, previa conferma:

- SmartTicker chiede **Confermare? (Are you sure?)** e avverte che il download sostituisce
  quotazioni, gruppi di quotazioni, approvazioni delle fonti, vista, aspetto e altre
  impostazioni dell'app esistenti. Le regole di avviso risiedono in un file separato e non
  vengono eliminate.
- **Esporta configurazione esistente... (Export existing config...)** è facoltativo. Salva
  la configurazione corrente in un file JSON locale, quindi torna alla stessa conferma.
- **Importa configurazione quotazioni di esempio (Import Sample Quotes Config)** scarica
  l'esempio da Internet e sostituisce la configurazione.
- **Annulla (Cancel)** non modifica nulla.

### Aggiungere una quotazione o una notizia

1. Immettere l'etichetta **Ticker**, ad esempio `MSFT`. SmartTicker la ritaglia e la
   memorizza in maiuscolo.
2. Facoltativamente, scegliere un **Gruppo (Group)** esistente dall'elenco di ricerca oppure
   digitare un nuovo nome come `Nasdaq`, `Precious Metals` o `Mag 7`. Lasciare vuoto per
   **Senza gruppo (Ungrouped)**.
3. Selezionare una preimpostazione **Fonte (Source)**.
4. Immettere il **Suffisso URL (URL suffix)** oppure un URL completo quando si usa
   **URL personalizzato (Custom URL)**.
5. Selezionare **Prezzo (Price)**, **Notizie (News)** o entrambi in **Raccogli (Collect)**.
   È obbligatoria almeno un'opzione.
6. Immettere manualmente i selettori, usare i pulsanti di individuazione oppure lasciare
   vuoti i selettori facoltativi per usare il rilevamento integrato.
7. Selezionare **Convalida URL (Validate URL)** per verificare il prezzo regolare e/o i titoli.
8. Se SmartTicker richiede l'approvazione della fonte, esaminare il sito web e confermare
   solo quando si è autorizzati a raccoglierne i dati.
9. Selezionare **Aggiungi voce indipendente (Add independent entry)**. SmartTicker salva la
   voce e ne aggiorna immediatamente i dati abilitati.

### Raggruppare le quotazioni

Un gruppo è una raccolta denominata definita dall'utente. Non è legato a una borsa o a una
categoria integrata, quindi è possibile organizzare le voci per mercato, tipo di attività,
strategia, portafoglio, regione o secondo qualsiasi altro schema. I nomi vengono ritagliati,
possono usare Unicode e possono contenere fino a 80 caratteri. Ogni quotazione può
appartenere al massimo a un gruppo.

Usare **Gestisci gruppi (Manage groups)** accanto al campo Gruppo oppure selezionare
**Gruppi di quotazioni... (Quote groups...)** dal menu del ticker accessibile con il pulsante
destro del mouse. La finestra contiene tre aree operative:

- A sinistra, immettere un **Nome gruppo (Group name)**, quindi scegliere **Crea (Create)**.
  Selezionare un gruppo esistente, modificarne il nome e scegliere **Aggiorna (Update)**
  oppure scegliere **Elimina (Delete)**. I gruppi vuoti vengono conservati.
- A destra, selezionare una quotazione. Il gruppo corrente è indicato nella colonna
  **Gruppo corrente (Current group)**; **Senza gruppo (Ungrouped)** indica che non esiste
  alcuna associazione.
- Al centro, scegliere **Associa (Associate)** dopo aver selezionato un gruppo e una
  quotazione. Se la quotazione appartiene già a un altro gruppo, SmartTicker la sposta nel
  gruppo selezionato.
- Scegliere **Rimuovi associazione (Remove association)** per riportare soltanto la
  quotazione selezionata in **Senza gruppo (Ungrouped)**.
- L'eliminazione di un gruppo riporta tutte le sue quotazioni in **Senza gruppo
  (Ungrouped)**. Quotazioni, fonti, dati correnti e avvisi non vengono eliminati.
- Durante l'aggiunta o la modifica di una quotazione è inoltre possibile scegliere un
  gruppo esistente dall'elenco di ricerca oppure digitare un nuovo nome di gruppo.
- Usare i controlli su/giù in Voci configurate (Configured entries) per determinare
  l'ordine dei gruppi e delle righe nella tabella statica.
- In modalità statica, trascinare l'intestazione di un riquadro per riordinare direttamente
  interi gruppi. Lo stesso ordine viene usato dalle finestre separate Quotazioni e Notizie.

L'esempio pubblicato contiene sei gruppi di esempio e lascia disattivata la modalità statica
per impostazione predefinita. Dopo averlo importato, abilitare la visualizzazione statica per
vedere tali gruppi come tabella.

### Preimpostazioni delle fonti e URL

| Fonte | Cosa immettere | Criterio mostrato da SmartTicker |
| --- | --- | --- |
| **Yahoo Finance** | Un suffisso dopo `https://finance.yahoo.com/`, ad esempio `quote/MSFT/`. | È richiesta un'autorizzazione scritta. Le condizioni di Yahoo vietano la raccolta automatizzata senza autorizzazione preventiva. |
| **CNBC** | Un suffisso dopo `https://www.cnbc.com/`. | Controllare il criterio corrente del sito e le direttive robots. |
| **Trading Economics** | Un suffisso dopo `https://tradingeconomics.com/`. | Preferire un'API documentata o un feed autorizzato e controllare il criterio corrente del sito. |
| **URL personalizzato (Custom URL)** | Un URL completo di una pagina pubblica `http://` o `https://`. | Esaminare le condizioni del sito, l'informativa sulla privacy e le regole per l'accesso automatizzato. |

Sono accettati solo URL HTTP e HTTPS assoluti. Gli URL che contengono nomi utente o
password incorporati vengono rifiutati. L'accesso tramite browser non autorizza SmartTicker
a raccogliere una pagina e SmartTicker non usa sessioni del browser autenticate.

La riga **URL completo (Full URL)** mostra l'indirizzo finale prodotto dal prefisso della
preimpostazione e dal suffisso immesso. Controllarlo prima della convalida o
dell'individuazione.

### Opzioni di raccolta

- **Prezzo (Price)** richiede il prezzo regolare. I selettori facoltativi di variazione,
  pre-market e after-hours vengono valutati nella stessa pagina scaricata.
- **Notizie (News)** richiede i collegamenti ai titoli presenti nella pagina.
- Selezionando entrambi, una voce può contribuire a entrambe le aree del ticker.
- Deselezionare entrambi non è valido.

### Riferimento dei campi selettore

Un selettore CSS identifica un elemento nell'HTML statico di una pagina web. I selettori
sono facoltativi, a meno che il rilevamento automatico non riesca a trovare il valore
necessario.

| Campo | Valore estratto da SmartTicker |
| --- | --- |
| **Selettore prezzo (Price selector)** | Prezzo regolare o di chiusura. |
| **Variazione prezzo (Price change)** | Variazione percentuale della sessione regolare. Se il campo è vuoto, viene tentato il rilevamento integrato della variazione. |
| **Selettore pre-market (Pre-market selector)** | Prezzo pre-market, quando tale sessione è presente nella pagina. |
| **Variazione pre-market (Pre-market change)** | Variazione percentuale pre-market. |
| **Selettore after-hours (After-hours selector)** | Prezzo post-market o after-hours. |
| **Variazione after-hours (After-hours change)** | Variazione percentuale post-market o after-hours. |
| **Selettore notizie (News selector)** | Collegamenti ai titoli. Selezionare un elemento anchor o un contenitore i cui risultati includano collegamenti. |

I valori pre-market e after-hours integrano il prezzo regolare, non lo sostituiscono. Una
pagina può omettere tali elementi al di fuori della sessione di mercato corrispondente.

Esempi di selettori Yahoo Finance usati dall'esempio pubblicato:

```text
Price:                  [data-testid="qsp-price"]
Price change:           section.primary span[data-testid="qsp-price-change-percent"]
Pre-market price:       section.secondary span[data-testid="qsp-pre-price"]
Pre-market change:      section.secondary span[data-testid="qsp-pre-price-change-percent"]
After-hours price:      section.secondary span[data-testid="qsp-post-price"]
After-hours change:     section.secondary span[data-testid="qsp-post-price-change-percent"]
```

Il markup dei siti web cambia nel tempo. Considerare gli esempi come punti di partenza,
non come contratti permanenti.

### Individuare i selettori

Ogni campo selettore ha un pulsante **Individua (Discover)** corrispondente.

1. Completare l'URL della fonte e approvare il sito web, se è richiesta l'approvazione.
2. Selezionare il pulsante di individuazione per il tipo esatto di valore.
3. SmartTicker scarica l'HTML statico pubblico ed elenca i possibili selettori con un valore
   di esempio, la percentuale di affidabilità e il motivo nella descrizione comando.
4. Selezionare **Usa (Use)** accanto a un suggerimento per copiarlo nel campo corrispondente.
5. Convalidare o osservare il risultato prima di farvi affidamento.

L'individuazione non esegue JavaScript, non effettua l'accesso, non aggira i controlli di
accesso e non ispeziona il browser. Un valore disponibile soltanto tramite JavaScript può
non avere alcun selettore individuabile. I tipi di individuazione separati evitano
deliberatamente di mescolare valori pre-market e after-hours.

### Convalidare una fonte

**Convalida URL (Validate URL)** richiede la pagina e indica il prezzo regolare e/o il
numero di titoli che riesce a leggere. Può essere usato in sicurezza prima di immettere un
ticker perché SmartTicker usa un'etichetta temporanea per la verifica.

Attualmente questa convalida non verifica i quattro campi selettore pre-market e
after-hours. Usare i valori di esempio dell'individuazione e poi confermare i dati della
sessione visualizzati.

Gli errori tipici includono un errore HTTP, un timeout, un valore mancante, zero titoli,
l'approvazione della fonte non concessa, contenuto disponibile solo tramite JavaScript o un
selettore obsoleto.

### Limite di ripetizione delle notizie

**Mostra al massimo _N_ volte (Show max _N_ times)** accetta valori da 1 a 100 e il valore
predefinito è 5. SmartTicker conta una visualizzazione per ogni ciclo di aggiornamento
Notizie completato in cui viene restituito lo stesso titolo. Quando il titolo è apparso nel
numero di cicli configurato, viene ritirato per il resto della sessione corrente
dell'applicazione. La modifica o la rimozione della voce ne cancella la cronologia delle
ripetizioni.

### Modificare, ordinare e rimuovere le voci

L'elenco **Voci configurate (Configured entries)** mostra simbolo, gruppo, fonte, URL,
indicatori di raccolta, selettore del prezzo regolare, selettore delle notizie e limite di
ripetizione delle notizie.

- **Modifica (Edit)** carica la voce nel modulo. Selezionare **Salva modifiche (Save
  changes)** per applicarle oppure **Annulla modifica (Cancel edit)** per ignorare le
  modifiche del modulo.
- I pulsanti freccia su e giù cambiano l'ordine nel ticker e lo salvano immediatamente.
- **Rimuovi (Remove)** elimina la voce e i dati attualmente visualizzati.
- Se alcune regole di avviso fanno riferimento alla voce, SmartTicker chiede se eliminarle.
  Un avviso senza una quotazione configurata corrispondente non può attivarsi.
- La ridenominazione di una voce aggiorna i simboli visualizzati nelle regole di avviso
  collegate a tale voce.

## Impostazioni dell'app

Aprire **Impostazioni dell'app... (App Settings...)** dal menu accessibile con il pulsante
destro del mouse. Le modifiche hanno effetto e vengono salvate automaticamente; non esiste
un pulsante Applica (Apply).

### Righe e velocità del ticker

| Impostazione | Scelte | Valore predefinito | Effetto |
| --- | --- | --- | --- |
| Righe prezzi (Price rows) | Da 1 a 8 | 1 | Numero di righe parallele del testo scorrevole dei prezzi. |
| Velocità di scorrimento prezzi (Price scroll speed) | 20, 30, 40, 50, 65, 80, 100 o 120 px/sec | 50 | Velocità del testo scorrevole dei prezzi. |
| Righe notizie (News rows) | Da 1 a 8 | 1 | Numero di righe parallele del testo scorrevole dei titoli. |
| Velocità di scorrimento notizie (News scroll speed) | 20, 30, 40, 50, 65, 80, 100 o 120 px/sec | 40 | Velocità del testo scorrevole delle notizie. |
| Dimensione carattere scorrevole (Scrolling font size) | Da 9 a 24 pt | 14 pt | Testo di Prezzi e Notizie nelle righe scorrevoli. |
| Dimensione carattere statico (Static font size) | Da 9 a 24 pt | 13 pt | Testo di quotazioni e titoli nelle righe statiche. |
| Aggiornamento prezzi (Price refresh) | Da 30 a 300 secondi, in incrementi di 15 secondi | 60 secondi | Tempo entro il quale ogni voce prezzo autorizzata riceve un aggiornamento pianificato. |
| Aggiornamento notizie (News refresh) | Da 30 a 300 secondi, in incrementi di 15 secondi | 300 secondi | Tempo entro il quale ogni voce Notizie autorizzata riceve un aggiornamento pianificato. |

Le righe dei prezzi e la velocità di scorrimento dei prezzi sono disabilitate quando sono
attive le tabelle raggruppate statiche, perché tale modalità visualizza tutte le voci prezzo
e non fa mai scorrere automaticamente nessuna delle due finestre. Le impostazioni delle
righe e della velocità delle notizie vengono conservate per la visualizzazione scorrevole.

Le richieste di Prezzi e Notizie vengono distribuite indipendentemente in fasce di un
secondo lungo i rispettivi intervalli anziché iniziare insieme. Ad esempio, 60 voci in 30
secondi pianificano due voci al secondo; cinque voci in 30 secondi ne pianificano una circa
ogni sei secondi. Vengono eseguite contemporaneamente al massimo quattro richieste alle
fonti, il lavoro duplicato per la stessa voce e lo stesso flusso viene ignorato e le fasce
perse non vengono recuperate in un'unica raffica. **Aggiorna i prezzi ora (Refresh prices
now)** o **Aggiorna le notizie ora (Refresh news now)** riavvia soltanto il relativo flusso
e ne richiede la prima fascia. I prezzi e i titoli acquisiti correttamente restano visibili
mentre vengono letti i dati sostitutivi.

Ogni richiesta HTTP ha un timeout fisso di 20 secondi. Una fonte lenta non blocca il
dispatcher dell'interfaccia utente e non impedisce alle fasce successive di usare la
capacità di richiesta rimanente. SmartTicker segnala errori come HTTP 403 e 429 e non aggira
le restrizioni. Non analizza né applica automaticamente le direttive robots, i valori
crawl-delay o le istruzioni di backoff del server; scegliere quindi fonti conformi ed
evitare richieste inutilmente frequenti.

### Dimensioni delle finestre

Impostazioni dell'app memorizza tre coppie di dimensioni indipendenti:

| Finestra | Larghezza | Altezza | Valore predefinito |
| --- | --- | --- | --- |
| Visualizzazione scorrevole | 420–7680 px | 50–900 px | 980 × 64 px |
| Visualizzazione Prezzi statica | 420–7680 px | 420–4320 px | 980 × 420 px |
| Visualizzazione Notizie statica | 420–7680 px | 240–4320 px | 680 × 340 px |

La modifica di un valore viene applicata immediatamente quando la finestra o la vista è
attiva. L'esempio pubblicato mostra 1200 × 96 per la vista scorrevole, 1200 × 720 per Prezzi
statici e 760 × 480 per Notizie statiche, con testo scorrevole da 15 punti e testo statico
da 14 punti. Un'altezza scorrevole inferiore allo spazio richiesto dalle righe abilitate
viene aumentata automaticamente al minimo necessario.

Usare le quattro opzioni in **Visualizza (View)** per scegliere se mostrare le Notizie e se
il layout debba scorrere o restare statico. Il cambio di vista non elimina mai le voci
configurate.

### Avviare SmartTicker all'accesso

Abilitare **Avvia SmartTicker quando accedo (Start SmartTicker when I sign in)** per
registrare l'eseguibile installato soltanto per l'utente corrente.

- In Windows, SmartTicker usa la chiave del Registro di sistema `Run` dell'utente corrente.
- Nei desktop Linux che supportano la convenzione freedesktop per l'avvio automatico,
  SmartTicker scrive `smartticker.desktop` nella directory di avvio automatico dell'utente.
- L'opzione è disabilitata sulle piattaforme per le quali SmartTicker non dispone di un
  meccanismo di registrazione supportato.

Il sistema operativo è autorevole. Se l'avvio viene modificato all'esterno di SmartTicker,
la casella di controllo riflette lo stato del sistema operativo al successivo caricamento
delle impostazioni.

### Accesso ai siti web

**Consenti cookie dei siti web e reindirizzamenti tra host (Allow website cookies and
cross-host redirects)** è disabilitato per impostazione predefinita.

Quando è disabilitato:

- SmartTicker richiede un'approvazione esplicita per ogni host di sito web prima di
  interrogarlo.
- I cookie dei siti web non vengono accettati.
- I reindirizzamenti verso un host diverso vengono bloccati.
- Gli host approvati vengono memorizzati nelle impostazioni locali.

Quando è abilitato:

- SmartTicker ignora il passaggio di approvazione per singolo host.
- I cookie impostati dai siti web richiesti vengono conservati soltanto in un contenitore
  isolato in memoria e scompaiono alla chiusura di SmartTicker.
- Possono essere seguiti i reindirizzamenti verso altri host.
- SmartTicker continua a non leggere i cookie del browser, inviare credenziali o inviare
  moduli di accesso.

Disattivando questa opzione, i dati attualmente visualizzati provenienti da fonti non
approvate vengono rimossi finché tali host non vengono approvati e aggiornati.

#### Scelte sulla privacy dei siti web

Se una risposta viene riconosciuta come modulo di privacy/cookie che contiene scelte sia
positive sia negative, SmartTicker si mette in pausa e mostra il titolo della pagina, l'URL
richiesto, l'URL del consenso, il riepilogo del modulo e le etichette Accetta/Rifiuta
(Accept/Reject) del sito web.

- **Accetta (Accept)** invia i campi nascosti forniti dal modulo insieme al controllo
  Accetta esatto selezionato.
- **Rifiuta (Reject)** invia tali campi nascosti insieme al controllo Rifiuta esatto
  selezionato.
- **Annulla (Cancel)** non invia nulla.

Questa è una scelta sulla privacy del sito web, non l'approvazione dell'autorizzazione per
singola fonte di SmartTicker.

#### Convalidare tutte le fonti

Selezionare **Convalida tutte le fonti (Validate all sources)** per esaminare e verificare
ogni voce configurata.

1. Se l'accesso ai siti web è limitato, SmartTicker raggruppa le voci non approvate per nome
   host e visualizza una finestra di esame della fonte per ogni host.
2. Esaminare l'host, il riepilogo dei criteri, le indicazioni, i nomi delle fonti e i simboli.
3. Selezionare la conferma soltanto dopo aver esaminato il sito web ed essersi accertati di
   essere autorizzati a usarlo.
4. Scegliere **Approva questa fonte (Approve this source)**, **Ignora questa fonte (Skip this
   source)** o **Annulla convalida (Cancel validation)**.
5. SmartTicker verifica ogni voce autorizzata e indica i totali superati, non superati e
   ignorati. I singoli problemi appaiono sotto la riga di stato.

I record di approvazione registrano l'autorizzazione all'interno di SmartTicker; non
concedono diritti legali né prevalgono sulle condizioni del sito web.

### Aspetto

**Trasparenza finestra (Window transparency)** modifica soltanto lo sfondo del ticker. Il
testo resta opaco. L'intervallo va dal 20% al 100%, in incrementi del 5%, e il valore
predefinito è 100%.

I campi colore accettano valori esadecimali `#RRGGBB` e forniscono anche un selettore colore.

| Colore | Valore predefinito | Usato per |
| --- | --- | --- |
| Sfondo (Background) | `#10151D` | Sfondo del ticker prima dell'applicazione della trasparenza. |
| Nome quotazione (Quote name) | `#79C0FF` | Etichetta simbolo/fonte. |
| Prezzo di chiusura (Close price) | `#FFA657` | Prezzo regolare. |
| After-hours (After hours) | `#00E5FF` | Prezzi pre-market e after-hours. |
| 1ª notizia (News 1st) | `#FFFFFF` | Titoli 1, 5, 9 e così via. |
| 2ª notizia (News 2nd) | `#00E5FF` | Titoli 2, 6, 10 e così via. |
| 3ª notizia (News 3rd) | `#A3E635` | Titoli 3, 7, 11 e così via. |
| 4ª notizia (News 4th) | `#79C0FF` | Titoli 4, 8, 12 e così via. |
| Variazione in aumento (Change up) | `#3FB950` | Variazioni percentuali positive. |
| Variazione in diminuzione (Change down) | `#F85149` | Variazioni percentuali negative. |
| Lampeggiamento avviso (Alert blink) | `#FF00FF` | Avvisi di prezzo attivati, alternati con il nero. |

**Ripristina valori predefiniti (Reset to defaults)** ripristina tutti i colori indicati
sopra e l'opacità dello sfondo al 100%. Non ripristina righe, velocità, dimensioni dei
caratteri, dimensioni delle finestre, fonti, intervalli di aggiornamento, avvisi o lingua.

### Backup e ripristino

SmartTicker conserva le impostazioni dell'applicazione e le regole di avviso in file JSON
separati e fornisce pulsanti distinti per ciascun tipo di backup.

#### Esportare e importare le impostazioni

- **Esporta impostazioni... (Export settings...)** scrive le voci configurate, le
  associazioni e le definizioni dei gruppi, le quotazioni di notizie nascoste, l'ordine
  delle voci, i selettori, la scelta della vista scorrevole/statica delle quotazioni, gli
  host approvati, la visibilità delle righe, righe, velocità, dimensioni dei caratteri
  scorrevoli/statici, tutte e tre le coppie di dimensioni delle finestre, gli intervalli di
  aggiornamento, la preferenza di avvio, l'opzione di accesso ai siti web, i colori incluso
  quello di lampeggiamento degli avvisi, la trasparenza e la lingua.
- **Importa impostazioni... (Import settings...)** convalida l'intero file prima di
  modificare qualsiasi cosa. Un file rifiutato lascia invariate le impostazioni correnti.
- Un'importazione riuscita sostituisce tutte le voci configurate e le preferenze
  dell'applicazione. Non sostituisce il file separato delle regole di avviso.
- I gruppi sono inclusi nel file delle impostazioni sia come assegnazioni delle quotazioni
  sia come definizioni dei gruppi, pertanto anche un gruppo senza quotazioni sopravvive a
  un backup. Non esiste un file separato per esportare o importare soltanto i gruppi.
- La preferenza di avvio è presente nel backup delle impostazioni, ma la sua importazione
  non modifica silenziosamente la registrazione dell'avvio nel sistema operativo. Il
  sistema operativo resta autorevole; usare la casella Avvio (Startup) per modificare la
  registrazione nel computer corrente.
- I file di importazione sono limitati a 1 MiB, versione dello schema 1 e un massimo di 200
  sottoscrizioni. Proprietà sconosciute, ID duplicati, URL non validi, colori non validi,
  intervalli non validi o codici lingua non supportati vengono rifiutati anziché ignorati.

#### Esportare e importare le regole di avviso- **Esporta regole di avviso... (Export alert rules...)** scrive tutte le regole più Buzz, il numero di segnali acustici e la durata del lampeggiamento.
- **Importa regole di avviso... (Import alert rules...)** convalida l'intero file, quindi
  sostituisce tutte le regole correnti e le impostazioni di attivazione degli avvisi.
- Le regole si riconnettono prima tramite ID sottoscrizione. Quando gli ID sono diversi,
  SmartTicker tenta una corrispondenza del simbolo senza distinzione tra maiuscole e minuscole.
- Una regola importata senza una quotazione corrispondente viene conservata ma non può
  attivarsi. Lo stato dell'importazione indica quante regole sono state ricollegate o
  restano senza corrispondenza.
- I file di importazione degli avvisi sono limitati a 1 MiB.

Per il trasferimento a un altro computer, importare prima le impostazioni dell'applicazione
e poi le regole di avviso. Importando gli avvisi per secondi, le regole possono
riconnettersi per simbolo ai nuovi ID sottoscrizione.

### Modificare direttamente i file di configurazione

**Modifica configurazione corrente dell'app (Edit Current App Config)** e **Modifica regole
di avviso correnti (Edit Current Alert Rules)** in Impostazioni dell'app aprono il file JSON
attivo nell'editor di testo associato dal sistema a `.json`. Questa funzione è destinata
agli utenti esperti; le finestre di SmartTicker gestiscono le stesse impostazioni senza
rischi.

Entrambi i pulsanti mostrano prima una conferma che chiede di esportare il file corrente.
Eseguire l'esportazione: la modifica manuale può danneggiare il file e non esiste una
funzione di annullamento.

- **Esporta configurazione esistente... (Export existing config...)** salva il file
  corrente, quindi torna alla stessa richiesta.
- **Apri nell'editor di testo (Open in text editor)** apre il file attivo.
- **Annulla (Cancel)** non modifica nulla.

SmartTicker controlla il file e lo ricarica non appena l'editor lo salva:

- Un file valido viene applicato immediatamente e il ticker si aggiorna senza riavvio.
- JSON non valido, una violazione dello schema o qualsiasi altro errore di convalida viene
  rifiutato. La configurazione in esecuzione resta invariata e la finestra Impostazioni
  dell'app segnala il problema.
- Dopo una modifica rifiutata, correggere il file oppure ripristinare un'esportazione valida
  con **Importa impostazioni... (Import settings...)** o **Importa regole di avviso...
  (Import alert rules...)**.
- Un file che resta bloccato da un altro programma viene ritentato brevemente e poi segnalato.

La modifica del file delle regole di avviso segue le stesse regole e non influisce sulle
impostazioni dell'applicazione, perché i due file sono separati.

## Regole di avviso

Aprire **Avvisi (Alerts)** dal menu accessibile con il pulsante destro del mouse. Le regole
vengono valutate dopo ogni aggiornamento del prezzo riuscito e osservano soltanto il prezzo
regolare, non i valori pre-market o after-hours.

### Creare una regola

1. Selezionare una **Quotazione (Quote)** configurata. Le voci con lo stesso simbolo restano
   distinte.
2. Selezionare una **Condizione (Condition)** e immettere una soglia numerica usando un
   decimale invariante come `250.50`.
3. Facoltativamente, scegliere **Attiva dal (Active from)**. Lasciare vuoto per attivare
   immediatamente.
4. Lasciare selezionato **Non scade mai (Never expires)** oppure deselezionarlo e scegliere
   una data di scadenza.
5. Selezionare **Aggiungi regola (Add rule)**.

I confronti disponibili sono:

| Scelta | Significato |
| --- | --- |
| `LessThan` | Prezzo `<` soglia. |
| `LessThanOrEqual` | Prezzo `<=` soglia. |
| `GreaterThan` | Prezzo `>` soglia. |
| `GreaterThanOrEqual` | Prezzo `>=` soglia. |
| `EqualTo` | Il prezzo è esattamente uguale alla soglia. |
| `NotEqualTo` | Il prezzo è diverso dalla soglia. |

Il limite iniziale è incluso. Anche il limite di scadenza è incluso; una volta superato, la
regola non si attiva più. SmartTicker rifiuta una scadenza precedente all'inizio.

### Quando si attiva una regola

Una regola abilitata e pianificata si attiva una volta quando la sua condizione passa da
falsa a vera. Non invia una notifica a ogni aggiornamento mentre la condizione resta vera.
Quando il prezzo esce dalla condizione, la regola si riattiva e può scattare quando il
prezzo vi rientra.

Anche modificare una regola oppure disabilitarla e riabilitarla la riattiva. Una regola
abilitata può quindi scattare immediatamente se il prezzo regolare più recente soddisfa già
la sua condizione. Un prezzo non riuscito o mancante non può attivare una regola.

Quando si attivano una o più regole:

- La voce prezzo interessata alterna il colore configurato per il lampeggiamento degli
  avvisi e il nero per la durata configurata. Il colore di lampeggiamento predefinito è il
  magenta (`#FF00FF`).
- Se **Segnale acustico (Buzz)** è abilitato, SmartTicker riproduce la sequenza acustica
  configurata.
- Il messaggio di avviso identifica una regola oppure indica il numero di regole attivate
  insieme.
- Lo scorrimento del ticker continua mentre l'evidenziazione dell'avviso è attiva.

### Impostazioni di output degli avvisi

| Impostazione | Intervallo | Valore predefinito |
| --- | --- | --- |
| **Segnale acustico (Buzz)** | Attivato o disattivato | Attivato |
| Numero segnali acustici (Buzz count) | Da 1 a 20 | 15 |
| **Lampeggia per (Blink for)** | Da 5 a 900 secondi, in incrementi di 15 secondi | 60 secondi |

La disabilitazione del Segnale acustico (Buzz) lascia attivo l'avviso visivo. Se più regole
si attivano nella stessa valutazione, SmartTicker avvia un'unica sequenza acustica
configurata per tale valutazione. Modificare **Lampeggiamento avviso (Alert blink)** in
**Impostazioni dell'app > Aspetto (App Settings > Appearance)**. Si tratta di una preferenza
di aspetto dell'applicazione, quindi l'esportazione/importazione delle Impostazioni la
include al posto del file separato delle regole di avviso.

### Gestire le regole configurate

- **Modifica (Edit)** carica una regola nel modulo. Selezionare **Aggiorna regola (Update
  rule)** per salvarla oppure **Annulla (Cancel)** per lasciarla invariata.
- **Disabilita (Disable)** conserva la regola ma le impedisce di trovare corrispondenze.
  **Abilita (Enable)** la riattiva e la valuta rispetto al prezzo regolare più recente.
- **Rimuovi (Remove)** elimina la regola.
- L'elenco mostra stato di abilitazione, simbolo, riepilogo della condizione e pianificazione.

Le modifiche alle regole di avviso e alle impostazioni di output degli avvisi vengono
salvate automaticamente.

## File locali e privacy

SmartTicker archivia la configurazione localmente e non la sincronizza con un servizio per
sviluppatori.

In Windows, i file predefiniti sono:

```text
%LocalAppData%\SmartTicker\settings.json
%LocalAppData%\SmartTicker\alerts.json
```

In Linux, .NET usa la directory locale dei dati dell'applicazione dell'utente corrente,
normalmente:

```text
~/.local/share/SmartTicker/settings.json
~/.local/share/SmartTicker/alerts.json
```

### Usare una directory dati isolata

La diagnostica avanzata e le esecuzioni di test possono impostare
`SMARTTICKER_DATA_DIRECTORY` prima di avviare SmartTicker. Quando il valore non è vuoto,
entrambi i file vengono inseriti direttamente nella directory risolta come `settings.json`
e `alerts.json`; per tale processo non vengono usate le impostazioni predefinite della
piattaforma indicate sopra. Preferire un percorso assoluto e assicurarsi che sia scrivibile.

Esempio PowerShell:

```powershell
$env:SMARTTICKER_DATA_DIRECTORY = 'D:\SmartTicker-Profile'
& 'C:\Program Files\SmartTicker\SmartTicker.Desktop.exe'
```

Esempio di shell Linux:

```bash
SMARTTICKER_DATA_DIRECTORY="$HOME/.local/share/SmartTicker-Test" smartticker
```

Impostare la variabile prima dell'avvio del processo. SmartTicker non copia il profilo
predefinito nella directory selezionata, quindi una directory vuota inizia con una
configurazione vuota. Le istanze indirizzate alla stessa directory possono rilevare le
modifiche salvate reciprocamente. Usare i normali comandi di esportazione/importazione di
Impostazioni e Regole di avviso per i backup e il trasferimento dei profili.

La finestra Avvisi mostra il percorso esatto del file degli avvisi in uso. Le scritture
usano un file temporaneo seguito dalla sostituzione, affinché un file scritto parzialmente
non venga considerato corrente.

SmartTicker non dispone di account, telemetria, analisi, pubblicità o sincronizzazione
cloud. Quando SmartTicker richiede una fonte, il sito web riceve normali informazioni di
rete, come l'indirizzo IP dell'utente. L'apertura della Guida richiede la guida non elaborata
da GitHub. Per informazioni complete, leggere `PRIVACY.md` nel repository.

È responsabilità dell'utente garantire che ogni URL e selettore di origine venga usato in
conformità alle condizioni, alla licenza, alle direttive robots e alle leggi applicabili del
sito web.

## Risoluzione dei problemi

### Una quotazione risulta non disponibile o senza prezzo

Una richiesta a una fonte scade dopo 20 secondi. Se la quotazione dispone di un'istantanea
precedente riuscita, un aggiornamento non riuscito la mantiene visibile; in caso contrario,
la quotazione mostra **Non disponibile (Unavailable)** finché un aggiornamento successivo
non riesce. Leggere l'errore di convalida o aggiornamento prima di modificare i selettori.

1. Aprire **Quotazioni... (Quotes...)**, modificare la voce e controllare l'URL completo
   (Full URL).
2. Verificare che **Prezzo (Price)** sia selezionato.
3. Approvare il sito web, se richiesto.
4. Selezionare **Convalida URL (Validate URL)** e leggerne il risultato esatto.
5. Eseguire **Individua prezzo (Discover price)** oppure esaminare l'HTML statico della
   pagina e aggiornare il selettore.
6. Controllare se la pagina richiede JavaScript, autenticazione o un consenso che
   SmartTicker non può gestire in sicurezza.
7. Rispettare HTTP 403, 429, le restrizioni robots e il criterio del sito per l'accesso
   automatizzato.

### Dati pre-market o after-hours mancanti

- La sessione di mercato corrispondente potrebbe non essere attiva.
- La pagina potrebbe omettere l'elemento della sessione quando non esiste alcun valore.
- Verificare che i selettori pre-market puntino a elementi pre-market e quelli after-hours
  a elementi post-market.
- Eseguire nuovamente il comando di individuazione corrispondente perché il markup del sito
  web potrebbe essere cambiato.

### Le notizie sono vuote

- Verificare che **Notizie (News)** sia selezionato.
- Convalidare la fonte ed eseguire **Individua notizie (Discover news)**.
- Assicurarsi che il selettore restituisca collegamenti con testo visibile del titolo.
- Una richiesta Notizie non riuscita o scaduta mantiene i titoli acquisiti correttamente in
  precedenza, se disponibili. Una fonte senza alcun risultato riuscito resta vuota fino al
  completamento di una fascia successiva.
- Un titolo scompare dopo aver raggiunto il limite di ripetizione configurato per la sessione.
- In Notizie statiche, verificare che la quotazione desiderata sia selezionata in
  **Mostra notizie per (Show news for)**.

### L'individuazione dei selettori non trova nulla

L'individuazione legge soltanto l'HTML statico scaricato. Non può vedere i valori creati
successivamente dal codice JavaScript della pagina. Immettere manualmente un selettore
verificato, scegliere una pagina o un feed statico oppure usare un'API autorizzata e
documentata tramite una pagina pubblica compatibile.

### Un avviso non si attiva

- Verificare che la quotazione collegata esista ancora, raccolga Prezzo e disponga di un
  prezzo regolare acquisito correttamente.
- Verificare che la regola sia Abilitata (Enabled) e compresa nella pianificazione di
  inizio/scadenza.
- Controllare il confronto e la soglia. `EqualTo` richiede l'uguaglianza decimale esatta.
- Ricordare che una condizione continuamente vera si attiva una sola volta; deve diventare
  falsa prima di potersi riattivare, a meno che la regola non venga modificata o riabilitata.
- I prezzi pre-market e after-hours non determinano le regole di avviso.

### SmartTicker non si sposta o ridimensiona

- Spostare soltanto dalla maniglia a punti verticali nella fascia sinistra.
- Ridimensionare da un bordo o un angolo; usare l'indicatore visibile in basso a destra se
  è difficile usare un bordo.
- Il contenuto del ticker non è intenzionalmente una superficie di spostamento.

### I gruppi statici o i valori non sono quelli previsti

- Aprire **Quotazioni... (Quotes...)** e verificare il valore Gruppo (Group) di ogni voce.
- Aprire **Gruppi di quotazioni... (Quote groups...)** per gestire le definizioni dei gruppi
  e controllare l'associazione corrente di ogni quotazione.
- Le voci con Gruppo vuoto appaiono sotto **Senza gruppo (Ungrouped)**.
- **Var. (Chg)** viene calcolata da Last e Chg%; non viene estratta indipendentemente dalla
  pagina. Resta `—` quando la percentuale non è disponibile.
- Riordinare le voci con i controlli su/giù per cambiare l'ordine dei gruppi e delle righe.
- Trascinare la maniglia puntinata sull'intestazione di un riquadro per spostare l'intero
  gruppo. Rilasciarla sulla metà sinistra di un altro riquadro per posizionarlo prima oppure
  sulla metà destra per posizionarlo dopo.
- Selezionare **Aggiorna i prezzi ora (Refresh prices now)** mentre SmartTicker non è in
  pausa per aggiornare la tabella.

### Il testo della Guida non è formattato o la navigazione non funziona

- La finestra Guida deve mostrare intestazioni, paragrafi, elenchi, tabelle, collegamenti e
  blocchi di codice formattati anziché la punteggiatura Markdown.
- Usare **In questa pagina (On this page)** a sinistra per passare a una sezione principale.
  Anche i collegamenti nella tabella Navigazione rapida scorrono all'interno del documento.
- Chiudere e riaprire la Guida, oppure cambiare **Lingua (Language)**, per richiedere la
  guida pubblicata corrispondente. Nell'attesa SmartTicker visualizza la guida formattata
  incorporata nell'applicazione installata.

### La Guida online non è disponibile o è obsoleta

- Chiudere e riaprire la Guida per richiedere nuovamente la guida pubblicata.
- Aprire in un browser l'indirizzo GitHub non elaborato mostrato all'inizio di questa guida
  per esaminare direttamente il file pubblicato.
- SmartTicker usa la guida incorporata quando la richiesta non riesce o restituisce un file
  vuoto.
- Le modifiche online appaiono soltanto dopo che `HELPME.md` o il file
  `help/HELPME.<language-code>.md` corrispondente è stato pubblicato nel ramo `main` del
  repository.

## Supporto

Segnalare i problemi riproducibili all'indirizzo:

<https://github.com/bulentozkir/smartticker/issues>

Includere la versione di SmartTicker, il sistema operativo, il nome host della fonte, lo
stato della convalida e il testo esatto dell'errore. Prima della pubblicazione, rimuovere URL
privati o altre informazioni sensibili.