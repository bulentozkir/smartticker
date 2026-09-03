# Ayuda de SmartTicker

Esta guía corresponde a SmartTicker 1.0.3. Explica el teletipo principal, la configuración
de la aplicación, las cotizaciones, las reglas de alerta, los permisos de los sitios web,
las copias de seguridad y los problemas habituales.

SmartTicker lee el HTML estático público de las páginas web que usted configure. No
proporciona un flujo de datos de mercado, y la información extraída puede llegar con
retraso, estar incompleta o ser incorrecta. Verifique la información financiera
importante con una fuente autorizada.

## Navegación rápida

| Área | Ir a |
| --- | --- |
| Primeros pasos | [Abrir la ayuda y las ventanas de configuración](#abrir-la-ayuda-y-las-ventanas-de-configuración) |
| Teletipo principal | [Controles](#controles-del-teletipo-principal) · [Vista con desplazamiento o estática](#elegir-la-vista-de-cotizaciones-con-desplazamiento-o-estática) · [Mover](#mover-el-teletipo) · [Cambiar tamaño](#cambiar-el-tamaño-del-teletipo) · [Pausar](#pausar-y-reanudar) · [Referencia del menú](#referencia-del-menú-principal) |
| Cotizaciones y noticias | [Cotizaciones](#cotizaciones) · [Añadir una entrada](#añadir-una-entrada-de-cotización-o-noticias) · [Agrupar cotizaciones](#agrupar-cotizaciones) · [URL de origen](#valores-predefinidos-de-origen-y-url) · [Selectores](#referencia-de-los-campos-de-selector) · [Detección](#detectar-selectores) · [Validación](#validar-un-origen) |
| Preferencias de la aplicación | [Configuración de la aplicación](#configuración-de-la-aplicación) · [Filas y velocidad](#filas-y-velocidad-del-teletipo) · [Inicio](#iniciar-smartticker-al-iniciar-sesión) · [Acceso a sitios web](#acceso-a-sitios-web) · [Apariencia](#apariencia) · [Copia de seguridad y restauración](#copia-de-seguridad-y-restauración) · [Editar archivos de configuración](#editar-directamente-los-archivos-de-configuración) |
| Alertas de precios | [Reglas de alerta](#reglas-de-alerta) · [Crear una regla](#crear-una-regla) · [Comportamiento al activarse](#cuando-se-activa-una-regla) · [Salida de alertas](#configuración-de-salida-de-alertas) · [Administrar reglas](#administrar-las-reglas-configuradas) |
| Datos y soporte | [Archivos locales y privacidad](#archivos-locales-y-privacidad) · [Solución de problemas](#solución-de-problemas) · [Soporte](#soporte) |

## Abrir la ayuda y las ventanas de configuración

Haga clic con el botón derecho en el teletipo para abrir su menú. Los principales
comandos de configuración son:

- **Cotizaciones... (Quotes...)**: añadir, probar, editar, ordenar y eliminar orígenes
	de cotizaciones o noticias.
- **Grupos de cotizaciones... (Quote groups...)**: crear, actualizar o eliminar grupos
	y asociarles cotizaciones.
- **Alertas (Alerts)**: crear y administrar reglas de alerta de precios.
- **Configuración de la aplicación... (App Settings...)**: configurar filas,
	velocidades, intervalos de actualización, inicio, acceso a sitios web, colores,
	transparencia y copias de seguridad.
- **Vista (View)**: seleccionar una de cuatro combinaciones mutuamente excluyentes:
	con desplazamiento o estática, solo con precios o con precios y noticias.
- **Ayuda (Help)**: abrir esta guía dentro de SmartTicker.
- **Acerca de SmartTicker (About SmartTicker)**: mostrar la versión instalada y el
	aviso de licencia.
- **Salir (Exit)**: cerrar SmartTicker por completo.

La ventana de Ayuda da formato y muestra de inmediato la guía integrada correspondiente
al idioma seleccionado de la aplicación. Después, consulta la guía en línea del mismo
idioma cada vez que se abre la Ayuda o se cambia el **Idioma (Language)**. La guía en
línea en español es:

<https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/help/HELPME.es.md>

Si falla la solicitud en línea, SmartTicker mantiene en pantalla la traducción integrada
correspondiente. Al cambiar el **Idioma (Language)**, el título, el estado, la navegación
y la guía completa de una ventana de Ayuda abierta se actualizan de inmediato. Cierre
la Ayuda con el control de cierre normal de su barra de título.

## Controles del teletipo principal

### Elegir la vista de cotizaciones con desplazamiento o estática

SmartTicker ofrece cuatro modos de visualización mutuamente excluyentes. Haga clic con
el botón derecho en el teletipo, abra **Vista (View)** y seleccione uno. El diseño cambia
de inmediato y la elección queda guardada.

| Opción de vista | Resultado |
| --- | --- |
| **Desplazamiento de izquierda a derecha: solo precios (Left-to-right scroll: Prices only)** | Marquesina de precios en el teletipo principal, sin noticias. Esta es la opción predeterminada. |
| **Desplazamiento de izquierda a derecha: precios con noticias (Left-to-right scroll: Prices with News)** | Marquesinas de precios y noticias en el teletipo principal. |
| **Vista estática: solo precios (Static view: Prices only)** | Mosaicos adaptables de precios en la ventana principal, sin ventana de Noticias. |
| **Vista estática: precios con noticias (Static view: Prices with News)** | Mosaicos adaptables de precios más una ventana estática independiente de **Noticias de SmartTicker (SmartTicker News)**. |

Los archivos de configuración creados antes de que se añadieran estas opciones se
asignan a la combinación equivalente de sus ajustes guardados de desplazamiento o
vista estática y de noticias. El modo de visualización se administra únicamente desde
el menú **Vista (View)** que se abre con el botón derecho en el teletipo.

- En cualquiera de los modos con desplazamiento, los precios usan la marquesina
	horizontal y el número de filas de precios y la velocidad de desplazamiento configurados.
- En cualquiera de los modos estáticos, los grupos aparecen como mosaicos adaptables
	dispuestos de izquierda a derecha. Los mosaicos pasan a otra fila solo cuando la
	ventana es demasiado estrecha. Los precios no se mueven automáticamente.
- Cada mosaico de cotización tiene sus propias columnas alineadas **Símbolo (Symbol)**,
	**Último (Last)**, **Cambio (Chg)** y **Cambio % (Chg%)**. **Cambio (Chg)** se calcula
	a partir de Last y Chg%, porque las páginas de origen proporcionan un selector de
	porcentaje en lugar de un selector independiente para el cambio absoluto. Muestra
	`—` cuando alguno de los valores no está disponible.
- Seleccione el encabezado de un grupo para contraerlo o expandirlo. Los grupos siguen
	la primera aparición de sus cotizaciones en el orden de las entradas configuradas;
	las filas de cada grupo conservan ese orden.
- Las entradas sin grupo aparecen en **Sin agrupar (Ungrouped)**.
- Mantenga el puntero sobre Last para ver los valores disponibles de premercado y fuera
	de horario. Haga doble clic en una fila de cotización para abrir su página de origen.
- El parpadeo de alertas y los colores de subida y bajada funcionan en ambos modos de precios.
- Las noticias se abren automáticamente en una ventana independiente de **Noticias de
	SmartTicker (SmartTicker News)** que contiene mosaicos estáticos de grupos con
	**Símbolo / Titular (Symbol / Headline)**. En modo estático no se muestran en una
	marquesina. La ventana de Noticias tiene una barra de título y un borde de cambio de
	tamaño normales, por lo que las ventanas de Cotizaciones y Noticias se pueden mover
	de forma independiente a monitores diferentes. Haga doble clic en la fila de un
	titular para abrir su origen.
- En el primer inicio, Noticias usa un tamaño compacto de 680×340. SmartTicker la sitúa
	en otro monitor cuando hay uno disponible; con un solo monitor, primero busca un área
	libre debajo, a la derecha, encima o a la izquierda de Precios. Después puede moverla
	y cambiar su tamaño con normalidad.
- Dentro de cada grupo de Noticias, los titulares se intercalan por cotización: un
	titular de la primera cotización, luego uno de la siguiente, y así sucesivamente en
	rondas. De este modo, una cotización con muchos titulares no puede ocupar toda la
	parte superior de su grupo.
- Abra la lista desplegable de una línea **Mostrar noticias de (Show news for)** y marque
	o desmarque cada cotización por separado. Puede mostrarse cualquier combinación de
	cotizaciones, incluidas todas o ninguna. El botón resume la selección actual, y las
	entradas incluyen la cotización y el origen para que los símbolos duplicados sigan
	siendo independientes. Las cotizaciones desmarcadas se guardan en el archivo de
	configuración como `hiddenNewsQuotes`, por lo que se conservan tras reiniciar y se
	incluyen en las copias de seguridad de la configuración.
- Arrastre el control de puntos situado junto al encabezado de cualquier mosaico de
	cotización o noticias y suéltelo sobre la mitad izquierda o derecha de otro mosaico.
	El orden cambia en ambas ventanas y se guarda reordenando las entradas configuradas
	subyacentes.
- Un grupo con muchas filas se desplaza dentro de su propio mosaico de tamaño limitado.
	La vista completa se desplaza verticalmente solo cuando las filas de mosaicos que han
	pasado a otra línea no caben en la altura actual de la ventana.

Cerrar **Noticias de SmartTicker (SmartTicker News)** no desactiva la recopilación de
noticias. Para volver a abrirla, haga clic con el botón derecho en la ventana de Precios
y seleccione **Vista > Abrir ventana estática de noticias (View > Open static news
window)**. Al seleccionar **Vista estática: solo precios (Static view: Prices only)** se
cierra; al seleccionar **Vista estática: precios con noticias (Static view: Prices with
News)** se abre de nuevo. Cualquiera de las opciones con desplazamiento cierra la ventana
independiente de Noticias; la opción con desplazamiento de precios y noticias restaura
la marquesina de noticias en el teletipo principal.

Al cambiar de modo se aplica el tamaño guardado para esa vista. El teletipo con
desplazamiento, la ventana estática de Precios y la ventana estática de Noticias
conservan cada uno su propia anchura y altura.

### Mover el teletipo

Mantenga presionado el control de puntos verticales situado en la parte superior de la
estrecha franja izquierda, arrastre el teletipo y suelte el botón del ratón. El texto
del teletipo no es una superficie de arrastre, por lo que seleccionar o pulsar contenido
no puede iniciar accidentalmente el movimiento de la ventana.

### Cambiar el tamaño del teletipo

Mueva el puntero sobre cualquier borde o esquina hasta que aparezca un cursor de cambio
de tamaño; después, presione y arrastre. La esquina inferior derecha tiene una pequeña
marca visible de cambio de tamaño. La anchura mínima de la ventana es de 420 píxeles.
La altura con desplazamiento va de 50 a 900 píxeles, la altura de Precios estáticos va
de 420 a 4320 píxeles y la altura de Noticias estáticas va de 240 a 4320 píxeles.

El cambio de tamaño manual actualiza las dimensiones guardadas para la vista activa una
vez que termina el arrastre. Los tres pares de dimensiones se incluyen en una copia de
seguridad de la configuración. Las posiciones de las ventanas no se guardan. Si un
tamaño con desplazamiento es demasiado bajo para las filas de Precios o Noticias
seleccionadas y el tamaño de fuente con desplazamiento, SmartTicker aumenta
automáticamente la altura guardada. Por tanto, seleccionar **Desplazamiento de izquierda
a derecha: precios con noticias (Left-to-right scroll: Prices with News)** siempre deja
espacio para las filas de Noticias en lugar de ocultarlas silenciosamente.
Cuando se abre o se mueve una ventana, SmartTicker mantiene al menos su esquina superior
izquierda de 32 píxeles dentro del área de trabajo de una pantalla y limita las
coordenadas globales X e Y a un mínimo de 1. Así, el control de movimiento o la esquina
de la barra de título siguen accesibles con el ratón incluso después de desconectar un
monitor.

### Pausar y reanudar

Seleccione el botón de estado situado debajo del control de movimiento, o haga clic con
el botón derecho y seleccione **Pausar / Reanudar (Pause / Resume)**. Al pausar se
detienen las actualizaciones automáticas de precios y noticias y se congela la marquesina.
También se impide que cualquiera de los comandos de actualización manual inicie trabajo
nuevo. Una solicitud de origen que ya estuviera en curso no se cancela de forma forzada
únicamente por la pausa y puede terminar antes de que cese toda la actividad. Al reanudar
se reinician los temporizadores automáticos.

En Windows, SmartTicker establece automáticamente la prioridad de su proceso del sistema
operativo en **Baja (Low)** y activa el **modo de eficiencia (Efficiency mode)** de
Windows (EcoQoS) antes de iniciar la interfaz. No existe ningún ajuste de la aplicación
para este comportamiento. También utiliza una ruta de renderizado por software de baja
sobrecarga. La temporización de la marquesina se adapta a la velocidad configurada, y
una marquesina pausada, vacía o desconectada detiene su temporizador de animación. Las
filas sin cambios suprimen las notificaciones visuales redundantes. El parpadeo de las
alertas y el resaltado marrón de cambios durante tres segundos son intencionados y no
detienen el desplazamiento. En Linux, la planificación del proceso se deja al sistema
operativo. Si Windows rechaza alguno de los ajustes del proceso, SmartTicker informa del
fallo en el seguimiento de diagnóstico y continúa iniciándose.

### Abrir enlaces

Haga doble clic en el texto enlazado del teletipo, incluido un titular de noticias, para
abrir su origen en el navegador predeterminado. SmartTicker no abre enlaces con un solo clic.

### Resaltado de cambios

Después de cada actualización, SmartTicker marca brevemente sobre un fondo marrón, durante
tres segundos, los elementos que han cambiado:

- Una cotización cuyo precio difiere del de la sincronización anterior.
- Cada titular que no estaba presente en la sincronización anterior de esa cotización.

La primera sincronización después del inicio no resalta nada porque no existe un valor
anterior con el que comparar. Una alerta activada conserva su propio color de parpadeo
de alerta y tiene prioridad.

### Referencia del menú principal

| Comando | Efecto |
| --- | --- |
| **Actualizar precios ahora (Refresh prices now)** | Reinicia el ciclo escalonado de precios y solicita su primer intervalo cuando SmartTicker no está en pausa. |
| **Actualizar noticias ahora (Refresh news now)** | Reinicia el ciclo escalonado de Noticias y solicita su primer intervalo cuando SmartTicker no está en pausa. |
| **Pausar / Reanudar (Pause / Resume)** | Alterna las actualizaciones y el movimiento de la marquesina. |
| **Vista > Desplazamiento de izquierda a derecha: solo precios (View > Left-to-right scroll: Prices only)** | Usa únicamente la marquesina horizontal de precios. Esta es la opción predeterminada. |
| **Vista > Desplazamiento de izquierda a derecha: precios con noticias (View > Left-to-right scroll: Prices with News)** | Usa ambas marquesinas horizontales. |
| **Vista > Vista estática: solo precios (View > Static view: Prices only)** | Usa únicamente mosaicos estáticos y adaptables de cotizaciones. |
| **Vista > Vista estática: precios con noticias (View > Static view: Prices with News)** | Usa mosaicos de cotizaciones y la ventana estática independiente de Noticias. |
| **Vista > Abrir ventana estática de noticias (View > Open static news window)** | Vuelve a abrir la ventana independiente de Noticias después de cerrarla. Está disponible en modo estático cuando las noticias están activadas. |
| **Idioma (Language)** | Permite elegir uno de 16 idiomas para los menús, el texto de estado y la guía de Ayuda completa. Una ventana de Ayuda abierta se actualiza de inmediato. |

La visibilidad de las líneas, el idioma y los demás valores de configuración se guardan
automáticamente.

## Cotizaciones

Abra **Cotizaciones... (Quotes...)** desde el menú del botón derecho. Cada entrada
configurada representa un símbolo y una página web. Se permiten símbolos duplicados y
siguen siendo independientes porque cada entrada tiene su propio origen, selectores,
opciones de recopilación y alertas.

### Inicio rápido con el ejemplo publicado

Cuando no hay ninguna entrada, la ventana Cotizaciones ofrece **Importar cotizaciones
de ejemplo desde GitHub (Import sample quotes from GitHub)**. Esta acción descarga el
ejemplo del repositorio y sustituye la configuración actual de la aplicación. Revise
cada URL importada y las condiciones vigentes de cada sitio web antes de usarla. Después
puede editar o eliminar cualquier entrada de ejemplo.

**Importar configuración de cotizaciones de ejemplo (Import Sample Quotes Config)**,
en la parte superior de las ventanas Cotizaciones y Configuración de la aplicación,
hace lo mismo en cualquier momento, tras una confirmación:

- SmartTicker pregunta **¿Está seguro? (Are you sure?)** y advierte que la descarga
	sustituye sus cotizaciones, grupos de cotizaciones, aprobaciones de orígenes, vista,
	apariencia y otros ajustes de la aplicación. Las reglas de alerta se guardan en su
	propio archivo y no se eliminan.
- **Exportar la configuración existente... (Export existing config...)** es opcional.
	Guarda la configuración actual en un archivo JSON local y vuelve a la misma confirmación.
- **Importar configuración de cotizaciones de ejemplo (Import Sample Quotes Config)**
	descarga el ejemplo de Internet y sustituye la configuración.
- **Cancelar (Cancel)** no cambia nada.

### Añadir una entrada de cotización o noticias

1. Introduzca la etiqueta **Símbolo (Ticker)**, como `MSFT`. SmartTicker elimina los
	 espacios sobrantes y la guarda en mayúsculas.
2. Si lo desea, elija un **Grupo (Group)** existente en la lista de búsqueda o escriba
	 un nombre nuevo como `Nasdaq`, `Precious Metals` o `Mag 7`. Déjelo en blanco para
	 **Sin agrupar (Ungrouped)**.
3. Seleccione un valor predefinido de **Origen (Source)**.
4. Introduzca el **Sufijo de URL (URL suffix)**, o una URL completa cuando use
	 **URL personalizada (Custom URL)**.
5. Seleccione **Precio (Price)**, **Noticias (News)** o ambos en **Recopilar (Collect)**.
	 Se requiere al menos uno.
6. Introduzca los selectores manualmente, use los botones de detección o deje en blanco
	 los selectores opcionales para usar la detección integrada.
7. Seleccione **Validar URL (Validate URL)** para probar el precio normal o los titulares.
8. Si SmartTicker solicita la aprobación del origen, revise el sitio web y confirme solo
	 si tiene permiso para recopilar información de él.
9. Seleccione **Añadir entrada independiente (Add independent entry)**. SmartTicker
	 guarda la entrada y actualiza inmediatamente los datos que tenga habilitados.

### Agrupar cotizaciones

Un grupo es una colección con nombre que usted define. No está vinculado a una bolsa ni
a una categoría integrada, por lo que puede organizar las entradas por mercado, tipo de
activo, estrategia, cartera, región o cualquier otro criterio. Los nombres se recortan,
pueden usar Unicode y pueden contener hasta 80 caracteres. Cada cotización puede
pertenecer como máximo a un grupo.

Use **Administrar grupos (Manage groups)** junto al campo Grupo, o seleccione **Grupos
de cotizaciones... (Quote groups...)** en el menú del botón derecho del teletipo. La
ventana tiene tres áreas de trabajo:

- A la izquierda, introduzca un **Nombre de grupo (Group name)** y elija **Crear
	(Create)**. Seleccione un grupo existente, edite su nombre y elija **Actualizar
	(Update)**, o elija **Eliminar (Delete)**. Los grupos vacíos se conservan.
- A la derecha, seleccione una cotización. Su grupo actual aparece en la columna
	**Grupo actual (Current group)**; **Sin agrupar (Ungrouped)** significa que no tiene
	ninguna asociación.
- En el centro, elija **Asociar (Associate)** después de seleccionar un grupo y una
	cotización. Si la cotización ya pertenece a otro grupo, SmartTicker la mueve al grupo
	seleccionado.
- Elija **Quitar asociación (Remove association)** para devolver únicamente la
	cotización seleccionada a **Sin agrupar (Ungrouped)**.
- Al eliminar un grupo, todas sus cotizaciones vuelven a **Sin agrupar (Ungrouped)**.
	No se eliminan las cotizaciones, los orígenes, los datos actuales ni las alertas.
- También puede elegir un grupo existente en la lista de búsqueda al añadir o editar
	una cotización, o escribir allí el nombre de un grupo nuevo.
- Use los controles arriba y abajo de Entradas configuradas (Configured entries) para
	determinar el orden de los grupos y las filas en la tabla estática.
- En modo estático, arrastre el encabezado de un mosaico para reordenar directamente
	grupos completos. Las ventanas independientes de Cotizaciones y Noticias usan el
	mismo orden.

El ejemplo publicado contiene seis grupos de ejemplo, aunque deja desactivado el modo
estático de manera predeterminada. Active la vista estática después de importarlo para
ver esos grupos como una tabla.

### Valores predefinidos de origen y URL

| Origen | Qué debe introducir | Política mostrada por SmartTicker |
| --- | --- | --- |
| **Yahoo Finance** | Un sufijo después de `https://finance.yahoo.com/`, por ejemplo `quote/MSFT/`. | Se requiere permiso por escrito. Las condiciones de Yahoo prohíben la recopilación automatizada sin permiso previo. |
| **CNBC** | Un sufijo después de `https://www.cnbc.com/`. | Consulte la política vigente del sitio y sus directivas para robots. |
| **Trading Economics** | Un sufijo después de `https://tradingeconomics.com/`. | Prefiera una API documentada o un flujo autorizado y consulte la política vigente del sitio. |
| **URL personalizada (Custom URL)** | Una URL completa de una página pública `http://` o `https://`. | Revise las condiciones, la política de privacidad y las reglas de acceso automatizado del sitio. |

Solo se aceptan URL HTTP y HTTPS absolutas. Se rechazan las URL que contienen nombres
de usuario o contraseñas incrustados. Iniciar sesión en un navegador no autoriza a
SmartTicker a recopilar una página, y SmartTicker no usa sesiones autenticadas del navegador.

La línea **URL completa (Full URL)** muestra la dirección final generada a partir del
prefijo predefinido y su sufijo. Compruébela antes de la validación o la detección.

### Opciones de recopilación

- **Precio (Price)** solicita el precio normal. Los selectores opcionales de cambio,
	premercado y fuera de horario se evalúan a partir de la misma página descargada.
- **Noticias (News)** solicita los enlaces de titulares de la página.
- Seleccionar ambos permite que una entrada contribuya a las dos áreas del teletipo.
- Desmarcar ambos no es válido.

### Referencia de los campos de selector

Un selector CSS identifica un elemento del HTML estático de una página web. Los
selectores son opcionales, salvo que la detección automática no encuentre el valor que
necesita.

| Campo | Valor que extrae SmartTicker |
| --- | --- |
| **Selector de precio (Price selector)** | Precio normal o de cierre. |
| **Cambio de precio (Price change)** | Cambio porcentual de la sesión normal. Cuando está en blanco, se intenta la detección integrada del cambio. |
| **Selector de premercado (Pre-market selector)** | Precio de premercado, cuando esa sesión existe en la página. |
| **Cambio de premercado (Pre-market change)** | Cambio porcentual de premercado. |
| **Selector de fuera de horario (After-hours selector)** | Precio posmercado o fuera de horario. |
| **Cambio fuera de horario (After-hours change)** | Cambio porcentual posmercado o fuera de horario. |
| **Selector de noticias (News selector)** | Enlaces de titulares. Seleccione un ancla o un contenedor cuyos resultados incluyan enlaces. |

Los valores de premercado y fuera de horario complementan el precio normal; no lo
sustituyen. Una página puede omitir esos elementos fuera de la sesión de mercado
correspondiente.

Estos son ejemplos de selectores de Yahoo Finance usados por el ejemplo publicado:

```text
Price:                  [data-testid="qsp-price"]
Price change:           section.primary span[data-testid="qsp-price-change-percent"]
Pre-market price:       section.secondary span[data-testid="qsp-pre-price"]
Pre-market change:      section.secondary span[data-testid="qsp-pre-price-change-percent"]
After-hours price:      section.secondary span[data-testid="qsp-post-price"]
After-hours change:     section.secondary span[data-testid="qsp-post-price-change-percent"]
```

El marcado de los sitios web cambia con el tiempo. Considere los ejemplos como puntos
de partida, no como contratos permanentes.

### Detectar selectores

Cada campo de selector tiene un botón **Detectar (Discover)** correspondiente.

1. Complete la URL de origen y apruebe el sitio web si se requiere autorización.
2. Seleccione el botón de detección del tipo de valor exacto.
3. SmartTicker descarga el HTML estático público y enumera los posibles selectores con
	 un valor de ejemplo, un porcentaje de confianza y una explicación en la información
	 sobre herramientas.
4. Seleccione **Usar (Use)** junto a una sugerencia para copiarla en el campo correspondiente.
5. Valide u observe el resultado antes de confiar en él.

La detección no ejecuta JavaScript, no inicia sesión, no elude controles de acceso ni
examina su navegador. Un valor disponible solo mediante JavaScript puede no tener ningún
selector detectable. Los tipos de detección independientes evitan deliberadamente
mezclar valores de premercado y fuera de horario.

### Validar un origen

**Validar URL (Validate URL)** solicita la página e informa del precio normal o del
número de titulares que puede leer. Se puede usar de forma segura antes de introducir
un símbolo, porque SmartTicker emplea una etiqueta temporal para la prueba.

Actualmente, esta validación no comprueba los cuatro campos de selectores de premercado
y fuera de horario. Use los valores de ejemplo de su detección y después confirme los
datos de sesión mostrados.

Entre los fallos habituales se incluyen un error HTTP, un tiempo de espera agotado, un
valor ausente, cero titulares, un permiso de origen sin aprobar, contenido disponible
solo mediante JavaScript o un selector obsoleto.

### Límite de repetición de noticias

**Mostrar un máximo de _N_ veces (Show max _N_ times)** acepta valores de 1 a 100 y el
valor predeterminado es 5. SmartTicker cuenta una aparición por cada ciclo completo de
actualización de Noticias en el que se devuelve el mismo título de titular. Cuando el
título ha aparecido durante el número de ciclos configurado, se retira durante el resto
de la sesión actual de la aplicación. Editar o eliminar esa entrada borra su historial
de repeticiones.

### Editar, ordenar y eliminar entradas

La lista **Entradas configuradas (Configured entries)** muestra el símbolo, el grupo,
el origen, la URL, los indicadores de recopilación, el selector de precio normal, el
selector de noticias y el límite de repetición de noticias.

- **Editar (Edit)** carga la entrada en el formulario. Seleccione **Guardar cambios
	(Save changes)** para aplicarlos o **Cancelar edición (Cancel edit)** para descartar
	los cambios del formulario.
- Los botones de flecha arriba y abajo cambian el orden del teletipo y lo guardan de inmediato.
- **Eliminar (Remove)** elimina la entrada y sus datos mostrados actualmente.
- Si alguna regla de alerta apunta a la entrada, SmartTicker pregunta si se deben
	eliminar esas reglas. Una alerta sin una cotización configurada coincidente no puede
	activarse.
- Al cambiar el nombre de una entrada se actualizan los símbolos mostrados para las
	reglas de alerta asociadas a ella.

## Configuración de la aplicación

Abra **Configuración de la aplicación... (App Settings...)** desde el menú del botón
derecho. Los cambios surten efecto y se guardan automáticamente; no hay ningún botón
Aplicar (Apply).

### Filas y velocidad del teletipo

| Ajuste | Opciones | Valor predeterminado | Efecto |
| --- | --- | --- | --- |
| Filas de precios (Price rows) | De 1 a 8 | 1 | Número de filas paralelas de la marquesina de precios. |
| Velocidad de desplazamiento de precios (Price scroll speed) | 20, 30, 40, 50, 65, 80, 100 o 120 px/sec | 50 | Velocidad de la marquesina de precios. |
| Filas de noticias (News rows) | De 1 a 8 | 1 | Número de filas paralelas de la marquesina de titulares. |
| Velocidad de desplazamiento de noticias (News scroll speed) | 20, 30, 40, 50, 65, 80, 100 o 120 px/sec | 40 | Velocidad de la marquesina de noticias. |
| Tamaño de fuente con desplazamiento (Scrolling font size) | De 9 a 24 pt | 14 pt | Texto de Precios y Noticias en las filas con desplazamiento. |
| Tamaño de fuente estática (Static font size) | De 9 a 24 pt | 13 pt | Texto de cotizaciones y titulares en las filas estáticas. |
| Actualización de precios (Price refresh) | De 30 a 300 segundos, en pasos de 15 segundos | 60 segundos | Tiempo en el que cada entrada de precios permitida recibe una actualización programada. |
| Actualización de noticias (News refresh) | De 30 a 300 segundos, en pasos de 15 segundos | 300 segundos | Tiempo en el que cada entrada de Noticias permitida recibe una actualización programada. |

Las filas de precios y la velocidad de desplazamiento de precios se deshabilitan mientras
están activas las tablas agrupadas estáticas, porque ese modo muestra todas las entradas
de precios y nunca desplaza automáticamente ninguna de las ventanas. Los ajustes de filas
y velocidad de Noticias se conservan para la vista con desplazamiento.

Las solicitudes de Precios y Noticias se distribuyen de forma independiente entre
intervalos de un segundo a lo largo de todo su periodo, en lugar de iniciarse juntas.
Por ejemplo, 60 entradas en 30 segundos programan dos entradas por segundo; cinco
entradas en 30 segundos programan una aproximadamente cada seis segundos. Se ejecutan
como máximo cuatro solicitudes de origen a la vez, se omite el trabajo duplicado para
la misma entrada y flujo, y los intervalos perdidos no se repiten en ráfaga. **Actualizar
precios ahora (Refresh prices now)** o **Actualizar noticias ahora (Refresh news now)**
reinicia únicamente ese flujo y solicita su primer intervalo. Los precios y titulares
obtenidos correctamente permanecen visibles mientras se leen los datos de sustitución.

Cada solicitud HTTP tiene un tiempo de espera fijo de 20 segundos. Un origen lento no
retiene el distribuidor de la interfaz de usuario ni impide que los intervalos posteriores
usen la capacidad de solicitudes restante. SmartTicker informa de fallos como HTTP 403
y 429 y no elude las restricciones. No analiza ni aplica automáticamente directivas para
robots, valores `crawl-delay` ni instrucciones de espera del servidor, por lo que debe
elegir orígenes conformes y evitar solicitudes innecesariamente frecuentes.

### Tamaños de las ventanas

Configuración de la aplicación guarda tres pares de tamaños independientes:

| Ventana | Anchura | Altura | Valor predeterminado |
| --- | --- | --- | --- |
| Vista con desplazamiento | 420–7680 px | 50–900 px | 980 × 64 px |
| Vista estática de Precios | 420–7680 px | 420–4320 px | 980 × 420 px |
| Vista estática de Noticias | 420–7680 px | 240–4320 px | 680 × 340 px |

Cambiar un valor se aplica de inmediato cuando esa ventana o vista está activa. El
ejemplo publicado muestra un tamaño con desplazamiento de 1200 × 96, Precios estáticos
de 1200 × 720 y Noticias estáticas de 760 × 480, con texto con desplazamiento de 15
puntos y texto estático de 14 puntos. Una altura con desplazamiento menor que el espacio
necesario para las filas habilitadas aumenta automáticamente hasta el mínimo requerido.

Use las cuatro opciones de **Vista (View)** para elegir si se muestran Noticias y si el
diseño se desplaza o permanece estático. Cambiar la vista nunca elimina las entradas
configuradas.

### Iniciar SmartTicker al iniciar sesión

Active **Iniciar SmartTicker al iniciar sesión (Start SmartTicker when I sign in)** para
registrar el ejecutable instalado únicamente para el usuario actual.

- En Windows, SmartTicker usa la clave `Run` del Registro del usuario actual.
- En escritorios Linux compatibles con la convención de inicio automático de
	freedesktop, SmartTicker escribe `smartticker.desktop` en el directorio de inicio
	automático del usuario.
- La opción está deshabilitada en plataformas para las que SmartTicker no dispone de
	un mecanismo de registro compatible.

El sistema operativo es la autoridad. Si el inicio se modifica fuera de SmartTicker,
la casilla refleja el estado del sistema operativo la próxima vez que se carga la
configuración.

### Acceso a sitios web

**Permitir cookies de sitios web y redirecciones entre hosts (Allow website cookies and
cross-host redirects)** está deshabilitado de forma predeterminada.

Cuando está deshabilitado:

- SmartTicker requiere una aprobación explícita para cada host de sitio web antes de
	solicitarlo.
- No se aceptan cookies de sitios web.
- Se bloquean las redirecciones a un host diferente.
- Los hosts aprobados se recuerdan en la configuración local.

Cuando está habilitado:

- SmartTicker omite su paso de aprobación por host.
- Las cookies establecidas por los sitios web solicitados se conservan únicamente en
	un contenedor aislado en memoria y desaparecen cuando SmartTicker se cierra.
- Se pueden seguir redirecciones a otros hosts.
- SmartTicker sigue sin leer las cookies del navegador, enviar credenciales ni enviar
	formularios de inicio de sesión.

Desactivar esta opción elimina los datos mostrados actualmente de orígenes no aprobados
hasta que esos hosts se aprueben y se actualicen.

#### Opciones de privacidad de los sitios web

Si una respuesta se reconoce como un formulario de privacidad o cookies que contiene
tanto opciones positivas como negativas, SmartTicker se detiene y muestra el título de
la página, la URL solicitada, la URL de consentimiento, un resumen del formulario y las
etiquetas Aceptar/Rechazar (Accept/Reject) del sitio web.

- **Aceptar (Accept)** envía los campos ocultos proporcionados por ese formulario más
	el control Accept exacto que usted seleccionó.
- **Rechazar (Reject)** envía esos campos ocultos más el control Reject exacto que
	usted seleccionó.
- **Cancelar (Cancel)** no envía nada.

Esta es una opción de privacidad de un sitio web, no la aprobación de permisos por
origen de SmartTicker.

#### Validar todos los orígenes

Seleccione **Validar todos los orígenes (Validate all sources)** para revisar y probar
cada entrada configurada.

1. Si el acceso a sitios web está restringido, SmartTicker agrupa las entradas no
	 aprobadas por nombre de host y muestra un cuadro de diálogo de revisión de origen
	 por cada host.
2. Revise el host, el resumen de la política, las indicaciones, los nombres de origen
	 y los símbolos.
3. Marque la confirmación solo si ha revisado el sitio web y tiene permiso para usarlo.
4. Elija **Aprobar este origen (Approve this source)**, **Omitir este origen (Skip this
	 source)** o **Cancelar validación (Cancel validation)**.
5. SmartTicker prueba cada entrada permitida e informa del total de aprobadas, fallidas
	 y omitidas. Los problemas individuales aparecen debajo de la línea de estado.

Los registros de aprobación reflejan el permiso dentro de SmartTicker; no conceden
derechos legales ni invalidan las condiciones del sitio web.

### Apariencia

**Transparencia de la ventana (Window transparency)** cambia únicamente el fondo del
teletipo. El texto permanece opaco. El intervalo es del 20% al 100%, en pasos del 5%,
y el valor predeterminado es 100%.

Los campos de color aceptan valores hexadecimales `#RRGGBB` y también proporcionan un
selector de color.

| Color | Valor predeterminado | Se usa para |
| --- | --- | --- |
| Fondo (Background) | `#10151D` | Fondo del teletipo antes de aplicar la transparencia. |
| Nombre de cotización (Quote name) | `#79C0FF` | Etiqueta del símbolo u origen. |
| Precio de cierre (Close price) | `#FFA657` | Precio normal. |
| Fuera de horario (After hours) | `#00E5FF` | Precios de premercado y fuera de horario. |
| 1.ª noticia (News 1st) | `#FFFFFF` | Titulares 1, 5, 9, etc. |
| 2.ª noticia (News 2nd) | `#00E5FF` | Titulares 2, 6, 10, etc. |
| 3.ª noticia (News 3rd) | `#A3E635` | Titulares 3, 7, 11, etc. |
| 4.ª noticia (News 4th) | `#79C0FF` | Titulares 4, 8, 12, etc. |
| Subida (Change up) | `#3FB950` | Cambios porcentuales positivos. |
| Bajada (Change down) | `#F85149` | Cambios porcentuales negativos. |
| Parpadeo de alerta (Alert blink) | `#FF00FF` | Alertas de precio activadas, alternando con negro. |

**Restablecer valores predeterminados (Reset to defaults)** restaura todos los colores
anteriores y una opacidad de fondo del 100%. No restablece las filas, las velocidades,
los tamaños de fuente, los tamaños de ventana, los orígenes, los intervalos de
actualización, las alertas ni el idioma.

### Copia de seguridad y restauración

SmartTicker mantiene la configuración de la aplicación y las reglas de alerta en
archivos JSON separados y proporciona botones distintos para cada tipo de copia de seguridad.

#### Exportar e importar la configuración

- **Exportar configuración... (Export settings...)** escribe las entradas configuradas,
	las asignaciones de grupos, las definiciones de grupos, las cotizaciones de noticias
	ocultas, el orden de las entradas, los selectores, la elección de vista de cotizaciones
	con desplazamiento o estática, los hosts aprobados, la visibilidad de las líneas, las
	filas, las velocidades, los tamaños de fuente con desplazamiento y estática, los tres
	pares de tamaños de ventana, los intervalos de actualización, la preferencia de inicio,
	la opción de acceso a sitios web, los colores, incluido el color de parpadeo de alerta,
	la transparencia y el idioma.
- **Importar configuración... (Import settings...)** valida el archivo completo antes
	de cambiar nada. Si se rechaza el archivo, la configuración actual no cambia.
- Una importación correcta sustituye todas las entradas configuradas y preferencias de
	la aplicación. No sustituye el archivo independiente de reglas de alerta.
- Los grupos se incluyen como asignaciones de cotizaciones en el archivo de configuración,
	junto con las propias definiciones de grupo, de modo que un grupo sin cotizaciones
	también se conserva en una copia de seguridad. No existe un archivo independiente de
	exportación o importación solo para grupos.
- La preferencia de inicio está presente en una copia de seguridad de la configuración,
	pero importarla no cambia silenciosamente el registro de inicio del sistema operativo.
	El sistema operativo sigue siendo la autoridad; use la casilla Inicio (Startup) para
	cambiar el registro en el equipo actual.
- Los archivos de importación están limitados a 1 MiB, la versión 1 del esquema y un
	máximo de 200 suscripciones. Las propiedades desconocidas, los identificadores
	duplicados, las URL con formato incorrecto, los colores no válidos, los intervalos no
	válidos o los códigos de idioma no admitidos se rechazan en lugar de ignorarse
	silenciosamente.

#### Exportar e importar reglas de alerta- **Exportar reglas de alerta... (Export alert rules...)** escribe todas las reglas, además de Buzz, el número de zumbidos y la duración del parpadeo.
- **Importar reglas de alerta... (Import alert rules...)** valida todo el archivo y
después sustituye todas las reglas actuales y la configuración de activación de alertas.
- Las reglas vuelven a conectarse primero por el identificador de suscripción. Cuando
los identificadores difieren, SmartTicker intenta buscar una coincidencia de símbolo
sin distinguir entre mayúsculas y minúsculas.
- Una regla importada sin una cotización coincidente se conserva, pero no puede activarse.
El estado de la importación informa de cuántas reglas se han vuelto a vincular y cuántas
siguen sin coincidencia.
- Los archivos de importación de alertas están limitados a 1 MiB.

Para transferir los datos a otro equipo, importe primero la configuración de la
aplicación y después las reglas de alerta. Importar las alertas en segundo lugar permite
que las reglas vuelvan a conectarse a los nuevos identificadores de suscripción por símbolo.

### Editar directamente los archivos de configuración

**Editar la configuración actual de la aplicación (Edit Current App Config)** y
**Editar las reglas de alerta actuales (Edit Current Alert Rules)**, en Configuración
de la aplicación, abren el archivo JSON activo en el editor de texto que el sistema
tenga asociado con `.json`. Esta función está destinada a usuarios avanzados; las
ventanas de SmartTicker permiten configurar los mismos ajustes sin riesgo.

Ambos botones muestran primero una confirmación que le pide exportar el archivo actual.
Realice esa exportación: editar el archivo manualmente puede dañarlo y no se puede deshacer.

- **Exportar la configuración existente... (Export existing config...)** guarda el
	archivo actual y después vuelve a la misma pregunta.
- **Abrir en el editor de texto (Open in text editor)** abre el archivo activo.
- **Cancelar (Cancel)** no cambia nada.

SmartTicker supervisa el archivo y lo vuelve a cargar en cuanto el editor lo guarda:

- Un archivo válido se aplica inmediatamente y el teletipo se actualiza sin reiniciar.
- Se rechaza un JSON con formato incorrecto, una infracción del esquema o cualquier
	otro error de validación. La configuración en ejecución queda intacta y la ventana
	Configuración de la aplicación informa del problema.
- Después de rechazar una edición, corrija el archivo o restaure una exportación válida
	mediante **Importar configuración... (Import settings...)** o **Importar reglas de
	alerta... (Import alert rules...)**.
- Si otro programa mantiene bloqueado un archivo, se vuelve a intentar durante un breve
	periodo y después se informa del problema.

La edición del archivo de reglas de alerta sigue las mismas reglas y no afecta a la
configuración de la aplicación, porque los dos archivos son independientes.

## Reglas de alerta

Abra **Alertas (Alerts)** desde el menú del botón derecho. Las reglas se evalúan después
de cada actualización de precio correcta y supervisan únicamente el precio normal, no
los valores de premercado ni fuera de horario.

### Crear una regla

1. Seleccione una **Cotización (Quote)** configurada. Las entradas con el mismo símbolo
	 siguen siendo distintas.
2. Seleccione una **Condición (Condition)** e introduzca un umbral numérico con un
	 decimal invariable, como `250.50`.
3. Si lo desea, elija **Activa desde (Active from)**. Déjelo vacío para activarla de inmediato.
4. Mantenga marcada la opción **Nunca caduca (Never expires)** o desmárquela y elija
	 una fecha de caducidad.
5. Seleccione **Añadir regla (Add rule)**.

Estas son las comparaciones disponibles:

| Opción | Significado |
| --- | --- |
| `LessThan` | Precio `<` que el umbral. |
| `LessThanOrEqual` | Precio `<=` que el umbral. |
| `GreaterThan` | Precio `>` que el umbral. |
| `GreaterThanOrEqual` | Precio `>=` que el umbral. |
| `EqualTo` | El precio es exactamente igual al umbral. |
| `NotEqualTo` | El precio difiere del umbral. |

El límite inicial es inclusivo. El límite de caducidad también lo es; una vez superado,
la regla deja de activarse. SmartTicker rechaza una caducidad anterior al inicio.

### Cuando se activa una regla

Una regla habilitada y programada se activa una vez cuando su condición cambia de falsa
a verdadera. No genera una notificación en cada actualización mientras la condición
siga siendo verdadera. Cuando el precio deja de cumplir la condición, la regla se rearma
y puede activarse cuando el precio vuelve a cumplirla.

Editar una regla o deshabilitarla y volver a habilitarla también la rearma. Por tanto,
una regla habilitada puede activarse de inmediato si el precio normal más reciente ya
cumple su condición. Un precio ausente o cuya solicitud haya fallado no puede activar
una regla.

Cuando se activan una o varias reglas:

- La entrada de precio afectada alterna entre el color de parpadeo de alerta configurado
	y el negro durante el tiempo configurado. El color de parpadeo predeterminado es
	magenta (`#FF00FF`).
- Si **Zumbido (Buzz)** está habilitado, SmartTicker reproduce la secuencia de zumbidos
	configurada.
- El mensaje de alerta identifica una regla o informa del número de reglas que se han
	activado a la vez.
- El teletipo continúa desplazándose mientras el resaltado de alerta está activo.

### Configuración de salida de alertas

| Ajuste | Intervalo | Valor predeterminado |
| --- | --- | --- |
| **Zumbido (Buzz)** | Activado o desactivado | Activado |
| Número de zumbidos (Buzz count) | De 1 a 20 | 15 |
| **Parpadear durante (Blink for)** | De 5 a 900 segundos, en pasos de 15 segundos | 60 segundos |

Deshabilitar Buzz mantiene activa la alerta visual. Si se activan varias reglas en la
misma evaluación, SmartTicker inicia una sola secuencia de zumbidos configurada para esa
evaluación. Cambie **Parpadeo de alerta (Alert blink)** en **Configuración de la aplicación
> Apariencia (App Settings > Appearance)**. Es una preferencia de apariencia de la
aplicación, por lo que la exportación o importación de la Configuración la incluye en
lugar de incluirla en el archivo independiente de reglas de alerta.

### Administrar las reglas configuradas

- **Editar (Edit)** carga una regla en el formulario. Seleccione **Actualizar regla
	(Update rule)** para guardarla o **Cancelar (Cancel)** para dejarla sin cambios.
- **Deshabilitar (Disable)** conserva la regla, pero impide que coincida. **Habilitar
	(Enable)** la rearma y la evalúa con el precio normal más reciente.
- **Eliminar (Remove)** elimina la regla.
- La lista muestra el estado habilitado, el símbolo, el resumen de la condición y la
	programación.

Los cambios en las reglas de alerta y en la configuración de salida de alertas se
guardan automáticamente.

## Archivos locales y privacidad

SmartTicker guarda la configuración localmente y no la sincroniza con un servicio del
desarrollador.

En Windows, los archivos predeterminados son:

```text
%LocalAppData%\SmartTicker\settings.json
%LocalAppData%\SmartTicker\alerts.json
```

En Linux, .NET usa el directorio local de datos de aplicación del usuario actual,
normalmente:

```text
~/.local/share/SmartTicker/settings.json
~/.local/share/SmartTicker/alerts.json
```

### Usar un directorio de datos aislado

Los diagnósticos avanzados y las ejecuciones de prueba pueden establecer
`SMARTTICKER_DATA_DIRECTORY` antes de iniciar SmartTicker. Cuando el valor no está en
blanco, ambos archivos se colocan directamente en ese directorio resuelto con los
nombres `settings.json` y `alerts.json`; los valores predeterminados de la plataforma
indicados anteriormente no se usan para ese proceso. Es preferible usar una ruta
absoluta y asegurarse de que se pueda escribir en ella.

Ejemplo de PowerShell:

```powershell
$env:SMARTTICKER_DATA_DIRECTORY = 'D:\SmartTicker-Profile'
& 'C:\Program Files\SmartTicker\SmartTicker.Desktop.exe'
```

Ejemplo de shell de Linux:

```bash
SMARTTICKER_DATA_DIRECTORY="$HOME/.local/share/SmartTicker-Test" smartticker
```

Establezca la variable antes de iniciar el proceso. SmartTicker no copia el perfil
predeterminado en el directorio seleccionado, por lo que un directorio vacío comienza
con una configuración vacía. Las instancias dirigidas al mismo directorio pueden
observar las ediciones guardadas de las demás. Use los comandos normales de exportación
e importación de Configuración y Reglas de alerta para realizar copias de seguridad y
transferir perfiles.

La ventana Alertas muestra la ruta exacta del archivo de alertas en uso. Para escribir,
se usa un archivo temporal seguido de una sustitución, de modo que un archivo escrito
parcialmente no se considere la configuración actual.

SmartTicker no tiene cuenta, telemetría, análisis, publicidad ni sincronización en la
nube. Un sitio web de origen recibe información normal de la red, como su dirección IP,
cuando SmartTicker solicita ese origen. Al abrir la Ayuda se solicita la guía sin
procesar a GitHub. Para obtener todos los detalles, lea `PRIVACY.md` en el repositorio.

Usted es responsable de asegurarse de que cada URL y selector de origen se utilice de
acuerdo con las condiciones, la licencia, las directivas para robots y la legislación
aplicable del sitio web.

## Solución de problemas

### Una cotización aparece como no disponible o sin precio

Una solicitud de origen agota el tiempo de espera después de 20 segundos. Si esa
cotización tiene una instantánea anterior correcta, una actualización fallida la
mantiene visible; de lo contrario, la cotización muestra **No disponible (Unavailable)**
hasta que se complete correctamente una actualización posterior. Lea el error de
validación o actualización antes de cambiar los selectores.

1. Abra **Cotizaciones... (Quotes...)**, edite la entrada y compruebe la URL completa
	 (Full URL).
2. Confirme que **Precio (Price)** está seleccionado.
3. Apruebe el sitio web si se le solicita.
4. Seleccione **Validar URL (Validate URL)** y lea el resultado exacto.
5. Ejecute **Detectar precio (Discover price)** o examine el HTML estático de la página
	 y actualice el selector.
6. Compruebe si la página requiere JavaScript, autenticación o un consentimiento que
	 SmartTicker no pueda gestionar de forma segura.
7. Respete los códigos HTTP 403 y 429, las restricciones para robots y la política de
	 acceso automatizado del sitio.

### Faltan datos de premercado o fuera de horario

- Es posible que la sesión de mercado correspondiente no esté activa.
- La página puede omitir el elemento de sesión cuando no existe ningún valor para esa sesión.
- Verifique que los selectores de premercado apunten a elementos de premercado y que
	los selectores de fuera de horario apunten a elementos posmercado.
- Vuelva a ejecutar el comando de detección correspondiente, porque el marcado del sitio
	web puede haber cambiado.

### Las noticias están vacías

- Confirme que **Noticias (News)** está seleccionado.
- Valide el origen y ejecute **Detectar noticias (Discover news)**.
- Asegúrese de que el selector devuelve enlaces con texto visible de titulares.
- Una solicitud de Noticias fallida o que agote el tiempo de espera conserva los
	titulares anteriores obtenidos correctamente, cuando estén disponibles. Un origen sin
	ningún resultado correcto permanece vacío hasta que un intervalo posterior finalice
	correctamente.
- Un titular desaparece después de alcanzar su límite de repetición configurado para
	esta sesión.
- En Noticias estáticas, confirme que la cotización deseada esté marcada en **Mostrar
	noticias de (Show news for)**.

### La detección de selectores no encuentra nada

La detección solo lee el HTML estático descargado. No puede ver los valores que el
JavaScript de la página crea posteriormente. Introduzca manualmente un selector
verificado, elija una página o un flujo estático, o use una API autorizada y documentada
mediante una página pública compatible.

### Una alerta no se activa

- Confirme que la cotización asociada aún existe, recopila Precio y tiene un precio
	normal obtenido correctamente.
- Confirme que la regla está Habilitada (Enabled) y dentro de su periodo de inicio y caducidad.
- Compruebe la comparación y el umbral. `EqualTo` requiere una igualdad decimal exacta.
- Recuerde que una condición que permanece verdadera se activa una vez; debe pasar a
	falsa antes de que pueda activarse de nuevo, a menos que edite o vuelva a habilitar la regla.
- Los precios de premercado y fuera de horario no controlan las reglas de alerta.

### SmartTicker no se puede mover ni cambiar de tamaño

- Mueva la ventana únicamente desde el control de puntos verticales de la franja izquierda.
- Cambie el tamaño desde un borde o una esquina; use la marca visible de la esquina
	inferior derecha si resulta difícil localizar un borde.
- El contenido del teletipo no es deliberadamente una superficie de movimiento.

### Los grupos o valores estáticos no son los esperados

- Abra **Cotizaciones... (Quotes...)** y confirme el valor Grupo de cada entrada.
- Abra **Grupos de cotizaciones... (Quote groups...)** para administrar las definiciones
	de grupos y revisar la asociación actual de cada cotización.
- Las entradas con un Grupo en blanco aparecen en **Sin agrupar (Ungrouped)**.
- **Cambio (Chg)** se calcula a partir de Last y Chg%; no se extrae de forma independiente
	de la página. Permanece como `—` cuando el porcentaje no está disponible.
- Reordene las entradas con los controles arriba y abajo para cambiar el orden de grupos
	y filas.
- Arrastre el control de puntos del encabezado de un mosaico para mover todo el grupo.
	Suéltelo sobre la mitad izquierda de otro mosaico para colocarlo antes, o sobre la
	mitad derecha para colocarlo después.
- Seleccione **Actualizar precios ahora (Refresh prices now)** mientras SmartTicker no
	esté en pausa para actualizar la tabla.

### El texto de la Ayuda no tiene formato o la navegación no se desplaza

- La ventana de Ayuda debe mostrar encabezados, párrafos, listas, tablas, enlaces y
	bloques de código con formato en lugar de signos de puntuación de Markdown.
- Use **En esta página (On this page)**, a la izquierda, para ir a una sección principal.
	Los enlaces de la tabla Navegación rápida también desplazan el documento.
- Cierre y vuelva a abrir la Ayuda, o cambie el **Idioma (Language)**, para solicitar la
	guía en línea publicada correspondiente. Hasta que llegue, SmartTicker muestra la guía
	correspondiente con formato integrada en la aplicación instalada.

### La Ayuda en línea no está disponible o está desactualizada

- Cierre y vuelva a abrir la Ayuda para solicitar de nuevo la guía publicada.
- Abra en un navegador la dirección sin procesar de GitHub que aparece cerca del principio
	de esta guía para examinar directamente el archivo publicado.
- SmartTicker usa la guía integrada cuando la solicitud falla o devuelve un archivo vacío.
- Los cambios en línea solo aparecen después de que `HELPME.md` o el archivo localizado
	correspondiente `help/HELPME.es.md` se publique en la rama `main` del repositorio.

## Soporte

Informe de problemas reproducibles en:

<https://github.com/bulentozkir/smartticker/issues>

Incluya la versión de SmartTicker, el sistema operativo, el nombre de host del origen,
el estado de validación y el texto exacto del error. Elimine las URL privadas u otra
información confidencial antes de publicar.