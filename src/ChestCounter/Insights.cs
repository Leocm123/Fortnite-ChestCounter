namespace ChestCounter;

/// <summary>
/// Records, landing spots, when the player does best, weekly trend, session summary and the season's recap, all
/// computed from the counted matches.
/// </summary>
public static class Insights
{
    // ---- Records -----------------------------------------------------------------------------------------------

    /// <param name="Value">What is compared (higher is better, unless <paramref name="LowerIsBetter"/>).</param>
    /// <param name="When">The day the record was set (shown as "new!" when it is today).</param>
    public sealed record Record(string Id, string Icon, string Name, double Value, string ValueText, string Detail, DateTime When, bool LowerIsBetter = false)
    {
        public bool IsNew => When.Date >= Goals.Today;
        public bool Beats(Record before) => LowerIsBetter ? Value < before.Value : Value > before.Value;
    }

    private static string MinSec(double s) => $"{(int)s / 60}:{(int)s % 60:00}";
    private static string Day(DateTime d) => d.ToString("dd/MM", L.Culture);
    private static string MatchDetail(MatchRecord m) => $"{L.DateTimeShort(Goals.When(m))} · {Pretty.Playlist(m.Playlist)}";

    public static List<Record> Records(IReadOnlyList<MatchRecord> matches)
    {
        var list = new List<Record>();
        if (matches.Count == 0)
        {
            return list;
        }
        string T(string fr, string en, string es, string pt) => L.T(fr, en, es, pt);
        string Chests(int n) => L.Count(n, "coffre", "chest", "cofre", "baú");

        var best = matches.OrderByDescending(m => m.ChestsOpened).ThenBy(Goals.When).First();
        list.Add(new("match", "chest", T("Meilleure partie", "Best match", "Mejor partida", "Melhor partida"), best.ChestsOpened, Chests(best.ChestsOpened), MatchDetail(best), Goals.When(best)));

        var day = matches.GroupBy(m => Goals.When(m).Date).Select(g => (Day: g.Key, Chests: g.Sum(m => m.ChestsOpened))).OrderByDescending(x => x.Chests).ThenBy(x => x.Day).First();
        list.Add(new("day", "calendar", T("Meilleur jour", "Best day", "Mejor día", "Melhor dia"), day.Chests, Chests(day.Chests), day.Day.ToString("dddd dd/MM", L.Culture), day.Day));

        var week = matches.GroupBy(m => Goals.WeekStart(Goals.When(m).Date)).Select(g => (Week: g.Key, Chests: g.Sum(m => m.ChestsOpened))).OrderByDescending(x => x.Chests).ThenBy(x => x.Week).First();
        list.Add(new("week", "calendar-check", T("Meilleure semaine", "Best week", "Mejor semana", "Melhor semana"), week.Chests, Chests(week.Chests),
            T($"semaine du {Day(week.Week)}", $"week of {Day(week.Week)}", $"semana del {Day(week.Week)}", $"semana de {Day(week.Week)}"), week.Week.AddDays(6) < Goals.Today ? week.Week.AddDays(6) : Goals.Today));

        var fast = matches.Select(m => (m, n: ProgressStats.MostInWindow(m, 60))).OrderByDescending(x => x.n).ThenBy(x => Goals.When(x.m)).First();
        if (fast.n > 0)
        {
            list.Add(new("minute", "stopwatch", T("Le plus en 1 minute", "Most within 1 minute", "Más en 1 minuto", "Mais baús em 1 minuto"), fast.n, Chests(fast.n), MatchDetail(fast.m), Goals.When(fast.m)));
        }

        var storm = matches.Select(m => (m, n: m.Chests.Count(c => c.InStorm == true))).OrderByDescending(x => x.n).ThenBy(x => Goals.When(x.m)).First();
        if (storm.n > 0)
        {
            list.Add(new("storm", "storm", T("Le plus dans la tempête", "Most in the storm", "Más en la tormenta", "Mais baús na tempestade"), storm.n, Chests(storm.n), MatchDetail(storm.m), Goals.When(storm.m)));
        }

        var rare = matches.Select(m => (m, n: m.Chests.Count(c => ChestKinds.Canonical(c.Kind) == ChestKinds.Rare))).OrderByDescending(x => x.n).ThenBy(x => Goals.When(x.m)).First();
        if (rare.n > 0)
        {
            list.Add(new("rare", "gem", T("Le plus de coffres rares", "Most rare chests", "Más cofres raros", "Mais baús raros"), rare.n, L.Count(rare.n, "coffre rare", "coffres rares", "rare chest", "rare chests", "cofre raro", "cofres raros", "baú raro", "baús raros"), MatchDetail(rare.m), Goals.When(rare.m)));
        }

        var elims = matches.Where(m => m.Eliminations is > 0).OrderByDescending(m => m.Eliminations).ThenBy(Goals.When).FirstOrDefault();
        if (elims != null)
        {
            list.Add(new("elims", "target", T("Le plus d'éliminations", "Most eliminations", "Más eliminaciones", "Mais abates"), elims.Eliminations!.Value,
                L.Count(elims.Eliminations.Value, "élimination", "elimination", "eliminación", "abate"), MatchDetail(elims), Goals.When(elims)));
        }

        var first = matches.Where(m => m.Chests.Count > 0).Select(m => (m, t: m.Chests.Min(c => c.TimeSeconds))).OrderBy(x => x.t).ThenBy(x => Goals.When(x.m)).FirstOrDefault();
        if (first.m != null)
        {
            list.Add(new("first", "bolt", T("1er coffre le plus rapide", "Fastest first chest", "1.er cofre más rápido", "1º baú mais rápido"), first.t, MinSec(first.t), MatchDetail(first.m), Goals.When(first.m), LowerIsBetter: true));
        }

        var treasureDay = matches.Where(m => m.TreasureFinds.Count > 0).GroupBy(m => Goals.When(m).Date).Select(g => (Day: g.Key, N: g.Sum(m => m.TreasureFinds.Count))).OrderByDescending(x => x.N).ThenBy(x => x.Day).FirstOrDefault();
        if (treasureDay.N > 0)
        {
            list.Add(new("treasures", "star", T("Le plus de trésors en un jour", "Most treasures in a day", "Más tesoros en un día", "Mais tesouros num dia"), treasureDay.N,
                L.Count(treasureDay.N, "trésor", "treasure", "tesoro", "tesouro"), treasureDay.Day.ToString("dddd dd/MM", L.Culture), treasureDay.Day));
        }

        var (_, streak) = ProgressStats.Streaks(matches);
        if (streak > 1)
        {
            list.Add(new("streak", "repeat", T("Plus longue série", "Longest streak", "Racha más larga", "Maior sequência"), streak, L.Count(streak, "jour", "day", "día", "dia"),
                T("jours de suite avec une partie", "days in a row with a match", "días seguidos con una partida", "dias seguidos com partida"), DateTime.MinValue));
        }
        return list;
    }

