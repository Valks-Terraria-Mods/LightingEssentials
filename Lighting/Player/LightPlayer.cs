namespace LightingEssentials;

public class LightPlayer : ModPlayer
{
    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!LightRuntime.TryGetPlayerLightColor(out Vector3 lightColor))
            return;

        Lighting.AddLight(drawInfo.Position, lightColor);
    }
}
