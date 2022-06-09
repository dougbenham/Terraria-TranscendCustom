using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TranscendsCustomizations
{
	public class TranscendsCustomizations : Mod
	{
		public override uint ExtraPlayerBuffSlots => 200;
		
		public static ModHotKey TeleportKeybind { get; set; }

		public static ModHotKey CrazySpawnsKeybind { get; set; }

		public override void Load()
		{
			TeleportKeybind = RegisterHotKey("Teleport to cursor", "F");
			CrazySpawnsKeybind = RegisterHotKey("Farming mode", "OemOpenBrackets");
			
			if (Config.Instance.MaxCraftingRange)
				IL.Terraria.Player.AdjTiles += Player_AdjTiles;

			if (Config.Instance.MaxLifeAllowed != 500 ||
			    Config.Instance.MaxManaAllowed != 200)
			{
				IL.Terraria.Player.ItemCheck += Player_ItemCheck;
				IL.Terraria.Player.LoadPlayer += Player_LoadPlayer;

				if (Config.Instance.MaxManaAllowed != 200)
				{
					IL.Terraria.Player.Update += Player_Update;
				}
			}

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
								HookEndpointManager.Modify(killPlayerMethod, new ILContext.Manipulator(CalamityPlayer_KillPlayer));
							}

							break;
						}
					}
				}
			}
		}

		private void CalamityPlayer_KillPlayer(ILContext il)
		{
			var c = new ILCursor(il);
			c.Emit(OpCodes.Ret);
		}

		private void Player_LoadPlayer(ILContext il)
		{
			var spot = il.Method.ScanForOpcodePattern((i, instruction) => il.Instrs[i].Operand is FieldReference fr && fr.Name == "statLifeMax" &&
			                                                              (int) il.Instrs[i + 1].Operand == 500 &&
			                                                              (int) il.Instrs[i + 4].Operand == 500 &&
			                                                              il.Instrs[i + 5].Operand is FieldReference fr2 && fr2.Name == "statLifeMax",
				OpCodes.Ldfld,
				OpCodes.Ldc_I4,
				OpCodes.Ble_S,
				OpCodes.Ldloc_1,
				OpCodes.Ldc_I4,
				OpCodes.Stfld
			);

			if (spot >= 0)
			{
				il.Body.Instructions[spot + 1].Operand = Config.Instance.MaxLifeAllowed;
				il.Body.Instructions[spot + 4].Operand = Config.Instance.MaxLifeAllowed;
			}
			else
				Logger.Error("Could not patch Player_LoadPlayer for max life");

			var spot2 = il.Method.ScanForOpcodePattern((i, instruction) => il.Instrs[i].Operand is FieldReference fr && fr.Name == "statManaMax" &&
			                                                               (int) il.Instrs[i + 1].Operand == 200 &&
			                                                               (int) il.Instrs[i + 4].Operand == 200 &&
			                                                               il.Instrs[i + 5].Operand is FieldReference fr2 && fr2.Name == "statManaMax",
				OpCodes.Ldfld,
				OpCodes.Ldc_I4,
				OpCodes.Ble_S,
				OpCodes.Ldloc_1,
				OpCodes.Ldc_I4,
				OpCodes.Stfld
			);

			if (spot2 >= 0)
			{
				il.Body.Instructions[spot2 + 1].Operand = Config.Instance.MaxManaAllowed;
				il.Body.Instructions[spot2 + 4].Operand = Config.Instance.MaxManaAllowed;
			}
			else
				Logger.Error("Could not patch Player_LoadPlayer for max mana");

			var spot3 = il.Method.ScanForOpcodePattern((i, instruction) => il.Instrs[i].Operand is FieldReference fr && fr.Name == "statMana" &&
			                                                               (int) il.Instrs[i + 1].Operand == 400 &&
			                                                               (int) il.Instrs[i + 4].Operand == 400 &&
			                                                               il.Instrs[i + 5].Operand is FieldReference fr2 && fr2.Name == "statMana",
				OpCodes.Ldfld,
				OpCodes.Ldc_I4,
				OpCodes.Ble_S,
				OpCodes.Ldloc_1,
				OpCodes.Ldc_I4,
				OpCodes.Stfld
			);

			if (spot3 >= 0)
			{
				il.Body.Instructions[spot3 + 1].Operand = Config.Instance.MaxManaAllowed + 200;
				il.Body.Instructions[spot3 + 4].Operand = Config.Instance.MaxManaAllowed + 200;
			}
			else
				Logger.Error("Could not patch Player_LoadPlayer for max mana");
		}

		private void Player_ItemCheck(ILContext il)
		{
			var spot = il.Method.ScanForOpcodePattern((i, instruction) => il.Instrs[i].Operand is FieldReference fr && fr.Name == "statLifeMax" &&
			                                                              (int) il.Instrs[i + 1].Operand == 500,
				OpCodes.Ldfld,
				OpCodes.Ldc_I4
			);

			if (spot >= 0)
				il.Body.Instructions[spot + 1].Operand = Config.Instance.MaxLifeAllowed;
			else
				Logger.Error("Could not patch Player_ItemCheck");

			var spot2 = il.Method.ScanForOpcodePattern((i, instruction) => il.Instrs[i].Operand is FieldReference fr && fr.Name == "statManaMax" &&
			                                                              (int) il.Instrs[i + 1].Operand == 200,
				OpCodes.Ldfld,
				OpCodes.Ldc_I4
			);

			if (spot2 >= 0)
				il.Body.Instructions[spot2 + 1].Operand = Config.Instance.MaxManaAllowed;
			else
				Logger.Error("Could not patch Player_ItemCheck_UseManaCrystal");
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

		private void Player_Update(ILContext il)
		{
			var spot = il.Method.ScanForOpcodePattern((i, instruction) => il.Instrs[i].Operand is FieldReference fr && fr.Name == "statManaMax2" &&
			                                                              il.Instrs[i + 5].Operand is FieldReference fr2 && fr2.Name == "statManaMax2",
				OpCodes.Ldfld,
				OpCodes.Ldc_I4,
				OpCodes.Ble_S,
				OpCodes.Ldarg_0,
				OpCodes.Ldc_I4,
				OpCodes.Stfld
			);

			if (spot >= 0)
			{
				il.Body.Instructions[spot + 1].Operand =
					il.Body.Instructions[spot + 4].Operand = Config.Instance.MaxManaAllowed + 200;
			}
			else
				Logger.Error("Could not patch Player_Update for max mana");
		}

		public override void PostDrawFullscreenMap(ref string mouseText)
		{
			if (Config.Instance.MapTeleport && Main.mouseRight && Main.keyState.IsKeyUp(Keys.LeftControl))
			{
				var player = Main.player[Main.myPlayer];

				var target = Main.MouseScreen;
				target.X -= Main.screenWidth / 2;
				target.Y -= Main.screenHeight / 2;
				target /= Main.mapFullscreenScale;
				target += Main.mapFullscreenPos;
				target *= 16f;
				target.X -= player.width / 2;
				target.Y -= player.height / 2;

				var maxX = Main.maxTilesX * 16;
				if (target.X < 0f)
				{
					target.X = 0f;
				}
				else if (target.X + player.width > maxX)
				{
					target.X = maxX - player.width;
				}

				var maxY = Main.maxTilesY * 16;
				if (target.Y < 0f)
				{
					target.Y = 0f;
				}
				else if (target.Y + player.height > maxY)
				{
					target.Y = maxY - player.height;
				}

				player.Teleport(target, 1, 0);
				player.velocity = Vector2.Zero;
				NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, target.X, target.Y, 1, 0, 0);
			}
		}
	}
}