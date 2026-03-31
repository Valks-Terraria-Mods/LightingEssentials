using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using LightingEssentials.UI.SettingsPanel.Styling;

namespace LightingEssentials.UI.SettingsPanel.Components.Common;

internal class FlatTextButton : UITextPanel<string>
{
    private readonly Color _normalColor;
    private readonly Color _hoverColor;

    /// <summary>
    /// Controls whether hover transitions should modify background color.
    /// </summary>
    public bool HoverStyleEnabled { get; set; } = true;

    /// <summary>
    /// Creates a lightweight text button with consistent panel theme coloring.
    /// </summary>
    /// <param name="text">Button label text.</param>
    /// <param name="textScale">Font scale for button text.</param>
    public FlatTextButton(string text, float textScale = 0.8f)
        : base(text, textScale, large: false)
    {
        TextColor = Color.White;
        BackgroundColor = SettingsPanelTheme.ButtonBackground;
        BorderColor = SettingsPanelTheme.ButtonBorder;

        _normalColor = SettingsPanelTheme.ButtonBackground;
        _hoverColor = SettingsPanelTheme.ButtonHoverBackground;
    }

    /// <summary>
    /// Applies hover background tint.
    /// </summary>
    public override void MouseOver(UIMouseEvent evt)
    {
        base.MouseOver(evt);

        if (!HoverStyleEnabled)
            return;

        BackgroundColor = _hoverColor;
    }

    /// <summary>
    /// Restores non-hover background tint.
    /// </summary>
    public override void MouseOut(UIMouseEvent evt)
    {
        base.MouseOut(evt);

        if (!HoverStyleEnabled)
            return;

        BackgroundColor = _normalColor;
    }
}
