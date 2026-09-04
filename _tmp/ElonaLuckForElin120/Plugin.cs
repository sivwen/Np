using BepInEx;using BepInEx.Configuration;using HarmonyLib;using System;using System.Collections.Generic;using System.Diagnostics;
namespace ElonaLuckForElin{
[BepInPlugin(G,N,V)]public sealed class Plugin:BaseUnityPlugin{
public const string G="sivwen.elin.elonaluck",N="Elona Luck for Elin",V="1.7.0";
internal static ConfigEntry<bool>Q=null!,D=null!,EC=null!,ET=null!,EV=null!,Craft=null!,Ammo=null!,Drop=null!,StealWeight=null!,WitnessLuck=null!,LockLuck=null!,FishLuck=null!,DismantleLuck=null!,ActivityBonus=null!,BonusMine=null!,BonusDig=null!,BonusHarvest=null!,BonusFish=null!,BonusCraft=null!,AutoDisableSALM=null!,NestLuck=null!,SeedLuck=null!,TreasureLuck=null!,ScratchLuck=null!,GachaLuck=null!,CasinoLuck=null!,Log=null!;
internal static ConfigEntry<int>QD=null!,CountDiv=null!,CountCap=null!,TierDiv=null!,TierCap=null!,ValueDiv=null!,ValueCap=null!,DropDiv=null!,DropCap=null!,StealDiv=null!,StealCap=null!,WitnessDiv=null!,WitnessCap=null!,LockDiv=null!,LockCap=null!,FishDiv=null!,FishCap=null!,DismantleDiv=null!,DismantleCap=null!,ActSkillWeight=null!,ActLuckWeight=null!,ActSkillMod=null!,ActLuckMod=null!,NestDiv=null!,NestCap=null!,SeedDiv=null!,SeedCap=null!,TreasureDiv=null!,TreasureCap=null!,TreasureMythDiv=null!,ScratchDiv=null!,ScratchCap=null!,GachaDiv=null!,GachaCap=null!,CasinoDiv=null!,CasinoCap=null!;
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
WitnessLuck=Config.Bind("Crime Luck","EnableWitnessAvoidanceLuck",true,"Luck can make witnesses fail to notice PC crimes, including stealing.");
WitnessDiv=Config.Bind("Crime Luck","WitnessAvoidLuckDivisor",25,"Base witness-avoidance chance is Luck/divisor percent.");
WitnessCap=Config.Bind("Crime Luck","WitnessAvoidChanceCapPercent",60,"Maximum witness-avoidance chance.");
LockLuck=Config.Bind("Lock Luck","EnableLockpickLuck",true,"Luck can reduce effective lock level for a lockpicking attempt.");
LockDiv=Config.Bind("Lock Luck","LockLuckDivisor",20,"Effective lock-level reduction is Luck/divisor.");
LockCap=Config.Bind("Lock Luck","LockLevelReductionCap",100,"Maximum effective lock-level reduction.");
FishLuck=Config.Bind("Fishing Luck","EnableFishingQualityLuck",true,"Luck can increase caught fish tier by one.");
FishDiv=Config.Bind("Fishing Luck","FishingTierLuckDivisor",25,"Fish tier-up chance is Luck/divisor percent.");
FishCap=Config.Bind("Fishing Luck","FishingTierChanceCapPercent",50,"Maximum fish tier-up chance.");
DismantleLuck=Config.Bind("Harvest Luck","EnableDismantleYieldLuck",true,"Luck improves fractional dismantle material recovery.");
DismantleDiv=Config.Bind("Harvest Luck","DismantleYieldLuckDivisor",50,"Each divisor Luck adds 1% effective fractional recovery chance.");
DismantleCap=Config.Bind("Harvest Luck","DismantleYieldBonusCapPercent",100,"Maximum effective fractional recovery bonus.");
ActivityBonus=Config.Bind("SkillAndLuckMatter Replacement","EnableActivityBonusRolls",true,"Enable SkillAndLuckMatter-style activity bonus rolls.");
BonusMine=Config.Bind("SkillAndLuckMatter Replacement","BonusMining",true,"Apply bonus rolls to mining.");
BonusDig=Config.Bind("SkillAndLuckMatter Replacement","BonusDigging",true,"Apply bonus rolls to digging.");
BonusHarvest=Config.Bind("SkillAndLuckMatter Replacement","BonusHarvestGatherReap",true,"Apply bonus rolls to harvesting/gathering/reaping/chopping.");
BonusFish=Config.Bind("SkillAndLuckMatter Replacement","BonusFishing",true,"Apply bonus rolls to fishing.");
BonusCraft=Config.Bind("SkillAndLuckMatter Replacement","BonusCraftingProcessing",true,"Refund crafting materials using the SkillAndLuckMatter curve.");
AutoDisableSALM=Config.Bind("SkillAndLuckMatter Replacement","AutoDisableWhenOriginalDetected",true,"Disable replacement bonus rolls when original SkillAndLuckMatter assembly is loaded, preventing double rewards.");
ActSkillWeight=Config.Bind("SkillAndLuckMatter Replacement","SkillWeight",3,"Original default skill weight.");
ActLuckWeight=Config.Bind("SkillAndLuckMatter Replacement","LuckWeight",2,"Original default luck weight.");
ActSkillMod=Config.Bind("SkillAndLuckMatter Replacement","SkillLevelModifierPercent",100,"Original default modifier: 100%.");
ActLuckMod=Config.Bind("SkillAndLuckMatter Replacement","LuckLevelModifierPercent",100,"Original default modifier: 100%.");
NestLuck=Config.Bind("Rare Outcome Luck","EnableFertilizedEggLuck",true,"Luck improves fertilized egg chance from searchable bird nests.");
NestDiv=Config.Bind("Rare Outcome Luck","FertilizedEggLuckDivisor",50,"Extra fertilized-egg reroll chance is Luck/divisor percent.");
NestCap=Config.Bind("Rare Outcome Luck","FertilizedEggExtraChanceCapPercent",40,"Maximum extra fertilized-egg reroll chance.");
SeedLuck=Config.Bind("Rare Outcome Luck","EnableSeedRecoveryLuck",true,"Luck improves manual seed recovery checks.");
SeedDiv=Config.Bind("Rare Outcome Luck","SeedRecoveryLuckDivisor",50,"Each divisor Luck adds roughly 1% effective seed-check strength.");
SeedCap=Config.Bind("Rare Outcome Luck","SeedRecoveryBonusCapPercent",200,"Maximum effective seed-check strength bonus.");
TreasureLuck=Config.Bind("Rare Outcome Luck","EnableTreasureRarityLuck",true,"Luck improves equipment rarity generated inside treasure chests.");
TreasureDiv=Config.Bind("Rare Outcome Luck","TreasureLegendaryLuckDivisor",50,"Luck/divisor is subtracted from the chest rarity roll.");
TreasureCap=Config.Bind("Rare Outcome Luck","TreasureLegendaryRollReductionCap",50,"Maximum reduction on the 0-99 treasure rarity roll.");
TreasureMythDiv=Config.Bind("Rare Outcome Luck","TreasureMythicalLuckDivisor",500,"Each divisor Luck reduces the Mythical 1/N denominator by 1, minimum N=5.");
ScratchLuck=Config.Bind("Rare Outcome Luck","EnableScratchPrizeLuck",true,"Luck improves high-tier scratch prize checks.");
ScratchDiv=Config.Bind("Rare Outcome Luck","ScratchLuckDivisor",50,"Each divisor Luck adds 1% effective chance to scratch prize checks.");
ScratchCap=Config.Bind("Rare Outcome Luck","ScratchChanceBonusCapPercent",150,"Maximum effective scratch prize chance bonus.");
GachaLuck=Config.Bind("Gacha Luck","EnableGachaBestOfLuck",true,"Luck adds best-of candidate rolls to character and item gacha.");
GachaDiv=Config.Bind("Gacha Luck","GachaLuckPerExtraCandidate",500,"Each divisor Luck adds one extra candidate.");
GachaCap=Config.Bind("Gacha Luck","GachaExtraCandidateCap",5,"Maximum extra candidates.");
CasinoLuck=Config.Bind("Casino Luck","EnableCasinoPayoutLuck",true,"Luck can grant a bonus on positive net casino winnings at minigame settlement.");
CasinoDiv=Config.Bind("Casino Luck","CasinoBonusChanceLuckDivisor",25,"Bonus payout chance is Luck/divisor percent.");
CasinoCap=Config.Bind("Casino Luck","CasinoBonusChanceCapPercent",50,"Maximum bonus payout chance.");
Craft=Config.Bind("Compatibility","ApplyToCraftedEquipment",false,"Include crafted equipment.");
Ammo=Config.Bind("Compatibility","IncludeAmmo",false,"Include ammo.");
Log=Config.Bind("Diagnostics","DebugLogging",false,"Log Luck upgrades.");
h=new Harmony(G);h.PatchAll();Logger.LogInfo(N+" "+V+" loaded.");}
void OnDestroy(){h?.UnpatchSelf();}
internal static int Luck(){int l=EClass.pc==null?1:EClass.pc.Evalue(78);if(l<1)l=1;if(l>9999)l=9999;return l;}
internal static bool Eligible(Thing x){return x.IsEquipmentOrRangedOrAmmo&&(!x.IsAmmo||Ammo.Value)&&(x.bp==null||!x.bp.isCraft||Craft.Value)&&(x.sourceCard==null||x.sourceCard.quality==0)&&!x.HasTag(CTAG.noRandomEnc);}
internal static void Info(string s){if(Log.Value)I.Logger.LogInfo(s);}
internal static bool OriginalSALMLoaded(){foreach(var a in AppDomain.CurrentDomain.GetAssemblies()){string n=a.GetName().Name??"";if(n.IndexOf("SkillAndLuckMatter",StringComparison.OrdinalIgnoreCase)>=0)return true;}return false;}
internal static bool ActivityEnabled(){return ActivityBonus.Value&&(!AutoDisableSALM.Value||!OriginalSALMLoaded());}
internal static double ActivityScore(int skill){
int sw=Math.Max(0,ActSkillWeight.Value),lw=Math.Max(0,ActLuckWeight.Value);int den=sw+lw;if(den<=0)return 0;
double sv=Math.Max(0,skill)*(Math.Max(0,ActSkillMod.Value)/100.0);double lv=Luck()*(Math.Max(0,ActLuckMod.Value)/100.0);
return (sv*sw+lv*lw)/den;
}
internal static double BonusChance(double x){
if(x<=0)return 0;if(x<10)return x/10.0*0.15;if(x<40)return 0.15+(x-10)/30.0*0.35;if(x<100)return 0.50+(x-40)/60.0*0.25;if(x<200)return 0.75+(x-100)/100.0*0.25;return 1.0+(x-200)/100.0;
}
internal static int BonusRolls(int skill,bool crafting=false){
double p=BonusChance(ActivityScore(skill));if(crafting)p/=10.0;int n=(int)Math.Floor(p);double f=p-n;if(f>0&&EClass.rnd(100000)<(int)(f*100000.0))n++;return n;
}
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
[HarmonyPatch(typeof(Point),nameof(Point.TryWitnessCrime),new Type[]{typeof(Chara),typeof(Chara),typeof(int),typeof(Func<Chara,bool>)})]static class WitnessLuckPatch{
static void Prefix(Chara criminal,ref Func<Chara,bool> funcWitness){
if(!Plugin.WitnessLuck.Value||criminal==null||EClass.pc==null||criminal!=EClass.pc)return;
int avoid=Math.Min(Plugin.WitnessCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.WitnessDiv.Value)));if(avoid<=0)return;
var orig=funcWitness;
if(orig==null)funcWitness=(Chara c)=>EClass.rnd(10)==0&&EClass.rnd(100)>=avoid;
else funcWitness=(Chara c)=>orig(c)&&EClass.rnd(100)>=avoid;
}}

