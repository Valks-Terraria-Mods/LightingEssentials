using System;

namespace LightingEssentials;

public class LightTiles : GlobalTile
{
    private const float MinBossMultiplier = 1f;
    private const float MaxBossMultiplier = 4f;

    private static readonly int[] Boss1GrassPlantsTiles =
    [
        TileID.Grass,
        TileID.Plants,
        TileID.Plants2,
    ];

    private static readonly int[] Boss2EvilBiomeTiles =
    [
        TileID.CrimsonGrass,
        TileID.CrimsonJungleGrass,
        TileID.CrimsonPlants,
        TileID.CrimsonThorns,
        TileID.CrimsonVines,
        TileID.CorruptGrass,
    ];

    private static readonly int[] PlanteraJungleTiles =
    [
        TileID.JungleGrass,
        TileID.JunglePlants,
        TileID.JunglePlants2,
        TileID.JungleThorns,
        TileID.JungleVines,
    ];

    private static readonly int[] MechBossOreTiles =
    [
        TileID.Cobalt,
        TileID.Palladium,
        TileID.Mythril,
        TileID.Orichalcum,
        TileID.Adamantite,
        TileID.Titanium,
    ];

    private static readonly int[] Boss3MeteoriteTiles =
    [
        TileID.Meteorite,
    ];

    private static readonly int[] MoonLordLunarOreTiles =
    [
        TileID.LunarOre,
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

        BossProgressionState progression = BossProgressionState.Capture();

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
        SetEnv(TileID.Crystals, c.LifeCrystal);
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
        ApplyBossBrightnessBonuses(c, progression);

        return;

        static void SetOre(int tileId, Color color)
        {
            SetTileLight(tileId, color.ToVector3());
        }

        static void SetEnv(int tileId, Color color)
        {
            SetTileLight(tileId, color.ToVector3());
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

    private static void ApplyBossBrightnessBonuses(Config config, BossProgressionState progression)
    {
        if (config.BossEyeofCthulhuEffects && progression.DownedBoss1)
        {
            BrightenTiles(Boss1GrassPlantsTiles, ClampBossMultiplier(config.BossEyeofCthulhuEffectsMultiplier));
        }

        if (config.BossEvilBiomeEffects && progression.DownedBoss2)
        {
            BrightenTiles(Boss2EvilBiomeTiles, ClampBossMultiplier(config.BossEvilBiomeEffectsMultiplier));
        }

        if (config.BossSkeletronEffects && progression.DownedBoss3)
        {
            BrightenTiles(Boss3MeteoriteTiles, ClampBossMultiplier(config.BossSkeletronEffectsMultiplier));
        }

        if (config.BossPlanteraEffects && progression.DownedPlantera)
        {
            BrightenTiles(PlanteraJungleTiles, ClampBossMultiplier(config.BossPlanteraEffectsMultiplier));
        }

        if (config.BossMechEffects && progression.MechBossesDowned > 0)
        {
            float mechMaxMultiplier = ClampBossMultiplier(config.BossMechEffectsMultiplier);
            float mechProgress = progression.MechBossesDowned / 3f;
            float mechMultiplier = 1f + ((mechMaxMultiplier - 1f) * mechProgress);
            BrightenTiles(MechBossOreTiles, mechMultiplier);
        }

        if (config.BossMoonLordEffects && progression.DownedMoonLord)
        {
            BrightenTiles(MoonLordLunarOreTiles, ClampBossMultiplier(config.BossMoonLordEffectsMultiplier));
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

    /*private static Color GetAverageTileColor(int tileId)
    {
        // Attempt to get the tile texture asset
        Asset<Texture2D> tex = TextureAssets.Tile[tileId];

        LightingEssentials.Log.Debug("Valk: " + tileId);

        Color[] pixels = new Color[tex.Width() * tex.Height()];
        ((Texture2D)tex).GetData(pixels);
        long r = 0, g = 0, b = 0;

        foreach (Color c in pixels)
        {
            r += c.R; g += c.G; b += c.B;
        }

        int total = pixels.Length;

        Color defaultTileColor = new((int)(r / total), (int)(g / total), (int)(b / total));

        return defaultTileColor;
    }

    public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
    {
        base.PostDraw(i, j, type, spriteBatch);

        // We can safely get the texture tile in here with texture = TextureAssets.Tile[type] and texture.GetData(pixels)
    }*/

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
