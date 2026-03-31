namespace LightingEssentials;

public readonly struct LightRuntimeState
{
    public static readonly LightRuntimeState Disabled = new(
        modEnabled: false,
        playerLightEnabled: false,
        playerLightColor: Vector3.Zero,
        projectileLightEnabled: false,
        projectileLightColor: Vector3.Zero,
        enemyLightEnabled: false,
        enemyLightColor: Vector3.Zero);

    public bool ModEnabled { get; }
    public bool PlayerLightEnabled { get; }
    public Vector3 PlayerLightColor { get; }
    public bool ProjectileLightEnabled { get; }
    public Vector3 ProjectileLightColor { get; }
    public bool EnemyLightEnabled { get; }
    public Vector3 EnemyLightColor { get; }

    public LightRuntimeState(
        bool modEnabled,
        bool playerLightEnabled,
        Vector3 playerLightColor,
        bool projectileLightEnabled,
        Vector3 projectileLightColor,
        bool enemyLightEnabled,
        Vector3 enemyLightColor)
    {
        ModEnabled = modEnabled;
        PlayerLightEnabled = playerLightEnabled;
        PlayerLightColor = playerLightColor;
        ProjectileLightEnabled = projectileLightEnabled;
        ProjectileLightColor = projectileLightColor;
        EnemyLightEnabled = enemyLightEnabled;
        EnemyLightColor = enemyLightColor;
    }
}