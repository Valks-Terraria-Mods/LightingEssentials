using System;

namespace LightingEssentials;

class LightTiles : GlobalTile
{
    public static readonly ushort[] Ores = {
        TileID.Iron, 
        TileID.Lead, 
        TileID.Copper, 
        TileID.Tin, 
        TileID.Silver, 
        TileID.Gold, 
        TileID.Platinum, 
        TileID.Tungsten,
        TileID.Meteorite, 
        TileID.Chlorophyte, 
        TileID.Hellstone, 
        TileID.Cobalt, 
        TileID.Palladium, 
        TileID.Mythril, 
        TileID.Orichalcum, 
        TileID.Adamantite,
        TileID.Titanium, 
        TileID.LunarOre
    };

    private static readonly ushort[] Environment = { 
        TileID.Crystals, 
        TileID.LifeFruit, 
        TileID.Heart, 
        TileID.BlueMoss, 
        TileID.BrownMoss, 
        TileID.GreenMoss, 
        TileID.LavaMoss, 
        TileID.LongMoss, 
        TileID.PurpleMoss, 
        TileID.RedMoss, 
        TileID.Cactus, 
        TileID.JunglePlants, 
        TileID.JunglePlants2, 
        TileID.JungleThorns, 
        TileID.JungleVines, 
        TileID.JungleGrass, 
        TileID.LargePiles, 
        TileID.LargePiles2, 
        TileID.MushroomPlants, 
        TileID.Plants, 
        TileID.Plants2, 
        TileID.Containers, 
        TileID.Containers2,
        TileID.CorruptGrass,
        TileID.CorruptIce,
        TileID.CorruptJungleGrass,
        TileID.CorruptPlants,
        TileID.CorruptThorns,
        TileID.CorruptVines,
        TileID.Pots,
        TileID.IceBlock,
        TileID.FossilOre,
        TileID.CrimsonGrass,
        TileID.CrimsonJungleGrass,
        TileID.CrimsonPlants,
        TileID.CrimsonThorns,
        TileID.CrimsonVines
    };

    public override void SetStaticDefaults()
    {
        if (LightingEssentials.Config.LightOres)
            LightOres(true);

        for (int i = 0; i < Environment.Length; i++)
        {
            Main.tileLighted[Environment[i]] = true;
        }
    }

    public static void LightOres(bool enabled) 
    {
        for (int i = 0; i < Ores.Length; i++)
        {
            Main.tileLighted[Ores[i]] = enabled;
            Main.tileShine[Ores[i]] = 400;
        }
    }

    public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        if (LightingEssentials.Config.LightOres)
            LightOres(i, j, type, ref r, ref g, ref b);

        if (LightingEssentials.Config.LightEnvironment)
            LightEnvironment(i, j, type, ref r, ref g, ref b);