[HarmonyPatch(typeof(Trait),nameof(Trait.TryOpenLock),new Type[]{typeof(Chara),typeof(bool)})]static class LockLuckPatch{
public sealed class S{public int lv;public bool changed;}
static void Prefix(Trait __instance,Chara cc,out S __state){
__state=new S();if(!Plugin.LockLuck.Value||cc==null||!cc.IsPC||__instance.owner==null)return;
int cut=Math.Min(Plugin.LockCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.LockDiv.Value)));if(cut<=0)return;
__state.lv=__instance.owner.c_lockLv;int nl=Math.Max(0,__state.lv-cut);if(nl<__state.lv){__instance.owner.c_lockLv=nl;__state.changed=true;}
}
static void Postfix(Trait __instance,LockOpenState __result,S __state){if(__state!=null&&__state.changed&&__result!=LockOpenState.Success)__instance.owner.c_lockLv=__state.lv;}
}

[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]static class FishingLuckPatch{
static void Postfix(Chara c,ref Thing __result){
if(!Plugin.FishLuck.Value||c==null||!c.IsPC||__result==null||__result.category==null||!__result.category.IsChildOf("fish"))return;
int chance=Math.Min(Plugin.FishCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.FishDiv.Value)));if(chance<=0||__result.tier>=3)return;
if(EClass.rnd(100)<chance){int before=__result.tier;__result.SetTier(Math.Min(3,before+1));Plugin.Info($"Luck {Plugin.Luck()}: fishing tier {before}->{__result.tier}");}
}}

