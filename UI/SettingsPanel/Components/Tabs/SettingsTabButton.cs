using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Styling;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.Components.Tabs;

internal sealed class SettingsTabButton : FlatTextButton
{
    private bool _isActive;

    /// <summary>
    /// Creates a compact tab button for the settings tab strip.
    /// </summary>
    /// <param name="text">Tab label text.</param>
    public SettingsTabButton(string text)
        : base(text, 0.76f)
    {
        Height.Set(25f, 0f);
    }

    /// <summary>
    /// Applies active/inactive visual styling for this tab button.
    /// </summary>
    /// <param name="isActive">Whether this tab is currently selected.</param>
    public void SetActive(bool isActive)
    {
        _isActive = isActive;

        if (isActive)
        {
            BackgroundColor = new Color(58, 84, 112, 255);
            TextColor = Color.White;
            BorderColor = SettingsPanelTheme.Accent;
            return;
        }

        BackgroundColor = SettingsPanelTheme.ButtonBackground;
        TextColor = Color.White;
        BorderColor = SettingsPanelTheme.ButtonBorder;
    }

    /// <summary>
    /// Preserves active style while hovered and applies default hover handling otherwise.
    /// </summary>
    public override void MouseOver(UIMouseEvent evt)
    {
        if (_isActive)
            return;

        base.MouseOver(evt);
    }

    /// <summary>
    /// Preserves active style when cursor leaves and applies default behavior otherwise.
    /// </summary>
    public override void MouseOut(UIMouseEvent evt)
    {
        if (_isActive)
            return;

        base.MouseOut(evt);
    }
}
