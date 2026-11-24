using System.Text.RegularExpressions;

namespace ChestCounter;

/// <summary>
/// What came out of the player's chests (<see cref="ChestRecord.Items"/>). The replays only carry Fortnite's internal
/// ids, and those use code names ("WID_Assault_Chrono_Pan_Rifle_Athena_R" is the Flapjack Rifle), so the names come
/// from a table of the season's items (<see cref="Known"/>), checked against the "Weapon ID" / "Item ID" sections of
/// the Fortnite wiki (fortnite.weirdgloop.org). An id missing from the table is shown as "unidentified", never with a
/// made-up name. Names are the game's English names.
/// </summary>
public static class Loot
{
    public sealed record Category(string Id, string Fr, string En, string Es, string Pt, string Color)
    {
        public string Name => L.T(Fr, En, Es, Pt);
    }

    public sealed record Rarity(string Id, int Rank, string Fr, string En, string Es, string Pt, string Color)
    {
        public string Name => L.T(Fr, En, Es, Pt);
    }

    /// <summary>An item of the table: the pattern of its id, its name in the game and its category.</summary>
    private sealed record Item(Regex Id, string Name, string Category);

    public static readonly Category[] Categories =
    {
        new("shotgun", "Fusils à pompe", "Shotguns", "Escopetas", "Escopetas", "#c0392b"),
        new("rifle", "Fusils d'assaut", "Assault rifles", "Fusiles de asalto", "Fuzis de assalto", "#c9a24f"),
        new("smg", "Mitraillettes", "SMGs", "Subfusiles", "Submetralhadoras", "#8e44ad"),
        new("pistol", "Pistolets", "Pistols", "Pistolas", "Pistolas", "#7f8c8d"),
        new("sniper", "Fusils de précision", "Snipers", "Francotiradores", "Fuzis de precisão", "#16a085"),
        new("explosive", "Explosifs", "Explosives", "Explosivos", "Explosivos", "#d65a2f"),
        new("melee", "Corps à corps", "Melee", "Cuerpo a cuerpo", "Corpo a corpo", "#9b5f2f"),
        new("heal", "Soins et bouclier", "Healing & shields", "Curación y escudo", "Cura e escudo", "#2f9bd6"),
        new("utility", "Utilitaires", "Utility", "Utilidades", "Utilitários", "#3aa57a"),
        new("ammo", "Munitions", "Ammo", "Munición", "Munição", "#8a8f99"),
        new("mats", "Matériaux", "Materials", "Materiales", "Materiais", "#9a6b3f"),
        new("other", "Non identifiés", "Unidentified", "Sin identificar", "Não identificados", "#b0a48a"),
    };

    private static Regex R(string pattern) => new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // The season's items (Treasure Hunters, v43). Checked in this order: the first match wins, so a more precise id
    // comes before a shorter one ("Apple_X" before "Apple", "PurpleDrool_..._M" before "PurpleDrool").
    private static readonly Item[] Known =
    {
        // Weapons
        new(R(@"Pistol_Scavenger"), "Tactical Submachine Gun", "smg"),
        new(R(@"Pistol_Machine"), "Machine SMG", "smg"),
        new(R(@"SMG_SnoringDweller"), "Lil Uzi Vert's Lil' SMG", "smg"),
        new(R(@"Pistol_CorePistol"), "Sidearm Pistol", "pistol"),
        new(R(@"Pistol_Flintlock"), "Flint-Knock Pistol", "pistol"),
        new(R(@"Slingshot_HarmlessThrift"), "Usopp's Slingshot", "pistol"),
        new(R(@"Shotgun_Break_Flintlock"), "Gilded Dragoon Shotgun", "shotgun"),
        new(R(@"Shotgun_Pump_FirePetal"), "Sentinel Pump Shotgun", "shotgun"),
        new(R(@"Shotgun_Swing"), "Lever Action Shotgun", "shotgun"),
        new(R(@"Shotgun_CoreDPS"), "Auto Shotgun", "shotgun"),
        new(R(@"Shotgun_Auto_Paprika"), "Frenzy Auto Shotgun", "shotgun"),
        new(R(@"Shotgun_Standard"), "Pump Shotgun", "shotgun"),
        new(R(@"Assault_Flintlock"), "Bundlebuss Repeater Rifle", "rifle"),
        new(R(@"Assault_Heavy_Recoil"), "Hammer Assault Rifle", "rifle"),
        new(R(@"Assault_Chrono_Pan_Rifle"), "Flapjack Rifle", "rifle"),
        new(R(@"WharfShaft"), "A$AP Rocky's Shock Rocket", "explosive"),
        new(R(@"GraftedCrunchy"), "Zoro's Three Sword Style", "melee"),
        // Healing and shields
        new(R(@"Paprika_ShieldSmall"), "Small Shield Potion", "heal"),
        new(R(@"Paprika_ShieldPot"), "Shield Potion", "heal"),
        new(R(@"Paprika_MedBox"), "Med Kit", "heal"),
        new(R(@"Paprika_Bandage"), "Bandage", "heal"),
        new(R(@"PurpleStuff"), "Slurp Juice", "heal"),
        new(R(@"ShreddingJune"), "Nami's Tangerine", "heal"),
        new(R(@"MeatOnStick"), "Meat on a Stick", "heal"),
        new(R(@"Athena_Apple_X"), "Golden Apple", "heal"),
        new(R(@"Athena_Apple"), "Apple", "heal"),
        new(R(@"Pepper|Foraged_HealthSpeedSmall"), "Pepper", "heal"),
        new(R(@"Athena_Cabbage"), "Cabbage", "heal"),
        new(R(@"ChillBronco"), "Chug Splash", "heal"),
        new(R(@"SuperMedkit"), "Chug Jug", "heal"),
        new(R(@"Flopper_HopFlopper"), "Hop Flopper", "heal"),
        new(R(@"Flopper_Effective"), "Slurpfish", "heal"),
        new(R(@"Athena_Flopper"), "Flopper", "heal"),
        // Explosives and utility
        new(R(@"PurpleDrool_Athena_M"), "Special Buggy Ball", "explosive"),
        new(R(@"PurpleDrool"), "Explosive Cannon Ball", "explosive"),
        new(R(@"Stimpulse"), "Overdrive Grenade", "utility"),
        new(R(@"ZiplineDeployer"), "Skyline Deployer", "utility"),
        new(R(@"SilverBlazer_Mini"), "Shield Bubble Jr.", "utility"),
        new(R(@"SilverBlazer"), "Shield Bubble", "utility"),
        new(R(@"TowerGrenade"), "Port-A-Fort", "utility"),
        new(R(@"ZippyTrout"), "Zapper Trap", "utility"),
        new(R(@"Athena_Bush"), "Bush", "utility"),
        new(R(@"Delilah_Keycard"), "Vault Key", "utility"),
        // Ammo and materials
        new(R(@"AmmoDataBulletsLight"), "Light Bullets", "ammo"),
        new(R(@"AmmoDataBulletsMedium"), "Medium Bullets", "ammo"),
        new(R(@"AmmoDataBulletsHeavy"), "Heavy Bullets", "ammo"),
        new(R(@"AmmoDataShells"), "Shells", "ammo"),
        new(R(@"AmmoDataRockets"), "Rockets", "ammo"),
        new(R(@"^WoodItemData"), "Wood", "mats"),
        new(R(@"^StoneItemData"), "Stone", "mats"),
        new(R(@"^MetalItemData"), "Metal", "mats"),
    };

