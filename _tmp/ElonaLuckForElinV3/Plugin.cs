using BepInEx;using BepInEx.Configuration;using HarmonyLib;using System;using System.Collections.Generic;using System.Diagnostics;using System.Reflection;using System.Reflection.Emit;
namespace ElonaLuckForElinV3{
[BepInPlugin(G,N,V)]public sealed class Plugin:BaseUnityPlugin{
public const string G="sivwen.elin.elonaluck",N="Elona Luck for Elin v3.3",V="3.3.0";
internal static Plugin I=null!;Harmony? h;
internal static ConfigEntry<bool> Witness=null!,Lock=null!,Fish=null!,Activity=null!,Mine=null!,Dig=null!,Harvest=null!,FishBonus=null!,Craft=null!,Casino=null!,DropPatch=null!,Corpse=null!,Gene=null!,Materials=null!,UniqueLoot=null!,CombatLoot=null!,StealWeight=null!,SeedLuck=null!,TreasureLuck=null!,ScratchLuck=null!,FertEggLuck=null!;
internal static ConfigEntry<int> WitnessDiv=null!,WitnessCap=null!,LockDiv=null!,LockCap=null!,FishDiv=null!,FishCap=null!,SkillW=null!,LuckW=null!,CasinoDiv=null!,CasinoCap=null!,AnatomyW=null!,AnatomyLuckW=null!,GeneDiv=null!,GeneCap=null!,MaterialDiv=null!,MaterialCap=null!,UniqueDiv=null!,UniqueCap=null!,CombatLuckDiv=null!,CombatLuckCap=null!,CritBonus=null!,ExecutionerBonus=null!,OverkillCap=null!,CombatTotalCap=null!,StealWeightDiv=null!,StealWeightCap=null!,SeedDiv=null!,SeedCap=null!,TreasureDiv=null!,TreasureCap=null!,ScratchDiv=null!,ScratchCap=null!,EggDiv=null!,EggCap=null!;
void Awake(){I=this;
Witness=Config.Bind("범죄/발각 운","범죄 목격 회피 운",true,"범죄 목격 판정이 성공했을 때 운에 따라 추가 회피 판정을 합니다.");
WitnessDiv=Config.Bind("범죄/발각 운","목격 회피 운 분모",25,"기본 회피 확률은 운/이 값(%)입니다.");
WitnessCap=Config.Bind("범죄/발각 운","목격 회피 확률 상한",60,"목격 회피 확률의 최대값입니다.");
Lock=Config.Bind("자물쇠 운","자물쇠 따기 운",true,"자물쇠 따기 시에만 유효 자물쇠 레벨을 운으로 낮춥니다.");
LockDiv=Config.Bind("자물쇠 운","자물쇠 운 분모",20,"유효 자물쇠 레벨 감소량은 운/이 값입니다.");
LockCap=Config.Bind("자물쇠 운","자물쇠 레벨 감소 상한",100,"유효 자물쇠 레벨 감소량 상한입니다.");
Fish=Config.Bind("낚시 운","낚시 품질 운",true,"잡힌 물고기의 tier를 운에 따라 1단계 올립니다.");
FishDiv=Config.Bind("낚시 운","낚시 품질 운 분모",25,"등급 상승 확률은 운/이 값(%)입니다.");
FishCap=Config.Bind("낚시 운","낚시 품질 확률 상한",50,"등급 상승 확률 상한입니다.");
Activity=Config.Bind("SkillAndLuckMatter 대체","활동 보너스 사용",true,"SkillAndLuckMatter 방식의 스킬+운 활동 보너스를 사용합니다.");
Mine=Config.Bind("SkillAndLuckMatter 대체","채광 보너스",true,"채광 보상 수량에 보너스 롤을 적용합니다.");
Dig=Config.Bind("SkillAndLuckMatter 대체","땅파기 보너스",true,"땅파기 보상 수량에 보너스 롤을 적용합니다.");
Harvest=Config.Bind("SkillAndLuckMatter 대체","수확/채집/벌목 보너스",true,"수확·채집·벌목 보상 수량에 보너스 롤을 적용합니다.");
FishBonus=Config.Bind("SkillAndLuckMatter 대체","낚시 보너스",true,"낚시 보상 수량에 보너스 롤을 적용합니다.");
Craft=Config.Bind("SkillAndLuckMatter 대체","제작/가공 재료 환급",true,"제작/가공 시 스킬+운 공식으로 재료를 환급합니다.");
SkillW=Config.Bind("SkillAndLuckMatter 대체","스킬 가중치",3,"활동 점수에서 스킬의 가중치입니다.");
LuckW=Config.Bind("SkillAndLuckMatter 대체","운 가중치",2,"활동 점수에서 운의 가중치입니다.");
Casino=Config.Bind("카지노 운","카지노 배당 운",true,"순이익이 양수일 때 운에 따라 추가 배당을 지급합니다.");
CasinoDiv=Config.Bind("카지노 운","카지노 보너스 운 분모",25,"추가 배당 확률은 운/이 값(%)입니다.");
CasinoCap=Config.Bind("카지노 운","카지노 보너스 확률 상한",50,"추가 배당 확률 상한입니다.");
DropPatch=Config.Bind("드롭 운","v3.1 드롭 패치 사용",true,"SpawnLoot의 개별 판정식만 좁게 수정합니다. 패턴이 맞지 않으면 전체 드롭 패치를 자동 건너뜁니다.");
Corpse=Config.Bind("드롭 운","시체 해부학+운",true,"SpawnLoot 내부 해부학 판정에서만 운을 보조값으로 사용합니다.");
Gene=Config.Bind("드롭 운","유전자 해부학+운",true,"원래 유전자 chance(200) 판정의 분모만 해부학+운으로 완화합니다.");
Materials=Config.Bind("드롭 운","일반 소재 드롭 운",true,"송곳니/가죽/내장/심장/기계부품/골렘 결정의 기존 chance() 분모만 완화합니다.");
UniqueLoot=Config.Bind("드롭 운","몬스터 고유 드롭 운",true,"sourceCard/race 고유 loot의 원래 rnd(1000) 판정만 운으로 보정합니다.");
CombatLoot=Config.Bind("드롭 운","전투 마무리 장비/소지품 드롭",true,"몬스터 장비/소지품의 기존 판정에 운·크리티컬·처형자·오버킬을 반영합니다.");
AnatomyW=Config.Bind("드롭 운","해부학 가중치",3,"시체/유전자 혼합값에서 해부학의 가중치입니다.");
AnatomyLuckW=Config.Bind("드롭 운","운 가중치",2,"시체/유전자 혼합값에서 운의 가중치입니다.");
GeneDiv=Config.Bind("드롭 운","유전자 보너스 강도 분모",2,"해부학+운 혼합값을 이 값으로 나눈 %만큼 유전자 드롭 확률을 상대적으로 높입니다.");
GeneCap=Config.Bind("드롭 운","유전자 상대 보너스 상한",200,"유전자 드롭 상대 확률 증가 상한입니다.");
MaterialDiv=Config.Bind("드롭 운","일반 소재 운 분모",50,"운이 이 값만큼 오를 때 일반 소재 상대 드롭률이 1% 증가합니다.");
MaterialCap=Config.Bind("드롭 운","일반 소재 보너스 상한",100,"일반 소재 상대 드롭률 증가 상한입니다.");
UniqueDiv=Config.Bind("드롭 운","고유 드롭 운 분모",10,"운이 이 값만큼 오를 때 몬스터 고유 드롭의 상대 확률이 1% 증가합니다.");
UniqueCap=Config.Bind("드롭 운","고유 드롭 보너스 상한",300,"몬스터 고유 드롭 상대 확률 증가 상한입니다.");
CombatLuckDiv=Config.Bind("드롭 운","전투 드롭 운 분모",10,"운이 이 값만큼 오를 때 장비/소지품 상대 드롭률이 1% 증가합니다.");
CombatLuckCap=Config.Bind("드롭 운","전투 드롭 운 기여 상한",100,"전투 드롭에서 운이 기여하는 상대 보너스 상한입니다.");
CritBonus=Config.Bind("드롭 운","크리티컬 처치 보너스",50,"마지막 공격이 실제 크리티컬이면 더하는 상대 드롭 보너스입니다.");
ExecutionerBonus=Config.Bind("드롭 운","처형자 레벨당 보너스",25,"처형자 특성(1420) 1레벨당 더하는 상대 드롭 보너스입니다.");
OverkillCap=Config.Bind("드롭 운","오버킬 보너스 상한",100,"대상 최대 HP 대비 음수 HP 비율에서 얻는 보너스 상한입니다.");
CombatTotalCap=Config.Bind("드롭 운","전투 드롭 총 보너스 상한",300,"운·크리티컬·처형자·오버킬 합산 상대 보너스 상한입니다.");
StealWeight=Config.Bind("훔치기 운","중량 제한 운 우회",true,"AI_Steal 내부의 tooHeavy 판정에서만 운으로 중량 제한을 확률적으로 우회합니다.");
StealWeightDiv=Config.Bind("훔치기 운","중량 우회 운 분모",20,"기본 우회 확률은 운/이 값(%)이며 초과 중량 비율만큼 감소합니다.");
StealWeightCap=Config.Bind("훔치기 운","중량 우회 확률 상한",75,"초과 중량 보정 전 기본 우회 확률 상한입니다.");
SeedLuck=Config.Bind("채집/농사 운","씨앗 생성 운",true,"TryPopSeed의 원래 씨앗 생성 판정이 실패했을 때 운으로 한 번만 추가 판정합니다.");
SeedDiv=Config.Bind("채집/농사 운","씨앗 운 분모",25,"추가 씨앗 판정 확률은 운/이 값(%)입니다.");
SeedCap=Config.Bind("채집/농사 운","씨앗 추가 판정 상한",40,"씨앗 추가 판정 확률 상한입니다.");
TreasureLuck=Config.Bind("보물상자 운","보물 장비 희귀도 운",true,"CreateTreasureContent의 장비 rarity 결정에서만 운으로 상위 희귀도 승급 기회를 줍니다.");
TreasureDiv=Config.Bind("보물상자 운","보물 희귀도 운 분모",25,"희귀도 승급 확률은 운/이 값(%)입니다.");
TreasureCap=Config.Bind("보물상자 운","보물 희귀도 승급 상한",50,"희귀도 승급 확률 상한입니다.");
ScratchLuck=Config.Bind("스크래치 운","스크래치 당첨 운",true,"스크래치의 각 Prize 실패 후 운에 따른 추가 판정을 합니다. 원래 상품 우선순서는 유지합니다.");
ScratchDiv=Config.Bind("스크래치 운","스크래치 운 분모",25,"각 실패 판정의 추가 당첨 확률은 운/이 값(%)에 원래 1/chance를 곱합니다.");
ScratchCap=Config.Bind("스크래치 운","스크래치 운 상한",60,"스크래치 운 보정의 기본 상한입니다.");
FertEggLuck=Config.Bind("생산 운","수정란 운",true,"MakeEgg의 원래 수정란 판정이 실패했을 때 운으로 한 번 추가 판정합니다.");
EggDiv=Config.Bind("생산 운","수정란 운 분모",25,"추가 수정란 판정 확률은 운/이 값(%)에 원래 1/fertChance를 반영합니다.");
EggCap=Config.Bind("생산 운","수정란 운 상한",50,"수정란 운 보정의 기본 상한입니다.");
h=new Harmony(G);h.PatchAll();Logger.LogInfo(N+" "+V+" loaded. Card.Die/전역 RNG 패치 없음. SpawnLoot/AI_Steal/씨앗/보물/스크래치/수정란의 개별 판정만 좁게 패치.");}
void OnDestroy(){h?.UnpatchSelf();}
internal static int Luck(){int l=EClass.pc==null?1:EClass.pc.Evalue(78);return Math.Max(1,Math.Min(9999,l));}
internal static double Score(int skill){int sw=Math.Max(0,SkillW.Value),lw=Math.Max(0,LuckW.Value),d=sw+lw;if(d<=0)return 0;return (Math.Max(0,skill)*sw+Luck()*lw)/(double)d;}
internal static double Curve(double x){if(x<=0)return 0;if(x<10)return x/10*0.15;if(x<40)return .15+(x-10)/30*.35;if(x<100)return .5+(x-40)/60*.25;if(x<200)return .75+(x-100)/100*.25;return 1+(x-200)/100;}
internal static int Rolls(int skill,bool craft=false){double p=Curve(Score(skill));if(craft)p/=10;int n=(int)Math.Floor(p);double f=p-n;if(f>0&&EClass.rnd(100000)<(int)(f*100000))n++;return n;}
}
[HarmonyPatch(typeof(Point),nameof(Point.TryWitnessCrime),new Type[]{typeof(Chara),typeof(Chara),typeof(int),typeof(Func<Chara,bool>)})]static class WitnessPatch{
static void Prefix(Chara criminal,ref Func<Chara,bool> funcWitness){if(!Plugin.Witness.Value||criminal==null||EClass.pc==null||criminal!=EClass.pc)return;int a=Math.Min(Plugin.WitnessCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.WitnessDiv.Value)));if(a<=0)return;var old=funcWitness;if(old==null)funcWitness=c=>EClass.rnd(10)==0&&EClass.rnd(100)>=a;else funcWitness=c=>old(c)&&EClass.rnd(100)>=a;}}
[HarmonyPatch(typeof(Trait),nameof(Trait.TryOpenLock),new Type[]{typeof(Chara),typeof(bool)})]static class LockPatch{
public sealed class S{public int lv;public bool changed;}static void Prefix(Trait __instance,Chara cc,out S __state){__state=new S();if(!Plugin.Lock.Value||cc==null||!cc.IsPC||__instance.owner==null)return;int cut=Math.Min(Plugin.LockCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.LockDiv.Value)));__state.lv=__instance.owner.c_lockLv;int n=Math.Max(0,__state.lv-cut);if(n<__state.lv){__instance.owner.c_lockLv=n;__state.changed=true;}}static Exception? Finalizer(Trait __instance,S __state,Exception? __exception){if(__state!=null&&__state.changed&&__instance.owner.c_lockLv>0)__instance.owner.c_lockLv=__state.lv;return __exception;}}
[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]static class FishPatch{
static void Postfix(Chara c,ref Thing __result){if(!Plugin.Fish.Value||c==null||!c.IsPC||__result==null||__result.category==null||!__result.category.IsChildOf("fish"))return;int a=Math.Min(Plugin.FishCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.FishDiv.Value)));if(__result.tier<3&&EClass.rnd(100)<a)__result.SetTier(Math.Min(3,__result.tier+1));}}
[HarmonyPatch(typeof(Map),nameof(Map.TrySmoothPick),new Type[]{typeof(Point),typeof(Thing),typeof(Chara)})]static class ActivityPatch{
static void Prefix(Thing t,Chara c){if(!Plugin.Activity.Value||t==null||c==null||!c.IsPC)return;var fs=new StackTrace(1,false).GetFrames();if(fs==null)return;int skill=-1;bool on=false;for(int i=0;i<fs.Length&&i<12;i++){string dn=fs[i].GetMethod()?.DeclaringType?.FullName??"";if(dn.Contains("TaskMine")){skill=c.Evalue(220);on=Plugin.Mine.Value;break;}if(dn.Contains("TaskDig")){skill=c.Evalue(230);on=Plugin.Dig.Value;break;}if(dn.Contains("TaskChopWood")){skill=c.Evalue(225);on=Plugin.Harvest.Value;break;}if(dn.Contains("TaskHarvest")||dn.Contains("GrowSystem")){skill=Math.Max(c.Evalue(250),c.Evalue(286));on=Plugin.Harvest.Value;break;}}if(!on||skill<0)return;int r=Plugin.Rolls(skill);if(r>0){int baseN=Math.Max(1,t.Num);t.ModNum(baseN*r);}}}
[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]static class FishBonusPatch{
static void Postfix(Chara c,ref Thing __result){if(!Plugin.Activity.Value||!Plugin.FishBonus.Value||c==null||!c.IsPC||__result==null)return;int r=Plugin.Rolls(c.Evalue(245));if(r>0){int b=Math.Max(1,__result.Num);__result.ModNum(b*r);}}}
static class CraftCore{internal static List<Thing>? Prep(Recipe r,List<Thing> ings,bool model){if(!Plugin.Activity.Value||!Plugin.Craft.Value||model||r==null||ings==null||EClass.pc==null)return null;Element e=r.source?.GetReqSkill();int rr=Plugin.Rolls(e==null?0:EClass.pc.Evalue(e.id),true);if(rr<=0)return null;var list=new List<Thing>();foreach(var x in ings)if(x!=null)list.Add(x.Duplicate(Math.Max(1,x.Num)*rr));return list;}internal static void Give(List<Thing>? xs){if(xs==null||EClass.pc==null)return;foreach(var x in xs)EClass.pc.AddCard(x);}}
[HarmonyPatch(typeof(Recipe),nameof(Recipe.Craft),new Type[]{typeof(BlessedState),typeof(bool),typeof(List<Thing>),typeof(TraitCrafter),typeof(bool)})]static class RecipePatch{static void Prefix(Recipe __instance,List<Thing> ings,bool model,out List<Thing>? __state){__state=CraftCore.Prep(__instance,ings,model);}static void Postfix(List<Thing>? __state,Thing __result){if(__result!=null)CraftCore.Give(__state);}}
[HarmonyPatch(typeof(RecipeCard),nameof(RecipeCard.Craft),new Type[]{typeof(BlessedState),typeof(bool),typeof(List<Thing>),typeof(TraitCrafter),typeof(bool)})]static class RecipeCardPatch{static void Prefix(RecipeCard __instance,List<Thing> ings,bool model,out List<Thing>? __state){__state=CraftCore.Prep(__instance,ings,model);}static void Postfix(List<Thing>? __state,Thing __result){if(__result!=null)CraftCore.Give(__state);}}
[HarmonyPatch(typeof(MiniGame),nameof(MiniGame.Deactivate),new Type[]{})]static class CasinoPatch{static void Prefix(MiniGame __instance){if(!Plugin.Casino.Value||__instance?.balance==null||__instance.balance.changeCoin<=0)return;int a=Math.Min(Plugin.CasinoCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.CasinoDiv.Value)));if(EClass.rnd(100)<a)__instance.balance.changeCoin+=Math.Max(1,__instance.balance.changeCoin/2);}}
static class DropMath{
internal static int Anatomy(Card actor,int id){
 int original=actor==null?0:actor.Evalue(id);if(!Plugin.DropPatch.Value||!Plugin.Corpse.Value||id!=290)return original;
 int sw=Math.Max(0,Plugin.AnatomyW.Value),lw=Math.Max(0,Plugin.AnatomyLuckW.Value),d=sw+lw;if(d<=0)return original;
 int mixed=(Math.Max(0,original)*sw+Plugin.Luck()*lw)/d;return Math.Max(original,mixed);
}
internal static int AdjustChance(int denominator,Card victim,Card origin,int kind){
 if(!Plugin.DropPatch.Value||denominator<=1)return denominator;int bonus=0;
 if(kind==1&&Plugin.Gene.Value){
   int anatomy=origin==null?0:Anatomy(origin,290);
   bonus=Math.Min(Math.Max(0,Plugin.GeneCap.Value),Math.Max(0,anatomy/Math.Max(1,Plugin.GeneDiv.Value)));
 }else if(kind==2&&Plugin.Materials.Value){
   bonus=Math.Min(Math.Max(0,Plugin.MaterialCap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.MaterialDiv.Value)));
 }else return denominator;
 return ReduceDenom(denominator,bonus);
}
internal static int UniqueRnd(int max){
 if(!Plugin.DropPatch.Value||!Plugin.UniqueLoot.Value||max<=1)return EClass.rnd(max);
 int bonus=Math.Min(Math.Max(0,Plugin.UniqueCap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.UniqueDiv.Value)));
 return EClass.rnd(ReduceDenom(max,bonus));
}
internal static int CombatBonus(Card victim,Card origin){
 if(!Plugin.DropPatch.Value||!Plugin.CombatLoot.Value||origin==null)return 0;
 int b=Math.Min(Math.Max(0,Plugin.CombatLuckCap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.CombatLuckDiv.Value)));
 var ap=AttackProcess.Current;
 if(ap!=null&&ap.CC==origin&&ap.TC==victim&&ap.crit)b+=Math.Max(0,Plugin.CritBonus.Value);
 b+=Math.Min(100,Math.Max(0,origin.Evalue(1420)*Math.Max(0,Plugin.ExecutionerBonus.Value)));
 if(victim!=null){int mh=Math.Max(1,victim.MaxHP);int ov=Math.Max(0,-victim.hp*100/mh);b+=Math.Min(Math.Max(0,Plugin.OverkillCap.Value),ov);}
 return Math.Min(Math.Max(0,Plugin.CombatTotalCap.Value),b);
}
internal static int AdjustCombatDenom(int denominator,Card victim,Card origin){return ReduceDenom(denominator,CombatBonus(victim,origin));}
internal static float CombatRndf(float max,Card victim,Card origin){
 if(max<=0)return EClass.rndf(max);int b=CombatBonus(victim,origin);if(b<=0)return EClass.rndf(max);
 float scale=1f+b/100f;return EClass.rndf(max)/scale;
}
internal static int ReduceDenom(int d,int bonus){if(d<=1||bonus<=0)return d;long n=(long)d*100L;int r=(int)((n+99+bonus)/(100+bonus));return Math.Max(1,r);}
}

