namespace LightingEssentials;

public static class LightRuntime
{
    private static readonly Vector3 MinColor = Vector3.Zero;
    private static readonly Vector3 MaxColor = Vector3.One;

    public static bool ModEnabled { get; private set; }

    public static bool PlayerLightEnabled { get; private set; }
    public static Vector3 PlayerLightColor { get; private set; }

    public static bool ProjectileLightEnabled { get; private set; }
    public static Vector3 ProjectileLightColor { get; private set; }

    public static void ApplyConfig(Config config)
    {
        if (config is null)
        {
            ModEnabled = false;
            PlayerLightEnabled = false;
            ProjectileLightEnabled = false;
            PlayerLightColor = Vector3.Zero;
            ProjectileLightColor = Vector3.Zero;
            return;
        }

        ModEnabled = config.ModEnabled;

        PlayerLightColor = ClampColor(config.PlayerLight.ToVector3());
        PlayerLightEnabled = ModEnabled && !IsDark(PlayerLightColor);

        ProjectileLightColor = ClampColor(config.ProjectileLightColor.ToVector3());
        ProjectileLightEnabled = ModEnabled && config.ProjectileLightEnabled && !IsDark(ProjectileLightColor);
    }

    public static Vector3 ClampColor(Vector3 color)
    {
        return Vector3.Clamp(color, MinColor, MaxColor);
    }

    private static bool IsDark(Vector3 color)
    {
        return color.X <= 0f && color.Y <= 0f && color.Z <= 0f;
    }
}
