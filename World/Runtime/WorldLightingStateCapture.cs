using Terraria.GameContent.Events;

namespace LightingEssentials;

internal static class WorldLightingStateCapture
{
    public static WorldLightingFlags CaptureEventFlags()
    {
        WorldLightingFlags flags = WorldLightingFlags.None;

        if (BirthdayParty.PartyIsUp)
            flags |= WorldLightingFlags.PartyActive;

        if (LanternNight.LanternsUp)
            flags |= WorldLightingFlags.LanternNightActive;

        if (Main.IsItRaining)
            flags |= WorldLightingFlags.RainActive;

        if (Sandstorm.Happening)
            flags |= WorldLightingFlags.SandstormActive;

        if (Main.IsItAHappyWindyDay)
            flags |= WorldLightingFlags.WindyDayActive;

        if (Main.IsItStorming)
            flags |= WorldLightingFlags.ThunderstormActive;

        if (Main.starGame)
            flags |= WorldLightingFlags.StarfallActive;

        if (Main.bloodMoon)
            flags |= WorldLightingFlags.BloodMoonActive;

        if (Main.invasionType == InvasionID.GoblinArmy)
            flags |= WorldLightingFlags.GoblinArmyActive;

        if (Main.slimeRain)
            flags |= WorldLightingFlags.SlimeRainActive;

        if (DD2Event.Ongoing)
            flags |= WorldLightingFlags.OldOnesArmyActive;

        if (Main.netMode != NetmodeID.Server && HasBuffSafe(Main.LocalPlayer, BuffID.Blackout))
            flags |= WorldLightingFlags.TorchGodActive;

        if (Main.invasionType == InvasionID.SnowLegion)
            flags |= WorldLightingFlags.FrostLegionActive;

        if (Main.eclipse)
            flags |= WorldLightingFlags.EclipseActive;

        if (Main.invasionType == InvasionID.PirateInvasion)
            flags |= WorldLightingFlags.PirateInvasionActive;

        if (Main.pumpkinMoon)
            flags |= WorldLightingFlags.PumpkinMoonActive;

        if (Main.snowMoon)
            flags |= WorldLightingFlags.FrostMoonActive;

        if (Main.invasionType == InvasionID.MartianMadness)
            flags |= WorldLightingFlags.MartianMadnessActive;

        if (NPC.LunarApocalypseIsUp)
            flags |= WorldLightingFlags.LunarEventsActive;

        return flags;
    }

    public static WorldLightingState Capture()
    {
        int mechBossesDowned = 0;
        if (NPC.downedMechBoss1)
            mechBossesDowned++;
        if (NPC.downedMechBoss2)
            mechBossesDowned++;
        if (NPC.downedMechBoss3)
            mechBossesDowned++;

        WorldLightingFlags flags = CaptureProgressionFlags() | CaptureEventFlags();
        return new WorldLightingState(flags, mechBossesDowned);
    }

    private static bool HasBuffSafe(Player player, int buffType)
    {
        if (player is null || buffType <= 0)
            return false;

        int[] activeBuffs = player.buffType;
        if (activeBuffs is null)
            return false;

        for (int i = 0; i < activeBuffs.Length; i++)
        {
            if (activeBuffs[i] == buffType)
                return true;
        }

        return false;
    }

    private static WorldLightingFlags CaptureProgressionFlags()
    {
        WorldLightingFlags flags = WorldLightingFlags.None;

        if (NPC.downedSlimeKing)
            flags |= WorldLightingFlags.DownedKingSlime;

        if (NPC.downedBoss1)
            flags |= WorldLightingFlags.DownedEyeOfCthulhu;

        if (NPC.downedBoss2)
        {
            if (WorldGen.crimson)
                flags |= WorldLightingFlags.DownedBrainOfCthulhu;
            else
                flags |= WorldLightingFlags.DownedEaterOfWorlds;

            flags |= WorldLightingFlags.DownedEvilBoss;
        }

        if (NPC.downedQueenBee)
            flags |= WorldLightingFlags.DownedQueenBee;

        if (NPC.downedBoss3)
            flags |= WorldLightingFlags.DownedSkeletron;

        if (NPC.downedDeerclops)
            flags |= WorldLightingFlags.DownedDeerclops;

        if (Main.hardMode)
            flags |= WorldLightingFlags.HardModeUnlocked;

        if (NPC.downedQueenSlime)
            flags |= WorldLightingFlags.DownedQueenSlime;

        if (NPC.downedMechBoss2)
            flags |= WorldLightingFlags.DownedTwins;

        if (NPC.downedMechBoss1)
            flags |= WorldLightingFlags.DownedDestroyer;

        if (NPC.downedMechBoss3)
            flags |= WorldLightingFlags.DownedSkeletronPrime;

        if (NPC.downedPlantBoss)
            flags |= WorldLightingFlags.DownedPlantera;

        if (NPC.downedGolemBoss)
            flags |= WorldLightingFlags.DownedGolem;

        if (NPC.downedFishron)
            flags |= WorldLightingFlags.DownedFishron;

        if (NPC.downedEmpressOfLight)
            flags |= WorldLightingFlags.DownedEmpressOfLight;

        if (NPC.downedAncientCultist)
            flags |= WorldLightingFlags.DownedLunaticCultist;

        if (NPC.downedMoonlord)
            flags |= WorldLightingFlags.DownedMoonLord;

        if (DD2Event.DownedInvasionT1)
            flags |= WorldLightingFlags.DownedDarkMage;

        if (DD2Event.DownedInvasionT2)
            flags |= WorldLightingFlags.DownedOgre;

        if (DD2Event.DownedInvasionT3)
            flags |= WorldLightingFlags.DownedBetsy;

        if (NPC.downedPirates)
            flags |= WorldLightingFlags.DownedFlyingDutchman;

        if (NPC.downedHalloweenTree)
            flags |= WorldLightingFlags.DownedMourningWood;

        if (NPC.downedHalloweenKing)
            flags |= WorldLightingFlags.DownedPumpking;

        if (NPC.downedChristmasTree)
            flags |= WorldLightingFlags.DownedEverscream;

        if (NPC.downedChristmasSantank)
            flags |= WorldLightingFlags.DownedSantaNk1;

        if (NPC.downedChristmasIceQueen)
            flags |= WorldLightingFlags.DownedIceQueen;

        if (NPC.downedMartians)
            flags |= WorldLightingFlags.DownedMartianSaucer;

        if (NPC.downedTowerSolar)
            flags |= WorldLightingFlags.DownedSolarPillar;

        if (NPC.downedTowerNebula)
            flags |= WorldLightingFlags.DownedNebulaPillar;

        if (NPC.downedTowerVortex)
            flags |= WorldLightingFlags.DownedVortexPillar;

        if (NPC.downedTowerStardust)
            flags |= WorldLightingFlags.DownedStardustPillar;

        return flags;
    }
}