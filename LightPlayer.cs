using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LightingEssentials
{
    class LightPlayer : ModPlayer
    {
        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            Lighting.AddLight(drawInfo.position, new Vector3(0.2f, 0.2f, 0.2f));
        }
    }
}
