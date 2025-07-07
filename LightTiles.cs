using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.GameContent;

namespace LightingEssentials
{
    public class LightTiles : GlobalTile
    {
        // Precomputed light color lookups
        public static readonly Dictionary<int, Color> OreLight = [];
        public static readonly Dictionary<int, Color> EnvLight = [];

        public override void SetStaticDefaults()
        {
            if (!LightingEssentials.Config.ModEnabled)
                return;

            InitLight();
        }

        public static void InitLight()
        {
            // Initialize ore lighting flags
            if (LightingEssentials.Config.LightOres)
            {
                BuildOreLightTable();
                ApplyTileLightFlags(OreLight, true);
            }

            // Initialize environment lighting flags
            if (LightingEssentials.Config.LightEnvironment)
            {
                BuildEnvLightTable();
                ApplyTileLightFlags(EnvLight, true);
            }
        }

        /// <summary>
        /// Set Main.tileLighted and tileShine for any tile types that have non-zero light.
        /// </summary>
        private static void ApplyTileLightFlags(Dictionary<int, Color> table, bool enabled)
        {
            for (int type = 0; type < table.Count; type++)
            {
                if (table.ContainsKey(type))
                {
                    Main.tileLighted[type] = enabled;
                    Main.tileShine[type] = 1_000_000_000;
                    Main.tileShine2[type] = false;
                }
            }
        }

        /// <summary>
        /// Build lookup table for ore light colors.
        /// </summary>
        private static void BuildOreLightTable()
        {
            Config cfg = LightingEssentials.Config;

            // Gems
            Set(TileID.Sapphire, cfg.Sapphire);
            Set(TileID.Ruby, cfg.Ruby);
            Set(TileID.Diamond, cfg.Diamond);
            Set(TileID.AmberGemspark, cfg.AmberGemspark);
            Set(TileID.Emerald, cfg.Emerald);
            Set(TileID.Topaz, cfg.Topaz);
            Set(TileID.Amethyst, cfg.Amethyst);

            // Common ores share same intensity
            ushort[] commons =
            [
                TileID.Iron, TileID.Lead, TileID.Copper, TileID.Tin, TileID.Silver, TileID.Gold, TileID.Platinum, TileID.Tungsten
            ];
            
            foreach (ushort t in commons) 
                Set(t, cfg.CommonOres);

            // Other ores
            Set(TileID.Meteorite, cfg.Meteorite);
            Set(TileID.Chlorophyte, cfg.Chlorophyte);
            Set(TileID.Hellstone, cfg.Hellstone);
            Set(TileID.Cobalt, cfg.Cobalt);
            Set(TileID.Palladium, cfg.Palladium);
            Set(TileID.Mythril, cfg.Mythril);
            Set(TileID.Orichalcum, cfg.Orichalcum);
            Set(TileID.Adamantite, cfg.Adamantite);
            Set(TileID.Titanium, cfg.Titanium);
            Set(TileID.LunarOre, cfg.LunarOre);
            return;

            static void Set(int type, Color color)
            {
                if (color != Color.Transparent)
                {
                    if (!OreLight.TryAdd(type, color))
                    {
                        OreLight[type] = color;
                    }
                }
                else
                {
                    // TODO: Put default tile color calculated from PostDraw ONLY if the tile was added to the dictionary
                }
            }
        }

        bool once;

        public override void PostDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            base.PostDraw(i, j, type, spriteBatch);

            if (once)
            {
                return;
            }

            once = true;

            ReLogic.Content.Asset<Texture2D> tex = TextureAssets.Tile[type];
            Color[] pixels = new Color[tex.Width() * tex.Height()];
            ((Texture2D)tex).GetData(pixels);
            long r = 0, g = 0, b = 0;

            foreach (var c in pixels)
            {
                r += c.R; g += c.G; b += c.B;
            }

            int total = pixels.Length;

