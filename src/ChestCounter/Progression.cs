namespace ChestCounter;

/// <summary>Daily / weekly chest goals (Settings), shown on the overlay, the home tab and in Discord.</summary>
public static class Goals
{
    /// <summary>Chests to open today (0 = no goal). Counted matches only: the chests added by hand have no date.</summary>
    public static int Daily { get; set; }

    /// <summary>Chests to open this week, from Monday (0 = no goal).</summary>
    public static int Weekly { get; set; }

    public sealed record Progress(string Label, int Value, int Target)
    {
        public bool Done => Value >= Target;
        public string Text => $"{L.N(Value)}/{L.N(Target)}{(Done ? " ✓" : "")}";
        public int Percent => Target <= 0 ? 0 : Math.Min(100, (int)Math.Round(100.0 * Value / Target));
    }

    public static DateTime Today => L.Shown(DateTime.UtcNow).Date;
    public static DateTime WeekStart(DateTime day) => day.AddDays(-(((int)day.DayOfWeek + 6) % 7));
    public static DateTime When(MatchRecord m) => L.Shown(m.MatchStartUtc ?? m.AnalyzedAtUtc);

    public static int ChestsToday(IEnumerable<MatchRecord> matches) => matches.Where(m => When(m).Date == Today).Sum(m => m.ChestsOpened);

    public static int ChestsThisWeek(IEnumerable<MatchRecord> matches)
    {
        var from = WeekStart(Today);
        return matches.Where(m => When(m).Date >= from).Sum(m => m.ChestsOpened);
    }

    public static string DailyLabel => L.T("Objectif du jour", "Daily goal", "Meta del día", "Meta do dia");
    public static string WeeklyLabel => L.T("Objectif de la semaine", "Weekly goal", "Meta de la semana", "Meta da semana");

    /// <param name="liveChests">Chests of the match in progress (already in today's figures on the overlay).</param>
    public static List<Progress> Of(IReadOnlyList<MatchRecord> matches, int liveChests = 0)
    {
        var list = new List<Progress>();
        if (Daily > 0)
        {
            list.Add(new Progress(DailyLabel, ChestsToday(matches) + liveChests, Daily));
        }
        if (Weekly > 0)
        {
            list.Add(new Progress(WeeklyLabel, ChestsThisWeek(matches) + liveChests, Weekly));
        }
        return list;
    }
}

/// <summary>Figures the achievements are checked against.</summary>
public sealed class ProgressStats
{
    public int Total, Matches, BestMatch, BestDay, MostMatchesDay, BestStreak, Wins, LootedWins, Treasures, Golden, Rare, Destroyed, Fastest;

    public static ProgressStats From(IReadOnlyList<MatchRecord> matches, int manual)
    {
        var s = new ProgressStats
        {
            Total = Math.Max(0, matches.Sum(m => m.ChestsOpened) + manual),
            Matches = matches.Count,
            BestMatch = matches.Count > 0 ? matches.Max(m => m.ChestsOpened) : 0,
            Wins = matches.Count(m => m.Placement == 1),
            LootedWins = matches.Count(m => m.Placement == 1 && m.ChestsOpened >= 10),
            Treasures = matches.Sum(m => m.TreasureFinds.Count),
            Golden = matches.Sum(m => m.TreasureFinds.Count(f => f.FromGoldenChest)),
            Rare = matches.Sum(m => m.Chests.Count(c => ChestKinds.Canonical(c.Kind) == ChestKinds.Rare)),
            Destroyed = matches.Sum(m => m.Chests.Count(c => c.Destroyed)),
            Fastest = matches.Count > 0 ? matches.Max(m => MostInWindow(m, 60)) : 0,
        };
        var byDay = matches.GroupBy(m => Goals.When(m).Date).ToList();
        s.BestDay = byDay.Count > 0 ? byDay.Max(g => g.Sum(m => m.ChestsOpened)) : 0;
        s.MostMatchesDay = byDay.Count > 0 ? byDay.Max(g => g.Count()) : 0;
        s.BestStreak = Streaks(matches).Best;
        return s;
    }