[HarmonyPatch(typeof(EClass),nameof(EClass.rndf),new Type[]{typeof(float)})]static class DismantleLuckPatch{
static void Prefix(ref float a){
if(!Plugin.DismantleLuck.Value||a<=1f)return;
var st=new StackTrace(1,false);bool ok=false;var fs=st.GetFrames();
if(fs!=null)for(int i=0;i<fs.Length&&i<10;i++){var m=fs[i].GetMethod();var dn=m?.DeclaringType?.FullName??"";if(dn.Contains("TaskHarvest")&&(m?.Name??"").Contains("HarvestThing")){ok=true;break;}}
if(!ok)return;
int bonus=Math.Min(Plugin.DismantleCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.DismantleDiv.Value)));if(bonus<=0)return;
a=Math.Max(1f,a*100f/(100f+bonus));
}}
[HarmonyPatch(typeof(Map),nameof(Map.TrySmoothPick),new Type[]{typeof(Point),typeof(Thing),typeof(Chara)})]static class ActivityDropBonusPatch{
static void Prefix(Thing t,Chara c){
if(!Plugin.ActivityEnabled()||t==null||c==null||!c.IsPC)return;
var fs=new StackTrace(1,false).GetFrames();if(fs==null)return;int skill=-1;bool enabled=false;
for(int i=0;i<fs.Length&&i<14;i++){string dn=fs[i].GetMethod()?.DeclaringType?.FullName??"";
if(dn.Contains("TaskMine")){skill=220;enabled=Plugin.BonusMine.Value;break;}
if(dn.Contains("TaskDig")){skill=230;enabled=Plugin.BonusDig.Value;break;}
if(dn.Contains("TaskChopWood")){skill=225;enabled=Plugin.BonusHarvest.Value;break;}
if(dn.Contains("TaskHarvest")||dn.Contains("GrowSystem")){skill=Math.Max(c.Evalue(250),c.Evalue(286));enabled=Plugin.BonusHarvest.Value;break;}}
if(!enabled||skill<0)return;int rolls=Plugin.BonusRolls(skill);if(rolls<=0)return;int baseNum=Math.Max(1,t.Num);t.ModNum(baseNum*rolls);Plugin.Info($"Activity bonus x{rolls}: {t.id}");
}}

