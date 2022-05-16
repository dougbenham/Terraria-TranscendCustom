using Microsoft.Xna.Framework.Input;
using Mono.Cecil.Cil;
using Terraria.ModLoader;

namespace DougCustom;

public class DougCustom : Mod
{
    public override uint ExtraPlayerBuffSlots => 200;

    public static ModKeybind TeleportKeybind { get; set; }

    public static ModKeybind CrazySpawnsKeybind { get; set; }

    public override void Load()
    {
        TeleportKeybind = KeybindLoader.RegisterKeybind(this, "Teleport to cursor", Keys.F);
        CrazySpawnsKeybind = KeybindLoader.RegisterKeybind(this, "Farming mode", Keys.OemOpenBrackets);

        IL.Terraria.Player.AdjTiles += Player_AdjTiles;
    }

    private void Player_AdjTiles(MonoMod.Cil.ILContext il)
    {
        var spot = il.Method.ScanForOpcodePattern(OpCodes.Ldc_I4_4,
            OpCodes.Stloc_0,
            OpCodes.Ldc_I4_3,
            OpCodes.Stloc_1
        );

        il.Body.Instructions[spot].OpCode = OpCodes.Ldc_I4;
        il.Body.Instructions[spot].Operand = 30;
        il.Body.Instructions[spot + 2].OpCode = OpCodes.Ldc_I4;
        il.Body.Instructions[spot + 2].Operand = 30;
    }
}