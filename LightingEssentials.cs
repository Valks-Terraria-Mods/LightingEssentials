global using Microsoft.Xna.Framework;
global using Terraria;
global using Terraria.DataStructures;
global using Terraria.ID;
global using Terraria.ModLoader;
using log4net;

namespace LightingEssentials;

public class LightingEssentials : Mod
{
	public static Config Config { get; set; }

	public static ILog Log { get; private set; }

    public override void Load()
    {
        Log = Logger;
    }
}
