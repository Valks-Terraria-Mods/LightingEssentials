using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LightingEssentials
{
    class LightNPCs : GlobalNPC
    {
        float red = 0f;

        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (red > 0.0f)
            {
                Lighting.AddLight(npc.position, new Vector3(red, 0, 0));
                red -= 0.01f;
            }
        }

        public override void HitEffect(NPC npc, int hitDirection, double damage) {
            float ratio = (float) npc.life / npc.lifeMax;
            red = (ratio - 1) * -1;
        }
    }
}
