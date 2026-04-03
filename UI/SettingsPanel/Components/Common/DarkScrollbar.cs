using System;
using LightingEssentials.UI.SettingsPanel.Styling;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.Components.Common;

internal sealed class DarkScrollbar : UIScrollbar
{
    private const float MouseWheelStep = 32f;

    /// <summary>
    /// Applies a predictable wheel step amount on top of vanilla scrollbar handling.
    /// </summary>
    /// <param name="evt">Scroll-wheel event data.</param>
    public override void ScrollWheel(UIScrollWheelEvent evt)
    {
        base.ScrollWheel(evt);

        float scrollRange = Math.Max(0f, MaxViewSize - ViewSize);
        if (scrollRange <= 0f)
            return;

        float steps = evt.ScrollWheelValue / 120f;
        float scaledStep = SettingsPanelScale.Pixels(MouseWheelStep);
        ViewPosition = Math.Clamp(ViewPosition - (steps * scaledStep), 0f, scrollRange);
    }

    /// <summary>
    /// Draws a fully dark visual skin over the vanilla scrollbar.
    /// </summary>
    /// <param name="spriteBatch">Current sprite batch.</param>
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        CalculatedStyle dimensions = GetDimensions();
        Rectangle track = dimensions.ToRectangle();

        int edgePadding = Math.Max(1, (int)MathF.Round(SettingsPanelScale.Pixels(8f)));

        // Overpaint vanilla texture artifacts with a dark backdrop layer.
        Rectangle backdrop = new(track.X - edgePadding, track.Y - edgePadding, track.Width + (edgePadding * 2), track.Height + (edgePadding * 2));
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, backdrop, new Color(8, 8, 8, 150));
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, track, new Color(8, 8, 8, 190));

        // Cover the top and bottom caps
        Rectangle topCap = new(track.X, track.Y - 6, track.Width, 6);
        Rectangle bottomCap = new(track.X, track.Y + track.Height, track.Width, 6);

        spriteBatch.Draw(TextureAssets.MagicPixel.Value, topCap, new Color(8, 8, 8, 200));
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, bottomCap, new Color(8, 8, 8, 200));
    }
}
