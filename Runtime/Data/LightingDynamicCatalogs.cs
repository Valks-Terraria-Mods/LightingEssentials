using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.GameContent.Events;
using Terraria.ID;

namespace LightingEssentials;

internal readonly record struct LightingTileCatalogItem(int TileId, string DisplayName);
internal readonly record struct LightingEventCatalogItem(LightingEventId EventId, string DisplayName, Color DefaultColor);
internal readonly record struct LightingBossCatalogItem(LightingBossId BossId, string DisplayName, float DefaultMultiplier, bool UsesProgressiveMultiplier = false);
internal readonly record struct LightingEntityCatalogItem(string Key, string DisplayName, Color SuggestedColor);
internal readonly record struct LightingBossTargetTileGroupCatalogItem(string Key, string DisplayName);

internal sealed record DefaultTileTemplate(string Name, int[] TileIds, Color Color);
internal sealed record BossTargetTileGroupDefinition(string Key, string DisplayName, int[] TileIds);

internal static class LightingDynamicCatalogs
{
    private const string BossTileGroupSurfaceGrowth = "boss-tile-group:surface-growth";
    private const string BossTileGroupCorruptionFlora = "boss-tile-group:corruption-flora";
    private const string BossTileGroupHallowedFlora = "boss-tile-group:hallowed-flora";
    private const string BossTileGroupMushroomFlora = "boss-tile-group:mushroom-flora";
    private const string BossTileGroupHerbFlora = "boss-tile-group:herb-flora";
    private const string BossTileGroupAquaticFlora = "boss-tile-group:aquatic-flora";
    private const string BossTileGroupAshFlora = "boss-tile-group:ash-flora";
    private const string BossTileGroupBambooFlora = "boss-tile-group:bamboo-flora";
    private const string BossTileGroupExoticMoss = "boss-tile-group:exotic-moss";
    private const string BossTileGroupJungleRareFlora = "boss-tile-group:jungle-rare-flora";
    private const string BossTileGroupEvilBiome = "boss-tile-group:evil-biome";
    private const string BossTileGroupJungle = "boss-tile-group:jungle";
    private const string BossTileGroupHardmodeOre = "boss-tile-group:hardmode-ore";
    private const string BossTileGroupUnderworldOre = "boss-tile-group:underworld-ore";
    private const string BossTileGroupLunarOre = "boss-tile-group:lunar-ore";
    private const string BossTileGroupSnow = "boss-tile-group:snow";
    private const string BossTileGroupGem = "boss-tile-group:gem";
    private const string BossTileGroupCrystal = "boss-tile-group:crystal";
    private const string BossTileGroupChlorophyte = "boss-tile-group:chlorophyte";
    private const string BossTileGroupHardmodeProgression = "boss-tile-group:hardmode-progression";
    private const string BossTileTemplateKeyPrefix = "boss-tile-template:";

    private static readonly IReadOnlyList<LightingEventCatalogItem> EventCatalogItems =
    [
        new(LightingEventId.Party, "Party", Rgb(255, 188, 90)),
        new(LightingEventId.LanternNight, "Lantern Night", Rgb(255, 220, 140)),
        new(LightingEventId.Rain, "Rain", Rgb(95, 130, 185)),
        new(LightingEventId.Sandstorm, "Sandstorm", Rgb(188, 156, 96)),
        new(LightingEventId.WindyDay, "Windy Day", Rgb(175, 220, 255)),
        new(LightingEventId.Thunderstorm, "Thunderstorm", Rgb(130, 150, 245)),
        new(LightingEventId.Starfall, "Starfall", Rgb(145, 165, 255)),
        new(LightingEventId.BloodMoon, "Blood Moon", Rgb(255, 35, 35)),
        new(LightingEventId.GoblinArmy, "Goblin Army", Rgb(95, 155, 95)),
        new(LightingEventId.SlimeRain, "Slime Rain", Rgb(145, 190, 255)),
        new(LightingEventId.OldOnesArmy, "Old One's Army", Rgb(170, 130, 255)),
        new(LightingEventId.TorchGod, "Torch God", Rgb(255, 160, 70)),
        new(LightingEventId.FrostLegion, "Frost Legion", Rgb(90, 160, 255)),
        new(LightingEventId.SolarEclipse, "Solar Eclipse", Rgb(255, 130, 35)),
        new(LightingEventId.PirateInvasion, "Pirate Invasion", Rgb(176, 148, 104)),
        new(LightingEventId.PumpkinMoon, "Pumpkin Moon", Rgb(255, 120, 40)),
        new(LightingEventId.FrostMoon, "Frost Moon", Rgb(130, 190, 255)),
        new(LightingEventId.MartianMadness, "Martian Madness", Rgb(125, 255, 180)),
        new(LightingEventId.LunarEvents, "Lunar Events", Rgb(156, 140, 255)),
    ];

