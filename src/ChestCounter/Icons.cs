using System.Text.Json;

namespace ChestCounter;

/// <summary>
/// The app's own icons (no emoji anywhere): line drawings on a 24 × 24 grid, drawn with the text color
/// (<c>currentColor</c>), so they take the color of the theme and of their place. Used by the window, the report and
/// the achievements.
/// </summary>
public static class Icons
{
    private static readonly Dictionary<string, string> Paths = new()
    {
        ["chest"] = "<path d=\"M3 11h18v8a1.5 1.5 0 0 1-1.5 1.5h-15A1.5 1.5 0 0 1 3 19z\"/><path d=\"M3 11c0-4 4-6.5 9-6.5s9 2.5 9 6.5\"/><path d=\"M3 14.5h18\"/><rect x=\"10.2\" y=\"12.5\" width=\"3.6\" height=\"4.2\" rx=\".8\"/>",
        ["key"] = "<circle cx=\"7.5\" cy=\"12\" r=\"4\"/><path d=\"M11.5 12H21M17.5 12v3.2M20.5 12v2.2\"/><circle cx=\"7.5\" cy=\"12\" r=\"1.2\"/>",
        ["coins"] = "<ellipse cx=\"9\" cy=\"6.5\" rx=\"6\" ry=\"2.5\"/><path d=\"M3 6.5v4.5c0 1.4 2.7 2.5 6 2.5M15 6.5V9\"/><ellipse cx=\"15\" cy=\"13\" rx=\"6\" ry=\"2.5\"/><path d=\"M9 13v4.5c0 1.4 2.7 2.5 6 2.5s6-1.1 6-2.5V13\"/>",
        ["crown"] = "<path d=\"M4 18 3 7.5l5 3.5 4-6 4 6 5-3.5L20 18z\"/><path d=\"M4 21h16\"/><circle cx=\"12\" cy=\"13.5\" r=\"1.2\"/>",
        ["flag"] = "<path d=\"M5 21V3\"/><path d=\"M5 4h13.5l-2.5 4.2 2.5 4.3H5\"/><circle cx=\"11.2\" cy=\"7.4\" r=\"1.7\"/><path d=\"m8.6 10.6 5.2-1.4M8.6 9.2l5.2 1.4\"/>",
        ["anchor"] = "<circle cx=\"12\" cy=\"5\" r=\"2\"/><path d=\"M12 7v14M8 10.5h8\"/><path d=\"M4.5 13.5A7.5 7.5 0 0 0 12 21a7.5 7.5 0 0 0 7.5-7.5\"/><path d=\"m3 15 1.5-1.5L6 15M18 15l1.5-1.5L21 15\"/>",
        ["bolt"] = "<path d=\"M13.5 2.5 4.5 14h7l-1 7.5 9-11.5h-7z\"/>",
        ["flame"] = "<path d=\"M12 2.5c.8 3.6 5.5 5.6 5.5 10.6a5.5 5.5 0 0 1-11 0c0-2.8 1.6-4.4 2.7-5.4.3 1.8 1 2.8 2.1 3.3-.2-3.2-.3-5.8.7-8.5z\"/><path d=\"M12 20c-1.6 0-2.5-1.1-2.5-2.5 0-1.6 1.4-2.4 2.5-3.8 1.1 1.4 2.5 2.2 2.5 3.8 0 1.4-.9 2.5-2.5 2.5z\"/>",
        ["burst"] = "<path d=\"m12 2.5 1.8 5.2 5.4-1.9-3.3 4.7 4.8 3-5.6.6.9 5.6L12 15.6l-4 4.1.9-5.6-5.6-.6 4.8-3-3.3-4.7 5.4 1.9z\"/>",
        ["stopwatch"] = "<circle cx=\"12\" cy=\"13.5\" r=\"7.5\"/><path d=\"M12 13.5V9.5M9.5 2.5h5M12 2.5V6M18.3 7.2l1.6-1.6\"/>",
        ["calendar"] = "<rect x=\"3\" y=\"5\" width=\"18\" height=\"16\" rx=\"2\"/><path d=\"M3 10h18M8 3v4M16 3v4\"/><path d=\"M7.5 14h2M11 14h2M14.5 14h2M7.5 17.5h2M11 17.5h2\"/>",
        ["calendar-check"] = "<rect x=\"3\" y=\"5\" width=\"18\" height=\"16\" rx=\"2\"/><path d=\"M3 10h18M8 3v4M16 3v4\"/><path d=\"m8.5 15.5 2.3 2.3 4.7-4.6\"/>",
        ["repeat"] = "<path d=\"m17 2.5 3.5 3.5L17 9.5\"/><path d=\"M3.5 11.5V10A4 4 0 0 1 7.5 6h13\"/><path d=\"M7 21.5 3.5 18 7 14.5\"/><path d=\"M20.5 12.5V14a4 4 0 0 1-4 4h-13\"/>",
        ["route"] = "<circle cx=\"6\" cy=\"18.5\" r=\"2.5\"/><path d=\"M8.5 18.5H15a3.5 3.5 0 0 0 0-7H9a3.5 3.5 0 0 1 0-7h6.5\"/><path d=\"m16 2 2.5 2.5L16 7\"/>",
        ["trophy"] = "<path d=\"M7.5 3.5h9V9a4.5 4.5 0 0 1-9 0z\"/><path d=\"M7.5 5.5H4.5a3 3 0 0 0 3.2 4.4M16.5 5.5h3a3 3 0 0 1-3.2 4.4\"/><path d=\"M12 13.5V17M8 21h8M9.5 17h5v4h-5z\"/>",
        ["target"] = "<circle cx=\"12\" cy=\"12\" r=\"9\"/><circle cx=\"12\" cy=\"12\" r=\"5\"/><circle cx=\"12\" cy=\"12\" r=\"1.4\"/>",
        ["star"] = "<path d=\"m12 3 2.8 5.7 6.2.9-4.5 4.4 1.1 6.2L12 17.3l-5.6 2.9 1.1-6.2L3 9.6l6.2-.9z\"/>",
        ["sparkle"] = "<path d=\"M12 2.5c.6 4.6 2.9 6.9 7.5 7.5-4.6.6-6.9 2.9-7.5 7.5-.6-4.6-2.9-6.9-7.5-7.5 4.6-.6 6.9-2.9 7.5-7.5z\"/><path d=\"M18.5 16.5c.3 1.9 1.1 2.7 3 3-1.9.3-2.7 1.1-3 3-.3-1.9-1.1-2.7-3-3 1.9-.3 2.7-1.1 3-3z\"/>",
        ["gem"] = "<path d=\"M6.5 3.5h11l3.5 5.5-9 11.5L3 9z\"/><path d=\"M3 9h18M9.5 3.5 8 9l4 11.5L16 9l-1.5-5.5\"/>",
        ["hammer"] = "<path d=\"m13.5 9.5-9 9a1.6 1.6 0 0 0 2.3 2.3l9-9\"/><path d=\"M11.5 6.5 15 3l6 6-3.5 3.5z\"/><path d=\"m13 8 3 3\"/>",
        ["controller"] = "<path d=\"M7 7.5h10a5 5 0 0 1 4.9 6l-.6 3a2.6 2.6 0 0 1-4.5 1.2L15 15.5H9l-1.8 2.2a2.6 2.6 0 0 1-4.5-1.2l-.6-3A5 5 0 0 1 7 7.5z\"/><path d=\"M7.5 10.5v3M6 12h3\"/><circle cx=\"15.5\" cy=\"11\" r=\".9\"/><circle cx=\"17.5\" cy=\"13\" r=\".9\"/>",
        ["check"] = "<path d=\"m4.5 12.5 5 5 10-11\"/>",
        ["check-circle"] = "<circle cx=\"12\" cy=\"12\" r=\"9\"/><path d=\"m8 12.3 2.8 2.8 5.2-5.6\"/>",
        ["warning"] = "<path d=\"M10.3 4 2.6 17.5A2 2 0 0 0 4.3 20.5h15.4a2 2 0 0 0 1.7-3L13.7 4a2 2 0 0 0-3.4 0z\"/><path d=\"M12 9.5v4.5M12 17.2v.3\"/>",
        ["error"] = "<circle cx=\"12\" cy=\"12\" r=\"9\"/><path d=\"m9 9 6 6M15 9l-6 6\"/>",
        ["info"] = "<circle cx=\"12\" cy=\"12\" r=\"9\"/><path d=\"M12 11v5.5M12 7.8v.2\"/>",
        ["clock"] = "<circle cx=\"12\" cy=\"12\" r=\"9\"/><path d=\"M12 7v5l3.5 2\"/>",
        ["team"] = "<circle cx=\"9\" cy=\"8\" r=\"3.2\"/><path d=\"M3 20c0-3.5 2.7-6 6-6s6 2.5 6 6\"/><circle cx=\"17\" cy=\"9\" r=\"2.6\"/><path d=\"M16 14c3 0 5 2.2 5 5\"/>",
        ["low"] = "<path d=\"M5 21V4h11l-2 4 2 4H5\"/>",
        ["session"] = "<path d=\"M12 3a9 9 0 1 0 9 9\"/><path d=\"M12 7v5l3 3\"/><path d=\"M16.5 3.5 21 3l-.5 4.5\"/>",
        ["last"] = "<path d=\"M4 4v16\"/><path d=\"M20 6.5 9 12l11 5.5z\"/>",
        ["medal"] = "<path d=\"M8 2.5l2.5 6M16 2.5l-2.5 6\"/><circle cx=\"12\" cy=\"15\" r=\"6.5\"/>",
        ["arrow"] = "<path d=\"M5 12h14M13 6l6 6-6 6\"/>",
        ["avg"] = "<path d=\"M4 18.5 9.5 12l4 4 6.5-8\"/><path d=\"M16 8h4v4\"/>",
        ["home"] = "<path d=\"M3.5 11 12 4l8.5 7\"/><path d=\"M5.5 9.5V20h13V9.5\"/><path d=\"M10 20v-5h4v5\"/>",
        ["report"] = "<path d=\"M4 20V11M10 20V4.5M16 20v-6.5M21.5 20h-19\"/>",
        ["map"] = "<path d=\"M9 4.5 3.5 6.5v13L9 17.5l6 2 5.5-2v-13L15 6.5z\"/><path d=\"M9 4.5v13M15 6.5v13\"/>",
        ["overlay"] = "<rect x=\"3\" y=\"4\" width=\"18\" height=\"13\" rx=\"2\"/><path d=\"M8.5 21h7M12 17v4\"/><path d=\"M14 7.5h4v3.5h-4z\"/>",
        ["gear"] = "<circle cx=\"12\" cy=\"12\" r=\"3\"/><path d=\"M12 2.5v2.6M12 18.9v2.6M4.6 4.6l1.9 1.9M17.5 17.5l1.9 1.9M2.5 12h2.6M18.9 12h2.6M4.6 19.4l1.9-1.9M17.5 6.5l1.9-1.9\"/><circle cx=\"12\" cy=\"12\" r=\"6.5\"/>",
        ["help"] = "<circle cx=\"12\" cy=\"12\" r=\"9\"/><path d=\"M9.5 9.5a2.5 2.5 0 1 1 3.5 2.3c-.6.3-1 .9-1 1.6v.6M12 17v.2\"/>",
        ["play"] = "<path d=\"M7.5 4.5v15l12-7.5z\"/>",
        ["power"] = "<path d=\"M12 3v8\"/><path d=\"M6.5 6.5a8 8 0 1 0 11 0\"/>",
        ["lock"] = "<rect x=\"5\" y=\"10.5\" width=\"14\" height=\"10\" rx=\"2\"/><path d=\"M8 10.5V7.5a4 4 0 0 1 8 0v3\"/>",
        ["eye"] = "<path d=\"M2.5 12S6 5.5 12 5.5 21.5 12 21.5 12 18 18.5 12 18.5 2.5 12 2.5 12z\"/><circle cx=\"12\" cy=\"12\" r=\"3\"/>",
        ["storm"] = "<path d=\"M7 16.5a4 4 0 0 1-.4-8A6 6 0 0 1 18 9a3.8 3.8 0 0 1-.5 7.5\"/><path d=\"m12.5 12-2.5 4h3.5L11 20.5\"/>",
        ["palette"] ="<path d=\"M12 3a9 9 0 1 0 0 18c1.4 0 2-1 1.6-2.1-.5-1.3.4-2.4 1.8-2.4H18a3 3 0 0 0 3-3c0-5.5-4-10.5-9-10.5z\"/><circle cx=\"7.5\" cy=\"11\" r=\"1.2\"/><circle cx=\"10.5\" cy=\"7\" r=\"1.2\"/><circle cx=\"15.5\" cy=\"7.5\" r=\"1.2\"/>",
        ["folder"] = "<path d=\"M3 7.5A1.5 1.5 0 0 1 4.5 6h4.4l2 2.2h8.6A1.5 1.5 0 0 1 21 9.7v8.8a1.5 1.5 0 0 1-1.5 1.5h-15A1.5 1.5 0 0 1 3 18.5z\"/>",
        ["keyboard"] = "<rect x=\"2.5\" y=\"6\" width=\"19\" height=\"12\" rx=\"2\"/><path d=\"M6 9.5h.01M9.5 9.5h.01M13 9.5h.01M16.5 9.5h.01M6 13h.01M18 13h.01M9 15h6\"/>",
        ["chat"] = "<path d=\"M4 5.5h16a1 1 0 0 1 1 1v10a1 1 0 0 1-1 1H9l-4.5 3.5V17.5H4a1 1 0 0 1-1-1v-10a1 1 0 0 1 1-1z\"/><path d=\"M7.5 10h9M7.5 13.5h6\"/>",
        ["bug"] = "<rect x=\"7.5\" y=\"7\" width=\"9\" height=\"13\" rx=\"4.5\"/><path d=\"M9.5 7a2.5 2.5 0 0 1 5 0M12 11v9M3.5 11h4M16.5 11h4M4 17h3.5M16.5 17H20M5 5l2.5 2.5M19 5l-2.5 2.5\"/>",
        ["trash"] = "<path d=\"M4 7h16M9.5 7V4.5h5V7M6 7l1 13h10l1-13\"/><path d=\"M10 11v5.5M14 11v5.5\"/>",
        ["plus"] = "<path d=\"M12 5v14M5 12h14\"/>",
        ["minus"] = "<path d=\"M5 12h14\"/>",
        ["crosshair"] = "<circle cx=\"12\" cy=\"12\" r=\"7\"/><path d=\"M12 2.5v4M12 17.5v4M2.5 12h4M17.5 12h4\"/>",
        ["layers"] = "<path d=\"m12 3 9 5-9 5-9-5z\"/><path d=\"m3 13 9 5 9-5\"/>",
        ["window"] = "<rect x=\"3\" y=\"4.5\" width=\"18\" height=\"15\" rx=\"2\"/><path d=\"M3 9h18M6.5 6.8h.01M9 6.8h.01\"/>",
        ["dots"] = "<circle cx=\"7\" cy=\"8\" r=\"1.6\"/><circle cx=\"15\" cy=\"7\" r=\"1.6\"/><circle cx=\"11\" cy=\"13\" r=\"1.6\"/><circle cx=\"17\" cy=\"15\" r=\"1.6\"/><circle cx=\"7\" cy=\"17\" r=\"1.6\"/>",
        ["refresh"] ="<path d=\"M20 11a8 8 0 0 0-14.6-4.5L4 8\"/><path d=\"M4 4v4h4\"/><path d=\"M4 13a8 8 0 0 0 14.6 4.5L20 16\"/><path d=\"M20 20v-4h-4\"/>",
    };

