using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria.ModLoader;

namespace TranscendsCustomizations
{
	public class TranscendsCustomizations : Mod
	{
		public override uint ExtraPlayerBuffSlots => 200;
		
		public static ModKeybind TeleportKeybind { get; set; }

		public static ModKeybind CrazySpawnsKeybind { get; set; }

		public override void Load()
		{
			TeleportKeybind = KeybindLoader.RegisterKeybind(this, "Teleport to cursor", "F");
			CrazySpawnsKeybind = KeybindLoader.RegisterKeybind(this, "Farming mode", "OemOpenBrackets");
			
			if (Config.Instance.MaxCraftingRange)
				Terraria.IL_Player.AdjTiles += Player_AdjTiles;

            Terraria.DataStructures.On_TileReachCheckSettings.GetRanges += On_TileReachCheckSettings_GetRanges;
			
			if (Config.Instance.DemigodMode)
			{
				var calamity = ModLoader.GetMod("CalamityMod");
				if (calamity != null)
				{
					foreach (var t in calamity.Code.GetTypes())
					{
						if (t.Name == "CalamityPlayer")
						{
							var killPlayerMethod = t.GetMethod("KillPlayer", BindingFlags.Instance | BindingFlags.Public);
							if (killPlayerMethod != null)
							{
								MonoModHooks.Modify(killPlayerMethod, new ILContext.Manipulator(CalamityPlayer_KillPlayer));
							}

							break;
						}
					}
				}
			}
		}

        private void On_TileReachCheckSettings_GetRanges(Terraria.DataStructures.On_TileReachCheckSettings.orig_GetRanges orig, ref Terraria.DataStructures.TileReachCheckSettings self, Terraria.Player player, out int x, out int y)
        {
            if (Config.Instance.MaxReachRange)
	            self.TileReachLimit = null;
            orig(ref self, player, out x, out y);
        }

        private void CalamityPlayer_KillPlayer(ILContext il)
		{
			var c = new ILCursor(il);
			c.Emit(OpCodes.Ret);
		}

		private void Player_AdjTiles(ILContext il)
		{
			var spot = il.Method.ScanForOpcodePattern(OpCodes.Ldc_I4_4,
				OpCodes.Stloc_0,
				OpCodes.Ldc_I4_3,
				OpCodes.Stloc_1
			);

			if (spot >= 0)
			{
				il.Body.Instructions[spot].OpCode = OpCodes.Ldc_I4;
				il.Body.Instructions[spot].Operand = 30;
				il.Body.Instructions[spot + 2].OpCode = OpCodes.Ldc_I4;
				il.Body.Instructions[spot + 2].Operand = 30;
			}
			else
				Logger.Error("Could not patch Player_AdjTiles");
		}
	}
}