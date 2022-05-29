using Microsoft.Xna.Framework.Input;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Terraria.ModLoader;

namespace TranscendCustom;

public class DougCustom : Mod
{
    public override uint ExtraPlayerBuffSlots => 200;

    public static ModKeybind TeleportKeybind { get; set; }

    public static ModKeybind CrazySpawnsKeybind { get; set; }

    public override void Load()
    {
        TeleportKeybind = KeybindLoader.RegisterKeybind(this, "Teleport to cursor", Keys.F);
        CrazySpawnsKeybind = KeybindLoader.RegisterKeybind(this, "Farming mode", Keys.OemOpenBrackets);

        if (Config.Instance.MaxCraftingRange)
            IL.Terraria.Player.AdjTiles += Player_AdjTiles;

        if (Config.Instance.MaxLifeAllowed != 500 ||
            Config.Instance.MaxManaAllowed != 200)
        {
            IL.Terraria.Player.LoadPlayer += Player_LoadPlayer;

            if (Config.Instance.MaxLifeAllowed != 500)
            {
                IL.Terraria.Player.ItemCheck_UseLifeFruit += Player_ItemCheck_UseLifeFruit;
            }

            if (Config.Instance.MaxManaAllowed != 200)
            {
                IL.Terraria.Player.Update += Player_Update;
                IL.Terraria.Player.ItemCheck_UseManaCrystal += Player_ItemCheck_UseManaCrystal;
            }
        }
    }

    private void Player_LoadPlayer(MonoMod.Cil.ILContext il)
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

    private void Player_ItemCheck_UseLifeFruit(MonoMod.Cil.ILContext il)
    {
        var spot = il.Method.ScanForOpcodePattern((i, instruction) => il.Instrs[i].Operand is FieldReference fr && fr.Name == "statLifeMax" &&
                                                                      (int) il.Instrs[i + 1].Operand == 500,
            OpCodes.Ldfld,
            OpCodes.Ldc_I4
        );
        
        if (spot >= 0)
            il.Body.Instructions[spot + 1].Operand = Config.Instance.MaxLifeAllowed;
        else
            Logger.Error("Could not patch Player_ItemCheck_UseLifeFruit");
    }

    private void Player_AdjTiles(MonoMod.Cil.ILContext il)
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

    private void Player_Update(MonoMod.Cil.ILContext il)
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

    private void Player_ItemCheck_UseManaCrystal(MonoMod.Cil.ILContext il)
    {
        var spot = il.Method.ScanForOpcodePattern((i, instruction) => il.Instrs[i].Operand is FieldReference fr && fr.Name == "statManaMax" &&
                                                                      (int) il.Instrs[i + 1].Operand == 200,
            OpCodes.Ldfld,
            OpCodes.Ldc_I4
        );

        if (spot >= 0)
            il.Body.Instructions[spot + 1].Operand = Config.Instance.MaxManaAllowed;
        else
            Logger.Error("Could not patch Player_ItemCheck_UseManaCrystal");
    }
}