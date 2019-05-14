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
            if (npc.type == NPCID.EyeofCthulhu)
            {
                for (int x = 0; x < 10; x++) {
                    for (int y = 0; y < 10; y++) {
                        Lighting.AddLight(npc.position + new Vector2(16 * x, 16 * y), new Vector3(1.0f, 0, 0));
                    }
                }
            }

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
