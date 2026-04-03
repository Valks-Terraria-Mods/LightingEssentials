using LightingEssentials.UI.SettingsPanel.Components.Tabs;
using LightingEssentials.UI.SettingsPanel.Models;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal static class LightingSettingsPanelVisualState
{
    public static void ApplyMinimizeState(LightingSettingsPanelRuntimeState state, LightingSettingsPanelPopupManager popupManager)
    {
        state.MinimizeButton?.SetText(state.IsMinimized ? "+" : "-");

        if (state.HeaderText is not null)
        {
            state.HeaderText.VAlign = state.IsMinimized ? 0.5f : 0f;
            state.HeaderText.Top.Set(state.IsMinimized ? 0f : state.Scale(2f), 0f);
        }

        if (state.MinimizeButton is not null)
        {
            state.MinimizeButton.VAlign = state.IsMinimized ? 0.5f : 0f;
            state.MinimizeButton.Top.Set(0f, 0f);
        }

        if (state.CloseButton is not null)
        {
            state.CloseButton.VAlign = state.IsMinimized ? 0.5f : 0f;
            state.CloseButton.Top.Set(0f, 0f);
        }

        if (state.ContentContainer is not null)
        {
            if (state.IsMinimized)
                state.ContentContainer.Remove();
            else if (state.ContentContainer.Parent is null)
                state.RootPanel?.Append(state.ContentContainer);
        }

        float targetHeight = state.IsMinimized ? state.Scale(36f) : state.ExpandedPanelHeight;
        if (state.RootPanel is not null)
        {
            state.RootPanel.Height.Set(targetHeight, 0f);
            state.RootPanel.Left.Set(-(state.PanelWidth + state.EdgeMargin), 1f);
            state.RootPanel.Top.Set(-(targetHeight + state.EdgeMargin), 1f);
        }

        if (state.IsMinimized)
            popupManager.CloseAll();
    }

    public static void RefreshTabButtonStyles(LightingSettingsPanelRuntimeState state)
    {
        foreach ((LightingSettingsTab tab, SettingsTabButton button) in state.TabButtons)
            button.SetActive(tab == state.ActiveTab);
    }
}
