# Ajuda do SmartTicker

Este guia se aplica ao SmartTicker 1.0.3. Ele explica o ticker principal, as Configurações do aplicativo,
as cotações, as regras de alerta, as permissões de sites, os backups e os problemas comuns.

O SmartTicker lê HTML estático público das páginas da Web que você configura. Ele não
fornece um feed de dados de mercado, e as informações extraídas podem estar atrasadas, incompletas ou
incorretas. Confirme informações financeiras importantes com uma fonte oficial.

## Navegação rápida

| Área | Ir para |
| --- | --- |
| Primeiros passos | [Abrir as janelas de Ajuda e configuração](#abrir-as-janelas-de-ajuda-e-configuração) |
| Ticker principal | [Controles](#controles-do-ticker-principal) · [Visualização com rolagem ou estática](#escolher-a-visualização-de-cotações-com-rolagem-ou-estática) · [Mover](#mover-o-ticker) · [Redimensionar](#redimensionar-o-ticker) · [Pausar](#pausar-e-retomar) · [Referência do menu](#referência-do-menu-principal) |
| Cotações e notícias | [Cotações](#cotações) · [Adicionar uma entrada](#adicionar-uma-entrada-de-cotação-ou-notícia) · [Agrupar cotações](#agrupar-cotações) · [URLs de origem](#predefinições-de-fonte-e-urls) · [Seletores](#referência-dos-campos-de-seletor) · [Descoberta](#descobrir-seletores) · [Validação](#validar-uma-fonte) |
| Preferências do aplicativo | [Configurações do aplicativo](#configurações-do-aplicativo) · [Linhas e velocidade](#linhas-e-velocidade-do-ticker) · [Inicialização](#iniciar-o-smartticker-ao-entrar) · [Acesso a sites](#acesso-a-sites) · [Aparência](#aparência) · [Backup e restauração](#backup-e-restauração) · [Editar arquivos de configuração](#editar-os-arquivos-de-configuração-no-local) |
| Alertas de preço | [Regras de alerta](#regras-de-alerta) · [Criar uma regra](#criar-uma-regra) · [Comportamento ao disparar](#quando-uma-regra-dispara) · [Saída de alerta](#configurações-de-saída-de-alerta) · [Gerenciar regras](#gerenciar-regras-configuradas) |
| Dados e suporte | [Arquivos locais e privacidade](#arquivos-locais-e-privacidade) · [Solução de problemas](#solução-de-problemas) · [Suporte](#suporte) |

## Abrir as janelas de Ajuda e configuração

Clique com o botão direito do mouse no ticker para abrir seu menu. Os principais comandos de configuração são:

- **Quotes...** (*Cotações...*): adicionar, testar, editar, ordenar e remover fontes de cotações ou notícias.
- **Quote groups...** (*Grupos de cotações...*): criar, atualizar ou excluir grupos e associar cotações a eles.
- **Alerts** (*Alertas*): criar e gerenciar regras de alerta de preço.
- **App Settings...** (*Configurações do aplicativo...*): configurar linhas, velocidades, intervalos de atualização, inicialização, acesso a
	sites, cores, transparência e backups.
- **View** (*Exibir*): selecionar uma das quatro combinações mutuamente exclusivas: com rolagem ou estática,
	com Prices only (*Somente preços*) ou Prices with News (*Preços com notícias*).
- **Help** (*Ajuda*): abrir este guia dentro do SmartTicker.
- **About SmartTicker** (*Sobre o SmartTicker*): mostrar a versão instalada e o aviso de licença.
- **Exit** (*Sair*): fechar completamente o SmartTicker.

A janela Ajuda formata e exibe imediatamente o guia incorporado correspondente ao idioma
selecionado no aplicativo. Em seguida, verifica o guia online correspondente sempre que você
abre a Ajuda ou altera **Language** (*Idioma*). O guia online em português é:

<https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/help/HELPME.pt.md>

Se não for possível baixar o documento online, o SmartTicker mantém em exibição a tradução
incorporada correspondente. Alterar **Language** (*Idioma*) atualiza imediatamente o título, o status,
a navegação e o guia completo em uma janela Ajuda aberta. Feche a Ajuda usando o controle normal
de fechar da barra de título.

## Controles do ticker principal

### Escolher a visualização de cotações com rolagem ou estática

O SmartTicker oferece quatro modos de exibição mutuamente exclusivos. Clique com o botão direito do mouse no ticker, abra
**View** (*Exibir*) e selecione um deles. O layout muda imediatamente e sua escolha é salva.

| Opção de exibição | Resultado |
| --- | --- |
| **Left-to-right scroll: Prices only** (*Rolagem da esquerda para a direita: somente preços*) | Faixa móvel de preços no ticker principal; nenhuma exibição de notícias. Este é o padrão. |
| **Left-to-right scroll: Prices with News** (*Rolagem da esquerda para a direita: preços com notícias*) | Faixas móveis de preços e notícias no ticker principal. |
| **Static view: Prices only** (*Visualização estática: somente preços*) | Blocos de preço responsivos na janela principal; nenhuma janela News (*Notícias*). |
| **Static view: Prices with News** (*Visualização estática: preços com notícias*) | Blocos de preço responsivos e uma janela estática **SmartTicker News** separada. |

Os arquivos de configurações criados antes da inclusão dessas opções são mapeados para a combinação correspondente
das configurações salvas de rolagem/estática e notícias. O modo de exibição é gerenciado somente pelo menu
**View** (*Exibir*) aberto com o botão direito do mouse no ticker.

- Em qualquer modo com rolagem, os preços usam a faixa móvel horizontal, a quantidade configurada de linhas
	de preço e a velocidade de rolagem.
- Em qualquer modo estático, os grupos aparecem como blocos responsivos dispostos da esquerda para a direita. Os blocos
  passam para outra linha somente quando a janela é estreita demais. Os preços não se movem
  automaticamente.
- Cada bloco de cotação tem suas próprias colunas alinhadas **Symbol** (*Símbolo*), **Last** (*Último*), **Chg** (*Var.*) e **Chg%** (*Var.%*).
  **Chg** é derivado
	de Last e Chg%, pois as páginas de origem fornecem um seletor de porcentagem, e não um
	seletor separado de variação absoluta. Ele exibe `—` quando um dos valores não está disponível.
- Selecione o cabeçalho de um grupo para recolhê-lo ou expandi-lo. Os grupos seguem a primeira ocorrência
	das cotações na ordem das entradas configuradas; as linhas de um grupo mantêm essa ordem.
- As entradas sem grupo aparecem em **Ungrouped** (*Sem grupo*).
- Passe o cursor sobre Last para ver os valores disponíveis de pré-mercado e pós-mercado. Clique duas vezes em uma
	linha de cotação para abrir sua página de origem.
- O piscar dos alertas e as cores de alta/baixa funcionam nos dois modos de preço.
- As notícias abrem automaticamente em uma janela **SmartTicker News** separada que contém blocos estáticos
	de grupos **Symbol / Headline** (*Símbolo / Manchete*). Elas não passam em faixa móvel no modo estático. A janela News
	tem uma barra de título normal e uma borda de redimensionamento, portanto as janelas Quotes e News podem
	ser movidas de forma independente para monitores diferentes. Clique duas vezes em uma linha de manchete para abrir
	sua fonte.
- Na primeira execução, News usa o tamanho compacto de 680×340. O SmartTicker a posiciona em outro
	monitor quando houver um disponível; em um único monitor, ele primeiro tenta uma área livre abaixo,
	à direita, acima ou à esquerda de Prices. Depois, você pode movê-la e redimensioná-la normalmente.
- Dentro de cada grupo de News, as manchetes são intercaladas por cotação: uma manchete da
	primeira cotação, depois uma da próxima cotação, continuando em rodadas. Assim, uma cotação com muitas
	manchetes não pode ocupar toda a parte superior do grupo.
- Abra a lista suspensa de uma linha **Show news for** (*Mostrar notícias de*) e marque ou desmarque cada cotação
	de forma independente. Qualquer combinação de cotações pode ficar visível, inclusive todas ou nenhuma. O
	botão resume a escolha atual, e as entradas incluem a cotação e a fonte para que
	símbolos duplicados permaneçam independentes. As cotações desmarcadas são salvas no arquivo de configurações
	como `hiddenNewsQuotes`, portanto persistem após uma reinicialização e acompanham um backup das configurações.
- Arraste a alça pontilhada ao lado do título de qualquer bloco de cotação ou notícia e solte-a na metade esquerda
	ou direita de outro bloco. A ordem muda nas duas janelas e é salva por meio da
	reordenação das entradas configuradas subjacentes.
- Um grupo com muitas linhas rola dentro de seu próprio bloco delimitado. A visualização geral rola
	verticalmente somente quando as linhas de blocos quebradas não cabem na altura atual da janela.

Fechar **SmartTicker News** não desativa a coleta de notícias. Para reabri-la, clique com o botão direito do mouse
na janela Prices e selecione **View > Open static news window** (*Exibir > Abrir janela estática de notícias*). Selecionar **Static
view: Prices only** a fecha; selecionar **Static view: Prices with News** a abre
novamente. Qualquer opção com rolagem fecha a janela News separada; a opção com rolagem
Prices-with-News restaura a faixa móvel de notícias no ticker principal.

A troca de modos aplica o tamanho salvo para aquela visualização. O ticker com rolagem, a janela estática Prices
e a janela estática News mantêm, cada um, largura e altura independentes.

### Mover o ticker

Mantenha pressionada a alça de pontos verticais na parte superior da faixa estreita à esquerda, arraste o
ticker e solte o botão do mouse. O texto do ticker não é uma superfície de arraste; assim, selecionar
ou clicar no conteúdo não inicia acidentalmente o movimento da janela.

### Redimensionar o ticker

Mova o ponteiro para qualquer borda ou canto até aparecer um cursor de redimensionamento; depois, pressione e
arraste. O canto inferior direito tem uma pequena marca de redimensionamento visível. A largura mínima da janela
é 420 pixels. A altura com rolagem varia de 50 a 900 pixels, a altura estática de Prices varia de 420
a 4320 pixels e a altura estática de News varia de 240 a 4320 pixels.

O redimensionamento manual atualiza as dimensões salvas da visualização ativa após o fim do arraste.
Os três pares de tamanhos são incluídos em um backup das configurações. As posições das janelas não são armazenadas.
Se um tamanho com rolagem for baixo demais para as linhas Price/News selecionadas e o tamanho da fonte
de rolagem, o SmartTicker aumentará automaticamente essa altura salva. Portanto, selecionar **Left-to-right
scroll: Prices with News** sempre reserva espaço para as linhas de News, em vez de
ocultá-las silenciosamente.
Sempre que uma janela abre ou é movida, o SmartTicker mantém pelo menos seu canto superior esquerdo de 32 pixels
dentro da área de trabalho de uma tela e limita X e Y globais a, no mínimo, 1. Isso mantém a alça de movimento
ou o canto do título acessível com o mouse mesmo depois que um monitor é desconectado.

### Pausar e retomar

Selecione o botão de status abaixo da alça de movimento ou clique com o botão direito e selecione
**Pause / Resume** (*Pausar / Retomar*). Pausar interrompe as atualizações automáticas de preços e notícias e congela a
faixa móvel. Também impede que qualquer comando de atualização manual inicie um novo trabalho. Uma solicitação de fonte
que já estava em andamento não é cancelada à força apenas por causa de Pause e pode
terminar antes de toda a atividade parar. Resume reinicia os temporizadores automáticos.

No Windows, o SmartTicker define automaticamente a prioridade de seu processo do sistema operacional como **Low** (*Baixa*) e ativa o
**Efficiency mode** (*Modo de eficiência*) do Windows (EcoQoS) antes de iniciar a interface. Não há uma configuração no aplicativo para
esse comportamento. Ele também usa um caminho de renderização de software de baixa sobrecarga. O tempo da faixa móvel se adapta
à velocidade configurada, e uma faixa móvel pausada, vazia ou desanexada interrompe seu temporizador de
animação. Linhas inalteradas suprimem notificações visuais redundantes. O piscar dos alertas e o
destaque marrom de alteração por três segundos são intencionais e não pausam a rolagem. O agendamento de processos
no Linux fica a cargo do sistema operacional. Se o Windows recusar qualquer uma das configurações do processo,
o SmartTicker registrará a falha no rastreamento de diagnóstico e continuará a inicialização.

### Abrir links

Clique duas vezes no texto com link do ticker, incluindo uma manchete de notícia, para abrir a fonte no
navegador padrão. O SmartTicker não abre links com um único clique.

### Destaques de alterações

Após cada atualização, o SmartTicker marca brevemente, por três segundos e com fundo marrom, o que mudou:

- Uma cotação cujo preço difere da sincronização anterior.
- Cada manchete que não estava presente na sincronização anterior dessa cotação.

A primeira sincronização após a inicialização não destaca nada, pois não há valor anterior para
comparação. Um alerta disparado mantém sua própria cor de piscar e tem prioridade.

### Referência do menu principal

| Comando | Efeito |
| --- | --- |
| **Refresh prices now** (*Atualizar preços agora*) | Reiniciar o ciclo escalonado de preços e solicitar seu primeiro intervalo quando o SmartTicker não estiver pausado. |
| **Refresh news now** (*Atualizar notícias agora*) | Reiniciar o ciclo escalonado de notícias e solicitar seu primeiro intervalo quando o SmartTicker não estiver pausado. |
| **Pause / Resume** (*Pausar / Retomar*) | Alternar a atualização e o movimento da faixa móvel. |
| **View > Left-to-right scroll: Prices only** (*Exibir > Rolagem da esquerda para a direita: somente preços*) | Usar somente a faixa móvel horizontal de preços. Este é o padrão. |
| **View > Left-to-right scroll: Prices with News** (*Exibir > Rolagem da esquerda para a direita: preços com notícias*) | Usar as duas faixas móveis horizontais. |
| **View > Static view: Prices only** (*Exibir > Visualização estática: somente preços*) | Usar somente blocos estáticos responsivos de cotação. |
| **View > Static view: Prices with News** (*Exibir > Visualização estática: preços com notícias*) | Usar blocos de cotação e a janela estática News separada. |
| **View > Open static news window** (*Exibir > Abrir janela estática de notícias*) | Reabrir a janela News separada depois de fechá-la. Disponível no modo estático quando as notícias estão ativadas. |
| **Language** (*Idioma*) | Escolher um dos 16 idiomas para os menus, o texto de status e o guia completo da Ajuda. Uma janela Ajuda aberta é atualizada imediatamente. |

A visibilidade das linhas, o idioma e os outros valores de configuração são salvos automaticamente.

## Cotações

Abra **Quotes...** (*Cotações...*) no menu de contexto. Cada entrada configurada representa um
símbolo e uma página da Web. Símbolos duplicados são permitidos e permanecem independentes, pois
cada entrada tem sua própria fonte, seletores, opções de coleta e alertas.

### Início rápido com a amostra publicada

Quando não há entradas, a janela Quotes oferece **Import sample quotes from GitHub** (*Importar cotações de amostra do GitHub*).
Esse comando baixa a amostra do repositório e substitui as configurações atuais do aplicativo.
Revise cada URL importada e os termos atuais de cada site antes de usá-lo. Depois, você pode
editar ou remover qualquer entrada de amostra.

**Import Sample Quotes Config** (*Importar configuração de cotações de amostra*), na parte superior das janelas Quotes e App Settings,
faz a mesma coisa a qualquer momento, mediante confirmação:

- O SmartTicker pergunta **Are you sure?** (*Tem certeza?*) e avisa que o download substitui suas
	cotações, grupos de cotações, aprovações de fontes, visualização, aparência e outras configurações existentes do aplicativo.
	As regras de alerta ficam em um arquivo próprio e não são excluídas.
- **Export existing config...** (*Exportar configuração existente...*) é opcional. Ele salva sua configuração atual em um
	arquivo JSON local e retorna à mesma confirmação.
- **Import Sample Quotes Config** (*Importar configuração de cotações de amostra*) baixa a amostra da Internet e substitui
	sua configuração.
- **Cancel** (*Cancelar*) não altera nada.

### Adicionar uma entrada de cotação ou notícia

1. Insira o rótulo **Ticker**, como `MSFT`. O SmartTicker remove espaços nas extremidades e o armazena em
	 letras maiúsculas.
2. Opcionalmente, escolha um **Group** (*Grupo*) existente na lista ou digite um novo nome, como
	 `Nasdaq`, `Precious Metals` ou `Mag 7`. Deixe em branco para **Ungrouped** (*Sem grupo*).
3. Selecione uma predefinição de **Source** (*Fonte*).
4. Insira o **URL suffix** (*Sufixo da URL*) ou uma URL completa ao usar **Custom URL** (*URL personalizada*).
5. Selecione **Price** (*Preço*), **News** (*Notícias*) ou ambos em **Collect** (*Coletar*). Pelo menos um é obrigatório.
6. Insira os seletores manualmente, use os botões de descoberta ou deixe os seletores opcionais
	 em branco para usar a detecção incorporada.
7. Selecione **Validate URL** (*Validar URL*) para testar o preço normal e/ou as manchetes.
8. Se o SmartTicker solicitar aprovação da fonte, revise o site e confirme somente quando
	 você tiver permissão para coletar dados dele.
9. Selecione **Add independent entry** (*Adicionar entrada independente*). O SmartTicker salva a entrada e atualiza seus
	 dados habilitados imediatamente.

### Agrupar cotações

Um grupo é uma coleção nomeada que você define. Ele não está vinculado a uma bolsa ou categoria
incorporada, portanto você pode organizar entradas por mercado, tipo de ativo, estratégia, carteira,
região ou qualquer outro esquema. Os nomes têm os espaços das extremidades removidos, podem usar Unicode e conter até
80 caracteres. Cada cotação pode pertencer a, no máximo, um grupo.

Use **Manage groups** (*Gerenciar grupos*) ao lado do campo Group ou selecione **Quote groups...** (*Grupos de cotações...*) no
menu de contexto do ticker. A janela tem três áreas de trabalho:

- À esquerda, insira um **Group name** (*Nome do grupo*) e escolha **Create** (*Criar*). Selecione um grupo existente,
	edite seu nome e escolha **Update** (*Atualizar*) ou escolha **Delete** (*Excluir*). Grupos vazios são mantidos.
- À direita, selecione uma cotação. Seu grupo atual aparece na coluna **Current group** (*Grupo atual*);
	**Ungrouped** significa que ela não tem associação.
- No centro, escolha **Associate** (*Associar*) depois de selecionar um grupo e uma cotação. Se essa
	cotação já pertencer a outro grupo, o SmartTicker a moverá para o grupo selecionado.
- Escolha **Remove association** (*Remover associação*) para retornar somente a cotação selecionada a **Ungrouped**.
- Excluir um grupo retorna todas as suas cotações a **Ungrouped**. Cotações, fontes, dados atuais
	e alertas não são excluídos.
- Você também pode escolher um grupo existente na lista ao adicionar ou editar uma cotação,
	ou digitar um novo nome de grupo nesse campo.
- Use os controles para cima/baixo em Configured entries (*Entradas configuradas*) para determinar a ordem dos grupos e linhas na
	tabela estática.
- No modo estático, arraste o cabeçalho de um bloco para reordenar grupos completos diretamente. A mesma
	ordem é usada pelas janelas Quotes e News separadas.

A amostra publicada contém seis grupos de exemplo, mas mantém o modo estático desativado por
padrão. Ative a visualização estática depois de importá-la para ver esses grupos como uma tabela.

### Predefinições de fonte e URLs

| Fonte | O que inserir | Política mostrada pelo SmartTicker |
| --- | --- | --- |
| **Yahoo Finance** | Um sufixo após `https://finance.yahoo.com/`, por exemplo `quote/MSFT/`. | É necessária permissão por escrito. Os termos do Yahoo proíbem a coleta automatizada sem permissão prévia. |
| **CNBC** | Um sufixo após `https://www.cnbc.com/`. | Verifique a política atual e as diretivas para robôs do site. |
| **Trading Economics** | Um sufixo após `https://tradingeconomics.com/`. | Prefira uma API documentada ou um feed autorizado e verifique a política atual do site. |
| **Custom URL** (*URL personalizada*) | Uma URL completa de página pública `http://` ou `https://`. | Revise os termos, a política de privacidade e as regras de acesso automatizado do site. |

Somente URLs HTTP e HTTPS absolutas são aceitas. URLs que contêm nomes de usuário ou
senhas incorporados são rejeitadas. Um login no navegador não autoriza o SmartTicker a coletar uma
página, e o SmartTicker não usa sessões autenticadas do navegador.

A linha **Full URL** (*URL completa*) mostra o endereço final produzido pelo prefixo predefinido e seu
sufixo. Verifique-o antes da validação ou descoberta.

### Opções de coleta

- **Price** (*Preço*) solicita o preço normal. Os seletores opcionais de variação, pré-mercado e pós-mercado
	são avaliados na mesma página baixada.
- **News** (*Notícias*) solicita os links de manchetes da página.
- Selecionar ambos permite que uma entrada contribua para as duas áreas do ticker.
- Desmarcar ambos é inválido.

### Referência dos campos de seletor

Um seletor CSS identifica um elemento no HTML estático de uma página da Web. Os seletores são
opcionais, a menos que a detecção automática não consiga encontrar o valor desejado.

| Campo | Valor extraído pelo SmartTicker |
| --- | --- |
| **Price selector** (*Seletor de preço*) | Preço normal ou de fechamento. |
| **Price change** (*Variação do preço*) | Variação percentual da sessão normal. Quando em branco, tenta-se a detecção de variação incorporada. |
| **Pre-market selector** (*Seletor de pré-mercado*) | Preço de pré-mercado, quando essa sessão existe na página. |
| **Pre-market change** (*Variação de pré-mercado*) | Variação percentual de pré-mercado. |
| **After-hours selector** (*Seletor de pós-mercado*) | Preço pós-mercado ou fora do horário regular. |
| **After-hours change** (*Variação de pós-mercado*) | Variação percentual pós-mercado ou fora do horário regular. |
| **News selector** (*Seletor de notícias*) | Links de manchetes. Selecione uma âncora ou um contêiner cujos resultados incluam links. |

Os valores de pré-mercado e pós-mercado complementam o preço normal; eles não o substituem.
Uma página pode omitir esses elementos fora da sessão de mercado correspondente.

Exemplos de seletores do Yahoo Finance usados pela amostra publicada:

```text
Price:                  [data-testid="qsp-price"]
Price change:           section.primary span[data-testid="qsp-price-change-percent"]
Pre-market price:       section.secondary span[data-testid="qsp-pre-price"]
Pre-market change:      section.secondary span[data-testid="qsp-pre-price-change-percent"]
After-hours price:      section.secondary span[data-testid="qsp-post-price"]
After-hours change:     section.secondary span[data-testid="qsp-post-price-change-percent"]
```

A marcação dos sites muda com o tempo. Trate os exemplos como pontos de partida, não como
contratos permanentes.

### Descobrir seletores

Cada campo de seletor tem um botão **Discover** (*Descobrir*) correspondente.

1. Preencha a URL de origem e aprove o site se a aprovação for necessária.
2. Selecione o botão de descoberta para o tipo exato de valor.
3. O SmartTicker baixa o HTML estático público e lista possíveis seletores com um valor de
	 amostra, porcentagem de confiança e motivo na dica de ferramenta.
4. Selecione **Use** (*Usar*) ao lado de uma sugestão para copiá-la para o campo correspondente.
5. Valide ou observe o resultado antes de confiar nele.

A descoberta não executa JavaScript, não entra em contas, não contorna controles de acesso nem inspeciona seu
navegador. Um valor disponível apenas por JavaScript pode não ter um seletor detectável. Tipos de descoberta
separados evitam deliberadamente misturar valores de pré-mercado e pós-mercado.

### Validar uma fonte

**Validate URL** (*Validar URL*) solicita a página e informa o preço normal e/ou a quantidade de
manchetes que consegue ler. É seguro usá-lo antes de inserir um ticker, pois o SmartTicker
usa um rótulo temporário para o teste.

Atualmente, essa validação não verifica os quatro campos de seletor de pré-mercado e pós-mercado.
Use os valores de amostra da descoberta e depois confirme os dados de sessão exibidos.

As falhas comuns incluem erro HTTP, tempo limite, valor ausente, zero manchetes, permissão da fonte
não aprovada, conteúdo disponível apenas por JavaScript ou um seletor desatualizado.

### Limite de repetição de notícias

**Show max _N_ times** (*Mostrar no máximo _N_ vezes*) aceita de 1 a 100 e o padrão é 5. O SmartTicker conta uma
exibição para cada ciclo concluído de atualização de News no qual o mesmo título de manchete é
retornado. Depois que o título aparece no número de ciclos configurado, ele é retirado
pelo restante da sessão atual do aplicativo. Editar ou remover essa entrada apaga
seu histórico de repetição.

### Editar, ordenar e remover entradas

A lista **Configured entries** (*Entradas configuradas*) mostra símbolo, grupo, fonte, URL, indicadores de coleta,
seletor de preço normal, seletor de notícias e limite de repetição de notícias.

- **Edit** (*Editar*) carrega a entrada no formulário. Selecione **Save changes** (*Salvar alterações*) para aplicá-la ou
	**Cancel edit** (*Cancelar edição*) para descartar as alterações do formulário.
- Os botões de seta para cima e para baixo alteram a ordem do ticker e a salvam imediatamente.
- **Remove** (*Remover*) exclui a entrada e seus dados atualmente exibidos.
- Se houver regras de alerta direcionadas à entrada, o SmartTicker perguntará se deve excluí-las. Um
	alerta sem uma cotação configurada correspondente não pode disparar.
- Renomear uma entrada atualiza os símbolos exibidos nas regras de alerta vinculadas a ela.

## Configurações do aplicativo

Abra **App Settings...** (*Configurações do aplicativo...*) no menu de contexto. As alterações entram em vigor e são salvas
automaticamente; não há botão Apply (*Aplicar*).

### Linhas e velocidade do ticker

| Configuração | Opções | Padrão | Efeito |
| --- | --- | --- | --- |
| Linhas de preço | 1 a 8 | 1 | Quantidade de linhas paralelas da faixa móvel de preços. |
| Velocidade de rolagem dos preços | 20, 30, 40, 50, 65, 80, 100 ou 120 px/sec | 50 | Velocidade da faixa móvel de preços. |
| Linhas de notícias | 1 a 8 | 1 | Quantidade de linhas paralelas da faixa móvel de manchetes. |
| Velocidade de rolagem das notícias | 20, 30, 40, 50, 65, 80, 100 ou 120 px/sec | 40 | Velocidade da faixa móvel de notícias. |
| Tamanho da fonte com rolagem | 9 a 24 pt | 14 pt | Texto de Price e News nas linhas com rolagem. |
| Tamanho da fonte estática | 9 a 24 pt | 13 pt | Texto de cotação e manchete nas linhas estáticas. |
| Atualização de preços | 30 a 300 segundos, em etapas de 15 segundos | 60 segundos | Tempo em que cada entrada de preço permitida recebe uma atualização agendada. |
| Atualização de notícias | 30 a 300 segundos, em etapas de 15 segundos | 300 segundos | Tempo em que cada entrada de News permitida recebe uma atualização agendada. |

As linhas de preço e a velocidade de rolagem dos preços ficam desativadas enquanto as tabelas agrupadas estáticas estão ativas,
pois esse modo exibe todas as entradas de preço e nunca rola automaticamente nenhuma das janelas.
As configurações de linha e velocidade de News são mantidas para a visualização com rolagem.

As solicitações de Price e News são distribuídas de forma independente por intervalos de um segundo durante
todo o período, em vez de começarem juntas. Por exemplo, 60 entradas em 30 segundos
agendam duas entradas por segundo; cinco entradas em 30 segundos agendam aproximadamente uma
a cada seis segundos. No máximo quatro solicitações de fonte são executadas ao mesmo tempo, trabalhos duplicados para a mesma
entrada e fluxo são ignorados, e intervalos perdidos não são repetidos em uma rajada. **Refresh
prices now** (*Atualizar preços agora*) ou **Refresh news now** (*Atualizar notícias agora*) reinicia somente esse fluxo e solicita seu primeiro intervalo.
Os preços e as manchetes obtidos com sucesso permanecem visíveis enquanto os dados substitutos são lidos.

Cada solicitação HTTP tem um tempo limite fixo de 20 segundos. Uma fonte lenta não prende o dispatcher da
interface nem impede que intervalos posteriores usem a capacidade restante de solicitações. O SmartTicker
informa falhas como HTTP 403 e 429 e não contorna restrições. Ele não analisa nem aplica automaticamente
diretivas para robôs, valores de crawl-delay ou instruções de espera do servidor; portanto, escolha fontes em conformidade e evite
solicitações desnecessariamente frequentes.

### Tamanhos das janelas

App Settings armazena três pares de tamanhos independentes:

| Janela | Largura | Altura | Padrão |
| --- | --- | --- | --- |
| Visualização com rolagem | 420–7680 px | 50–900 px | 980 × 64 px |
| Visualização estática de Prices | 420–7680 px | 420–4320 px | 980 × 420 px |
| Visualização estática de News | 420–7680 px | 240–4320 px | 680 × 340 px |

Alterar um valor o aplica imediatamente quando essa janela ou visualização está ativa. A amostra publicada
demonstra 1200 × 96 com rolagem, 1200 × 720 para Prices estático e 760 × 480 para News estático,
com texto de rolagem de 15 pontos e texto estático de 14 pontos. Uma altura de rolagem inferior ao
espaço exigido pelas linhas habilitadas é aumentada automaticamente até o mínimo necessário.

Use as quatro opções em **View** (*Exibir*) para escolher se News será exibido e se o
layout terá rolagem ou permanecerá estático. Alterar a visualização nunca exclui entradas configuradas.

### Iniciar o SmartTicker ao entrar

Ative **Start SmartTicker when I sign in** (*Iniciar o SmartTicker quando eu entrar*) para registrar o executável instalado somente para
o usuário atual.

- No Windows, o SmartTicker usa a chave `Run` do Registro do usuário atual.
- Em desktops Linux compatíveis com a convenção de inicialização automática do freedesktop, o SmartTicker
	grava `smartticker.desktop` no diretório de inicialização automática do usuário.
- A opção fica desativada em plataformas nas quais o SmartTicker não possui um mecanismo de registro
	compatível.

O sistema operacional é a autoridade. Se a inicialização for alterada fora do SmartTicker, a
caixa de seleção refletirá o estado do sistema operacional na próxima vez que as configurações forem carregadas.

### Acesso a sites

**Allow website cookies and cross-host redirects** (*Permitir cookies de sites e redirecionamentos entre hosts*) fica desativado por padrão.

Quando desativado:

- O SmartTicker exige uma aprovação explícita para cada host de site antes de solicitá-lo.
- Cookies de sites não são aceitos.
- Redirecionamentos para outro host são bloqueados.
- Os hosts aprovados são lembrados nas configurações locais.

Quando ativado:

- O SmartTicker ignora a etapa de aprovação por host.
- Os cookies definidos pelos sites solicitados são mantidos somente em um contêiner isolado na memória
	e desaparecem quando o SmartTicker é encerrado.
- Redirecionamentos para outros hosts podem ser seguidos.
- O SmartTicker ainda não lê cookies do navegador, não envia credenciais nem envia
	formulários de entrada.

Desativar essa opção remove os dados exibidos no momento provenientes de fontes não aprovadas,
até que esses hosts sejam aprovados e atualizados.

#### Opções de privacidade dos sites

Se uma resposta for reconhecida como um formulário de privacidade/cookies que contenha opções positivas e
negativas, o SmartTicker pausa e exibe o título da página, a URL solicitada,
a URL de consentimento, o resumo do formulário e os rótulos Accept/Reject do site.

- **Accept** (*Aceitar*) envia os campos ocultos fornecidos pelo formulário e o controle Accept exato
	que você selecionou.
- **Reject** (*Rejeitar*) envia esses campos ocultos e o controle Reject exato que você selecionou.
- **Cancel** (*Cancelar*) não envia nada.

Essa é uma opção de privacidade do site, não a aprovação de permissão por fonte do SmartTicker.

#### Validar todas as fontes

Selecione **Validate all sources** (*Validar todas as fontes*) para revisar e testar cada entrada configurada.

1. Se o acesso a sites estiver restrito, o SmartTicker agrupa as entradas não aprovadas por nome de host
	 e exibe uma caixa de diálogo de revisão da fonte por host.
2. Revise o host, o resumo da política, as orientações, os nomes das fontes e os símbolos.
3. Marque a confirmação somente se você revisou o site e tem permissão para usá-lo.
4. Escolha **Approve this source** (*Aprovar esta fonte*), **Skip this source** (*Ignorar esta fonte*) ou **Cancel validation** (*Cancelar validação*).
5. O SmartTicker testa cada entrada permitida e informa os totais de aprovadas, reprovadas e ignoradas.
	 Os problemas individuais aparecem abaixo da linha de status.

Os registros de aprovação registram a permissão dentro do SmartTicker; eles não concedem direitos legais nem
substituem os termos do site.

### Aparência

**Window transparency** (*Transparência da janela*) altera somente o fundo do ticker. O texto permanece opaco. O
intervalo é de 20% a 100%, em etapas de 5%, e o padrão é 100%.

Os campos de cor aceitam valores hexadecimais `#RRGGBB` e também oferecem um seletor de cores.

| Cor | Padrão | Usada para |
| --- | --- | --- |
| Fundo | `#10151D` | Fundo do ticker antes da aplicação da transparência. |
| Nome da cotação | `#79C0FF` | Rótulo de símbolo/fonte. |
| Preço de fechamento | `#FFA657` | Preço normal. |
| Fora do horário | `#00E5FF` | Preços de pré-mercado e pós-mercado. |
| 1ª notícia | `#FFFFFF` | Manchetes 1, 5, 9 e assim por diante. |
| 2ª notícia | `#00E5FF` | Manchetes 2, 6, 10 e assim por diante. |
| 3ª notícia | `#A3E635` | Manchetes 3, 7, 11 e assim por diante. |
| 4ª notícia | `#79C0FF` | Manchetes 4, 8, 12 e assim por diante. |
| Alta | `#3FB950` | Variações percentuais positivas. |
| Baixa | `#F85149` | Variações percentuais negativas. |
| Piscar do alerta | `#FF00FF` | Alertas de preço disparados, alternando com preto. |

**Reset to defaults** (*Restaurar padrões*) restaura todas as cores acima e 100% de opacidade do fundo. Ele não
redefine linhas, velocidades, tamanhos de fonte, tamanhos de janela, fontes, intervalos de atualização, alertas ou
idioma.

### Backup e restauração

O SmartTicker mantém as configurações do aplicativo e as regras de alerta em arquivos JSON separados e
fornece botões separados para cada tipo de backup.

#### Exportar e importar configurações

- **Export settings...** (*Exportar configurações...*) grava as entradas configuradas, associações de grupos, definições de grupos,
	cotações de notícias ocultas, ordem das entradas, seletores, a opção de visualização de cotações com rolagem/estática,
	hosts aprovados, visibilidade das linhas, linhas, velocidades, tamanhos de fonte com rolagem/estática, os três
	pares de tamanhos de janela, intervalos de atualização, preferência de inicialização,
	opção de acesso a sites, cores incluindo a cor de piscar do alerta, transparência e
	idioma.
- **Import settings...** (*Importar configurações...*) valida o arquivo inteiro antes de alterar qualquer coisa. Um arquivo rejeitado
	mantém as configurações atuais inalteradas.
- Uma importação bem-sucedida substitui todas as entradas configuradas e preferências do aplicativo. Ela
	não substitui o arquivo separado de regras de alerta.
- Os grupos são incluídos como associações de cotações no arquivo de configurações, junto com as próprias
	definições dos grupos, de modo que um grupo sem cotações também sobrevive a um backup. Não há
	arquivo separado de exportação ou importação somente de grupos.
- A preferência de inicialização está presente no backup das configurações, mas importá-la não
	altera silenciosamente o registro de inicialização do sistema operacional. O sistema operacional continua sendo a autoridade;
	use a caixa de seleção Startup (*Inicialização*) para alterar o registro no computador atual.
- Os arquivos de importação são limitados a 1 MiB, versão 1 do esquema e, no máximo, 200 assinaturas.
	Propriedades desconhecidas, IDs duplicados, URLs malformadas, cores inválidas, intervalos inválidos
	ou códigos de idioma sem suporte são rejeitados, em vez de ignorados silenciosamente.

#### Exportar e importar regras de alerta- **Export alert rules...** (*Exportar regras de alerta...*) grava todas as regras, além de Buzz, contagem de buzzes e duração do piscar.
- **Import alert rules...** (*Importar regras de alerta...*) valida o arquivo inteiro e, em seguida, substitui todas as regras atuais
	e as configurações de disparo de alertas.
- Primeiro, as regras são reconectadas pelo ID da assinatura. Quando os IDs diferem, o SmartTicker tenta uma
	correspondência de símbolo sem diferenciar maiúsculas de minúsculas.
- Uma regra importada sem cotação correspondente é mantida, mas não pode disparar. O status da importação
	informa quantas regras foram vinculadas novamente ou permanecem sem correspondência.
- Os arquivos de importação de alertas são limitados a 1 MiB.

Para transferir para outro computador, importe primeiro as configurações do aplicativo e depois as regras de alerta.
Importar os alertas por último permite que as regras sejam reconectadas aos novos IDs de assinatura pelo símbolo.

### Editar os arquivos de configuração no local

**Edit Current App Config** (*Editar configuração atual do aplicativo*) e **Edit Current Alert Rules** (*Editar regras de alerta atuais*) em App Settings abrem o
arquivo JSON ativo no editor de texto associado a `.json` em seu sistema. Isso é destinado a
usuários avançados; as janelas do SmartTicker abrangem as mesmas configurações sem esse risco.

Os dois botões primeiro mostram uma confirmação que solicita a exportação do arquivo atual. Faça
essa exportação: a edição manual pode danificar o arquivo, e não há como desfazer.

- **Export existing config...** (*Exportar configuração existente...*) salva o arquivo atual e retorna à mesma pergunta.
- **Open in text editor** (*Abrir no editor de texto*) abre o arquivo ativo.
- **Cancel** (*Cancelar*) não altera nada.

O SmartTicker monitora o arquivo e o recarrega assim que seu editor salva:

- Um arquivo válido é aplicado imediatamente, e o ticker é atualizado sem reiniciar.
- JSON malformado, uma violação do esquema ou qualquer outro erro de validação é rejeitado. Sua
	configuração em execução permanece intacta, e a janela App Settings informa o
	problema.
- Depois de uma edição rejeitada, corrija o arquivo ou restaure uma exportação válida com
	**Import settings...** (*Importar configurações...*) ou **Import alert rules...** (*Importar regras de alerta...*).
- Um arquivo que permanece bloqueado por outro programa é tentado novamente por alguns instantes e depois informado.

A edição do arquivo de regras de alerta segue as mesmas regras e não afeta as configurações do
aplicativo, pois os dois arquivos são separados.

## Regras de alerta

Abra **Alerts** (*Alertas*) no menu de contexto. As regras são avaliadas após cada atualização de preço
bem-sucedida e monitoram somente o preço normal, não os valores de pré-mercado ou pós-mercado.

### Criar uma regra

1. Selecione uma **Quote** (*Cotação*) configurada. As entradas com o mesmo símbolo permanecem distintas.
2. Selecione uma **Condition** (*Condição*) e insira um limite numérico usando um decimal invariável, como
	 `250.50`.
3. Opcionalmente, escolha **Active from** (*Ativo a partir de*). Deixe em branco para ativar imediatamente.
4. Mantenha **Never expires** (*Nunca expira*) marcado ou desmarque-o e escolha uma data de expiração.
5. Selecione **Add rule** (*Adicionar regra*).

As comparações disponíveis são:

| Opção | Significado |
| --- | --- |
| `LessThan` | Preço `<` limite. |
| `LessThanOrEqual` | Preço `<=` limite. |
| `GreaterThan` | Preço `>` limite. |
| `GreaterThanOrEqual` | Preço `>=` limite. |
| `EqualTo` | O preço é exatamente igual ao limite. |
| `NotEqualTo` | O preço é diferente do limite. |

O limite inicial é inclusivo. O limite de expiração também é inclusivo; depois que ele
passa, a regra deixa de disparar. O SmartTicker rejeita uma expiração anterior ao início.

### Quando uma regra dispara

Uma regra habilitada e agendada dispara uma vez quando sua condição muda de falsa para verdadeira.
Ela não notifica a cada atualização enquanto a condição continua verdadeira. Depois que o preço
sai da condição, a regra é rearmada e pode disparar quando o preço voltar a atendê-la.

Editar uma regra ou desabilitá-la e habilitá-la novamente também a rearma. Portanto, uma regra habilitada
pode disparar imediatamente se o preço normal mais recente já atender à sua
condição. Um preço ausente ou cuja obtenção falhou não pode disparar uma regra.

Quando uma ou mais regras disparam:

- A entrada de preço afetada alterna entre a cor configurada de piscar do alerta e o preto durante
	o período configurado. A cor de piscar padrão é magenta (`#FF00FF`).
- Se **Buzz** estiver habilitado, o SmartTicker reproduzirá a sequência de buzz configurada.
- A mensagem de alerta identifica uma regra ou informa o número de regras disparadas juntas.
- A rolagem do ticker continua enquanto o destaque do alerta está ativo.

### Configurações de saída de alerta

| Configuração | Intervalo | Padrão |
| --- | --- | --- |
| **Buzz** | Ligado ou desligado | Ligado |
| Contagem de buzzes | 1 a 20 | 15 |
| **Blink for** (*Piscar por*) | 5 a 900 segundos, em etapas de 15 segundos | 60 segundos |

Desabilitar Buzz mantém o alerta visual ativo. Se várias regras dispararem na mesma
avaliação, o SmartTicker iniciará uma sequência de buzz configurada para essa avaliação.
Altere **Alert blink** (*Piscar do alerta*) em **App Settings > Appearance** (*Configurações do aplicativo > Aparência*). Essa é uma preferência de
aparência do aplicativo; por isso, a exportação/importação das Settings a inclui, e não o arquivo separado
de regras de alerta.

### Gerenciar regras configuradas

- **Edit** (*Editar*) carrega uma regra no formulário. Selecione **Update rule** (*Atualizar regra*) para salvar ou **Cancel** (*Cancelar*) para
	deixá-la inalterada.
- **Disable** (*Desabilitar*) mantém a regra, mas impede que ela corresponda. **Enable** (*Habilitar*) a rearma e
	a avalia em relação ao preço normal mais recente.
- **Remove** (*Remover*) exclui a regra.
- A lista mostra o estado habilitado, o símbolo, o resumo da condição e a agenda.

As alterações nas regras de alerta e nas configurações de saída de alerta são salvas automaticamente.

## Arquivos locais e privacidade

O SmartTicker armazena a configuração localmente e não a sincroniza com um serviço do
desenvolvedor.

No Windows, os arquivos padrão são:

```text
%LocalAppData%\SmartTicker\settings.json
%LocalAppData%\SmartTicker\alerts.json
```

No Linux, o .NET usa o diretório local de dados do aplicativo do usuário atual, normalmente:

```text
~/.local/share/SmartTicker/settings.json
~/.local/share/SmartTicker/alerts.json
```

### Usar um diretório de dados isolado

Os diagnósticos avançados e as execuções de teste podem definir `SMARTTICKER_DATA_DIRECTORY` antes de iniciar o
SmartTicker. Quando o valor não está em branco, os dois arquivos são colocados diretamente nesse diretório resolvido
como `settings.json` e `alerts.json`; os padrões da plataforma acima não são usados
nesse processo. Prefira um caminho absoluto e verifique se ele permite gravação.

Exemplo do PowerShell:

```powershell
$env:SMARTTICKER_DATA_DIRECTORY = 'D:\SmartTicker-Profile'
& 'C:\Program Files\SmartTicker\SmartTicker.Desktop.exe'
```

Exemplo do shell do Linux:

```bash
SMARTTICKER_DATA_DIRECTORY="$HOME/.local/share/SmartTicker-Test" smartticker
```

Defina a variável antes de iniciar o processo. O SmartTicker não copia o perfil padrão
para o diretório selecionado; portanto, um diretório vazio começa com uma configuração vazia.
As instâncias direcionadas ao mesmo diretório podem observar as edições salvas umas das outras. Use os
comandos normais de exportação/importação de Settings e Alert Rules para backups e transferência de perfil.

A janela Alerts exibe o caminho exato do arquivo de alertas em uso. As gravações usam um arquivo
temporário seguido de substituição, para que um arquivo parcialmente gravado não seja tratado como a
configuração atual.

O SmartTicker não tem conta, telemetria, análise, publicidade nem sincronização em nuvem. Um site de
origem recebe informações normais de rede, como seu endereço IP, quando o SmartTicker
solicita essa fonte. Abrir a Ajuda solicita o guia bruto do GitHub. Para obter todos os
detalhes, leia `PRIVACY.md` no repositório.

Você é responsável por garantir que cada URL de origem e seletor seja usado de
acordo com os termos, a licença, as diretivas para robôs e a legislação aplicável do site.

## Solução de problemas

### Uma cotação mostra indisponível ou não mostra preço

Uma solicitação de fonte atinge o tempo limite após 20 segundos. Se essa cotação tiver um snapshot anterior
bem-sucedido, uma atualização com falha a manterá visível; caso contrário, a cotação mostrará **Unavailable** (*Indisponível*)
até que uma atualização posterior seja bem-sucedida. Leia o erro de validação ou atualização antes de alterar os
seletores.

1. Abra **Quotes...** (*Cotações...*), edite a entrada e verifique Full URL (*URL completa*).
2. Confirme se **Price** (*Preço*) está selecionado.
3. Aprove o site se solicitado.
4. Selecione **Validate URL** (*Validar URL*) e leia o resultado exato.
5. Execute **Discover price** (*Descobrir preço*) ou inspecione o HTML estático da página e atualize o seletor.
6. Verifique se a página exige JavaScript, autenticação ou consentimento que o
	 SmartTicker não possa tratar com segurança.
7. Respeite HTTP 403, 429, restrições para robôs e a política de acesso automatizado do site.

### Faltam dados de pré-mercado ou pós-mercado

- A sessão de mercado correspondente pode não estar ativa.
- A página pode omitir o elemento da sessão quando não existe um valor de sessão.
- Verifique se os seletores de pré-mercado apontam para elementos de pré-mercado e se os seletores de pós-mercado
	apontam para elementos pós-mercado.
- Execute novamente o comando de descoberta correspondente, pois a marcação do site pode ter mudado.

### As notícias estão vazias

- Confirme se **News** (*Notícias*) está selecionado.
- Valide a fonte e execute **Discover news** (*Descobrir notícias*).
- Verifique se o seletor retorna links com texto visível de manchete.
- Uma solicitação de News com falha ou tempo esgotado mantém manchetes anteriores obtidas com sucesso, quando disponíveis.
	Uma fonte sem resultado bem-sucedido permanece vazia até que um intervalo posterior tenha sucesso.
- Uma manchete desaparece depois de atingir o limite de repetição configurado para esta sessão.
- Em News estático, confirme se a cotação desejada está marcada em **Show news for** (*Mostrar notícias de*).

### A descoberta de seletores não encontra nada

A descoberta lê somente o HTML estático baixado. Ela não consegue ver valores criados posteriormente pelo
JavaScript da página. Insira manualmente um seletor verificado, escolha uma página/feed estático ou use
uma API documentada e autorizada por meio de uma página pública compatível.

### Um alerta não dispara

- Confirme se a cotação vinculada ainda existe, coleta Price e tem um preço normal
	obtido com sucesso.
- Confirme se a regra está Enabled (*Habilitada*) e dentro de sua agenda de início/expiração.
- Verifique a comparação e o limite. `EqualTo` exige igualdade decimal exata.
- Lembre-se de que uma condição continuamente verdadeira dispara uma vez; ela deve se tornar falsa antes de
	poder disparar novamente, a menos que você edite ou reabilite a regra.
- Os preços de pré-mercado e pós-mercado não acionam regras de alerta.

### Não é possível mover ou redimensionar o SmartTicker

- Mova somente pela alça de pontos verticais na faixa esquerda.
- Redimensione por uma borda ou canto; use a marca visível no canto inferior direito se for difícil
	localizar uma borda.
- O conteúdo do ticker não é intencionalmente uma superfície de movimento.

### Os grupos ou valores estáticos não são os esperados

- Abra **Quotes...** (*Cotações...*) e confirme o valor de Group de cada entrada.
- Abra **Quote groups...** (*Grupos de cotações...*) para gerenciar as definições de grupo e revisar a associação
	atual de cada cotação.
- As entradas com Group em branco aparecem em **Ungrouped** (*Sem grupo*).
- **Chg** é calculado com base em Last e Chg%; ele não é extraído de forma independente da
	página. Ele permanece `—` quando a porcentagem não está disponível.
- Reordene as entradas com os controles para cima/baixo para alterar a ordem dos grupos e linhas.
- Arraste a alça pontilhada no cabeçalho de um bloco para mover o grupo inteiro. Solte-a na metade esquerda
  de outro bloco para posicioná-lo antes, ou na metade direita para posicioná-lo depois.
- Selecione **Refresh prices now** (*Atualizar preços agora*) enquanto o SmartTicker não estiver pausado para atualizar a tabela.

### O texto da Ajuda não está formatado ou a navegação não se move

- A janela Ajuda deve mostrar títulos, parágrafos, listas, tabelas, links e
	blocos de código formatados, em vez de pontuação Markdown.
- Use **On this page** (*Nesta página*) à esquerda para ir a uma seção principal. Os links na tabela de Navegação
	rápida também rolam dentro do documento.
- Feche e reabra a Ajuda, ou altere **Language** (*Idioma*), para solicitar o guia publicado
	correspondente. Enquanto ele não chega, o SmartTicker exibe o guia formatado incorporado
	ao aplicativo instalado.

### A Ajuda online está indisponível ou desatualizada

- Feche e reabra a Ajuda para solicitar novamente o guia publicado.
- Abra no navegador o endereço bruto do GitHub mostrado perto do início deste guia para
	inspecionar diretamente o arquivo publicado.
- O SmartTicker usa o guia incorporado quando a solicitação falha ou retorna um arquivo vazio.
- As alterações online aparecem somente depois que `HELPME.md` ou o arquivo localizado correspondente
  `help/HELPME.<language-code>.md` é publicado na ramificação `main` do repositório.

## Suporte

Relate problemas reproduzíveis em:

<https://github.com/bulentozkir/smartticker/issues>

Inclua a versão do SmartTicker, o sistema operacional, o nome do host da fonte, o status da validação
e o texto exato do erro. Remova URLs privadas ou outras informações confidenciais antes de publicar.