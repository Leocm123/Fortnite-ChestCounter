using System.Net;
using System.Text;

namespace ChestCounter;

/// <summary>
/// "Your stats" section of the report: pace (chests per hour played), streak of days played, this week against last
/// week, recent form, and the average by mode and by placement. Built from the counted matches only (the chests added
/// by hand have no match, no date and no duration).
/// </summary>
public static class ReportStats
{
    public const string Css = """
        .tiles.stats{grid-template-columns:repeat(4,minmax(0,1fr))}
        @media (max-width:820px){.tiles.stats{grid-template-columns:repeat(2,minmax(0,1fr))}}
        .trend{font-weight:700}.trend.up{color:#2f7d4f}.trend.down{color:var(--warn)}
        td.sub{color:var(--cmuted);font-size:13px;white-space:nowrap;text-align:right}
        th.r{text-align:right}
        """;

    private static string H(string? s) => WebUtility.HtmlEncode(s ?? "");
    private static DateTime When(MatchRecord m) => L.Shown(m.MatchStartUtc ?? m.AnalyzedAtUtc);
    private static string F1(double v) => v.ToString("0.0", L.Culture);

    private static string Tile(string value, string label, string? sub) =>
        $"""<div class="tile"><div class="v num">{value}</div><div class="l">{label}</div>{(sub != null ? $"<div class=\"s\">{sub}</div>" : "")}</div>""";

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

    public static string Section(IReadOnlyList<MatchRecord> matches)
    {
        if (matches.Count == 0)
        {
            return "";
        }
        var sb = new StringBuilder();
        var average = matches.Average(m => m.ChestsOpened);
        var today = L.Shown(DateTime.UtcNow).Date;

        // Pace: chests per hour spent in matches (the replay's length: from the bus to the end of the player's match).
        var timed = matches.Where(m => m.DurationSeconds is > 60).ToList();
        var hours = timed.Sum(m => (double)m.DurationSeconds!.Value) / 3600;
        var pace = hours > 0.05
            ? Tile(F1(timed.Sum(m => m.ChestsOpened) / hours), L.T("coffres par heure de jeu", "chests per hour played", "cofres por hora de juego", "baús por hora de jogo"),
                L.T($"sur {F1(hours)} h de parties", $"over {F1(hours)} h of matches", $"en {F1(hours)} h de partidas", $"em {F1(hours)} h de partidas"))
            : "";

        // Streak: days in a row with at least one counted match (today, or up to yesterday while today is still open).
        var days = matches.Select(m => When(m).Date).ToHashSet();
        var current = 0;
        for (var d = days.Contains(today) ? today : today.AddDays(-1); days.Contains(d); d = d.AddDays(-1))
        {
            current++;
        }
        int best = 0, run = 0;
        DateTime? previous = null;
        foreach (var d in days.OrderBy(x => x))
        {
            run = previous == d.AddDays(-1) ? run + 1 : 1;
            best = Math.Max(best, run);
            previous = d;
        }
        var streak = Tile(L.Count(current, "jour", "day", "día", "dia"), L.T("série en cours", "current streak", "racha actual", "sequência atual"),
            L.T("record : ", "best: ", "récord: ", "recorde: ") + L.Count(best, "jour", "day", "día", "dia"));

        // This week (from Monday, so far) against the whole of last week.
        var weekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));
        int ChestsOfWeek(DateTime from) => matches.Where(m => When(m).Date >= from && When(m).Date < from.AddDays(7)).Sum(m => m.ChestsOpened);
        var thisWeek = ChestsOfWeek(weekStart);
        var lastWeek = ChestsOfWeek(weekStart.AddDays(-7));
        var week = Tile(L.N(thisWeek), L.T("coffres cette semaine", "chests this week", "cofres esta semana", "baús nesta semana"),
            L.T($"semaine dernière : {L.N(lastWeek)}", $"last week: {L.N(lastWeek)}", $"semana pasada: {L.N(lastWeek)}", $"semana passada: {L.N(lastWeek)}")
            + " " + Trend(thisWeek, lastWeek));

        // Form: the last 10 matches against the overall average.
        var recent = matches.OrderByDescending(When).Take(10).ToList();
        var recentAverage = recent.Average(m => m.ChestsOpened);
        var form = matches.Count >= 15
            ? Tile(F1(recentAverage), L.T("moyenne des 10 dernières parties", "average of the last 10 matches", "media de las 10 últimas partidas", "média das últimas 10 partidas"),
                L.T($"moyenne générale : {F1(average)}", $"overall average: {F1(average)}", $"media general: {F1(average)}", $"média geral: {F1(average)}")
                + " " + Trend(recentAverage, average))
            : "";

        sb.Append($"<section><h2 class=\"pirate\">{L.T("Tes statistiques", "Your stats", "Tus estadísticas", "Suas estatísticas")}</h2>"
            + $"<section class=\"tiles stats\">{pace}{streak}{week}{form}</section></section>");

        // By mode and by placement: matches, average chests (with a bar), best match.
        string Table(string title, string firstColumn, IEnumerable<(string Label, List<MatchRecord> Matches)> groups)
        {
            var rows = groups.Where(g => g.Matches.Count > 0).ToList();
            var top = Math.Max(0.1, rows.Count > 0 ? rows.Max(g => g.Matches.Average(m => m.ChestsOpened)) : 0.1);
            var t = new StringBuilder($"<section><h2 class=\"pirate\">{title}</h2><div class=\"card\"><table><tr><th>{firstColumn}</th><th></th>"
                + $"<th class=\"r\">{L.T("Moyenne", "Average", "Media", "Média")}</th><th class=\"r\">{L.T("Parties", "Matches", "Partidas", "Partidas")}</th></tr>");
            foreach (var (label, list) in rows)
            {
                var avg = list.Average(m => m.ChestsOpened);
                var w = Math.Max(2, (int)Math.Round(100 * avg / top));
                t.Append($"<tr><td>{H(label)}</td><td style=\"width:150px\"><span class=\"meter\"><i style=\"width:{w}%;background:var(--gold)\"></i></span></td>"
                    + $"<td class=\"n num big\">{F1(avg)}</td><td class=\"sub num\">{L.N(list.Count)}</td></tr>");
            }
            return t.Append("</table></div></section>").ToString();
        }

        var byMode = matches.GroupBy(m => Pretty.Playlist(m.Playlist))
            .OrderByDescending(g => g.Count())
            .Select(g => (g.Key, g.ToList()));
        var placed = matches.Where(m => m.Placement is > 0).ToList();
        var byPlacement = new (string, List<MatchRecord>)[]
        {
            (L.T("Top 1", "Victory Royale", "Victoria magistral", "Vitória Royale"), placed.Where(m => m.Placement == 1).ToList()),
            ("Top 2–10", placed.Where(m => m.Placement is >= 2 and <= 10).ToList()),
            ("Top 11–25", placed.Where(m => m.Placement is >= 11 and <= 25).ToList()),
            (L.T("26e et au-delà", "26th and beyond", "26.º o peor", "26º ou pior"), placed.Where(m => m.Placement >= 26).ToList()),
        };
        sb.Append("<div class=\"two\">")
            .Append(Table(L.T("Coffres par mode", "Chests by mode", "Cofres por modo", "Baús por modo"), L.T("Mode", "Mode", "Modo", "Modo"), byMode))
            .Append(Table(L.T("Coffres par classement", "Chests by placement", "Cofres por puesto", "Baús por colocação"), L.T("Classement", "Placement", "Puesto", "Colocação"), byPlacement))
            .Append("</div>");
        return sb.ToString();
    }
}
