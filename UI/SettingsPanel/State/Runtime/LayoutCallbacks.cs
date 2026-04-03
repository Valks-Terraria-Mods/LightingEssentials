using System;
using LightingEssentials.UI.SettingsPanel.Models;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal readonly record struct LightingSettingsPanelLayoutCallbacks(
    Action ToggleMinimize,
    Action ClosePanel,
    Action<LightingSettingsTab> SelectTab,
    Action OpenCatalogForActiveTab);
