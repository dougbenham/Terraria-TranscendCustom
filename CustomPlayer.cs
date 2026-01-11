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
    public class CustomPlayer : ModPlayer
    {
        private static readonly int _defaultTileRangeX = Terraria.Player.tileRangeX;
        private static readonly int _defaultTileRangeY = Terraria.Player.tileRangeY;
        private static readonly int _defaultItemGrabRange = Terraria.Player.defaultItemGrabRange;
		
        public override void PreUpdate()
        {
            if (Config.Instance.PermaBuffs)
            {
	            Player.AddBuff(BuffID.Dangersense, 2);
	            Player.AddBuff(BuffID.Hunter, 2);
	            Player.AddBuff(BuffID.NightOwl, 2);
	            Player.AddBuff(BuffID.Spelunker, 2);
            }

            if (Config.Instance.NoPotionSickness)
            {
	            Player.potionDelay = 0;

                var i = Player.FindBuffIndex(BuffID.PotionSickness);
                if (i >= 0)
	                Player.DelBuff(i);
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (TranscendsCustomizations.CrazySpawnsKeybind.JustPressed)
            {
	            CustomNPC.FarmingMode = !CustomNPC.FarmingMode;
                Main.NewText("Farming mode is " + (CustomNPC.FarmingMode ? "on" : "off"));
            }

            if (TranscendsCustomizations.TeleportKeybind.JustPressed)
            {
                var vector = new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y);
                Player.Teleport(vector, 1, 0);
                Player.velocity = Vector2.Zero;
                NetMessage.SendData(MessageID.TeleportEntity, -1, -1, null, 0, Player.whoAmI, vector.X, vector.Y, 1, 0, 0);
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

        /// <inheritdoc />
        public override void UpdateEquips()
        {
	        if (Config.Instance.MaxMinions)
		        Player.maxMinions = 100;
	        if (Config.Instance.MaxTurrets)
		        Player.maxTurrets = 100;
        }

        public override void ModifyManaCost(Terraria.Item item, ref float reduce, ref float mult)
        {
            if (Config.Instance.InfiniteMana)
                mult = 0;
        }

        /// <inheritdoc />
        public override bool CanConsumeAmmo(Terraria.Item weapon, Terraria.Item ammo)
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
                Player.respawnTimer = 0;
        }
        
        public override void OnRespawn()
        {
            if (Config.Instance.InstantRespawn)
                Player.statLife = Player.statLifeMax;
        }
        
        public override void SaveData(TagCompound tag)
        {
	        tag.Add(nameof(CustomItem.CustomUseTimes) + "_Keys", CustomItem.CustomUseTimes.Keys.ToList());
	        tag.Add(nameof(CustomItem.CustomUseTimes) + "_Values", CustomItem.CustomUseTimes.Values.ToList());
        }
        
        public override void LoadData(TagCompound tag)
        {
            try
            {
                var keys = tag.Get<List<int>>(nameof(CustomItem.CustomUseTimes) + "_Keys");
                var values = tag.Get<List<int>>(nameof(CustomItem.CustomUseTimes) + "_Values");
                CustomItem.CustomUseTimes = new(Enumerable.Zip(keys, values, KeyValuePair.Create));
            }
            catch (Exception e)
            {
                base.Mod.Logger.WarnFormat("CustomPlayer.LoadData(): Could not load CustomUseTimes. Exception: {0}", e.Message);
            }
        }

        /// <inheritdoc />
        public override bool PreItemCheck()
        {
            CustomItem.SetUseTime(Player.inventory[Player.selectedItem]);

	        return base.PreItemCheck();
        }
    }
}