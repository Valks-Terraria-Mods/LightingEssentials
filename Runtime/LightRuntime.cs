namespace LightingEssentials;

public static class LightRuntime
{
    private static LightRuntimeState _state = LightRuntimeState.Disabled;

    public static bool ModEnabled => _state.ModEnabled;

    public static void ApplyConfig(LightingSettings config)
    {
        _state = LightRuntimeStateFactory.Create(config);
    }

    public static bool TryGetPlayerLightColor(out Vector3 color)
    {
        return _state.TryGetPlayerLightColor(out color);
    }

    public static bool TryGetProjectileLightColor(int projectileType, out Vector3 color)
    {
        return _state.TryGetProjectileLightColor(projectileType, out color);
    }

    public static bool TryGetEnemyLightColor(int npcType, out Vector3 color)
    {
        return _state.TryGetEnemyLightColor(npcType, out color);
    }

    public static Vector3 ClampColor(Vector3 color)
    {
        return LightColorMath.Clamp(color);
    }
}