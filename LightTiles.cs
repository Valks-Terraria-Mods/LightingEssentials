using System;
using System.Collections.Generic;

namespace LightingEssentials
{
    public class LightTiles : GlobalTile
    {
        // Precomputed light color lookups
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
        private static void ApplyTileLightFlags(Dictionary<int, Vector3> table, bool enabled)
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
            Set(TileID.Sapphire, Math.Max(0, cfg.Sapphire - 0.9f), Math.Max(0, cfg.Sapphire - 0.9f), cfg.Sapphire);
            Set(TileID.Ruby, cfg.Ruby, Math.Max(0, cfg.Ruby - 0.9f), Math.Max(0, cfg.Ruby - 0.9f));
            Set(TileID.Diamond, cfg.Diamond, cfg.Diamond, cfg.Diamond);
            Set(TileID.AmberGemspark, cfg.AmberGemspark, Math.Max(0, cfg.AmberGemspark - 0.5f), 0f);
            Set(TileID.Emerald, cfg.Emerald, Math.Max(0, cfg.Emerald - 0.9f), Math.Max(0, cfg.Emerald - 0.9f));
            Set(TileID.Topaz, cfg.Topaz, Math.Max(0, cfg.Topaz - 0.5f), 0f);
            Set(TileID.Amethyst, cfg.Amethyst, 0f, cfg.Amethyst);

            // Common ores share same intensity
            float common = cfg.CommonOres;
            ushort[] commons =
            [
                TileID.Iron, TileID.Lead, TileID.Copper, TileID.Tin, TileID.Silver, TileID.Gold, TileID.Platinum, TileID.Tungsten
            ];
            
            foreach (ushort t in commons) Set(t, common, common, common);

            // Other ores
            Set(TileID.Meteorite, cfg.Meteorite, Math.Max(0, cfg.Meteorite - 0.9f), Math.Max(0, cfg.Meteorite - 0.9f));
            Set(TileID.Chlorophyte, Math.Max(0, cfg.Chlorophyte - 0.9f), cfg.Chlorophyte, Math.Max(0, cfg.Chlorophyte - 0.9f));
            Set(TileID.Hellstone, cfg.Hellstone, 0f, 0f);
            Set(TileID.Cobalt, Math.Max(0, cfg.Cobalt - 0.9f), Math.Max(0, cfg.Cobalt - 0.9f), cfg.Cobalt);
            Set(TileID.Palladium, cfg.Palladium, Math.Max(0, cfg.Palladium - 0.5f), 0f);
            Set(TileID.Mythril, Math.Max(0, cfg.Mythril - 0.9f), Math.Max(0, cfg.Mythril - 0.9f), cfg.Mythril);
            Set(TileID.Orichalcum, cfg.Orichalcum, 0f, cfg.Orichalcum);
            Set(TileID.Adamantite, cfg.Adamantite, 0f, cfg.Adamantite);
            Set(TileID.Titanium, Math.Max(0, cfg.Titanium - 0.8f), Math.Max(0, cfg.Titanium - 0.8f), cfg.Titanium);
            Set(TileID.LunarOre, Math.Max(0, cfg.LunarOre - 0.9f), Math.Max(0, cfg.LunarOre - 0.9f), cfg.LunarOre);
            return;

            void Set(int type, float r, float g, float b)
            {
                if (OreLight.ContainsKey(type))
                {
                    OreLight[type] = new Vector3(r, g, b);
                }
                else
                {
                    OreLight.Add(type, new Vector3(r, g, b));
                }
            }
        }

