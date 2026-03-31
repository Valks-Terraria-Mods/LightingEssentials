using System.Collections.Generic;

namespace LightingEssentials;

internal static class TrackedBossNpcTypes
{
    private static readonly HashSet<int> Values = new()
    {
        NPCID.KingSlime,
        NPCID.EyeofCthulhu,
        NPCID.EaterofWorldsHead,
        NPCID.BrainofCthulhu,
        NPCID.QueenBee,
        NPCID.SkeletronHead,
        NPCID.Deerclops,
        NPCID.WallofFlesh,
        NPCID.QueenSlimeBoss,
        NPCID.TheDestroyer,
        NPCID.Retinazer,
        NPCID.Spazmatism,
        NPCID.SkeletronPrime,
        NPCID.Plantera,
        NPCID.Golem,
        NPCID.DukeFishron,
        NPCID.HallowBoss,
        NPCID.CultistBoss,
        NPCID.MoonLordCore,
    };

    public static bool Contains(int npcType)
    {
        return Values.Contains(npcType);
    }
}