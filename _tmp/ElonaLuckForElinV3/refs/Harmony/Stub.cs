using System;using System.Reflection;using System.Reflection.Emit;
namespace HarmonyLib{
 public sealed class Harmony{public Harmony(string id){}public PatchClassProcessor CreateClassProcessor(Type t)=>new PatchClassProcessor();public void UnpatchSelf(){}}
 public sealed class PatchClassProcessor{public void Patch(){}}
 [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method,AllowMultiple=true)]public sealed class HarmonyPatch:Attribute{public HarmonyPatch(){}public HarmonyPatch(Type t,string n){}public HarmonyPatch(Type t,string n,Type[] a){}}
 public class CodeInstruction{public OpCode opcode;public object operand;public CodeInstruction(OpCode opcode){this.opcode=opcode;}public CodeInstruction(OpCode opcode,object operand){this.opcode=opcode;this.operand=operand;}}
}