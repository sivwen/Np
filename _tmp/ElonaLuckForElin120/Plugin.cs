using BepInEx;using BepInEx.Configuration;using HarmonyLib;using System;using System.Collections.Generic;using System.Diagnostics;
namespace ElonaLuckForElin{
[BepInPlugin(G,N,V)]public sealed class Plugin:BaseUnityPlugin{
public const string G="sivwen.elin.elonaluck",N="Elona Luck for Elin",V="2.0.2";
internal static ConfigEntry<bool>Q=null!,D=null!,EC=null!,ET=null!,EV=null!,Craft=null!,Ammo=null!,Drop=null!,StealWeight=null!,WitnessLuck=null!,LockLuck=null!,FishLuck=null!,DismantleLuck=null!,ActivityBonus=null!,BonusMine=null!,BonusDig=null!,BonusHarvest=null!,BonusFish=null!,BonusCraft=null!,AutoDisableSALM=null!,NestLuck=null!,SeedLuck=null!,TreasureLuck=null!,ScratchLuck=null!,GachaLuck=null!,CasinoLuck=null!,CorpseLuck=null!,GeneLuck=null!,UniqueLootLuck=null!,CombatLootLuck=null!,GeneralMaterialLuck=null!,Log=null!;
internal static ConfigEntry<int>QD=null!,CountDiv=null!,CountCap=null!,TierDiv=null!,TierCap=null!,ValueDiv=null!,ValueCap=null!,DropDiv=null!,DropCap=null!,StealDiv=null!,StealCap=null!,WitnessDiv=null!,WitnessCap=null!,LockDiv=null!,LockCap=null!,FishDiv=null!,FishCap=null!,DismantleDiv=null!,DismantleCap=null!,ActSkillWeight=null!,ActLuckWeight=null!,ActSkillMod=null!,ActLuckMod=null!,NestDiv=null!,NestCap=null!,SeedDiv=null!,SeedCap=null!,TreasureDiv=null!,TreasureCap=null!,TreasureMythDiv=null!,ScratchDiv=null!,ScratchCap=null!,GachaDiv=null!,GachaCap=null!,CasinoDiv=null!,CasinoCap=null!,AnatomySkillWeight=null!,AnatomyLuckWeight=null!,GeneBonusCap=null!,UniqueLootDiv=null!,UniqueLootCap=null!,CombatLuckDiv=null!,CombatLuckCap=null!,CritKillBonus=null!,FinishKillBonus=null!,ExecutionerBonus=null!,OverkillCap=null!,CombatTotalCap=null!;
internal static Plugin I=null!; Harmony? h;
void Awake(){I=this;
Q=Config.Bind("엘로나식 운","장비 품질 운 적용",true,"엘로나식 장비 품질 1단계 상승을 적용합니다.");
D=Config.Bind("엘로나식 운","Elin 전역 운 재굴림 제거",true,"Elin 기본 전역 운 재굴림을 제거합니다.");
QD=Config.Bind("엘로나식 운","장비 품질 판정 분모",5000,"엘로나 기본값은 5000입니다.");
EC=Config.Bind("인챈트 운","인챈트 개수 운 적용",true,"운에 따라 랜덤 인챈트 개수를 늘립니다.");
CountDiv=Config.Bind("인챈트 운","인챈트 개수 운 분모",250,"Each divisor points of Luck adds 1% extra-enchant chance.");
CountCap=Config.Bind("인챈트 운","인챈트 개수 확률 상한",35,"Maximum chance for each extra-enchant roll.");
ET=Config.Bind("인챈트 운","인챈트 등급 운 적용",true,"Luck raises virtual generation level for bonus enchants.");
TierDiv=Config.Bind("인챈트 운","인챈트 등급 운 분모",20,"Luck/divisor is added to generation level.");
TierCap=Config.Bind("인챈트 운","인챈트 등급 레벨 보너스 상한",150,"Maximum virtual generation-level bonus.");
EV=Config.Bind("인챈트 운","인챈트 수치 운 적용",true,"Luck boosts newly generated random enchant values.");
ValueDiv=Config.Bind("인챈트 운","인챈트 수치 운 분모",2000,"Luck/divisor is fractional value bonus; 2000 Luck = +100% before cap.");
ValueCap=Config.Bind("인챈트 운","인챈트 수치 보너스 상한",50,"Maximum enchant-value bonus percent.");
Drop=Config.Bind("드롭 운","드롭 운 전체 사용",true,"Master switch for monster-drop Luck extensions. Does not affect figures or taxidermy.");
DropDiv=Config.Bind("드롭 운","일반 소재 운 분모",50,"Each divisor Luck adds 1% relative chance to common monster-material drops.");
DropCap=Config.Bind("드롭 운","일반 소재 보너스 상한",100,"Maximum relative bonus to common monster-material drops.");
GeneralMaterialLuck=Config.Bind("드롭 운","일반 소재 드롭 운",true,"Luck affects fang/skin/offal/heart/machine parts and similar common materials.");
CorpseLuck=Config.Bind("드롭 운","시체 해부학+운",true,"Luck can supplement Anatomy for corpse-related Anatomy calculations.");
GeneLuck=Config.Bind("드롭 운","유전자 해부학+운",true,"Anatomy and Luck together improve gene-drop chance.");
AnatomySkillWeight=Config.Bind("드롭 운","해부학 가중치",3,"Weight of Anatomy (skill 290) in corpse/gene composite.");
AnatomyLuckWeight=Config.Bind("드롭 운","운 가중치",2,"Weight of Luck in corpse/gene composite.");
GeneBonusCap=Config.Bind("드롭 운","유전자 상대 보너스 상한",300,"Maximum relative bonus to gene drop chance from Anatomy+Luck.");
UniqueLootLuck=Config.Bind("드롭 운","몬스터 고유 드롭 운",true,"Luck improves sourceCard/race unique drops such as monster-specific rare loot and artifacts.");
UniqueLootDiv=Config.Bind("드롭 운","고유 드롭 운 분모",10,"Each divisor Luck adds 1% relative chance to unique monster loot.");
UniqueLootCap=Config.Bind("드롭 운","고유 드롭 상대 보너스 상한",300,"Maximum relative bonus to unique monster loot.");
CombatLootLuck=Config.Bind("드롭 운","전투 마무리 드롭 운",true,"Held-item/equipment drops can improve from Luck plus kill quality.");
CombatLuckDiv=Config.Bind("드롭 운","전투 드롭 운 분모",10,"Each divisor Luck adds 1% relative held-item/equipment drop chance.");
CombatLuckCap=Config.Bind("드롭 운","전투 드롭 운 기여 상한",100,"Maximum Luck contribution to combat-loot bonus.");
CritKillBonus=Config.Bind("드롭 운","크리티컬 처치 보너스",50,"Relative drop bonus when the killing attack was critical.");
FinishKillBonus=Config.Bind("드롭 운","Finish 처치 보너스",100,"Relative drop bonus for AttackSource.Finish kills.");
ExecutionerBonus=Config.Bind("드롭 운","처형자 레벨당 보너스",25,"Relative bonus per Executioner feat level (1420).");
OverkillCap=Config.Bind("드롭 운","오버킬 보너스 상한",100,"Maximum relative bonus from overkill percent of target MaxHP.");
CombatTotalCap=Config.Bind("드롭 운","전투 드롭 총 보너스 상한",300,"Maximum combined Luck/critical/Finish/Executioner/overkill bonus.");
StealWeight=Config.Bind("훔치기 운","훔치기 중량 제한 운 우회",true,"Luck can bypass stealing weight limit.");
StealDiv=Config.Bind("훔치기 운","중량 우회 운 분모",20,"Base bypass chance is Luck/divisor percent before overweight penalty.");
StealCap=Config.Bind("훔치기 운","중량 우회 확률 상한",75,"Maximum bypass chance before overweight penalty.");
WitnessLuck=Config.Bind("범죄/발각 운","범죄 목격 회피 운",true,"Luck can make witnesses fail to notice PC crimes, including stealing.");
WitnessDiv=Config.Bind("범죄/발각 운","목격 회피 운 분모",25,"Base witness-avoidance chance is Luck/divisor percent.");
WitnessCap=Config.Bind("범죄/발각 운","목격 회피 확률 상한",60,"Maximum witness-avoidance chance.");
LockLuck=Config.Bind("자물쇠 운","자물쇠 따기 운",true,"Luck can reduce effective lock level for a lockpicking attempt.");
LockDiv=Config.Bind("자물쇠 운","자물쇠 운 분모",20,"Effective lock-level reduction is Luck/divisor.");
LockCap=Config.Bind("자물쇠 운","자물쇠 레벨 감소 상한",100,"Maximum effective lock-level reduction.");
FishLuck=Config.Bind("낚시 운","낚시 품질 운",true,"Luck can increase caught fish tier by one.");
FishDiv=Config.Bind("낚시 운","낚시 품질 운 분모",25,"Fish tier-up chance is Luck/divisor percent.");
FishCap=Config.Bind("낚시 운","낚시 품질 확률 상한",50,"Maximum fish tier-up chance.");
DismantleLuck=Config.Bind("수확/해체 운","해체 회수 운",true,"Luck improves fractional dismantle material recovery.");
DismantleDiv=Config.Bind("수확/해체 운","해체 회수 운 분모",50,"Each divisor Luck adds 1% effective fractional recovery chance.");
DismantleCap=Config.Bind("수확/해체 운","해체 회수 보너스 상한",100,"Maximum effective fractional recovery bonus.");
ActivityBonus=Config.Bind("SkillAndLuckMatter 대체","활동 보너스 롤 사용",true,"SkillAndLuckMatter 방식의 활동 보너스 롤을 사용합니다.");
BonusMine=Config.Bind("SkillAndLuckMatter 대체","채광 보너스",true,"Apply bonus rolls to mining.");
BonusDig=Config.Bind("SkillAndLuckMatter 대체","땅파기 보너스",true,"Apply bonus rolls to digging.");
BonusHarvest=Config.Bind("SkillAndLuckMatter 대체","수확/채집/벌목 보너스",true,"Apply bonus rolls to harvesting/gathering/reaping/chopping.");
BonusFish=Config.Bind("SkillAndLuckMatter 대체","낚시 보너스",true,"Apply bonus rolls to fishing.");
BonusCraft=Config.Bind("SkillAndLuckMatter 대체","제작/가공 보너스",true,"Refund crafting materials using the SkillAndLuckMatter curve.");
AutoDisableSALM=Config.Bind("SkillAndLuckMatter 대체","원본 모드 감지 시 자동 비활성",true,"원본 SkillAndLuckMatter가 감지되면 중복 보상을 막기 위해 대체 보너스 롤을 자동으로 끕니다.");
ActSkillWeight=Config.Bind("SkillAndLuckMatter 대체","스킬 가중치",3,"Original default skill weight.");
ActLuckWeight=Config.Bind("SkillAndLuckMatter 대체","운 가중치",2,"Original default luck weight.");
ActSkillMod=Config.Bind("SkillAndLuckMatter 대체","스킬 배율",100,"Original default modifier: 100%.");
ActLuckMod=Config.Bind("SkillAndLuckMatter 대체","운 배율",100,"Original default modifier: 100%.");
NestLuck=Config.Bind("희귀 결과 운","수정란 운 적용",true,"Luck improves fertilized egg chance from searchable bird nests.");
NestDiv=Config.Bind("희귀 결과 운","수정란 운 분모",50,"Extra fertilized-egg reroll chance is Luck/divisor percent.");
NestCap=Config.Bind("희귀 결과 운","수정란 추가 확률 상한",40,"Maximum extra fertilized-egg reroll chance.");
SeedLuck=Config.Bind("희귀 결과 운","씨앗 회수 운",true,"Luck improves manual seed recovery checks.");
SeedDiv=Config.Bind("희귀 결과 운","씨앗 회수 운 분모",50,"Each divisor Luck adds roughly 1% effective seed-check strength.");
SeedCap=Config.Bind("희귀 결과 운","씨앗 회수 보너스 상한",200,"Maximum effective seed-check strength bonus.");
TreasureLuck=Config.Bind("희귀 결과 운","보물상자 희귀도 운",true,"Luck improves equipment rarity generated inside treasure chests.");
TreasureDiv=Config.Bind("희귀 결과 운","레전더리 운 분모",50,"Luck/divisor is subtracted from the chest rarity roll.");
TreasureCap=Config.Bind("희귀 결과 운","레전더리 판정 감소 상한",50,"Maximum reduction on the 0-99 treasure rarity roll.");
TreasureMythDiv=Config.Bind("희귀 결과 운","미시컬 운 분모",500,"Each divisor Luck reduces the Mythical 1/N denominator by 1, minimum N=5.");
ScratchLuck=Config.Bind("희귀 결과 운","스크래치 당첨 운",true,"Luck improves high-tier scratch prize checks.");
ScratchDiv=Config.Bind("희귀 결과 운","스크래치 운 분모",50,"Each divisor Luck adds 1% effective chance to scratch prize checks.");
ScratchCap=Config.Bind("희귀 결과 운","스크래치 보너스 상한",150,"Maximum effective scratch prize chance bonus.");
GachaLuck=Config.Bind("가챠 운","가챠 Best-of 운",true,"Luck adds best-of candidate rolls to character and item gacha.");
GachaDiv=Config.Bind("가챠 운","추가 후보당 운",500,"Each divisor Luck adds one extra candidate.");
GachaCap=Config.Bind("가챠 운","추가 후보 상한",5,"Maximum extra candidates.");
CasinoLuck=Config.Bind("카지노 운","카지노 배당 운",true,"Luck can grant a bonus on positive net casino winnings at minigame settlement.");
CasinoDiv=Config.Bind("카지노 운","카지노 보너스 운 분모",25,"Bonus payout chance is Luck/divisor percent.");
CasinoCap=Config.Bind("카지노 운","카지노 보너스 확률 상한",50,"Maximum bonus payout chance.");
Craft=Config.Bind("호환성","제작 장비에도 적용",false,"Include crafted equipment.");
Ammo=Config.Bind("호환성","탄약 포함",false,"Include ammo.");
Log=Config.Bind("진단","디버그 로그",false,"운 보정 발생 내용을 로그에 기록합니다.");
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


static class DeathLootContext{
[ThreadStatic] internal static Card? victim;
[ThreadStatic] internal static Card? origin;
[ThreadStatic] internal static AttackSource attackSource;
[ThreadStatic] internal static bool critical;
[ThreadStatic] internal static int overkillPct;
[ThreadStatic] internal static bool active;
[ThreadStatic] internal static bool geneCreated;
[ThreadStatic] internal static HashSet<string>? createdIds;
[ThreadStatic] internal static List<Thing>? heldBefore;

internal static int AnatomyComposite(Card? killer){
 if(killer==null)return 0;int skill=Math.Max(0,killer.Evalue(290));int luck=Plugin.Luck();
 int sw=Math.Max(0,Plugin.AnatomySkillWeight.Value),lw=Math.Max(0,Plugin.AnatomyLuckWeight.Value),den=sw+lw;
 if(den<=0)return skill;int mix=(skill*sw+luck*lw)/den;return Math.Max(skill,mix);
}
internal static double IncrementalChance(double baseP,int relativeBonusPct){
 if(baseP<=0||baseP>=1||relativeBonusPct<=0)return 0;
 double target=Math.Min(0.95,baseP*(1.0+relativeBonusPct/100.0));
 if(target<=baseP)return 0;return (target-baseP)/(1.0-baseP);
}
internal static bool Roll(double p){if(p<=0)return false;if(p>=1)return true;return EClass.rnd(1000000)<(int)(p*1000000.0);}
internal static Point DropPoint(Card v){Point p=v.GetRootCard().pos;if(p.IsBlocked)p=p.GetNearestPoint()??p;return p;}
internal static void DropThing(Thing t,Card v){
 if(t==null)return;t.isHidden=false;t.isNPCProperty=false;t.SetInt(116);EClass._zone.AddCard(t,DropPoint(v));
}
internal static void TryBonusCreated(string id,int denom,int relativePct,Card v){
 if(denom<=1||relativePct<=0||createdIds==null||createdIds.Contains(id))return;
 double q=IncrementalChance(1.0/denom,relativePct);if(!Roll(q))return;
 Thing t=ThingGen.Create(id,-1,v.LV);DropThing(t,v);createdIds.Add(id);Plugin.Info($"Drop Luck: bonus {id}");
}
internal static int CombatBonus(Card killer){
 int b=Math.Min(Math.Max(0,Plugin.CombatLuckCap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.CombatLuckDiv.Value)));
 if(critical)b+=Math.Max(0,Plugin.CritKillBonus.Value);
 if(attackSource==AttackSource.Finish)b+=Math.Max(0,Plugin.FinishKillBonus.Value);
 b+=Math.Min(100,Math.Max(0,killer.Evalue(1420)*Math.Max(0,Plugin.ExecutionerBonus.Value)));
 b+=Math.Min(Math.Max(0,Plugin.OverkillCap.Value),Math.Max(0,overkillPct));
 return Math.Min(Math.Max(0,Plugin.CombatTotalCap.Value),b);
}
}

[HarmonyPatch(typeof(Card),nameof(Card.Die),new Type[]{typeof(Element),typeof(Card),typeof(AttackSource),typeof(Chara)})]static class DeathLootSignalPatch{
static void Prefix(Card __instance,Card origin,AttackSource attackSource){
 DeathLootContext.victim=__instance;DeathLootContext.origin=origin;DeathLootContext.attackSource=attackSource;
 DeathLootContext.critical=AttackProcess.Current!=null&&AttackProcess.Current.TC==__instance&&AttackProcess.Current.CC==origin&&AttackProcess.Current.crit;
 int mh=Math.Max(1,__instance.MaxHP);DeathLootContext.overkillPct=Math.Min(500,Math.Max(0,-__instance.hp*100/mh));
}
static Exception? Finalizer(Exception? __exception){
 DeathLootContext.victim=null;DeathLootContext.origin=null;DeathLootContext.attackSource=AttackSource.None;DeathLootContext.critical=false;DeathLootContext.overkillPct=0;return __exception;
}
}

[HarmonyPatch(typeof(Card),nameof(Card.Evalue),new Type[]{typeof(int)})]static class AnatomyLuckEvaluePatch{
static void Postfix(Card __instance,int ele,ref int __result){
 if(!Plugin.Drop.Value||!Plugin.CorpseLuck.Value||!DeathLootContext.active||ele!=290||DeathLootContext.origin!=__instance)return;
 int mix=DeathLootContext.AnatomyComposite(__instance);if(mix>__result)__result=mix;
}
}

[HarmonyPatch(typeof(ThingGen),nameof(ThingGen.Create),new Type[]{typeof(string),typeof(int),typeof(int)})]static class LootCreatedTrackerPatch{
static void Postfix(string id){
 if(DeathLootContext.active&&DeathLootContext.createdIds!=null&&!string.IsNullOrEmpty(id))DeathLootContext.createdIds.Add(id);
}
}

[HarmonyPatch(typeof(Chara),nameof(Chara.MakeGene),new Type[]{typeof(Nullable<DNA.Type>)})]static class GeneCreatedTrackerPatch{
static void Postfix(){
 if(DeathLootContext.active)DeathLootContext.geneCreated=true;
}
}

[HarmonyPatch(typeof(Card),nameof(Card.SpawnLoot),new Type[]{typeof(Card)})]static class SpawnLootRebalancedPatch{
static void Prefix(Card __instance,Card origin){
 DeathLootContext.active=true;DeathLootContext.geneCreated=false;
 DeathLootContext.createdIds=new HashSet<string>();DeathLootContext.heldBefore=new List<Thing>();
 foreach(Thing t in __instance.things)if(t!=null)DeathLootContext.heldBefore.Add(t);
}
static void Postfix(Card __instance,Card origin){
 if(!Plugin.Drop.Value||!__instance.isChara)return;
 var made=DeathLootContext.createdIds??new HashSet<string>();int luck=Plugin.Luck();

 if(Plugin.GeneralMaterialLuck.Value){
   int rel=Math.Min(Math.Max(0,Plugin.DropCap.Value),Math.Max(0,luck/Math.Max(1,Plugin.DropDiv.Value)));
   if(__instance.Chara.IsMachine){
     bool scrap=__instance.Chara.HasElement(1248);
     DeathLootContext.TryBonusCreated("memory_chip",200,rel,__instance);
     DeathLootContext.TryBonusCreated(scrap?"scrap":"microchip",20,rel,__instance);
     DeathLootContext.TryBonusCreated(scrap?"bolt":"battery",15,rel,__instance);
   }else{
     if(__instance.Chara.IsAnimal){
       DeathLootContext.TryBonusCreated("fang",15,rel,__instance);
       DeathLootContext.TryBonusCreated("skin",10,rel,__instance);
     }
     DeathLootContext.TryBonusCreated("offal",20,rel,__instance);
     DeathLootContext.TryBonusCreated("heart",20,rel,__instance);
   }
   switch(__instance.id){
     case "golem_wood":DeathLootContext.TryBonusCreated("crystal_earth",30,rel,__instance);break;
     case "golem_fish":case "golem_stone":DeathLootContext.TryBonusCreated("crystal_sun",30,rel,__instance);break;
     case "golem_steel":DeathLootContext.TryBonusCreated("crystal_mana",30,rel,__instance);break;
   }
 }

 if(Plugin.GeneLuck.Value&&!DeathLootContext.geneCreated&&origin!=null&&!__instance.IsPCFaction){
   int comp=DeathLootContext.AnatomyComposite(origin);
   int rel=Math.Min(Math.Max(0,Plugin.GeneBonusCap.Value),Math.Max(0,comp));
   double q=DeathLootContext.IncrementalChance(1.0/200.0,rel);
   if(DeathLootContext.Roll(q)){Thing g=__instance.Chara.MakeGene();DeathLootContext.DropThing(g,__instance);DeathLootContext.geneCreated=true;Plugin.Info($"Gene Luck: anatomy/luck composite {comp}");}
 }

 if(Plugin.UniqueLootLuck.Value&&!__instance.IsPCFaction){
   int rel=Math.Min(Math.Max(0,Plugin.UniqueLootCap.Value),Math.Max(0,luck/Math.Max(1,Plugin.UniqueLootDiv.Value)));
   Action<string> tryEntry=(string entry)=>{
     if(string.IsNullOrEmpty(entry))return;string[] p=entry.Split('/');if(p.Length<2)return;
     if(!int.TryParse(p[1],out int n)||n<=0||n>=1000)return;string id=p[0];if(made.Contains(id))return;
     double q=DeathLootContext.IncrementalChance(n/1000.0,rel);if(!DeathLootContext.Roll(q))return;
     Thing t=ThingGen.Create(id,-1,__instance.LV);DeathLootContext.DropThing(t,__instance);made.Add(id);Plugin.Info($"Unique Loot Luck: {id}");
   };
   if(__instance.sourceCard!=null&&__instance.sourceCard.loot!=null)foreach(string e in __instance.sourceCard.loot)tryEntry(e);
   if(__instance.Chara.race!=null&&__instance.Chara.race.loot!=null)foreach(string e in __instance.Chara.race.loot)tryEntry(e);
 }

 if(Plugin.CombatLootLuck.Value&&origin!=null&&DeathLootContext.heldBefore!=null){
   int rel=DeathLootContext.CombatBonus(origin);
   foreach(Thing item in DeathLootContext.heldBefore){
     if(item==null||item.parent!=__instance||item.isGifted||item.rarity>=Rarity.Artifact||item.HasTag(CTAG.gift)||item.trait is TraitChestMerchant)continue;
     double baseP;
     if(item.trait!=null&&item.trait.DropChance>0f)baseP=Math.Min(0.95,item.trait.DropChance);
     else if(item.IsEquipmentOrRanged)baseP=(item.rarity>=Rarity.Legendary)?0.05:0.01;
     else baseP=0.20;
     double q=DeathLootContext.IncrementalChance(baseP,rel);
     if(DeathLootContext.Roll(q)){DeathLootContext.DropThing(item,__instance);Plugin.Info($"Combat Loot: {item.id}, bonus {rel}%");}
   }
 }
}
static Exception? Finalizer(Exception? __exception){
 DeathLootContext.active=false;DeathLootContext.geneCreated=false;DeathLootContext.createdIds=null;DeathLootContext.heldBefore=null;return __exception;
}
}
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

[HarmonyPatch(typeof(TaskHarvest),nameof(TaskHarvest.HarvestThing),new Type[]{})]static class DismantleContextPatch{
static void Prefix(){if(Plugin.DismantleLuck.Value)RareOutcomeContext.dismantleDepth++;}
static Exception? Finalizer(Exception? __exception){if(Plugin.DismantleLuck.Value&&RareOutcomeContext.dismantleDepth>0)RareOutcomeContext.dismantleDepth--;return __exception;}
}
[HarmonyPatch(typeof(EClass),nameof(EClass.rndf),new Type[]{typeof(float)})]static class DismantleLuckPatch{
static void Prefix(ref float a){
if(!Plugin.DismantleLuck.Value||a<=1f||RareOutcomeContext.dismantleDepth<=0)return;
int bonus=Math.Min(Plugin.DismantleCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.DismantleDiv.Value)));if(bonus<=0)return;
a=Math.Max(1f,a*100f/(100f+bonus));
}}
[HarmonyPatch(typeof(Map),nameof(Map.TrySmoothPick),new Type[]{typeof(Point),typeof(Thing),typeof(Chara)})]static class ActivityDropBonusPatch{
static void Prefix(Thing t,Chara c){
if(!Plugin.ActivityEnabled()||t==null||c==null||!c.IsPC)return;
var fs=new StackTrace(1,false).GetFrames();if(fs==null)return;int skill=-1;bool enabled=false;
for(int i=0;i<fs.Length&&i<14;i++){string dn=fs[i].GetMethod()?.DeclaringType?.FullName??"";
if(dn.Contains("TaskMine")){skill=c.Evalue(220);enabled=Plugin.BonusMine.Value;break;}
if(dn.Contains("TaskDig")){skill=c.Evalue(230);enabled=Plugin.BonusDig.Value;break;}
if(dn.Contains("TaskChopWood")){skill=c.Evalue(225);enabled=Plugin.BonusHarvest.Value;break;}
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
[ThreadStatic] internal static int lootDepth;
[ThreadStatic] internal static int dismantleDepth;
[ThreadStatic] internal static int treasureDepth;
[ThreadStatic] internal static int nestDepth;
internal static int PercentBonus(ConfigEntry<int> div,ConfigEntry<int> cap){return Math.Min(Math.Max(0,cap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,div.Value)));}
}

[HarmonyPatch(typeof(GrowSystem),nameof(GrowSystem.TryPopSeed),new Type[]{typeof(Chara)})]static class SeedLuckContextPatch{
static void Prefix(){if(Plugin.SeedLuck.Value){RareOutcomeContext.seedDepth++;RareOutcomeContext.seedRndIndex=0;}}
static Exception? Finalizer(Exception? __exception){if(Plugin.SeedLuck.Value&&RareOutcomeContext.seedDepth>0)RareOutcomeContext.seedDepth--;return __exception;}
}

[HarmonyPatch(typeof(ThingGen),nameof(ThingGen.CreateTreasureContent),new Type[]{typeof(Thing),typeof(int),typeof(TreasureType),typeof(bool)})]static class TreasureLuckContextPatch{
static void Prefix(){if(Plugin.TreasureLuck.Value)RareOutcomeContext.treasureDepth++;}
static Exception? Finalizer(Exception? __exception){if(Plugin.TreasureLuck.Value&&RareOutcomeContext.treasureDepth>0)RareOutcomeContext.treasureDepth--;return __exception;}
}
[HarmonyPatch(typeof(SurvivalManager),nameof(SurvivalManager.OnMineWreck),new Type[]{typeof(Point)})]static class NestLuckContextPatch{
static void Prefix(){if(Plugin.NestLuck.Value)RareOutcomeContext.nestDepth++;}
static Exception? Finalizer(Exception? __exception){if(Plugin.NestLuck.Value&&RareOutcomeContext.nestDepth>0)RareOutcomeContext.nestDepth--;return __exception;}
}
[HarmonyPatch(typeof(EClass),nameof(EClass.rnd),new Type[]{typeof(int)})]static class RareOutcomeRndPatch{
static void Prefix(ref int a){
if(a<=1)return;
var fs=new StackTrace(1,false).GetFrames();if(fs==null)return;
bool scratch=false,treasureSet=false;
for(int i=0;i<fs.Length&&i<8;i++){
 var m=fs[i].GetMethod();string dn=m?.DeclaringType?.FullName??"";string n=m?.Name??"";
 if(Plugin.ScratchLuck.Value&&dn.Contains("TraitCrafter")&&n.Contains("g__Prize"))scratch=true;
 if(Plugin.TreasureLuck.Value&&RareOutcomeContext.treasureDepth>0&&dn.Contains("ThingGen")&&n.Contains("g__SetRarity"))treasureSet=true;
}
if(Plugin.NestLuck.Value&&RareOutcomeContext.nestDepth>0&&a==10){
 int bonus=RareOutcomeContext.PercentBonus(Plugin.NestDiv,Plugin.NestCap);
 if(bonus>0){int na=(int)(((long)a*100+99+bonus)/(100+bonus));if(na<2)na=2;a=na;}
 return;
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
if(!Plugin.TreasureLuck.Value||RareOutcomeContext.treasureDepth<=0||a!=100)return;
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
GachaLuckCore.rerolling=true;try{Chara best=__result;long score=GachaLuckCore.Score(best);for(int i=0;i<extra;i++){Chara c=LayerGachaResult.Draw(id);long sc=GachaLuckCore.Score(c);if(sc>score){best?.Destroy();best=c;score=sc;}else c?.Destroy();}__result=best;}finally{GachaLuckCore.rerolling=false;}
}}

[HarmonyPatch(typeof(MiniGame),nameof(MiniGame.Deactivate),new Type[]{})]static class CasinoLuckSettlementPatch{
static void Prefix(MiniGame __instance){
if(!Plugin.CasinoLuck.Value||__instance==null||__instance.balance==null||__instance.balance.changeCoin<=0)return;
int chance=Math.Min(Plugin.CasinoCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.CasinoDiv.Value)));if(chance<=0)return;
if(EClass.rnd(100)<chance){int bonus=Math.Max(1,__instance.balance.changeCoin/2);__instance.balance.changeCoin+=bonus;Plugin.Info($"Casino Luck bonus +{bonus}");}
}}

}
