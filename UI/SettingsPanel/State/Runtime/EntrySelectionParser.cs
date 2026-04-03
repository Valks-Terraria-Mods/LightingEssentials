using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Models;
using Terraria.ID;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal static class LightingSettingsPanelEntrySelectionParser
{
    internal sealed class EntitySelection
    {
        public bool IncludePlayer;
        public bool IncludeAllEnemies;
        public bool IncludeAllProjectiles;
        public List<int> NpcIds = [];
        public List<int> ProjectileIds = [];

        public int MemberCount =>
            (IncludePlayer ? 1 : 0)
            + (IncludeAllEnemies ? 1 : 0)
            + (IncludeAllProjectiles ? 1 : 0)
            + NpcIds.Count
            + ProjectileIds.Count;

        public bool HasAny => MemberCount > 0;
    }

    public static List<int> ParseSelectedTileIds(IReadOnlyList<CatalogPickerOption> selectedOptions)
    {
        List<int> tileIds = [];
        HashSet<int> seen = [];
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!LightingSettingsPanelEntryCatalogService.TryParseOptionKey(option.Key, "tile", out int tileId) || !seen.Add(tileId))
                continue;

            tileIds.Add(tileId);
        }

        return tileIds;
    }

    public static List<LightingEventId> ParseSelectedEventIds(IReadOnlyList<CatalogPickerOption> selectedOptions)
    {
        List<LightingEventId> eventIds = [];
        HashSet<LightingEventId> seen = [];
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!LightingSettingsPanelEntryCatalogService.TryParseOptionKey(option.Key, "event", out int eventIdRaw))
                continue;

            LightingEventId eventId = (LightingEventId)eventIdRaw;
            if (!LightingDynamicCatalogs.TryGetEventCatalogItem(eventId, out _) || !seen.Add(eventId))
                continue;

            eventIds.Add(eventId);
        }

        return eventIds;
    }

    public static List<LightingBossId> ParseSelectedBossIds(IReadOnlyList<CatalogPickerOption> selectedOptions)
    {
        List<LightingBossId> bossIds = [];
        HashSet<LightingBossId> seen = [];
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!LightingSettingsPanelEntryCatalogService.TryParseOptionKey(option.Key, "boss", out int bossIdRaw))
                continue;

            LightingBossId bossId = (LightingBossId)bossIdRaw;
            if (!LightingDynamicCatalogs.TryGetBossCatalogItem(bossId, out _) || !seen.Add(bossId))
                continue;

            bossIds.Add(bossId);
        }

        return bossIds;
    }

    public static EntitySelection ParseSelectedEntitySelection(IReadOnlyList<CatalogPickerOption> selectedOptions)
    {
        EntitySelection selection = new();
        HashSet<int> seenNpcIds = [];
        HashSet<int> seenProjectileIds = [];

        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!LightingSettingsPanelEntryCatalogService.TryParseEntityOptionKey(option.Key, out LightingEntityCatalogOptionKind kind, out int value))
                continue;

            switch (kind)
            {
                case LightingEntityCatalogOptionKind.Player:
                    selection.IncludePlayer = true;
                    break;
                case LightingEntityCatalogOptionKind.AllEnemies:
                    selection.IncludeAllEnemies = true;
                    break;
                case LightingEntityCatalogOptionKind.AllProjectiles:
                    selection.IncludeAllProjectiles = true;
                    break;
                case LightingEntityCatalogOptionKind.Npc:
                    if (value >= 0 && value < NPCID.Count && seenNpcIds.Add(value))
                        selection.NpcIds.Add(value);

                    break;
                case LightingEntityCatalogOptionKind.Projectile:
                    if (value >= 0 && value < ProjectileID.Count && seenProjectileIds.Add(value))
                        selection.ProjectileIds.Add(value);

                    break;
            }
        }

        return selection;
    }

    public static List<string> BuildSelectedEntityKeys(LightingEntityEffectEntry entry)
    {
        List<string> selectedKeys = [];
        if (entry is null)
            return selectedKeys;

        if (entry.IncludePlayer)
            selectedKeys.Add("entity:player");

        if (entry.IncludeAllEnemies)
            selectedKeys.Add("entity:npc-all");

        if (entry.IncludeAllProjectiles)
            selectedKeys.Add("entity:projectile-all");

        if (entry.NpcIds is not null)
            for (int i = 0; i < entry.NpcIds.Count; i++)
                selectedKeys.Add($"entity:npc:{entry.NpcIds[i]}");

        if (entry.ProjectileIds is not null)
            for (int i = 0; i < entry.ProjectileIds.Count; i++)
                selectedKeys.Add($"entity:projectile:{entry.ProjectileIds[i]}");

        return selectedKeys;
    }
}
