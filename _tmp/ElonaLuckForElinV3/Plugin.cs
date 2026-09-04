using BepInEx;using BepInEx.Configuration;using HarmonyLib;using System;using System.Collections.Generic;using System.Diagnostics;
namespace ElonaLuckForElinV3{
[BepInPlugin(G,N,V)]public sealed class Plugin:BaseUnityPlugin{
public const string G="sivwen.elin.elonaluck",N="Elona Luck for Elin v3 Core",V="3.0.0";
internal static Plugin I=null!;Harmony? h;
internal static ConfigEntry<bool> Witness=null!,Lock=null!,Fish=null!,Activity=null!,Mine=null!,Dig=null!,Harvest=null!,FishBonus=null!,Craft=null!,Casino=null!;
internal static ConfigEntry<int> WitnessDiv=null!,WitnessCap=null!,LockDiv=null!,LockCap=null!,FishDiv=null!,FishCap=null!,SkillW=null!,LuckW=null!,CasinoDiv=null!,CasinoCap=null!;
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
h=new Harmony(G);h.PatchAll();Logger.LogInfo(N+" "+V+" loaded. 사망/드롭/전역 RNG 패치 없음.");}
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
}