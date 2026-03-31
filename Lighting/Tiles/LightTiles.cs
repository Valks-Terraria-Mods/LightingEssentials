using System;

namespace LightingEssentials;

public class LightTiles : GlobalTile
{
    private const float MinBossMultiplier = 1f;
    private const float MaxBossMultiplier = 4f;
    private const float EventTintBlend = 0.82f;
    private const float MinimumTintIntensity = 0.12f;

    private static readonly int[] SurfaceGrowthTiles =
    [
        TileID.Grass,
        TileID.Plants,
        TileID.Plants2,
        TileID.Cactus,
        TileID.Sunflower,
    ];

    private static readonly int[] CorruptionFloraTiles =
    [
        TileID.CorruptPlants,
        TileID.CorruptThorns,
        TileID.CorruptVines,
    ];

    private static readonly int[] HallowedFloraTiles =
    [
        TileID.HallowedGrass,
        TileID.HallowedPlants,
        TileID.HallowedPlants2,
        TileID.HallowedVines,
    ];

    private static readonly int[] MushroomFloraTiles =
    [
        TileID.MushroomGrass,
        TileID.MushroomPlants,
        TileID.MushroomVines,
        TileID.MushroomTrees,
        TileID.MushroomBlock,
    ];

    private static readonly int[] HerbFloraTiles =
    [
        TileID.ImmatureHerbs,
        TileID.MatureHerbs,
        TileID.BloomingHerbs,
        TileID.DyePlants,
    ];

    private static readonly int[] AquaticFloraTiles =
    [
        TileID.Coral,
        TileID.Seaweed,
        TileID.SeaOats,
        TileID.OasisPlants,
        TileID.LilyPad,
    ];

    private static readonly int[] AshFloraTiles =
    [
        TileID.AshGrass,
        TileID.AshPlants,
        TileID.AshVines,
    ];

    private static readonly int[] BambooFloraTiles =
    [
        TileID.Bamboo,
        TileID.BambooBlock,
        TileID.LargeBambooBlock,
    ];

    private static readonly int[] ExoticMossTiles =
    [
        TileID.KryptonMoss,
        TileID.XenonMoss,
        TileID.ArgonMoss,
        TileID.VioletMoss,
        TileID.RainbowMoss,
        TileID.KryptonMossBlock,
        TileID.XenonMossBlock,
        TileID.ArgonMossBlock,
        TileID.VioletMossBlock,
        TileID.RainbowMossBlock,
    ];

    private static readonly int[] JungleRareFloraTiles =
    [
        TileID.PlanteraBulb,
        TileID.PlantDetritus,
        TileID.PlantDetritus3x2Echo,
        TileID.PlantDetritus2x2Echo,
        TileID.VineFlowers,
    ];

    private static readonly int[] EvilBiomeTiles =
    [
        TileID.CrimsonGrass,
        TileID.CrimsonJungleGrass,
        TileID.CrimsonPlants,
        TileID.CrimsonThorns,
        TileID.CrimsonVines,
        TileID.CorruptGrass,
    ];

    private static readonly int[] JungleTiles =
    [
        TileID.JungleGrass,
        TileID.JunglePlants,
        TileID.JunglePlants2,
        TileID.JungleThorns,
        TileID.JungleVines,
    ];

    private static readonly int[] HardmodeOreTiles =
    [
        TileID.Cobalt,
        TileID.Palladium,
        TileID.Mythril,
        TileID.Orichalcum,
        TileID.Adamantite,
        TileID.Titanium,
    ];

    private static readonly int[] UnderworldOreTiles =
    [
        TileID.Meteorite,
        TileID.Hellstone,
    ];

    private static readonly int[] LunarOreTiles =
    [
        TileID.LunarOre,
    ];

    private static readonly int[] SnowTiles =
    [
        TileID.IceBlock,
        TileID.BlueMoss,
    ];

    private static readonly int[] GemTiles =
    [
        TileID.Sapphire,
        TileID.Ruby,
        TileID.Diamond,
        TileID.AmberGemspark,
        TileID.Emerald,
        TileID.Topaz,
        TileID.Amethyst,
    ];

    private static readonly int[] CrystalTiles =
    [
        TileID.Heart,
        TileID.ManaCrystal,
        TileID.LifeFruit,
    ];

    private static readonly int[] ChlorophyteTiles =
    [
        TileID.Chlorophyte,
    ];

    private static readonly int[] HardmodeProgressionTiles =
    [
        TileID.Cobalt,
        TileID.Palladium,
        TileID.Mythril,
        TileID.Orichalcum,
        TileID.Adamantite,
        TileID.Titanium,
        TileID.Chlorophyte,
    ];

