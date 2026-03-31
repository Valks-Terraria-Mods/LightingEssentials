namespace LightingEssentials;

public class LightTiles : GlobalTile
{
    public override void SetStaticDefaults()
    {
        InitLight();
    }

    public static void InitLight()
    {
        TileLightInitializer.Initialize();
    }

    public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        if (!LightRuntime.ModEnabled)
            return;

        if (!TileLightStore.TryGetColor(type, out Vector3 color))
            return;

        r = color.X;
        g = color.Y;
        b = color.Z;
    }
}