    private static readonly IReadOnlyList<LightingBossCatalogItem> BossCatalogItems =
    [
        new(LightingBossId.KingSlime, "King Slime", 1.4f),
        new(LightingBossId.EyeOfCthulhu, "Eye of Cthulhu", 1.4f),
        new(LightingBossId.EaterOfWorlds, "Eater of Worlds", 1.4f),
        new(LightingBossId.BrainOfCthulhu, "Brain of Cthulhu", 1.4f),
        new(LightingBossId.EvilBiomeBoss, "Evil Biome Boss", 1.4f),
        new(LightingBossId.QueenBee, "Queen Bee", 1.4f),
        new(LightingBossId.Skeletron, "Skeletron", 1.4f),
        new(LightingBossId.Deerclops, "Deerclops", 1.4f),
        new(LightingBossId.WallOfFlesh, "Wall of Flesh", 1.4f),
        new(LightingBossId.QueenSlime, "Queen Slime", 1.4f),
        new(LightingBossId.MechBosses, "Mech Bosses", 1.4f, UsesProgressiveMultiplier: true),
        new(LightingBossId.Twins, "The Twins", 1.4f),
        new(LightingBossId.Destroyer, "The Destroyer", 1.4f),
        new(LightingBossId.SkeletronPrime, "Skeletron Prime", 1.4f),
        new(LightingBossId.Plantera, "Plantera", 1.5f),
        new(LightingBossId.Golem, "Golem", 1.4f),
        new(LightingBossId.DukeFishron, "Duke Fishron", 1.4f),
        new(LightingBossId.EmpressOfLight, "Empress of Light", 1.4f),
        new(LightingBossId.LunaticCultist, "Lunatic Cultist", 1.4f),
        new(LightingBossId.MoonLord, "Moon Lord", 1.4f),
        new(LightingBossId.DarkMage, "Dark Mage", 1.4f),
        new(LightingBossId.Ogre, "Ogre", 1.4f),
        new(LightingBossId.Betsy, "Betsy", 1.4f),
        new(LightingBossId.FlyingDutchman, "Flying Dutchman", 1.4f),
        new(LightingBossId.MourningWood, "Mourning Wood", 1.4f),
        new(LightingBossId.Pumpking, "Pumpking", 1.4f),
        new(LightingBossId.Everscream, "Everscream", 1.4f),
        new(LightingBossId.SantaNk1, "Santa-NK1", 1.4f),
        new(LightingBossId.IceQueen, "Ice Queen", 1.4f),
        new(LightingBossId.MartianSaucer, "Martian Saucer", 1.4f),
        new(LightingBossId.SolarPillar, "Solar Pillar", 1.4f),
        new(LightingBossId.NebulaPillar, "Nebula Pillar", 1.4f),
        new(LightingBossId.VortexPillar, "Vortex Pillar", 1.4f),
        new(LightingBossId.StardustPillar, "Stardust Pillar", 1.4f),
    ];