    /// <summary>The records the new matches beat (records that already existed before them).</summary>
    public static List<Record> Broken(IReadOnlyList<MatchRecord> before, IReadOnlyList<MatchRecord> after)
    {
        var old = Records(before).ToDictionary(r => r.Id);
        return Records(after).Where(r => old.TryGetValue(r.Id, out var o) && r.Beats(o)).ToList();
    }

    // ---- Landing spots ------------------------------------------------------------------------------------------

    public sealed record Landing(string Place, int Matches, double AvgChests, double AvgPlacement, double Top10);

    /// <summary>Where each match started: the named place closest to its first chest. Places with 2 matches or more.</summary>
    public static List<Landing> Landings(IReadOnlyList<MatchRecord> matches)
    {
        var pois = MapData.Pois(L.Code);
        if (pois.Count == 0)
        {
            return new();
        }
        return matches
            .Select(m => (m, first: m.Chests.Where(c => c.X != null && c.Y != null).OrderBy(c => c.TimeSeconds).FirstOrDefault()))
            .Where(x => x.first != null)
            .GroupBy(x => MapData.Nearest(pois, x.first!.X!.Value, x.first.Y!.Value)?.Name ?? MapView.Between)
            .Where(g => g.Count() >= 2)
            .Select(g =>
            {
                var placed = g.Where(x => x.m.Placement is > 0).ToList();
                return new Landing(g.Key, g.Count(), g.Average(x => x.m.ChestsOpened),
                    placed.Count > 0 ? placed.Average(x => x.m.Placement!.Value) : 0,
                    placed.Count > 0 ? (double)placed.Count(x => x.m.Placement <= 10) / placed.Count : 0);
            })
            .OrderByDescending(l => l.Matches).ThenByDescending(l => l.AvgChests)
            .Take(8).ToList();
    }

    // ---- When the player does best: day of the week × time of day -----------------------------------------------

    public sealed record Slot(int Day, int Block, int Matches, double AvgChests);

