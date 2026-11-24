using System.Net;
using System.Text;

namespace ChestCounter;

/// <summary>
/// The report sections added by this fork: your stats (pace, streak, weeks, form, goals), achievements, chests by mode
/// and by placement, rhythm within a match, chests against the result, squad ranking and loot. Built from the counted
/// matches (the chests added by hand have no match, no date and no duration; they only count in the achievements'
/// totals).
/// </summary>
public static class ReportStats
{
    public const string Css = """
        .tiles.stats{grid-template-columns:repeat(4,minmax(0,1fr))}
        @media (max-width:820px){.tiles.stats{grid-template-columns:repeat(2,minmax(0,1fr))}}
        .trend{font-weight:700}.trend.up{color:#2f7d4f}.trend.down{color:var(--warn)}
        td.sub{color:var(--cmuted);font-size:13px;white-space:nowrap;text-align:right}
        th.r{text-align:right}
        .ic{display:inline-block;flex:none}.ici{width:1.05em;height:1.05em;vertical-align:-.18em}
        .tile .storm{color:#7b4fc9}.tile .storm .ici{width:.8em;height:.8em;vertical-align:-.05em}
        :root[data-accent="noite"] .pirate{font-family:"Segoe UI",system-ui,sans-serif;font-weight:700;font-size:.85em}
        :root[data-accent="arena"] .pirate{font-family:Bahnschrift,"Segoe UI",sans-serif;font-weight:700;font-style:italic;text-transform:uppercase;font-size:.85em}
        #mapa{scroll-margin-top:16px}
        html.incounter #mapa{display:none} /* the window has its own Map tab */
        html.incounter main>header{display:none} /* the window shows its own title */
        html.incounter main{padding-top:4px}
        .goals{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:12px;margin-top:12px}
        @media (max-width:820px){.goals{grid-template-columns:1fr}}
        .goal{background:var(--card);color:var(--cink);border:1px solid var(--line);border-radius:8px;padding:12px 16px}
        .goal .gh{display:flex;justify-content:space-between;font-weight:700}
        .goal.done{border-color:var(--team);box-shadow:inset 6px 0 0 var(--team);padding-left:22px}
        .gbar{display:block;height:8px;background:var(--track);border-radius:4px;overflow:hidden;margin-top:8px}
        .gbar i{display:block;height:8px;background:var(--gold)}.goal.done .gbar i{background:var(--team)}
        .achs{display:grid;grid-template-columns:repeat(auto-fill,minmax(230px,1fr));gap:10px}
        .ach{display:flex;gap:12px;align-items:flex-start;background:var(--card);color:var(--cink);border:1px solid var(--line);border-radius:8px;padding:12px 14px}
        .ach .ai{flex:none;width:38px;height:38px;border-radius:50%;display:grid;place-items:center;background:var(--track);color:var(--cmuted)}.ach .ai .ic{width:22px;height:22px}
        .ach.on .ai{background:var(--hero,#7a4a1e);color:var(--heroink,#ffd36a)}
        .goal .gl{display:flex;align-items:center;gap:8px}.goal .gl .ic{width:18px;height:18px;color:var(--gold)}
        .rank{display:inline-grid;place-items:center;width:26px;font-weight:700;color:var(--cmuted)}.rank .ic{width:24px;height:24px}.rank.r1{color:#c9a227}.rank.r2{color:#8f9aa6}.rank.r3{color:#b0703a}
        .ach .an{font-weight:700}.ach .ad{font-size:13px;color:var(--cmuted)}
        .ach.locked{opacity:.7}
        .ach.on{border-color:var(--gold);box-shadow:inset 0 0 0 1px var(--gold)}
        .ach .gbar{height:5px;margin-top:6px}.ach .ap{font-size:12px;color:var(--cmuted);margin-top:2px}
        .phases{display:flex;height:26px;border-radius:6px;overflow:hidden;margin:8px 0 6px}
        .phases i{display:flex;align-items:center;justify-content:center;font:700 12px "Segoe UI",system-ui,sans-serif;color:#fff;font-style:normal;min-width:2px}
        .plegend{display:flex;justify-content:space-between;font-size:13px;color:var(--cmuted)}
        .shead{display:flex;align-items:baseline;gap:10px;flex-wrap:wrap}.shead .muted{font-size:14px}
        .mapgrid{display:grid;grid-template-columns:minmax(0,3fr) minmax(0,2fr);gap:16px;align-items:start}
        @media (max-width:820px){.mapgrid{grid-template-columns:1fr}}
        .mapcard{padding:10px}
        .chestmap{display:block;width:100%;height:auto;aspect-ratio:1;border-radius:6px;background:#0d3a5c}
        .chestmap .pt{stroke:#0b0f18;stroke-width:1;opacity:.9}
        .chestmap text{fill:#fff;stroke:#0b0f18;stroke-width:3;paint-order:stroke;font-family:"Segoe UI",system-ui,sans-serif;font-weight:700;text-anchor:middle;pointer-events:none}
        #mapbox[data-f="week"] .pt:not(.w),#mapbox[data-f="last"] .pt:not(.l){display:none}
        .mapf{display:flex;gap:14px;flex-wrap:wrap;margin:2px 4px 8px;font-size:14px}.mapf label{display:flex;gap:5px;align-items:center;cursor:pointer}
        .maplegend{display:flex;gap:14px;flex-wrap:wrap;margin:8px 4px 2px;font-size:13px;color:var(--cmuted)}
        """;