    private static readonly IReadOnlyDictionary<LightingEventId, LightingEventCatalogItem> EventById = EventCatalogItems.ToDictionary(item => item.EventId);
    private static readonly IReadOnlyDictionary<LightingBossId, LightingBossCatalogItem> BossById = BossCatalogItems.ToDictionary(item => item.BossId);
    private static readonly IReadOnlyList<BossTargetTileGroupDefinition> BossTargetTileGroupDefinitions =
    [
        new(BossTileGroupSurfaceGrowth, "Surface Growth", TileLightGroups.SurfaceGrowthTiles),
        new(BossTileGroupCorruptionFlora, "Corruption Flora", TileLightGroups.CorruptionFloraTiles),
        new(BossTileGroupHallowedFlora, "Hallowed Flora", TileLightGroups.HallowedFloraTiles),
        new(BossTileGroupMushroomFlora, "Mushroom Flora", TileLightGroups.MushroomFloraTiles),
        new(BossTileGroupHerbFlora, "Herb Flora", TileLightGroups.HerbFloraTiles),
        new(BossTileGroupAquaticFlora, "Aquatic Flora", TileLightGroups.AquaticFloraTiles),
        new(BossTileGroupAshFlora, "Ash Flora", TileLightGroups.AshFloraTiles),
        new(BossTileGroupBambooFlora, "Bamboo Flora", TileLightGroups.BambooFloraTiles),
        new(BossTileGroupExoticMoss, "Exotic Moss", TileLightGroups.ExoticMossTiles),
        new(BossTileGroupJungleRareFlora, "Jungle Rare Flora", TileLightGroups.JungleRareFloraTiles),
        new(BossTileGroupEvilBiome, "Evil Biome", TileLightGroups.EvilBiomeTiles),
        new(BossTileGroupJungle, "Jungle", TileLightGroups.JungleTiles),
        new(BossTileGroupHardmodeOre, "Hardmode Ore", TileLightGroups.HardmodeOreTiles),
        new(BossTileGroupUnderworldOre, "Underworld Ore", TileLightGroups.UnderworldOreTiles),
        new(BossTileGroupLunarOre, "Lunar Ore", TileLightGroups.LunarOreTiles),
        new(BossTileGroupSnow, "Snow", TileLightGroups.SnowTiles),
        new(BossTileGroupGem, "Gem", TileLightGroups.GemTiles),
        new(BossTileGroupCrystal, "Crystal", TileLightGroups.CrystalTiles),
        new(BossTileGroupChlorophyte, "Chlorophyte", TileLightGroups.ChlorophyteTiles),
        new(BossTileGroupHardmodeProgression, "Hardmode Progression", TileLightGroups.HardmodeProgressionTiles),
    ];
    private static readonly IReadOnlyDictionary<string, BossTargetTileGroupDefinition> BossTargetTileGroupByKey = BossTargetTileGroupDefinitions.ToDictionary(item => item.Key, StringComparer.Ordinal);
    private static readonly IReadOnlyList<LightingBossTargetTileGroupCatalogItem> BossTargetTileGroupCatalogItems =
        [..BossTargetTileGroupDefinitions.Select(static item => new LightingBossTargetTileGroupCatalogItem(item.Key, item.DisplayName))];
    private static IReadOnlyList<LightingBossTargetTileGroupCatalogItem> _bossTargetTileGroupCatalogItemsWithTemplates;
    private static readonly IReadOnlyDictionary<LightingBossId, string[]> BossDefaultTargetTileGroupKeysById = new Dictionary<LightingBossId, string[]>
    {
        [LightingBossId.KingSlime] = [BossTileGroupSurfaceGrowth],
        [LightingBossId.EyeOfCthulhu] = [BossTileGroupSurfaceGrowth, BossTileGroupHerbFlora],
        [LightingBossId.EaterOfWorlds] = [BossTileGroupEvilBiome, BossTileGroupCorruptionFlora],
        [LightingBossId.BrainOfCthulhu] = [BossTileGroupEvilBiome, BossTileGroupCorruptionFlora],
        [LightingBossId.EvilBiomeBoss] = [BossTileGroupEvilBiome, BossTileGroupCorruptionFlora],
        [LightingBossId.QueenBee] = [BossTileGroupJungle, BossTileGroupJungleRareFlora],
        [LightingBossId.Skeletron] = [BossTileGroupUnderworldOre],
        [LightingBossId.Deerclops] = [BossTileGroupSnow],
        [LightingBossId.WallOfFlesh] = [BossTileGroupUnderworldOre, BossTileGroupAshFlora],
        [LightingBossId.QueenSlime] = [BossTileGroupHardmodeProgression, BossTileGroupHallowedFlora, BossTileGroupMushroomFlora],
        [LightingBossId.MechBosses] = [BossTileGroupHardmodeOre],
        [LightingBossId.Twins] = [BossTileGroupHardmodeOre],
        [LightingBossId.Destroyer] = [BossTileGroupHardmodeOre],
        [LightingBossId.SkeletronPrime] = [BossTileGroupHardmodeOre],
        [LightingBossId.Plantera] = [BossTileGroupJungle, BossTileGroupJungleRareFlora, BossTileGroupHerbFlora],
        [LightingBossId.Golem] = [BossTileGroupChlorophyte],
        [LightingBossId.DukeFishron] = [BossTileGroupGem, BossTileGroupAquaticFlora],
        [LightingBossId.EmpressOfLight] = [BossTileGroupCrystal, BossTileGroupHallowedFlora],
        [LightingBossId.LunaticCultist] = [BossTileGroupLunarOre],
        [LightingBossId.MoonLord] = [BossTileGroupLunarOre, BossTileGroupExoticMoss],
        [LightingBossId.DarkMage] = [BossTileGroupSurfaceGrowth],
        [LightingBossId.Ogre] = [BossTileGroupHardmodeOre],
        [LightingBossId.Betsy] = [BossTileGroupHardmodeProgression, BossTileGroupChlorophyte],
        [LightingBossId.FlyingDutchman] = [BossTileGroupAquaticFlora, BossTileGroupGem],
        [LightingBossId.MourningWood] = [BossTileGroupAshFlora, BossTileGroupHerbFlora],
        [LightingBossId.Pumpking] = [BossTileGroupAshFlora, BossTileGroupCrystal],
        [LightingBossId.Everscream] = [BossTileGroupSnow],
        [LightingBossId.SantaNk1] = [BossTileGroupSnow, BossTileGroupHardmodeOre],
        [LightingBossId.IceQueen] = [BossTileGroupSnow, BossTileGroupCrystal],
        [LightingBossId.MartianSaucer] = [BossTileGroupLunarOre, BossTileGroupGem],
        [LightingBossId.SolarPillar] = [BossTileGroupLunarOre],
        [LightingBossId.NebulaPillar] = [BossTileGroupLunarOre],
        [LightingBossId.VortexPillar] = [BossTileGroupLunarOre],
        [LightingBossId.StardustPillar] = [BossTileGroupLunarOre],
    };
    private static readonly IReadOnlyList<LightingEntityCatalogItem> EntityCatalogItems = BuildEntityCatalogItems();
    private static readonly IReadOnlyDictionary<string, LightingEntityCatalogItem> EntityByKey = EntityCatalogItems.ToDictionary(item => item.Key, StringComparer.Ordinal);

