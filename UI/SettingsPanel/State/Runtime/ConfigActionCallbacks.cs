using System;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal readonly record struct LightingSettingsPanelConfigActionCallbacks(
    Action ToggleModEnabled,
    Action ResetAll,
    Action CopyModified,
    Action OpenImportPopup,
    Action ExportModified);
