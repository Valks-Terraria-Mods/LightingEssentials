using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LightingEssentials
{
    class LightItems : GlobalItem
    {
        public override void MeleeEffects(Item item, Player player, Rectangle hitbox)
        {
            Lighting.AddLight(player.Center, new Vector3(0.2f, 0.2f, 0.2f));
        }
    }
}