    private static readonly IReadOnlyList<DefaultTileTemplate> DefaultTileTemplates =
    [
        new("Grass", [TileID.Grass], Rgb(0, 30, 0)),
        new("Plants", [TileID.Plants, TileID.Plants2], Rgb(0, 30, 0)),
        new("Containers", [TileID.LargePiles, TileID.LargePiles2, TileID.Containers, TileID.Containers2], Rgb(7, 7, 7)),
        new("Pots", [TileID.Pots], Rgb(7, 7, 7)),
        new("Cactus", [TileID.Cactus], Rgb(20, 40, 0)),
        new("Hallowed Flora", TileLightGroups.HallowedFloraTiles, Rgb(30, 16, 40)),
        new("Mushroom Flora", TileLightGroups.MushroomFloraTiles, Rgb(0, 18, 32)),
        new("Herb Flora", TileLightGroups.HerbFloraTiles, Rgb(18, 30, 8)),
        new("Aquatic Flora", TileLightGroups.AquaticFloraTiles, Rgb(0, 24, 22)),
        new("Sunflower Flora", [TileID.Sunflower], Rgb(34, 30, 10)),
        new("Ash Flora", TileLightGroups.AshFloraTiles, Rgb(18, 12, 8)),
        new("Bamboo Flora", TileLightGroups.BambooFloraTiles, Rgb(5, 22, 8)),
        new("Exotic Moss", TileLightGroups.ExoticMossTiles, Rgb(12, 12, 20)),
        new("Jungle Biome", [..TileLightGroups.JungleTiles, ..TileLightGroups.JungleRareFloraTiles], Rgb(0, 20, 0)),
        new("Snow Biome", [TileID.IceBlock], Rgb(0, 0, 20)),
        new("Desert Biome", [TileID.FossilOre], Rgb(20, 20, 0)),
        new("Corruption Biome", [TileID.CorruptGrass, ..TileLightGroups.CorruptionFloraTiles], Rgb(40, 0, 40)),
        new("Crimson Biome", [TileID.CrimsonGrass, TileID.CrimsonJungleGrass, TileID.CrimsonPlants, TileID.CrimsonThorns, TileID.CrimsonVines], Rgb(40, 0, 0)),
        new("Life Crystal", [TileID.Heart], Rgb(255, 0, 0)),
        new("Mana Crystal", [TileID.ManaCrystal], Rgb(0, 0, 255)),
        new("Life Fruit", [TileID.LifeFruit], Rgb(0, 255, 0)),
        new("Red Moss", [TileID.RedMoss], Rgb(10, 0, 0)),
        new("Purple Moss", [TileID.PurpleMoss], Rgb(10, 0, 10)),
        new("Long Moss", [TileID.LongMoss], Rgb(10, 10, 10)),
        new("Lava Moss", [TileID.LavaMoss], Rgb(10, 0, 0)),
        new("Green Moss", [TileID.GreenMoss], Rgb(0, 10, 0)),
        new("Brown Moss", [TileID.BrownMoss], Rgb(10, 10, 10)),
        new("Blue Moss", [TileID.BlueMoss], Rgb(0, 0, 10)),
        new("Lunar Ore", [TileID.LunarOre], Rgb(3, 3, 3)),
        new("Titanium", [TileID.Titanium], Rgb(3, 3, 3)),
        new("Adamantite", [TileID.Adamantite], Rgb(3, 3, 3)),
        new("Orichalcum", [TileID.Orichalcum], Rgb(3, 3, 3)),
        new("Mythril", [TileID.Mythril], Rgb(3, 3, 3)),
        new("Palladium", [TileID.Palladium], Rgb(3, 3, 3)),
        new("Cobalt", [TileID.Cobalt], Rgb(3, 3, 3)),
        new("Hellstone", [TileID.Hellstone], Rgb(50, 0, 0)),
        new("Chlorophyte", [TileID.Chlorophyte], Rgb(0, 3, 0)),
        new("Meteorite", [TileID.Meteorite], Rgb(255, 0, 0)),
        new("Iron", [TileID.Iron], Rgb(3, 3, 3)),
        new("Lead", [TileID.Lead], Rgb(3, 3, 3)),
        new("Copper", [TileID.Copper], Rgb(3, 3, 3)),
        new("Tin", [TileID.Tin], Rgb(3, 3, 3)),
        new("Silver", [TileID.Silver], Rgb(3, 3, 3)),
        new("Gold", [TileID.Gold], Rgb(3, 3, 3)),
        new("Platinum", [TileID.Platinum], Rgb(3, 3, 3)),
        new("Tungsten", [TileID.Tungsten], Rgb(3, 3, 3)),
        new("Amethyst", [TileID.Amethyst], Rgb(3, 3, 3)),
        new("Topaz", [TileID.Topaz], Rgb(3, 3, 3)),
        new("Emerald", [TileID.Emerald], Rgb(3, 3, 3)),
        new("Amber Gemspark", [TileID.AmberGemspark], Rgb(3, 3, 3)),
        new("Diamond", [TileID.Diamond], Rgb(3, 3, 3)),
        new("Ruby", [TileID.Ruby], Rgb(3, 3, 3)),
        new("Sapphire", [TileID.Sapphire], Rgb(3, 3, 3)),
    ];

    private static readonly IReadOnlyList<LightingTileCatalogItem> TileCatalogItems = BuildTileCatalogItems();
    private static readonly IReadOnlyDictionary<int, LightingTileCatalogItem> TileCatalogById = TileCatalogItems.ToDictionary(item => item.TileId);

    private static readonly IReadOnlyDictionary<int, Color> SuggestedTileColors = BuildSuggestedTileColors();

    public static IReadOnlyList<LightingTileCatalogItem> GetTileCatalogItems()
    {
        return TileCatalogItems;
    }

    public static bool TryGetTileCatalogItem(int tileId, out LightingTileCatalogItem item)
    {
        return TileCatalogById.TryGetValue(tileId, out item);
    }