    /// <summary>Monday = 0; blocks of 4 hours (0 = 0h–4h ... 5 = 20h–24h), in the time shown (local or UTC).</summary>
    public static List<Slot> Slots(IReadOnlyList<MatchRecord> matches) =>
        matches.GroupBy(m => (Day: ((int)Goals.When(m).DayOfWeek + 6) % 7, Block: Goals.When(m).Hour / 4))
            .Select(g => new Slot(g.Key.Day, g.Key.Block, g.Count(), g.Average(m => m.ChestsOpened))).ToList();

    // ---- Weekly trend -----------------------------------------------------------------------------------------------

    public sealed record Week(DateTime Start, int Chests, int Matches, int Treasures);

    public static List<Week> Weeks(IReadOnlyList<MatchRecord> matches, int count = 8)
    {
        var start = Goals.WeekStart(Goals.Today);
        return Enumerable.Range(0, count).Select(i => start.AddDays(-7 * (count - 1 - i))).Select(w =>
        {
            var inWeek = matches.Where(m => Goals.When(m).Date >= w && Goals.When(m).Date < w.AddDays(7)).ToList();
            return new Week(w, inWeek.Sum(m => m.ChestsOpened), inWeek.Count, inWeek.Sum(m => m.TreasureFinds.Count));
        }).ToList();
    }

    // ---- Session summary (when Fortnite closes) ---------------------------------------------------------------------

    public sealed record Session(DateTime From, DateTime To, int Matches, int Chests, int BestMatch, int Treasures, int Storm, double AvgPlacement, List<string> Records);

    public static Session? SessionOf(IReadOnlyList<MatchRecord> all, DateTime fromUtc, DateTime toUtc)
    {
        var inSession = all.Where(m => (m.MatchStartUtc ?? m.AnalyzedAtUtc) >= fromUtc.AddMinutes(-2)).ToList();
        if (inSession.Count == 0)
        {
            return null;
        }
        var before = all.Where(m => !inSession.Contains(m)).ToList();
        var placed = inSession.Where(m => m.Placement is > 0).ToList();
        return new Session(fromUtc, toUtc, inSession.Count, inSession.Sum(m => m.ChestsOpened), inSession.Max(m => m.ChestsOpened),
            inSession.Sum(m => m.TreasureFinds.Count), inSession.Sum(m => m.Chests.Count(c => c.InStorm == true)),
            placed.Count > 0 ? placed.Average(m => m.Placement!.Value) : 0,
            Broken(before, all).Select(r => $"{r.Name}: {r.ValueText}").ToList());
    }

    // ---- The season's recap (shareable picture) ----------------------------------------------------------------------

    public static object Recap(TrackerData view)
    {
        var matches = view.Matches;
        var stats = ProgressStats.From(matches, view.ManualAdjust);
        var landing = Landings(matches).FirstOrDefault();
        // The most found weapon (not a heal, ammo or utility item).
        var named = Loot.FromChests(matches).Where(i => Loot.NameOf(i) != null && Loot.CategoryOf(i).Id is "shotgun" or "rifle" or "smg" or "pistol" or "sniper" or "explosive" or "melee")
            .GroupBy(i => Loot.NameOf(i)).OrderByDescending(g => g.Count()).FirstOrDefault();
        var first = matches.Count > 0 ? matches.Min(Goals.When) : Goals.Today;
        var placed = matches.Where(m => m.Placement is > 0).ToList();
        return new
        {
            player = matches.Select(m => m.PlayerName).FirstOrDefault(n => !string.IsNullOrEmpty(n)) ?? "",
            period = $"{first.ToString("dd/MM", L.Culture)} – {Goals.Today.ToString("dd/MM/yyyy", L.Culture)}",
            total = L.N(view.TotalChests),
            matches = L.N(matches.Count),
            average = matches.Count > 0 ? matches.Average(m => m.ChestsOpened).ToString("0.0", L.Culture) : "0",
            best = L.N(stats.BestMatch),
            bestDay = L.N(stats.BestDay),
            treasures = L.N(stats.Treasures),
            storm = L.N(stats.Storm),
            wins = L.N(stats.Wins),
            top10 = placed.Count > 0 ? $"{(int)Math.Round(100.0 * placed.Count(m => m.Placement <= 10) / placed.Count)}%" : "—",
            streak = L.N(stats.BestStreak),
            achievements = $"{Achievements.Unlocked(stats).Count}/{Achievements.All.Length}",
            landing = landing?.Place,
            weapon = named?.Key,
        };
    }
}
