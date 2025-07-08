using System.Collections.Generic;

namespace LightingEssentials;

public class LightTiles : GlobalTile
{
    public static readonly Dictionary<int, Vector3> OreLight = [];
    public static readonly Dictionary<int, Vector3> EnvLight = [];

    public override void SetStaticDefaults()
    {
        if (!LightingEssentials.Config.ModEnabled)
            return;

        InitLight();
    }

    public static void InitLight()
    {
        Config c = LightingEssentials.Config;

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
        ApplyTileLightFlags(OreLight, true);

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
        ApplyTileLightFlags(EnvLight, true);

        return;

        static void SetOre(int tileId, Color color)
        {
            OreLight[tileId] = color.ToVector3();
        }

        static void SetEnv(int tileId, Color color)
        {
            EnvLight[tileId] = color.ToVector3();
        }
    }

    private static void ApplyTileLightFlags(Dictionary<int, Vector3> table, bool enabled)
    {
        foreach (int tileId in table.Keys)
        {
            Main.tileLighted[tileId] = enabled;
            Main.tileShine[tileId] = 1_000_000_000;
            Main.tileShine2[tileId] = false;
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
        // This code needs to be performance critical
        ModifyLightCode(type, ref r, ref g, ref b);
    }

    public delegate void ModifyLightDelegate(int type, ref float r, ref float g, ref float b);

    public static ModifyLightDelegate ModifyLightCode = (int type, ref float r, ref float g, ref float b) =>
    {
        if (OreLight.TryGetValue(type, out Vector3 oreColor))
        {
            r = oreColor.X;
            g = oreColor.Y;
            b = oreColor.Z;
            return;
        }

        if (EnvLight.TryGetValue(type, out Vector3 envColor))
        {
            r = envColor.X;
            g = envColor.Y;
            b = envColor.Z;
        }
    };
}
