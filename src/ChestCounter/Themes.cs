namespace ChestCounter;

/// <summary>
/// A color theme (Settings → Theme): colors of the window, the report and the in-game overlay.
/// <see cref="Css"/> overrides the CSS variables of the pages (empty: the original "logbook" look). The cards stay
/// light in every theme, so every text written on them keeps its contrast; only the background, the titles, the
/// accent lines and the big "Chests in total" tile change.
/// </summary>
public sealed record Theme(string Id, string Fr, string En, string Es, string Pt, string Css,
    (byte R, byte G, byte B) Accent, (byte R, byte G, byte B) AccentDeep, (byte R, byte G, byte B) Label, (byte R, byte G, byte B) ClassicTotal)
{
    public string Name => L.T(Fr, En, Es, Pt);
}

public static class Themes
{
    public static readonly Theme[] All =
    {
        new("pirate", "Pirate (original)", "Pirate (original)", "Pirata (original)", "Pirata (original)", "",
            (255, 210, 63), (224, 165, 38), (255, 233, 166), (242, 184, 75)),
        new("noite", "Nuit", "Night", "Noche", "Noite",
            "--bg:#0b1016;--ink:#e7edf4;--ink2:#ffffff;--muted:#8b9bb0;--title:#f2b84b;--gold:#f2b84b;--card:#121a24;--cink:#e7edf4;--cmuted:#8b9bb0;--line:#1f2b38;--track:#223041;--team:#3fbf8f;--hero:#121a24;--heroline:#f2b84b;--heroink:#ffffff;--herotext:#8b9bb0;--herokey:#f2b84b",
            (242, 184, 75), (200, 140, 40), (230, 236, 244), (242, 184, 75)),
        new("arena", "Arène", "Arena", "Arena", "Arena",
            "--bg:#120f33;--ink:#ffffff;--ink2:#ffffff;--muted:#a9a6e0;--title:#ffe14d;--gold:#ffe14d;--card:#1d1946;--cink:#ffffff;--cmuted:#a9a6e0;--line:#2f2a66;--track:#2a2560;--team:#3ff0a0;--hero:#5a35e8;--heroline:#ffe14d;--heroink:#ffffff;--herotext:#e8e4ff;--herokey:#ffe14d",
            (255, 225, 77), (255, 159, 28), (220, 215, 255), (255, 225, 77)),
        new("brasil", "Brésil", "Brazil", "Brasil", "Brasil",
            "--bg:#062b1b;--ink:#eaf4e4;--ink2:#ffffff;--muted:#a8c6b0;--title:#ffdf00;--gold:#f2c500;--card:#f6f8ee;--cink:#0d2617;--cmuted:#4d6a55;--line:#cfdcc2;--track:#e3ecd6;--team:#1f5fbf;--hero:#002776;--heroline:#ffdf00;--heroink:#ffdf00;--herotext:#e6eeff;--herokey:#a9c6ff",
            (255, 223, 0), (0, 156, 59), (205, 245, 215), (255, 223, 0)),
        new("neon", "Néon", "Neon", "Neón", "Neon",
            "--bg:#0e0a1c;--ink:#ece6ff;--ink2:#ffffff;--muted:#a99fc9;--title:#ff5fd8;--gold:#b45cff;--card:#f5f0ff;--cink:#1d1233;--cmuted:#5f5480;--line:#ddd0f5;--track:#ebe2fb;--team:#00a896;--hero:#2b0f52;--heroline:#ff5fd8;--heroink:#5ef1ff;--herotext:#f3e8ff;--herokey:#ff9be9",
            (255, 95, 216), (180, 92, 255), (230, 205, 255), (255, 95, 216)),
        new("gelo", "Glace", "Ice", "Hielo", "Gelo",
            "--bg:#0b1a2b;--ink:#e6f1fb;--ink2:#ffffff;--muted:#9fb6cc;--title:#7fd3ff;--gold:#5bb8f0;--card:#f1f7fc;--cink:#0f2236;--cmuted:#4d6680;--line:#cfe0ee;--track:#e2edf6;--team:#1f7f7a;--hero:#0f3c66;--heroline:#7fd3ff;--heroink:#bff0ff;--herotext:#e6f4ff;--herokey:#9ad9ff",
            (127, 211, 255), (40, 140, 210), (210, 238, 255), (127, 211, 255)),
        new("lava", "Lave", "Lava", "Lava", "Lava",
            "--bg:#1c0b07;--ink:#fbe6dc;--ink2:#ffffff;--muted:#c9a596;--title:#ff7a2f;--gold:#e8541e;--card:#fdf1ea;--cink:#2d120a;--cmuted:#7a4f40;--line:#f0d3c4;--track:#f7e2d6;--team:#1f7f7a;--hero:#6e1a0a;--heroline:#ff8a3d;--heroink:#ffd166;--herotext:#ffe9dc;--herokey:#ffb184",
            (255, 138, 61), (220, 60, 20), (255, 214, 190), (255, 138, 61)),
    };

    /// <summary>The theme in use (config.json "Theme").</summary>
    public static Theme Current { get; private set; } = All[0];

    public static void Set(string? id) => Current = Find(id) ?? All[0];

    public static Theme? Find(string? id) => All.FirstOrDefault(t => t.Id == id);

    /// <summary>
    /// CSS rules of every theme, keyed on <c>&lt;html data-accent="id"&gt;</c>. Placed after the page's own variables:
    /// same specificity as its light-mode rules, so a theme wins over them.
    /// </summary>
    public static string Css => string.Concat(All.Where(t => t.Css.Length > 0).Select(t => $":root[data-accent=\"{t.Id}\"]{{{t.Css}}}\n"));
}
