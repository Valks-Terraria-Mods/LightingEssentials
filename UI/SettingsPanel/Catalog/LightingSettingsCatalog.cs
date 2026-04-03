using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Models;

namespace LightingEssentials.UI.SettingsPanel.Catalog;

internal static class LightingSettingsCatalog
{
    // Dynamic tabs are composed from config entry lists; only these tabs remain fixed descriptors.
    private static readonly IReadOnlyDictionary<LightingSettingsTab, IReadOnlyList<LightingSettingDescriptor>> _hardcodedTabDescriptors =
        new Dictionary<LightingSettingsTab, IReadOnlyList<LightingSettingDescriptor>>
        {
            [LightingSettingsTab.Config] =
            [
                new FloatSettingDescriptor("UI Scale", 1f, 1.10f, 0.025f, static s => s.UiScale, static (s, v) => s.UiScale = v),
            ]
        };

    /// <summary>
    /// Returns descriptor definitions for tabs that intentionally remain static.
    /// </summary>
    /// <param name="tab">Tab whose descriptors should be returned.</param>
    /// <returns>Ordered descriptor list for the requested tab.</returns>
    public static IReadOnlyList<LightingSettingDescriptor> GetTabDescriptors(LightingSettingsTab tab)
    {
        return _hardcodedTabDescriptors.TryGetValue(tab, out IReadOnlyList<LightingSettingDescriptor> descriptors)
            ? descriptors
            : Array.Empty<LightingSettingDescriptor>();
    }

    /// <summary>
    /// Returns true when a tab should be rendered from dynamic user-managed entries.
    /// </summary>
    /// <param name="tab">Tab to inspect.</param>
    public static bool UsesDynamicEntries(LightingSettingsTab tab)
    {
        return tab is LightingSettingsTab.TileEffects or LightingSettingsTab.Events or LightingSettingsTab.EntityLights or LightingSettingsTab.BossEffects;
    }

    /// <summary>
    /// Returns the add button label for dynamic tabs.
    /// </summary>
    /// <param name="tab">Dynamic tab identifier.</param>
    public static string GetAddButtonLabel(LightingSettingsTab tab)
    {
        return tab switch
        {
            LightingSettingsTab.TileEffects => "Add new tile",
            LightingSettingsTab.Events => "Add new event",
            LightingSettingsTab.EntityLights => "Add new entity",
            LightingSettingsTab.BossEffects => "Add new boss",
            _ => "Add",
        };
    }

    /// <summary>
    /// Returns display title text for a tab identifier.
    /// </summary>
    /// <param name="tab">Tab enum value.</param>
    /// <returns>User-facing title string.</returns>
    public static string GetTabTitle(LightingSettingsTab tab)
    {
        return tab switch
        {
            LightingSettingsTab.TileEffects => "Tiles",
            LightingSettingsTab.Events => "Events",
            LightingSettingsTab.EntityLights => "Entity",
            LightingSettingsTab.BossEffects => "Boss",
            LightingSettingsTab.Config => "Config",
            _ => "TabNeedsName",
        };
    }
}
