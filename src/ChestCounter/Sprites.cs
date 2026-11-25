using System.Text.RegularExpressions;

namespace ChestCounter;

/// <summary>
/// Sprites ("elementals" in the Brazilian version): the companion a player carries, read from the replay as
/// "ESD_WhiteTigerSprite_Variant_Cursed" (kind + variant). Names: the ones confirmed in game, else the codename made
/// readable ("Daffodil Churn"), never an invented one.
/// </summary>
public static class Sprites
{
    private static readonly Regex Id = new(@"^ESD_(?<kind>.+?)Sprite(?:_Variant_(?<variant>.+))?$", RegexOptions.Compiled);

    // Codename -> (English, Portuguese): the names in the game (Treasure Hunters, November 2026). The codename to sprite
    // link comes from the replays and the abilities (CoinPile = Plundarrgh / Moedonho, confirmed in game); the
    // Portuguese names from the pt-BR client. A sprite missing here shows its codename made readable.
    private static readonly Dictionary<string, (string? En, string? Pt)> Known = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CoinPile"] = ("Plundarrgh", "Moedonho"),
        ["WhiteTiger"] = ("Sir Oscar the Second", "Oscar II"),
        ["FeedJet"] = ("Transponder Snail", "Caracol Telepático"),
        ["Parrot"] = ("Re-Pete", "Repetácio"),
        ["UnbotheredFish"] = ("Kelper", "Apatilda"),
        ["TreasureChest"] = ("Chester", "Baunardo"),
        ["Compass"] = ("Wayfinder", "Localizador"),
        ["TreasureMap"] = ("Exmarks", "Xisconde"),
        ["BarrelOfFish"] = ("Barry O'Fish", "Barrilson"),
        ["PeelyBone"] = ("Halfpeel'd", "Descascadinho"),
        ["Squibbly"] = ("Lil' Squibbly", "Polvitozinho"),
        ["PirateCalamari"] = ("Squiplash", "Mô Lusco"),
        ["PlaymakerBrewery"] = ("Lil' Flacko", "Flacko Jr."),
        ["RookGecko"] = ("Lil' Uzi Vert Jr.", "Lil' Uzi Vert Jr."),
        ["DaffodilChurn"] = ("Mini Miku", "Mini Miku"),
        ["YortMelee"] = ("Zee", "Ponto Zé"),
        ["StarLight"] = ("Sweet Dreams", "Doces Sonhos"),
    };

    private static readonly Dictionary<string, string?> Images = new();

    /// <summary>The sprite's picture (built into the exe) as a data URL: its variant, else its plain look; null when unknown.</summary>
    public static string? Image(string kind, string variant)
    {
        lock (Images)
        {
            var key = kind + "-" + variant;
            if (Images.TryGetValue(key, out var cached)) return cached;
            var asm = typeof(Sprites).Assembly;
            using var s = asm.GetManifestResourceStream($"Sprites.{key}.png") ?? asm.GetManifestResourceStream($"Sprites.{kind}-A.png");
            string? url = null;
            if (s != null)
            {
                using var ms = new MemoryStream();
                s.CopyTo(ms);
                url = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
            }
            return Images[key] = url;
        }
    }

    public static (string Kind, string Variant) Parse(string id)
    {
        var m = Id.Match(id);
        return m.Success ? (m.Groups["kind"].Value, m.Groups["variant"].Success ? m.Groups["variant"].Value : "A") : (id, "A");
    }

    /// <summary>"Oscar II" (the sprite, whatever its variant).</summary>
    public static string KindName(string kind)
    {
        Known.TryGetValue(kind, out var k);
        var readable = Regex.Replace(kind, "(?<=[a-z])(?=[A-Z])", " ");
        return L.Code == "pt" ? k.Pt ?? k.En ?? readable : k.En ?? readable;
    }

    /// <summary>"Cursed", "Jeweled"...; empty for the plain one.</summary>
    public static string VariantName(string variant) => variant switch
    {
        "A" or "" => "",
        "Cursed" => L.T("Maudit", "Cursed", "Maldito", "Amaldiçoado"),
        "Jeweled" => L.T("Fortune", "Fortune", "Fortuna", "Valioso"),
        "LiquidGold" => L.T("Doré", "Gold", "Dorado", "Dourado"),
        _ => Regex.Replace(variant, "(?<=[a-z])(?=[A-Z])", " "),
    };

    /// <summary>"Oscar II Amaldiçoado".</summary>
    public static string Name(string id)
    {
        var (kind, variant) = Parse(id);
        var v = VariantName(variant);
        return v.Length == 0 ? KindName(kind) : $"{KindName(kind)} {v}";
    }

    /// <summary>
    /// The Elementals tab: chests per sprite (with its variants), opened or destroyed, by kind of chest, the treasures
    /// found and the pace (chests per minute carried, from the first chest of the match on). Matches analyzed before
    /// sprites were read are left out.
    /// </summary>
    public static object Stats(TrackerData data)
    {
        var view = Profiles.Counted(data).ToList();
        var matches = view.Where(m => m.Sprites != null).ToList();
        var groups = new Dictionary<string, Acc>();
        Acc Get(string key) => groups.TryGetValue(key, out var a) ? a : groups[key] = new Acc();
        foreach (var m in matches)
        {
            var start = m.Chests.Count > 0 ? m.Chests.Min(c => c.TimeSeconds) : float.MaxValue;
            var end = m.Chests.Count > 0 ? m.Chests.Max(c => c.TimeSeconds) : 0;
            var carried = new HashSet<string>();
            foreach (var s in m.Sprites!)
            {
                // Time that counts: from the first chest of the match (landed) to the last one.
                var secs = Math.Max(0, Math.Min(s.To, end) - Math.Max(s.From, start));
                if (secs <= 0) continue;
                var (kind, variant) = Parse(s.Id);
                Get(kind).Seconds += secs;
                Get(kind).Carried[variant] = Get(kind).Carried.GetValueOrDefault(variant) + secs;
                carried.Add(kind);
            }
            foreach (var kind in carried) Get(kind).Matches++;
            // Treasures: the sprite carried when each was received.
            foreach (var f in m.TreasureFinds)
            {
                if (f.ObtainedAtSeconds is not float t) continue;
                var span = m.Sprites!.FirstOrDefault(s => s.From <= t && t < s.To);
                Get(span != null ? Parse(span.Id).Kind : "").Treasures++;
            }
            foreach (var c in m.Chests)
            {
                var key = c.Sprite != null ? Parse(c.Sprite).Kind : "";
                var a = Get(key);
                a.Chests++;
                if (c.Destroyed) a.Destroyed++;
                var k = ChestKinds.Canonical(c.Kind);
                a.Kinds[k] = a.Kinds.GetValueOrDefault(k) + 1;
                if (c.Sprite != null)
                {
                    var v = Parse(c.Sprite).Variant;
                    a.Variants[v] = a.Variants.GetValueOrDefault(v) + 1;
                }
            }
        }

        var total = groups.Values.Sum(a => a.Chests);
        // The same kinds of chest on every card (zeros included), in a fixed order: the cards keep the same height.
        var order = new[] { ChestKinds.Normal, ChestKinds.Rare, ChestKinds.Buried, ChestKinds.Extraction };
        var kindList = order.Concat(groups.Values.SelectMany(a => a.Kinds.Keys).Distinct().Where(k => !order.Contains(k)).OrderBy(k => k)).ToList();
        var rows = groups.Where(g => g.Value.Chests > 0 || g.Value.Seconds > 0 || g.Value.Treasures > 0)
            .OrderBy(g => g.Key.Length == 0).ThenByDescending(g => g.Value.Chests).ThenByDescending(g => g.Value.Seconds)
            .Select(g => new
            {
                id = g.Key,
                name = g.Key.Length == 0 ? L.T("Sans esprit", "No sprite", "Sin espíritu", "Sem elemental") : KindName(g.Key),
                none = g.Key.Length == 0,
                img = g.Key.Length == 0 ? null : Image(g.Key, g.Value.Variants.OrderByDescending(v => v.Value).Select(v => v.Key).FirstOrDefault()
                    ?? g.Value.Carried.OrderByDescending(v => v.Value).Select(v => v.Key).FirstOrDefault() ?? "A"),
                chests = g.Value.Chests,
                opened = g.Value.Chests - g.Value.Destroyed,
                destroyed = g.Value.Destroyed,
                treasures = g.Value.Treasures,
                matches = g.Value.Matches,
                minutes = Math.Round(g.Value.Seconds / 60.0, 1),
                perMin = g.Value.Seconds >= 60 ? Math.Round(g.Value.Chests / (g.Value.Seconds / 60.0), 2) : (double?)null,
                share = total == 0 ? 0 : (int)Math.Round(100.0 * g.Value.Chests / total),
                kinds = kindList.Select(k => new { name = L.Name(k), key = k, n = g.Value.Kinds.GetValueOrDefault(k) }),
                variants = g.Value.Variants.OrderByDescending(v => v.Value).Select(v => new { name = VariantName(v.Key) is { Length: > 0 } n ? n : L.T("Normal", "Normal", "Normal", "Normal"), n = v.Value, img = Image(g.Key, v.Key) }),
            })
            .ToList();
        return new { matches = matches.Count, skipped = view.Count - matches.Count, total, rows };
    }

    private sealed class Acc
    {
        public int Chests, Destroyed, Treasures, Matches;
        public double Seconds;
        public Dictionary<string, int> Kinds = new();
        public Dictionary<string, int> Variants = new();
        public Dictionary<string, double> Carried = new();
    }
}
