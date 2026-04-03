using System.Collections.Generic;

namespace LightingEssentials;

public readonly struct LightRuntimeState
{
    private static readonly IReadOnlyDictionary<int, Vector3> EmptyColorMap = new Dictionary<int, Vector3>();

    public static readonly LightRuntimeState Disabled = new(
        modEnabled: false,
        playerLightEnabled: false,
        playerLightColor: Vector3.Zero,
        allProjectileLightEnabled: false,
        allProjectileLightColor: Vector3.Zero,
        projectileTypeLightColors: EmptyColorMap,
        allEnemyLightEnabled: false,
        allEnemyLightColor: Vector3.Zero,
        enemyTypeLightColors: EmptyColorMap);

    public bool ModEnabled { get; }
    public bool PlayerLightEnabled { get; }
    public Vector3 PlayerLightColor { get; }
    public bool AllProjectileLightEnabled { get; }
    public Vector3 AllProjectileLightColor { get; }
    public IReadOnlyDictionary<int, Vector3> ProjectileTypeLightColors { get; }
    public bool AllEnemyLightEnabled { get; }
    public Vector3 AllEnemyLightColor { get; }
    public IReadOnlyDictionary<int, Vector3> EnemyTypeLightColors { get; }

    public bool ProjectileLightEnabled => AllProjectileLightEnabled || ProjectileTypeLightColors.Count > 0;
    public bool EnemyLightEnabled => AllEnemyLightEnabled || EnemyTypeLightColors.Count > 0;

    public LightRuntimeState(
        bool modEnabled,
        bool playerLightEnabled,
        Vector3 playerLightColor,
        bool allProjectileLightEnabled,
        Vector3 allProjectileLightColor,
        IReadOnlyDictionary<int, Vector3> projectileTypeLightColors,
        bool allEnemyLightEnabled,
        Vector3 allEnemyLightColor,
        IReadOnlyDictionary<int, Vector3> enemyTypeLightColors)
    {
        ModEnabled = modEnabled;
        PlayerLightEnabled = playerLightEnabled;
        PlayerLightColor = playerLightColor;
        AllProjectileLightEnabled = allProjectileLightEnabled;
        AllProjectileLightColor = allProjectileLightColor;
        ProjectileTypeLightColors = projectileTypeLightColors ?? EmptyColorMap;
        AllEnemyLightEnabled = allEnemyLightEnabled;
        AllEnemyLightColor = allEnemyLightColor;
        EnemyTypeLightColors = enemyTypeLightColors ?? EmptyColorMap;
    }

    public bool TryGetPlayerLightColor(out Vector3 color)
    {
        if (ModEnabled && PlayerLightEnabled)
        {
            color = PlayerLightColor;
            return true;
        }

        color = Vector3.Zero;
        return false;
    }

    public bool TryGetProjectileLightColor(int projectileType, out Vector3 color)
    {
        if (!ModEnabled)
        {
            color = Vector3.Zero;
            return false;
        }

        if (ProjectileTypeLightColors.TryGetValue(projectileType, out color))
            return true;

        if (AllProjectileLightEnabled)
        {
            color = AllProjectileLightColor;
            return true;
        }

        color = Vector3.Zero;
        return false;
    }

    public bool TryGetEnemyLightColor(int npcType, out Vector3 color)
    {
        if (!ModEnabled)
        {
            color = Vector3.Zero;
            return false;
        }

        if (EnemyTypeLightColors.TryGetValue(npcType, out color))
            return true;

        if (AllEnemyLightEnabled)
        {
            color = AllEnemyLightColor;
            return true;
        }

        color = Vector3.Zero;
        return false;
    }
}