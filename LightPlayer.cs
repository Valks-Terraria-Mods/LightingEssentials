namespace LightingEssentials;

public class LightPlayer : ModPlayer
{
    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (LightingEssentials.Config.PlayerLight > 0)
        {
            Lighting.AddLight(drawInfo.Position, Vector3.One * LightingEssentials.Config.PlayerLight);
        }
    }
}
