using System;

namespace LightingEssentials;

internal static class TileLightStore
{
    private static Vector3[] _tileLight = [];
    private static bool[] _hasTileLight = [];

    public static void Reset(int tileCount)
    {
        EnsureCapacity(tileCount);
        Array.Clear(_hasTileLight, 0, _hasTileLight.Length);
    }

    public static void SetColor(int tileId, Vector3 color)
    {
        if ((uint)tileId >= (uint)_tileLight.Length)
            return;

        _tileLight[tileId] = LightRuntime.ClampColor(color);
        _hasTileLight[tileId] = true;

        Main.tileLighted[tileId] = true;
        Main.tileShine[tileId] = 1_000_000_000;
        Main.tileShine2[tileId] = false;
    }

    public static void Brighten(int[] tileIds, float multiplier)
    {
        if (multiplier <= 1f)
            return;

        for (int i = 0; i < tileIds.Length; i++)
        {
            int tileId = tileIds[i];
            if ((uint)tileId >= (uint)_tileLight.Length || !_hasTileLight[tileId])
                continue;

            _tileLight[tileId] = LightRuntime.ClampColor(_tileLight[tileId] * multiplier);
        }
    }

    public static void TintAll(Vector3 tintColor, float blendAmount, float minimumTintIntensity)
    {
        for (int tileId = 0; tileId < _tileLight.Length; tileId++)
        {
            if (!_hasTileLight[tileId])
                continue;

            _tileLight[tileId] = BlendTint(_tileLight[tileId], tintColor, blendAmount, minimumTintIntensity);
        }
    }

    public static bool TryGetColor(int tileId, out Vector3 color)
    {
        if ((uint)tileId >= (uint)_tileLight.Length || !_hasTileLight[tileId])
        {
            color = Vector3.Zero;
            return false;
        }

        color = _tileLight[tileId];
        return true;
    }

    private static Vector3 BlendTint(Vector3 baseColor, Vector3 tintColor, float blendAmount, float minimumTintIntensity)
    {
        if (blendAmount <= 0f)
            return baseColor;

        Vector3 clampedTint = LightRuntime.ClampColor(tintColor);
        if (clampedTint.X <= 0f && clampedTint.Y <= 0f && clampedTint.Z <= 0f)
            return baseColor;

        float intensity = Math.Max(baseColor.X, Math.Max(baseColor.Y, baseColor.Z));
        if (intensity < minimumTintIntensity)
            intensity = minimumTintIntensity;

        Vector3 target = LightRuntime.ClampColor(clampedTint * intensity);
        return LightRuntime.ClampColor(Vector3.Lerp(baseColor, target, blendAmount));
    }

    private static void EnsureCapacity(int tileCount)
    {
        if (_tileLight.Length == tileCount)
            return;

        _tileLight = new Vector3[tileCount];
        _hasTileLight = new bool[tileCount];
    }
}