[HarmonyPatch(typeof(Card),nameof(Card.SpawnLoot),new Type[]{typeof(Card)})]static class SpawnLootNarrowPatch{
static readonly MethodInfo MAnatomy=typeof(DropMath).GetMethod(nameof(DropMath.Anatomy),BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public)!;
static readonly MethodInfo MAdjustChance=typeof(DropMath).GetMethod(nameof(DropMath.AdjustChance),BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public)!;
static readonly MethodInfo MUniqueRnd=typeof(DropMath).GetMethod(nameof(DropMath.UniqueRnd),BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public)!;
static readonly MethodInfo MCombatDenom=typeof(DropMath).GetMethod(nameof(DropMath.AdjustCombatDenom),BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public)!;
static readonly MethodInfo MCombatRndf=typeof(DropMath).GetMethod(nameof(DropMath.CombatRndf),BindingFlags.Static|BindingFlags.NonPublic|BindingFlags.Public)!;

static bool IsCall(CodeInstruction c,string name){return c.operand is MethodInfo m&&m.Name==name&&(c.opcode==OpCodes.Call||c.opcode==OpCodes.Callvirt);}
static bool IsChance(CodeInstruction c){return c.operand is MethodInfo m&&m.Name.Contains("g__chance")&&(c.opcode==OpCodes.Call||c.opcode==OpCodes.Callvirt);}
static bool Ldc(CodeInstruction c,int v){
 if(c.opcode==OpCodes.Ldc_I4)return c.operand is int x&&x==v;
 if(v==-1)return c.opcode==OpCodes.Ldc_I4_M1;if(v>=0&&v<=8)return c.opcode==new[]{OpCodes.Ldc_I4_0,OpCodes.Ldc_I4_1,OpCodes.Ldc_I4_2,OpCodes.Ldc_I4_3,OpCodes.Ldc_I4_4,OpCodes.Ldc_I4_5,OpCodes.Ldc_I4_6,OpCodes.Ldc_I4_7,OpCodes.Ldc_I4_8}[v];
 if(c.opcode==OpCodes.Ldc_I4_S)return Convert.ToInt32(c.operand)==v;return false;
}
static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions){
 var src=new List<CodeInstruction>(instructions);var chance=new List<int>();var anatomy=new List<int>();
 for(int i=0;i<src.Count;i++){if(IsChance(src[i]))chance.Add(i);if(i>0&&IsCall(src[i],"Evalue")&&Ldc(src[i-1],290))anatomy.Add(i);}
 if(chance.Count<14||anatomy.Count<2){Plugin.I.Logger.LogWarning($"v3.1 SpawnLoot 패턴 불일치: chance={chance.Count}, anatomy={anatomy.Count}. 드롭 패치를 적용하지 않습니다.");foreach(var x in src)yield return x;yield break;}
 int unique=-1;for(int i=chance[2]+1;i<chance[3];i++){if(i>0&&IsCall(src[i],"rnd")&&Ldc(src[i-1],1000)){unique=i;break;}}
 int rndf=-1;for(int i=chance[13]+1;i<src.Count;i++){if(IsCall(src[i],"rndf")){rndf=i;break;}}
 int eq100=-1,item5=-1;if(rndf>=0){for(int i=rndf+1;i<src.Count;i++){if(eq100<0&&i>0&&IsCall(src[i],"rnd")&&Ldc(src[i-1],100)){eq100=i;continue;}if(eq100>=0&&i>0&&IsCall(src[i],"rnd")&&Ldc(src[i-1],5)){item5=i;break;}}}
 if(unique<0||rndf<0||eq100<0||item5<0){Plugin.I.Logger.LogWarning($"v3.1 SpawnLoot 세부 패턴 불일치: unique={unique}, rndf={rndf}, eq100={eq100}, item5={item5}. 드롭 패치를 적용하지 않습니다.");foreach(var x in src)yield return x;yield break;}
 var anatomySet=new HashSet<int>(anatomy);var chanceKind=new Dictionary<int,int>{{chance[2],1}};
 for(int n=3;n<=9;n++)chanceKind[chance[n]]=2;for(int n=11;n<=13;n++)chanceKind[chance[n]]=2;
 for(int i=0;i<src.Count;i++){
   var ci=src[i];
   if(anatomySet.Contains(i)){yield return new CodeInstruction(OpCodes.Call,MAnatomy);continue;}
   if(chanceKind.TryGetValue(i,out int kind)){yield return new CodeInstruction(OpCodes.Ldarg_0);yield return new CodeInstruction(OpCodes.Ldarg_1);yield return new CodeInstruction(OpCodes.Ldc_I4,kind);yield return new CodeInstruction(OpCodes.Call,MAdjustChance);yield return ci;continue;}
   if(i==unique){yield return new CodeInstruction(OpCodes.Call,MUniqueRnd);continue;}
   if(i==rndf){yield return new CodeInstruction(OpCodes.Ldarg_0);yield return new CodeInstruction(OpCodes.Ldarg_1);yield return new CodeInstruction(OpCodes.Call,MCombatRndf);continue;}
   if(i==eq100||i==item5){yield return new CodeInstruction(OpCodes.Ldarg_0);yield return new CodeInstruction(OpCodes.Ldarg_1);yield return new CodeInstruction(OpCodes.Call,MCombatDenom);yield return ci;continue;}
   yield return ci;
 }
 Plugin.I.Logger.LogInfo($"v3.1 SpawnLoot 좁은 패치 적용: chance={chance.Count}, anatomy={anatomy.Count}.");
}
}

