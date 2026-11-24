namespace ChestCounter;

/// <summary>
/// The page of the graphical interface: one HTML file (styles and script inside), drawn by the script from the state
/// the counter sends (<see cref="GuiServer"/>). Texts come with the state, in the chosen language.
/// </summary>
public static class GuiPage
{
    /// <summary>Every text of the page, in the chosen language.</summary>
    public static Dictionary<string, string> Texts() => new()
    {
        ["home"] = L.T("Accueil", "Home", "Inicio", "Início"),
        ["report"] = L.T("Rapport", "Report", "Informe", "Relatório"),
        ["overlay"] = L.T("Overlay", "Overlay", "Overlay", "Overlay"),
        ["settings"] = L.T("Réglages", "Settings", "Ajustes", "Configurações"),
        ["help"] = L.T("Aide", "Help", "Ayuda", "Ajuda"),
        ["total"] = L.T("Coffres au total", "Chests in total", "Cofres en total", "Baús no total"),
        ["totalSub"] = L.T("ouverts ou détruits par toi", "opened or destroyed by you", "abiertos o destruidos por ti", "abertos ou destruídos por você"),
        ["byHand"] = L.T("dont {0} ajoutés à la main", "{0} of them added by hand", "de ellos, {0} añadidos a mano", "{0} deles adicionados manualmente"),
        ["session"] = L.T("Session", "Session", "Sesión", "Sessão"),
        ["last"] = L.T("Dernière partie", "Last match", "Última partida", "Última partida"),
        ["squad"] = L.T("Total d'équipe", "Squad total", "Total del equipo", "Total do esquadrão"),
        ["squadSub"] = L.T("toi + {0} par tes coéquipiers", "you + {0} by your teammates", "tú + {0} de tus compañeros", "você + {0} dos seus colegas de equipe"),
        ["inaccurate"] = L.T("imprécis", "inaccurate", "impreciso", "impreciso"),
        ["inaccurateWhy"] = L.T("Compté dans TES replays : un coffre ouvert par un coéquipier à plus de 150 m de toi n'y est pas, et rien n'est enregistré après ton départ de la partie. Le vrai total de l'équipe est plus haut.",
            "Counted in YOUR replays: a chest a teammate opens more than 150 m away from you is not in them, and nothing is recorded once you leave the match. The real squad total is higher.",
            "Contado en TUS repeticiones: un cofre que un compañero abre a más de 150 m de ti no está en ellas, y no se graba nada después de que salgas de la partida. El total real del equipo es mayor.", "Contado nos SEUS replays: um baú que um colega abre a mais de 150 m de você não aparece neles, e nada é gravado depois que você sai da partida. O total real do esquadrão é maior."),
        ["treasures"] = L.T("Trésors", "Treasures", "Tesoros", "Tesouros"),
        ["matches"] = L.T("parties", "matches", "partidas", "partidas"),
        ["perMatch"] = L.T("{0} par partie", "{0} per match", "{0} por partida", "{0} por partida"),
        ["openReport"] = L.T("Ouvrir le rapport", "Open the report", "Abrir el informe", "Abrir o relatório"),
        ["showOverlay"] = L.T("Afficher l'overlay", "Show the overlay", "Mostrar el overlay", "Mostrar o overlay"),
        ["hideOverlay"] = L.T("Masquer l'overlay", "Hide the overlay", "Ocultar el overlay", "Esconder o overlay"),
        ["lock"] = L.T("Verrouiller l'overlay", "Lock the overlay", "Bloquear el overlay", "Travar o overlay"),
        ["unlock"] = L.T("Déverrouiller pour le déplacer", "Unlock to move it", "Desbloquear para moverlo", "Destravar para mover"),
        ["activity"] = L.T("Activité", "Activity", "Actividad", "Atividade"),
        ["noActivity"] = L.T("Rien pour l'instant.", "Nothing yet.", "Nada por ahora.", "Nada ainda."),
        ["analyzing"] = L.T("Analyse de tes replays…", "Analyzing your replays…", "Analizando tus repeticiones…", "Analisando seus replays…"),
        ["whatsNew"] = L.T("Nouveautés de la {0}", "What's new in {0}", "Novedades de la {0}", "Novidades da {0}"),
        ["newVersion"] = L.T("Nouvelle version {0} disponible", "New version {0} available", "Nueva versión {0} disponible", "Nova versão {0} disponível"),
        ["install"] = L.T("Installer", "Install", "Instalar", "Instalar"),
        ["checkUpdate"] = L.T("Rechercher une mise à jour", "Check for updates", "Buscar actualizaciones", "Procurar atualizações"),
        ["welcome"] = L.T("Bienvenue !", "Welcome!", "¡Bienvenido!", "Bem-vindo!"),
        ["welcomeText"] = L.T("Active d'abord les rediffusions dans Fortnite : Paramètres → Jeu → Rediffusions → Enregistrer les rediffusions. Ensuite, laisse le compteur ouvert : chaque partie est comptée quelques secondes après sa fin. Des coffres ouverts avant ? Le crayon de « Coffres au total » permet de les ajouter.",
            "First turn on replays in Fortnite: Settings → Game → Replays → Record Replays. Then keep the counter open: each match is counted a few seconds after it ends. Chests opened before? The pencil of \"Chests in total\" adds them.",
            "Primero activa las repeticiones en Fortnite: Ajustes → Juego → Repeticiones → Grabar repeticiones. Luego deja el contador abierto: cada partida se cuenta unos segundos después de terminar. ¿Cofres abiertos antes? El lápiz de «Cofres en total» permite añadirlos.", "Primeiro, ative os replays no Fortnite: Configurações → Jogo → Replays → Gravar Replays. Depois, deixe o contador aberto: cada partida é contada alguns segundos depois de terminar. Abriu baús antes? O lápis de \"Baús no total\" adiciona eles."),
        ["ok"] = L.T("OK", "OK", "OK", "OK"),
        ["moreInfo"] = L.T("En savoir plus", "More info", "Más información", "Mais informações"),
        ["wMin"] = L.T("Réduire", "Minimize", "Minimizar", "Minimizar"),
        ["wMax"] = L.T("Agrandir", "Maximize", "Maximizar", "Maximizar"),
        ["wRestore"] = L.T("Niveau inférieur", "Restore down", "Restaurar", "Restaurar"),
        ["wClose"] = L.T("Fermer", "Close", "Cerrar", "Fechar"),
        ["keepOpen"] = L.T("Garder le compteur ouvert", "Keep the counter open", "Mantener el contador abierto", "Manter o contador aberto"),
        ["autoCloseText"] = L.T("Fortnite est fermé : le compteur, ouvert avec Fortnite, va se fermer tout seul.", "Fortnite is closed: the counter, opened with Fortnite, is about to close by itself.", "Fortnite está cerrado: el contador, abierto con Fortnite, se va a cerrar solo.", "O Fortnite foi fechado: o contador, aberto junto com o Fortnite, vai fechar sozinho."),
        ["welcomeQuick"] = L.T("Tes premiers réglages (tu les retrouves à tout moment dans Réglages) :", "Your first settings (you find them at any time in Settings):", "Tus primeros ajustes (los encuentras en cualquier momento en Ajustes):", "Suas primeiras configurações (você encontra elas a qualquer momento em Configurações):"),
        ["welcomeDone"] = L.T("C'est parti !", "Let's go!", "¡Vamos!", "Bora!"),
        ["inGame"] = L.T("Overlay en jeu", "In-game overlay", "Overlay en el juego", "Overlay no jogo"),
        ["inGameHelp"] = L.T("Petite fenêtre toujours au premier plan qui affiche tes coffres en jeu (Fortnite en « Plein écran fenêtré »). Ctrl+Maj+O : afficher / masquer, Ctrl+Maj+L : verrouiller.",
            "Small always-on-top window that shows your chests in game (Fortnite in \"Windowed Fullscreen\"). Ctrl+Shift+O: show / hide, Ctrl+Shift+L: lock.",
            "Pequeña ventana siempre visible que muestra tus cofres en el juego (Fortnite en «Pantalla completa en ventana»). Ctrl+Mayús+O: mostrar / ocultar, Ctrl+Mayús+L: bloquear.", "Janelinha sempre por cima que mostra seus baús no jogo (Fortnite em \"Tela Cheia em Janela\"). Ctrl+Shift+O: mostrar / esconder, Ctrl+Shift+L: travar."),
        ["style"] = L.T("Style", "Style", "Estilo", "Estilo"),
        ["hud"] = L.T("HUD (intégré au jeu)", "HUD (blends into the game)", "HUD (integrado en el juego)", "HUD (se mistura ao jogo)"),
        ["classic"] = L.T("Classique (carte sombre)", "Classic (dark card)", "Clásico (tarjeta oscura)", "Clássico (cartão escuro)"),
        ["size"] = L.T("Taille", "Size", "Tamaño", "Tamanho"),
        ["opacity"] = L.T("Opacité", "Opacity", "Opacidad", "Opacidade"),
        ["squadToggle"] = L.T("Afficher le total d'équipe", "Show the squad total", "Mostrar el total del equipo", "Mostrar o total do esquadrão"),
        ["squadHelp"] = L.T("Tes coffres + ceux de tous tes coéquipiers (même aléatoires), sur l'overlay et l'accueil.",
            "Your chests + those of all your teammates (random ones too), on the overlay and the home tab.",
            "Tus cofres + los de todos tus compañeros (también los aleatorios), en el overlay y el inicio.", "Seus baús + os de todos os seus colegas (inclusive aleatórios), no overlay e na aba Início."),
        ["obs"] = L.T("Overlay pour OBS (stream)", "Overlay for OBS (stream)", "Overlay para OBS (directo)", "Overlay para o OBS (stream)"),
        ["obsSteps"] = L.T("Dans OBS : Sources → + → Navigateur, coche « Fichier local », choisis ce fichier. Largeur 360, hauteur 220 (450 × 300 en classique). Pour ne l'avoir que sur le stream, désactive l'overlay en jeu : la page continue d'être mise à jour.",
            "In OBS: Sources → + → Browser, tick \"Local file\", pick this file. Width 360, height 220 (450 × 300 for classic). To have it on the stream only, turn the in-game overlay off: the page keeps being updated.",
            "En OBS: Fuentes → + → Navegador, marca «Archivo local», elige este archivo. Ancho 360, alto 220 (450 × 300 en clásico). Para tenerlo solo en el directo, desactiva el overlay en el juego: la página se sigue actualizando.", "No OBS: Fontes → + → Navegador, marque \"Arquivo local\" e escolha este arquivo. Largura 360, altura 220 (450 × 300 no clássico). Para ele aparecer só na stream, desligue o overlay no jogo: a página continua sendo atualizada."),
        ["copy"] = L.T("Copier le chemin", "Copy the path", "Copiar la ruta", "Copiar o caminho"),
        ["copied"] = L.T("Copié !", "Copied!", "¡Copiado!", "Copiado!"),
        ["preview"] = L.T("Aperçu", "Preview", "Vista previa", "Prévia"),
        ["language"] = L.T("Langue", "Language", "Idioma", "Idioma"),
        ["times"] = L.T("Heures affichées", "Times shown in", "Horas mostradas en", "Horários mostrados em"),
        ["timesHelp"] = L.T("Dans le rapport et ici : semaines, heures du trésor, parties.", "In the report and here: weeks, Treasure Hours, matches.", "En el informe y aquí: semanas, horas del tesoro, partidas.", "No relatório e aqui: semanas, Horas do Tesouro, partidas."),
        ["timesLocal"] = L.T("Heure locale ({0})", "Local time ({0})", "Hora local ({0})", "Horário local ({0})"),
        ["timesUtc"] = L.T("UTC (heure du jeu)", "UTC (game time)", "UTC (hora del juego)", "UTC (horário do jogo)"),
        ["perf"] = L.T("Mode performance", "Performance mode", "Modo rendimiento", "Modo desempenho"),
        ["perfRecommended"] = L.T("conseillé pour ce PC", "recommended for this PC", "recomendado para este PC", "recomendado para este PC"),
        ["perfHelp"] = L.T("Pour les PC modestes : pas de lecture du replay pendant la partie (les coffres sont comptés à la fin), priorité minimale, fenêtre fermée en mémoire quand elle est réduite, rapport allégé et pas de page OBS.",
            "For low-end PCs: no reading of the replay during the match (the chests are counted at the end), lowest priority, window released from memory when minimized, lean report and no OBS page.",
            "Para PC modestos: sin lectura de la repetición durante la partida (los cofres se cuentan al final), prioridad mínima, ventana liberada de la memoria al minimizarla, informe reducido y sin página de OBS.",
            "Para PCs mais fracos: não lê o replay durante a partida (os baús são contados no final), prioridade mínima, janela liberada da memória ao minimizar, relatório enxuto e sem página do OBS."),
        ["obsPerf"] = L.T("Désactivé en mode performance (Réglages).", "Off in performance mode (Settings).", "Desactivado en modo rendimiento (Ajustes).", "Desligado no modo desempenho (Configurações)."),
        ["goals"] = L.T("Objectifs", "Goals", "Metas", "Metas"),
        ["goalsHelp"] = L.T("Un nombre de coffres à ouvrir : affiché sur l'overlay, l'accueil et le rapport. 0 = pas d'objectif.",
            "A number of chests to open: shown on the overlay, the home tab and the report. 0 = no goal.",
            "Un número de cofres que abrir: se muestra en el overlay, el inicio y el informe. 0 = sin meta.",
            "Um número de baús para abrir: aparece no overlay, no Início e no relatório. 0 = sem meta."),
        ["goalDaily"] = L.T("Objectif du jour", "Daily goal", "Meta del día", "Meta do dia"),
        ["goalWeekly"] = L.T("Objectif de la semaine (depuis lundi)", "Weekly goal (from Monday)", "Meta de la semana (desde el lunes)", "Meta da semana (a partir de segunda)"),
        ["achievements"] = L.T("Succès", "Achievements", "Logros", "Conquistas"),
        ["brand"] = L.T("Compteur de coffres", "Chest Counter", "Contador de cofres", "Contador de Baús"),
        ["map"] = L.T("Carte", "Map", "Mapa", "Mapa"),
        ["account"] = L.T("Compte", "Account", "Cuenta", "Conta"),
        ["recent"] = L.T("le plus récent en haut", "newest first", "lo más reciente arriba", "mais recentes no topo"),
        ["today"] = L.T("aujourd'hui", "today", "hoy", "hoje"),
        ["last14"] = L.T("14 derniers jours", "last 14 days", "últimos 14 días", "últimos 14 dias"),
        ["lastShort"] = L.T("Dernière", "Last", "Última", "Última"),
        ["avgShort"] = L.T("Moyenne", "Average", "Media", "Média"),
        ["every"] = L.T("1 tous les {0} coffres", "1 every {0} chests", "1 cada {0} cofres", "1 a cada {0} baús"),
        ["openInBrowser"] = L.T("Ouvrir dans le navigateur", "Open in the browser", "Abrir en el navegador", "Abrir no navegador"),
        ["achievementsSub"] =L.T("détails dans le rapport", "details in the report", "detalles en el informe", "detalhes no relatório"),
        ["avgTile"] = L.T("Moyenne par partie", "Average per match", "Media por partida", "Média por partida"),
        ["inMatches"] = L.T("sur {0} parties", "over {0} matches", "en {0} partidas", "em {0} partidas"),
        ["goalDone"] = L.T("objectif atteint", "goal reached", "meta cumplida", "meta batida"),
        ["goalLeft"] = L.T("encore {0} coffres", "{0} chests to go", "faltan {0} cofres", "faltam {0} baús"),
        ["discordHelp"] = L.T("Optionnel : colle le webhook d'un salon (Paramètres du salon → Intégrations → Webhooks) pour y publier un résumé après chaque partie. Rien d'autre n'est envoyé ; vide = désactivé.",
            "Optional: paste a channel's webhook (Channel settings → Integrations → Webhooks) to post a summary there after each match. Nothing else is sent; empty = off.",
            "Opcional: pega el webhook de un canal (Ajustes del canal → Integraciones → Webhooks) para publicar un resumen tras cada partida. No se envía nada más; vacío = desactivado.",
            "Opcional: cole o webhook de um canal (Configurações do canal → Integrações → Webhooks) para postar um resumo depois de cada partida. Nada mais é enviado; vazio = desligado."),
        ["discordUrl"] = L.T("Webhook du salon", "Channel webhook", "Webhook del canal", "Webhook do canal"),
        ["discordTest"] = L.T("Tester", "Test", "Probar", "Testar"),
        ["discordSent"] = L.T("Message de test envoyé : regarde le salon.", "Test message sent: check the channel.", "Mensaje de prueba enviado: mira el canal.", "Mensagem de teste enviada: confira o canal."),
        ["discordMode"] = L.T("Publier", "Post", "Publicar", "Postar"),
        ["discordAll"] = L.T("Chaque partie", "Every match", "Cada partida", "Toda partida"),
        ["discordNotable"] = L.T("Seulement trésors, succès et objectifs", "Only treasures, achievements and goals", "Solo tesoros, logros y metas", "Só tesouros, conquistas e metas"),
        ["saved"] = L.T("Enregistré.", "Saved.", "Guardado.", "Salvo."),
        ["theme"] = L.T("Thème", "Theme", "Tema", "Tema"),
        ["themeHelp"] = L.T("Couleurs de la fenêtre, du rapport et de l'overlay.", "Colors of the window, the report and the overlay.",
            "Colores de la ventana, del informe y del overlay.", "Cores da janela, do relatório e do overlay."),
        ["clock"] = L.T("Horloge", "Clock", "Reloj", "Relógio"),
        ["clockAuto"] = L.T("Comme Windows ({0})", "Same as Windows ({0})", "Como Windows ({0})", "Igual ao Windows ({0})"),
        ["clock12"] = L.T("12 heures (10:54 PM)", "12-hour (10:54 PM)", "12 horas (10:54 PM)", "12 horas (10:54 PM)"),
        ["clock24"] = L.T("24 heures (22:54)", "24-hour (22:54)", "24 horas (22:54)", "24 horas (22:54)"),
        ["h12"] = L.T("12 h", "12-hour", "12 h", "12 horas"),
        ["h24"] = L.T("24 h", "24-hour", "24 h", "24 horas"),
        ["tray"] = L.T("Réduire dans la zone de notification", "Minimize to the notification area", "Minimizar al área de notificación", "Minimizar para a área de notificação"),
        ["trayHelp"] = L.T("Oui : réduire la fenêtre la range à côté de l'horloge, en bas à droite de la barre des tâches (icône du coffre, parfois derrière la petite flèche ^). Non : elle reste dans la barre des tâches comme une fenêtre normale. Dans les deux cas, le compteur continue de compter.",
            "Yes: minimizing the window puts it next to the clock, at the bottom right of the taskbar (chest icon, sometimes behind the small ^ arrow). No: it stays in the taskbar like a normal window. Either way, the counter keeps counting.",
            "Sí: minimizar la ventana la coloca junto al reloj, abajo a la derecha de la barra de tareas (icono del cofre, a veces detrás de la pequeña flecha ^). No: se queda en la barra de tareas como una ventana normal. En ambos casos, el contador sigue contando.", "Sim: minimizar a janela coloca ela ao lado do relógio, no canto inferior direito da barra de tarefas (ícone de baú, às vezes atrás da setinha ^). Não: ela fica na barra de tarefas como uma janela normal. Nos dois casos, o contador continua contando."),
        ["footerWindow"] = L.T("La croix de la fenêtre ferme le compteur (après confirmation).", "The close button of the window stops the counter (after asking).", "La cruz de la ventana cierra el contador (tras confirmarlo).", "O botão de fechar da janela para o contador (depois de perguntar)."),
        ["withFortnite"] = L.T("Ouvrir avec Fortnite", "Open with Fortnite", "Abrir con Fortnite", "Abrir com o Fortnite"),
        ["withFortniteHelp"] = L.T("Un petit processus invisible attend Fortnite et ouvre le compteur, qui se ferme après.", "A tiny invisible process waits for Fortnite and opens the counter, which closes afterwards.", "Un pequeño proceso invisible espera a Fortnite y abre el contador, que se cierra después.", "Um processo minúsculo e invisível espera o Fortnite e abre o contador, que fecha depois."),
        ["updates"] = L.T("Mises à jour", "Updates", "Actualizaciones", "Atualizações"),
        ["upPrompt"] = L.T("Proposées", "Offered", "Propuestas", "Oferecidas"),
        ["upAuto"] = L.T("Installées automatiquement", "Installed automatically", "Instaladas automáticamente", "Instaladas automaticamente"),
        ["upOff"] = L.T("Désactivées", "Off", "Desactivadas", "Desligadas"),
        ["profile"] = L.T("Compte (profil)", "Account (profile)", "Cuenta (perfil)", "Conta (perfil)"),
        ["demos"] = L.T("Dossier des replays", "Replays folder", "Carpeta de repeticiones", "Pasta dos replays"),
        ["demosMissing"] = L.T("Introuvable : Fortnite le crée à la première partie enregistrée.", "Not found: Fortnite creates it with the first recorded match.", "No encontrada: Fortnite la crea con la primera partida grabada.", "Não encontrada: o Fortnite cria ela com a primeira partida gravada."),
        ["save"] = L.T("Enregistrer", "Save", "Guardar", "Salvar"),
        ["editChests"] = L.T("Modifier mon nombre de coffres", "Edit my number of chests", "Modificar mi número de cofres", "Editar meu número de baús"),
        ["close"] = L.T("Fermer", "Close", "Cerrar", "Fechar"),
        ["manual"] = L.T("Coffres ajoutés à la main", "Chests added by hand", "Cofres añadidos a mano", "Baús adicionados manualmente"),
        ["manualHelp"] = L.T("Les coffres ouverts avant d'utiliser le compteur. Chaque nombre s'ajoute à ce que tu as déjà ajouté (un nombre négatif en retire) ; la croix annule un ajout.", "The chests opened before using the counter. Each number is added to what you already added (a negative number removes some); the cross undoes an addition.", "Los cofres abiertos antes de usar el contador. Cada número se suma a lo que ya añadiste (un número negativo quita cofres); la cruz anula un añadido.", "Os baús abertos antes de usar o contador. Cada número é somado ao que você já adicionou (um número negativo remove); o X desfaz uma adição."),
        ["manualTotal"] = L.T("Ajoutés à la main : {0}", "Added by hand: {0}", "Añadidos a mano: {0}", "Adicionados manualmente: {0}"),
        ["addBtn"] = L.T("Ajouter", "Add", "Añadir", "Adicionar"),
        ["removeEntry"] = L.T("Annuler cet ajout", "Undo this addition", "Anular este añadido", "Desfazer esta adição"),
        ["thisWeek"] = L.T("Ouverts cette semaine", "Opened this week", "Abiertos esta semana", "Abertos nesta semana"),
        ["thisWeekHelp"] = L.T("comptent aussi pour les trésors hebdo de la semaine {0}", "also count for the weekly treasures of week {0}", "cuentan también para los tesoros semanales de la semana {0}", "também contam para os tesouros semanais da semana {0}"),
        ["estimate"] = L.T("Ou estimer à partir de tes parties", "Or estimate from your matches", "O estimar a partir de tus partidas", "Ou estimar pelas suas partidas"),
        ["estimateHelp"] = L.T("Nombre de parties Battle Royale de la saison, dans Fortnite : Carrière → Statistiques.", "Number of Battle Royale matches this season, in Fortnite: Career → Stats.", "Número de partidas de Battle Royale de la temporada, en Fortnite: Carrera → Estadísticas.", "Número de partidas de Battle Royale nesta temporada, no Fortnite: Carreira → Estatísticas."),
        ["career"] = L.T("Parties jouées", "Matches played", "Partidas jugadas", "Partidas jogadas"),
        ["compute"] = L.T("Calculer", "Compute", "Calcular", "Calcular"),
        ["add"] = L.T("Ajouter {0} coffres", "Add {0} chests", "Añadir {0} cofres", "Adicionar {0} baús"),
        ["estimateTooFew"] = L.T("Il faut au moins {0} parties comptées pour une moyenne fiable.", "At least {0} counted matches are needed for a reliable average.", "Hacen falta al menos {0} partidas contadas para una media fiable.", "São necessárias pelo menos {0} partidas contadas para uma média confiável."),
        ["estimateNothing"] = L.T("Rien à ajouter : tes parties comptées couvrent déjà ce nombre.", "Nothing to add: your counted matches already cover that number.", "Nada que añadir: tus partidas contadas ya cubren ese número.", "Nada a adicionar: suas partidas contadas já cobrem esse número."),
        ["dataFolder"] = L.T("Ouvrir le dossier des données", "Open the data folder", "Abrir la carpeta de datos", "Abrir a pasta de dados"),
        ["errorLog"] = L.T("Ouvrir le journal d'erreurs", "Open the error log", "Abrir el registro de errores", "Abrir o log de erros"),
        ["issue"] = L.T("Signaler un problème (GitHub)", "Report a problem (GitHub)", "Informar de un problema (GitHub)", "Informar um problema (GitHub)"),
        ["project"] = L.T("Page du projet (GitHub)", "Project page (GitHub)", "Página del proyecto (GitHub)", "Página do projeto (GitHub)"),
        ["helpText"] = L.T("Le compteur lit les replays que Fortnite enregistre et compte tes coffres quelques secondes après chaque partie (Battle Royale de la saison seulement). Laisse-le ouvert pendant que tu joues : tu peux réduire la fenêtre, il continue de compter.",
            "The counter reads the replays Fortnite saves and counts your chests a few seconds after each match (the season's Battle Royale only). Keep it open while you play: you can minimize the window, it keeps counting.",
            "El contador lee las repeticiones que guarda Fortnite y cuenta tus cofres unos segundos después de cada partida (solo el Battle Royale de la temporada). Déjalo abierto mientras juegas: puedes minimizar la ventana, sigue contando.", "O contador lê os replays que o Fortnite salva e conta seus baús alguns segundos depois de cada partida (só o Battle Royale da temporada). Deixe ele aberto enquanto joga: você pode minimizar a janela, ele continua contando."),
        ["uninstall"] = L.T("Désinstaller ChestCounter", "Uninstall ChestCounter", "Desinstalar ChestCounter", "Desinstalar o ChestCounter"),
        ["keepData"] = L.T("Garder mon historique et mes réglages", "Keep my history and settings", "Conservar mi historial y mis ajustes", "Manter meu histórico e configurações"),
        ["deleteData"] = L.T("Tout supprimer", "Delete everything", "Borrarlo todo", "Apagar tudo"),
        ["uninstallConfirm"] = L.T("Désinstaller maintenant ? Le compteur se ferme et ChestCounter.exe est supprimé.", "Uninstall now? The counter closes and ChestCounter.exe is deleted.", "¿Desinstalar ahora? El contador se cierra y se borra ChestCounter.exe.", "Desinstalar agora? O contador fecha e o ChestCounter.exe é apagado."),
        ["quit"] = L.T("Quitter le compteur", "Quit the counter", "Salir del contador", "Sair do contador"),
        ["quitConfirm"] = L.T("Arrêter le compteur ? Les parties jouées ensuite ne seront comptées qu'à sa prochaine ouverture.", "Stop the counter? Matches played afterwards are only counted when it is opened again.", "¿Detener el contador? Las partidas jugadas después solo se cuentan cuando se vuelva a abrir.", "Parar o contador? As partidas jogadas depois só serão contadas quando ele for aberto de novo."),
        ["closed"] = L.T("Le compteur est arrêté. Tu peux fermer cette fenêtre.", "The counter is stopped. You can close this window.", "El contador está detenido. Puedes cerrar esta ventana.", "O contador está parado. Você pode fechar esta janela."),
        ["offline"] = L.T("Le compteur ne répond plus (fermé ?). Relance ChestCounter.exe.", "The counter does not answer (closed?). Start ChestCounter.exe again.", "El contador no responde (¿cerrado?). Vuelve a abrir ChestCounter.exe.", "O contador não responde (fechado?). Abra o ChestCounter.exe de novo."),
        ["footer"] = L.T("Fermer cette fenêtre ne coupe pas le compteur.", "Closing this window does not stop the counter.", "Cerrar esta ventana no detiene el contador.", "Fechar esta janela não para o contador."),
    };

