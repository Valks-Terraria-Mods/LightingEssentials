using System;
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
        ViewPosition = Math.Clamp(ViewPosition - (steps * MouseWheelStep), 0f, scrollRange);
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

        // Overpaint vanilla texture artifacts with a dark backdrop layer.
        Rectangle backdrop = new(track.X - 8, track.Y - 8, track.Width + 16, track.Height + 16);
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, backdrop, new Color(8, 8, 8, 205));
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, track, new Color(8, 8, 8, 190));

        // Cover tiny vanilla tip remnants at top/bottom.
        const int tipCover = 10;
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(track.X - 2, track.Y - tipCover, track.Width + 4, tipCover + 2), new Color(8, 8, 8, 255));
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(track.X - 2, track.Bottom - 2, track.Width + 4, tipCover + 2), new Color(8, 8, 8, 255));

        float scrollRange = Math.Max(0f, MaxViewSize - ViewSize);
        float progress = scrollRange <= 0f
            ? 0f
            : Math.Clamp(ViewPosition / scrollRange, 0f, 1f);

        float handleRatio = MaxViewSize <= 0f
            ? 1f
            : Math.Clamp(ViewSize / MaxViewSize, 0.08f, 1f);
        int innerHeight = Math.Max(1, track.Height - 4);
        int handleHeight = Math.Clamp((int)Math.Round(innerHeight * handleRatio), 14, innerHeight);
        int handleTop = track.Y + 2 + (int)Math.Round((innerHeight - handleHeight) * progress);

        // Draw custom handle with subtle hover feedback.
        Rectangle handle = new(
            track.X + 2,
            handleTop,
            Math.Max(4, track.Width - 4),
            handleHeight);
        Color handleColor = IsMouseHovering ? new Color(42, 42, 42, 250) : new Color(28, 28, 28, 245);
        spriteBatch.Draw(TextureAssets.MagicPixel.Value, handle, handleColor);
    }
}