        WalkingOnPlantsLightsThemUp(i, j, type, ref r, ref g, ref b);
    }

    void WalkingOnPlantsLightsThemUp(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        int radius = 2;

        Point point = Utils.ToTileCoordinates(Main.LocalPlayer.position);

        if (point.X >= i - radius && point.X <= i + radius && point.Y >= j - radius && point.Y <= j + radius)
        {
            switch (type)
            {
                case TileID.Plants:
                case TileID.Plants2:
                case TileID.JungleGrass:
                case TileID.JunglePlants:
                case TileID.JunglePlants2:
                case TileID.JungleThorns:
                case TileID.JungleVines:
                    r += LightingEssentials.Config.WalkingOnPlantsLightsThemUpRed;
                    g += LightingEssentials.Config.WalkingOnPlantsLightsThemUpGreen;
                    b += LightingEssentials.Config.WalkingOnPlantsLightsThemUpBlue;
                    break;
            }
        }
    }

    void LightOres(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        // Note that anything greater than a 'j' value of 300 is the start of the
        // underground (this is just a FYI)
        switch (type)
        {
            case TileID.Sapphire:
                r = Math.Max(0, LightingEssentials.Config.Sapphire - 0.9f);
                g = Math.Max(0, LightingEssentials.Config.Sapphire - 0.9f);
                b = LightingEssentials.Config.Sapphire;
                break;
            case TileID.Ruby:
                r = LightingEssentials.Config.Ruby;
                g = Math.Max(0, LightingEssentials.Config.Ruby - 0.9f);
                b = Math.Max(0, LightingEssentials.Config.Ruby - 0.9f);
                break;
            case TileID.Diamond:
                r = LightingEssentials.Config.Diamond;
                g = LightingEssentials.Config.Diamond;
                b = LightingEssentials.Config.Diamond;
                break;
            case TileID.AmberGemspark:
                r = LightingEssentials.Config.AmberGemspark;
                g = Math.Max(0, LightingEssentials.Config.AmberGemspark - 0.5f);
                b = 0.0f;
                break;
            case TileID.Emerald:
                r = LightingEssentials.Config.Emerald;
                g = Math.Max(0, LightingEssentials.Config.Emerald - 0.9f);
                b = Math.Max(0, LightingEssentials.Config.Emerald - 0.9f);
                break;
            case TileID.Topaz:
                r = LightingEssentials.Config.Topaz;
                g = Math.Max(0, LightingEssentials.Config.Topaz - 0.5f);
                b = 0.0f;
                break;
            case TileID.Amethyst:
                r = LightingEssentials.Config.Amethyst;
                g = 0.0f;
                b = LightingEssentials.Config.Amethyst;
                break;
            case TileID.Iron:
            case TileID.Lead:
            case TileID.Copper:
            case TileID.Tin:
            case TileID.Silver:
            case TileID.Gold:
            case TileID.Platinum:
            case TileID.Tungsten:
                r = LightingEssentials.Config.CommonOres;
                g = LightingEssentials.Config.CommonOres;
                b = LightingEssentials.Config.CommonOres;
                break;
            case TileID.Meteorite:
                r = LightingEssentials.Config.Meteorite;
                g = Math.Max(0, LightingEssentials.Config.Meteorite - 0.9f);
                b = Math.Max(0, LightingEssentials.Config.Meteorite - 0.9f);
                break;
            case TileID.Chlorophyte:
                r = Math.Max(0, LightingEssentials.Config.Chlorophyte - 0.9f);
                g = LightingEssentials.Config.Chlorophyte;
                b = Math.Max(0, LightingEssentials.Config.Chlorophyte - 0.9f);
                break;
            case TileID.Hellstone:
                r = LightingEssentials.Config.Hellstone;
                g = 0.0f;
                b = 0.0f;
                break;
            case TileID.Cobalt:
                r = Math.Max(0, LightingEssentials.Config.Cobalt - 0.9f);
                g = Math.Max(0, LightingEssentials.Config.Cobalt - 0.9f);
                b = LightingEssentials.Config.Cobalt;
                break;
            case TileID.Palladium:
                r = LightingEssentials.Config.Palladium;
                g = Math.Max(0, LightingEssentials.Config.Palladium - 0.5f);
                b = 0.0f;
                break;
            case TileID.Mythril:
                r = Math.Max(0, LightingEssentials.Config.Mythril - 0.9f);
                g = Math.Max(0, LightingEssentials.Config.Mythril - 0.9f);
                b = LightingEssentials.Config.Mythril;
                break;
            case TileID.Orichalcum:
                r = LightingEssentials.Config.Orichalcum;
                g = 0.0f;
                b = LightingEssentials.Config.Orichalcum;
                break;
            case TileID.Adamantite:
                r = LightingEssentials.Config.Adamantite;
                g = 0.0f;
                b = LightingEssentials.Config.Adamantite;
                break;
            case TileID.Titanium:
                r = Math.Max(0, LightingEssentials.Config.Titanium - 0.8f);
                g = Math.Max(0, LightingEssentials.Config.Titanium - 0.8f);
                b = LightingEssentials.Config.Titanium;
                break;
            case TileID.LunarOre:
                r = Math.Max(0, LightingEssentials.Config.LunarOre - 0.9f);
                g = Math.Max(0, LightingEssentials.Config.LunarOre - 0.9f);
                b = LightingEssentials.Config.LunarOre;
                break;
        }
    }

    void LightEnvironment(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        switch (type)
        {
            case TileID.CrimsonGrass:
            case TileID.CrimsonJungleGrass:
            case TileID.CrimsonPlants:
            case TileID.CrimsonThorns:
            case TileID.CrimsonVines:
                r = LightingEssentials.Config.CrimsonBiome;
                g = 0;
                b = 0;
                break;
            case TileID.CorruptGrass:
            case TileID.CorruptIce:
            case TileID.CorruptJungleGrass:
            case TileID.CorruptPlants:
            case TileID.CorruptThorns:
            case TileID.CorruptVines:
                // killed brain of chulutu (lol)
                if (NPC.downedBoss2)
                {
                    r = LightingEssentials.Config.CrimsonBiome;
                    g = 0;
                    b = LightingEssentials.Config.CrimsonBiome;
                }
                else
                {
                    r = Math.Max(0, LightingEssentials.Config.CrimsonBiome - 0.1f);
                    g = 0;
                    b = Math.Max(0, LightingEssentials.Config.CrimsonBiome - 0.1f);
                }
                break;
            case TileID.Pots:
                var player = Main.player[Main.myPlayer];

                if (player.ZoneCorrupt)
                {
                    // killed eater of worlds boss
                    if (NPC.downedBoss2)
                    {
                        r = LightingEssentials.Config.Pots;
                        g = 0;
                        b = LightingEssentials.Config.Pots;
                    }
                    else
                    {
                        r = Math.Max(0, LightingEssentials.Config.Pots - 0.1f);
                        g = 0;
                        b = Math.Max(0, LightingEssentials.Config.Pots - 0.1f);
                    }
                }
                else if (player.ZoneSnow)
                {
                    r = 0;
                    g = 0;
                    b = LightingEssentials.Config.Pots;
                }
                else if (player.ZoneDesert || player.ZoneUndergroundDesert)
                {
                    r = LightingEssentials.Config.Pots;
                    g = Math.Max(0, LightingEssentials.Config.Pots - 0.3f);
                    b = 0;
                }
                break;
            case TileID.FossilOre:
                r = LightingEssentials.Config.DesertBiome;
                g = Math.Max(0, LightingEssentials.Config.DesertBiome - 0.3f);
                b = 0;
                break;
            case TileID.IceBlock:
                r = 0;
                g = 0;
                b = LightingEssentials.Config.SnowBiome;
                break;
            case TileID.BlueMoss:
                r = 0.00f;
                g = 0.00f;
                b = LightingEssentials.Config.BlueMoss;
                break;
            case TileID.BrownMoss:
                r = LightingEssentials.Config.BrownMoss;
                g = LightingEssentials.Config.BrownMoss;
                b = LightingEssentials.Config.BrownMoss;
                break;
            case TileID.GreenMoss:
                r = 0.00f;
                g = LightingEssentials.Config.GreenMoss;
                b = 0.00f;
                break;
            case TileID.LavaMoss:
                r = LightingEssentials.Config.LavaMoss;
                g = 0.00f;
                b = 0.00f;
                break;
            case TileID.LongMoss:
                r = LightingEssentials.Config.LongMoss;
                g = LightingEssentials.Config.LongMoss;
                b = LightingEssentials.Config.LongMoss;
                break;
            case TileID.PurpleMoss:
                r = LightingEssentials.Config.PurpleMoss;
                g = 0.00f;
                b = LightingEssentials.Config.PurpleMoss;
                break;
            case TileID.RedMoss:
                r = LightingEssentials.Config.RedMoss;
                g = 0.00f;
                b = 0.00f;
                break;
            case TileID.LifeFruit:
                r = LightingEssentials.Config.LifeFruitRed;
                g = LightingEssentials.Config.LifeFruitGreen;
                b = LightingEssentials.Config.LifeFruitBlue;
                break;
            case TileID.Heart:
            case TileID.Crystals:
                r = LightingEssentials.Config.LifeCrystal;
                g = 0.0f;
                b = 0.0f;
                break;
            case TileID.JungleGrass:
            case TileID.JunglePlants:
            case TileID.JunglePlants2:
            case TileID.JungleThorns:
            case TileID.JungleVines:
                if (NPC.downedPlantBoss)
                {
                    r = 0.0f;
                    g = LightingEssentials.Config.JungleBiome;
                    b = 0.0f;
                }
                else
                {
                    r = 0.0f;
                    g = Math.Max(0, LightingEssentials.Config.JungleBiome - 0.1f);
                    b = 0.0f;
                }
                break;
            case TileID.LargePiles:
            case TileID.LargePiles2:
            case TileID.Containers:
            case TileID.Containers2:
                r = LightingEssentials.Config.Containers;
                g = LightingEssentials.Config.Containers;
                b = LightingEssentials.Config.Containers;
                break;
            case TileID.Plants:
            case TileID.Plants2:
                r = LightingEssentials.Config.PlantsRed;
                g = LightingEssentials.Config.PlantsGreen;
                b = LightingEssentials.Config.PlantsBlue;
                break;
            case TileID.Cactus:
                r = LightingEssentials.Config.Cactus;
                g = Math.Max(0, LightingEssentials.Config.Cactus - 0.3f);
                b = 0.0f;
                break;
        }
    }
}
