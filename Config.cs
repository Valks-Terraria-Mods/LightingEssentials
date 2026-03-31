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

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossKingSlimeEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossKingSlimeEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossEyeofCthulhuEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossEyeofCthulhuEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossEvilBiomeEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossEvilBiomeEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossQueenBeeEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossQueenBeeEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossSkeletronEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossSkeletronEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossDeerclopsEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossDeerclopsEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossWallOfFleshEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossWallOfFleshEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossQueenSlimeEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossQueenSlimeEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossMechEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossMechEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossPlanteraEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.5f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossPlanteraEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossGolemEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossGolemEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossDukeFishronEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossDukeFishronEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossEmpressOfLightEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossEmpressOfLightEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossLunaticCultistEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossLunaticCultistEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossMoonLordEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossMoonLordEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BloodMoonEventEffects;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "255, 35, 35, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color BloodMoonEventColor;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool SolarEclipseEventEffects;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "255, 130, 35, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color SolarEclipseEventColor;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool FrostLegionEventEffects;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "90, 160, 255, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color FrostLegionEventColor;

    [DefaultValue(false)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool ProjectileLightEnabled;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "10, 10, 10, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color ProjectileLightColor;

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
    [DefaultValue(typeof(Color), "30, 16, 40, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color HallowedFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 18, 32, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color MushroomFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "18, 30, 8, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color HerbFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 24, 22, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color AquaticFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "34, 30, 10, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color SunflowerFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "18, 12, 8, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color AshFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "5, 22, 8, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color BambooFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "12, 12, 20, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color ExoticMoss;

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
    [DefaultValue(typeof(Color), "0, 0, 255, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color ManaCrystal;

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
        LightRuntime.ApplyConfig(this);
    }

    public override void OnChanged()
    {
        LightRuntime.ApplyConfig(this);
        LightTiles.InitLight();
    }
}
