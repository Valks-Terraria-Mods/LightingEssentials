namespace LightingEssentials;

internal static class LightRuntimeStateFactory
{
    public static LightRuntimeState Create(LightingSettings config)
    {
        if (config is null)
            return LightRuntimeState.Disabled;

        bool modEnabled = config.ModEnabled;

        Vector3 playerLightColor = LightColorMath.Clamp(config.PlayerLight.ToVector3());
        bool playerLightEnabled = modEnabled && config.PlayerLightEnabled && !LightColorMath.IsDark(playerLightColor);

        Vector3 projectileLightColor = LightColorMath.Clamp(config.ProjectileLightColor.ToVector3());
        bool projectileLightEnabled = modEnabled && config.ProjectileLightEnabled && !LightColorMath.IsDark(projectileLightColor);

        Vector3 enemyLightColor = LightColorMath.Clamp(config.EnemyLightColor.ToVector3());
        bool enemyLightEnabled = modEnabled && config.EnemyLightEnabled && !LightColorMath.IsDark(enemyLightColor);

        return new LightRuntimeState(
            modEnabled,
            playerLightEnabled,
            playerLightColor,
            projectileLightEnabled,
            projectileLightColor,
            enemyLightEnabled,
            enemyLightColor);
    }
}