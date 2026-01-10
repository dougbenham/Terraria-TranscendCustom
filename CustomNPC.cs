using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TranscendsCustomizations
{
    public class CustomNPC : GlobalNPC
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

        public override void ModifyActiveShop(NPC npc, string shopName, Item[] items)
        {
            // Scaling Health and Mana Potions
            if (Config.Instance.ScalingPotions && npc.type == NPCID.Merchant)
            {
                var player = Main.player[Main.myPlayer];

                var h = Array.FindIndex(items, i => i.type == ItemID.LesserHealingPotion);
                if (player.statLifeMax is >= 200 and <= 299)
                {
	                items[h].SetDefaults(ItemID.HealingPotion);
                }
                else if (player.statLifeMax is >= 300 and <= 499)
                {
	                items[h].SetDefaults(ItemID.GreaterHealingPotion);
                }
                else if (player.statLifeMax >= 500)
                {
	                items[h].SetDefaults(ItemID.SuperHealingPotion);
                }

                var m = Array.FindIndex(items, i => i.type == ItemID.LesserManaPotion);
                if (player.statManaMax is >= 160 and <= 200)
                {
	                items[m].SetDefaults(ItemID.ManaPotion);
                }
                else if (player.statManaMax is >= 201 and <= 399)
                {
	                items[m].SetDefaults(ItemID.GreaterManaPotion);
                }
                else if (player.statManaMax >= 400)
                {
	                items[m].SetDefaults(ItemID.SuperManaPotion);
                }
            }
        }
    }
}