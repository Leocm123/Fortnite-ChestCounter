namespace ChestCounter;

/// <summary>
/// What the map shows (the "Map" tab of the window and the map of the report): the chests that have a position, where
/// to crop the island's picture, and the named places where the most chests are opened.
/// </summary>
public static class MapView
{
    public sealed record Point(MatchRecord Match, ChestRecord Chest, double U, double V);

    public static List<Point> Points(IEnumerable<MatchRecord> matches) =>
        matches.SelectMany(m => m.Chests.Where(c => c.X != null && c.Y != null).Select(c =>
        {
            var (u, v) = MapData.Project(c.X!.Value, c.Y!.Value);
            return new Point(m, c, u, v);
        })).ToList();

    /// <summary>A square on the picture (0..1) around the named places and the chests, with a margin.</summary>
    public static (double U, double V, double Size) Crop(IReadOnlyList<Point> points, IReadOnlyList<MapData.Poi> pois)
    {
        var us = points.Select(p => (p.U, p.V)).Concat(pois.Select(p => MapData.Project(p.X, p.Y))).ToList();
        if (us.Count == 0)
        {
            return (0, 0, 1);
        }
        double u0 = us.Min(p => p.U), u1 = us.Max(p => p.U), v0 = us.Min(p => p.V), v1 = us.Max(p => p.V);
        var size = Math.Clamp(Math.Max(u1 - u0, v1 - v0) + 0.08, 0.15, 1.0);
        var cu = Math.Clamp((u0 + u1) / 2, size / 2, 1 - size / 2);
        var cv = Math.Clamp((v0 + v1) / 2, size / 2, 1 - size / 2);
        return (cu - size / 2, cv - size / 2, size);
    }

    public static string Between => L.T("Entre deux lieux", "Between places", "Entre lugares", "Entre locais");

    /// <summary>The named places with the most chests (the closest place within 300 m of each chest).</summary>
    public static List<(string Name, int Count, int Matches)> Places(IReadOnlyList<Point> points, IReadOnlyList<MapData.Poi> pois, int top = 10) =>
        points.GroupBy(p => MapData.Nearest(pois, p.Chest.X!.Value, p.Chest.Y!.Value)?.Name ?? Between)
            .Select(g => (Name: g.Key, Count: g.Count(), Matches: g.Select(x => x.Match.SessionId).Distinct().Count()))
            .OrderByDescending(x => x.Count).Take(top).ToList();

    /// <summary>
    /// Everything the window's map draws, in a compact form: each chest as [u, v, kind, flags, match, time] (u and v in
    /// thousandths of the picture; flags: 1 = this week, 2 = last match, 4 = opened in the storm; match: index in
    /// "matches", -1 for an older one; time: seconds into the replay).
    /// </summary>
    public static object Data(TrackerData view)
    {
        var matches = view.Matches;
        var points = Points(matches);
        var pois = MapData.Pois(L.Code);
        var crop = Crop(points, pois);
        var weekStart = Goals.WeekStart(Goals.Today);
        var last = matches.OrderByDescending(Goals.When).FirstOrDefault();
        var kinds = points.Select(p => ChestKinds.Canonical(p.Chest.Kind)).Distinct().ToList();
        // The latest matches with placed chests, to show one of them as a route (newest first).
        var recentMatches = matches.Where(m => m.Chests.Any(c => c.X != null)).OrderByDescending(Goals.When).Take(60).ToList();
        var matchIndex = recentMatches.Select((m, i) => (m.SessionId, i)).ToDictionary(x => x.SessionId, x => x.i);
        var image = MapData.HasImage && MapData.Folder != null
            ? $"/images/{MapData.ImageName}?v={File.GetLastWriteTimeUtc(Path.Combine(MapData.Folder, MapData.ImageName)).Ticks}"
            : null;
        return new
        {
            image,
            crop = new[] { crop.U * 1000, crop.V * 1000, crop.Size * 1000 },
            kinds = kinds.Select(k => new { name = L.Name(k), color = ReportWriter.KindColor.GetValueOrDefault(k, "#9a8f7a"), count = points.Count(p => ChestKinds.Canonical(p.Chest.Kind) == k) }).ToList(),
            points = points.Select(p => new[]
            {
                (int)Math.Round(p.U * 1000), (int)Math.Round(p.V * 1000), kinds.IndexOf(ChestKinds.Canonical(p.Chest.Kind)),
                (Goals.When(p.Match).Date >= weekStart ? 1 : 0) | (p.Match.SessionId == last?.SessionId ? 2 : 0) | (p.Chest.InStorm == true ? 4 : 0),
                matchIndex.GetValueOrDefault(p.Match.SessionId, -1), (int)Math.Round(p.Chest.TimeSeconds),
            }).ToList(),
            matches = recentMatches.Select(m => new
            {
                id = m.SessionId,
                label = $"{L.DateTimeShort(Goals.When(m))} · {Pretty.Playlist(m.Playlist)} · {Pretty.Placement(m)} · {L.Count(m.ChestsOpened, "coffre", "chest", "cofre", "baú")}",
                chests = m.ChestsOpened,
                storm = m.Chests.Count(c => c.InStorm == true),
                place = Pretty.Placement(m),
                elims = m.Eliminations,
                // The match timeline: its length, chests [t, color, kind, destroyed, storm], treasures [t, name],
                // sprites carried [from, to, id] and the player's events [t, kind].
                tl = new
                {
                    dur = Math.Max(m.DurationSeconds ?? 0, (int)Math.Ceiling(Math.Max(m.Chests.Count > 0 ? m.Chests.Max(c => c.TimeSeconds) : 0, m.Events?.Count > 0 ? m.Events.Max(e => e.T) : 0))),
                    chests = m.Chests.OrderBy(c => c.TimeSeconds).Select(c => new object[] { (int)Math.Round(c.TimeSeconds), ReportWriter.KindColor.GetValueOrDefault(ChestKinds.Canonical(c.Kind), "#9a8f7a"), L.Name(ChestKinds.Canonical(c.Kind)), c.Destroyed ? 1 : 0, c.InStorm == true ? 1 : 0 }),
                    tre = m.TreasureFinds.Where(f => f.ObtainedAtSeconds != null).Select(f => new object[] { (int)Math.Round(f.ObtainedAtSeconds!.Value), f.DisplayName }),
                    spr = (m.Sprites ?? []).Select(s => new object[] { (int)Math.Round(s.From), (int)Math.Round(s.To), s.Id }),
                    ev = (m.Events ?? []).Select(e => new object[] { (int)Math.Round(e.T), e.Kind }),
                },
            }).ToList(),
            sprites = recentMatches.SelectMany(m => m.Sprites ?? []).Select(s => s.Id).Distinct()
                .ToDictionary(id => id, id => { var (k, v) = Sprites.Parse(id); return new { name = Sprites.Name(id), img = Sprites.Image(k, v) }; }),
            pois = pois.Select(p => { var (u, v) = MapData.Project(p.X, p.Y); return new { name = p.Name, u = Math.Round(u * 1000, 1), v = Math.Round(v * 1000, 1) }; }).ToList(),
            places = Places(points, pois).Select(p => new { name = p.Name, count = L.N(p.Count), matches = L.Count(p.Matches, "partie", "parties", "match", "matches", "partida", "partidas", "partida", "partidas"), n = p.Count }).ToList(),
            total = L.N(points.Count),
            week = L.N(points.Count(p => Goals.When(p.Match).Date >= weekStart)),
            lastCount = L.N(points.Count(p => p.Match.SessionId == last?.SessionId)),
            source = image != null ? "fortnite-api.com" : null,
        };
    }
}
