using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Models;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal enum LightingEntityCatalogOptionKind
{
    None,
    Player,
    AllEnemies,
    AllProjectiles,
    Npc,
    Projectile,
}

internal sealed class LightingSettingsPanelEntryCatalogService
{
    private readonly LightingSettingsPanelGroupAddService _groupAddService = new();

    public List<CatalogPickerOption> BuildCatalogOptionsForTab(LightingSettingsTab tab, LightingSettings settings, int ignoreEntryIndex = -1)
    {
        List<CatalogPickerOption> options = [];

        if (tab == LightingSettingsTab.TileEffects)
            AddTileOptions(settings, options, ignoreEntryIndex);
        else if (tab == LightingSettingsTab.Events)
            AddEventOptions(settings, options, ignoreEntryIndex);
        else if (tab == LightingSettingsTab.EntityLights)
            AddEntityOptions(settings, options, ignoreEntryIndex);
        else if (tab == LightingSettingsTab.BossEffects)
            AddBossOptions(settings, options, ignoreEntryIndex);

        if (tab != LightingSettingsTab.EntityLights)
            options.Sort(static (a, b) => string.Compare(a.Label, b.Label, StringComparison.OrdinalIgnoreCase));

        return options;
    }

    public List<CatalogPickerOption> BuildBossTargetTileGroupOptions()
    {
        IReadOnlyList<LightingBossTargetTileGroupCatalogItem> groups = LightingDynamicCatalogs.GetBossTargetTileGroupCatalogItems();
        List<CatalogPickerOption> options = new(groups.Count);

        for (int i = 0; i < groups.Count; i++)
        {
            LightingBossTargetTileGroupCatalogItem group = groups[i];
            options.Add(new CatalogPickerOption(group.Key, group.DisplayName));
        }

        options.Sort(static (a, b) => string.Compare(a.Label, b.Label, StringComparison.OrdinalIgnoreCase));
        return options;
    }

    public bool TryAddSelectedGroup(LightingSettingsTab tab, LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        return _groupAddService.TryAddSelectedGroup(tab, settings, selectedOptions, groupName);
    }

    public bool TryAddBossGroup(LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedBossOptions, IReadOnlyList<string> targetTileGroupKeys, string groupName)
    {
        return _groupAddService.TryAddBossGroup(settings, selectedBossOptions, targetTileGroupKeys, groupName);
    }

    public static bool TryParseOptionKey(string key, string expectedPrefix, out int value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(key))
            return false;

