using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TranscendCustom;

public class NPC : GlobalNPC
{
    public static bool FarmingMode { get; set; }

    public override void EditSpawnRate(Terraria.Player player, ref int spawnRate, ref int maxSpawns)
    {
        if (FarmingMode)
        {
            spawnRate = 1;
            maxSpawns = 30;
        }
    }

    public override void SetupShop(int type, Chest shop, ref int nextSlot)
    {
        // Scaling Health and Mana Potions
        if (Config.Instance.ScalingPotions && type == NPCID.Merchant)
        {
            var player = Main.player[Main.myPlayer];

            var h = Array.FindIndex(shop.item, i => i.type == ItemID.LesserHealingPotion);
            if (player.statLifeMax is >= 200 and <= 299)
            {
                shop.item[h].SetDefaults(ItemID.HealingPotion);
            }
            else if (player.statLifeMax is >= 300 and <= 499)
            {
                shop.item[h].SetDefaults(ItemID.GreaterHealingPotion);
            }
            else if (player.statLifeMax >= 500)
            {
                shop.item[h].SetDefaults(ItemID.SuperHealingPotion);
            }

            var m = Array.FindIndex(shop.item, i => i.type == ItemID.LesserManaPotion);
            if (player.statManaMax is >= 160 and <= 200)
            {
                shop.item[m].SetDefaults(ItemID.ManaPotion);
            }
            else if (player.statManaMax is >= 201 and <= 399)
            {
                shop.item[m].SetDefaults(ItemID.GreaterManaPotion);
            }
            else if (player.statManaMax >= 400)
            {
                shop.item[m].SetDefaults(ItemID.SuperManaPotion);
            }
        }
    }
}