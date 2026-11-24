using System.Text.RegularExpressions;

namespace ChestCounter;

/// <summary>
/// What came out of the player's chests (<see cref="ChestRecord.Items"/>): Fortnite's internal ids
/// ("WID_Assault_AutoHigh_Athena_SR_Ore_T03", "AmmoDataBulletsMedium", "WoodItemData"...) sorted into categories and
/// rarities. Built from the naming patterns of the game: an id that matches none of them falls in "Other".
/// </summary>
public static class Loot
{
    public sealed record Category(string Id, string Fr, string En, string Es, string Pt, string Color, Regex Match)
    {
        public string Name => L.T(Fr, En, Es, Pt);
    }

    public sealed record Rarity(string Id, int Rank, string Fr, string En, string Es, string Pt, string Color)
    {
        public string Name => L.T(Fr, En, Es, Pt);
    }

    private static Regex R(string pattern) => new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // Checked in this order: the first match wins.
    public static readonly Category[] Categories =
    {
        new("ammo", "Munitions", "Ammo", "Munición", "Munição", "#8a8f99", R(@"^AmmoData|Ammo")),
        new("mats", "Matériaux", "Materials", "Materiales", "Materiais", "#9a6b3f", R(@"^(Wood|Stone|Metal)ItemData")),
        new("heal", "Soins et bouclier", "Healing & shields", "Curación y escudo", "Cura e escudo", "#2f9bd6",
            R(@"Shield|Medkit|Bandage|Heal|Chug|Slurp|Flowberry|Apple|Banana|Mushroom|Fish|Potion|Splash")),
        new("explosive", "Explosifs", "Explosives", "Explosivos", "Explosivos", "#d65a2f", R(@"Grenade|Explosive|C4|Rocket|Launcher|RPG|Clinger|Bomb|Throw")),
        new("shotgun", "Fusils à pompe", "Shotguns", "Escopetas", "Escopetas", "#c0392b", R(@"Shotgun|Pump")),
        new("smg", "Mitraillettes", "SMGs", "Subfusiles", "Submetralhadoras", "#8e44ad", R(@"SMG|PDW|Submachine")),
        new("sniper", "Fusils de précision", "Snipers", "Francotiradores", "Fuzis de precisão", "#16a085", R(@"Sniper|DMR|Hunting|Bolt")),
        new("pistol", "Pistolets", "Pistols", "Pistolas", "Pistolas", "#7f8c8d", R(@"Pistol|Revolver|HandCannon|Sidearm")),
        new("rifle", "Fusils d'assaut", "Assault rifles", "Fusiles de asalto", "Fuzis de assalto", "#c9a24f", R(@"Assault|Rifle|_AR_|Minigun|LMG")),
        new("other", "Autres objets", "Other items", "Otros objetos", "Outros itens", "#b0a48a", R(@".")),
    };

    // Fortnite's rarity codes: C common, UC uncommon, R rare, VR epic, SR legendary, UR mythic.
    public static readonly Rarity[] Rarities =
    {
        new("C", 0, "Commun", "Common", "Común", "Comum", "#9d9d9d"),
        new("UC", 1, "Peu commun", "Uncommon", "Poco común", "Incomum", "#4caf3c"),
        new("R", 2, "Rare", "Rare", "Raro", "Raro", "#3c8de0"),
        new("VR", 3, "Épique", "Epic", "Épico", "Épico", "#a14ee0"),
        new("SR", 4, "Légendaire", "Legendary", "Legendario", "Lendário", "#e8921e"),
        new("UR", 5, "Mythique", "Mythic", "Mítico", "Mítico", "#e0c23c"),
    };

    private static readonly Regex RarityCode = R(@"(?:^|_)(UC|VR|SR|UR|C|R)(?:_|$)");
    private static readonly Regex RarityWord = R(@"(Uncommon|Common|Rare|Epic|Legendary|Mythic)");
    private static readonly Regex Noise = R(@"(^WID_|^Athena_?|_Athena|(?:_(?:UC|VR|SR|UR|C|R))(?=_|$)|_Ore|_T\d+$|ItemData$|^AmmoData)");

    public static Category CategoryOf(string item) => Categories.First(c => c.Match.IsMatch(item));

    /// <summary>The rarity written in the id, or null (ammo, materials and most consumables have none).</summary>
    public static Rarity? RarityOf(string item)
    {
        if (RarityCode.Match(item) is { Success: true } m)
        {
            return Rarities.First(r => r.Id.Equals(m.Groups[1].Value, StringComparison.OrdinalIgnoreCase));
        }
        if (RarityWord.Match(item) is { Success: true } w)
        {
            var i = Array.FindIndex(new[] { "common", "uncommon", "rare", "epic", "legendary", "mythic" }, x => x.Equals(w.Value, StringComparison.OrdinalIgnoreCase));
            return i >= 0 ? Rarities[i] : null;
        }
        return null;
    }

    /// <summary>A readable name: "WID_Shotgun_Pump_Athena_R_Ore_T03" → "Shotgun Pump".</summary>
    public static string Pretty(string item)
    {
        // Twice: "WID_Athena_Grenade" only shows its "Athena_" once "WID_" is gone.
        var name = Noise.Replace(Noise.Replace(item, ""), "").Replace("_", " ").Trim();
        name = Regex.Replace(name, @"(?<=[a-z])(?=[A-Z])", " ");
        return name.Length > 0 ? name : item;
    }

    /// <summary>Every item thrown out of the player's chests (treasure hunt items left out: they have their own sections).</summary>
    public static List<string> FromChests(IEnumerable<MatchRecord> matches) =>
        matches.SelectMany(m => m.Chests).SelectMany(c => c.Items.Where(i => !c.TreasureItems.Contains(i))).ToList();
}
