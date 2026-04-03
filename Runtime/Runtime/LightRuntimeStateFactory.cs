using System.Collections.Generic;

namespace LightingEssentials;

internal static class LightRuntimeStateFactory
{
    public static LightRuntimeState Create(LightingSettings config)
    {
        if (config is null)
            return LightRuntimeState.Disabled;

        bool modEnabled = config.ModEnabled;
        if (!modEnabled)
            return LightRuntimeState.Disabled;

        bool playerLightEnabled = false;
        Vector3 playerLightColor = Vector3.Zero;

        bool allProjectileLightEnabled = false;
        Vector3 allProjectileLightColor = Vector3.Zero;

        bool allEnemyLightEnabled = false;
        Vector3 allEnemyLightColor = Vector3.Zero;

        Dictionary<int, Vector3> projectileTypeLightColors = [];
        Dictionary<int, Vector3> enemyTypeLightColors = [];

        if (config.EntityEffectEntries is not null)
        {
            for (int i = 0; i < config.EntityEffectEntries.Count; i++)
            {
                LightingEntityEffectEntry entry = config.EntityEffectEntries[i];
                if (entry is null || !entry.Enabled)
                    continue;

                Vector3 color = LightColorMath.Clamp(entry.Color.ToVector3());
                if (LightColorMath.IsDark(color))
                    continue;

                if (entry.IncludePlayer && !playerLightEnabled)
                {
                    playerLightEnabled = true;
                    playerLightColor = color;
                }

                if (entry.IncludeAllProjectiles && !allProjectileLightEnabled)
                {
                    allProjectileLightEnabled = true;
                    allProjectileLightColor = color;
                }

                if (entry.ProjectileIds is not null)
                {
                    for (int j = 0; j < entry.ProjectileIds.Count; j++)
                    {
                        int projectileId = entry.ProjectileIds[j];
                        if (!projectileTypeLightColors.ContainsKey(projectileId))
                            projectileTypeLightColors[projectileId] = color;
                    }
                }

                if (entry.IncludeAllEnemies && !allEnemyLightEnabled)
                {
                    allEnemyLightEnabled = true;
                    allEnemyLightColor = color;
                }

                if (entry.NpcIds is not null)
                {
                    for (int j = 0; j < entry.NpcIds.Count; j++)
                    {
                        int npcId = entry.NpcIds[j];
                        if (!enemyTypeLightColors.ContainsKey(npcId))
                            enemyTypeLightColors[npcId] = color;
                    }
                }
            }
        }

        return new LightRuntimeState(
            modEnabled,
            playerLightEnabled,
            playerLightColor,
            allProjectileLightEnabled,
            allProjectileLightColor,
            projectileTypeLightColors,
            allEnemyLightEnabled,
            allEnemyLightColor,
            enemyTypeLightColors);
    }
}