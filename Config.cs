using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace LightingEssentials;

[BackgroundColor(0, 0, 0, 100)]
public class Config : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [DefaultValue(true)] 
    [BackgroundColor(0, 0, 0, 100)]
    public bool ModEnabled;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "7, 7, 7, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color PlayerLight;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 30, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Grass;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 30, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Plants;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "7, 7, 7, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Containers;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "7, 7, 7, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Pots;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "20, 40, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Cactus;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 20, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color JungleBiome;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 0, 20, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color SnowBiome;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "20, 20, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color DesertBiome;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "40, 0, 40, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color CorruptionBiome;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "40, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color CrimsonBiome;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "255, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color LifeCrystal;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 255, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color LifeFruit;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "10, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color RedMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "10, 0, 10, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color PurpleMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "10, 10, 10, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color LongMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "10, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color LavaMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 10, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color GreenMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "10, 10, 10, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color BrownMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 0, 10, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color BlueMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color LunarOre;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Titanium;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Adamantite;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Orichalcum;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Mythril;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Palladium;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Cobalt;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "50, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Hellstone;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 3, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Chlorophyte;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "255, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Meteorite;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Iron;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Lead;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Copper;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Tin;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Silver;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Gold;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Platinum;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Tungsten;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Amethyst;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Topaz;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Emerald;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color AmberGemspark;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Diamond;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Ruby;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Sapphire;

    public override void OnLoaded()
    {
        LightingEssentials.Config = this;
    }

    public override void OnChanged()
    {
        LightTiles.InitLight();

        if (ModEnabled)
        {
            LightTiles.ModifyLightCode = (int type, ref float r, ref float g, ref float b) =>
            {
                if (LightTiles.OreLight.TryGetValue(type, out Vector3 oreColor))
                {
                    r = oreColor.X;
                    g = oreColor.Y;
                    b = oreColor.Z;
                    return;
                }

                if (LightTiles.EnvLight.TryGetValue(type, out Vector3 envColor))
                {
                    r = envColor.X;
                    g = envColor.Y;
                    b = envColor.Z;
                }
            };
        }
        else
        {
            LightTiles.ModifyLightCode = (int type, ref float r, ref float g, ref float b) => { };
        }
        
    }
}