    public static Color GetSuggestedTileColor(int tileId)
    {
        return SuggestedTileColors.TryGetValue(tileId, out Color color)
            ? color
            : Rgb(8, 8, 8);
    }

    public static IReadOnlyList<LightingEventCatalogItem> GetEventCatalogItems()
    {
        return EventCatalogItems;
    }

    public static bool TryGetEventCatalogItem(LightingEventId eventId, out LightingEventCatalogItem item)
    {
        return EventById.TryGetValue(eventId, out item);
    }

    public static IReadOnlyList<LightingBossCatalogItem> GetBossCatalogItems()
    {
        return BossCatalogItems;
    }

    public static bool TryGetBossCatalogItem(LightingBossId bossId, out LightingBossCatalogItem item)
    {
        return BossById.TryGetValue(bossId, out item);
    }

    public static IReadOnlyList<LightingBossTargetTileGroupCatalogItem> GetBossTargetTileGroupCatalogItems()
    {
        if (_bossTargetTileGroupCatalogItemsWithTemplates is not null)
            return _bossTargetTileGroupCatalogItemsWithTemplates;

        List<LightingBossTargetTileGroupCatalogItem> combined = [..BossTargetTileGroupCatalogItems];
        HashSet<string> existingLabels = new(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < combined.Count; i++)
            existingLabels.Add(combined[i].DisplayName);

        for (int i = 0; i < DefaultTileTemplates.Count; i++)
        {
            DefaultTileTemplate template = DefaultTileTemplates[i];
            if (!existingLabels.Add(template.Name))
                continue;

            combined.Add(new LightingBossTargetTileGroupCatalogItem(BuildBossTemplateKey(template.Name), template.Name));
        }

        combined.Sort(static (a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));
        _bossTargetTileGroupCatalogItemsWithTemplates = combined;
        return _bossTargetTileGroupCatalogItemsWithTemplates;
    }

