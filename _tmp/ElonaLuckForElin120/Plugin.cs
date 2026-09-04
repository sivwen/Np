using BepInEx;using BepInEx.Configuration;using HarmonyLib;using System;using System.Collections.Generic;
namespace ElonaLuckForElin{
[BepInPlugin(G,N,V)]public sealed class Plugin:BaseUnityPlugin{
public const string G="sivwen.elin.elonaluck",N="Elona Luck for Elin",V="1.2.0";
internal static ConfigEntry<bool>Q=null!,D=null!,EC=null!,ET=null!,EV=null!,Craft=null!,Ammo=null!,Log=null!;
internal static ConfigEntry<int>QD=null!,CountDiv=null!,CountCap=null!,TierDiv=null!,TierCap=null!,ValueDiv=null!,ValueCap=null!;
internal static Plugin I=null!; Harmony? h;
void Awake(){I=this;
Q=Config.Bind("Elona Luck","EnableEquipmentQualityLuck",true,"Elona-style one-tier equipment quality upgrade.");
D=Config.Bind("Elona Luck","ReplaceElinDiceLuck",true,"Disable Elin global Luck dice rerolls.");
QD=Config.Bind("Elona Luck","EquipmentLuckDenominator",5000,"Elona default: 5000.");
EC=Config.Bind("Enchant Luck","EnableEnchantCountLuck",true,"Luck can add extra random enchants.");
CountDiv=Config.Bind("Enchant Luck","EnchantCountLuckDivisor",250,"Each divisor points of Luck adds 1% extra-enchant chance.");
CountCap=Config.Bind("Enchant Luck","EnchantCountChanceCapPercent",35,"Maximum chance for each extra-enchant roll.");
ET=Config.Bind("Enchant Luck","EnableEnchantTierLuck",true,"Luck raises virtual generation level for bonus enchants.");
TierDiv=Config.Bind("Enchant Luck","EnchantTierLuckDivisor",20,"Luck/divisor is added to generation level.");
TierCap=Config.Bind("Enchant Luck","EnchantTierLevelBonusCap",150,"Maximum virtual generation-level bonus.");
EV=Config.Bind("Enchant Luck","EnableEnchantValueLuck",true,"Luck boosts newly generated random enchant values.");
ValueDiv=Config.Bind("Enchant Luck","EnchantValueLuckDivisor",1000,"Luck/divisor is fractional value bonus; 1000 Luck = +100% before cap.");
ValueCap=Config.Bind("Enchant Luck","EnchantValueBonusCapPercent",100,"Maximum enchant-value bonus percent.");
Craft=Config.Bind("Compatibility","ApplyToCraftedEquipment",false,"Include crafted equipment.");
Ammo=Config.Bind("Compatibility","IncludeAmmo",false,"Include ammo.");
Log=Config.Bind("Diagnostics","DebugLogging",false,"Log Luck upgrades.");
h=new Harmony(G);h.PatchAll();Logger.LogInfo(N+" "+V+" loaded.");}
void OnDestroy(){h?.UnpatchSelf();}
internal static int Luck(){int l=EClass.pc==null?1:EClass.pc.Evalue(78);if(l<1)l=1;if(l>9999)l=9999;return l;}
internal static bool Eligible(Thing x){return x.IsEquipmentOrRangedOrAmmo&&(!x.IsAmmo||Ammo.Value)&&(x.bp==null||!x.bp.isCraft||Craft.Value)&&(x.sourceCard==null||x.sourceCard.quality==0)&&!x.HasTag(CTAG.noRandomEnc);}
internal static void Info(string s){if(Log.Value)I.Logger.LogInfo(s);}
}
[HarmonyPatch(typeof(Thing),nameof(Thing.OnCreate),new Type[]{typeof(int)})]static class ThingCreateLuckPatch{
public sealed class Snap{public readonly Dictionary<int,int>B=new();}
static void Prefix(Thing __instance,out Snap __state){__state=new Snap();if(__instance.elements!=null)foreach(var kv in __instance.elements.dict)__state.B[kv.Key]=kv.Value.vBase;}
static void Postfix(Thing __instance,int genLv,Snap __state){
var x=__instance;if(!Plugin.Eligible(x))return;int luck=Plugin.Luck();
var touched=new List<Element>();if(x.elements!=null)foreach(var kv in x.elements.dict){int old=__state.B.TryGetValue(kv.Key,out var v)?v:0;if(kv.Value.vBase!=old)touched.Add(kv.Value);}
if(Plugin.EV.Value){int pct=Math.Min(Plugin.ValueCap.Value,Math.Max(0,luck*100/Math.Max(1,Plugin.ValueDiv.Value)));if(pct>0)foreach(var e in touched){int add=Math.Abs(e.vBase)*pct/100;if(add<1&&e.vBase!=0)add=1;x.elements.ModBase(e.id,e.vBase>=0?add:-add);}}
if(Plugin.EC.Value){int chance=Math.Min(Plugin.CountCap.Value,Math.Max(0,luck/Math.Max(1,Plugin.CountDiv.Value)));int rolls=1+luck/2500;if(rolls>4)rolls=4;int lv=genLv;if(Plugin.ET.Value)lv+=Math.Min(Plugin.TierCap.Value,Math.Max(0,luck/Math.Max(1,Plugin.TierDiv.Value)));for(int i=0;i<rolls;i++){if(chance>0&&EClass.rnd(100)<chance){var e=x.AddEnchant(lv);if(e!=null&&Plugin.EV.Value){int pct=Math.Min(Plugin.ValueCap.Value,Math.Max(0,luck*100/Math.Max(1,Plugin.ValueDiv.Value)));int add=Math.Abs(e.vBase)*pct/100;if(add<1&&e.vBase!=0)add=1;x.elements.ModBase(e.id,e.vBase>=0?add:-add);}}}}
if(Plugin.Q.Value){var a=x.rarity;if(a>=Rarity.Crude&&a<Rarity.Mythical){int d=Math.Max(1,Plugin.QD.Value);if(luck>=d||EClass.rnd(d)<luck){Rarity b=a switch{Rarity.Crude=>Rarity.Normal,Rarity.Normal=>Rarity.Superior,Rarity.Superior=>Rarity.Legendary,Rarity.Legendary=>Rarity.Mythical,_=>a};if(b!=a){x.rarity=b;x.ApplyMaterial(true);x.ApplyMaterial(false);Plugin.Info($"Luck {luck}: {x.id} rarity {a}->{b}");}}}}
}}
[HarmonyPatch(typeof(Dice),nameof(Dice.Roll),new Type[]{typeof(int),typeof(int),typeof(int),typeof(Card)})]static class DicePatch{static void Prefix(ref Card? card){if(Plugin.D.Value)card=null;}}
}