[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]static class ActivityFishingBonusPatch{
static void Postfix(Chara c,ref Thing __result){
if(!Plugin.ActivityEnabled()||!Plugin.BonusFish.Value||c==null||!c.IsPC||__result==null)return;
int rolls=Plugin.BonusRolls(c.Evalue(245));if(rolls<=0)return;int baseNum=Math.Max(1,__result.Num);__result.ModNum(baseNum*rolls);Plugin.Info($"Fishing activity bonus x{rolls}: {__result.id}");
}}

static class CraftRefundCore{
internal static List<Thing>? Prepare(Recipe r,List<Thing> ings,bool model){
if(!Plugin.ActivityEnabled()||!Plugin.BonusCraft.Value||model||r==null||ings==null||EClass.pc==null)return null;
Element e=r.source?.GetReqSkill();int skill=e==null?0:EClass.pc.Evalue(e.id);int rolls=Plugin.BonusRolls(skill,true);if(rolls<=0)return null;
var list=new List<Thing>();foreach(var ing in ings){if(ing==null)continue;int n=Math.Max(1,ing.Num)*rolls;Thing d=ing.Duplicate(n);if(d!=null)list.Add(d);}return list;
}
internal static void Refund(List<Thing>? list){if(list==null||EClass.pc==null)return;foreach(var t in list)EClass.pc.AddCard(t);Plugin.Info($"Crafting material refund: {list.Count} stacks");}
}
[HarmonyPatch(typeof(Recipe),nameof(Recipe.Craft),new Type[]{typeof(BlessedState),typeof(bool),typeof(List<Thing>),typeof(TraitCrafter),typeof(bool)})]static class RecipeCraftRefundPatch{
static void Prefix(Recipe __instance,List<Thing> ings,bool model,out List<Thing>? __state){__state=CraftRefundCore.Prepare(__instance,ings,model);}
static void Postfix(List<Thing>? __state,Thing __result){if(__result!=null)CraftRefundCore.Refund(__state);}
}
[HarmonyPatch(typeof(RecipeCard),nameof(RecipeCard.Craft),new Type[]{typeof(BlessedState),typeof(bool),typeof(List<Thing>),typeof(TraitCrafter),typeof(bool)})]static class RecipeCardCraftRefundPatch{
static void Prefix(RecipeCard __instance,List<Thing> ings,bool model,out List<Thing>? __state){__state=CraftRefundCore.Prepare(__instance,ings,model);}
static void Postfix(List<Thing>? __state,Thing __result){if(__result!=null)CraftRefundCore.Refund(__state);}
}