            Color DefaultTileColor = new((int)(r / total), (int)(g / total), (int)(b / total));

            LightingEssentials.Log.Info($"Default Color: r: {DefaultTileColor.R}, g: {DefaultTileColor.G}, b: {DefaultTileColor.B} for tile type {type}");
        }

        /// <summary>
        /// Build lookup table for environment light colors.
        /// </summary>
        private static void BuildEnvLightTable()
        {
            Config cfg = LightingEssentials.Config;

            // Grass and biome-specific
            Set(TileID.Grass, cfg.Grass);
            Set(TileID.CrimsonGrass, cfg.CrimsonBiome);
            Set(TileID.CrimsonJungleGrass, cfg.CrimsonBiome);
            Set(TileID.CrimsonPlants, cfg.CrimsonBiome);
            Set(TileID.CrimsonThorns, cfg.CrimsonBiome);
            Set(TileID.CrimsonVines, cfg.CrimsonBiome);
            Set(TileID.CorruptGrass, cfg.CorruptionBiome);
            
            // Biome containers, pots, etc.
            Set(TileID.Pots, cfg.Pots);
            Set(TileID.FossilOre, cfg.DesertBiome);
            Set(TileID.IceBlock, cfg.SnowBiome);

            // Moss types
            Set(TileID.BlueMoss, cfg.BlueMoss);
            Set(TileID.BrownMoss, cfg.BrownMoss);
            Set(TileID.GreenMoss, cfg.GreenMoss);
            Set(TileID.LavaMoss, cfg.LavaMoss);
            Set(TileID.LongMoss, cfg.LongMoss);
            Set(TileID.PurpleMoss, cfg.PurpleMoss);
            Set(TileID.RedMoss, cfg.RedMoss);

            // Life fruit/crystals
            Set(TileID.LifeFruit, cfg.LifeFruit);
            Set(TileID.Heart, cfg.LifeCrystal);
            Set(TileID.Crystals, cfg.LifeCrystal);

            // Jungle
            Set(TileID.JungleGrass, cfg.JungleBiome);
            Set(TileID.JunglePlants, cfg.JungleBiome);
            Set(TileID.JunglePlants2, cfg.JungleBiome);
            Set(TileID.JungleThorns, cfg.JungleBiome);
            Set(TileID.JungleVines, cfg.JungleBiome);

            // Generic piles/containers
            Set(TileID.LargePiles, cfg.Containers);
            Set(TileID.LargePiles2, cfg.Containers);
            Set(TileID.Containers, cfg.Containers);
            Set(TileID.Containers2, cfg.Containers);

            // Plants and cactus
            Set(TileID.Plants, cfg.Plants);
            Set(TileID.Plants2, cfg.Plants);
            Set(TileID.Cactus, cfg.Cactus);
            return;

            static void Set(int type, Color color)
            {
                if (color != Color.Transparent)
                {
                    if (!EnvLight.TryAdd(type, color))
                    {
                        EnvLight[type] = color;
                    }
                }
                else
                {
                    // TODO: Put default tile color calculated from PostDraw ONLY if the tile was added to the dictionary
                }
            }
        }

        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            if (!LightingEssentials.Config.ModEnabled)
                return;

            if (LightingEssentials.Config.LightOres)
            {
                if (OreLight.TryGetValue(type, out Color oreColor))
                {
                    if (oreColor != Color.Transparent)
                    {
                        r = oreColor.R / 255f;
                        g = oreColor.G / 255f;
                        b = oreColor.B / 255f;
                        return;
                    }
                }
            }

            if (LightingEssentials.Config.LightEnvironment)
            {
                if (EnvLight.TryGetValue(type, out Color envColor))
                {
                    if (envColor != Color.Transparent)
                    {
                        r = envColor.R / 255f;
                        g = envColor.G / 255f;
                        b = envColor.B / 255f;
                    }
                }
            }
        }
    }
}
