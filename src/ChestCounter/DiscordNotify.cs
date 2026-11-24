using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace ChestCounter;

/// <summary>
/// Optional: posts a summary to a Discord channel through a webhook the player pasted in Settings (off by default).
/// Only what is shown here is sent: the match's figures, treasures, achievements and goals. Nothing else leaves the PC.
/// </summary>
public static class DiscordNotify
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(10) };

    /// <summary>Only Discord's own webhook addresses are accepted.</summary>
    private static readonly Regex WebhookUrl = new(@"^https://(?:(?:ptb|canary)\.)?discord(?:app)?\.com/api/webhooks/\d+/[\w-]+$", RegexOptions.Compiled);

    public static bool IsValid(string? url) => url != null && WebhookUrl.IsMatch(url.Trim());

    /// <summary>"match": every counted match. "treasure": only matches with a treasure, an achievement or a goal reached.</summary>
    public static readonly string[] Modes = { "match", "treasure" };

    private static int Color => Themes.Current.Accent is var (r, g, b) ? (r << 16) | (g << 8) | b : 0xC9A24F;

    /// <summary>A just-counted match. Returns at once: the post is sent in the background (failures are ignored).</summary>
    public static void MatchCounted(Config config, MatchRecord m, IReadOnlyList<MatchRecord> allMatches, int total,
        IReadOnlyList<Achievement> unlocked, IReadOnlyList<Goals.Progress> goalsReached)
    {
        if (!IsValid(config.DiscordWebhook))
        {
            return;
        }
        var notable = m.TreasureFinds.Count > 0 || unlocked.Count > 0 || goalsReached.Count > 0;
        if (config.DiscordMode == "treasure" && !notable)
        {
            return;
        }

        var lines = new List<string>
        {
            $"{Pretty.Playlist(m.Playlist)} · {Pretty.Placement(m)}" + (m.Eliminations is int k ? $" · {L.Count(k, "élimination", "elimination", "eliminación", "eliminação")}" : ""),
        };
        foreach (var f in m.TreasureFinds)
        {
            lines.Add($"★ **{f.DisplayName}**");
        }
        foreach (var a in unlocked)
        {
            lines.Add($"{a.Icon} {L.T("Succès débloqué", "Achievement unlocked", "Logro desbloqueado", "Conquista desbloqueada")} : **{a.Name}** — {a.Description}");
        }
        foreach (var g in goalsReached)
        {
            lines.Add($"🎯 {L.T("Objectif atteint", "Goal reached", "Meta cumplida", "Meta batida")} : **{g.Label}** ({g.Text})");
        }

        var fields = new List<object>
        {
            new { name = L.T("Coffres au total", "Chests in total", "Cofres en total", "Baús no total"), value = L.N(total), inline = true },
            new { name = L.T("Aujourd'hui", "Today", "Hoy", "Hoje"), value = L.N(Goals.ChestsToday(allMatches)), inline = true },
        };
        foreach (var g in Goals.Of(allMatches))
        {
            fields.Add(new { name = g.Label, value = g.Text, inline = true });
        }

        var payload = new
        {
            username = L.T("Compteur de coffres", "Chest Counter", "Contador de cofres", "Contador de Baús"),
            embeds = new[]
            {
                new
                {
                    title = $"📦 {L.Count(m.ChestsOpened, "coffre", "chest", "cofre", "baú")}" + (m.PlayerName is { Length: > 0 } p ? $" · {p}" : ""),
                    description = string.Join("\n", lines),
                    color = Color,
                    fields,
                    timestamp = (m.MatchStartUtc ?? m.AnalyzedAtUtc).ToString("o"),
                },
            },
        };
        _ = Task.Run(async () =>
        {
            try { using var _ = await Http.PostAsJsonAsync(config.DiscordWebhook!.Trim(), payload); }
            catch { /* offline: the next match will try again */ }
        });
    }

    /// <summary>The "Test" button: a short message, and the error to show if it failed (null = sent).</summary>
    public static async Task<string?> TestAsync(string? url)
    {
        if (!IsValid(url))
        {
            return L.T("Adresse de webhook Discord invalide.", "Not a valid Discord webhook address.", "Dirección de webhook de Discord no válida.", "Endereço de webhook do Discord inválido.");
        }
        try
        {
            var payload = new
            {
                username = L.T("Compteur de coffres", "Chest Counter", "Contador de cofres", "Contador de Baús"),
                content = "📦 " + L.T("Le compteur de coffres est connecté à ce salon.", "The chest counter is connected to this channel.",
                    "El contador de cofres está conectado a este canal.", "O contador de baús está conectado a este canal."),
            };
            using var response = await Http.PostAsJsonAsync(url!.Trim(), payload);
            return response.IsSuccessStatusCode ? null : $"Discord: {(int)response.StatusCode} {response.ReasonPhrase}";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