[HarmonyPatch]
static class StealWeightNarrowPatch{
static readonly MethodInfo Getter=typeof(Card).GetProperty("ChildrenAndSelfWeight",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic)?.GetGetMethod(true)!;
static readonly MethodInfo Helper=typeof(StealWeightNarrowPatch).GetMethod(nameof(EffectiveWeight),BindingFlags.Static|BindingFlags.NonPublic)!;

static IEnumerable<MethodBase> TargetMethods(){
 int found=0;
 foreach(var t in Nested(typeof(AI_Steal))){
   foreach(var m in t.GetMethods(BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic)){
     if(m.IsAbstract||m.ContainsGenericParameters)continue;
     if(CallsGetter(m)){found++;yield return m;}
   }
 }
 if(found==0&&Plugin.I!=null)Plugin.I.Logger.LogWarning("v3.2: AI_Steal tooHeavy 내부 메서드를 찾지 못해 중량 우회 기능을 건너뜁니다.");
}
static IEnumerable<Type> Nested(Type root){
 foreach(var t in root.GetNestedTypes(BindingFlags.Public|BindingFlags.NonPublic)){
   yield return t;foreach(var c in Nested(t))yield return c;
 }
}
static bool CallsGetter(MethodBase m){
 if(Getter==null)return false;
 try{
   var il=m.GetMethodBody()?.GetILAsByteArray();if(il==null)return false;int token=Getter.MetadataToken;
   for(int i=0;i+4<il.Length;i++){
     byte op=il[i];if(op!=0x28&&op!=0x6f)continue;
     if(BitConverter.ToInt32(il,i+1)==token)return true;
   }
 }catch{}
 return false;
}
static long EffectiveWeight(Card target){
 long w=target==null?0:target.ChildrenAndSelfWeight;
 if(!Plugin.StealWeight.Value||target==null||EClass.pc==null)return w;
 long limit=(long)EClass.pc.Evalue(281)*200L+(long)EClass.pc.STR*100L+1000L;
 if(w<=limit||limit<=0)return w;
 int baseChance=Math.Min(Math.Max(0,Plugin.StealWeightCap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.StealWeightDiv.Value)));
 if(baseChance<=0)return w;
 long scaled=(long)baseChance*limit/Math.Max(1L,w);
 int chance=(int)Math.Max(1L,Math.Min(baseChance,scaled));
 int roll=EClass.rnd(100);
 if(roll<chance){Plugin.I.Logger.LogInfo($"훔치기 중량 운 우회: {w}>{limit}, 확률 {chance}%");return limit;}
 return w;
}
static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,MethodBase __originalMethod){
 int n=0;
 foreach(var ci in instructions){
   if(Getter!=null&&ci.operand is MethodInfo m&&m==Getter&&(ci.opcode==OpCodes.Call||ci.opcode==OpCodes.Callvirt)){
     n++;yield return new CodeInstruction(OpCodes.Call,Helper);
   }else yield return ci;
 }
 if(n!=1)Plugin.I.Logger.LogWarning($"v3.2: {__originalMethod.DeclaringType?.Name}.{__originalMethod.Name}의 중량 getter 교체 수={n}. 예상=1.");
}
}

