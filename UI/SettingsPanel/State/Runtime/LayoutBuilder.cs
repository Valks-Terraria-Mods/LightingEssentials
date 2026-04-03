using System;
using LightingEssentials.UI.SettingsPanel.Catalog;
using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Components.Tabs;
using LightingEssentials.UI.SettingsPanel.Models;
using LightingEssentials.UI.SettingsPanel.Styling;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal sealed class LightingSettingsPanelLayoutBuilder
{
    public void Build(UIState host, LightingSettingsPanelRuntimeState state, LightingSettingsPanelLayoutCallbacks callbacks)
    {
        _ = ModContent.GetInstance<LightingSettings>();

        state.RootPanel = new UIPanel();
        state.RootPanel.Width.Set(state.PanelWidth, 0f);
        state.RootPanel.Height.Set(state.ExpandedPanelHeight, 0f);
        state.RootPanel.Left.Set(-(state.PanelWidth + state.EdgeMargin), 1f);
        state.RootPanel.Top.Set(-(state.ExpandedPanelHeight + state.EdgeMargin), 1f);
        state.RootPanel.SetPadding(state.Scale(12f));
        state.RootPanel.BackgroundColor = SettingsPanelTheme.PanelBackground;
        state.RootPanel.BorderColor = SettingsPanelTheme.PanelBorder;
        host.Append(state.RootPanel);

        state.HeaderText = new UIText("Lighting Essentials", state.ScaleText(0.92f)) { HAlign = 0f };
        state.HeaderText.Top.Set(state.Scale(2f), 0f);
        state.RootPanel.Append(state.HeaderText);

        state.MinimizeButton = new FlatTextButton("-", state.ScaleText(0.74f)) { HAlign = 1f };
        state.MinimizeButton.Width.Set(state.Scale(22f), 0f);
        state.MinimizeButton.Height.Set(state.Scale(22f), 0f);
        state.MinimizeButton.Left.Set(-state.Scale(35f), 0f);
        state.MinimizeButton.OnLeftClick += (_, _) => callbacks.ToggleMinimize();
        state.RootPanel.Append(state.MinimizeButton);

        state.CloseButton = new FlatTextButton("X", state.ScaleText(0.74f)) { HAlign = 1f };
        state.CloseButton.Width.Set(state.Scale(22f), 0f);
        state.CloseButton.Height.Set(state.Scale(22f), 0f);
        state.CloseButton.OnLeftClick += (_, _) => callbacks.ClosePanel();
        state.RootPanel.Append(state.CloseButton);

        state.ContentContainer = new UIElement();
        state.ContentContainer.Width.Set(0f, 1f);
        state.ContentContainer.Height.Set(-state.Scale(30f), 1f);
        state.ContentContainer.Top.Set(state.Scale(30f), 0f);
        state.RootPanel.Append(state.ContentContainer);

        BuildTopControls(state, callbacks);
        BuildTabBar(state, callbacks);
        BuildSettingsList(state);

        state.AddEntryButton = new FlatTextButton(string.Empty, state.ScaleText(0.74f));
        state.AddEntryButton.Width.Set(0f, 1f);
        state.AddEntryButton.Height.Set(state.Scale(24f), 0f);
        state.AddEntryButton.Top.Set(-state.Scale(30f), 1f);
        state.AddEntryButton.OnLeftClick += (_, _) => callbacks.OpenCatalogForActiveTab();
        state.ContentContainer.Append(state.AddEntryButton);
    }

    private static void BuildTopControls(LightingSettingsPanelRuntimeState state, LightingSettingsPanelLayoutCallbacks callbacks)
    {
        UIElement topControls = new();
        topControls.Width.Set(0f, 1f);
        topControls.Height.Set(state.Scale(26f), 0f);
        state.ContentContainer.Append(topControls);

        UIText modEnabledText = new("Mod Enabled", state.ScaleText(0.78f)) { VAlign = 0.5f };
        topControls.Append(modEnabledText);

        FlatTextButton modEnabledButton = new(string.Empty, state.ScaleText(0.72f)) { VAlign = 0.5f };
        modEnabledButton.Left.Set(state.Scale(92f), 0f);
        modEnabledButton.Width.Set(state.Scale(58f), 0f);
        modEnabledButton.Height.Set(state.Scale(22f), 0f);
        modEnabledButton.OnLeftClick += (_, _) =>
        {
            callbacks.ToggleModEnabled();
            RefreshModEnabledButton(modEnabledButton);
        };
        topControls.Append(modEnabledButton);

        FlatTextButton resetAllButton = new("Reset to Defaults", state.ScaleText(0.72f)) { VAlign = 0.5f };
        resetAllButton.Left.Set(state.Scale(156f), 0f);
        resetAllButton.Width.Set(state.Scale(122f), 0f);
        resetAllButton.Height.Set(state.Scale(22f), 0f);
        resetAllButton.OnLeftClick += (_, _) =>
        {
            callbacks.ResetAll();
            RefreshModEnabledButton(modEnabledButton);
        };
        topControls.Append(resetAllButton);

        FlatTextButton copyModifiedButton = new("Copy Modified", state.ScaleText(0.72f)) { VAlign = 0.5f };
        copyModifiedButton.Left.Set(state.Scale(284f), 0f);
        copyModifiedButton.Width.Set(state.Scale(98f), 0f);
        copyModifiedButton.Height.Set(state.Scale(22f), 0f);
        copyModifiedButton.OnLeftClick += (_, _) => callbacks.CopyModified();
        topControls.Append(copyModifiedButton);

        RefreshModEnabledButton(modEnabledButton);
    }

    private static void RefreshModEnabledButton(FlatTextButton modEnabledButton)
    {
        LightingSettings currentSettings = ModContent.GetInstance<LightingSettings>();
        bool isEnabled = currentSettings.ModEnabled;
        modEnabledButton.SetText(isEnabled ? "ON" : "OFF");
        modEnabledButton.BackgroundColor = isEnabled ? SettingsPanelTheme.Positive : SettingsPanelTheme.Negative;
    }

    private static void BuildTabBar(LightingSettingsPanelRuntimeState state, LightingSettingsPanelLayoutCallbacks callbacks)
    {
        UIElement tabBar = new();
        tabBar.Width.Set(0f, 1f);
        tabBar.Height.Set(state.Scale(32f), 0f);
        tabBar.Top.Set(state.Scale(34f), 0f);
        state.ContentContainer.Append(tabBar);

        LightingSettingsTab[] tabs =
        [
            LightingSettingsTab.TileEffects,
            LightingSettingsTab.Events,
            LightingSettingsTab.EntityLights,
            LightingSettingsTab.BossEffects,
            LightingSettingsTab.Config
        ];

        float tabWidth = 1f / tabs.Length;
        for (int i = 0; i < tabs.Length; i++)
            AddTabButton(tabBar, state, tabs[i], i, tabWidth, callbacks.SelectTab);
    }

    private static void AddTabButton(UIElement tabBar, LightingSettingsPanelRuntimeState state, LightingSettingsTab tab, int index, float tabWidth, Action<LightingSettingsTab> onSelect)
    {
        SettingsTabButton button = new(LightingSettingsCatalog.GetTabTitle(tab), state.UiScale);
        button.Width.Set(0f, tabWidth);
        button.Left.Set(0f, index * tabWidth);
        button.OnLeftClick += (_, _) => onSelect(tab);
        state.SetTabButton(tab, button);
        tabBar.Append(button);
    }

    private static void BuildSettingsList(LightingSettingsPanelRuntimeState state)
    {
        state.SettingsScrollPanel = new UIPanel();
        state.SettingsScrollPanel.Width.Set(0f, 1f);
        state.SettingsScrollPanel.Height.Set(-state.Scale(76f), 1f);
        state.SettingsScrollPanel.Top.Set(state.Scale(80f), 0f);
        state.SettingsScrollPanel.SetPadding(state.Scale(8f));
        state.SettingsScrollPanel.BackgroundColor = new Color(14, 14, 14, 150);
        state.SettingsScrollPanel.BorderColor = SettingsPanelTheme.RowBorder;
        state.ContentContainer.Append(state.SettingsScrollPanel);

        float scrollbarWidth = state.Scale(20f);
        float scrollbarGap = state.Scale(6f);

        state.SettingsList = new UIList { ListPadding = state.Scale(8f), ManualSortMethod = static _ => { } };
        state.SettingsList.Width.Set(-(scrollbarWidth + scrollbarGap), 1f);
        state.SettingsList.Height.Set(0f, 1f);
        state.SettingsScrollPanel.Append(state.SettingsList);

        UIScrollbar scrollbar = new DarkScrollbar { HAlign = 1f };
        scrollbar.Width.Set(scrollbarWidth, 0f);
        scrollbar.Height.Set(0f, 1f);
        state.SettingsScrollPanel.Append(scrollbar);
        state.SettingsList.SetScrollbar(scrollbar);
    }
}