        string[] parts = key.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2 && parts[0] == expectedPrefix && int.TryParse(parts[1], out value);
    }

    public static bool TryParseEntityOptionKey(string key, out LightingEntityCatalogOptionKind kind, out int value)
    {
        kind = LightingEntityCatalogOptionKind.None;
        value = 0;

        if (string.IsNullOrWhiteSpace(key))
            return false;

        if (string.Equals(key, "entity:player", StringComparison.Ordinal))
        {
            kind = LightingEntityCatalogOptionKind.Player;
            return true;
        }

        if (string.Equals(key, "entity:npc-all", StringComparison.Ordinal))
        {
            kind = LightingEntityCatalogOptionKind.AllEnemies;
            return true;
        }

        if (string.Equals(key, "entity:projectile-all", StringComparison.Ordinal))
        {
            kind = LightingEntityCatalogOptionKind.AllProjectiles;
            return true;
        }

        string[] parts = key.Split(':', 3, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3 || !string.Equals(parts[0], "entity", StringComparison.Ordinal) || !int.TryParse(parts[2], out value))
            return false;

        if (string.Equals(parts[1], "npc", StringComparison.Ordinal))
        {
            kind = LightingEntityCatalogOptionKind.Npc;
            return true;
        }

        if (string.Equals(parts[1], "projectile", StringComparison.Ordinal))
        {
            kind = LightingEntityCatalogOptionKind.Projectile;
            return true;
        }

        return false;
    }

    public static string ResolveGroupName(string groupName, IReadOnlyList<CatalogPickerOption> selectedOptions, string multiSelectionFallback)
    {
        if (!string.IsNullOrWhiteSpace(groupName))
            return groupName.Trim();

        return selectedOptions.Count == 1 ? selectedOptions[0].Label : multiSelectionFallback;
    }

    public static string FormatGroupLabel(string baseLabel, int groupCount)
    {
        return groupCount > 1 ? $"{baseLabel} ({groupCount})" : baseLabel;
    }

    private static void AddTileOptions(LightingSettings settings, List<CatalogPickerOption> options, int ignoreEntryIndex)
    {
        HashSet<int> usedTileIds = [];
        for (int i = 0; i < settings.TileEffectEntries.Count; i++)
        {
            if (i == ignoreEntryIndex)
                continue;

            LightingTileEffectEntry entry = settings.TileEffectEntries[i];
            if (entry?.TileIds is null)
                continue;

            for (int j = 0; j < entry.TileIds.Count; j++)
                usedTileIds.Add(entry.TileIds[j]);
        }

        IReadOnlyList<LightingTileCatalogItem> tiles = LightingDynamicCatalogs.GetTileCatalogItems();
        for (int i = 0; i < tiles.Count; i++)
        {
            LightingTileCatalogItem tile = tiles[i];
            if (!usedTileIds.Contains(tile.TileId))
                options.Add(new CatalogPickerOption($"tile:{tile.TileId}", tile.DisplayName));
        }
    }

    private static void AddEventOptions(LightingSettings settings, List<CatalogPickerOption> options, int ignoreEntryIndex)
    {
        HashSet<LightingEventId> usedEvents = [];
        for (int i = 0; i < settings.EventEffectEntries.Count; i++)
        {
            if (i == ignoreEntryIndex)
                continue;

            LightingEventEffectEntry entry = settings.EventEffectEntries[i];
            if (entry is null)
                continue;

            if (entry.EventIds is null || entry.EventIds.Count == 0)
            {
                usedEvents.Add(entry.EventId);
                continue;
            }

            for (int j = 0; j < entry.EventIds.Count; j++)
                usedEvents.Add(entry.EventIds[j]);
        }

        IReadOnlyList<LightingEventCatalogItem> events = LightingDynamicCatalogs.GetEventCatalogItems();
        for (int i = 0; i < events.Count; i++)
        {
            LightingEventCatalogItem eventItem = events[i];
            if (!usedEvents.Contains(eventItem.EventId))
                options.Add(new CatalogPickerOption($"event:{(int)eventItem.EventId}", eventItem.DisplayName));
        }
    }

    private static void AddBossOptions(LightingSettings settings, List<CatalogPickerOption> options, int ignoreEntryIndex)
    {
        HashSet<LightingBossId> usedBosses = [];
        for (int i = 0; i < settings.BossEffectEntries.Count; i++)
        {
            if (i == ignoreEntryIndex)
                continue;

            LightingBossEffectEntry entry = settings.BossEffectEntries[i];
            if (entry is null)
                continue;

            if (entry.BossIds is null || entry.BossIds.Count == 0)
            {
                usedBosses.Add(entry.BossId);
                continue;
            }

            for (int j = 0; j < entry.BossIds.Count; j++)
                usedBosses.Add(entry.BossIds[j]);
        }

        IReadOnlyList<LightingBossCatalogItem> bosses = LightingDynamicCatalogs.GetBossCatalogItems();
        for (int i = 0; i < bosses.Count; i++)
        {
            LightingBossCatalogItem bossItem = bosses[i];
            if (!usedBosses.Contains(bossItem.BossId))
                options.Add(new CatalogPickerOption($"boss:{(int)bossItem.BossId}", bossItem.DisplayName));
        }
    }

    private static void AddEntityOptions(LightingSettings settings, List<CatalogPickerOption> options, int ignoreEntryIndex)
    {
        HashSet<string> usedKeys = [];
        for (int i = 0; i < settings.EntityEffectEntries.Count; i++)
        {
            if (i == ignoreEntryIndex)
                continue;

            LightingEntityEffectEntry entry = settings.EntityEffectEntries[i];
            AddUsedEntityKeys(entry, usedKeys);
        }

        IReadOnlyList<LightingEntityCatalogItem> entities = LightingDynamicCatalogs.GetEntityCatalogItems();
        for (int i = 0; i < entities.Count; i++)
        {
            LightingEntityCatalogItem entityItem = entities[i];
            if (IsPinnedTopEntityOption(entityItem.Key))
                continue;

            if (!usedKeys.Contains(entityItem.Key))
                options.Add(new CatalogPickerOption(entityItem.Key, entityItem.DisplayName));
        }

        for (int i = 0; i < entities.Count; i++)
        {
            LightingEntityCatalogItem entityItem = entities[i];
            if (IsPinnedTopEntityOption(entityItem.Key))
                options.Add(new CatalogPickerOption(entityItem.Key, entityItem.DisplayName));
        }
    }

    private static bool IsPinnedTopEntityOption(string key)
    {
        return string.Equals(key, "entity:player", StringComparison.Ordinal)
            || string.Equals(key, "entity:npc-all", StringComparison.Ordinal)
            || string.Equals(key, "entity:projectile-all", StringComparison.Ordinal);
    }

    private static void AddUsedEntityKeys(LightingEntityEffectEntry entry, HashSet<string> usedKeys)
    {
        if (entry is null)
            return;

        if (entry.IncludePlayer)
            usedKeys.Add("entity:player");

        if (entry.IncludeAllEnemies)
            usedKeys.Add("entity:npc-all");

        if (entry.IncludeAllProjectiles)
            usedKeys.Add("entity:projectile-all");

        if (entry.NpcIds is not null)
            for (int i = 0; i < entry.NpcIds.Count; i++)
                usedKeys.Add($"entity:npc:{entry.NpcIds[i]}");

        if (entry.ProjectileIds is not null)
            for (int i = 0; i < entry.ProjectileIds.Count; i++)
                usedKeys.Add($"entity:projectile:{entry.ProjectileIds[i]}");
    }

}
