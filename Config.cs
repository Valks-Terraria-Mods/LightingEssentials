using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace LightingEssentials;

[BackgroundColor(0, 0, 0, 100)]
public class Config : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool PlayerMeleeLight;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool EntityRedHitLight;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool PlayerLight;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool LightOres;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool LightEnvironment;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool LightLifeFruitAndLifeCrystalsAndHearts;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool WalkingOnPlantsLightsThemUp;

    [DefaultValue(0.5f)]
    [BackgroundColor(75, 0, 0, 100)]
    public float LifeCrystal;

    [DefaultValue(0.5f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Cactus;

    [DefaultValue(0.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Plants;

    [DefaultValue(0.1f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Containers;

    [DefaultValue(0.2f)]
    [BackgroundColor(0, 75, 0, 100)]
    public float Jungle;

    [DefaultValue(0.02f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float RedMoss;

    [DefaultValue(0.02f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float PurpleMoss;

    [DefaultValue(0.01f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float LongMoss;

    [DefaultValue(0.02f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float LavaMoss;

    [DefaultValue(0.02f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float GreenMoss;

    [DefaultValue(0.01f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BrownMoss;

    [DefaultValue(0.02f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BlueMoss;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float LunarOre;
    
    [DefaultValue(0.9f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Titanium;

    [DefaultValue(0.9f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Adamantite;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Orichalcum;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Mythril;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Palladium;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Cobalt;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Hellstone;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Chlorophyte;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Meteorite;

    [DefaultValue(0.02f)]
    [BackgroundColor(0, 0, 75, 100)]
    public float CommonOres;

    [DefaultValue(0.9f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Amethyst;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Topaz;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Emerald;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float AmberGemspark;

    [DefaultValue(0.5f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Diamond;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Ruby;

    [DefaultValue(1.0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Sapphire;


    

    [DefaultValue(false)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool Experimental;

    public override void OnLoaded()
    {
        LightingEssentials.Config = this;
    }

    public override void OnChanged()
    {
        LightTiles.LightOres(LightOres);
    }
}