        /// <summary>
        /// Build lookup table for environment light colors.
        /// </summary>
        private static void BuildEnvLightTable()
        {
            Config cfg = LightingEssentials.Config;

            // Grass and biome-specific
            Set(TileID.Grass, 0f, cfg.Grass, 0f);
            Set(TileID.CrimsonGrass, cfg.CrimsonBiome, 0f, 0f);
            Set(TileID.CrimsonJungleGrass, cfg.CrimsonBiome, 0f, 0f);
            Set(TileID.CrimsonPlants, cfg.CrimsonBiome, 0f, 0f);
            Set(TileID.CrimsonThorns, cfg.CrimsonBiome, 0f, 0f);
            Set(TileID.CrimsonVines, cfg.CrimsonBiome, 0f, 0f);
            Set(TileID.CorruptGrass, Math.Max(0, cfg.CorruptionBiome - 0.1f), 0f, Math.Max(0, cfg.CorruptionBiome - 0.1f));
            
            // Biome containers, pots, etc.
            Set(TileID.Pots, cfg.Pots, 0f, cfg.Pots);
            Set(TileID.FossilOre, cfg.DesertBiome, Math.Max(0, cfg.DesertBiome - 0.3f), 0f);
            Set(TileID.IceBlock, 0f, 0f, cfg.SnowBiome);

            // Moss types
            Set(TileID.BlueMoss, 0f, 0f, cfg.BlueMoss);
            Set(TileID.BrownMoss, cfg.BrownMoss, cfg.BrownMoss, cfg.BrownMoss);
            Set(TileID.GreenMoss, 0f, cfg.GreenMoss, 0f);
            Set(TileID.LavaMoss, cfg.LavaMoss, 0f, 0f);
            Set(TileID.LongMoss, cfg.LongMoss, cfg.LongMoss, cfg.LongMoss);
            Set(TileID.PurpleMoss, cfg.PurpleMoss, 0f, cfg.PurpleMoss);
            Set(TileID.RedMoss, cfg.RedMoss, 0f, 0f);

            // Life fruit/crystals
            Set(TileID.LifeFruit, cfg.LifeFruitRed, cfg.LifeFruitGreen, cfg.LifeFruitBlue);
            Set(TileID.Heart, cfg.LifeCrystal, 0f, 0f);
            Set(TileID.Crystals, cfg.LifeCrystal, 0f, 0f);

            // Jungle
            Set(TileID.JungleGrass, 0f, cfg.JungleBiome, 0f);
            Set(TileID.JunglePlants, 0f, cfg.JungleBiome, 0f);
            Set(TileID.JunglePlants2, 0f, cfg.JungleBiome, 0f);
            Set(TileID.JungleThorns, 0f, cfg.JungleBiome, 0f);
            Set(TileID.JungleVines, 0f, cfg.JungleBiome, 0f);

            // Generic piles/containers
            Set(TileID.LargePiles, cfg.Containers, cfg.Containers, cfg.Containers);
            Set(TileID.LargePiles2, cfg.Containers, cfg.Containers, cfg.Containers);
            Set(TileID.Containers, cfg.Containers, cfg.Containers, cfg.Containers);
            Set(TileID.Containers2, cfg.Containers, cfg.Containers, cfg.Containers);

            // Plants and cactus
            Set(TileID.Plants, 0.1f, cfg.Plants, 0.1f);
            Set(TileID.Plants2, 0.1f, cfg.Plants, 0.1f);
            Set(TileID.Cactus, cfg.Cactus, Math.Max(0, cfg.Cactus - 0.3f), 0f);
            return;

            void Set(int type, float r, float g, float b)
            {
                if (EnvLight.ContainsKey(type))
                {
                    EnvLight[type] = new Vector3(r, g, b);
                }
                else
                {
                    EnvLight.Add(type, new Vector3(r, g, b));
                }
            }
        }

        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            if (!LightingEssentials.Config.ModEnabled)
                return;

            if (LightingEssentials.Config.LightOres)
            {
                if (OreLight.TryGetValue(type, out Vector3 oreColor))
                {
                    if (oreColor != Vector3.Zero)
                    {
                        r = oreColor.X;
                        g = oreColor.Y;
                        b = oreColor.Z;
                        return;
                    }
                }
            }

            if (LightingEssentials.Config.LightEnvironment)
            {
                if (EnvLight.TryGetValue(type, out Vector3 envColor))
                {
                    if (envColor != Vector3.Zero)
                    {
                        r = envColor.X;
                        g = envColor.Y;
                        b = envColor.Z;
                    }
                }
            }
        }
    }
}
