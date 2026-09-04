using BepInEx;using BepInEx.Configuration;using HarmonyLib;using System;using System.Collections.Generic;using System.Diagnostics;
namespace ElonaLuckForElin{
[BepInPlugin(G,N,V)]public sealed class Plugin:BaseUnityPlugin{
public const string G="sivwen.elin.elonaluck",N="Elona Luck for Elin",V="1.3.0";
internal static ConfigEntry<bool>Q=null!,D=null!,EC=null!,ET=null!,EV=null!,Craft=null!,Ammo=null!,Drop=null!,StealWeight=null!,Log=null!;
internal static ConfigEntry<int>QD=null!,CountDiv=null!,CountCap=null!,TierDiv=null!,TierCap=null!,ValueDiv=null!,ValueCap=null!,DropDiv=null!,DropCap=null!,StealDiv=null!,StealCap=null!;
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
Drop=Config.Bind("Drop Luck","EnableGeneralDropLuck",true,"Luck improves common SpawnLoot chance() checks.");
DropDiv=Config.Bind("Drop Luck","DropLuckPerPercentDivisor",50,"Each divisor points of Luck adds 1% effective drop-rate bonus.");
DropCap=Config.Bind("Drop Luck","DropRateBonusCapPercent",100,"Maximum effective drop-rate bonus.");
StealWeight=Config.Bind("Steal Luck","EnableStealWeightBypassLuck",true,"Luck can bypass stealing weight limit.");
StealDiv=Config.Bind("Steal Luck","StealWeightLuckDivisor",20,"Base bypass chance is Luck/divisor percent before overweight penalty.");
StealCap=Config.Bind("Steal Luck","StealWeightBypassCapPercent",75,"Maximum bypass chance before overweight penalty.");
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

[HarmonyPatch(typeof(EClass),nameof(EClass.rnd),new Type[]{typeof(int)})]static class DropChanceLuckPatch{
static void Prefix(ref int a){
if(!Plugin.Drop.Value||a<=1)return;
var st=new StackTrace(1,false);bool ok=false;
var fs=st.GetFrames();if(fs!=null)for(int i=0;i<fs.Length&&i<8;i++){var m=fs[i].GetMethod();var dt=m?.DeclaringType;var n=m?.Name??"";var dn=dt?.FullName??"";if(n.Contains("g__chance")&&dn.Contains("Card")){ok=true;break;}if(n.Contains("SpawnLoot")&&dn.Contains("Card")){ok=true;break;}}
if(!ok)return;
int luck=Plugin.Luck();int bonus=Math.Min(Plugin.DropCap.Value,Math.Max(0,luck/Math.Max(1,Plugin.DropDiv.Value)));if(bonus<=0)return;
long v=(long)a*100L;int na=(int)((v+99+bonus)/(100+bonus));if(na<1)na=1;if(na<a)a=na;
}}

[HarmonyPatch(typeof(Card),"get_ChildrenAndSelfWeight")]static class StealWeightLuckPatch{
static readonly HashSet<int> passed=new HashSet<int>();
static void Postfix(Card __instance,ref int __result){
if(!Plugin.StealWeight.Value||__result<=0||EClass.pc==null)return;
bool inSteal=false;var st=new StackTrace(1,false);var fs=st.GetFrames();if(fs!=null)for(int i=0;i<fs.Length&&i<12;i++){var m=fs[i].GetMethod();var dn=m?.DeclaringType?.FullName??"";var n=m?.Name??"";if(dn.Contains("AI_Steal")&&(n.Contains("Run")||n.Contains("b__")||n=="MoveNext")){inSteal=true;break;}}
if(!inSteal)return;
int limit=EClass.pc.Evalue(281)*200+EClass.pc.STR*100+1000;if(__result<=limit)return;
if(passed.Contains(__instance.uid)){__result=0;return;}
int luck=Plugin.Luck();int chance=Math.Min(Plugin.StealCap.Value,Math.Max(0,luck/Math.Max(1,Plugin.StealDiv.Value)));if(chance<=0)return;
chance=(int)((long)chance*Math.Max(1,limit)/Math.Max(1,__result));if(chance<1)chance=1;
if(EClass.rnd(100)<chance){passed.Add(__instance.uid);__result=0;Plugin.Info($"Luck {luck}: bypassed steal weight limit for uid {__instance.uid} ({chance}%)");}
}}
}