    // Ids that are not in the table: their kind, from the words of the id, when it is plain enough.
    private static readonly (Regex Id, string Category)[] Fallback =
    {
        (R(@"Ammo"), "ammo"),
        (R(@"^(Wood|Stone|Metal)ItemData"), "mats"),
        (R(@"Shotgun"), "shotgun"),
        (R(@"_SMG_|SMG"), "smg"),
        (R(@"Sniper|DMR"), "sniper"),
        (R(@"Assault|_AR_"), "rifle"),
        (R(@"Pistol"), "pistol"),
        (R(@"Launcher|Rocket|Grenade"), "explosive"),
        (R(@"Shield|Medkit|Bandage|Heal|Chug|Slurp"), "heal"),
    };

    // Fortnite's rarity codes: C common, UC uncommon, R rare, VR epic, SR legendary, UR / M mythic, X exotic.
    public static readonly Rarity[] Rarities =
    {
        new("C", 0, "Commun", "Common", "Común", "Comum", "#9d9d9d"),
        new("UC", 1, "Peu commun", "Uncommon", "Poco común", "Incomum", "#4caf3c"),
        new("R", 2, "Rare", "Rare", "Raro", "Raro", "#3c8de0"),
        new("VR", 3, "Épique", "Epic", "Épico", "Épico", "#a14ee0"),
        new("SR", 4, "Légendaire", "Legendary", "Legendario", "Lendário", "#e8921e"),
        new("UR", 5, "Mythique", "Mythic", "Mítico", "Mítico", "#e0c23c"),
        new("X", 6, "Exotique", "Exotic", "Exótico", "Exótico", "#2fc4c0"),
    };

    private static readonly Regex RarityCode = R(@"_(UC|VR|SR|UR|C|R|X|M)(?=_|$)");

    private static Item? Find(string item) => Known.FirstOrDefault(k => k.Id.IsMatch(item));

    public static Category CategoryOf(string item)
    {
        var id = Find(item)?.Category ?? Fallback.FirstOrDefault(f => f.Id.IsMatch(item)).Category ?? "other";
        return Categories.First(c => c.Id == id);
    }

    /// <summary>The rarity written in the id, or null (ammo, materials and most consumables have none).</summary>
    public static Rarity? RarityOf(string item)
    {
        if (RarityCode.Match(item) is not { Success: true } m)
        {
            return null;
        }
        var code = m.Groups[1].Value.ToUpperInvariant() == "M" ? "UR" : m.Groups[1].Value.ToUpperInvariant();
        return Rarities.First(r => r.Id == code);
    }

    /// <summary>The name in the game, or null when the id is not in the table.</summary>
    public static string? NameOf(string item) => Find(item)?.Name;

    /// <summary>The name to show: the game's name, else "Unidentified" (the id is kept for the tooltip).</summary>
    public static string Pretty(string item) => NameOf(item) ?? L.T("Objet non identifié", "Unidentified item", "Objeto sin identificar", "Item não identificado");

    /// <summary>Every item thrown out of the player's chests (treasure hunt items left out: they have their own sections).</summary>
    public static List<string> FromChests(IEnumerable<MatchRecord> matches) =>
        matches.SelectMany(m => m.Chests).SelectMany(c => c.Items.Where(i => !c.TreasureItems.Contains(i))).ToList();
}
