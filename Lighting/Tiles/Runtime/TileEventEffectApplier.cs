namespace LightingEssentials;

internal static class TileEventEffectApplier
{
    private const float EventTintBlend = 0.82f;
    private const float MinimumTintIntensity = 0.12f;

    public static void Apply(Config config, in WorldLightingState state)
    {
        if (state.BloodMoonActive && config.BloodMoonEventEffects)
        {
            TileLightStore.TintAll(config.BloodMoonEventColor.ToVector3(), EventTintBlend, MinimumTintIntensity);
        }

        if (state.EclipseActive && config.SolarEclipseEventEffects)
        {
            TileLightStore.TintAll(config.SolarEclipseEventColor.ToVector3(), EventTintBlend, MinimumTintIntensity);
        }

        if (state.FrostLegionActive && config.FrostLegionEventEffects)
        {
            TileLightStore.TintAll(config.FrostLegionEventColor.ToVector3(), EventTintBlend, MinimumTintIntensity);
        }
    }
}