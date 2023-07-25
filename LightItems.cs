namespace LightingEssentials;

class LightItems : GlobalItem
{
    public override void MeleeEffects(Item item, Player player, Rectangle hitbox)
    {
        if (LightingEssentials.Config.PlayerMeleeLight > 0)
            Lighting.AddLight(player.Center, Vector3.One * LightingEssentials.Config.PlayerMeleeLight);
    }
}
