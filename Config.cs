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
