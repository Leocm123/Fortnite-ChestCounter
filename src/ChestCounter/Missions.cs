namespace ChestCounter;

/// <summary>
/// Weekly missions: three challenges drawn each Monday from what the player usually does (buried chests, storm,
/// destroyed, rare, sprite chests, treasures, favourite sprite, best match), each target a little above their average
/// of the 4 weeks before. Same missions all week (drawn from the week's date), progress from this week's matches.
/// </summary>
public static class Missions
{
    public sealed record Mission(string Id, string Icon, string Title, int Value, int Target, string? Image = null)
    {
        public bool Done => Value >= Target;
        public int Percent => Target <= 0 ? 0 : Math.Min(100, (int)Math.Round(100.0 * Value / Target));
    }

    private static int Metric(string id, IEnumerable<MatchRecord> matches, string? sprite) => id switch
    {
        "buried" => matches.Sum(m => m.Chests.Count(c => ChestKinds.Canonical(c.Kind) == ChestKinds.Buried)),
        "storm" => matches.Sum(m => m.Chests.Count(c => c.InStorm == true)),
        "destroyed" => matches.Sum(m => m.Chests.Count(c => c.Destroyed)),
        "rare" => matches.Sum(m => m.Chests.Count(c => ChestKinds.Canonical(c.Kind) == ChestKinds.Rare)),
        "spirit" => matches.Sum(m => m.Chests.Count(c => ChestKinds.Canonical(c.Kind) == ChestKinds.Extraction)),
        "treasure" => matches.Sum(m => m.TreasureFinds.Count),
        "sprite" => matches.Sum(m => m.Chests.Count(c => c.Sprite != null && Sprites.Parse(c.Sprite).Kind == sprite)),
        "best" => matches.Select(m => m.ChestsOpened).DefaultIfEmpty(0).Max(),
        _ => 0,
    };

    public static List<Mission> Of(IReadOnlyList<MatchRecord> matches)
    {
        var week = Goals.WeekStart(Goals.Today);
        var thisWeek = matches.Where(m => Goals.When(m).Date >= week).ToList();
        var history = matches.Where(m => Goals.When(m).Date < week && Goals.When(m).Date >= week.AddDays(-28)).ToList();
        if (history.Count == 0)
        {
            history = matches.Where(m => Goals.When(m).Date < week).ToList();
        }
        var weeks = Math.Max(1, history.Select(m => Goals.WeekStart(Goals.When(m).Date)).Distinct().Count());
        var favorite = matches.SelectMany(m => m.Chests).Where(c => c.Sprite != null)
            .GroupBy(c => Sprites.Parse(c.Sprite!).Kind).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();

        // Target: the weekly average of the 4 weeks before plus 20 %, at least the minimum.
        int Target(string id, int min) => Math.Max(min, (int)Math.Ceiling(Metric(id, history, favorite) / (double)weeks * 1.2));
        var candidates = new List<(string Id, string Icon, int Target, Func<int, string> Title, bool Ok)>
        {
            ("buried", "anchor", Target("buried", 3), n => L.T($"Ouvre {n} coffres enterrés", $"Open {n} buried chests", $"Abre {n} cofres enterrados", $"Abra {n} baús enterrados"), true),
            ("storm", "storm", Target("storm", 3), n => L.T($"Ouvre {n} coffres dans la tempête", $"Open {n} chests in the storm", $"Abre {n} cofres en la tormenta", $"Abra {n} baús na tempestade"), true),
            ("destroyed", "hammer", Target("destroyed", 10), n => L.T($"Détruis {n} coffres", $"Destroy {n} chests", $"Destruye {n} cofres", $"Destrua {n} baús"), true),
            ("rare", "gem", Target("rare", 3), n => L.T($"Ouvre {n} coffres rares", $"Open {n} rare chests", $"Abre {n} cofres raros", $"Abra {n} baús raros"), Metric("rare", history, null) > 0),
            ("spirit", "spirit", Target("spirit", 2), n => L.T($"Ouvre {n} coffres d'esprit", $"Open {n} sprite chests", $"Abre {n} cofres de espíritu", $"Abra {n} baús de espírito"), Metric("spirit", history, null) > 0),
            ("treasure", "star", Target("treasure", 1), n => L.T($"Trouve {n} trésors", $"Find {n} treasures", $"Encuentra {n} tesoros", $"Encontre {n} tesouros"), Metric("treasure", history, null) > 0),
            ("best", "bolt", Math.Max(8, (int)Math.Ceiling((history.Count > 0 ? history.Average(m => m.ChestsOpened) : 6) * 1.5)),
                n => L.T($"Ouvre {n} coffres en une partie", $"Open {n} chests in one match", $"Abre {n} cofres en una partida", $"Abra {n} baús numa partida"), true),
        };
        if (favorite != null)
        {
            var name = Sprites.KindName(favorite);
            candidates.Add(("sprite", "spirit", Target("sprite", 10), n => L.T($"Ouvre {n} coffres avec {name}", $"Open {n} chests with {name}", $"Abre {n} cofres con {name}", $"Abra {n} baús com {name}"), true));
        }

        // Three of them, the same all week: shuffled with the week's date as the seed.
        var rng = new Random(week.Year * 1000 + week.DayOfYear);
        return candidates.Where(c => c.Ok).OrderBy(_ => rng.Next()).Take(3)
            .Select(c => new Mission(c.Id, c.Icon, c.Title(c.Target), Math.Min(Metric(c.Id, thisWeek, favorite), int.MaxValue), c.Target,
                c.Id == "sprite" && favorite != null ? Sprites.Image(favorite, "A") : null))
            .ToList();
    }
}