    public static bool TryGetBossTargetTileGroupCatalogItem(string key, out LightingBossTargetTileGroupCatalogItem item)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            item = default;
            return false;
        }

        if (BossTargetTileGroupByKey.TryGetValue(key, out BossTargetTileGroupDefinition definition))
        {
            item = new LightingBossTargetTileGroupCatalogItem(definition.Key, definition.DisplayName);
            return true;
        }

        if (TryGetDefaultTileTemplateFromBossTargetKey(key, out DefaultTileTemplate template))
        {
            item = new LightingBossTargetTileGroupCatalogItem(key, template.Name);
            return true;
        }

        item = default;
        return false;
    }

    public static bool TryGetBossTargetTileIds(string key, out int[] tileIds)
    {
        if (!string.IsNullOrWhiteSpace(key) && BossTargetTileGroupByKey.TryGetValue(key, out BossTargetTileGroupDefinition definition))
        {
            tileIds = definition.TileIds;
            return true;
        }

        if (TryGetDefaultTileTemplateFromBossTargetKey(key, out DefaultTileTemplate template))
        {
            tileIds = template.TileIds;
            return true;
        }

        tileIds = null;
        return false;
    }

    public static List<string> GetDefaultBossTargetTileGroupKeys(LightingBossId bossId)
    {
        if (!BossDefaultTargetTileGroupKeysById.TryGetValue(bossId, out string[] groupKeys))
            return [];

        return [..groupKeys];
    }

    public static List<string> ResolveBossTargetTileGroupKeys(IReadOnlyList<LightingBossId> bossIds, IReadOnlyList<string> configuredKeys)
    {
        HashSet<string> seen = new(StringComparer.Ordinal);
        List<string> resolved = [];

        if (configuredKeys is not null)
        {
            for (int i = 0; i < configuredKeys.Count; i++)
            {
                string key = configuredKeys[i];
                if (!IsBossTargetTileGroupKeyValid(key) || !seen.Add(key))
                    continue;

                resolved.Add(key);
            }
        }

        if (resolved.Count > 0 || bossIds is null || bossIds.Count == 0)
            return resolved;

        for (int i = 0; i < bossIds.Count; i++)
        {
            LightingBossId bossId = bossIds[i];
            if (!BossDefaultTargetTileGroupKeysById.TryGetValue(bossId, out string[] defaultKeys))
                continue;

            for (int j = 0; j < defaultKeys.Length; j++)
            {
                string key = defaultKeys[j];
                if (seen.Add(key))
                    resolved.Add(key);
            }
        }

        return resolved;
    }

    public static IReadOnlyList<LightingEntityCatalogItem> GetEntityCatalogItems()
    {
        return EntityCatalogItems;
    }

    public static bool TryGetEntityCatalogItem(string key, out LightingEntityCatalogItem item)
    {
        return EntityByKey.TryGetValue(key, out item);
    }

    public static Color GetSuggestedEntityColor(string key)
    {
        return EntityByKey.TryGetValue(key, out LightingEntityCatalogItem item)
            ? item.SuggestedColor
            : Rgb(8, 8, 8);
    }

    public static List<LightingTileEffectEntry> CreateDefaultTileEntries()
    {
        List<LightingTileEffectEntry> entries = new(DefaultTileTemplates.Count);

        for (int i = 0; i < DefaultTileTemplates.Count; i++)
        {
            DefaultTileTemplate template = DefaultTileTemplates[i];
            entries.Add(new LightingTileEffectEntry(template.Name, template.TileIds, template.Color, enabled: true));
        }

        return entries;
    }

    public static List<LightingEventEffectEntry> CreateDefaultEventEntries()
    {
        List<LightingEventEffectEntry> entries =
        [
            new LightingEventEffectEntry("Blood Moon", [LightingEventId.BloodMoon], true, Rgb(255, 35, 35)),
            new LightingEventEffectEntry("Solar Eclipse", [LightingEventId.SolarEclipse], true, Rgb(255, 130, 35)),
            new LightingEventEffectEntry("Frost Legion", [LightingEventId.FrostLegion], true, Rgb(90, 160, 255)),
        ];

        return entries;
    }

    public static List<LightingBossEffectEntry> CreateDefaultBossEntries()
    {
        List<LightingBossEffectEntry> entries =
        [
            CreateDefaultBossEntry(LightingBossId.KingSlime),
            CreateDefaultBossEntry(LightingBossId.EyeOfCthulhu),
            CreateDefaultBossEntry(LightingBossId.EvilBiomeBoss),
            CreateDefaultBossEntry(LightingBossId.QueenBee),
            CreateDefaultBossEntry(LightingBossId.Skeletron),
            CreateDefaultBossEntry(LightingBossId.Deerclops),
            CreateDefaultBossEntry(LightingBossId.WallOfFlesh),
            CreateDefaultBossEntry(LightingBossId.QueenSlime),
            CreateDefaultBossEntry(LightingBossId.MechBosses),
            CreateDefaultBossEntry(LightingBossId.Plantera),
            CreateDefaultBossEntry(LightingBossId.Golem),
            CreateDefaultBossEntry(LightingBossId.DukeFishron),
            CreateDefaultBossEntry(LightingBossId.EmpressOfLight),
            CreateDefaultBossEntry(LightingBossId.LunaticCultist),
            CreateDefaultBossEntry(LightingBossId.MoonLord),
        ];

        return entries;
    }

    public static List<LightingEntityEffectEntry> CreateDefaultEntityEntries(LightingSettings settings)
    {
        Color playerColor = settings is null ? Rgb(7, 7, 7) : settings.PlayerLight;
        Color projectileColor = settings is null ? Rgb(10, 10, 10) : settings.ProjectileLightColor;
        Color enemyColor = settings is null ? Rgb(35, 0, 0) : settings.EnemyLightColor;

        bool playerEnabled = settings?.PlayerLightEnabled ?? true;
        bool projectileEnabled = settings?.ProjectileLightEnabled ?? false;
        bool enemyEnabled = settings?.EnemyLightEnabled ?? false;

        List<LightingEntityEffectEntry> entries =
        [
            new LightingEntityEffectEntry(
                "Player Light",
                playerEnabled,
                playerColor,
                includePlayer: true,
                includeAllEnemies: false,
                includeAllProjectiles: false,
                npcIds: null,
                projectileIds: null),

            new LightingEntityEffectEntry(
                "Projectile Light",
                projectileEnabled,
                projectileColor,
                includePlayer: false,
                includeAllEnemies: false,
                includeAllProjectiles: true,
                npcIds: null,
                projectileIds: null),

            new LightingEntityEffectEntry(
                "Enemy Light",
                enemyEnabled,
                enemyColor,
                includePlayer: false,
                includeAllEnemies: true,
                includeAllProjectiles: false,
                npcIds: null,
                projectileIds: null),
        ];

        return entries;
    }

    public static LightingBossEffectEntry CreateDefaultBossEntry(LightingBossId bossId)
    {
        LightingBossCatalogItem item = BossById[bossId];
        return new LightingBossEffectEntry(item.DisplayName, [item.BossId], true, item.DefaultMultiplier, GetDefaultBossTargetTileGroupKeys(item.BossId));
    }

    public static WorldLightingFlags GetEventFlag(LightingEventId eventId)
    {
        return eventId switch
        {
            LightingEventId.Party => WorldLightingFlags.PartyActive,
            LightingEventId.LanternNight => WorldLightingFlags.LanternNightActive,
            LightingEventId.Rain => WorldLightingFlags.RainActive,
            LightingEventId.Sandstorm => WorldLightingFlags.SandstormActive,
            LightingEventId.WindyDay => WorldLightingFlags.WindyDayActive,
            LightingEventId.Thunderstorm => WorldLightingFlags.ThunderstormActive,
            LightingEventId.Starfall => WorldLightingFlags.StarfallActive,
            LightingEventId.BloodMoon => WorldLightingFlags.BloodMoonActive,
            LightingEventId.GoblinArmy => WorldLightingFlags.GoblinArmyActive,
            LightingEventId.SlimeRain => WorldLightingFlags.SlimeRainActive,
            LightingEventId.OldOnesArmy => WorldLightingFlags.OldOnesArmyActive,
            LightingEventId.TorchGod => WorldLightingFlags.TorchGodActive,
            LightingEventId.FrostLegion => WorldLightingFlags.FrostLegionActive,
            LightingEventId.SolarEclipse => WorldLightingFlags.EclipseActive,
            LightingEventId.PirateInvasion => WorldLightingFlags.PirateInvasionActive,
            LightingEventId.PumpkinMoon => WorldLightingFlags.PumpkinMoonActive,
            LightingEventId.FrostMoon => WorldLightingFlags.FrostMoonActive,
            LightingEventId.MartianMadness => WorldLightingFlags.MartianMadnessActive,
            LightingEventId.LunarEvents => WorldLightingFlags.LunarEventsActive,
            _ => WorldLightingFlags.None,
        };
    }

    public static bool IsBossTriggered(LightingBossId bossId, in WorldLightingState state)
    {
        return bossId switch
        {
            LightingBossId.KingSlime => state.Has(WorldLightingFlags.DownedKingSlime),
            LightingBossId.EyeOfCthulhu => state.Has(WorldLightingFlags.DownedEyeOfCthulhu),
            LightingBossId.EaterOfWorlds => state.Has(WorldLightingFlags.DownedEaterOfWorlds),
            LightingBossId.BrainOfCthulhu => state.Has(WorldLightingFlags.DownedBrainOfCthulhu),
            LightingBossId.EvilBiomeBoss => state.Has(WorldLightingFlags.DownedEvilBoss),
            LightingBossId.QueenBee => state.Has(WorldLightingFlags.DownedQueenBee),
            LightingBossId.Skeletron => state.Has(WorldLightingFlags.DownedSkeletron),
            LightingBossId.Deerclops => state.Has(WorldLightingFlags.DownedDeerclops),
            LightingBossId.WallOfFlesh => state.Has(WorldLightingFlags.HardModeUnlocked),
            LightingBossId.QueenSlime => state.Has(WorldLightingFlags.DownedQueenSlime),
            LightingBossId.MechBosses => state.MechBossesDowned > 0,
            LightingBossId.Twins => state.Has(WorldLightingFlags.DownedTwins),
            LightingBossId.Destroyer => state.Has(WorldLightingFlags.DownedDestroyer),
            LightingBossId.SkeletronPrime => state.Has(WorldLightingFlags.DownedSkeletronPrime),
            LightingBossId.Plantera => state.Has(WorldLightingFlags.DownedPlantera),
            LightingBossId.Golem => state.Has(WorldLightingFlags.DownedGolem),
            LightingBossId.DukeFishron => state.Has(WorldLightingFlags.DownedFishron),
            LightingBossId.EmpressOfLight => state.Has(WorldLightingFlags.DownedEmpressOfLight),
            LightingBossId.LunaticCultist => state.Has(WorldLightingFlags.DownedLunaticCultist),
            LightingBossId.MoonLord => state.Has(WorldLightingFlags.DownedMoonLord),
            LightingBossId.DarkMage => state.Has(WorldLightingFlags.DownedDarkMage),
            LightingBossId.Ogre => state.Has(WorldLightingFlags.DownedOgre),
            LightingBossId.Betsy => state.Has(WorldLightingFlags.DownedBetsy),
            LightingBossId.FlyingDutchman => state.Has(WorldLightingFlags.DownedFlyingDutchman),
            LightingBossId.MourningWood => state.Has(WorldLightingFlags.DownedMourningWood),
            LightingBossId.Pumpking => state.Has(WorldLightingFlags.DownedPumpking),
            LightingBossId.Everscream => state.Has(WorldLightingFlags.DownedEverscream),
            LightingBossId.SantaNk1 => state.Has(WorldLightingFlags.DownedSantaNk1),
            LightingBossId.IceQueen => state.Has(WorldLightingFlags.DownedIceQueen),
            LightingBossId.MartianSaucer => state.Has(WorldLightingFlags.DownedMartianSaucer),
            LightingBossId.SolarPillar => state.Has(WorldLightingFlags.DownedSolarPillar),
            LightingBossId.NebulaPillar => state.Has(WorldLightingFlags.DownedNebulaPillar),
            LightingBossId.VortexPillar => state.Has(WorldLightingFlags.DownedVortexPillar),
            LightingBossId.StardustPillar => state.Has(WorldLightingFlags.DownedStardustPillar),
            _ => false,
        };
    }

    public static int[][] GetBossTargetTileGroups(LightingBossId bossId)
    {
        if (!BossDefaultTargetTileGroupKeysById.TryGetValue(bossId, out string[] defaultGroupKeys))
            return [];

        return BuildBossTargetTileGroups(defaultGroupKeys);
    }

    public static bool UsesProgressiveMultiplier(LightingBossId bossId)
    {
        return bossId == LightingBossId.MechBosses;
    }

    private static int[][] BuildBossTargetTileGroups(IReadOnlyList<string> groupKeys)
    {
        if (groupKeys is null || groupKeys.Count == 0)
            return [];

        List<int[]> groups = [];
        for (int i = 0; i < groupKeys.Count; i++)
        {
            if (TryGetBossTargetTileIds(groupKeys[i], out int[] tileIds))
                groups.Add(tileIds);
        }

        return [..groups];
    }

    private static bool IsBossTargetTileGroupKeyValid(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return false;

        return BossTargetTileGroupByKey.ContainsKey(key)
            || TryGetDefaultTileTemplateFromBossTargetKey(key, out _);
    }

    private static bool TryGetDefaultTileTemplateFromBossTargetKey(string key, out DefaultTileTemplate template)
    {
        template = null;

        if (string.IsNullOrWhiteSpace(key) || !key.StartsWith(BossTileTemplateKeyPrefix, StringComparison.Ordinal))
            return false;

        string normalized = key[BossTileTemplateKeyPrefix.Length..];
        for (int i = 0; i < DefaultTileTemplates.Count; i++)
        {
            DefaultTileTemplate candidate = DefaultTileTemplates[i];
            if (!string.Equals(NormalizeBossTargetKeyToken(candidate.Name), normalized, StringComparison.Ordinal))
                continue;

            template = candidate;
            return true;
        }

        return false;
    }

    private static string BuildBossTemplateKey(string templateName)
    {
        return BossTileTemplateKeyPrefix + NormalizeBossTargetKeyToken(templateName);
    }

    private static string NormalizeBossTargetKeyToken(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        StringBuilder builder = new(value.Length);
        bool previousWasSeparator = false;

        string trimmed = value.Trim();
        for (int i = 0; i < trimmed.Length; i++)
        {
            char character = char.ToLowerInvariant(trimmed[i]);
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
                previousWasSeparator = false;
                continue;
            }

            if (builder.Length == 0 || previousWasSeparator)
                continue;

            builder.Append('-');
            previousWasSeparator = true;
        }

        if (builder.Length > 0 && builder[^1] == '-')
            builder.Length--;

        return builder.ToString();
    }

    private static IReadOnlyList<LightingTileCatalogItem> BuildTileCatalogItems()
    {
        List<LightingTileCatalogItem> items = [];

        for (int tileId = 0; tileId < TileID.Count; tileId++)
        {
            string displayName;
            if (TileID.Search.TryGetName(tileId, out string name) && !string.IsNullOrWhiteSpace(name))
            {
                displayName = HumanizeIdentifier(name);
            }
            else
            {
                displayName = $"Tile {tileId}";
            }

            items.Add(new LightingTileCatalogItem(tileId, displayName));
        }

        items.Sort(static (a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));
        return items;
    }

    private static IReadOnlyList<LightingEntityCatalogItem> BuildEntityCatalogItems()
    {
        const string playerKey = "entity:player";
        const string enemiesAllKey = "entity:npc-all";
        const string projectilesAllKey = "entity:projectile-all";

        List<LightingEntityCatalogItem> items =
        [
            new LightingEntityCatalogItem(playerKey, "Player", Rgb(7, 7, 7)),
            new LightingEntityCatalogItem(enemiesAllKey, "Enemies (All)", Rgb(35, 0, 0)),
            new LightingEntityCatalogItem(projectilesAllKey, "Projectiles (All)", Rgb(10, 10, 10)),
        ];

        List<LightingEntityCatalogItem> npcItems = [];
        for (int npcId = 0; npcId < NPCID.Count; npcId++)
        {
            string displayName = NPCID.Search.TryGetName(npcId, out string npcName) && !string.IsNullOrWhiteSpace(npcName)
                ? HumanizeIdentifier(npcName)
                : $"NPC {npcId}";

            npcItems.Add(new LightingEntityCatalogItem($"entity:npc:{npcId}", $"NPC: {displayName} ({npcId})", Rgb(35, 0, 0)));
        }

        List<LightingEntityCatalogItem> projectileItems = [];
        for (int projectileId = 0; projectileId < ProjectileID.Count; projectileId++)
        {
            string displayName = ProjectileID.Search.TryGetName(projectileId, out string projectileName) && !string.IsNullOrWhiteSpace(projectileName)
                ? HumanizeIdentifier(projectileName)
                : $"Projectile {projectileId}";

            projectileItems.Add(new LightingEntityCatalogItem($"entity:projectile:{projectileId}", $"Projectile: {displayName} ({projectileId})", Rgb(10, 10, 10)));
        }

        npcItems.Sort(static (a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));
        projectileItems.Sort(static (a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));

        items.AddRange(npcItems);
        items.AddRange(projectileItems);
        return items;
    }

    private static IReadOnlyDictionary<int, Color> BuildSuggestedTileColors()
    {
        Dictionary<int, Color> map = [];

        for (int i = 0; i < DefaultTileTemplates.Count; i++)
        {
            DefaultTileTemplate template = DefaultTileTemplates[i];
            for (int j = 0; j < template.TileIds.Length; j++)
            {
                int tileId = template.TileIds[j];
                if (map.ContainsKey(tileId))
                    continue;

                map[tileId] = template.Color;
            }
        }

        return map;
    }

    private static string HumanizeIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return identifier;

        StringBuilder builder = new(identifier.Length + 8);
        char previous = '\0';

        for (int i = 0; i < identifier.Length; i++)
        {
            char current = identifier[i];

            if (current == '_')
            {
                if (builder.Length > 0 && builder[^1] != ' ')
                    builder.Append(' ');

                previous = current;
                continue;
            }

            bool insertSpace =
                builder.Length > 0
                && previous != '_'
                && ((char.IsLower(previous) && char.IsUpper(current))
                    || (char.IsLetter(previous) && char.IsDigit(current))
                    || (char.IsDigit(previous) && char.IsLetter(current)));

            if (insertSpace)
                builder.Append(' ');

            builder.Append(current);
            previous = current;
        }

        return builder.ToString();
    }

    private static Color Rgb(byte r, byte g, byte b)
    {
        return new Color(r, g, b);
    }
}