    private static string H(string? s) => WebUtility.HtmlEncode(s ?? "");
    private static string F1(double v) => v.ToString("0.0", L.Culture);
    private static string MinSec(double seconds) => $"{(int)seconds / 60}:{(int)seconds % 60:00}";

    private static string Tile(string value, string label, string? sub) =>
        $"""<div class="tile"><div class="v num">{value}</div><div class="l">{label}</div>{(sub != null ? $"<div class=\"s\">{sub}</div>" : "")}</div>""";

    private static string Bar(int percent, string? color = null) =>
        $"<span class=\"meter\"><i style=\"width:{Math.Clamp(percent, 2, 100)}%;background:{color ?? "var(--gold)"}\"></i></span>";

    private static string Title(string text, string? note = null) =>
        $"<div class=\"shead\"><h2 class=\"pirate\">{text}</h2>{(note != null ? $"<span class=\"muted\">{note}</span>" : "")}</div>";

    /// <summary>"▲ +12%" / "▼ −8%" (nothing when there is nothing to compare with).</summary>
    private static string Trend(double now, double before)
    {
        if (before <= 0)
        {
            return "";
        }
        var pct = (int)Math.Round(100 * (now - before) / before);
        if (pct == 0)
        {
            return "<span class=\"trend\">=</span>";
        }
        return pct > 0 ? $"<span class=\"trend up\">▲ +{pct}%</span>" : $"<span class=\"trend down\">▼ −{-pct}%</span>";
    }

    public static string Section(IReadOnlyList<MatchRecord> matches, int manual)
    {
        if (matches.Count == 0)
        {
            return "";
        }
        var sb = new StringBuilder();
        sb.Append(YourStats(matches));
        sb.Append(MapSection(matches));
        sb.Append(AchievementsSection(matches, manual));
        sb.Append("<div class=\"two\">").Append(ByMode(matches)).Append(ByPlacement(matches)).Append("</div>");
        sb.Append(Rhythm(matches));
        sb.Append(ChestsAndResult(matches));
        sb.Append(SquadRanking(matches));
        sb.Append(LootSection(matches));
        return sb.ToString();
    }

    // ---- Your stats --------------------------------------------------------------------------------------------

    private static string YourStats(IReadOnlyList<MatchRecord> matches)
    {
        var average = matches.Average(m => m.ChestsOpened);

        // Pace: chests per hour spent in matches (the replay's length: from the bus to the end of the player's match).
        var timed = matches.Where(m => m.DurationSeconds is > 60).ToList();
        var hours = timed.Sum(m => (double)m.DurationSeconds!.Value) / 3600;
        var pace = hours > 0.05
            ? Tile(F1(timed.Sum(m => m.ChestsOpened) / hours), L.T("coffres par heure de jeu", "chests per hour played", "cofres por hora de juego", "baús por hora de jogo"),
                L.T($"sur {F1(hours)} h de parties", $"over {F1(hours)} h of matches", $"en {F1(hours)} h de partidas", $"em {F1(hours)} h de partidas"))
            : "";

        var (current, best) = ProgressStats.Streaks(matches);
        var streak = Tile(L.Count(current, "jour", "day", "día", "dia"), L.T("série en cours", "current streak", "racha actual", "sequência atual"),
            L.T("record : ", "best: ", "récord: ", "recorde: ") + L.Count(best, "jour", "day", "día", "dia"));

        // This week (from Monday, so far) against the whole of last week.
        var weekStart = Goals.WeekStart(Goals.Today);
        int ChestsOfWeek(DateTime from) => matches.Where(m => Goals.When(m).Date >= from && Goals.When(m).Date < from.AddDays(7)).Sum(m => m.ChestsOpened);
        var thisWeek = ChestsOfWeek(weekStart);
        var lastWeek = ChestsOfWeek(weekStart.AddDays(-7));
        var week = Tile(L.N(thisWeek), L.T("coffres cette semaine", "chests this week", "cofres esta semana", "baús nesta semana"),
            L.T($"semaine dernière : {L.N(lastWeek)}", $"last week: {L.N(lastWeek)}", $"semana pasada: {L.N(lastWeek)}", $"semana passada: {L.N(lastWeek)}")
            + " " + Trend(thisWeek, lastWeek));

        // Form: the last 10 matches against the overall average.
        var recentAverage = matches.OrderByDescending(Goals.When).Take(10).Average(m => m.ChestsOpened);
        var form = matches.Count >= 15
            ? Tile(F1(recentAverage), L.T("moyenne des 10 dernières parties", "average of the last 10 matches", "media de las 10 últimas partidas", "média das últimas 10 partidas"),
                L.T($"moyenne générale : {F1(average)}", $"overall average: {F1(average)}", $"media general: {F1(average)}", $"média geral: {F1(average)}")
                + " " + Trend(recentAverage, average))
            : "";

        var goals = Goals.Of(matches);
        var goalsHtml = goals.Count == 0 ? "" : "<div class=\"goals\">" + string.Concat(goals.Select(g =>
            $"<div class=\"goal{(g.Done ? " done" : "")}\"><div class=\"gh\"><span class=\"gl\">{Icons.Svg("target")}{H(g.Label)}</span><span class=\"num\">{H(g.Text)}</span></div><span class=\"gbar\"><i style=\"width:{g.Percent}%\"></i></span></div>")) + "</div>";

        return $"<section>{Title(L.T("Tes statistiques", "Your stats", "Tus estadísticas", "Suas estatísticas"))}<section class=\"tiles stats\">{pace}{streak}{week}{form}</section>{goalsHtml}</section>";
    }

    // ---- Map of the chests -------------------------------------------------------------------------------------

    private static string MapSection(IReadOnlyList<MatchRecord> matches)
    {
        var title = L.T("Où tu ouvres tes coffres", "Where you open your chests", "Dónde abres tus cofres", "Onde você abre seus baús");
        var points = matches.SelectMany(m => m.Chests.Where(c => c.X != null && c.Y != null).Select(c => (m, c))).ToList();
        if (points.Count == 0)
        {
            return $"<section id=\"mapa\">{Title(title)}<div class=\"card\"><p class=\"items\">{L.T(
                "La carte se remplit avec les parties analysées à partir de la version 1.8 (les replays encore sur ton PC sont réanalysés).",
                "The map fills up with the matches analyzed from version 1.8 on (the replays still on your PC are analyzed again).",
                "El mapa se llena con las partidas analizadas desde la versión 1.8 (las repeticiones que siguen en tu PC se analizan de nuevo).",
                "O mapa é preenchido com as partidas analisadas a partir da versão 1.8 (os replays que ainda estão no seu PC são analisados de novo).")}</p></div></section>";
        }

        var pois = MapData.Pois(L.Code);
        var weekStart = Goals.WeekStart(Goals.Today);
        var last = matches.OrderByDescending(Goals.When).First();

        // Crop on the island: the named places and the chests, with a margin, as a square (picture units 0..1000).
        var mapPoints = MapView.Points(matches);
        var (cropU, cropV, size) = MapView.Crop(mapPoints, pois);
        string N1(double v) => v.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture);
        var box = $"{N1(cropU * 1000)} {N1(cropV * 1000)} {N1(size * 1000)} {N1(size * 1000)}";
        var r = size * 1000 * 0.0065;
        var font = size * 1000 * 0.021;

        var svg = new StringBuilder($"<svg viewBox=\"{box}\" class=\"chestmap\" role=\"img\" aria-label=\"{H(title)}\">");
        svg.Append(MapData.HasImage
            ? $"<image href=\"images/{MapData.ImageName}\" x=\"0\" y=\"0\" width=\"1000\" height=\"1000\"/>"
            : "<defs><pattern id=\"mgrid\" width=\"50\" height=\"50\" patternUnits=\"userSpaceOnUse\"><path d=\"M50 0H0V50\" fill=\"none\" stroke=\"#2c4a5e\" stroke-width=\"1\"/></pattern></defs><rect x=\"0\" y=\"0\" width=\"1000\" height=\"1000\" fill=\"#123247\"/><rect x=\"0\" y=\"0\" width=\"1000\" height=\"1000\" fill=\"url(#mgrid)\"/>");
        foreach (var (m, c) in points)
        {
            var (u, v) = MapData.Project(c.X!.Value, c.Y!.Value);
            var cls = "pt" + (Goals.When(m).Date >= weekStart ? " w" : "") + (m.SessionId == last.SessionId ? " l" : "");
            var color = ReportWriter.KindColor.GetValueOrDefault(ChestKinds.Canonical(c.Kind), "#9a8f7a");
            svg.Append($"<circle class=\"{cls}\" cx=\"{N1(u * 1000)}\" cy=\"{N1(v * 1000)}\" r=\"{N1(r)}\" fill=\"{color}\"/>");
        }
        foreach (var p in pois)
        {
            var (u, v) = MapData.Project(p.X, p.Y);
            svg.Append($"<text x=\"{N1(u * 1000)}\" y=\"{N1(v * 1000)}\" font-size=\"{N1(font)}\">{H(p.Name)}</text>");
        }
        svg.Append("</svg>");

        // Legend: the kinds of chest seen on the map.
        var legend = string.Concat(points.GroupBy(p => ChestKinds.Canonical(p.c.Kind)).OrderByDescending(g => g.Count()).Select(g =>
            $"<span><span class=\"sw\" style=\"background:{ReportWriter.KindColor.GetValueOrDefault(g.Key, "#9a8f7a")}\"></span>{H(L.Name(g.Key))} · {L.N(g.Count())}</span>"));

        // Where: the named place closest to each chest (within 300 m).
        var places = MapView.Places(mapPoints, pois);
        var top = Math.Max(1, places.Count > 0 ? places.Max(x => x.Count) : 1);
        var placesHtml = new StringBuilder($"<div class=\"card\"><b>{L.T("Là où tu en ouvres le plus", "Where you open the most", "Donde más abres", "Onde você mais abre")}</b><table>");
        foreach (var p in places)
        {
            placesHtml.Append($"<tr><td>{H(p.Name)}</td><td style=\"width:110px\">{Bar((int)Math.Round(100.0 * p.Count / top))}</td><td class=\"n num\">{L.N(p.Count)}</td><td class=\"sub num\">{L.Count(p.Matches, "partie", "parties", "match", "matches", "partida", "partidas", "partida", "partidas")}</td></tr>");
        }
        placesHtml.Append("</table></div>");

        var filters = string.Join("", new[] { ("all", L.T("Tout", "All", "Todo", "Tudo")), ("week", L.T("Cette semaine", "This week", "Esta semana", "Esta semana")), ("last", L.T("Dernière partie", "Last match", "Última partida", "Última partida")) }
            .Select(f => $"<label><input type=\"radio\" name=\"mapf\" value=\"{f.Item1}\"{(f.Item1 == "all" ? " checked" : "")} onchange=\"document.getElementById('mapbox').dataset.f=this.value;try{{localStorage.setItem('cc-mapf',this.value)}}catch(e){{}}\">{f.Item2}</label>"));
        // The page reloads itself: the filter chosen is kept.
        const string keepFilter = "<script>try{var f=localStorage.getItem('cc-mapf'),b=document.getElementById('mapbox'),i=f&&b.querySelector('input[value=\"'+f+'\"]');if(i){i.checked=true;b.dataset.f=f}}catch(e){}</script>";

        return $"<section id=\"mapa\">{Title(title, L.T($"{L.N(points.Count)} coffres placés", $"{L.N(points.Count)} chests placed", $"{L.N(points.Count)} cofres situados", $"{L.N(points.Count)} baús no mapa"))}"
            + $"<div class=\"mapgrid\"><div><div class=\"card mapcard\" id=\"mapbox\" data-f=\"all\"><div class=\"mapf\">{filters}</div>{svg}<div class=\"maplegend\">{legend}</div></div>"
            + (MapData.HasImage ? $"<div class=\"items\" style=\"margin-top:6px\">{L.T("Carte : fortnite-api.com", "Map: fortnite-api.com", "Mapa: fortnite-api.com", "Mapa: fortnite-api.com")}</div>" : "")
            + $"</div>{placesHtml}</div>{keepFilter}</section>";
    }

    // ---- Achievements ------------------------------------------------------------------------------------------

    private static string AchievementsSection(IReadOnlyList<MatchRecord> matches, int manual)
    {
        var stats = ProgressStats.From(matches, manual);
        var unlocked = Achievements.All.Count(a => a.Unlocked(stats));
        var sb = new StringBuilder($"<section>{Title(L.T("Succès", "Achievements", "Logros", "Conquistas"), $"{unlocked} / {Achievements.All.Length}")}<div class=\"achs\">");
        // Unlocked first, then the closest to being unlocked.
        foreach (var a in Achievements.All.OrderByDescending(a => a.Unlocked(stats)).ThenByDescending(a => (double)Math.Min(a.Value(stats), a.Target) / a.Target))
        {
            var on = a.Unlocked(stats);
            var value = Math.Min(a.Value(stats), a.Target);
            var progress = on ? "" : $"<span class=\"gbar\"><i style=\"width:{(int)Math.Round(100.0 * value / a.Target)}%\"></i></span><div class=\"ap num\">{L.N(value)} / {L.N(a.Target)}</div>";
            sb.Append($"<div class=\"ach{(on ? " on" : " locked")}\" title=\"{H(a.Description)}\"><span class=\"ai\">{Icons.Svg(a.Icon)}</span><div style=\"flex:1;min-width:0\"><div class=\"an\">{H(a.Name)}</div><div class=\"ad\">{H(a.Description)}</div>{progress}</div></div>");
        }
        return sb.Append("</div></section>").ToString();
    }

    // ---- By mode / by placement -------------------------------------------------------------------------------

    private static string Table(string title, string firstColumn, IEnumerable<(string Label, List<MatchRecord> Matches)> groups)
    {
        var rows = groups.Where(g => g.Matches.Count > 0).ToList();
        var top = Math.Max(0.1, rows.Count > 0 ? rows.Max(g => g.Matches.Average(m => m.ChestsOpened)) : 0.1);
        var t = new StringBuilder($"<section>{Title(title)}<div class=\"card\"><table><tr><th>{firstColumn}</th><th></th>"
            + $"<th class=\"r\">{L.T("Moyenne", "Average", "Media", "Média")}</th><th class=\"r\">{L.T("Parties", "Matches", "Partidas", "Partidas")}</th></tr>");
        foreach (var (label, list) in rows)
        {
            var avg = list.Average(m => m.ChestsOpened);
            t.Append($"<tr><td>{H(label)}</td><td style=\"width:150px\">{Bar((int)Math.Round(100 * avg / top))}</td>"
                + $"<td class=\"n num big\">{F1(avg)}</td><td class=\"sub num\">{L.N(list.Count)}</td></tr>");
        }
        return t.Append("</table></div></section>").ToString();
    }

    private static string ByMode(IReadOnlyList<MatchRecord> matches) =>
        Table(L.T("Coffres par mode", "Chests by mode", "Cofres por modo", "Baús por modo"), L.T("Mode", "Mode", "Modo", "Modo"),
            matches.GroupBy(m => Pretty.Playlist(m.Playlist)).OrderByDescending(g => g.Count()).Select(g => (g.Key, g.ToList())));

    private static string WinLabel => L.T("Top 1", "Victory Royale", "Victoria magistral", "Vitória Royale");

    private static string ByPlacement(IReadOnlyList<MatchRecord> matches)
    {
        var placed = matches.Where(m => m.Placement is > 0).ToList();
        return Table(L.T("Coffres par classement", "Chests by placement", "Cofres por puesto", "Baús por colocação"), L.T("Classement", "Placement", "Puesto", "Colocação"),
            new (string, List<MatchRecord>)[]
            {
                (WinLabel, placed.Where(m => m.Placement == 1).ToList()),
                ("Top 2–10", placed.Where(m => m.Placement is >= 2 and <= 10).ToList()),
                ("Top 11–25", placed.Where(m => m.Placement is >= 11 and <= 25).ToList()),
                (L.T("26e et au-delà", "26th and beyond", "26.º o peor", "26º ou pior"), placed.Where(m => m.Placement >= 26).ToList()),
            });
    }

    // ---- Rhythm within a match ---------------------------------------------------------------------------------

    private static string Rhythm(IReadOnlyList<MatchRecord> matches)
    {
        var withChests = matches.Where(m => m.Chests.Count > 0).ToList();
        if (withChests.Count == 0)
        {
            return "";
        }
        // Times are from the start of the replay (the bus included), so the first chest comes after the drop.
        var firsts = withChests.Select(m => (double)m.Chests.Min(c => c.TimeSeconds)).OrderBy(x => x).ToList();
        var median = firsts[firsts.Count / 2];
        var bestMinute = withChests.Max(m => ProgressStats.MostInWindow(m, 60));

        // Share of the chests opened in each third of the player's own match.
        var phases = new int[3];
        foreach (var m in withChests.Where(m => m.DurationSeconds is > 60))
        {
            foreach (var c in m.Chests)
            {
                phases[Math.Clamp((int)(3 * c.TimeSeconds / m.DurationSeconds!.Value), 0, 2)]++;
            }
        }
        var phaseTotal = Math.Max(1, phases.Sum());
        int Pct(int i) => (int)Math.Round(100.0 * phases[i] / phaseTotal);
        string[] names = { L.T("Début", "Early", "Inicio", "Início"), L.T("Milieu", "Mid", "Mitad", "Meio"), L.T("Fin", "Late", "Final", "Fim") };
        string[] colors = { "#2f7d4f", "#c9a24f", "#a3361f" };
        var bar = phases.Sum() == 0 ? "" : "<div class=\"phases\">" + string.Concat(Enumerable.Range(0, 3).Select(i =>
            $"<i style=\"width:{Pct(i)}%;background:{colors[i]}\">{(Pct(i) >= 8 ? $"{Pct(i)}%" : "")}</i>")) + "</div>"
            + $"<div class=\"plegend\">{string.Concat(Enumerable.Range(0, 3).Select(i => $"<span>{names[i]} · {Pct(i)}%</span>"))}</div>";

        // Time between two chests of the same match (median).
        var gaps = withChests.SelectMany(m => m.Chests.Select(c => c.TimeSeconds).OrderBy(t => t).Zip(m.Chests.Select(c => c.TimeSeconds).OrderBy(t => t).Skip(1), (a, b) => (double)(b - a)))
            .OrderBy(x => x).ToList();
        var gapTile = gaps.Count > 0
            ? Tile(MinSec(gaps[gaps.Count / 2]), L.T("entre deux coffres (médiane)", "between two chests (median)", "entre dos cofres (mediana)", "entre um baú e outro (mediana)"),
                L.T("dans la même partie", "in the same match", "en la misma partida", "na mesma partida"))
            : Tile("—", L.T("entre deux coffres (médiane)", "between two chests (median)", "entre dos cofres (mediana)", "entre um baú e outro (mediana)"), null);

        return $"<section>{Title(L.T("Rythme dans la partie", "Rhythm within a match", "Ritmo en la partida", "Ritmo na partida"))}<section class=\"tiles stats\">"
            + Tile(MinSec(median), L.T("jusqu'au 1er coffre (médiane)", "to the first chest (median)", "hasta el 1.er cofre (mediana)", "até o 1º baú (mediana)"),
                L.T("depuis le début du replay, bus compris", "from the start of the replay, bus included", "desde el inicio de la repetición, autobús incluido", "desde o início do replay, ônibus incluído"))
            + gapTile
            + Tile(L.N(bestMinute), L.T("coffres en 1 minute (record)", "chests within 1 minute (best)", "cofres en 1 minuto (récord)", "baús em 1 minuto (recorde)"),
                L.T("dans une même partie", "in a single match", "en una misma partida", "numa única partida"))
            + StormTile(matches)
            + "</section>"
            + (bar.Length > 0 ? $"<div class=\"card chart\" style=\"margin-top:12px\"><b>{L.T("Quand tu ouvres tes coffres", "When you open your chests", "Cuándo abres tus cofres", "Quando você abre seus baús")}</b> <span class=\"items\">{L.T("(par tiers de ta partie)", "(by thirds of your match)", "(por tercios de tu partida)", "(por terços da sua partida)")}</span>{bar}</div>" : "")
            + "</section>";
    }

    /// <summary>Chests opened outside the safe zone: how many, their share, and the most in one match.</summary>
    private static string StormTile(IReadOnlyList<MatchRecord> matches)
    {
        var label = L.T("coffres ouverts dans la tempête", "chests opened in the storm", "cofres abiertos en la tormenta", "baús abertos na tempestade");
        var known = matches.SelectMany(m => m.Chests).Where(c => c.InStorm != null).ToList();
        if (known.Count == 0)
        {
            return Tile("—", label, L.T("parties analysées à partir de la 1.11", "matches analyzed from 1.11 on", "partidas analizadas desde la 1.11", "partidas analisadas a partir da 1.11"));
        }
        var storm = known.Count(c => c.InStorm == true);
        var best = matches.Max(m => m.Chests.Count(c => c.InStorm == true));
        return Tile($"<span class=\"storm\">{Icons.Svg("storm", "ic ici")}</span> {L.N(storm)}", label,
            L.T($"{Percent(storm, known.Count)} des coffres · record : {L.N(best)}", $"{Percent(storm, known.Count)} of the chests · best: {L.N(best)}",
                $"{Percent(storm, known.Count)} de los cofres · récord: {L.N(best)}", $"{Percent(storm, known.Count)} dos baús · recorde: {L.N(best)} numa partida"));
    }

    private static string Percent(int part, int whole) => whole == 0 ? "0%" : $"{(int)Math.Round(100.0 * part / whole)}%";

    // ---- Chests against the result -----------------------------------------------------------------------------

    private static string ChestsAndResult(IReadOnlyList<MatchRecord> matches)
    {
        var placed = matches.Where(m => m.Placement is > 0).ToList();
        if (placed.Count < 5)
        {
            return "";
        }
        var buckets = new (string Label, Func<int, bool> In)[]
        {
            ("0–2", n => n <= 2), ("3–5", n => n is >= 3 and <= 5), ("6–8", n => n is >= 6 and <= 8), ("9–11", n => n is >= 9 and <= 11), ("12+", n => n >= 12),
        };
        var sb = new StringBuilder($"<section>{Title(L.T("Coffres et résultat", "Chests and result", "Cofres y resultado", "Baús × resultado"),
            L.T("ouvrir plus de coffres te fait-il aller plus loin ?", "do more chests take you further?", "¿abrir más cofres te lleva más lejos?", "abrir mais baús te leva mais longe?"))}<div class=\"card\"><table><tr>"
            + $"<th>{L.T("Coffres dans la partie", "Chests in the match", "Cofres en la partida", "Baús na partida")}</th><th class=\"r\">{L.T("Parties", "Matches", "Partidas", "Partidas")}</th>"
            + $"<th class=\"r\">{L.T("Classement moyen", "Average placement", "Puesto medio", "Colocação média")}</th><th class=\"r\">{L.T("Éliminations", "Eliminations", "Eliminaciones", "Abates")}</th>"
            + $"<th class=\"r\">Top 10</th><th class=\"r\">{WinLabel}</th></tr>");
        foreach (var (label, inBucket) in buckets)
        {
            var list = placed.Where(m => inBucket(m.ChestsOpened)).ToList();
            if (list.Count == 0)
            {
                continue;
            }
            var elims = list.Where(m => m.Eliminations is not null).ToList();
            sb.Append($"<tr><td><b>{label}</b></td><td class=\"sub num\">{L.N(list.Count)}</td>"
                + $"<td class=\"n num\">{L.Ordinal((int)Math.Round(list.Average(m => m.Placement!.Value)))}</td>"
                + $"<td class=\"n num\">{(elims.Count > 0 ? F1(elims.Average(m => m.Eliminations!.Value)) : "—")}</td>"
                + $"<td class=\"n num\">{(int)Math.Round(100.0 * list.Count(m => m.Placement <= 10) / list.Count)}%</td>"
                + $"<td class=\"n num\">{(int)Math.Round(100.0 * list.Count(m => m.Placement == 1) / list.Count)}%</td></tr>");
        }
        return sb.Append("</table></div></section>").ToString();
    }

    // ---- Squad ranking -----------------------------------------------------------------------------------------

    private static string SquadRanking(IReadOnlyList<MatchRecord> matches)
    {
        var withTeam = matches.Where(m => m.TeamChests.Any(t => !t.IsBot)).ToList();
        if (withTeam.Count == 0)
        {
            return "";
        }
        var mates = withTeam
            .SelectMany(m => m.TeamChests.Where(t => !t.IsBot).Select(t => (m, t)))
            .GroupBy(x => x.t.EpicId ?? x.t.Name)
            .Select(g =>
            {
                var latest = g.OrderByDescending(x => Goals.When(x.m)).First().t.Name;
                return (Name: latest, Matches: g.Select(x => x.m.SessionId).Distinct().Count(), Chests: g.Sum(x => x.t.Chests), You: false);
            })
            .ToList();
        var you = matches.Select(m => m.PlayerName).FirstOrDefault(n => !string.IsNullOrEmpty(n)) ?? L.T("toi", "you", "tú", "você");
        // The player, over the same matches (the ones played with somebody).
        mates.Add((you, withTeam.Count, withTeam.Sum(m => m.ChestsOpened), true));
        var rows = mates.OrderByDescending(x => (double)x.Chests / x.Matches).ThenByDescending(x => x.Matches).Take(12).ToList();
        var top = Math.Max(0.1, rows.Max(x => (double)x.Chests / x.Matches));

        var sb = new StringBuilder($"<section>{Title(L.T("Classement de l'équipe", "Squad ranking", "Clasificación del equipo", "Ranking do esquadrão"),
            L.T("coffres par partie jouée ensemble · imprécis au-delà de 150 m de toi", "chests per match played together · inaccurate beyond 150 m from you",
                "cofres por partida jugada juntos · impreciso a más de 150 m de ti", "baús por partida jogada junto · impreciso a mais de 150 m de você"))}"
            + $"<div class=\"card\"><table><tr><th>#</th><th>{L.T("Joueur", "Player", "Jugador", "Jogador")}</th><th></th><th class=\"r\">{L.T("Moyenne", "Average", "Media", "Média")}</th>"
            + $"<th class=\"r\">{L.T("Coffres", "Chests", "Cofres", "Baús")}</th><th class=\"r\">{L.T("Parties ensemble", "Matches together", "Partidas juntos", "Partidas juntos")}</th></tr>");
        for (var i = 0; i < rows.Count; i++)
        {
            var r = rows[i];
            var avg = (double)r.Chests / r.Matches;
            var medal = i < 3 ? $"<span class=\"rank r{i + 1}\">{Icons.Medal(i + 1)}</span>" : $"<span class=\"rank\">{L.N(i + 1)}</span>";
            sb.Append($"<tr><td class=\"num\">{medal}</td><td>{(r.You ? $"<b>{H(r.Name)}</b> ({L.T("toi", "you", "tú", "você")})" : H(r.Name))}</td>"
                + $"<td style=\"width:150px\">{Bar((int)Math.Round(100 * avg / top), r.You ? "var(--team)" : null)}</td>"
                + $"<td class=\"n num big\">{F1(avg)}</td><td class=\"sub num\">{L.N(r.Chests)}</td><td class=\"sub num\">{L.N(r.Matches)}</td></tr>");
        }
        return sb.Append("</table></div></section>").ToString();
    }

    // ---- Loot --------------------------------------------------------------------------------------------------

    private static string LootSection(IReadOnlyList<MatchRecord> matches)
    {
        var items = Loot.FromChests(matches);
        if (items.Count == 0)
        {
            return "";
        }
        string Bars(IEnumerable<(string Label, string Color, int Count)> rows)
        {
            var list = rows.Where(r => r.Count > 0).ToList();
            var max = Math.Max(1, list.Count > 0 ? list.Max(r => r.Count) : 1);
            var total = Math.Max(1, list.Sum(r => r.Count));
            return "<table>" + string.Concat(list.Select(r =>
                $"<tr><td><span class=\"sw\" style=\"background:{r.Color}\"></span>{H(r.Label)}</td><td style=\"width:150px\">{Bar((int)Math.Round(100.0 * r.Count / max), r.Color)}</td>"
                + $"<td class=\"n num\">{L.N(r.Count)}</td><td class=\"sub num\">{(int)Math.Round(100.0 * r.Count / total)}%</td></tr>")) + "</table>";
        }

        var byCategory = items.GroupBy(Loot.CategoryOf).OrderByDescending(g => g.Count()).Select(g => (g.Key.Name, g.Key.Color, g.Count()));
        var rarities = items.Select(Loot.RarityOf).Where(r => r != null).Select(r => r!).ToList();
        var byRarity = Loot.Rarities.Select(r => (r.Name, r.Color, rarities.Count(x => x == r)));
        var weapons = items.Where(i => Loot.CategoryOf(i).Id is not ("ammo" or "mats" or "other")).ToList();
        var topItems = weapons.GroupBy(Loot.Pretty).OrderByDescending(g => g.Count()).Take(10).ToList();

        var sb = new StringBuilder($"<section>{Title(L.T("Ce que tes coffres donnent", "What your chests give", "Lo que dan tus cofres", "O que seus baús dão"),
            L.T($"{L.N(items.Count)} objets sortis de tes coffres", $"{L.N(items.Count)} items out of your chests", $"{L.N(items.Count)} objetos de tus cofres", $"{L.N(items.Count)} itens dos seus baús"))}");
        sb.Append("<div class=\"two\">");
        sb.Append($"<div><div class=\"card\"><b>{L.T("Par catégorie", "By category", "Por categoría", "Por categoria")}</b>{Bars(byCategory)}</div></div>");
        sb.Append($"<div><div class=\"card\"><b>{L.T("Rareté des armes et objets", "Rarity of weapons and items", "Rareza de armas y objetos", "Raridade das armas e itens")}</b>"
            + (rarities.Count > 0 ? Bars(byRarity) : $"<p class=\"items\">{L.T("Pas encore de rareté lue.", "No rarity read yet.", "Todavía ninguna rareza leída.", "Nenhuma raridade lida ainda.")}</p>") + "</div></div>");
        sb.Append("</div>");
        if (topItems.Count > 0)
        {
            sb.Append($"<div class=\"card\" style=\"margin-top:12px\"><b>{L.T("Armes et objets les plus trouvés", "Most found weapons and items", "Armas y objetos más encontrados", "Armas e itens mais encontrados")}</b><table>");
            foreach (var g in topItems)
            {
                var rarity = g.Select(Loot.RarityOf).Where(r => r != null).GroupBy(r => r!).OrderByDescending(x => x.Count()).FirstOrDefault()?.Key;
                var category = Loot.CategoryOf(g.First());
                sb.Append($"<tr><td><span class=\"sw\" style=\"background:{rarity?.Color ?? category.Color}\"></span>{H(g.Key)}</td><td class=\"items\">{H(category.Name)}{(rarity != null ? $" · {H(rarity.Name)}" : "")}</td><td class=\"n num\">{L.N(g.Count())}</td></tr>");
            }
            sb.Append("</table></div>");
        }
        return sb.Append("</section>").ToString();
    }
}
