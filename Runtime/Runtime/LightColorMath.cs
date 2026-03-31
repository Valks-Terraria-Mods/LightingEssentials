namespace LightingEssentials;

internal static class LightColorMath
{
    private static readonly Vector3 MinColor = Vector3.Zero;
    private static readonly Vector3 MaxColor = Vector3.One;

    public static Vector3 Clamp(Vector3 color)
    {
        return Vector3.Clamp(color, MinColor, MaxColor);
    }

    public static bool IsDark(Vector3 color)
    {
        return color.X <= 0f && color.Y <= 0f && color.Z <= 0f;
    }
}