using System;
using Mono.Cecil.Cil;

namespace TranscendsCustomizations
{
    public static class ILHelpers
    {
        public static int ScanForOpcodePattern(this ILProcessor il, params OpCode[] inst)
        {
            return ScanForOpcodePattern(il, (v, i) => true, 0, inst);
        }

        public static int ScanForOpcodePattern(this ILProcessor il, Func<int, Instruction, bool> check, params OpCode[] inst)
        {
            return ScanForOpcodePattern(il, check, 0, inst);
        }

        public static int ScanForOpcodePattern(this ILProcessor il, Func<int, Instruction, bool> check, int nStartOffset, params OpCode[] inst)
        {
            for (var x = nStartOffset; x < il.Body.Instructions.Count - inst.Length; x++)
            {
                if (il.Body.Instructions[x].OpCode != inst[0])
                    continue;

                for (var y = 0; y < inst.Length; y++)
                {
                    if (il.Body.Instructions[x + y].OpCode != inst[y])
                        break;
                    if (y == inst.Length - 1 && check(x, il.Body.Instructions[x]))
                        return x;
                }
            }

            return -1;
        }
    }
}