    /// <summary>An icon as inline SVG ("chest", "trophy"...). <paramref name="fill"/>: filled shape (the star of a treasure).</summary>
    public static string Svg(string name, string cls = "ic", bool fill = false) =>
        $"<svg class=\"{cls}\" viewBox=\"0 0 24 24\" fill=\"{(fill ? "currentColor" : "none")}\" stroke=\"currentColor\" stroke-width=\"1.8\" stroke-linecap=\"round\" stroke-linejoin=\"round\" aria-hidden=\"true\">{Paths.GetValueOrDefault(name, Paths["star"])}</svg>";

    /// <summary>A ranking medal with its number (1, 2, 3...) inside.</summary>
    public static string Medal(int rank, string cls = "ic") =>
        $"<svg class=\"{cls}\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"1.8\" stroke-linecap=\"round\" stroke-linejoin=\"round\" aria-hidden=\"true\">{Paths["medal"]}<text x=\"12\" y=\"18.3\" text-anchor=\"middle\" font-size=\"8.5\" font-weight=\"700\" fill=\"currentColor\" stroke=\"none\" font-family=\"Segoe UI,sans-serif\">{rank}</text></svg>";

    /// <summary>Every icon as a JSON object { name: svg }, for the window's script.</summary>
    public static string Json() => JsonSerializer.Serialize(Paths.Keys.ToDictionary(k => k, k => Svg(k)));
}