static class RareOutcomeContext{
[ThreadStatic] internal static int seedDepth;
[ThreadStatic] internal static int seedRndIndex;
internal static int PercentBonus(ConfigEntry<int> div,ConfigEntry<int> cap){return Math.Min(Math.Max(0,cap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,div.Value)));}
}

[HarmonyPatch(typeof(GrowSystem),nameof(GrowSystem.TryPopSeed),new Type[]{typeof(Chara)})]static class SeedLuckContextPatch{
static void Prefix(){if(Plugin.SeedLuck.Value){RareOutcomeContext.seedDepth++;RareOutcomeContext.seedRndIndex=0;}}
static Exception? Finalizer(Exception? __exception){if(Plugin.SeedLuck.Value&&RareOutcomeContext.seedDepth>0)RareOutcomeContext.seedDepth--;return __exception;}
}

[HarmonyPatch(typeof(EClass),nameof(EClass.rnd),new Type[]{typeof(int)})]static class RareOutcomeRndPatch{
static void Prefix(ref int a){
if(a<=1)return;
var fs=new StackTrace(1,false).GetFrames();if(fs==null)return;
bool scratch=false,treasureSet=false;
for(int i=0;i<fs.Length&&i<12;i++){
 var m=fs[i].GetMethod();string dn=m?.DeclaringType?.FullName??"";string n=m?.Name??"";
 if(Plugin.ScratchLuck.Value&&dn.Contains("TraitCrafter")&&n.Contains("g__Prize"))scratch=true;
 if(Plugin.TreasureLuck.Value&&dn.Contains("ThingGen")&&n.Contains("g__SetRarity"))treasureSet=true;
 if(Plugin.NestLuck.Value&&dn.Contains("SurvivalManager")&&n.Contains("OnMineWreck")&&a==10){
   int bonus=RareOutcomeContext.PercentBonus(Plugin.NestDiv,Plugin.NestCap);
   if(bonus>0){int na=(int)(((long)a*100+99+bonus)/(100+bonus));if(na<2)na=2;a=na;}
   return;
 }
}
if(scratch){
 int bonus=RareOutcomeContext.PercentBonus(Plugin.ScratchDiv,Plugin.ScratchCap);
 if(bonus>0){int na=(int)(((long)a*100+99+bonus)/(100+bonus));if(na<1)na=1;if(na<a)a=na;}return;
}
if(treasureSet&&a==20){
 int cut=Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.TreasureMythDiv.Value));a=Math.Max(5,a-cut);return;
}
if(Plugin.SeedLuck.Value&&RareOutcomeContext.seedDepth>0){
 RareOutcomeContext.seedRndIndex++;
 if(RareOutcomeContext.seedRndIndex==1){
   int bonus=RareOutcomeContext.PercentBonus(Plugin.SeedDiv,Plugin.SeedCap);
   if(bonus>0){int na=(int)(((long)a*100+99+bonus)/(100+bonus));if(na<1)na=1;a=na;}
 }
}
}
static void Postfix(int a,ref int __result){
if(!Plugin.TreasureLuck.Value||a!=100)return;
var fs=new StackTrace(1,false).GetFrames();if(fs==null)return;
for(int i=0;i<fs.Length&&i<10;i++){var m=fs[i].GetMethod();string dn=m?.DeclaringType?.FullName??"";string n=m?.Name??"";if(dn.Contains("ThingGen")&&n.Contains("g__SetRarity")){
 int cut=Math.Min(Math.Max(0,Plugin.TreasureCap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.TreasureDiv.Value)));
 __result=Math.Max(0,__result-cut);return;
}}
}
}

