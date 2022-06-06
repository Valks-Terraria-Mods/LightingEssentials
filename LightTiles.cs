namespace LightingEssentials;

class LightTiles : GlobalTile
{
    static ushort[] ores = {TileID.Iron, TileID.Lead, TileID.Copper, TileID.Tin, TileID.Silver, TileID.Gold, TileID.Platinum, TileID.Tungsten,
            TileID.Meteorite, TileID.Chlorophyte, TileID.Hellstone, TileID.Cobalt, TileID.Palladium, TileID.Mythril, TileID.Orichalcum, TileID.Adamantite,
            TileID.Titanium, TileID.LunarOre};

    static ushort[] environment = { TileID.Crystals, TileID.LifeFruit, TileID.Heart, TileID.BlueMoss, TileID.BrownMoss, TileID.GreenMoss, TileID.LavaMoss, TileID.LongMoss, TileID.PurpleMoss, TileID.RedMoss, TileID.Cactus, TileID.JunglePlants, TileID.JunglePlants2, TileID.JungleThorns, TileID.JungleVines, TileID.JungleGrass, TileID.LargePiles, TileID.LargePiles2, TileID.MushroomPlants, TileID.Plants, TileID.Plants2, TileID.Containers, TileID.Containers2 };

    public override void SetStaticDefaults()
    {
        for (int i = 0; i < ores.Length; i++)
        {
            Main.tileLighted[ores[i]] = true;
            Main.tileShine[ores[i]] = 400;
        }

        for (int i = 0; i < environment.Length; i++)
        {
            Main.tileLighted[environment[i]] = true;
        }
    }

    public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        /*
         * Vector2 vectorP = Main.LocalPlayer.position / new Vector2(16, 16);
                Vector2 vectorP2 = new Vector2((float)Math.Floor(vectorP.X), (float)Math.Floor(vectorP.X));



                Lighting.AddLight(new Vector2(i, j), new Vector3(0, 1, 0));
                */

        Point point = Utils.ToTileCoordinates(Main.LocalPlayer.position);



        LightOres(i, j, type, ref r, ref g, ref b);
        LightEnvironment(i, j, type, ref r, ref g, ref b);

        int radius = 3;

        if (point.X >= i - radius && point.X <= i + radius && point.Y >= j - radius && point.Y <= j + radius)
        {
            switch (type)
            {
                case TileID.Plants:
                case TileID.Plants2:
                case TileID.JungleGrass:
                case TileID.JunglePlants:
                case TileID.JunglePlants2:
                case TileID.JungleThorns:
                case TileID.JungleVines:
                    g += 0.1f;
                    break;
                case TileID.LifeFruit:
                case TileID.Heart:
                case TileID.Crystals:
                    r += 0.3f;
                    break;
            }
        }
    }

    private void LightOres(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        if (j > 300)
        {
            switch (type)
            {
                case TileID.Sapphire:
                    r = 0.1f;
                    g = 0.1f;
                    b = 1.0f;
                    break;
                case TileID.Ruby:
                    r = 1.0f;
                    g = 0.1f;
                    b = 0.1f;
                    break;
                case TileID.Diamond:
                    r = 0.5f;
                    g = 0.5f;
                    b = 0.5f;
                    break;
                case TileID.AmberGemspark:
                    r = 1.0f;
                    g = 0.5f;
                    b = 0.0f;
                    break;
                case TileID.Emerald:
                    r = 1.0f;
                    g = 0.1f;
                    b = 0.1f;
                    break;
                case TileID.Topaz:
                    r = 1.0f;
                    g = 0.5f;
                    b = 0.0f;
                    break;
                case TileID.Amethyst:
                    r = 0.9f;
                    g = 0.0f;
                    b = 0.9f;
                    break;
                case TileID.Iron:
                case TileID.Lead:
                case TileID.Copper:
                case TileID.Tin:
                case TileID.Silver:
                case TileID.Gold:
                case TileID.Platinum:
                case TileID.Tungsten:
                    r = 0.02f;
                    g = 0.02f;
                    b = 0.02f;
                    break;
                case TileID.Meteorite:
                    r = 1.0f;
                    g = 0.1f;
                    b = 0.1f;
                    break;
                case TileID.Chlorophyte:
                    r = 0.1f;
                    g = 1.0f;
                    b = 0.1f;
                    break;
                case TileID.Hellstone:
                    r = 1.0f;
                    g = 0.0f;
                    b = 0.0f;
                    break;
                case TileID.Cobalt:
                    r = 0.1f;
                    g = 0.1f;
                    b = 1.0f;
                    break;
                case TileID.Palladium:
                    r = 1.0f;
                    g = 0.5f;
                    b = 0.0f;
                    break;
                case TileID.Mythril:
                    r = 0.1f;
                    g = 0.1f;
                    b = 1.0f;
                    break;
                case TileID.Orichalcum:
                    r = 1.0f;
                    g = 0.0f;
                    b = 1.0f;
                    break;
                case TileID.Adamantite:
                    r = 0.9f;
                    g = 0.0f;
                    b = 0.9f;
                    break;
                case TileID.Titanium:
                    r = 0.1f;
                    g = 0.1f;
                    b = 0.9f;
                    break;
                case TileID.LunarOre:
                    r = 0.1f;
                    g = 0.1f;
                    b = 1.0f;
                    break;
            }
        }
    }

    private void LightEnvironment(int i, int j, int type, ref float r, ref float g, ref float b)
    {
        switch (type)
        {
            case TileID.BlueMoss:
                r = 0.00f;
                g = 0.00f;
                b = 0.02f;
                break;
            case TileID.BrownMoss:
                r = 0.01f;
                g = 0.01f;
                b = 0.01f;
                break;
            case TileID.GreenMoss:
                r = 0.00f;
                g = 0.02f;
                b = 0.00f;
                break;
            case TileID.LavaMoss:
                r = 0.02f;
                g = 0.00f;
                b = 0.00f;
                break;
            case TileID.LongMoss:
                r = 0.01f;
                g = 0.01f;
                b = 0.01f;
                break;
            case TileID.PurpleMoss:
                r = 0.02f;
                g = 0.00f;
                b = 0.02f;
                break;
            case TileID.RedMoss:
                r = 0.02f;
                g = 0.00f;
                b = 0.00f;
                break;
            case TileID.LifeFruit:
            case TileID.Heart:
            case TileID.Crystals:
                r = 0.5f;
                g = 0.0f;
                b = 0.0f;
                break;
            case TileID.JungleGrass:
            case TileID.JunglePlants:
            case TileID.JunglePlants2:
            case TileID.JungleThorns:
            case TileID.JungleVines:
                if (NPC.downedPlantBoss)
                {
                    r = 0.0f;
                    g = 0.2f;
                    b = 0.0f;
                }
                else
                {
                    r = 0.0f;
                    g = 0.1f;
                    b = 0.0f;
                }
                break;
            case TileID.LargePiles:
            case TileID.LargePiles2:
            case TileID.Containers:
            case TileID.Containers2:
                r = 0.1f;
                g = 0.1f;
                b = 0.1f;
                break;
            case TileID.Plants:
            case TileID.Plants2:
                r = 0.1f;
                g = 0.4f;
                b = 0.1f;
                break;
            case TileID.Cactus:
                r = 0.5f;
                g = 0.2f;
                b = 0.0f;
                break;
        }
    }
}