    public static string Html(string token) => Template.Replace("__TOKEN__", token).Replace("__FONT__", ReportWriter.FontFaceCss).Replace("__PENCIL__", ReportWriter.PencilIcon).Replace("__THEMES__", Themes.Css).Replace("__ICONS__", Icons.Json());

    private const string Template = """
        <!doctype html><html><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
        <title>Chest Counter</title>
        <style>__FONT__
        /* Design tokens. Pirate (default): parchment panels, wooden side bar, Pirata One titles. */
        :root{--bg:#101a22;--bgimg:none;--ink:#f2e6c9;--muted:#b8b0a0;
          --side:linear-gradient(180deg,#33200f,#24170c);--sideline:#4a3218;--sideink:#d9c39b;--sidemuted:#b59a73;--navon:#f2e6c9;--navonink:#2a1c0f;--navicon:#7a4a1e;--logo:#7a4a1e;--logoink:#ffd36a;--logoline:#e7b440;
          --surface:#f2e6c9;--sink:#2a1c0f;--smuted:#6b5a44;--sline:#d8c59c;--strack:#e4d3ad;--sdash:dashed;--kicon:#a9701f;--accent:#b9821c;--ring:#b9821c;
          --btn:#7a4a1e;--btnink:#fbecc4;--btnline:#e7b440;--ghost:transparent;--ghostink:#f2e6c9;--ghostline:#c9a24f;
          --herobg:radial-gradient(120% 140% at 100% 0,#93602c 0,#7a4a1e 55%,#62391a 100%);--heroline:#e7b440;--heroink:#ffd36a;--herotext:#f3dfb4;--herokey:#f3cf7a;--herobar:rgba(255,211,106,.28);--herobarhi:#ffd36a;--herostrong:#ffffff;
          --title:#f0c057;--ptitle:#7a4a1e;--tfont:"Pirata One","Segoe UI",serif;--tweight:400;--tstyle:normal;--ttrans:none;--tsize:36px;--psize:22px;--nstyle:normal;
          --field:#fffaf0;--fieldink:#2a1c0f;--fieldline:#d8c59c;--optbg:#fffaf0;--optink:#2a1c0f;
          --ok:#2f8a4a;--warnc:#b87510;--err:#c0392b;--tre:#7b4fc9;--info:#2a7f9e;--team:#1f7f7a;--warn:#a3361f;--glass:none;--shadow:none}
        :root[data-accent="noite"]{--bg:#0b1016;--ink:#e7edf4;--muted:#7d8ea3;
          --side:#0e151d;--sideline:#18222e;--sideink:#9fb0c3;--sidemuted:#6f8196;--navon:#18222e;--navonink:#ffffff;--navicon:#f2b84b;--logo:#f2b84b;--logoink:#1a1206;--logoline:#f2b84b;
          --surface:#121a24;--sink:#e7edf4;--smuted:#8b9bb0;--sline:#1c2734;--strack:#223041;--sdash:solid;--kicon:#f2b84b;--accent:#f2b84b;--ring:#f2b84b;
          --btn:#f2b84b;--btnink:#1a1206;--btnline:#f2b84b;--ghost:#18222e;--ghostink:#cfd9e4;--ghostline:#18222e;
          --herobg:#121a24;--heroline:#1c2734;--heroink:#ffffff;--herotext:#8b9bb0;--herokey:#f2b84b;--herobar:#243243;--herobarhi:#f2b84b;--herostrong:#5fd38d;
          --title:#ffffff;--ptitle:#e7edf4;--tfont:"Segoe UI",system-ui,sans-serif;--tweight:700;--tsize:26px;--psize:16px;
          --field:#0e151d;--fieldink:#e7edf4;--fieldline:#243243;--optbg:#121a24;--optink:#e7edf4;
          --ok:#5fd38d;--warnc:#f2b84b;--err:#ff6b5b;--tre:#b48cff;--info:#5bb8e0;--team:#3fbf8f;--warn:#ff6b5b}
        :root[data-accent="arena"]{--bg:#0d0b2b;--bgimg:radial-gradient(80% 60% at 85% 0,#3b1d8f 0,transparent 60%),radial-gradient(70% 60% at 0 100%,#0f4f9c 0,transparent 60%);--ink:#ffffff;--muted:#a9a6e0;
          --side:rgba(255,255,255,.04);--sideline:rgba(255,255,255,.08);--sideink:#c6c3f5;--sidemuted:#a9a6e0;--navon:linear-gradient(90deg,#ffe14d,#ffc21c);--navonink:#1d1240;--navicon:#1d1240;--logo:linear-gradient(135deg,#ffe14d,#ff9f1c);--logoink:#2a1300;--logoline:transparent;
          --surface:rgba(255,255,255,.06);--sink:#ffffff;--smuted:#a9a6e0;--sline:rgba(255,255,255,.1);--strack:rgba(255,255,255,.12);--sdash:solid;--kicon:#ffe14d;--accent:#ffe14d;--ring:#ffe14d;
          --btn:linear-gradient(90deg,#ffe14d,#ffc21c);--btnink:#1d1240;--btnline:transparent;--ghost:rgba(255,255,255,.08);--ghostink:#ffffff;--ghostline:rgba(255,255,255,.08);
          --herobg:linear-gradient(120deg,#2b6bff 0%,#7a3cff 60%,#b42cff 100%);--heroline:transparent;--heroink:#ffffff;--herotext:#e8e4ff;--herokey:#e8e4ff;--herobar:rgba(255,255,255,.28);--herobarhi:#ffe14d;--herostrong:#ffe14d;
          --title:#ffffff;--ptitle:#ffffff;--tfont:Bahnschrift,"Segoe UI",sans-serif;--tweight:700;--tstyle:italic;--ttrans:uppercase;--tsize:28px;--psize:16px;--nstyle:italic;
          --field:rgba(255,255,255,.08);--fieldink:#ffffff;--fieldline:rgba(255,255,255,.16);--optbg:#1b1640;--optink:#ffffff;
          --ok:#3ff0a0;--warnc:#ffe14d;--err:#ff6b7a;--tre:#d4b0ff;--info:#7fd3ff;--team:#3ff0a0;--warn:#ff6b7a;--glass:blur(8px);--shadow:0 18px 50px rgba(90,60,255,.35)}
        :root[data-accent="brasil"]{--bg:#062b1b;--ink:#eaf4e4;--muted:#a8c6b0;--side:#04200f;--sideline:#0d3d24;--sideink:#cfe6d4;--sidemuted:#8fb39a;--navon:#ffdf00;--navonink:#062b1b;--navicon:#002776;--logo:#002776;--logoink:#ffdf00;--logoline:#ffdf00;
          --surface:#f6f8ee;--sink:#0d2617;--smuted:#4d6a55;--sline:#cfdcc2;--strack:#e3ecd6;--kicon:#1f9e4f;--accent:#1f9e4f;--ring:#1f9e4f;--btn:#002776;--btnink:#ffffff;--btnline:#ffdf00;--ghostink:#eaf4e4;--ghostline:#ffdf00;
          --herobg:linear-gradient(135deg,#003a9c,#002776);--heroline:#ffdf00;--heroink:#ffdf00;--herotext:#e6eeff;--herokey:#a9c6ff;--herobar:rgba(255,223,0,.28);--herobarhi:#ffdf00;--title:#ffdf00;--ptitle:#002776;--field:#ffffff;--fieldink:#0d2617;--fieldline:#cfdcc2}
        :root[data-accent="neon"]{--bg:#0e0a1c;--ink:#ece6ff;--muted:#a99fc9;--side:#140d29;--sideline:#2b1d4f;--sideink:#d6c9ff;--sidemuted:#9d8fc7;--navon:#ff5fd8;--navonink:#1a0b2e;--navicon:#1a0b2e;--logo:#2b0f52;--logoink:#5ef1ff;--logoline:#ff5fd8;
          --surface:#f5f0ff;--sink:#1d1233;--smuted:#5f5480;--sline:#ddd0f5;--strack:#ebe2fb;--kicon:#b45cff;--accent:#b45cff;--ring:#b45cff;--btn:#2b0f52;--btnink:#f3e8ff;--btnline:#ff5fd8;--ghostink:#ece6ff;--ghostline:#ff5fd8;
          --herobg:linear-gradient(135deg,#3a1170,#2b0f52);--heroline:#ff5fd8;--heroink:#5ef1ff;--herotext:#f3e8ff;--herokey:#ff9be9;--herobar:rgba(94,241,255,.28);--herobarhi:#5ef1ff;--title:#ff5fd8;--ptitle:#2b0f52;--field:#ffffff;--fieldink:#1d1233;--fieldline:#ddd0f5}
        :root[data-accent="gelo"]{--bg:#0b1a2b;--ink:#e6f1fb;--muted:#9fb6cc;--side:#08131f;--sideline:#163049;--sideink:#cfe3f5;--sidemuted:#8fa9c2;--navon:#7fd3ff;--navonink:#0b1a2b;--navicon:#0f3c66;--logo:#0f3c66;--logoink:#bff0ff;--logoline:#7fd3ff;
          --surface:#f1f7fc;--sink:#0f2236;--smuted:#4d6680;--sline:#cfe0ee;--strack:#e2edf6;--kicon:#3a9be0;--accent:#3a9be0;--ring:#3a9be0;--btn:#0f3c66;--btnink:#e6f4ff;--btnline:#7fd3ff;--ghostink:#e6f1fb;--ghostline:#7fd3ff;
          --herobg:linear-gradient(135deg,#15528a,#0f3c66);--heroline:#7fd3ff;--heroink:#bff0ff;--herotext:#e6f4ff;--herokey:#9ad9ff;--herobar:rgba(191,240,255,.28);--herobarhi:#bff0ff;--title:#7fd3ff;--ptitle:#0f3c66;--field:#ffffff;--fieldink:#0f2236;--fieldline:#cfe0ee}
        :root[data-accent="lava"]{--bg:#1c0b07;--ink:#fbe6dc;--muted:#c9a596;--side:#140704;--sideline:#3a160c;--sideink:#f3d2c4;--sidemuted:#c09584;--navon:#ff8a3d;--navonink:#1c0b07;--navicon:#6e1a0a;--logo:#6e1a0a;--logoink:#ffd166;--logoline:#ff8a3d;
          --surface:#fdf1ea;--sink:#2d120a;--smuted:#7a4f40;--sline:#f0d3c4;--strack:#f7e2d6;--kicon:#e8541e;--accent:#e8541e;--ring:#e8541e;--btn:#6e1a0a;--btnink:#ffe9dc;--btnline:#ff8a3d;--ghostink:#fbe6dc;--ghostline:#ff8a3d;
          --herobg:linear-gradient(135deg,#8a2410,#6e1a0a);--heroline:#ff8a3d;--heroink:#ffd166;--herotext:#ffe9dc;--herokey:#ffb184;--herobar:rgba(255,209,102,.28);--herobarhi:#ffd166;--title:#ff7a2f;--ptitle:#6e1a0a;--field:#ffffff;--fieldink:#2d120a;--fieldline:#f0d3c4}
        *{box-sizing:border-box}
        html,body{height:100%}
        body{margin:0;background:var(--bg);background-image:var(--bgimg);background-attachment:fixed;color:var(--ink);font:15px/1.5 "Segoe UI",system-ui,sans-serif}
        .ic{display:inline-block;flex:none;width:18px;height:18px}
        .num{font-family:Bahnschrift,"Segoe UI",sans-serif;font-stretch:75%;font-variant-numeric:tabular-nums;font-style:var(--nstyle)}
        .ttl{font-family:var(--tfont);font-weight:var(--tweight);font-style:var(--tstyle);text-transform:var(--ttrans)}
        .hidden{display:none!important}
        /* ---- Shell: side bar + content ---- */
        #shell{display:flex;min-height:100vh}
        #side{width:204px;flex:none;display:flex;flex-direction:column;gap:4px;padding:22px 16px 18px;background:var(--side);border-right:1px solid var(--sideline);position:sticky;top:0;height:100vh}
        .brand{display:flex;align-items:center;gap:11px;padding:0 6px 20px}
        .brand .logo{width:40px;height:40px;border-radius:11px;display:grid;place-items:center;background:var(--logo);color:var(--logoink);border:1.5px solid var(--logoline);flex:none}
        .brand .logo .ic{width:23px;height:23px}
        .brand b{display:block;font-size:20px;line-height:1.05;color:var(--title)}
        .brand small{color:var(--sidemuted);font-size:12px}
        .navi{display:flex;align-items:center;gap:12px;width:100%;padding:10px 12px;border:0;border-radius:10px;background:transparent;color:var(--sideink);font:600 14.5px "Segoe UI",system-ui,sans-serif;cursor:pointer;text-align:left}
        :root[data-accent="arena"] .navi{font-family:Bahnschrift,"Segoe UI";font-style:italic;text-transform:uppercase;letter-spacing:.03em}
        .navi:hover{background:color-mix(in srgb,var(--sideink) 10%,transparent)}
        .navi.on{background:var(--navon);color:var(--navonink)}.navi.on .ic{color:var(--navicon)}
        .navi:focus-visible{outline:2px solid var(--accent);outline-offset:2px}
        .sfoot{margin-top:auto;display:flex;flex-direction:column;gap:10px;padding-top:14px}
        .status{display:flex;align-items:center;gap:9px;font-size:13.5px;font-weight:600;padding:10px 12px;border-radius:10px;background:color-mix(in srgb,var(--sideink) 9%,transparent);color:var(--sideink)}
        .status i{width:9px;height:9px;border-radius:50%;flex:none;background:#3fbf6f;box-shadow:0 0 0 4px rgba(63,191,111,.18)}
        .status.live i{background:#ff3b30;box-shadow:0 0 0 4px rgba(255,59,48,.2)}.status.busy i{background:#e0a526;box-shadow:0 0 0 4px rgba(224,165,38,.2)}.status.warn i{background:#c8412f;box-shadow:0 0 0 4px rgba(200,65,47,.2)}
        .acct{font-size:12.5px;color:var(--sidemuted);padding:0 6px}
        .acct select{width:100%;margin-top:4px;min-height:34px;font-size:13px}
        .sbtn{display:flex;align-items:center;gap:8px;padding:8px 10px;border:1px solid var(--sideline);border-radius:9px;background:transparent;color:var(--sidemuted);font:600 13px "Segoe UI",system-ui,sans-serif;cursor:pointer}
        .sbtn:hover{color:var(--err);border-color:var(--err)}.sbtn .ic{width:16px;height:16px}
        #scroll{flex:1;min-width:0}
        main{display:grid;grid-template-columns:minmax(0,1fr);gap:18px;padding:24px 24px 32px;max-width:1360px}
        main.home{grid-template-columns:minmax(0,1fr) 292px}
        #app{display:flex;flex-direction:column;gap:18px;min-width:0}
        #reportPane{grid-column:1/-1}
        #rep{width:100%;min-height:400px;border:0;display:block;background:transparent;overflow:hidden;border-radius:12px}
        @media (max-width:1100px){main.home{grid-template-columns:minmax(0,1fr)}#activityPane{position:static;max-height:420px}}
        @media (max-width:760px){#side{width:68px;padding:18px 10px}#side .brand div,.navi span,.sfoot .acct,.status span,.sbtn span{display:none}.brand{padding:0 0 18px;justify-content:center}.navi{justify-content:center;padding:10px}.status{justify-content:center}main{padding:20px 16px}}
        /* ---- Page header ---- */
        .top{display:flex;justify-content:space-between;align-items:flex-end;gap:14px;flex-wrap:wrap}
        .top h1{margin:0;font-size:var(--tsize);line-height:1.05;color:var(--title)}
        .top .sub{font-size:13.5px;color:var(--muted);margin-top:4px}
        .btnrow{display:flex;gap:10px;flex-wrap:wrap}
        h2.sec{margin:6px 0 -6px;font-size:calc(var(--psize) + 4px);color:var(--title);line-height:1.2}
        /* ---- Buttons and fields ---- */
        button.b{display:inline-flex;align-items:center;gap:8px;min-height:40px;padding:8px 16px;border-radius:10px;border:1px solid var(--ghostline);background:var(--ghost);color:var(--ghostink);font:600 14px "Segoe UI",system-ui,sans-serif;cursor:pointer}
        .panel button.b,.card button.b,dialog button.b{border-color:var(--sline);background:transparent;color:var(--sink)}
        button.b:hover{filter:brightness(1.08)}
        button.b.main,.panel button.b.main,.card button.b.main,dialog button.b.main{background:var(--btn);color:var(--btnink);border-color:var(--btnline);font-weight:700}
        :root[data-accent="arena"] button.b.main{font-family:Bahnschrift,"Segoe UI";font-style:italic;text-transform:uppercase}
        button.b.danger,.card button.b.danger{border-color:var(--err);color:var(--err);background:transparent}
        button.b .ic{width:17px;height:17px}
        select,input[type=text],input[type=number]{min-height:40px;padding:6px 10px;border-radius:8px;border:1px solid var(--fieldline);background:var(--field);color:var(--fieldink);font:15px "Segoe UI",system-ui,sans-serif}
        select option{background:var(--optbg);color:var(--optink)}
        input[type=text]{width:min(520px,100%)}
        .switch{position:relative;display:inline-block;width:52px;height:30px;flex:none;vertical-align:middle}
        .switch input{opacity:0;width:100%;height:100%;margin:0;cursor:pointer;position:absolute;z-index:1}
        .switch span{position:absolute;inset:0;border-radius:15px;background:var(--strack);transition:background .15s}
        .switch span:after{content:"";position:absolute;top:3px;left:3px;width:24px;height:24px;border-radius:50%;background:#fff;box-shadow:0 1px 3px rgba(0,0,0,.25);transition:transform .15s}
        .switch input:checked+span{background:var(--team)}.switch input:checked+span:after{transform:translateX(22px)}
        .switch input:focus-visible+span{outline:3px solid var(--accent)}
        .badge{display:inline-block;margin-left:6px;padding:1px 8px;border-radius:999px;background:#f6d8c9;color:#a3361f;font-size:12px;font-weight:700;vertical-align:1px;cursor:help;text-transform:none;letter-spacing:0;font-style:normal}
        .perfbadge{display:inline-block;margin-left:6px;padding:1px 8px;border-radius:999px;background:var(--team);color:#fff;font-size:12px;font-weight:700;vertical-align:1px}
        /* ---- Panels ---- */
        .card,.panel,.kpi,.goal{background:var(--surface);color:var(--sink);border:1px solid var(--sline);border-radius:14px;backdrop-filter:var(--glass)}
        .card{padding:16px 18px}
        .row{display:flex;align-items:center;justify-content:space-between;gap:16px;padding:12px 0;border-bottom:1px solid var(--sline)}
        .row:last-child{border-bottom:none}
        .row .t{font-weight:600}.row .h{font-size:13px;color:var(--smuted);margin-top:2px}
        .actions{display:flex;flex-wrap:wrap;gap:8px}
        .note{font-size:13px;color:var(--smuted)}
        .path{font:13px Consolas,monospace;word-break:break-all;background:var(--strack);padding:6px 8px;border-radius:6px;display:block;margin:6px 0}
        .bar{height:8px;background:var(--strack);border-radius:4px;overflow:hidden;margin-top:8px}.bar i{display:block;height:100%;background:var(--accent)}
        /* ---- Home: total, figures, goals ---- */
        .hero{position:relative;border-radius:16px;padding:22px 24px;display:grid;grid-template-columns:auto minmax(170px,1fr);gap:26px;align-items:end;background:var(--herobg);border:2px solid var(--heroline);color:var(--herotext);box-shadow:var(--shadow);overflow:hidden}
        :root[data-accent="noite"] .hero{border-width:1px}
        .hero .k{font-size:12px;font-weight:700;letter-spacing:.12em;text-transform:uppercase;display:flex;gap:8px;align-items:center;color:var(--herokey)}
        .hero .v{font-size:78px;font-weight:700;line-height:1;margin:10px 0 8px;color:var(--heroink)}
        .hero .d{font-size:13.5px}.hero .d b{color:var(--herostrong)}
        .spark{display:flex;align-items:flex-end;gap:5px;height:92px}
        .spark i{flex:1;border-radius:4px 4px 1px 1px;min-height:3px;background:var(--herobar)}
        .spark i.hi{background:var(--herobarhi)}
        .spark-l{display:flex;justify-content:space-between;font-size:11.5px;margin-top:6px;opacity:.85}
        .editbtn{position:absolute;top:12px;right:12px;width:34px;height:34px;padding:0;border-radius:9px;border:1px solid color-mix(in srgb,var(--heroink) 45%,transparent);background:transparent;color:var(--heroink);cursor:pointer;display:grid;place-items:center;z-index:1}
        .editbtn:hover{background:color-mix(in srgb,var(--heroink) 15%,transparent)}
        .kpis{display:grid;grid-template-columns:repeat(auto-fit,minmax(128px,1fr));gap:12px}
        .kpi{padding:14px 16px;min-width:0}
        .kpi .k{font-size:12px;font-weight:600;display:flex;gap:7px;align-items:center;color:var(--smuted)}
        .kpi .k .ic{width:15px;height:15px;color:var(--kicon)}
        :root[data-accent="arena"] .kpi .k{text-transform:uppercase;font-size:11.5px;letter-spacing:.06em}
        .kpi .v{font-size:36px;font-weight:700;line-height:1.1;margin-top:6px}
        .kpi .s{font-size:12.5px;color:var(--smuted);white-space:nowrap;overflow:hidden;text-overflow:ellipsis}
        .goals{display:grid;grid-template-columns:repeat(auto-fit,minmax(190px,1fr));gap:12px}
        .goal{padding:14px;display:flex;gap:13px;align-items:center;min-width:0}
        .goal>div:last-child{min-width:0}
        .ring{width:56px;height:56px;flex:none;position:relative;color:var(--sink)}
        .ring>svg{width:56px;height:56px;transform:rotate(-90deg)}
        .ring circle.bg{stroke:var(--strack)}.ring circle.fg{stroke:var(--ring)}.goal.done .ring circle.fg{stroke:var(--team)}
        .ring span{position:absolute;inset:0;display:grid;place-items:center;font-size:13px;font-weight:700}
        .ring span .ic{width:20px;height:20px;color:var(--team)}
        .goal .t{font-weight:700;font-size:13px;white-space:nowrap}
        .goal .v{font-size:23px;font-weight:700;line-height:1.2;white-space:nowrap}
        .goal .s{font-size:12.5px;color:var(--smuted);white-space:nowrap}
        .goal .ach{display:flex;gap:5px;margin-top:4px}
        .goal .ach i{width:22px;height:22px;border-radius:50%;display:grid;place-items:center;background:color-mix(in srgb,var(--ring) 18%,transparent);color:var(--ring)}
        .goal .ach .ic{width:13px;height:13px}
        /* ---- Activity ---- */
        #activityPane{position:sticky;top:20px;align-self:start;padding:16px 18px;max-height:calc(100vh - 40px);display:flex;flex-direction:column}
        body.windowed #activityPane{max-height:calc(100vh - 74px)}
        #activityPane h3{margin:0 0 4px;display:flex;justify-content:space-between;align-items:baseline;font-size:var(--psize);color:var(--ptitle)}
        #activityPane h3 small{font:600 12px "Segoe UI",system-ui,sans-serif;color:var(--smuted);text-transform:none;font-style:normal}
        .log{overflow:auto;min-height:0;margin:0 -6px;padding:0 6px}
        .feed{list-style:none;margin:0;padding:0}
        .feed>li{display:grid;grid-template-columns:30px minmax(0,1fr) auto;gap:10px;padding:11px 0}
        .feed>li+li{border-top:1px var(--sdash) var(--sline)}
        .feed .fi{width:30px;height:30px;border-radius:9px;display:grid;place-items:center;background:color-mix(in srgb,var(--smuted) 14%,transparent);color:var(--smuted)}
        .feed .fi .ic{width:16px;height:16px}
        .feed .ft{font-size:13.5px;font-weight:600;line-height:1.35;padding-top:5px}
        .feed .fs{display:flex;gap:7px;align-items:flex-start;font-size:12.5px;line-height:1.45;color:var(--smuted);margin-top:5px}
        .feed .fs .ic{width:13px;height:13px;margin-top:2px}
        .feed time{font-size:12px;color:var(--smuted);padding-top:6px}
        .feed .ok .fi{background:color-mix(in srgb,var(--ok) 16%,transparent);color:var(--ok)}.feed .fs.ok .ic{color:var(--ok)}
        .feed .warn .fi{background:color-mix(in srgb,var(--warnc) 18%,transparent);color:var(--warnc)}.feed .fs.warn .ic{color:var(--warnc)}
        .feed .err .fi{background:color-mix(in srgb,var(--err) 16%,transparent);color:var(--err)}.feed .fs.err .ic{color:var(--err)}
        .feed .tre .fi{background:color-mix(in srgb,var(--tre) 18%,transparent);color:var(--tre)}.feed .fs.tre .ic{color:var(--tre)}
        .feed .info .fi{background:color-mix(in srgb,var(--info) 16%,transparent);color:var(--info)}.feed .fs.info .ic{color:var(--info)}
        .feed .fs.ok,.feed .fs.tre{color:var(--sink)}
        .fempty{display:flex;align-items:center;gap:10px;padding:14px 0;color:var(--smuted);font-size:14px}.fempty .ic{width:18px;height:18px}
        /* ---- Notices, dialog, manual chests ---- */
        .notice{position:relative;display:flex;gap:12px;align-items:flex-start;border-left:5px solid var(--accent)}
        .notice.warning{border-left-color:var(--err)}
        .notice .ni{flex:none;width:26px;height:26px;border-radius:50%;display:grid;place-items:center;background:var(--accent);color:#fff;margin-top:1px}.notice .ni .ic{width:15px;height:15px}
        .notice.warning .ni{background:var(--err)}
        .notice .nb{flex:1;min-width:0}.notice .nt{font-weight:700;margin-bottom:2px}.notice .nx{white-space:pre-line}
        .notice .actions{margin-top:10px}
        .notice .nclose{flex:none;width:32px;height:32px;margin:-6px -8px 0 0;border:0;border-radius:8px;background:transparent;color:var(--smuted);font-size:20px;line-height:1;cursor:pointer}
        .card h2.in{margin:0 0 8px;font-size:var(--psize);color:var(--ptitle);line-height:1.2}
        .card ul{margin:6px 0 12px;padding-left:20px}
        dialog{width:min(560px,calc(100% - 32px));padding:20px 22px;border:1px solid var(--sline);border-radius:14px;background:var(--surface);color:var(--sink);backdrop-filter:var(--glass)}
        :root[data-accent="arena"] dialog{background:#1b1640}
        dialog::backdrop{background:rgba(5,10,14,.6)}
        .dhead{display:flex;align-items:center;justify-content:space-between;gap:12px;margin-bottom:6px}
        .dhead h2{margin:0;font-size:var(--psize);color:var(--ptitle)}
        .dclose{width:38px;height:38px;border-radius:9px;border:1px solid var(--sline);background:transparent;color:var(--sink);font-size:22px;line-height:1;cursor:pointer}
        .estimate{margin-top:8px;font-weight:600}
        .mtotal{font-weight:600;margin-bottom:6px}
        .mweek{display:flex;align-items:center;gap:8px;margin-top:8px;cursor:pointer}
        .mweek b{white-space:nowrap}.mweek span{color:var(--smuted);font-size:13px}
        .mhist{list-style:none;margin:10px 0 0;padding:0;max-height:190px;overflow:auto}
        .mhist li{display:flex;align-items:center;gap:12px;padding:5px 2px;border-top:1px solid var(--sline)}
        .mhist b{min-width:70px;text-align:right}.mhist span{flex:1;color:var(--smuted);font-size:13px}
        .mhist .mx{border:0;background:none;color:var(--smuted);font-size:18px;line-height:1;cursor:pointer;padding:2px 7px;border-radius:4px}
        .mhist .mx:hover{color:var(--err)}
        @media (max-width:640px){.row{flex-direction:column;align-items:flex-start}.hero{grid-template-columns:1fr}}
        body.perf *,body.perf *:before,body.perf *:after{transition:none!important;animation:none!important;backdrop-filter:none!important}
        /* ---- The counter's own window: its title bar is drawn here (drag to move, double click to maximize) ---- */
        .tbar{display:none}
        body.windowed{overflow:hidden}
        body.windowed .tbar{display:flex;justify-content:flex-end;position:fixed;top:0;left:0;right:0;height:34px;z-index:20;app-region:drag;user-select:none}
        body.windowed #shell{position:absolute;top:0;left:0;right:0;bottom:0;min-height:0}
        body.windowed #side{height:100vh;padding-top:44px}
        body.windowed #scroll{overflow:auto;height:100vh;padding-top:34px}
        body.windowed .brand,body.windowed .top{app-region:drag}
        body.windowed .top button,body.windowed .top select{app-region:no-drag}
        .wctl{display:flex;app-region:no-drag}
        .wctl button{width:46px;height:34px;padding:0;border:0;border-radius:0;background:transparent;color:var(--muted);display:grid;place-items:center;cursor:pointer;transition:background .12s,color .12s}
        .wctl button:hover{background:color-mix(in srgb,var(--ink) 12%,transparent);color:var(--ink)}
        .wctl button.x:hover{background:#b3261e;color:#fff}
        .wctl svg{display:block}
        /* Scrollbars in the colors of the theme. */
        #scroll::-webkit-scrollbar,html::-webkit-scrollbar{width:12px}
        #scroll::-webkit-scrollbar-track,html::-webkit-scrollbar-track{background:transparent}
        #scroll::-webkit-scrollbar-thumb,html::-webkit-scrollbar-thumb{background:color-mix(in srgb,var(--ink) 22%,transparent);border-radius:6px;border:3px solid transparent;background-clip:padding-box}
        .log::-webkit-scrollbar{width:8px}.log::-webkit-scrollbar-thumb{background:var(--sline);border-radius:4px}
        </style></head><body><div class="tbar"><div class="wctl">
          <button type="button" id="wmin"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><path d="M0 5.5h10" stroke="currentColor" stroke-width="1.1"/></svg></button>
          <button type="button" id="wmax"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><rect id="wmaxicon" x=".55" y=".55" width="8.9" height="8.9" fill="none" stroke="currentColor" stroke-width="1.1"/><path id="wresicon" d="M2.5 2.5V.55h6.95v6.95H7.5M.55 2.5h6.95v6.95H.55z" fill="none" stroke="currentColor" stroke-width="1.1" style="display:none"/></svg></button>
          <button type="button" id="wclose" class="x"><svg width="10" height="10" viewBox="0 0 10 10" aria-hidden="true"><path d="M.5.5l9 9M9.5.5l-9 9" stroke="currentColor" stroke-width="1.2"/></svg></button>
        </div></div>
        <div id="shell"><aside id="side"></aside><div id="scroll"><main id="main"><div id="app"><p class="note">…</p></div><aside id="activityPane" class="panel hidden"><h3 class="ttl"></h3><div class="log"></div></aside><section id="reportPane" class="hidden"><iframe id="rep" title="Report" scrolling="no"></iframe></section></main></div></div>
        <dialog id="dlg" aria-labelledby="dlgT"><div class="dhead"><h2 class="ttl" id="dlgT"></h2><button type="button" class="dclose" id="dlgX">×</button></div><div id="dlgB"></div></dialog>
        <script>
        const TOKEN='__TOKEN__',IC=__ICONS__,TABS=['home','report','map','overlay','settings','help'];
        let S=null,tab=(()=>{const h=location.hash.slice(1);if(TABS.includes(h))return h;try{const t=localStorage.getItem('cc-gui-tab');return TABS.includes(t)?t:'home'}catch(e){return 'home'}})(),estimate=null,stopped=false,copied=false;
        const $=s=>document.querySelector(s);
        // In the counter's own window (?w=1): the title bar of the page, its buttons sent to the window.
        const host=window.chrome&&window.chrome.webview;let maximized=false;
        if(host&&new URLSearchParams(location.search).get('w')==='1')document.body.classList.add('windowed');
        const scroller=()=>document.body.classList.contains('windowed')?document.getElementById('scroll'):document.scrollingElement;
        function winBtn(a){if(host)host.postMessage({win:a})}
        function winTitles(){if(!S)return;const T=S.txt;$('#wmin').title=T.wMin;$('#wmin').setAttribute('aria-label',T.wMin);const m=maximized?T.wRestore:T.wMax;$('#wmax').title=m;$('#wmax').setAttribute('aria-label',m);$('#wclose').title=T.wClose;$('#wclose').setAttribute('aria-label',T.wClose)}
        if(host)host.addEventListener('message',e=>{const d=e.data;if(d&&typeof d.maximized==='boolean'){maximized=d.maximized;$('#wmaxicon').style.display=maximized?'none':'';$('#wresicon').style.display=maximized?'':'none';winTitles()}});
        const esc=s=>String(s??'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
        const fmt=(s,...a)=>a.reduce((r,v,i)=>r.replace('{'+i+'}',v),s);
        async function api(action,value){const r=await fetch('/api/action',{method:'POST',headers:{'X-Token':TOKEN,'Content-Type':'application/json'},body:JSON.stringify({action,value})});const j=await r.json();await refresh(true);return j.result}
        async function refresh(force){if(stopped)return;try{const r=await fetch('/api/state',{headers:{'X-Token':TOKEN}});S=await r.json();render(force)}catch(e){if(S){$('#app').innerHTML='<div class="card">'+esc(S.txt.offline)+'</div>';appHtml=null;$('#activityPane').classList.add('hidden')}}}
        function busy(){const a=document.activeElement;return a&&a.closest&&a.closest('#app')&&/INPUT|SELECT|TEXTAREA/.test(a.tagName)}
        function busySide(){const a=document.activeElement;return a&&a.closest&&a.closest('#side')&&a.tagName==='SELECT'}
        function sw(id,on,act){return `<label class="switch"><input type="checkbox" id="${id}" ${on?'checked':''} onchange="api('${act}',this.checked)"><span></span></label>`}
        function row(title,help,control){return `<div class="row"><div><div class="t">${title}</div>${help?`<div class="h">${help}</div>`:''}</div><div>${control}</div></div>`}
        function sel(act,value,opts){return `<select onchange="api('${act}',this.value)">${opts.map(o=>`<option value="${esc(o[0])}" ${String(o[0])===String(value)?'selected':''}>${esc(o[1])}</option>`).join('')}</select>`}
        function setTab(t){const was=tab;tab=t;try{localStorage.setItem('cc-gui-tab',t)}catch(e){}render(true);if(t==='map')scrollToMap();else if(t!==was)scroller().scrollTop=0}
        const NAV={home:'home',report:'report',map:'map',overlay:'overlay',settings:'gear',help:'help'};
        // The side bar: the app, its tabs, and what the counter is doing (with the account and, in a browser, quit).
        function side(T){
          const tabs=TABS.filter(t=>t!=='map'||!S.perf);
          const acct=S.profiles.length>1?`<div class="acct">${esc(T.profile)}<select aria-label="${esc(T.profile)}" onchange="api('profile',this.value)">${S.profiles.map(p=>`<option value="${esc(p.key)}" ${p.key===S.profileKey?'selected':''}>${esc(p.name)} (${esc(p.matches)})</option>`).join('')}</select></div>`:(S.player?`<div class="acct">${esc(T.account)}: ${esc(S.player)}</div>`:'');
          return `<div class="brand"><span class="logo">${IC.chest}</span><div><b class="ttl">${esc(T.brand)}</b><small>Fortnite · v${esc(S.version)}</small></div></div>
            ${tabs.map(t=>`<button class="navi ${t===tab?'on':''}" onclick="setTab('${t}')" title="${esc(T[t])}">${IC[NAV[t]]}<span>${esc(T[t])}</span></button>`).join('')}
            <div class="sfoot"><div class="status ${S.status.kind}" title="${esc(S.status.text)}"><i></i><span>${esc(S.status.text)}</span></div>${acct}
            ${S.settings.windowed?'':`<button class="sbtn" onclick="quit()" title="${esc(T.quit)}">${IC.power}<span>${esc(T.quit)}</span></button>`}</div>`;
        }
        function header(T,buttons){
          const day=new Date().toLocaleDateString(S.locale,{weekday:'long',day:'numeric',month:'long'});
          return `<div class="top"><div><h1 class="ttl">${esc(T[tab])}</h1><div class="sub">${esc(day.charAt(0).toUpperCase()+day.slice(1))}</div></div>${buttons?`<div class="btnrow">${buttons}</div>`:''}</div>`;
        }
        function render(force){
          const T=S.txt;document.documentElement.lang=S.lang;document.documentElement.dataset.accent=S.settings.theme||'pirate';document.body.classList.toggle('perf',!!S.perf);
          if(tab==='map'&&S.perf)tab='report';
          if(!(busySide()&&!force)){const sd=side(T);if(force||sd!==sideHtml){$('#side').innerHTML=sd;sideHtml=sd}}
          if(!force&&busy())return;
          // A click in progress (button pressed, not released yet): rebuilding now would replace its button and the click
          // would be lost. The next refresh, 2 s later, catches up.
          if(!force&&pressedAt&&Date.now()-pressedAt<3000)return;
          $('#main').classList.toggle('home',tab==='home');
          let v='';
          if(tab==='home')v=home(T);else if(tab==='report'||tab==='map')v=header(T,`<button class="b main" onclick="api('openReportFile')">${IC.report}${esc(T.openInBrowser)}</button>`);else if(tab==='overlay')v=overlay(T);else if(tab==='settings')v=settings(T);else if(tab==='help')v=help(T);
          // Rebuilt only when something changed: the buttons stay the same elements between two refreshes.
          const app=notices(T)+v;
          if(force||app!==appHtml){$('#app').innerHTML=app;appHtml=app}
          showActivity(T);showReport();winTitles();
        }
        // pressedAt: when the mouse button went down (0 once released). Forgotten after 3 s in case the release was
        // missed (outside the window), so the page never stops refreshing.
        let appHtml=null,sideHtml=null,pressedAt=0;
        document.addEventListener('pointerdown',()=>{pressedAt=Date.now()},true);
        ['pointerup','pointercancel'].forEach(t=>document.addEventListener(t,()=>{pressedAt=0},true));
        let activityHtml=null;
        // The activity as a timeline, newest first: one entry per event (its time on the right, an icon for its kind),
        // the indented lines that follow it (match counted, treasure, warning...) as its details. The console's own
        // marks (✔ ⚠ ⚑ ★) give way to the icons.
        const FEED_ICON={ok:'check-circle',warn:'warning',err:'error',info:'info',tre:'star','':'clock'};
        function feed(T){
          if(!S.activity.length)return `<div class="fempty">${IC.clock}<span>${esc(T.noActivity)}</span></div>`;
          // A line with no time right after another one with no time is the rest of the same message.
          const items=[];let plain=false;
          for(const line of S.activity){
            let text=line.map(s=>s.t).join('');
            // Its kind: the color of the first colored part (the mark at its start), not of a figure inside it.
            const cls=(line.find(s=>s.t.trim()&&['err','warn','tre','ok','info'].includes(s.c))||{}).c||'';
            const indented=/^\s{2,}/.test(text);
            text=text.trim();
            let time='';const m=text.match(/^\[([^\]]+)\]\s*/);
            if(m){time=m[1].replace(/:\d\d(?=\s|$)/,'');text=text.slice(m[0].length)}
            text=text.replace(/^[✔↻⚠⚑★✓]+\s*/,'');
            if(!text){plain=false;continue}
            if(items.length&&(indented||(plain&&!m)))items[items.length-1].sub.push({text,cls});
            else items.push({text,cls,time,sub:[]});
            if(!indented)plain=!m;
          }
          // A plain event ("Match over: analyzing…") takes the kind of what came of it (match counted, treasure).
          for(const it of items)if(!it.cls)it.cls=(it.sub.find(s=>s.cls==='ok'||s.cls==='tre')||{}).cls||'';
          return '<ul class="feed">'+items.reverse().map(it=>`<li class="${it.cls}"><span class="fi">${IC[FEED_ICON[it.cls]]}</span><div class="fb"><div class="ft">${esc(it.text)}</div>${it.sub.map(s=>`<div class="fs ${s.cls}">${IC[s.cls?FEED_ICON[s.cls]:'arrow']}<span>${esc(s.text)}</span></div>`).join('')}</div>${it.time?`<time class="num">${esc(it.time)}</time>`:'<span></span>'}</li>`).join('')+'</ul>';
        }
        function showActivity(T){
          const pane=$('#activityPane');
          pane.classList.toggle('hidden',tab!=='home');
          if(tab!=='home')return;
          pane.querySelector('h3').innerHTML=`${esc(T.activity)}<small>${esc(T.recent)}</small>`;
          // Keep the scrolling element mounted while new log entries arrive.
          const log=pane.querySelector('.log');
          const html=feed(T);
          if(activityHtml===html)return;
          const top=log.scrollTop;
          log.innerHTML=html;
          log.scrollTop=top;
          activityHtml=html;
        }
        // The report, in the window: loaded once, then again only when the counter rewrote it (new match, setting...).
        // The "Map" tab is the same page, scrolled to its map.
        let repStamp=null,repObs=null;
        function fitReport(){const f=$('#rep');try{const m=f.contentDocument.querySelector('main');f.style.height=Math.ceil(m.getBoundingClientRect().height)+'px'}catch(e){}}
        function scrollToMap(){try{const f=$('#rep'),el=f.contentDocument&&f.contentDocument.getElementById('mapa');if(el)el.scrollIntoView({block:'start'})}catch(e){}}
        function showReport(){
          const pane=$('#reportPane'),f=$('#rep'),on=tab==='report'||tab==='map';pane.classList.toggle('hidden',!on);
          if(!on||S.reportStamp===repStamp)return;
          const y=scroller().scrollTop;repStamp=S.reportStamp;
          f.onload=()=>{fitReport();if(tab==='map')scrollToMap();else scroller().scrollTop=y;try{if(repObs)repObs.disconnect();repObs=new ResizeObserver(fitReport);repObs.observe(f.contentDocument.querySelector('main'))}catch(e){}};
          f.src='/report?t='+TOKEN+'&v='+encodeURIComponent(repStamp);
        }
        // Messages of content.json (a known problem, a fix to install...): above every tab.
        function notices(T){return (S.notices||[]).map(n=>`<div class="card notice ${n.level==='warning'?'warning':''}" role="${n.level==='warning'?'alert':'status'}"><span class="ni" aria-hidden="true">${n.level==='warning'?IC.warning:IC.info}</span><div class="nb">${n.title?`<div class="nt">${esc(n.title)}</div>`:''}<div class="nx">${esc(n.text)}</div>${n.link?`<div class="actions"><button class="b ${n.level==='warning'?'main':''}" onclick="window.open('${esc(n.link)}','_blank')">${esc(T.moreInfo)}</button></div>`:''}</div>${n.level==='info'?`<button class="nclose" title="${esc(T.close)}" aria-label="${esc(T.close)}" onclick="api('dismissNotice','${esc(n.id)}')">×</button>`:''}</div>`).join('')}
        // A progress ring (0..1), with what is written in its middle.
        function ring(p,mid){const r=24,c=2*Math.PI*r;return `<div class="ring"><svg viewBox="0 0 56 56"><circle class="bg" cx="28" cy="28" r="${r}" fill="none" stroke-width="6"/><circle class="fg" cx="28" cy="28" r="${r}" fill="none" stroke-width="6" stroke-linecap="round" stroke-dasharray="${c.toFixed(1)}" stroke-dashoffset="${(c*(1-Math.min(1,Math.max(0,p)))).toFixed(1)}"/></svg><span>${mid}</span></div>`}
        function home(T){
          let h='';
          const o=S.overlay;
          h+=header(T,(o.running?`<button class="b" onclick="api('toggleOverlayVisible')">${IC.eye}${esc(o.visible?T.hideOverlay:T.showOverlay)}</button><button class="b" onclick="api('toggleOverlayLock')">${IC.lock}${esc(o.locked?T.unlock:T.lock)}</button>`:'')+`<button class="b main" onclick="setTab('report')">${IC.report}${esc(T.openReport)}</button>`);
          if(S.autoClose)h+=`<div class="card notice warning"><span class="ni">${IC.warning}</span><div class="nb"><div class="nt">${esc(S.status.text)}</div><div>${esc(T.autoCloseText)}</div><div class="actions"><button class="b main" onclick="api('keepOpen')">${esc(T.keepOpen)}</button></div></div></div>`;
          if(S.firstRun){const s=S.settings;h+=`<div class="card"><h2 class="in ttl">${esc(T.welcome)}</h2><p style="margin-top:0">${esc(T.welcomeText)}</p>
            <div class="t" style="font-weight:600;margin-top:14px">${esc(T.welcomeQuick)}</div>
            ${row(esc(T.language),'',sel('language',s.language,s.languages))}
            ${row(esc(T.theme),esc(T.themeHelp),sel('theme',s.theme,s.themes))}
            ${s.performanceRecommended?perfRow(T,s,'wpm'):''}
            ${row(esc(T.inGame),esc(T.inGameHelp),sw('wov',S.overlay.show,'showOverlay'))}
            ${s.windowed?row(esc(T.tray),esc(T.trayHelp),sw('wtr',s.minimizeToTray,'minimizeToTray')):''}
            <div class="actions" style="margin-top:12px"><button class="b main" onclick="api('dismissWelcome')">${esc(T.welcomeDone)}</button></div></div>`}
          if(S.progress)h+=`<div class="card"><b>${esc(T.analyzing)} ${S.progress.done} / ${S.progress.total}</b><div class="bar"><i style="width:${Math.round(100*S.progress.done/Math.max(1,S.progress.total))}%"></i></div></div>`;
          if(S.whatsNew&&S.whatsNew.length)h+=`<div class="card"><h2 class="in ttl">${esc(fmt(T.whatsNew,S.version))}</h2><ul>${S.whatsNew.map(n=>`<li>${esc(n)}</li>`).join('')}</ul><button class="b main" onclick="api('dismissWhatsNew')">${esc(T.ok)}</button></div>`;
          if(S.update)h+=`<div class="card"><h2 class="in ttl">${esc(fmt(T.newVersion,S.update.version))}</h2><ul>${S.update.notes.map(n=>`<li>${esc(n)}</li>`).join('')}</ul><button class="b main" onclick="api('installUpdate')">${esc(T.install)}</button></div>`;
          // The total, with the chests of the last 14 days (today stands out).
          const sp=S.spark||[],max=Math.max(1,...sp);
          h+=`<section class="hero"><button type="button" class="editbtn" onclick="openManual()" title="${esc(T.editChests)}" aria-label="${esc(T.editChests)}">__PENCIL__</button>
            <div><div class="k">${IC.chest}${esc(T.total)}</div><div class="v num">${esc(S.total)}</div><div class="d"><b>+${esc(S.todayChests)} ${esc(T.today)}</b> · ${esc(T.totalSub)}${S.manual?' · '+esc(fmt(T.byHand,S.manualText.replace('+',''))):''}</div></div>
            <div><div class="spark" role="img" aria-label="${esc(T.last14)}">${sp.map((d,i)=>`<i class="${i===sp.length-1?'hi':''}" style="height:${Math.max(3,Math.round(d/max*100))}%" title="${d}"></i>`).join('')}</div><div class="spark-l"><span>${esc(S.sparkFrom)}</span><span>${esc(T.last14)}</span><span>${esc(T.today)}</span></div></div></section>`;
          const kpi=(icon,label,value,sub)=>`<div class="kpi"><div class="k">${IC[icon]}<span>${label}</span></div><div class="v num">${value}</div><div class="s">${sub||'&nbsp;'}</div></div>`;
          h+=`<section class="kpis">${kpi('session',esc(T.session),esc(S.session),esc(S.sessionMatches)+' '+esc(T.matches))}
            ${kpi('last',esc(T.lastShort),esc(S.last?S.last.chests:'—'),S.last?esc(S.last.mode)+' · '+esc(S.last.place):'')}
            ${kpi('avg',esc(T.avgShort),esc(S.average),esc(fmt(T.inMatches,S.matches)))}
            ${kpi('star',esc(T.treasures),esc(S.treasures),S.treasureEvery?esc(fmt(T.every,S.treasureEvery)):'')}
            ${S.squad?kpi('team',esc(T.squad)+`<span class="badge" title="${esc(T.inaccurateWhy)}">${esc(T.inaccurate)}</span>`,esc(S.squad.total),esc(fmt(T.squadSub,S.squad.team))):''}</section>`;
          const goals=(S.goals||[]).map(g=>`<div class="goal${g.done?' done':''}">${ring(g.pct/100,g.done?IC.check:g.pct+'%')}<div><div class="t">${esc(g.label)}</div><div class="v num">${esc(g.text.replace(' ✓','').replace('/',' / '))}</div><div class="s">${g.done?esc(T.goalDone):esc(fmt(T.goalLeft,g.left))}</div></div></div>`);
          if(S.achievements)goals.push(`<div class="goal">${ring(S.achPct,esc(S.achievements.split(' ')[0]))}<div><div class="t">${esc(T.achievements)}</div><div class="v num">${esc(S.achievements)}</div><div class="ach">${(S.achIcons||[]).map(i=>`<i>${IC[i]||''}</i>`).join('')}</div></div></div>`);
          if(goals.length)h+=`<section class="goals">${goals.join('')}</section>`;
          if(!S.update&&S.updatesOn)h+=`<div><button class="b" onclick="api('checkUpdate')">${IC.refresh}${esc(T.checkUpdate)}</button></div>`;
          return h;
        }
        function overlay(T){
          const o=S.overlay;
          let h=header(T,'')+`<div class="card">${row(esc(T.inGame),esc(T.inGameHelp),sw('ov',o.show,'showOverlay'))}
            ${row(esc(T.style),'',sel('overlayStyle',o.style,[['hud',T.hud],['classic',T.classic]]))}
            ${o.show?row(esc(T.size),'',sel('overlaySize',o.size,o.sizes.map(x=>[x,x+' %'])))+row(esc(T.opacity),'',sel('overlayOpacity',o.opacity,o.opacities.map(x=>[x,x+' %']))):''}
            ${row(esc(T.squadToggle)+`<span class="badge" title="${esc(T.inaccurateWhy)}">${esc(T.inaccurate)}</span>`,esc(T.squadHelp)+'<br>'+esc(T.inaccurateWhy),sw('sq',o.squad,'showSquadTotal'))}</div>
            <h2 class="sec ttl">${esc(T.obs)}</h2><div class="card">${S.perf?`<p class="note" style="margin:0">${esc(T.obsPerf)}</p>`:`<span class="path">${esc(S.paths.overlay)}</span><p class="note">${esc(T.obsSteps)}</p>
            <div class="actions"><button class="b main" onclick="copyObs()">${esc(copied?T.copied:T.copy)}</button><button class="b" onclick="window.open('/overlay?t='+TOKEN,'_blank')">${esc(T.preview)}</button></div>`}</div>`;
          return h;
        }
        function settings(T){
          const s=S.settings;
          let h=header(T,'')+`<div class="card">${row(esc(T.theme),esc(T.themeHelp),sel('theme',s.theme,s.themes))}
            ${perfRow(T,s,'pm')}${row(esc(T.language),'',sel('language',s.language,s.languages))}
            ${row(esc(T.times),esc(T.timesHelp),sel('timeZone',s.timeZone,[['local',fmt(T.timesLocal,s.localZone)],['utc',T.timesUtc]]))}
            ${row(esc(T.clock),'',sel('clockFormat',s.clockFormat,[['auto',fmt(T.clockAuto,s.windowsClock==='24'?T.h24:T.h12)],['12',T.clock12],['24',T.clock24]]))}
            ${S.profiles.length>1?row(esc(T.profile),'',sel('profile',S.profileKey,S.profiles.map(p=>[p.key,p.name+' ('+p.matches+' · '+p.chests+')']))):''}
            ${s.windowed?row(esc(T.tray),esc(T.trayHelp),sw('tr',s.minimizeToTray,'minimizeToTray')):''}
            ${s.canStartWithFortnite?row(esc(T.withFortnite),esc(T.withFortniteHelp),sw('wf',s.startWithFortnite,'startWithFortnite')):''}
            ${row(esc(T.updates),'',sel('updateMode',s.updateMode,[['prompt',T.upPrompt],['auto',T.upAuto],['off',T.upOff]]))}
            <div class="row"><div style="flex:1"><div class="t">${esc(T.demos)}</div>${s.demosExists?'':`<div class="h">${esc(T.demosMissing)}</div>`}<div class="actions" style="margin-top:6px"><input type="text" id="demos" value="${esc(s.demos)}"><button class="b" onclick="api('demosFolder',$('#demos').value)">${esc(T.save)}</button></div></div></div></div>`;
          h+=`<h2 class="sec ttl">${esc(T.goals)}</h2><div class="card"><p class="note" style="margin-top:0">${esc(T.goalsHelp)}</p>
            ${row(esc(T.goalDaily),'',`<div class="actions"><input type="number" id="gday" min="0" step="1" value="${s.dailyGoal||''}" placeholder="0" style="width:110px"><button class="b" onclick="api('dailyGoal',$('#gday').value||'0')">${esc(T.save)}</button></div>`)}
            ${row(esc(T.goalWeekly),'',`<div class="actions"><input type="number" id="gweek" min="0" step="1" value="${s.weeklyGoal||''}" placeholder="0" style="width:110px"><button class="b" onclick="api('weeklyGoal',$('#gweek').value||'0')">${esc(T.save)}</button></div>`)}</div>
            <h2 class="sec ttl">Discord</h2><div class="card"><p class="note" style="margin-top:0">${esc(T.discordHelp)}</p>
            <div class="row"><div style="flex:1"><div class="t">${esc(T.discordUrl)}</div><div class="actions" style="margin-top:6px"><input type="text" id="dhook" value="${esc(s.discordWebhook)}" placeholder="https://discord.com/api/webhooks/…"><button class="b" onclick="saveDiscord()">${esc(T.save)}</button><button class="b" onclick="testDiscord()">${esc(T.discordTest)}</button></div>${discordMsg?`<div class="h" style="margin-top:6px">${esc(discordMsg)}</div>`:''}</div></div>
            ${row(esc(T.discordMode),'',sel('discordMode',s.discordMode,[['match',T.discordAll],['treasure',T.discordNotable]]))}</div>`;
          return h;
        }
        function perfRow(T,s,id){return row(esc(T.perf)+(s.performanceRecommended&&!s.performanceMode?`<span class="perfbadge">${esc(T.perfRecommended)}</span>`:''),esc(T.perfHelp),sw(id,s.performanceMode,'performanceMode'))}
        let discordMsg='';
        async function saveDiscord(){const r=await api('discordWebhook',$('#dhook').value);discordMsg=r&&r.error?r.error:S.txt.saved;render(true)}
        async function testDiscord(){discordMsg='…';render(true);const r=await api('discordTest',$('#dhook').value);discordMsg=r&&r.error?r.error:S.txt.discordSent;render(true)}
        // Chests added by hand (and the estimate from the Career matches): p = 's' in the settings, 'd' in the edit box.
        // The box starts empty each time: the number typed is added to what was already added, and each addition can be
        // undone with its cross (issue #27).
        function manualForm(T,p){
          const hist=S.manualHistory||[];
          return `<p class="note" style="margin-top:0">${esc(T.manualHelp)}</p>
            <div class="mtotal">${esc(fmt(T.manualTotal,S.manualText))}</div>
            <div class="actions"><input type="number" id="${p}manual" step="1" placeholder="0" aria-label="${esc(T.manual)}" onkeydown="if(event.key==='Enter')addManual('${p}')"><button class="b main" onclick="addManual('${p}')">${esc(T.addBtn)}</button></div>
            ${S.manualWeek?`<label class="mweek"><input type="checkbox" id="${p}week"><b>${esc(T.thisWeek)}</b><span>${esc(fmt(T.thisWeekHelp,S.manualWeek))}</span></label>`:''}
            ${hist.length?`<ul class="mhist">${hist.map(e=>`<li><b class="num">${esc(e.chests)}</b><span>${esc([e.what,e.when].filter(Boolean).join(' · '))}</span><button type="button" class="mx" title="${esc(T.removeEntry)}" aria-label="${esc(T.removeEntry)}" onclick="removeManual('${p}','${esc(e.id)}')">×</button></li>`).join('')}</ul>`:''}
            <div class="t" style="margin-top:16px;font-weight:600">${esc(T.estimate)}</div><p class="note">${esc(T.estimateHelp)}</p>
            <div class="actions"><input type="number" id="${p}career" min="0" placeholder="${esc(T.career)}" aria-label="${esc(T.career)}"><button class="b" onclick="doEstimate('${p}')">${esc(T.compute)}</button></div>
            <div class="estimate" id="${p}est">${estimate&&estimate.p===p?estimateHtml(T,p):''}</div>`;
        }
        function estimateHtml(T,p){if(estimate.error)return esc(estimate.error);return esc(estimate.text)+`<div class="actions" style="margin-top:8px"><button class="b main" onclick="addEstimate('${p}')">${esc(fmt(T.add,estimate.chestsText))}</button></div>`}
        async function doEstimate(p){const r=await api('estimate',parseInt($('#'+p+'career').value||'0',10));estimate=Object.assign({p},r);const e=$('#'+p+'est');if(e)e.innerHTML=estimateHtml(S.txt,p)}
        async function addManual(p){const i=$('#'+p+'manual'),w=$('#'+p+'week'),n=parseInt(i&&i.value||'0',10);if(!n){if(i)i.focus();return}await api('manualAdd',{chests:n,thisWeek:!!(w&&w.checked)});redrawManual(p)}
        async function addEstimate(p){if(!estimate||estimate.error)return;await api('manualEstimate',{chests:estimate.chests,career:estimate.career});redrawManual(p)}
        async function removeManual(p,id){await api('manualRemove',id);redrawManual(p)}
        // After a change the box stays open, with the new total and history, ready for the next number.
        function redrawManual(p){estimate=null;if(p!=='d'){render(true);return}$('#dlgB').innerHTML=manualForm(S.txt,'d');const i=$('#dmanual');if(i)i.focus()}
        // The edit box of "chests in total" (home tab, and the report's pencil through a message).
        function openManual(){const T=S.txt,d=$('#dlg');estimate=null;$('#dlgT').textContent=T.editChests;$('#dlgB').innerHTML=manualForm(T,'d');$('#dlgX').setAttribute('aria-label',T.close);if(!d.open)d.showModal();const i=$('#dmanual');if(i){i.focus();i.select()}}
        $('#dlgX').onclick=()=>$('#dlg').close();
        $('#wmin').onclick=()=>winBtn('min');$('#wmax').onclick=()=>winBtn('max');$('#wclose').onclick=()=>winBtn('close');
        // A click on the dimmed backdrop closes the box, only when it starts there too: selecting the number with the mouse
        // and releasing the button outside the box must not close it.
        let dlgDownOut=false;
        function dlgOutside(e){const r=$('#dlg').getBoundingClientRect();return e.target.id==='dlg'&&(e.clientX<r.left||e.clientX>r.right||e.clientY<r.top||e.clientY>r.bottom)}
        $('#dlg').addEventListener('pointerdown',e=>{dlgDownOut=dlgOutside(e)});
        $('#dlg').addEventListener('click',e=>{if(dlgDownOut&&dlgOutside(e))$('#dlg').close();dlgDownOut=false});
        window.addEventListener('message',e=>{if(e.origin===location.origin&&e.data&&e.data.cc==='editChests'&&S)openManual()});
        async function copyObs(){await api('copyObsPath');copied=true;render(true);setTimeout(()=>{copied=false;render(true)},2000)}
        function help(T){
          return header(T,'')+`<div class="card"><p style="margin-top:0">${esc(T.helpText)}</p><div class="actions"><button class="b" onclick="api('openDataFolder')">${esc(T.dataFolder)}</button>${S.paths.errorLog?`<button class="b" onclick="api('openErrorLog')">${esc(T.errorLog)}</button>`:''}<button class="b" onclick="api('openIssues')">${esc(T.issue)}</button><button class="b" onclick="api('openRepo')">${esc(T.project)}</button></div></div>
          ${S.canUninstall?`<h2 class="sec ttl">${esc(T.uninstall)}</h2><div class="card"><span class="path">${esc(S.paths.data)}</span>
          <div class="actions"><button class="b danger" onclick="uninstall(false)">${esc(T.uninstall)} · ${esc(T.keepData)}</button><button class="b danger" onclick="uninstall(true)">${esc(T.uninstall)} · ${esc(T.deleteData)}</button></div></div>`:''}
          <p class="note" style="color:var(--muted)">${esc(S.settings.windowed?T.footerWindow:T.footer)}</p>`;
        }
        function closedView(){stopped=true;$('#app').innerHTML='<div class="card">'+esc(S.txt.closed)+'</div>';appHtml=null;$('#activityPane').classList.add('hidden');$('#reportPane').classList.add('hidden')}
        async function uninstall(all){if(!confirm(S.txt.uninstallConfirm))return;await api('uninstall',all);closedView()}
        async function quit(){if(!confirm(S.txt.quitConfirm))return;try{await api('quit')}catch(e){}closedView()}
        // Every 2 s (5 s in performance mode), never two at once.
        refresh(true);(function loop(){setTimeout(()=>refresh(false).finally(loop),S&&S.perf?5000:2000)})();
        </script></body></html>
        """;
}
