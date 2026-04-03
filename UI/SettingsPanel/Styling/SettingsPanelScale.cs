using System;

namespace LightingEssentials.UI.SettingsPanel.Styling;

internal static class SettingsPanelScale
{
    public const float BaselineWidth = 1600;
    public const float BaselineHeight = 900;
    public const float ScaleEpsilon = 0.001f;

    public static float Current
    {
        get
        {
            float widthScale = Main.screenWidth / BaselineWidth;
            float heightScale = Main.screenHeight / BaselineHeight;
            return MathF.Min(widthScale, heightScale);
        }
    }

    public static float Pixels(float baselinePixels, float uiScale)
    {
        return baselinePixels * uiScale;
    }

    public static float Pixels(float baselinePixels)
    {
        return Pixels(baselinePixels, Current);
    }

    public static float Text(float baselineScale, float uiScale)
    {
        return baselineScale * uiScale;
    }
}