using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text.Json;

namespace ChestCounter;

/// <summary>
/// The map of the report ("Where you open your chests"): the island's picture and its named places, from the public
/// fortnite-api.com API, downloaded into the data folder (images\map.jpg, images\map-pois-xx.json) and refreshed once a
/// week (the island changes with the seasons). Nothing of it is stored in the repository.
/// </summary>
public static class MapData
{
    private const string ImageUrl = "https://fortnite-api.com/images/map.png";
    private static string PoisUrl(string lang) => $"https://fortnite-api.com/v1/map?language={lang}";

    /// <summary>
    /// World area shown by fortnite-api.com's map picture (centimeters): fitted on the named places of Chapter 7
    /// season 3 (error about 10 m). Can be adjusted in config.json (MapMinX, MapMinY, MapSpan) if a new season moves it.
    /// </summary>
    public static double MinX { get; set; } = -146349;
    public static double MinY { get; set; } = -152558;
    public static double Span { get; set; } = 307797;

    /// <summary>Folder of the local copies (data folder\images), set at startup.</summary>
    public static string? Folder { get; set; }

    public const string ImageName = "map.jpg";
    private static string? ImagePath => Folder == null ? null : Path.Combine(Folder, ImageName);
    private static string ApiLang(string code) => code switch { "pt" => "pt-BR", "es" => "es-419", "fr" => "fr", _ => "en" };
    private static string? PoisPath(string code) => Folder == null ? null : Path.Combine(Folder, $"map-pois-{code}.json");

    public sealed record Poi(string Name, double X, double Y);

    public static bool HasImage => ImagePath is { } p && File.Exists(p);

    /// <summary>True when the picture or the places of this language are missing or older than a week.</summary>
    public static bool Stale(string code) =>
        ImagePath is { } img && PoisPath(code) is { } pois
        && (!File.Exists(img) || !File.Exists(pois) || DateTime.UtcNow - File.GetLastWriteTimeUtc(img) > TimeSpan.FromDays(7));

    /// <summary>Downloads the picture (resized to 1024 px, JPEG) and the named places. True when something changed.</summary>
    public static async Task<bool> RefreshAsync(string code)
    {
        if (ImagePath is not { } imagePath || PoisPath(code) is not { } poisPath)
        {
            return false;
        }
        var changed = false;
        try
        {
            Directory.CreateDirectory(Folder!);
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            http.DefaultRequestHeaders.UserAgent.ParseAdd($"ChestCounter/{AppInfo.VersionText}");

            using (var doc = JsonDocument.Parse(await http.GetStringAsync(PoisUrl(ApiLang(code)))))
            {
                var pois = doc.RootElement.GetProperty("data").GetProperty("pois").EnumerateArray()
                    .Select(p => new Poi(p.GetProperty("name").GetString() ?? "", p.GetProperty("location").GetProperty("x").GetDouble(), p.GetProperty("location").GetProperty("y").GetDouble()))
                    .Where(p => p.Name.Length > 0)
                    .ToList();
                if (pois.Count > 0)
                {
                    await File.WriteAllTextAsync(poisPath, JsonSerializer.Serialize(pois));
                    changed = true;
                }
            }

            if (!File.Exists(imagePath) || DateTime.UtcNow - File.GetLastWriteTimeUtc(imagePath) > TimeSpan.FromDays(7))
            {
                var bytes = await http.GetByteArrayAsync(ImageUrl);
                if (bytes.Length > 8 && bytes[0] == 0x89 && bytes[1] == 'P' && bytes[2] == 'N' && bytes[3] == 'G' && OperatingSystem.IsWindows())
                {
                    SaveSmall(bytes, imagePath);
                    changed = true;
                }
            }
        }
        catch
        {
            // offline or the API is down: the map is shown with what is already there (or as a plain grid)
        }
        return changed;
    }

    /// <summary>The 2048 px PNG (2.4 MB) as a 1024 px JPEG (about 200 KB): the report stays light.</summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static void SaveSmall(byte[] png, string path)
    {
        using var input = new MemoryStream(png);
        using var source = Image.FromStream(input);
        using var small = new Bitmap(1024, 1024);
        using (var g = Graphics.FromImage(small))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(source, 0, 0, 1024, 1024);
        }
        var jpeg = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
        using var options = new EncoderParameters(1);
        options.Param[0] = new EncoderParameter(Encoder.Quality, 85L);
        var tmp = path + ".tmp";
        small.Save(tmp, jpeg, options);
        File.Move(tmp, path, overwrite: true);
    }

    /// <summary>The named places in the language shown (falls back to any language already downloaded).</summary>
    public static List<Poi> Pois(string code)
    {
        foreach (var path in new[] { PoisPath(code), PoisPath("en") }.Concat(Folder != null && Directory.Exists(Folder) ? Directory.GetFiles(Folder, "map-pois-*.json") : []))
        {
            try
            {
                if (path != null && File.Exists(path) && JsonSerializer.Deserialize<List<Poi>>(File.ReadAllText(path)) is { Count: > 0 } list)
                {
                    return list;
                }
            }
            catch
            {
                // damaged file: try the next one
            }
        }
        return [];
    }

    /// <summary>Position on the picture, from 0 to 1 (left → right, top → bottom).</summary>
    public static (double U, double V) Project(double x, double y) => ((x - MinX) / Span, (y - MinY) / Span);

    /// <summary>The named place closest to a point, within 300 m (else null: somewhere between places).</summary>
    public static Poi? Nearest(IReadOnlyList<Poi> pois, double x, double y)
    {
        Poi? best = null;
        var bestD = 30000.0 * 30000.0;
        foreach (var p in pois)
        {
            var d = (p.X - x) * (p.X - x) + (p.Y - y) * (p.Y - y);
            if (d < bestD)
            {
                bestD = d;
                best = p;
            }
        }
        return best;
    }
}