static class GachaLuckCore{
[ThreadStatic] internal static bool rerolling;
internal static int Extra(){return Math.Min(Math.Max(0,Plugin.GachaCap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.GachaDiv.Value)));}
internal static long Score(CardRow r){if(r==null)return long.MinValue;long v=0;try{v=((long)r.LV*100000L)-Math.Max(1,r.chance);}catch{}try{var st=r as SourceThing.Row;if(st!=null)v+=(long)st.value*100L;}catch{}return v;}
internal static long Score(Chara c){if(c==null||c.source==null)return long.MinValue;long q=c.source.quality;long lv=c.source.LV;long ch=Math.Max(1,c.source.chance);return q*1000000000L+lv*100000L-ch;}
}

[HarmonyPatch(typeof(SpawnList),nameof(SpawnList.Select),new Type[]{typeof(int),typeof(int)})]static class ItemGachaBestOfPatch{
static void Postfix(SpawnList __instance,ref CardRow __result){
if(!Plugin.GachaLuck.Value||GachaLuckCore.rerolling||__result==null)return;
var fs=new StackTrace(1,false).GetFrames();bool ctx=false;if(fs!=null)for(int i=0;i<fs.Length&&i<12;i++){string dn=fs[i].GetMethod()?.DeclaringType?.FullName??"";if(dn.Contains("TraitGachaBall")){ctx=true;break;}}
if(!ctx)return;int extra=GachaLuckCore.Extra();if(extra<=0)return;
GachaLuckCore.rerolling=true;try{CardRow best=__result;long score=GachaLuckCore.Score(best);for(int i=0;i<extra;i++){CardRow r=__instance.Select();long sc=GachaLuckCore.Score(r);if(sc>score){best=r;score=sc;}}__result=best;}finally{GachaLuckCore.rerolling=false;}
}}

[HarmonyPatch(typeof(LayerGachaResult),nameof(LayerGachaResult.Draw),new Type[]{typeof(string)})]static class CharaGachaBestOfPatch{
static void Postfix(string id,ref Chara __result){
if(!Plugin.GachaLuck.Value||GachaLuckCore.rerolling||__result==null)return;int extra=GachaLuckCore.Extra();if(extra<=0)return;
GachaLuckCore.rerolling=true;try{Chara best=__result;long score=GachaLuckCore.Score(best);for(int i=0;i<extra;i++){Chara c=LayerGachaResult.Draw(id);long sc=GachaLuckCore.Score(c);if(sc>score){if(best!=__result)best.Destroy();best=c;score=sc;}else c?.Destroy();}__result=best;}finally{GachaLuckCore.rerolling=false;}
}}

[HarmonyPatch(typeof(MiniGame),nameof(MiniGame.Deactivate),new Type[]{})]static class CasinoLuckSettlementPatch{
static void Prefix(MiniGame __instance){
if(!Plugin.CasinoLuck.Value||__instance==null||__instance.balance==null||__instance.balance.changeCoin<=0)return;
int chance=Math.Min(Plugin.CasinoCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.CasinoDiv.Value)));if(chance<=0)return;
if(EClass.rnd(100)<chance){int bonus=Math.Max(1,__instance.balance.changeCoin/2);__instance.balance.changeCoin+=bonus;Plugin.Info($"Casino Luck bonus +{bonus}");}
}}

}
