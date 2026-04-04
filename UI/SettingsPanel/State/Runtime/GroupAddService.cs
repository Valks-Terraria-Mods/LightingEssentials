using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Models;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal sealed class LightingSettingsPanelGroupAddService
{
    public bool TryAddSelectedGroup(LightingSettingsTab tab, LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        if (selectedOptions is null || selectedOptions.Count == 0)
            return false;

        return tab switch
        {
            LightingSettingsTab.TileEffects => TryAddTileGroup(settings, selectedOptions, groupName),
            LightingSettingsTab.Events => TryAddEventGroup(settings, selectedOptions, groupName),
            LightingSettingsTab.EntityLights => TryAddEntityGroup(settings, selectedOptions, groupName),
            LightingSettingsTab.BossEffects => TryAddBossGroup(settings, selectedOptions, null, groupName),
            _ => false,
        };
    }

    public bool TryAddBossGroup(
        LightingSettings settings,
        IReadOnlyList<CatalogPickerOption> selectedBossOptions,
        IReadOnlyList<string> targetTileGroupKeys,
        string groupName)
    {
        if (selectedBossOptions is null || selectedBossOptions.Count == 0)
            return false;

        HashSet<LightingBossId> used = [];
        for (int i = 0; i < settings.BossEffectEntries.Count; i++)
        {
            LightingBossEffectEntry entry = settings.BossEffectEntries[i];
            if (entry is null)
                continue;

            if (entry.BossIds is null || entry.BossIds.Count == 0)
            {
                used.Add(entry.BossId);
                continue;
            }

            for (int j = 0; j < entry.BossIds.Count; j++)
                used.Add(entry.BossIds[j]);
        }

        List<LightingBossId> bossIds = [];
        float defaultMultiplier = 1.4f;
        for (int i = 0; i < selectedBossOptions.Count; i++)
        {
            CatalogPickerOption option = selectedBossOptions[i];
            if (!LightingSettingsPanelEntryCatalogService.TryParseOptionKey(option.Key, "boss", out int bossIdRaw))
                continue;

            LightingBossId bossId = (LightingBossId)bossIdRaw;
            if (!LightingDynamicCatalogs.TryGetBossCatalogItem(bossId, out LightingBossCatalogItem item) || !used.Add(bossId))
                continue;

            if (bossIds.Count == 0)
                defaultMultiplier = item.DefaultMultiplier;

            bossIds.Add(bossId);
        }

        if (bossIds.Count == 0)
            return false;

        string resolvedName = LightingSettingsPanelEntryCatalogService.ResolveGroupName(groupName, selectedBossOptions, "Boss Group");
        List<string> resolvedTargetTileGroups = LightingDynamicCatalogs.ResolveBossTargetTileGroupKeys(bossIds, targetTileGroupKeys);

        settings.BossEffectEntries.Add(
            new LightingBossEffectEntry(
                resolvedName,
                bossIds,
                true,
                Math.Clamp(defaultMultiplier, 1f, 2f),
                resolvedTargetTileGroups));

        return true;
    }

    private static bool TryAddTileGroup(LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        HashSet<int> used = [];
        for (int i = 0; i < settings.TileEffectEntries.Count; i++)
        {
            LightingTileEffectEntry entry = settings.TileEffectEntries[i];
            if (entry?.TileIds is null)
                continue;

            for (int j = 0; j < entry.TileIds.Count; j++)
                used.Add(entry.TileIds[j]);
        }

        List<int> tileIds = [];
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            if (!LightingSettingsPanelEntryCatalogService.TryParseOptionKey(selectedOptions[i].Key, "tile", out int tileId) || !used.Add(tileId))
                continue;

            tileIds.Add(tileId);
        }

        if (tileIds.Count == 0)
            return false;

        string resolvedName = LightingSettingsPanelEntryCatalogService.ResolveGroupName(groupName, selectedOptions, "Tile Group");
        Color suggestedColor = LightingDynamicCatalogs.GetSuggestedTileColor(tileIds[0]);
        settings.TileEffectEntries.Add(new LightingTileEffectEntry(resolvedName, tileIds, suggestedColor, enabled: true));
        return true;
    }

    private static bool TryAddEventGroup(LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        HashSet<LightingEventId> used = [];
        for (int i = 0; i < settings.EventEffectEntries.Count; i++)
        {
            LightingEventEffectEntry entry = settings.EventEffectEntries[i];
            if (entry is null)
                continue;

            if (entry.EventIds is null || entry.EventIds.Count == 0)
            {
                used.Add(entry.EventId);
                continue;
            }

            for (int j = 0; j < entry.EventIds.Count; j++)
                used.Add(entry.EventIds[j]);
        }

        List<LightingEventId> eventIds = [];
        Color defaultColor = Color.White;
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!LightingSettingsPanelEntryCatalogService.TryParseOptionKey(option.Key, "event", out int eventIdRaw))
                continue;

            LightingEventId eventId = (LightingEventId)eventIdRaw;
            if (!LightingDynamicCatalogs.TryGetEventCatalogItem(eventId, out LightingEventCatalogItem item) || !used.Add(eventId))
                continue;

            if (eventIds.Count == 0)
                defaultColor = item.DefaultColor;

            eventIds.Add(eventId);
        }

        if (eventIds.Count == 0)
            return false;

        string resolvedName = LightingSettingsPanelEntryCatalogService.ResolveGroupName(groupName, selectedOptions, "Event Group");
        settings.EventEffectEntries.Add(new LightingEventEffectEntry(resolvedName, eventIds, true, defaultColor));
        return true;
    }

    private static bool TryAddEntityGroup(LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        bool playerUsed = false;
        bool allEnemiesUsed = false;
        bool allProjectilesUsed = false;
        HashSet<int> usedNpcIds = [];
        HashSet<int> usedProjectileIds = [];

        for (int i = 0; i < settings.EntityEffectEntries.Count; i++)
        {
            LightingEntityEffectEntry entry = settings.EntityEffectEntries[i];
            if (entry is null)
                continue;

            playerUsed |= entry.IncludePlayer;
            allEnemiesUsed |= entry.IncludeAllEnemies;
            allProjectilesUsed |= entry.IncludeAllProjectiles;

            if (entry.NpcIds is not null)
                for (int j = 0; j < entry.NpcIds.Count; j++)
                    usedNpcIds.Add(entry.NpcIds[j]);

            if (entry.ProjectileIds is not null)
                for (int j = 0; j < entry.ProjectileIds.Count; j++)
                    usedProjectileIds.Add(entry.ProjectileIds[j]);
        }

        LightingSettingsPanelEntrySelectionParser.EntitySelection selection = LightingSettingsPanelEntrySelectionParser.ParseSelectedEntitySelection(selectedOptions);

        bool includePlayer = selection.IncludePlayer && !playerUsed;
        bool includeAllEnemies = selection.IncludeAllEnemies && !allEnemiesUsed;
        bool includeAllProjectiles = selection.IncludeAllProjectiles && !allProjectilesUsed;

        List<int> npcIds = [];
        for (int i = 0; i < selection.NpcIds.Count; i++)
        {
            int npcId = selection.NpcIds[i];
            if (usedNpcIds.Add(npcId))
                npcIds.Add(npcId);
        }

        List<int> projectileIds = [];
        for (int i = 0; i < selection.ProjectileIds.Count; i++)
        {
            int projectileId = selection.ProjectileIds[i];
            if (usedProjectileIds.Add(projectileId))
                projectileIds.Add(projectileId);
        }

        if (!includePlayer && !includeAllEnemies && !includeAllProjectiles && npcIds.Count == 0 && projectileIds.Count == 0)
            return false;

        string resolvedName = LightingSettingsPanelEntryCatalogService.ResolveGroupName(groupName, selectedOptions, "Entity Group");
        Color defaultColor = ResolveDefaultEntityColor(includePlayer, includeAllEnemies, npcIds, includeAllProjectiles, projectileIds);

        settings.EntityEffectEntries.Add(new LightingEntityEffectEntry(
            resolvedName,
            true,
            defaultColor,
            includePlayer,
            includeAllEnemies,
            includeAllProjectiles,
            npcIds,
            projectileIds));

        return true;
    }

    private static Color ResolveDefaultEntityColor(bool includePlayer, bool includeAllEnemies, IReadOnlyList<int> npcIds, bool includeAllProjectiles, IReadOnlyList<int> projectileIds)
    {
        if (includePlayer)
            return LightingDynamicCatalogs.GetSuggestedEntityColor("entity:player");

        if (includeAllEnemies || npcIds.Count > 0)
            return LightingDynamicCatalogs.GetSuggestedEntityColor("entity:npc-all");

        if (includeAllProjectiles || projectileIds.Count > 0)
            return LightingDynamicCatalogs.GetSuggestedEntityColor("entity:projectile-all");

        return new Color(8, 8, 8);
    }
}
