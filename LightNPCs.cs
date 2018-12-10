using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LightingEssentials
{
    class LightNPCs : GlobalNPC
    {
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.type == NPCID.EyeofCthulhu) {
                Lighting.AddLight(npc.position, new Vector3(1.0f, 0, 0));
            } else if (!npc.townNPC && !npc.friendly) {
                Lighting.AddLight(npc.position, new Vector3(0.3f, 0, 0));
            }
        }
    }
}