    private static Vector3[] _tileLight = [];
    private static bool[] _hasTileLight = [];

    public override void SetStaticDefaults()
    {
        InitLight();
    }

    public static void InitLight()
    {
        Config c = LightingEssentials.Config;
        if (c is null)
            return;

        EnsureCapacity(TileLoader.TileCount);
        Array.Clear(_hasTileLight, 0, _hasTileLight.Length);

        if (!LightRuntime.ModEnabled)
            return;

        WorldLightingState state = LightWorldSystem.GetCurrentState();

        // Ores
        SetOre(TileID.Sapphire, c.Sapphire);
        SetOre(TileID.Ruby, c.Ruby);
        SetOre(TileID.Diamond, c.Diamond);
        SetOre(TileID.AmberGemspark, c.AmberGemspark);
        SetOre(TileID.Emerald, c.Emerald);
        SetOre(TileID.Topaz, c.Topaz);
        SetOre(TileID.Amethyst, c.Amethyst);
        SetOre(TileID.Iron, c.Iron);
        SetOre(TileID.Lead, c.Lead);
        SetOre(TileID.Copper, c.Copper);
        SetOre(TileID.Tin, c.Tin);
        SetOre(TileID.Silver, c.Silver);
        SetOre(TileID.Gold, c.Gold);
        SetOre(TileID.Platinum, c.Platinum);
        SetOre(TileID.Tungsten, c.Tungsten);
        SetOre(TileID.Meteorite, c.Meteorite);
        SetOre(TileID.Chlorophyte, c.Chlorophyte);
        SetOre(TileID.Hellstone, c.Hellstone);
        SetOre(TileID.Cobalt, c.Cobalt);
        SetOre(TileID.Palladium, c.Palladium);
        SetOre(TileID.Mythril, c.Mythril);
        SetOre(TileID.Orichalcum, c.Orichalcum);
        SetOre(TileID.Adamantite, c.Adamantite);
        SetOre(TileID.Titanium, c.Titanium);
        SetOre(TileID.LunarOre, c.LunarOre);

        // Environment
        SetEnv(TileID.Grass, c.Grass);
        SetEnv(TileID.CrimsonGrass, c.CrimsonBiome);
        SetEnv(TileID.CrimsonJungleGrass, c.CrimsonBiome);
        SetEnv(TileID.CrimsonPlants, c.CrimsonBiome);
        SetEnv(TileID.CrimsonThorns, c.CrimsonBiome);
        SetEnv(TileID.CrimsonVines, c.CrimsonBiome);
        SetEnv(TileID.CorruptGrass, c.CorruptionBiome);
        SetEnvTiles(CorruptionFloraTiles, c.CorruptionBiome);
        SetEnvTiles(HallowedFloraTiles, c.HallowedFlora);
        SetEnvTiles(MushroomFloraTiles, c.MushroomFlora);
        SetEnvTiles(HerbFloraTiles, c.HerbFlora);
        SetEnvTiles(AquaticFloraTiles, c.AquaticFlora);
        SetEnvTiles(AshFloraTiles, c.AshFlora);
        SetEnvTiles(BambooFloraTiles, c.BambooFlora);
        SetEnvTiles(ExoticMossTiles, c.ExoticMoss);
        SetEnvTiles(JungleRareFloraTiles, c.JungleBiome);
        SetEnv(TileID.Sunflower, c.SunflowerFlora);
        SetEnv(TileID.Pots, c.Pots);
        SetEnv(TileID.FossilOre, c.DesertBiome);
        SetEnv(TileID.IceBlock, c.SnowBiome);
        SetEnv(TileID.BlueMoss, c.BlueMoss);
        SetEnv(TileID.BrownMoss, c.BrownMoss);
        SetEnv(TileID.GreenMoss, c.GreenMoss);
        SetEnv(TileID.LavaMoss, c.LavaMoss);
        SetEnv(TileID.LongMoss, c.LongMoss);
        SetEnv(TileID.PurpleMoss, c.PurpleMoss);
        SetEnv(TileID.RedMoss, c.RedMoss);
        SetEnv(TileID.LifeFruit, c.LifeFruit);
        SetEnv(TileID.Heart, c.LifeCrystal);
        SetEnv(TileID.ManaCrystal, c.ManaCrystal);
        SetEnv(TileID.JungleGrass, c.JungleBiome);
        SetEnv(TileID.JunglePlants, c.JungleBiome);
        SetEnv(TileID.JunglePlants2, c.JungleBiome);
        SetEnv(TileID.JungleThorns, c.JungleBiome);
        SetEnv(TileID.JungleVines, c.JungleBiome);
        SetEnv(TileID.LargePiles, c.Containers);
        SetEnv(TileID.LargePiles2, c.Containers);
        SetEnv(TileID.Containers, c.Containers);
        SetEnv(TileID.Containers2, c.Containers);
        SetEnv(TileID.Plants, c.Plants);
        SetEnv(TileID.Plants2, c.Plants);
        SetEnv(TileID.Cactus, c.Cactus);
        ApplyBossEffects(c, state);
        ApplyEventEffects(c, state);

        return;

        static void SetOre(int tileId, Color color)
        {
            SetTileLight(tileId, color.ToVector3());
        }

        static void SetEnv(int tileId, Color color)
        {
            SetTileLight(tileId, color.ToVector3());
        }

        static void SetEnvTiles(int[] tileIds, Color color)
        {
            Vector3 colorVector = color.ToVector3();
            for (int i = 0; i < tileIds.Length; i++)
            {
                SetTileLight(tileIds[i], colorVector);
            }
        }
    }

