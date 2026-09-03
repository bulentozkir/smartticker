# Aide SmartTicker

Ce guide concerne SmartTicker 1.0.3. Il explique le bandeau principal, les paramètres
de l’application, les cotations, les règles d’alerte, les autorisations des sites web,
les sauvegardes et les problèmes courants.

SmartTicker lit le HTML statique public des pages web que vous configurez. Il ne
fournit pas de flux de données de marché, et les informations extraites peuvent être
retardées, incomplètes ou erronées. Vérifiez les informations financières importantes
auprès d’une source faisant autorité.

## Navigation rapide

| Zone | Accéder à |
| --- | --- |
| Prise en main | [Ouvrir l’aide et les fenêtres de configuration](#ouvrir-laide-et-les-fenêtres-de-configuration) |
| Bandeau principal | [Commandes](#commandes-du-bandeau-principal) · [Affichage défilant ou statique](#choisir-laffichage-défilant-ou-statique-des-cotations) · [Déplacer](#déplacer-le-bandeau) · [Redimensionner](#redimensionner-le-bandeau) · [Pause](#mettre-en-pause-et-reprendre) · [Référence du menu](#référence-du-menu-principal) |
| Cotations et actualités | [Cotations](#cotations) · [Ajouter une entrée](#ajouter-une-cotation-ou-une-actualité) · [Regrouper les cotations](#regrouper-les-cotations) · [URL des sources](#préréglages-de-sources-et-url) · [Sélecteurs](#référence-des-champs-de-sélecteur) · [Découverte](#découvrir-des-sélecteurs) · [Validation](#valider-une-source) |
| Préférences de l’application | [Paramètres de l’application](#paramètres-de-lapplication) · [Lignes et vitesse](#lignes-et-vitesse-du-bandeau) · [Démarrage](#démarrer-smartticker-lors-de-la-connexion) · [Accès aux sites web](#accès-aux-sites-web) · [Apparence](#apparence) · [Sauvegarde et restauration](#sauvegarde-et-restauration) · [Modifier les fichiers de configuration](#modifier-directement-les-fichiers-de-configuration) |
| Alertes de prix | [Règles d’alerte](#règles-dalerte) · [Créer une règle](#créer-une-règle) · [Comportement au déclenchement](#lorsquune-règle-se-déclenche) · [Sortie des alertes](#paramètres-de-sortie-des-alertes) · [Gérer les règles](#gérer-les-règles-configurées) |
| Données et assistance | [Fichiers locaux et confidentialité](#fichiers-locaux-et-confidentialité) · [Dépannage](#dépannage) · [Assistance](#assistance) |

## Ouvrir l’aide et les fenêtres de configuration

Cliquez avec le bouton droit sur le bandeau pour ouvrir son menu. Les principales
commandes de configuration sont les suivantes :

- **Quotes...** : ajouter, tester, modifier, classer et supprimer des sources de
	cotations ou d’actualités.
- **Quote groups...** : créer, mettre à jour ou supprimer des groupes et leur associer
	des cotations.
- **Alerts** : créer et gérer des règles d’alerte de prix.
- **App Settings...** : configurer les lignes, les vitesses, les intervalles
	d’actualisation, le démarrage, l’accès aux sites web, les couleurs, la transparence
	et les sauvegardes.
- **View** : sélectionner l’une des quatre combinaisons mutuellement exclusives :
	défilante ou statique, avec les prix uniquement ou les prix et les actualités.
- **Help** : ouvrir ce guide dans SmartTicker.
- **About SmartTicker** : afficher la version installée et l’avis de licence.
- **Exit** : fermer complètement SmartTicker.

La fenêtre d’aide met immédiatement en forme et affiche le guide intégré correspondant
à la langue sélectionnée dans l’application, puis consulte le guide en ligne dans la
même langue chaque fois que vous ouvrez l’aide ou changez de **Language**. Le guide en
ligne français est :

<https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/help/HELPME.fr.md>

Si la requête en ligne échoue, SmartTicker conserve la traduction correspondante intégrée
à l’application installée. Lorsque l’aide est ouverte, tout changement de **Language**
met immédiatement à jour son titre, son état, sa navigation et l’intégralité du guide.
Fermez l’aide à l’aide du bouton de fermeture normal de sa barre de titre.

## Commandes du bandeau principal

### Choisir l’affichage défilant ou statique des cotations

SmartTicker propose quatre modes d’affichage mutuellement exclusifs. Cliquez avec le
bouton droit sur le bandeau, ouvrez **View**, puis sélectionnez-en un. La disposition
change immédiatement et votre choix est enregistré.

| Option d’affichage | Résultat |
| --- | --- |
| **Left-to-right scroll: Prices only** | Bandeau défilant des prix dans la fenêtre principale ; aucune actualité affichée. Il s’agit du réglage par défaut. |
| **Left-to-right scroll: Prices with News** | Bandeaux défilants des prix et des actualités dans la fenêtre principale. |
| **Static view: Prices only** | Vignettes de prix adaptatives dans la fenêtre principale ; aucune fenêtre d’actualités. |
| **Static view: Prices with News** | Vignettes de prix adaptatives et fenêtre statique **SmartTicker News** distincte. |

Les fichiers de paramètres créés avant l’ajout de ces choix sont associés à la
combinaison correspondante de leurs réglages enregistrés pour le défilement ou
l’affichage statique et les actualités. Le mode d’affichage se gère uniquement depuis
le menu **View** accessible par clic droit sur le bandeau.

- Dans l’un ou l’autre mode défilant, les prix utilisent le bandeau horizontal ainsi
	que le nombre de lignes de prix et la vitesse de défilement configurés.
- Dans l’un ou l’autre mode statique, les groupes apparaissent sous forme de vignettes
	adaptatives disposées de gauche à droite. Les vignettes ne passent à la ligne suivante
	que lorsque la fenêtre est trop étroite. Les prix ne se déplacent pas automatiquement.
- Chaque vignette de cotation possède ses propres colonnes alignées **Symbol**, **Last**,
	**Chg** et **Chg%**. **Chg** est calculé à partir de Last et Chg%, car les pages sources
	fournissent un sélecteur de pourcentage plutôt qu’un sélecteur distinct de variation
	absolue. La valeur `—` s’affiche lorsque l’une de ces données est indisponible.
- Sélectionnez l’en-tête d’un groupe pour le réduire ou le développer. Les groupes
	suivent la première occurrence de leurs cotations dans l’ordre des entrées configurées ;
	les lignes d’un groupe conservent cet ordre.
- Les entrées sans groupe apparaissent sous **Ungrouped**.
- Survolez Last pour voir les valeurs disponibles avant l’ouverture et après la clôture.
	Double-cliquez sur une ligne de cotation pour ouvrir sa page source.
- Le clignotement des alertes et les couleurs de hausse/baisse fonctionnent dans les
	deux modes de prix.
- Les actualités s’ouvrent automatiquement dans une fenêtre **SmartTicker News**
	distincte contenant des vignettes de groupes statiques **Symbol / Headline**. Elles ne
	défilent pas en mode statique. La fenêtre d’actualités possède une barre de titre et
	une bordure de redimensionnement normales ; les fenêtres de cotations et d’actualités
	peuvent donc être déplacées indépendamment sur différents écrans. Double-cliquez sur
	une ligne de titre pour ouvrir sa source.
- Au premier lancement, la fenêtre d’actualités utilise une taille compacte de 680×340.
	SmartTicker la place sur un autre écran lorsqu’il en existe un ; avec un seul écran,
	il recherche d’abord une zone libre sous les prix, puis à droite, au-dessus ou à
	gauche. Vous pouvez ensuite la déplacer et la redimensionner normalement.
- Dans chaque groupe d’actualités, les titres sont entrelacés par cotation : un titre de
	la première cotation, puis un de la suivante, et ainsi de suite par cycles. Une
	cotation comportant de nombreux titres ne peut donc pas occuper tout le haut de son
	groupe.
- Ouvrez la liste déroulante sur une ligne **Show news for**, puis cochez ou décochez
	chaque cotation séparément. Toute combinaison peut être visible, y compris toutes ou
	aucune. Le bouton résume la sélection actuelle, et les entrées indiquent la cotation
	et la source afin que les symboles en double restent indépendants. Les cotations
	décochées sont enregistrées dans votre fichier de paramètres sous `hiddenNewsQuotes` ;
	elles persistent après un redémarrage et sont incluses dans une sauvegarde des paramètres.
- Faites glisser la poignée pointillée située à côté de l’en-tête d’une vignette de
	cotation ou d’actualité et déposez-la sur la moitié gauche ou droite d’une autre
	vignette. L’ordre change dans les deux fenêtres et est enregistré en réorganisant les
	entrées configurées sous-jacentes.
- Un groupe comportant de nombreuses lignes défile à l’intérieur de sa propre vignette
	limitée. L’ensemble de la vue ne défile verticalement que si les rangées de vignettes
	revenues à la ligne ne tiennent pas dans la hauteur actuelle de la fenêtre.

Fermer **SmartTicker News** ne désactive pas la collecte des actualités. Pour la rouvrir,
cliquez avec le bouton droit sur la fenêtre des prix et sélectionnez **View > Open static
news window**. Sélectionner **Static view: Prices only** la ferme ; sélectionner **Static
view: Prices with News** l’ouvre de nouveau. L’un ou l’autre choix défilant ferme la
fenêtre d’actualités distincte ; le choix défilant avec prix et actualités rétablit le
bandeau des actualités dans la fenêtre principale.

Le changement de mode applique la taille enregistrée pour cette vue. Le bandeau
défilant, la fenêtre statique des prix et la fenêtre statique des actualités conservent
chacun une largeur et une hauteur indépendantes.

### Déplacer le bandeau

Maintenez enfoncée la poignée à points verticaux en haut de l’étroite bande de gauche,
faites glisser le bandeau, puis relâchez le bouton de la souris. Le texte du bandeau
n’est pas une zone de déplacement ; sélectionner ou cliquer sur son contenu ne peut
donc pas déplacer accidentellement la fenêtre.

### Redimensionner le bandeau

Placez le pointeur sur un bord ou un angle jusqu’à l’apparition d’un curseur de
redimensionnement, puis cliquez et faites glisser. L’angle inférieur droit comporte un
petit repère de redimensionnement visible. La largeur minimale de la fenêtre est de
420 pixels. La hauteur du mode défilant va de 50 à 900 pixels, celle de la fenêtre
statique des prix de 420 à 4320 pixels et celle de la fenêtre statique des actualités
de 240 à 4320 pixels.

Le redimensionnement manuel met à jour les dimensions enregistrées pour la vue active
une fois le déplacement terminé. Les trois paires de dimensions sont incluses dans une
sauvegarde des paramètres. Les positions des fenêtres ne sont pas enregistrées. Si une
taille défilante est trop basse pour les lignes de prix ou d’actualités sélectionnées et
la taille de police défilante, SmartTicker augmente automatiquement la hauteur
enregistrée. Sélectionner **Left-to-right scroll: Prices with News** réserve donc toujours
la place nécessaire aux lignes d’actualités au lieu de les masquer silencieusement.
Lorsqu’une fenêtre s’ouvre ou se déplace, SmartTicker maintient au moins son angle
supérieur gauche de 32 pixels dans la zone de travail d’un écran et limite les
coordonnées globales X et Y à un minimum de 1. La poignée de déplacement ou l’angle de
la barre de titre reste ainsi accessible à la souris même après la déconnexion d’un écran.

### Mettre en pause et reprendre

Sélectionnez le bouton d’état sous la poignée de déplacement, ou cliquez avec le bouton
droit et sélectionnez **Pause / Resume**. La mise en pause arrête les actualisations
automatiques des prix et des actualités et fige le bandeau. Elle empêche également les
deux commandes d’actualisation manuelle de démarrer un nouveau travail. Une requête de
source déjà en cours n’est pas annulée de force uniquement à cause de la pause et peut
se terminer avant que toute activité cesse. La reprise redémarre les minuteurs automatiques.

Sous Windows, SmartTicker règle automatiquement la priorité de son processus sur
**Low** et active le **Efficiency mode** (EcoQoS) de Windows avant de démarrer
l’interface. Aucun paramètre de l’application ne contrôle ce comportement. Il utilise
également un mode de rendu logiciel à faible surcharge. La cadence du bandeau s’adapte
à la vitesse configurée, et un bandeau en pause, vide ou détaché arrête son minuteur
d’animation. Les lignes inchangées n’émettent pas de notifications visuelles
redondantes. Le clignotement des alertes et le surlignage brun de trois secondes des
modifications sont intentionnels et n’interrompent pas le défilement. Sous Linux,
l’ordonnancement du processus est laissé au système d’exploitation. Si Windows refuse
l’un des réglages du processus, SmartTicker consigne l’échec dans la trace de diagnostic
et poursuit son démarrage.

### Ouvrir les liens

Double-cliquez sur le texte lié du bandeau, notamment sur un titre d’actualité, pour
ouvrir sa source dans votre navigateur par défaut. SmartTicker n’ouvre pas les liens
avec un simple clic.

### Indicateurs de modification

Après chaque actualisation, SmartTicker signale brièvement sur un fond brun, pendant
trois secondes, les éléments qui ont changé :

- Une cotation dont le prix diffère de la synchronisation précédente.
- Chaque titre absent de la synchronisation précédente pour cette cotation.

La première synchronisation après le démarrage ne surligne rien, puisqu’il n’existe
aucune valeur antérieure à comparer. Une alerte déclenchée conserve sa propre couleur
de clignotement et reste prioritaire.

### Référence du menu principal

| Commande | Effet |
| --- | --- |
| **Refresh prices now** | Redémarre le cycle échelonné des prix et demande son premier créneau lorsque SmartTicker n’est pas en pause. |
| **Refresh news now** | Redémarre le cycle échelonné des actualités et demande son premier créneau lorsque SmartTicker n’est pas en pause. |
| **Pause / Resume** | Active ou désactive l’actualisation et le déplacement du bandeau. |
| **View > Left-to-right scroll: Prices only** | Utilise uniquement le bandeau horizontal des prix. Il s’agit du réglage par défaut. |
| **View > Left-to-right scroll: Prices with News** | Utilise les deux bandeaux horizontaux. |
| **View > Static view: Prices only** | Utilise uniquement les vignettes de cotation statiques et adaptatives. |
| **View > Static view: Prices with News** | Utilise les vignettes de cotation et la fenêtre statique d’actualités distincte. |
| **View > Open static news window** | Rouvre la fenêtre d’actualités distincte après sa fermeture. Disponible en mode statique lorsque les actualités sont activées. |
| **Language** | Permet de choisir l’une des 16 langues pour les menus, le texte d’état et l’intégralité du guide d’aide. Une fenêtre d’aide ouverte se met immédiatement à jour. |

La visibilité des lignes, la langue et les autres valeurs de configuration sont
enregistrées automatiquement.

## Cotations

Ouvrez **Quotes...** depuis le menu contextuel. Chaque entrée configurée représente un
symbole et une page web. Les symboles en double sont autorisés et restent indépendants,
car chaque entrée possède sa propre source, ses sélecteurs, ses options de collecte et
ses alertes.

### Démarrage rapide avec l’exemple publié

Lorsqu’il n’existe aucune entrée, la fenêtre des cotations propose **Import sample quotes
from GitHub**. Cette action télécharge l’exemple du dépôt et remplace les paramètres
actuels de l’application. Vérifiez chaque URL importée et les conditions actuelles de
chaque site web avant de l’utiliser. Vous pouvez ensuite modifier ou supprimer toute
entrée d’exemple.

La commande **Import Sample Quotes Config**, en haut des fenêtres des cotations et des
paramètres de l’application, effectue la même opération à tout moment, après confirmation :

- SmartTicker demande **Are you sure?** et avertit que le téléchargement remplace vos
	cotations, groupes de cotations, approbations de sources, vue, apparence et autres
	paramètres de l’application. Les règles d’alerte se trouvent dans leur propre fichier
	et ne sont pas supprimées.
- **Export existing config...** est facultatif. Cette commande enregistre votre
	configuration actuelle dans un fichier JSON local, puis revient à la même confirmation.
- **Import Sample Quotes Config** télécharge l’exemple depuis Internet et remplace votre
	configuration.
- **Cancel** ne modifie rien.

### Ajouter une cotation ou une actualité

1. Saisissez l’étiquette **Ticker**, par exemple `MSFT`. SmartTicker la nettoie et
	 l’enregistre en majuscules.
2. Choisissez éventuellement un **Group** existant dans la liste, ou saisissez un nouveau
	 nom tel que `Nasdaq`, `Precious Metals` ou `Mag 7`. Laissez le champ vide pour
	 **Ungrouped**.
3. Sélectionnez un préréglage **Source**.
4. Saisissez le **URL suffix**, ou une URL complète lorsque vous utilisez **Custom URL**.
5. Sélectionnez **Price**, **News**, ou les deux sous **Collect**. Au moins une option est
	 requise.
6. Saisissez les sélecteurs manuellement, utilisez les boutons de découverte ou laissez
	 vides les sélecteurs facultatifs pour employer la détection intégrée.
7. Sélectionnez **Validate URL** pour tester le prix normal et/ou les titres.
8. Si SmartTicker demande l’approbation de la source, examinez le site web et ne confirmez
	 que si vous êtes autorisé à y effectuer une collecte.
9. Sélectionnez **Add independent entry**. SmartTicker enregistre l’entrée et actualise
	 immédiatement ses données activées.

### Regrouper les cotations

Un groupe est une collection nommée que vous définissez. Il n’est associé ni à une
bourse ni à une catégorie intégrée ; vous pouvez donc organiser les entrées par marché,
type d’actif, stratégie, portefeuille, région ou tout autre schéma. Les noms sont
nettoyés, peuvent utiliser Unicode et comporter jusqu’à 80 caractères. Chaque cotation
peut appartenir à un seul groupe au maximum.

Utilisez **Manage groups** à côté du champ Group, ou sélectionnez **Quote groups...**
dans le menu contextuel du bandeau. La fenêtre comporte trois zones de travail :

- À gauche, saisissez un **Group name**, puis choisissez **Create**. Sélectionnez un
	groupe existant, modifiez son nom et choisissez **Update**, ou choisissez **Delete**.
	Les groupes vides sont conservés.
- À droite, sélectionnez une cotation. Son groupe actuel apparaît dans la colonne
	**Current group** ; **Ungrouped** signifie qu’elle n’a aucune association.
- Au milieu, choisissez **Associate** après avoir sélectionné un groupe et une cotation.
	Si cette cotation appartient déjà à un autre groupe, SmartTicker la déplace dans le
	groupe sélectionné.
- Choisissez **Remove association** pour renvoyer uniquement la cotation sélectionnée
	dans **Ungrouped**.
- La suppression d’un groupe renvoie toutes ses cotations dans **Ungrouped**. Les
	cotations, les sources, les données actuelles et les alertes ne sont pas supprimées.
- Vous pouvez aussi choisir un groupe existant dans la liste lors de l’ajout ou de la
	modification d’une cotation, ou y saisir le nom d’un nouveau groupe.
- Utilisez les commandes haut/bas de Configured entries pour déterminer l’ordre des
	groupes et des lignes dans le tableau statique.
- En mode statique, faites glisser l’en-tête d’une vignette pour réorganiser directement
	des groupes entiers. Le même ordre est utilisé par les fenêtres distinctes des
	cotations et des actualités.

L’exemple publié contient six groupes d’exemple, tout en laissant le mode statique
désactivé par défaut. Activez l’affichage statique après l’importation pour voir ces
groupes sous forme de tableau.

### Préréglages de sources et URL

| Source | Valeur à saisir | Politique affichée par SmartTicker |
| --- | --- | --- |
| **Yahoo Finance** | Un suffixe après `https://finance.yahoo.com/`, par exemple `quote/MSFT/`. | Autorisation écrite requise. Les conditions de Yahoo interdisent la collecte automatisée sans autorisation préalable. |
| **CNBC** | Un suffixe après `https://www.cnbc.com/`. | Consultez la politique actuelle du site et ses directives robots. |
| **Trading Economics** | Un suffixe après `https://tradingeconomics.com/`. | Privilégiez une API documentée ou un flux autorisé et consultez la politique actuelle du site. |
| **Custom URL** | Une URL complète de page publique `http://` ou `https://`. | Consultez les conditions, la politique de confidentialité et les règles d’accès automatisé du site. |

Seules les URL HTTP et HTTPS absolues sont acceptées. Les URL contenant des noms
d’utilisateur ou mots de passe intégrés sont rejetées. Une connexion dans le navigateur
n’autorise pas SmartTicker à collecter une page, et SmartTicker n’utilise pas les
sessions authentifiées du navigateur.

La ligne **Full URL** affiche l’adresse finale produite à partir du préfixe prédéfini et
de votre suffixe. Vérifiez-la avant la validation ou la découverte.

### Options de collecte

- **Price** demande le prix normal. Les sélecteurs facultatifs de variation, de
	préouverture et d’après-clôture sont évalués à partir de la même page téléchargée.
- **News** demande les liens des titres sur la page.
- Sélectionner les deux permet à une entrée d’alimenter les deux zones du bandeau.
- Désélectionner les deux est invalide.

### Référence des champs de sélecteur

Un sélecteur CSS identifie un élément dans le HTML statique d’une page web. Les
sélecteurs sont facultatifs, sauf si la détection automatique ne trouve pas la valeur
souhaitée.

| Champ | Valeur extraite par SmartTicker |
| --- | --- |
| **Price selector** | Prix normal ou de clôture. |
| **Price change** | Variation en pourcentage de la séance normale. Lorsque le champ est vide, la détection de variation intégrée est tentée. |
| **Pre-market selector** | Prix de préouverture, lorsque cette séance existe sur la page. |
| **Pre-market change** | Variation en pourcentage avant l’ouverture. |
| **After-hours selector** | Prix après marché ou après la clôture. |
| **After-hours change** | Variation en pourcentage après marché ou après la clôture. |
| **News selector** | Liens des titres. Sélectionnez une ancre ou un conteneur dont les résultats comprennent des liens. |

Les valeurs de préouverture et d’après-clôture complètent le prix normal ; elles ne le
remplacent pas. Une page peut omettre ces éléments en dehors de la séance de marché
correspondante.

Voici des exemples de sélecteurs Yahoo Finance utilisés par l’exemple publié :

```text
Price:                  [data-testid="qsp-price"]
Price change:           section.primary span[data-testid="qsp-price-change-percent"]
Pre-market price:       section.secondary span[data-testid="qsp-pre-price"]
Pre-market change:      section.secondary span[data-testid="qsp-pre-price-change-percent"]
After-hours price:      section.secondary span[data-testid="qsp-post-price"]
After-hours change:     section.secondary span[data-testid="qsp-post-price-change-percent"]
```

Le balisage des sites web évolue avec le temps. Considérez ces exemples comme des
points de départ, et non comme des contrats permanents.

### Découvrir des sélecteurs

Chaque champ de sélecteur possède un bouton **Discover** correspondant.

1. Complétez l’URL de la source et approuvez le site web si une autorisation est requise.
2. Sélectionnez le bouton de découverte correspondant au type exact de valeur.
3. SmartTicker télécharge le HTML statique public et répertorie les sélecteurs possibles,
	 avec un exemple de valeur, un pourcentage de confiance et la raison dans l’infobulle.
4. Sélectionnez **Use** à côté d’une suggestion pour la copier dans le champ correspondant.
5. Validez ou observez le résultat avant de vous y fier.

La découverte n’exécute pas JavaScript, ne se connecte pas, ne contourne pas les
contrôles d’accès et n’inspecte pas votre navigateur. Une valeur produite uniquement par
JavaScript peut ne posséder aucun sélecteur détectable. Les types de découverte distincts
évitent volontairement de mélanger les valeurs de préouverture et d’après-clôture.

### Valider une source

**Validate URL** demande la page et indique le prix normal et/ou le nombre de titres
qu’il peut lire. Vous pouvez l’utiliser sans risque avant de saisir un symbole, car
SmartTicker emploie une étiquette temporaire pour le test.

Cette validation ne vérifie pas actuellement les quatre champs de sélecteur de
préouverture et d’après-clôture. Utilisez les exemples de valeurs proposés par leur
découverte, puis confirmez les données de séance affichées.

Les échecs courants comprennent une erreur HTTP, un délai d’attente, une valeur
manquante, aucun titre, une autorisation de source non approuvée, du contenu accessible
uniquement par JavaScript ou un sélecteur obsolète.

### Limite de répétition des actualités

**Show max _N_ times** accepte les valeurs de 1 à 100 et utilise 5 par défaut.
SmartTicker compte une affichage à chaque cycle complet d’actualisation des actualités
où le même titre est renvoyé. Dès que le titre est apparu pendant le nombre de cycles
configuré, il est retiré pour le reste de la session actuelle de l’application. Modifier
ou supprimer cette entrée efface son historique de répétition.

### Modifier, classer et supprimer des entrées

La liste **Configured entries** affiche le symbole, le groupe, la source, l’URL, les
badges de collecte, le sélecteur de prix normal, le sélecteur d’actualités et la limite
de répétition des actualités.

- **Edit** charge l’entrée dans le formulaire. Sélectionnez **Save changes** pour
	l’appliquer ou **Cancel edit** pour abandonner les modifications du formulaire.
- Les boutons fléchés vers le haut et vers le bas modifient l’ordre du bandeau et
	l’enregistrent immédiatement.
- **Remove** supprime l’entrée et ses données actuellement affichées.
- Si des règles d’alerte ciblent l’entrée, SmartTicker vous demande si vous souhaitez
	supprimer ces règles. Une alerte sans cotation configurée correspondante ne peut pas
	se déclencher.
- Renommer une entrée met à jour les symboles affichés des règles d’alerte qui y sont
	associées.

## Paramètres de l’application

Ouvrez **App Settings...** depuis le menu contextuel. Les modifications prennent effet
et sont enregistrées automatiquement ; il n’existe aucun bouton Apply.

### Lignes et vitesse du bandeau

| Paramètre | Choix | Valeur par défaut | Effet |
| --- | --- | --- | --- |
| Lignes de prix (**Price rows**) | 1 à 8 | 1 | Nombre de lignes parallèles du bandeau de prix. |
| Vitesse de défilement des prix (**Price scroll speed**) | 20, 30, 40, 50, 65, 80, 100 ou 120 px/sec | 50 | Vitesse du bandeau de prix. |
| Lignes d’actualités (**News rows**) | 1 à 8 | 1 | Nombre de lignes parallèles du bandeau de titres. |
| Vitesse de défilement des actualités (**News scroll speed**) | 20, 30, 40, 50, 65, 80, 100 ou 120 px/sec | 40 | Vitesse du bandeau d’actualités. |
| Taille de police défilante (**Scrolling font size**) | 9 à 24 pt | 14 pt | Texte des prix et des actualités dans les lignes défilantes. |
| Taille de police statique (**Static font size**) | 9 à 24 pt | 13 pt | Texte des cotations et des titres dans les lignes statiques. |
| Actualisation des prix (**Price refresh**) | 30 à 300 seconds, par pas de 15-second | 60 seconds | Durée pendant laquelle chaque entrée de prix autorisée reçoit une actualisation planifiée. |
| Actualisation des actualités (**News refresh**) | 30 à 300 seconds, par pas de 15-second | 300 seconds | Durée pendant laquelle chaque entrée d’actualités autorisée reçoit une actualisation planifiée. |

Les lignes de prix et la vitesse de défilement des prix sont désactivées lorsque les
tableaux groupés statiques sont actifs, car ce mode affiche toutes les entrées de prix
et ne fait jamais défiler automatiquement l’une ou l’autre fenêtre. Les paramètres de
lignes et de vitesse des actualités sont conservés pour l’affichage défilant.

Les requêtes de prix et d’actualités sont réparties indépendamment sur des créneaux
d’une seconde pendant la totalité de leur intervalle, au lieu de démarrer ensemble. Par
exemple, 60 entrées sur 30 secondes programment deux entrées par seconde ; cinq entrées
sur 30 secondes en programment une environ toutes les six secondes. Quatre requêtes de
source au maximum s’exécutent simultanément, le travail en double pour une même entrée
et un même flux est ignoré, et les créneaux manqués ne sont pas rejoués en rafale.
**Refresh prices now** ou **Refresh news now** redémarre uniquement le flux concerné et
demande son premier créneau. Les prix et titres déjà récupérés restent visibles pendant
la lecture des données de remplacement.

Chaque requête HTTP possède un délai d’attente fixe de 20-second. Une source lente ne
bloque pas le répartiteur de l’interface et n’empêche pas les créneaux ultérieurs
d’utiliser la capacité de requête restante. SmartTicker signale les échecs tels que
HTTP 403 et 429 et ne contourne pas les restrictions. Il n’analyse ni n’applique
automatiquement les directives robots, les valeurs crawl-delay ou les instructions de
temporisation du serveur ; choisissez donc des sources conformes et évitez les requêtes
inutilement fréquentes.

### Tailles des fenêtres

Les paramètres de l’application enregistrent trois paires de dimensions indépendantes :

| Fenêtre | Largeur | Hauteur | Valeur par défaut |
| --- | --- | --- | --- |
| Affichage défilant | 420–7680 px | 50–900 px | 980 × 64 px |
| Affichage statique des prix | 420–7680 px | 420–4320 px | 980 × 420 px |
| Affichage statique des actualités | 420–7680 px | 240–4320 px | 680 × 340 px |

La modification d’une valeur s’applique immédiatement lorsque la fenêtre ou la vue
concernée est active. L’exemple publié utilise un affichage défilant de 1200 × 96, des
prix statiques de 1200 × 720 et des actualités statiques de 760 × 480, avec du texte
défilant en 15-point et du texte statique en 14-point. Une hauteur défilante inférieure
à l’espace requis par les lignes activées est automatiquement portée au minimum nécessaire.

Utilisez les quatre choix sous **View** pour décider si les actualités s’affichent et si
la disposition défile ou reste statique. Changer de vue ne supprime jamais les entrées
configurées.

### Démarrer SmartTicker lors de la connexion

Activez **Start SmartTicker when I sign in** pour enregistrer l’exécutable installé
uniquement pour l’utilisateur actuel.

- Sous Windows, SmartTicker utilise la clé de Registre `Run` de l’utilisateur actuel.
- Sur les bureaux Linux compatibles avec la convention de démarrage automatique
	freedesktop, SmartTicker écrit `smartticker.desktop` dans le répertoire de démarrage
	automatique de l’utilisateur.
- L’option est désactivée sur les plateformes pour lesquelles SmartTicker ne dispose
	d’aucun mécanisme d’enregistrement pris en charge.

Le système d’exploitation fait autorité. Si le démarrage est modifié en dehors de
SmartTicker, la case à cocher reflète l’état du système d’exploitation au prochain
chargement des paramètres.

### Accès aux sites web

**Allow website cookies and cross-host redirects** est désactivé par défaut.

Lorsque cette option est désactivée :

- SmartTicker exige une approbation explicite pour chaque hôte de site web avant de
	le solliciter.
- Les cookies de sites web ne sont pas acceptés.
- Les redirections vers un autre hôte sont bloquées.
- Les hôtes approuvés sont mémorisés dans les paramètres locaux.

Lorsque cette option est activée :

- SmartTicker ignore son étape d’approbation par hôte.
- Les cookies définis par les sites web demandés sont conservés uniquement dans un
	conteneur isolé en mémoire et disparaissent à la fermeture de SmartTicker.
- Les redirections vers d’autres hôtes peuvent être suivies.
- SmartTicker ne lit toujours pas les cookies du navigateur, ne transmet pas
	d’identifiants et n’envoie pas de formulaires de connexion.

Désactiver cette option supprime les données actuellement affichées provenant de
sources non approuvées jusqu’à l’approbation et l’actualisation de ces hôtes.

#### Choix de confidentialité du site web

Si une réponse est reconnue comme un formulaire de confidentialité ou de cookies
contenant à la fois des choix positifs et négatifs, SmartTicker se met en pause et
affiche le titre de la page, l’URL demandée, l’URL de consentement, un résumé du
formulaire et les libellés Accept/Reject du site web.

- **Accept** envoie les champs masqués fournis par ce formulaire ainsi que la commande
	Accept exacte que vous avez sélectionnée.
- **Reject** envoie ces champs masqués ainsi que la commande Reject exacte que vous
	avez sélectionnée.
- **Cancel** n’envoie rien.

Il s’agit du choix de confidentialité d’un site web, et non de l’approbation des
autorisations par source de SmartTicker.

#### Valider toutes les sources

Sélectionnez **Validate all sources** pour examiner et tester chaque entrée configurée.

1. Si l’accès aux sites web est restreint, SmartTicker regroupe les entrées non approuvées
	 par nom d’hôte et affiche une boîte de dialogue d’examen de la source par hôte.
2. Examinez l’hôte, le résumé de la politique, les recommandations, les noms des sources
	 et les symboles.
3. Cochez la confirmation uniquement si vous avez examiné le site web et êtes autorisé
	 à l’utiliser.
4. Choisissez **Approve this source**, **Skip this source** ou **Cancel validation**.
5. SmartTicker teste chaque entrée autorisée et indique les totaux réussis, échoués et
	 ignorés. Les problèmes individuels apparaissent sous la ligne d’état.

Les enregistrements d’approbation consignent l’autorisation dans SmartTicker ; ils
n’accordent aucun droit légal et ne prévalent pas sur les conditions du site web.

### Apparence

**Window transparency** modifie uniquement l’arrière-plan du bandeau. Le texte reste
opaque. La plage va de 20% à 100%, par pas de 5%, et la valeur par défaut est 100%.

Les champs de couleur acceptent les valeurs hexadécimales `#RRGGBB` et proposent
également un sélecteur de couleur.

| Couleur | Valeur par défaut | Utilisation |
| --- | --- | --- |
| Arrière-plan (**Background**) | `#10151D` | Arrière-plan du bandeau avant l’application de la transparence. |
| Nom de la cotation (**Quote name**) | `#79C0FF` | Étiquette du symbole/de la source. |
| Cours de clôture (**Close price**) | `#FFA657` | Prix normal. |
| Hors séance (**After hours**) | `#00E5FF` | Prix de préouverture et d’après-clôture. |
| Actualité 1 (**News 1st**) | `#FFFFFF` | Titres 1, 5, 9, et ainsi de suite. |
| Actualité 2 (**News 2nd**) | `#00E5FF` | Titres 2, 6, 10, et ainsi de suite. |
| Actualité 3 (**News 3rd**) | `#A3E635` | Titres 3, 7, 11, et ainsi de suite. |
| Actualité 4 (**News 4th**) | `#79C0FF` | Titres 4, 8, 12, et ainsi de suite. |
| Hausse (**Change up**) | `#3FB950` | Variations positives en pourcentage. |
| Baisse (**Change down**) | `#F85149` | Variations négatives en pourcentage. |
| Clignotement d’alerte (**Alert blink**) | `#FF00FF` | Alertes de prix déclenchées, en alternance avec le noir. |

**Reset to defaults** rétablit toutes les couleurs ci-dessus et une opacité
d’arrière-plan de 100%. Cette commande ne réinitialise pas les lignes, les vitesses, les
tailles de police, les dimensions des fenêtres, les sources, les intervalles
d’actualisation, les alertes ou la langue.

### Sauvegarde et restauration

SmartTicker conserve les paramètres de l’application et les règles d’alerte dans des
fichiers JSON distincts et fournit des boutons séparés pour chaque type de sauvegarde.

#### Exporter et importer les paramètres

- **Export settings...** écrit les entrées configurées, les affectations et définitions
	de groupes, les cotations d’actualités masquées, l’ordre des entrées, les sélecteurs,
	le choix d’affichage défilant/statique des cotations, les hôtes approuvés, la visibilité
	des lignes, les lignes, les vitesses, les tailles de police défilante/statique, les
	trois paires de dimensions des fenêtres, les intervalles d’actualisation, la préférence
	de démarrage, l’option d’accès aux sites web, les couleurs, y compris celle du
	clignotement d’alerte, la transparence et la langue.
- **Import settings...** valide la totalité du fichier avant toute modification. Un
	fichier rejeté laisse les paramètres actuels inchangés.
- Une importation réussie remplace chaque entrée configurée et chaque préférence de
	l’application. Elle ne remplace pas le fichier distinct des règles d’alerte.
- Les groupes sont inclus dans le fichier de paramètres à la fois comme affectations
	de cotations et comme définitions de groupes ; un groupe sans cotation est donc lui
	aussi conservé dans une sauvegarde. Il n’existe aucun fichier distinct d’exportation
	ou d’importation réservé aux groupes.
- La préférence de démarrage figure dans une sauvegarde des paramètres, mais son
	importation ne modifie pas silencieusement l’enregistrement du démarrage dans le
	système d’exploitation. Le système d’exploitation reste l’autorité ; utilisez la case
	Startup pour modifier l’enregistrement sur l’ordinateur actuel.
- Les fichiers d’importation sont limités à 1 MiB, à la version 1 du schéma et à
	200 abonnements au maximum. Les propriétés inconnues, les ID en double, les URL mal
	formées, les couleurs ou plages invalides et les codes de langue non pris en charge
	sont rejetés au lieu d’être ignorés silencieusement.

#### Exporter et importer les règles d’alerte- **Export alert rules...** écrit toutes les règles ainsi que Buzz, le nombre de buzz et la durée de clignotement.
- **Import alert rules...** valide la totalité du fichier, puis remplace toutes les
	règles actuelles et les paramètres de déclenchement des alertes.
- Les règles se reconnectent d’abord par ID d’abonnement. Lorsque les ID diffèrent,
	SmartTicker tente une correspondance du symbole sans distinction entre majuscules et
	minuscules.
- Une règle importée sans cotation correspondante est conservée, mais ne peut pas se
	déclencher. L’état de l’importation indique combien de règles ont été réassociées ou
	restent sans correspondance.
- Les fichiers d’importation des alertes sont limités à 1 MiB.

Pour un transfert vers un autre ordinateur, importez d’abord les paramètres de
l’application, puis les règles d’alerte. Importer les alertes en second permet aux
règles de se reconnecter aux nouveaux ID d’abonnement par symbole.

### Modifier directement les fichiers de configuration

**Edit Current App Config** et **Edit Current Alert Rules** dans les paramètres de
l’application ouvrent le fichier JSON actif dans l’éditeur de texte associé à `.json`
par votre système. Cette fonction est destinée aux utilisateurs avancés ; les fenêtres
de SmartTicker couvrent les mêmes paramètres sans risque.

Les deux boutons affichent d’abord une confirmation vous demandant d’exporter le fichier
actuel. Effectuez cette exportation : une modification manuelle peut endommager le
fichier et il n’existe aucune fonction d’annulation.

- **Export existing config...** enregistre le fichier actuel, puis revient à la même invite.
- **Open in text editor** ouvre le fichier actif.
- **Cancel** ne modifie rien.

SmartTicker surveille le fichier et le recharge dès que votre éditeur l’enregistre :

- Un fichier valide est appliqué immédiatement et le bandeau se met à jour sans redémarrage.
- Un JSON mal formé, une violation du schéma ou toute autre erreur de validation est
	rejeté. Votre configuration en cours d’exécution reste intacte et la fenêtre des
	paramètres de l’application signale le problème.
- Après le rejet d’une modification, corrigez le fichier ou restaurez une exportation
	valide avec **Import settings...** ou **Import alert rules...**.
- Un fichier qui reste verrouillé par un autre programme fait l’objet de quelques
	nouvelles tentatives, puis l’erreur est signalée.

La modification du fichier des règles d’alerte suit les mêmes règles et n’affecte pas
les paramètres de l’application, car les deux fichiers sont distincts.

## Règles d’alerte

Ouvrez **Alerts** depuis le menu contextuel. Les règles sont évaluées après chaque
actualisation réussie du prix et surveillent uniquement le prix normal, pas les valeurs
de préouverture ou d’après-clôture.

### Créer une règle

1. Sélectionnez une **Quote** configurée. Les entrées ayant le même symbole restent
	 distinctes.
2. Sélectionnez une **Condition** et saisissez un seuil numérique utilisant un nombre
	 décimal invariant tel que `250.50`.
3. Choisissez éventuellement **Active from**. Laissez ce champ vide pour une activation
	 immédiate.
4. Laissez **Never expires** coché, ou décochez-le et choisissez une date d’expiration.
5. Sélectionnez **Add rule**.

Les comparaisons disponibles sont les suivantes :

| Choix | Signification |
| --- | --- |
| `LessThan` | Prix `<` au seuil. |
| `LessThanOrEqual` | Prix `<=` au seuil. |
| `GreaterThan` | Prix `>` au seuil. |
| `GreaterThanOrEqual` | Prix `>=` au seuil. |
| `EqualTo` | Prix exactement égal au seuil. |
| `NotEqualTo` | Prix différent du seuil. |

La limite de début est inclusive. La limite d’expiration l’est également ; une fois
dépassée, la règle ne se déclenche plus. SmartTicker refuse une expiration antérieure
au début.

### Lorsqu’une règle se déclenche

Une règle activée et planifiée se déclenche une fois lorsque sa condition passe de
fausse à vraie. Elle n’émet pas de notification à chaque actualisation tant que la
condition reste vraie. Lorsque le prix sort de la condition, la règle se réarme et peut
se déclencher lorsque le prix y entre de nouveau.

Modifier une règle, ou la désactiver puis la réactiver, la réarme également. Une règle
activée peut donc se déclencher immédiatement si le prix normal le plus récent satisfait
déjà sa condition. Un prix manquant ou en échec ne peut pas déclencher de règle.

Lorsqu’une ou plusieurs règles se déclenchent :

- L’entrée de prix concernée alterne entre la couleur de clignotement d’alerte configurée
	et le noir pendant la durée configurée. La couleur par défaut est le magenta
	(`#FF00FF`).
- Si **Buzz** est activé, SmartTicker joue la séquence de buzz configurée.
- Le message d’alerte identifie une règle ou indique le nombre de règles déclenchées
	ensemble.
- Le bandeau continue de défiler pendant que le surlignage d’alerte est actif.

### Paramètres de sortie des alertes

| Paramètre | Plage | Valeur par défaut |
| --- | --- | --- |
| **Buzz** | Activé ou désactivé | Activé |
| Nombre de buzz (**Buzz count**) | 1 à 20 | 15 |
| **Blink for** | 5 à 900 seconds, par pas de 15-second | 60 seconds |

Désactiver Buzz laisse l’alerte visuelle active. Si plusieurs règles se déclenchent au
cours de la même évaluation, SmartTicker lance une seule séquence de buzz configurée
pour cette évaluation. Modifiez **Alert blink** sous **App Settings > Appearance**. Il
s’agit d’une préférence d’apparence de l’application ; l’exportation/importation des
paramètres l’inclut donc, contrairement au fichier distinct des règles d’alerte.

### Gérer les règles configurées

- **Edit** charge une règle dans le formulaire. Sélectionnez **Update rule** pour
	l’enregistrer ou **Cancel** pour la laisser inchangée.
- **Disable** conserve la règle, mais l’empêche de correspondre. **Enable** la réarme et
	l’évalue par rapport au dernier prix normal.
- **Remove** supprime la règle.
- La liste affiche l’état d’activation, le symbole, un résumé de la condition et la
	planification.

Les modifications des règles d’alerte et des paramètres de sortie sont enregistrées
automatiquement.

## Fichiers locaux et confidentialité

SmartTicker stocke la configuration localement et ne la synchronise pas avec un service
du développeur.

Sous Windows, les fichiers par défaut sont :

```text
%LocalAppData%\SmartTicker\settings.json
%LocalAppData%\SmartTicker\alerts.json
```

Sous Linux, .NET utilise le répertoire local des données d’application de l’utilisateur
actuel, normalement :

```text
~/.local/share/SmartTicker/settings.json
~/.local/share/SmartTicker/alerts.json
```

### Utiliser un répertoire de données isolé

Les diagnostics avancés et les exécutions de test peuvent définir
`SMARTTICKER_DATA_DIRECTORY` avant de lancer SmartTicker. Lorsque la valeur n’est pas
vide, les deux fichiers sont placés directement dans ce répertoire résolu sous les noms
`settings.json` et `alerts.json` ; les emplacements par défaut de la plateforme ci-dessus
ne sont pas utilisés pour ce processus. Préférez un chemin absolu et vérifiez qu’il est
accessible en écriture.

Exemple PowerShell :

```powershell
$env:SMARTTICKER_DATA_DIRECTORY = 'D:\SmartTicker-Profile'
& 'C:\Program Files\SmartTicker\SmartTicker.Desktop.exe'
```

Exemple de shell Linux :

```bash
SMARTTICKER_DATA_DIRECTORY="$HOME/.local/share/SmartTicker-Test" smartticker
```

Définissez la variable avant le démarrage du processus. SmartTicker ne copie pas le
profil par défaut dans le répertoire sélectionné ; un répertoire vide démarre donc avec
une configuration vide. Les instances qui utilisent le même répertoire peuvent voir
les modifications enregistrées les unes par les autres. Utilisez les commandes normales
d’exportation/importation des paramètres et des règles d’alerte pour les sauvegardes et
le transfert de profils.

La fenêtre des alertes affiche le chemin exact du fichier d’alertes utilisé. Les
écritures passent par un fichier temporaire suivi d’un remplacement, afin qu’un fichier
partiellement écrit ne soit pas considéré comme la configuration actuelle.

SmartTicker ne possède aucun compte, aucune télémétrie, aucune analyse, aucune publicité
et aucune synchronisation dans le cloud. Lorsqu’il demande une source, le site web reçoit
les informations réseau habituelles, comme votre adresse IP. L’ouverture de l’aide
demande le guide brut à GitHub. Pour tous les détails, consultez `PRIVACY.md` dans le dépôt.

Il vous appartient de vous assurer que chaque URL et chaque sélecteur de source est
utilisé conformément aux conditions, à la licence, aux directives robots du site web
et à la législation applicable.

## Dépannage

### Une cotation est indisponible ou n’affiche aucun prix

Une requête de source expire après 20-second. Si cette cotation dispose d’un instantané
antérieur réussi, une actualisation échouée le laisse visible ; sinon, la cotation
affiche **Unavailable** jusqu’à la réussite d’une actualisation ultérieure. Lisez
l’erreur de validation ou d’actualisation avant de modifier les sélecteurs.

1. Ouvrez **Quotes...**, modifiez l’entrée et vérifiez Full URL.
2. Vérifiez que **Price** est sélectionné.
3. Approuvez le site web si vous y êtes invité.
4. Sélectionnez **Validate URL** et lisez son résultat exact.
5. Exécutez **Discover price**, ou examinez le HTML statique de la page et mettez à jour
	 le sélecteur.
6. Vérifiez si la page nécessite JavaScript, une authentification ou un consentement que
	 SmartTicker ne peut pas gérer en toute sécurité.
7. Respectez les codes HTTP 403 et 429, les restrictions robots et la politique d’accès
	 automatisé du site.

### Les données de préouverture ou d’après-clôture sont absentes

- La séance de marché correspondante n’est peut-être pas active.
- La page peut omettre l’élément de séance lorsqu’il n’existe aucune valeur pour celle-ci.
- Vérifiez que les sélecteurs de préouverture ciblent les éléments de préouverture et que
	les sélecteurs d’après-clôture ciblent les éléments d’après-marché.
- Exécutez de nouveau la commande de découverte correspondante, car le balisage du site
	web peut avoir changé.

### Les actualités sont vides

- Vérifiez que **News** est sélectionné.
- Validez la source et exécutez **Discover news**.
- Vérifiez que le sélecteur renvoie des liens contenant un texte de titre visible.
- Lorsqu’ils sont disponibles, une requête d’actualités échouée ou expirée conserve les
	titres récupérés précédemment. Une source sans résultat réussi reste vide jusqu’à la
	réussite d’un créneau ultérieur.
- Un titre disparaît après avoir atteint sa limite de répétition configurée pour cette
	session.
- Dans la fenêtre statique des actualités, vérifiez que la cotation voulue est cochée
	sous **Show news for**.

### La découverte de sélecteur ne trouve rien

La découverte lit uniquement le HTML statique téléchargé. Elle ne peut pas voir les
valeurs créées ultérieurement par le JavaScript de la page. Saisissez manuellement un
sélecteur vérifié, choisissez une page ou un flux statique, ou utilisez une API documentée
et autorisée par l’intermédiaire d’une page publique compatible.

### Une alerte ne se déclenche pas

- Vérifiez que la cotation associée existe toujours, collecte Price et dispose d’un
	prix normal récupéré avec succès.
- Vérifiez que la règle est Enabled et se trouve dans sa période de début/expiration.
- Vérifiez la comparaison et le seuil. `EqualTo` exige une égalité décimale exacte.
- N’oubliez pas qu’une condition continuellement vraie se déclenche une seule fois ;
	elle doit devenir fausse avant de pouvoir se déclencher à nouveau, sauf si vous
	modifiez ou réactivez la règle.
- Les prix de préouverture et d’après-clôture ne pilotent pas les règles d’alerte.

### SmartTicker ne peut pas être déplacé ou redimensionné

- Effectuez le déplacement uniquement depuis la poignée à points verticaux de la bande
	de gauche.
- Redimensionnez depuis un bord ou un angle ; utilisez le repère inférieur droit visible
	si un bord est difficile à atteindre.
- Le contenu du bandeau n’est volontairement pas une zone de déplacement.

### Les groupes ou les valeurs statiques ne correspondent pas à mes attentes

- Ouvrez **Quotes...** et vérifiez la valeur Group de chaque entrée.
- Ouvrez **Quote groups...** pour gérer les définitions de groupes et examiner
	l’association actuelle de chaque cotation.
- Les entrées dont le champ Group est vide apparaissent sous **Ungrouped**.
- **Chg** est calculé à partir de Last et Chg% ; il n’est pas extrait séparément de la
	page. Il reste à `—` lorsque le pourcentage est indisponible.
- Réorganisez les entrées à l’aide des commandes haut/bas pour modifier l’ordre des
	groupes et des lignes.
- Faites glisser la poignée pointillée de l’en-tête d’une vignette pour déplacer tout le
	groupe. Déposez-la sur la moitié gauche d’une autre vignette pour le placer avant, ou
	sur la moitié droite pour le placer après.
- Sélectionnez **Refresh prices now** lorsque SmartTicker n’est pas en pause pour mettre
	le tableau à jour.

### Le texte d’aide n’est pas mis en forme ou la navigation ne fonctionne pas

- La fenêtre d’aide doit afficher des titres, paragraphes, listes, tableaux, liens et
	blocs de code mis en forme, plutôt que la ponctuation Markdown.
- Utilisez **On this page** à gauche pour accéder à une section principale. Les liens du
	tableau de navigation rapide font également défiler le document.
- Fermez puis rouvrez l’aide, ou changez de **Language**, pour demander le guide en ligne
	publié correspondant. Dans l’attente de son chargement, SmartTicker affiche le guide
	intégré correspondant, mis en forme dans l’application installée.

### L’aide en ligne est indisponible ou obsolète

- Fermez puis rouvrez l’aide pour demander de nouveau le guide publié.
- Ouvrez dans un navigateur l’adresse GitHub brute indiquée au début de ce guide pour
	consulter directement le fichier publié.
- SmartTicker utilise le guide intégré lorsque la requête échoue ou renvoie un fichier vide.
- Les modifications en ligne n’apparaissent qu’après la publication de `HELPME.md` ou
	du fichier localisé correspondant `help/HELPME.fr.md` sur la branche `main` du dépôt.

## Assistance

Signalez les problèmes reproductibles à l’adresse suivante :

<https://github.com/bulentozkir/smartticker/issues>

Indiquez la version de SmartTicker, le système d’exploitation, le nom d’hôte de la
source, l’état de validation et le texte exact de l’erreur. Supprimez les URL privées
ou toute autre information sensible avant de publier.