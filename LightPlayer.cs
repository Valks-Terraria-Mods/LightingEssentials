namespace LightingEssentials;

class LightPlayer : ModPlayer
{
    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
    {
        Lighting.AddLight(drawInfo.Position, new Vector3(0.2f, 0.2f, 0.2f));
    }
}