    private static void EnsureCapacity(int tileCount)
    {
        if (_tileLight.Length == tileCount)
            return;

        _tileLight = new Vector3[tileCount];
        _hasTileLight = new bool[tileCount];
    }

    private static void SetTileLight(int tileId, Vector3 color)
    {
        if ((uint)tileId >= (uint)_tileLight.Length)
            return;

        _tileLight[tileId] = LightRuntime.ClampColor(color);
        _hasTileLight[tileId] = true;

        Main.tileLighted[tileId] = true;
        Main.tileShine[tileId] = 1_000_000_000;
        Main.tileShine2[tileId] = false;
    }

    private static void ApplyBossEffects(Config config, WorldLightingState state)
    {
        if (config.BossKingSlimeEffects && state.DownedKingSlime)
        {
            BrightenTiles(SurfaceGrowthTiles, ClampBossMultiplier(config.BossKingSlimeEffectsMultiplier));
        }

        if (config.BossEyeofCthulhuEffects && state.DownedEyeOfCthulhu)
        {
            BrightenTiles(SurfaceGrowthTiles, ClampBossMultiplier(config.BossEyeofCthulhuEffectsMultiplier));
            BrightenTiles(HerbFloraTiles, ClampBossMultiplier(config.BossEyeofCthulhuEffectsMultiplier));
        }

        if (config.BossEvilBiomeEffects && state.DownedEvilBoss)
        {
            BrightenTiles(EvilBiomeTiles, ClampBossMultiplier(config.BossEvilBiomeEffectsMultiplier));
            BrightenTiles(CorruptionFloraTiles, ClampBossMultiplier(config.BossEvilBiomeEffectsMultiplier));
        }

        if (config.BossQueenBeeEffects && state.DownedQueenBee)
        {
            BrightenTiles(JungleTiles, ClampBossMultiplier(config.BossQueenBeeEffectsMultiplier));
            BrightenTiles(JungleRareFloraTiles, ClampBossMultiplier(config.BossQueenBeeEffectsMultiplier));
        }

        if (config.BossSkeletronEffects && state.DownedSkeletron)
        {
            BrightenTiles(UnderworldOreTiles, ClampBossMultiplier(config.BossSkeletronEffectsMultiplier));
        }

        if (config.BossDeerclopsEffects && state.DownedDeerclops)
        {
            BrightenTiles(SnowTiles, ClampBossMultiplier(config.BossDeerclopsEffectsMultiplier));
        }

        if (config.BossWallOfFleshEffects && state.HardModeUnlocked)
        {
            BrightenTiles(UnderworldOreTiles, ClampBossMultiplier(config.BossWallOfFleshEffectsMultiplier));
            BrightenTiles(AshFloraTiles, ClampBossMultiplier(config.BossWallOfFleshEffectsMultiplier));
        }

        if (config.BossQueenSlimeEffects && state.DownedQueenSlime)
        {
            BrightenTiles(HardmodeProgressionTiles, ClampBossMultiplier(config.BossQueenSlimeEffectsMultiplier));
            BrightenTiles(HallowedFloraTiles, ClampBossMultiplier(config.BossQueenSlimeEffectsMultiplier));
            BrightenTiles(MushroomFloraTiles, ClampBossMultiplier(config.BossQueenSlimeEffectsMultiplier));
        }

        if (config.BossMechEffects && state.MechBossesDowned > 0)
        {
            float mechMaxMultiplier = ClampBossMultiplier(config.BossMechEffectsMultiplier);
            float mechProgress = state.MechBossesDowned / 3f;
            float mechMultiplier = 1f + ((mechMaxMultiplier - 1f) * mechProgress);
            BrightenTiles(HardmodeOreTiles, mechMultiplier);
        }

        if (config.BossPlanteraEffects && state.DownedPlantera)
        {
            BrightenTiles(JungleTiles, ClampBossMultiplier(config.BossPlanteraEffectsMultiplier));
            BrightenTiles(JungleRareFloraTiles, ClampBossMultiplier(config.BossPlanteraEffectsMultiplier));
            BrightenTiles(HerbFloraTiles, ClampBossMultiplier(config.BossPlanteraEffectsMultiplier));
        }

        if (config.BossGolemEffects && state.DownedGolem)
        {
            BrightenTiles(ChlorophyteTiles, ClampBossMultiplier(config.BossGolemEffectsMultiplier));
        }

        if (config.BossDukeFishronEffects && state.DownedFishron)
        {
            BrightenTiles(GemTiles, ClampBossMultiplier(config.BossDukeFishronEffectsMultiplier));
            BrightenTiles(AquaticFloraTiles, ClampBossMultiplier(config.BossDukeFishronEffectsMultiplier));
        }

        if (config.BossEmpressOfLightEffects && state.DownedEmpressOfLight)
        {
            BrightenTiles(CrystalTiles, ClampBossMultiplier(config.BossEmpressOfLightEffectsMultiplier));
            BrightenTiles(HallowedFloraTiles, ClampBossMultiplier(config.BossEmpressOfLightEffectsMultiplier));
        }

        if (config.BossLunaticCultistEffects && state.DownedLunaticCultist)
        {
            BrightenTiles(LunarOreTiles, ClampBossMultiplier(config.BossLunaticCultistEffectsMultiplier));
        }

        if (config.BossMoonLordEffects && state.DownedMoonLord)
        {
            BrightenTiles(LunarOreTiles, ClampBossMultiplier(config.BossMoonLordEffectsMultiplier));
            BrightenTiles(ExoticMossTiles, ClampBossMultiplier(config.BossMoonLordEffectsMultiplier));
        }
    }

