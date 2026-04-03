using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Components.Tabs;
using LightingEssentials.UI.SettingsPanel.Models;
using LightingEssentials.UI.SettingsPanel.Styling;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal sealed class LightingSettingsPanelRuntimeState
{
    private readonly Dictionary<LightingSettingsTab, SettingsTabButton> _tabButtons = [];

    public LightingSettingsTab ActiveTab { get; set; } = LightingSettingsTab.TileEffects;

    public LightingSettings DefaultSettings { get; set; }

    public UIPanel RootPanel { get; set; }

    public UIElement ContentContainer { get; set; }

    public UIText HeaderText { get; set; }

    public FlatTextButton MinimizeButton { get; set; }

    public FlatTextButton CloseButton { get; set; }

    public UIList SettingsList { get; set; }

    public UIPanel SettingsScrollPanel { get; set; }

    public FlatTextButton AddEntryButton { get; set; }

    public bool IsMinimized { get; set; }

    public bool PendingConfigPersist { get; set; }

    public int PersistCountdownFrames { get; set; }

    public float UiScale { get; private set; }

    public float PanelWidth { get; private set; }

    public float ExpandedPanelHeight { get; private set; }

    public float EdgeMargin { get; private set; }

    public IReadOnlyDictionary<LightingSettingsTab, SettingsTabButton> TabButtons => _tabButtons;

    public void RefreshScaleMetrics()
    {
        UiScale = SettingsPanelScale.Current;
        PanelWidth = Scale(508f);
        ExpandedPanelHeight = Scale(504f);
        EdgeMargin = Scale(24f);
    }

    public void ClearTabButtons()
    {
        _tabButtons.Clear();
    }

    public void SetTabButton(LightingSettingsTab tab, SettingsTabButton button)
    {
        _tabButtons[tab] = button;
    }

    public float Scale(float baselinePixels)
    {
        return SettingsPanelScale.Pixels(baselinePixels, UiScale);
    }

    public float ScaleText(float baselineTextScale)
    {
        return SettingsPanelScale.Text(baselineTextScale, UiScale);
    }
}
