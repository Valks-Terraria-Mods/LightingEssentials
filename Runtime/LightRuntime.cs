namespace LightingEssentials;

public static class LightRuntime
{
    private static LightRuntimeState _state = LightRuntimeState.Disabled;

    public static bool ModEnabled => _state.ModEnabled;

    public static bool PlayerLightEnabled => _state.PlayerLightEnabled;
    public static Vector3 PlayerLightColor => _state.PlayerLightColor;

    public static bool ProjectileLightEnabled => _state.ProjectileLightEnabled;
    public static Vector3 ProjectileLightColor => _state.ProjectileLightColor;

    public static bool EnemyLightEnabled => _state.EnemyLightEnabled;
    public static Vector3 EnemyLightColor => _state.EnemyLightColor;

    public static void ApplyConfig(LightingSettings config)
    {
        _state = LightRuntimeStateFactory.Create(config);
    }

    public static Vector3 ClampColor(Vector3 color)
    {
        return LightColorMath.Clamp(color);
    }
}