    private static void ApplyEventEffects(Config config, WorldLightingState state)
    {
        if (state.BloodMoonActive && config.BloodMoonEventEffects)
        {
            TintAllDefinedTiles(config.BloodMoonEventColor.ToVector3(), EventTintBlend);
        }

        if (state.EclipseActive && config.SolarEclipseEventEffects)
        {
            TintAllDefinedTiles(config.SolarEclipseEventColor.ToVector3(), EventTintBlend);
        }

        if (state.FrostLegionActive && config.FrostLegionEventEffects)
        {
            TintAllDefinedTiles(config.FrostLegionEventColor.ToVector3(), EventTintBlend);
        }
    }

    private static float ClampBossMultiplier(float multiplier)
    {
        return Math.Clamp(multiplier, MinBossMultiplier, MaxBossMultiplier);
    }

    private static void BrightenTiles(int[] tileIds, float multiplier)
    {
        if (multiplier <= 1f)
            return;

        for (int i = 0; i < tileIds.Length; i++)
        {
            int tileId = tileIds[i];
            if ((uint)tileId >= (uint)_tileLight.Length || !_hasTileLight[tileId])
                continue;

            _tileLight[tileId] = LightRuntime.ClampColor(_tileLight[tileId] * multiplier);
        }
    }

    private static void TintAllDefinedTiles(Vector3 tintColor, float blendAmount)
    {
        for (int tileId = 0; tileId < _tileLight.Length; tileId++)
        {
            if (!_hasTileLight[tileId])
                continue;

            _tileLight[tileId] = BlendTint(_tileLight[tileId], tintColor, blendAmount);
        }
    }

    private static Vector3 BlendTint(Vector3 baseColor, Vector3 tintColor, float blendAmount)
    {
        if (blendAmount <= 0f)
            return baseColor;

        Vector3 clampedTint = LightRuntime.ClampColor(tintColor);
        if (clampedTint.X <= 0f && clampedTint.Y <= 0f && clampedTint.Z <= 0f)
            return baseColor;

        float intensity = Math.Max(baseColor.X, Math.Max(baseColor.Y, baseColor.Z));
        if (intensity < MinimumTintIntensity)
            intensity = MinimumTintIntensity;

        Vector3 target = LightRuntime.ClampColor(clampedTint * intensity);
        return LightRuntime.ClampColor(Vector3.Lerp(baseColor, target, blendAmount));
    }

    public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        if (!LightRuntime.ModEnabled)
            return;

        if ((uint)type >= (uint)_tileLight.Length || !_hasTileLight[type])
            return;

        Vector3 color = _tileLight[type];
        r = color.X;
        g = color.Y;
        b = color.Z;
    }
}
