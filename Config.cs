﻿using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace LightingEssentials;

[BackgroundColor(0, 0, 0, 100)]
public class Config : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [DefaultValue(true)] 
    [BackgroundColor(0, 0, 0, 100)]
    public bool ModEnabled;

    [DefaultValue(0.03f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float PlayerLight;

    [DefaultValue(0.0f)]
    [BackgroundColor(0, 75, 0, 100)]
    public float Grass;

    [DefaultValue(0.2f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Plants;

    [DefaultValue(0.1f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Containers;

    [DefaultValue(0.1f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Pots;

    [DefaultValue(0.5f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float Cactus;

    [DefaultValue(0.3f)]
    [BackgroundColor(0, 75, 0, 100)]
    public float JungleBiome;

    [DefaultValue(0.03f)]
    [BackgroundColor(0, 0, 75, 100)]
    public float SnowBiome;

    [DefaultValue(0.2f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float DesertBiome;

    [DefaultValue(0.5f)]
    [BackgroundColor(75, 0, 75, 100)]
    public float CorruptionBiome;

    [DefaultValue(0.5f)]
    [BackgroundColor(75, 0, 0, 100)]
    public float CrimsonBiome;

    [DefaultValue(0.5f)]
    [BackgroundColor(75, 0, 0, 100)]
    public float LifeCrystal;

    [DefaultValue(0.5f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float LifeFruitRed;

    [DefaultValue(0.5f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float LifeFruitGreen;

    [DefaultValue(0f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float LifeFruitBlue;

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

    [DefaultValue(0.01f)]
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

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool LightOres;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool LightEnvironment;

    public override void OnLoaded()
    {
        LightingEssentials.Config = this;
    }

    public override void OnChanged()
    {
        LightTiles.InitLight();
    }
}
