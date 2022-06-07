namespace LightingEssentials;

class LightItems : GlobalItem
{
    public override void MeleeEffects(Item item, Player player, Rectangle hitbox)
    {
        if (LightingEssentials.Config.PlayerMeleeLight)
            Lighting.AddLight(player.Center, new Vector3(0.2f, 0.2f, 0.2f));
    }
}
