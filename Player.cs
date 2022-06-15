using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TranscendsCustomizations
{
    public class Player : ModPlayer
    {
        private static readonly int _defaultTileRangeX = Terraria.Player.tileRangeX;
        private static readonly int _defaultTileRangeY = Terraria.Player.tileRangeY;
        private static readonly int _defaultItemGrabRange = Terraria.Player.defaultItemGrabRange;

        public Player()
        {
            if (Config.Instance.MaxItemPickupSpeed)
            {
                var itemGrabSpeedField = typeof(Terraria.Player).GetField("itemGrabSpeed", BindingFlags.NonPublic | BindingFlags.Static);
                if (itemGrabSpeedField != null)
                    itemGrabSpeedField.SetValue(null, 10f);
            }
        }

        public override void PreUpdate()
        {
            if (Config.Instance.PermaBuffs)
            {
	            player.AddBuff(BuffID.Dangersense, 2);
	            player.AddBuff(BuffID.Hunter, 2);
	            player.AddBuff(BuffID.NightOwl, 2);
	            player.AddBuff(BuffID.Spelunker, 2);
            }

            if (Config.Instance.NoPotionSickness)
            {
	            player.potionDelay = 0;

                var i = player.FindBuffIndex(BuffID.PotionSickness);
                if (i >= 0)
	                player.DelBuff(i);
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (TranscendsCustomizations.CrazySpawnsKeybind.JustPressed)
            {
                NPC.FarmingMode = !NPC.FarmingMode;
                Main.NewText("Farming mode is " + (NPC.FarmingMode ? "on" : "off"));
            }

            if (TranscendsCustomizations.TeleportKeybind.JustPressed)
            {
                var vector = new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y);
                player.Teleport(vector, 1, 0);
                player.velocity = Vector2.Zero;
                NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, vector.X, vector.Y, 1, 0, 0);
            }
        }

        public override void ResetEffects()
        {
            SetBlockRange();
        }

        public override void PostUpdateBuffs()
        {
            SetBlockRange();
        }

        private void SetBlockRange()
        {
            Terraria.Player.tileRangeX = Config.Instance.MaxReachRange ? 100 : _defaultTileRangeX;
            Terraria.Player.tileRangeY = Config.Instance.MaxReachRange ? 100 : _defaultTileRangeY;
            Terraria.Player.defaultItemGrabRange = Config.Instance.MaxItemPickupRange ? 1000 : _defaultItemGrabRange;
        }
        
        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
	        if (Config.Instance.MaxMinions)
		        player.maxMinions = 100;
	        if (Config.Instance.MaxTurrets)
		        player.maxTurrets = 100;
        }

        public override void ModifyManaCost(Terraria.Item item, ref float reduce, ref float mult)
        {
            if (Config.Instance.InfiniteMana)
                mult = 0;
        }

        /// <inheritdoc />
        public override bool ConsumeAmmo(Terraria.Item weapon, Terraria.Item ammo)
        {
	        return !Config.Instance.InfiniteAmmo;
        }
        
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            return !Config.Instance.DemigodMode;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (Config.Instance.InstantRespawn)
                player.respawnTimer = 0;
        }
        
        public override void OnRespawn(Terraria.Player player)
        {
            if (Config.Instance.InstantRespawn)
                player.statLife = player.statLifeMax;
        }
        
        public override TagCompound Save()
        {
	        var tag = new TagCompound();
	        tag.Add(nameof(Item.CustomUseTimes) + "_Keys", Item.CustomUseTimes.Keys.ToList());
	        tag.Add(nameof(Item.CustomUseTimes) + "_Values", Item.CustomUseTimes.Values.ToList());
	        return tag;
        }
        
        public override void Load(TagCompound tag)
        {
            try
            {
                var keys = tag.Get<List<int>>(nameof(Item.CustomUseTimes) + "_Keys");
                var values = tag.Get<List<int>>(nameof(Item.CustomUseTimes) + "_Values");
                Item.CustomUseTimes = Enumerable.Zip(keys, values, (i, i1) => new KeyValuePair<int, int>(i, i1)).ToDictionary(p => p.Key, p => p.Value);
            }
            catch (Exception e)
            {
                base.mod.Logger.WarnFormat("DougCustom.Player.LoadData() - WARNING! Could not load CustomUseTimes. Exception: {0}", e.Message);
            }
        }

        /// <inheritdoc />
        public override bool PreItemCheck()
        {
            Item.SetUseTime(player.inventory[player.selectedItem]);

	        return base.PreItemCheck();
        }
    }
}