    /// <summary>Most chests opened within any window of that many seconds of one match.</summary>
    public static int MostInWindow(MatchRecord m, float seconds)
    {
        var t = m.Chests.Select(c => c.TimeSeconds).OrderBy(x => x).ToList();
        int best = 0, start = 0;
        for (var end = 0; end < t.Count; end++)
        {
            while (t[end] - t[start] > seconds)
            {
                start++;
            }
            best = Math.Max(best, end - start + 1);
        }
        return best;
    }

    /// <summary>Days in a row with a counted match: the current run (today, or up to yesterday) and the longest.</summary>
    public static (int Current, int Best) Streaks(IEnumerable<MatchRecord> matches)
    {
        var days = matches.Select(m => Goals.When(m).Date).ToHashSet();
        var today = Goals.Today;
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
        return (current, best);
    }
}

public sealed record Achievement(string Id, string Icon, string Fr, string En, string Es, string Pt,
    string DescFr, string DescEn, string DescEs, string DescPt, Func<ProgressStats, int> Value, int Target)
{
    public string Name => L.T(Fr, En, Es, Pt);
    public string Description => L.T(DescFr, DescEn, DescEs, DescPt);
    public bool Unlocked(ProgressStats s) => Value(s) >= Target;
}

/// <summary>
/// Achievements, computed from the counted matches (nothing is saved): an achievement is announced when the matches
/// just counted unlock it, by comparing the figures before and after them.
/// </summary>
public static class Achievements
{
    public static readonly Achievement[] All =
    {
        new("first", "🗝️", "Premier coffre", "First chest", "Primer cofre", "Primeiro baú",
            "Ouvre ton premier coffre.", "Open your first chest.", "Abre tu primer cofre.", "Abra seu primeiro baú.", s => s.Total, 1),
        new("c100", "📦", "Chasseur de coffres", "Chest hunter", "Cazador de cofres", "Caçador de baús",
            "100 coffres au total.", "100 chests in total.", "100 cofres en total.", "100 baús no total.", s => s.Total, 100),
        new("c1000", "💰", "Mille coffres", "A thousand chests", "Mil cofres", "Mil baús",
            "1 000 coffres au total.", "1,000 chests in total.", "1000 cofres en total.", "1.000 baús no total.", s => s.Total, 1000),
        new("c5000", "👑", "Légende des coffres", "Chest legend", "Leyenda de los cofres", "Lenda dos baús",
            "5 000 coffres au total.", "5,000 chests in total.", "5000 cofres en total.", "5.000 baús no total.", s => s.Total, 5000),
        new("c10000", "🏴‍☠️", "Roi des pirates", "Pirate king", "Rey de los piratas", "Rei dos piratas",
            "10 000 coffres au total.", "10,000 chests in total.", "10 000 cofres en total.", "10.000 baús no total.", s => s.Total, 10000),
        new("m10", "⚡", "Pilleur", "Looter", "Saqueador", "Saqueador",
            "10 coffres en une partie.", "10 chests in one match.", "10 cofres en una partida.", "10 baús numa partida.", s => s.BestMatch, 10),
        new("m15", "🔥", "Pilleur vorace", "Greedy looter", "Saqueador voraz", "Saqueador voraz",
            "15 coffres en une partie.", "15 chests in one match.", "15 cofres en una partida.", "15 baús numa partida.", s => s.BestMatch, 15),
        new("m20", "💥", "Rien ne reste fermé", "Nothing stays closed", "Nada se queda cerrado", "Nada fica fechado",
            "20 coffres en une partie.", "20 chests in one match.", "20 cofres en una partida.", "20 baús numa partida.", s => s.BestMatch, 20),
        new("fast", "⏱️", "Main rapide", "Quick hands", "Manos rápidas", "Mão rápida",
            "3 coffres en moins d'une minute.", "3 chests within one minute.", "3 cofres en menos de un minuto.", "3 baús em menos de um minuto.", s => s.Fastest, 3),
        new("day50", "📅", "Journée productive", "Productive day", "Día productivo", "Dia produtivo",
            "50 coffres dans la même journée.", "50 chests in one day.", "50 cofres en un mismo día.", "50 baús no mesmo dia.", s => s.BestDay, 50),
        new("marathon", "🏃", "Marathon", "Marathon", "Maratón", "Maratona",
            "10 parties dans la même journée.", "10 matches in one day.", "10 partidas en un mismo día.", "10 partidas no mesmo dia.", s => s.MostMatchesDay, 10),
        new("streak3", "🔁", "Régulier", "Regular", "Constante", "Constância",
            "Joue 3 jours de suite.", "Play 3 days in a row.", "Juega 3 días seguidos.", "Jogue 3 dias seguidos.", s => s.BestStreak, 3),
        new("streak7", "📆", "Semaine parfaite", "Perfect week", "Semana perfecta", "Semana perfeita",
            "Joue 7 jours de suite.", "Play 7 days in a row.", "Juega 7 días seguidos.", "Jogue 7 dias seguidos.", s => s.BestStreak, 7),
        new("streak14", "🗓️", "Accro", "Hooked", "Enganchado", "Viciado",
            "Joue 14 jours de suite.", "Play 14 days in a row.", "Juega 14 días seguidos.", "Jogue 14 dias seguidos.", s => s.BestStreak, 14),
        new("win", "🏆", "Top 1", "Victory Royale", "Victoria magistral", "Vitória Royale",
            "Gagne une partie.", "Win a match.", "Gana una partida.", "Vença uma partida.", s => s.Wins, 1),
        new("lootwin", "🎯", "Top 1 bien équipé", "Well-looted win", "Victoria bien saqueada", "Vitória com saque",
            "Gagne une partie avec 10 coffres ou plus.", "Win a match with 10 or more chests.", "Gana una partida con 10 cofres o más.", "Vença uma partida com 10 baús ou mais.", s => s.LootedWins, 1),
        new("treasure", "★", "Premier trésor", "First treasure", "Primer tesoro", "Primeiro tesouro",
            "Trouve un trésor dans un de tes coffres.", "Find a treasure in one of your chests.", "Encuentra un tesoro en uno de tus cofres.", "Encontre um tesouro num dos seus baús.", s => s.Treasures, 1),
        new("golden", "🌟", "Coffre doré", "Golden chest", "Cofre dorado", "Baú dourado",
            "Obtiens un trésor d'un coffre doré.", "Get a treasure from a golden chest.", "Consigue un tesoro de un cofre dorado.", "Ganhe um tesouro de um baú dourado.", s => s.Golden, 1),
        new("rare50", "💎", "Collectionneur de rares", "Rare collector", "Coleccionista de raros", "Colecionador de raros",
            "50 coffres rares.", "50 rare chests.", "50 cofres raros.", "50 baús raros.", s => s.Rare, 50),
        new("destroy25", "🔨", "Démolisseur", "Wrecker", "Demoledor", "Destruidor",
            "Casse 25 coffres au lieu de les ouvrir.", "Break 25 chests instead of opening them.", "Rompe 25 cofres en lugar de abrirlos.", "Quebre 25 baús em vez de abrir.", s => s.Destroyed, 25),
        new("veteran", "🎮", "Vétéran", "Veteran", "Veterano", "Veterano",
            "100 parties comptées.", "100 counted matches.", "100 partidas contadas.", "100 partidas contadas.", s => s.Matches, 100),
    };

    public static List<Achievement> Unlocked(ProgressStats s) => All.Where(a => a.Unlocked(s)).ToList();
}