static class LuckRoll{
 internal static int Pct(ConfigEntry<int> div,ConfigEntry<int> cap){return Math.Min(Math.Max(0,cap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,div.Value)));}
 internal static bool ExtraOneIn(int originalDenom,ConfigEntry<int> div,ConfigEntry<int> cap){
   if(originalDenom<=0)return false;int p=Pct(div,cap);if(p<=0)return false;
   // p%의 Luck 보정에 원래 사건의 1/N 희소성을 유지한다.
   long scale=(long)p*10000L/Math.Max(1,originalDenom);
   return EClass.rnd(1000000)<Math.Min(999999,(int)scale);
 }
}
[HarmonyPatch(typeof(GrowSystem),nameof(GrowSystem.TryPopSeed),new Type[]{typeof(Chara)})]
static class SeedLuckPatch{
 static void Postfix(GrowSystem __instance,Chara c,ref Thing __result){
   if(!Plugin.SeedLuck.Value||__result!=null||EClass.pc==null||c!=EClass.pc)return;
   int p=LuckRoll.Pct(Plugin.SeedDiv,Plugin.SeedCap);if(p<=0||EClass.rnd(100)>=p)return;
   try{Thing seed=TraitSeed.MakeSeed(GrowSystem.cell);if(seed==null)return;__instance.TryPick(GrowSystem.cell,seed,c);__result=seed;}
   catch(Exception e){Plugin.I.Logger.LogWarning("v3.3 씨앗 Luck 보너스 건너뜀: "+e.GetType().Name);}
 }
}
[HarmonyPatch(typeof(Card),nameof(Card.MakeEgg),new Type[]{typeof(bool),typeof(int),typeof(bool),typeof(int),typeof(BlessedState?)})]
static class FertEggLuckPatch{
 static void Prefix(Card __instance,ref int fertChance){
   if(!Plugin.FertEggLuck.Value||fertChance<=1||EClass.pc==null)return;
   if(LuckRoll.ExtraOneIn(fertChance,Plugin.EggDiv,Plugin.EggCap))fertChance=1;
 }
}
[HarmonyPatch(typeof(TraitCrafter),nameof(TraitCrafter.Craft),new Type[]{typeof(AI_UseCrafter)})]
static class ScratchLuckPatch{
 static void Postfix(TraitCrafter __instance,AI_UseCrafter ai,ref Thing __result){
   if(!Plugin.ScratchLuck.Value||__result!=null||ai==null||EClass.pc==null)return;
   SourceRecipe.Row src=__instance.GetSource(ai);if(src==null||src.type.ToString()!="Scratch")return;
   int p=LuckRoll.Pct(Plugin.ScratchDiv,Plugin.ScratchCap);if(p<=0)return;
   if(Roll(20,p)){__result=ThingGen.Create("medal",-1,EClass.pc.LV);return;}
   if(Roll(10,p)){__result=ThingGen.Create("plat",-1,EClass.pc.LV);return;}
   if(Roll(10,p)){__result=ThingGen.CreateFromCategory("furniture",EClass.pc.LV);return;}
   if(Roll(4,p)){__result=ThingGen.Create("plamo_box",-1,EClass.pc.LV);return;}
   if(Roll(4,p)){__result=ThingGen.Create("food",-1,EClass.pc.LV);return;}
   if(Roll(1,p)){__result=ThingGen.Create("casino_coin",-1,EClass.pc.LV);}
 }
 static bool Roll(int denom,int p){if(denom<=1)return EClass.rnd(100)<p;return EClass.rnd(100*denom)<p;}
}
[HarmonyPatch(typeof(ThingGen),nameof(ThingGen.CreateTreasureContent),new Type[]{typeof(Thing),typeof(int),typeof(TreasureType),typeof(bool)})]
static class TreasureLuckPatch{
 static void Postfix(Thing t){
   if(!Plugin.TreasureLuck.Value||t==null||EClass.pc==null)return;
   int p=LuckRoll.Pct(Plugin.TreasureDiv,Plugin.TreasureCap);if(p<=0)return;
   foreach(var x in t.things){
     if(x==null||!x.IsEquipment||EClass.rnd(100)>=p)continue;
     if(x.rarity==Rarity.Superior)x.rarity=Rarity.Legendary;
     else if(x.rarity==Rarity.Legendary)x.rarity=Rarity.Mythical;
   }
 }
}

}