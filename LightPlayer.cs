namespace LightingEssentials;

public class LightPlayer : ModPlayer
{
    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        if (!LightingEssentials.Config.ModEnabled) return;
        
        if (LightingEssentials.Config.PlayerLight != Color.Transparent)
        {
            Vector3 color = LightingEssentials.Config.PlayerLight.ToVector3();

            Lighting.AddLight(drawInfo.Position, color);
        }
    }
}
