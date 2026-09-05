using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ElonaLuckForElinV3
{
[BepInPlugin(G,N,V)]
public sealed class Plugin : BaseUnityPlugin
{
    public const string G="sivwen.elin.elonaluck", N="Elona Luck for Elin v3.10", V="3.10.0";
    internal static Plugin I=null!;
    Harmony? h;

    internal static ConfigEntry<bool> Witness=null!,Lock=null!,Fish=null!,Casino=null!,StealWeight=null!;
    internal static ConfigEntry<bool> SeedLuck=null!,TreasureLuck=null!,ScratchLuck=null!,FertEggLuck=null!;
    internal static ConfigEntry<bool> PostDeathBonus=null!,GeneBonus=null!,MaterialBonus=null!,UniqueBonus=null!;
    internal static ConfigEntry<bool> ActivityBonus=null!,MineBonus=null!,DigBonus=null!,ChopBonus=null!,HarvestBonus=null!;
    internal static ConfigEntry<int> WitnessDiv=null!,WitnessCap=null!,LockDiv=null!,LockCap=null!,FishDiv=null!,FishCap=null!,CasinoDiv=null!,CasinoCap=null!;
    internal static ConfigEntry<int> StealWeightDiv=null!,StealWeightCap=null!,SeedDiv=null!,SeedCap=null!,TreasureDiv=null!,TreasureCap=null!,ScratchDiv=null!,ScratchCap=null!,EggDiv=null!,EggCap=null!;
    internal static ConfigEntry<int> AnatomyW=null!,AnatomyLuckW=null!,GeneDiv=null!,GeneCap=null!,MaterialDiv=null!,MaterialCap=null!,UniqueDiv=null!,UniqueCap=null!;
    internal static ConfigEntry<int> SkillW=null!,LuckW=null!,ActivityCap=null!;

    void Awake()
    {
        I=this;
        Witness=Config.Bind("범죄/발각 운","범죄 목격 회피 운",true,"범죄 목격 판정 성공 후 운으로 추가 회피 판정을 합니다.");
        WitnessDiv=Config.Bind("범죄/발각 운","목격 회피 운 분모",25,"회피 확률은 운/이 값(%)입니다.");
        WitnessCap=Config.Bind("범죄/발각 운","목격 회피 확률 상한",60,"목격 회피 확률 상한입니다.");

        Lock=Config.Bind("자물쇠 운","자물쇠 따기 운",true,"자물쇠 따기 시 유효 자물쇠 레벨을 운으로 낮춥니다.");
        LockDiv=Config.Bind("자물쇠 운","자물쇠 운 분모",20,"유효 자물쇠 레벨 감소량은 운/이 값입니다.");
        LockCap=Config.Bind("자물쇠 운","자물쇠 레벨 감소 상한",100,"유효 자물쇠 레벨 감소량 상한입니다.");

        Fish=Config.Bind("낚시 운","낚시 품질 운",true,"잡힌 물고기의 tier를 운에 따라 1단계 올립니다.");
        FishDiv=Config.Bind("낚시 운","낚시 품질 운 분모",25,"등급 상승 확률은 운/이 값(%)입니다.");
        FishCap=Config.Bind("낚시 운","낚시 품질 확률 상한",50,"등급 상승 확률 상한입니다.");

        Casino=Config.Bind("카지노 운","카지노 배당 운",true,"순이익이 양수일 때 운에 따라 추가 배당을 지급합니다.");
        CasinoDiv=Config.Bind("카지노 운","카지노 보너스 운 분모",25,"추가 배당 확률은 운/이 값(%)입니다.");
        CasinoCap=Config.Bind("카지노 운","카지노 보너스 확률 상한",50,"추가 배당 확률 상한입니다.");

        StealWeight=Config.Bind("훔치기 운","중량 제한 운 우회",true,"AI_Steal의 tooHeavy 판정 한 곳에만 적용하며 훔치기 시도마다 새로 굴립니다.");
        StealWeightDiv=Config.Bind("훔치기 운","중량 우회 운 분모",20,"기본 우회 확률은 운/이 값(%)이며 초과 중량 비율만큼 감소합니다.");
        StealWeightCap=Config.Bind("훔치기 운","중량 우회 확률 상한",75,"초과 중량 보정 전 기본 우회 확률 상한입니다.");

        SeedLuck=Config.Bind("채집/농사 운","씨앗 회수 운",true,"TryPopSeed의 수동 씨앗 회수 RNG 한 곳만 보정합니다.");
        SeedDiv=Config.Bind("채집/농사 운","씨앗 운 분모",25,"운이 이 값만큼 오를 때 씨앗 회수 상대 확률이 1% 증가합니다.");
        SeedCap=Config.Bind("채집/농사 운","씨앗 보너스 상한",60,"씨앗 회수 상대 확률 증가 상한입니다.");

        TreasureLuck=Config.Bind("보물상자 운","보물 장비 희귀도 운",true,"CreateTreasureContent의 로컬 SetRarity RNG만 보정합니다.");
        TreasureDiv=Config.Bind("보물상자 운","보물 희귀도 운 분모",25,"운이 이 값만큼 오를 때 희귀도 판정 상대 보정이 1% 증가합니다.");
        TreasureCap=Config.Bind("보물상자 운","보물 희귀도 보너스 상한",75,"보물상자 장비 희귀도 상대 보정 상한입니다.");

        ScratchLuck=Config.Bind("스크래치 운","스크래치 당첨 운",true,"TraitCrafter의 로컬 Prize RNG만 보정합니다.");
        ScratchDiv=Config.Bind("스크래치 운","스크래치 운 분모",25,"운이 이 값만큼 오를 때 각 Prize 상대 당첨 확률이 1% 증가합니다.");
        ScratchCap=Config.Bind("스크래치 운","스크래치 보너스 상한",75,"스크래치 상대 당첨 확률 증가 상한입니다.");

        FertEggLuck=Config.Bind("생산 운","수정란 운",true,"플레이어 진영 Card.MakeEgg의 fertChance 분모만 보정합니다.");
        EggDiv=Config.Bind("생산 운","수정란 운 분모",25,"운이 이 값만큼 오를 때 수정란 상대 확률이 1% 증가합니다.");
        EggCap=Config.Bind("생산 운","수정란 보너스 상한",75,"수정란 상대 확률 증가 상한입니다.");

        PostDeathBonus=Config.Bind("사망 후 보너스 드롭","안전 보너스 드롭 사용",true,"SpawnLoot를 건드리지 않고 ZoneEventManager.OnCharaDie 종료 후 보너스를 추가합니다.");
        GeneBonus=Config.Bind("사망 후 보너스 드롭","유전자 보너스",true,"원본 유전자가 없을 때 해부학+운으로 추가 유전자 판정을 합니다.");
        MaterialBonus=Config.Bind("사망 후 보너스 드롭","일반 소재 보너스",true,"원본 소재가 같은 칸에 없을 때만 Luck 기반 추가 판정을 합니다.");
        UniqueBonus=Config.Bind("사망 후 보너스 드롭","몬스터 고유 드롭 보너스",true,"sourceCard/race loot가 원본에서 나오지 않았을 때만 추가 판정합니다.");
        AnatomyW=Config.Bind("사망 후 보너스 드롭","해부학 가중치",3,"유전자 추가 판정에서 해부학의 가중치입니다.");
        AnatomyLuckW=Config.Bind("사망 후 보너스 드롭","운 가중치",2,"유전자 추가 판정에서 운의 가중치입니다.");
        GeneDiv=Config.Bind("사망 후 보너스 드롭","유전자 보너스 강도 분모",2,"해부학+운 혼합값을 이 값으로 나눈 %만큼 원래 유전자 확률을 상대적으로 높입니다.");
        GeneCap=Config.Bind("사망 후 보너스 드롭","유전자 상대 보너스 상한",200,"유전자 추가 확률 계산용 상대 보너스 상한입니다.");
        MaterialDiv=Config.Bind("사망 후 보너스 드롭","일반 소재 운 분모",50,"운이 이 값만큼 오를 때 원래 소재 확률이 상대적으로 1% 증가합니다.");
        MaterialCap=Config.Bind("사망 후 보너스 드롭","일반 소재 보너스 상한",100,"일반 소재 상대 보너스 상한입니다.");
        UniqueDiv=Config.Bind("사망 후 보너스 드롭","고유 드롭 운 분모",10,"운이 이 값만큼 오를 때 원래 고유 드롭 확률이 상대적으로 1% 증가합니다.");
        UniqueCap=Config.Bind("사망 후 보너스 드롭","고유 드롭 보너스 상한",300,"고유 드롭 상대 보너스 상한입니다.");

        ActivityBonus=Config.Bind("SkillAndLuckMatter 대체","직접 활동 보너스 사용",true,"전역 TrySmoothPick/StackTrace 없이 실제 산출 메서드의 TrySmoothPick 호출만 보정합니다.");
        MineBonus=Config.Bind("SkillAndLuckMatter 대체","채광 산출 보너스",true,"Map.MineBlock에서 생성되는 채광 산출물에 스킬+운 보너스를 적용합니다.");
        DigBonus=Config.Bind("SkillAndLuckMatter 대체","땅파기 산출 보너스",true,"Map.MineFloor에서 생성되는 땅파기 산출물에 스킬+운 보너스를 적용합니다.");
        ChopBonus=Config.Bind("SkillAndLuckMatter 대체","벌목 산출 보너스",true,"TaskChopWood 완료 시 판자 산출물에 스킬+운 보너스를 적용합니다.");
        HarvestBonus=Config.Bind("SkillAndLuckMatter 대체","작물 수확 보너스",true,"GrowSystem.Harvest의 수확물에 스킬+운 보너스를 적용합니다.");
        SkillW=Config.Bind("SkillAndLuckMatter 대체","스킬 가중치",3,"활동 점수에서 스킬 가중치입니다.");
        LuckW=Config.Bind("SkillAndLuckMatter 대체","운 가중치",2,"활동 점수에서 운 가중치입니다.");
        ActivityCap=Config.Bind("SkillAndLuckMatter 대체","추가 산출 롤 상한",5,"한 산출물에 추가되는 최대 보너스 롤 수입니다.");

        h=new Harmony(G);
        PatchClass("범죄 목격",typeof(WitnessPatch));
        PatchClass("자물쇠",typeof(LockPatch));
        PatchClass("낚시 품질",typeof(FishPatch));
        PatchClass("낚시 희귀 보상",typeof(FishRareRewardPatch));
        PatchClass("카지노",typeof(CasinoPatch));
        PatchClass("훔치기 시도 난수",typeof(StealAttemptRollPatch));
        PatchClass("훔치기 중량",typeof(StealWeightNarrowPatch));
        PatchClass("씨앗",typeof(SeedLuckNarrowPatch));
        PatchClass("수정란",typeof(FertEggLuckNarrowPatch));
        PatchClass("스크래치",typeof(ScratchPrizeNarrowPatch));
        PatchClass("보물상자",typeof(TreasureRarityNarrowPatch));
        PatchClass("블랙마켓 희귀도 컨텍스트",typeof(BlackmarketContextPatch));
        PatchClass("블랙마켓 희귀도 승급",typeof(BlackmarketRarityPatch));
        PatchClass("몬스터 장비 난이도 운",typeof(MonsterEquipLuckPatch));
        PatchClass("사망 후 보너스 드롭",typeof(PostDeathBonusPatch));
        PatchClass("Finish 처치 컨텍스트",typeof(FinishKillContextPatch));
        PatchClass("전투 장비 보너스 드롭",typeof(CombatEquipmentBonusPatch));
        PatchClass("채광 직접 산출",typeof(MineBlockActivityPatch));
        PatchClass("땅파기 직접 산출",typeof(MineFloorActivityPatch));
        PatchClass("벌목 직접 산출",typeof(ChopActivityPatch));
        PatchClass("작물 직접 수확",typeof(HarvestActivityPatch));
        PatchClass("제작 재료 환급",typeof(CraftRefundPatch));
        Logger.LogInfo(N+" "+V+" loaded. SpawnLoot/Card.Die/PatchAll/전역 TrySmoothPick/StackTrace 사용 안 함.");
    }

    void PatchClass(string name,Type t){try{h!.CreateClassProcessor(t).Patch();Logger.LogInfo("[Luck] "+name+": 적용");}catch(Exception ex){Logger.LogWarning("[Luck] "+name+": 비활성 ("+ex.GetType().Name+") "+ex.Message);}}
    void OnDestroy(){h?.UnpatchSelf();}
    internal static int Luck(){int l=EClass.pc==null?1:EClass.pc.Evalue(78);return Math.Max(1,Math.Min(9999,l));}
    internal static int RelativeBonus(ConfigEntry<int> div,ConfigEntry<int> cap){return Math.Min(Math.Max(0,cap.Value),Math.Max(0,Luck()/Math.Max(1,div.Value)));}
    internal static int ReduceDenom(int d,int bonus){if(d<=1||bonus<=0)return d;long n=(long)d*100L;return Math.Max(1,(int)((n+99+bonus)/(100+bonus)));}
    internal static double ActivityScore(int skill){int sw=Math.Max(0,SkillW.Value),lw=Math.Max(0,LuckW.Value),d=sw+lw;if(d<=0)return 0;return (Math.Max(0,skill)*sw+Luck()*lw)/(double)d;}
    internal static double ActivityCurve(double x){if(x<=0)return 0;if(x<10)return x/10*.15;if(x<40)return .15+(x-10)/30*.35;if(x<100)return .5+(x-40)/60*.25;if(x<200)return .75+(x-100)/100*.25;return 1+(x-200)/100;}
    internal static int ActivityRolls(int skill){double p=ActivityCurve(ActivityScore(skill));int n=(int)Math.Floor(p);double f=p-n;if(f>0&&EClass.rnd(100000)<(int)(f*100000))n++;return Math.Min(Math.Max(0,ActivityCap.Value),Math.Max(0,n));}
}

[HarmonyPatch(typeof(Point),nameof(Point.TryWitnessCrime),new Type[]{typeof(Chara),typeof(Chara),typeof(int),typeof(Func<Chara,bool>)})]
static class WitnessPatch{static void Prefix(Chara criminal,ref Func<Chara,bool> funcWitness){if(!Plugin.Witness.Value||criminal==null||EClass.pc==null||criminal!=EClass.pc)return;int a=Math.Min(Plugin.WitnessCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.WitnessDiv.Value)));if(a<=0)return;var old=funcWitness;if(old==null)funcWitness=c=>EClass.rnd(10)==0&&EClass.rnd(100)>=a;else funcWitness=c=>old(c)&&EClass.rnd(100)>=a;}}

[HarmonyPatch(typeof(Trait),nameof(Trait.TryOpenLock),new Type[]{typeof(Chara),typeof(bool)})]
static class LockPatch{public sealed class S{public int lv;public bool changed;}static void Prefix(Trait __instance,Chara cc,out S __state){__state=new S();if(!Plugin.Lock.Value||cc==null||!cc.IsPC||__instance.owner==null)return;int cut=Math.Min(Plugin.LockCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.LockDiv.Value)));__state.lv=__instance.owner.c_lockLv;int n=Math.Max(0,__state.lv-cut);if(n<__state.lv){__instance.owner.c_lockLv=n;__state.changed=true;}}static Exception? Finalizer(Trait __instance,S __state,Exception? __exception){if(__state!=null&&__state.changed&&__instance.owner.c_lockLv>0)__instance.owner.c_lockLv=__state.lv;return __exception;}}

[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]
static class FishPatch{static void Postfix(Chara c,ref Thing __result){if(!Plugin.Fish.Value||c==null||!c.IsPC||__result==null||__result.category==null||!__result.category.IsChildOf("fish"))return;int a=Math.Min(Plugin.FishCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.FishDiv.Value)));if(__result.tier<3&&EClass.rnd(100)<a)__result.SetTier(Math.Min(3,__result.tier+1));}}

[HarmonyPatch(typeof(MiniGame),nameof(MiniGame.Deactivate),new Type[]{})]
static class CasinoPatch{static void Prefix(MiniGame __instance){if(!Plugin.Casino.Value||__instance?.balance==null||__instance.balance.changeCoin<=0)return;int a=Math.Min(Plugin.CasinoCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.CasinoDiv.Value)));if(EClass.rnd(100)<a)__instance.balance.changeCoin+=Math.Max(1,__instance.balance.changeCoin/2);}}

static class StealAttemptContext{[ThreadStatic]internal static int roll10000;[ThreadStatic]internal static bool hasRoll;}
[HarmonyPatch(typeof(AI_Steal),nameof(AI_Steal.Perform),new Type[]{})]
static class StealAttemptRollPatch{static void Prefix(){StealAttemptContext.roll10000=EClass.rnd(10000);StealAttemptContext.hasRoll=true;}}

[HarmonyPatch]
static class StealWeightNarrowPatch
{
    static readonly MethodInfo Getter=typeof(Card).GetProperty("ChildrenAndSelfWeight",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic)?.GetGetMethod(true)!;
    static readonly MethodInfo Helper=typeof(StealWeightNarrowPatch).GetMethod(nameof(EffectiveWeight),BindingFlags.Static|BindingFlags.NonPublic)!;
    static IEnumerable<MethodBase> TargetMethods(){foreach(var t in Nested(typeof(AI_Steal)))foreach(var m in t.GetMethods(BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic))if(!m.IsAbstract&&!m.ContainsGenericParameters&&CallsGetter(m))yield return m;}
    static IEnumerable<Type> Nested(Type root){foreach(var t in root.GetNestedTypes(BindingFlags.Public|BindingFlags.NonPublic)){yield return t;foreach(var c in Nested(t))yield return c;}}
    static bool CallsGetter(MethodBase m){if(Getter==null)return false;try{var il=m.GetMethodBody()?.GetILAsByteArray();if(il==null)return false;int token=Getter.MetadataToken;for(int i=0;i+4<il.Length;i++){byte op=il[i];if((op==0x28||op==0x6f)&&BitConverter.ToInt32(il,i+1)==token)return true;}}catch{}return false;}
    static long EffectiveWeight(Card target){long w=target==null?0:target.ChildrenAndSelfWeight;if(!Plugin.StealWeight.Value||target==null||EClass.pc==null)return w;long limit=(long)EClass.pc.Evalue(281)*200L+(long)EClass.pc.STR*100L+1000L;if(w<=limit||limit<=0)return w;int baseChance=Math.Min(Plugin.StealWeightCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.StealWeightDiv.Value)));if(baseChance<=0)return w;int chance=(int)Math.Max(1L,Math.Min(baseChance,(long)baseChance*limit/Math.Max(1L,w)));if(!StealAttemptContext.hasRoll){StealAttemptContext.roll10000=EClass.rnd(10000);StealAttemptContext.hasRoll=true;}return StealAttemptContext.roll10000<chance*100?limit:w;}
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions){foreach(var ci in instructions){if(Getter!=null&&ci.operand is MethodInfo m&&m==Getter&&(ci.opcode==OpCodes.Call||ci.opcode==OpCodes.Callvirt))yield return new CodeInstruction(OpCodes.Call,Helper);else yield return ci;}}
}

[HarmonyPatch(typeof(GrowSystem),nameof(GrowSystem.TryPopSeed),new Type[]{typeof(Chara)})]
static class SeedLuckNarrowPatch
{
    static readonly MethodInfo Rnd=typeof(EClass).GetMethod(nameof(EClass.rnd),new Type[]{typeof(int)})!;static readonly MethodInfo Helper=typeof(SeedLuckNarrowPatch).GetMethod(nameof(SeedRnd),BindingFlags.Static|BindingFlags.NonPublic)!;
    static int SeedRnd(int max,Chara c){if(!Plugin.SeedLuck.Value||c==null||!c.IsPC)return EClass.rnd(max);return EClass.rnd(Plugin.ReduceDenom(max,Plugin.RelativeBonus(Plugin.SeedDiv,Plugin.SeedCap)));}
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions){int count=0;foreach(var ci in instructions){if(ci.operand is MethodInfo m&&m==Rnd&&(ci.opcode==OpCodes.Call||ci.opcode==OpCodes.Callvirt)&&count++==0){yield return new CodeInstruction(OpCodes.Ldarg_1);yield return new CodeInstruction(OpCodes.Call,Helper);}else yield return ci;}}
}

[HarmonyPatch(typeof(Card),nameof(Card.MakeEgg),new Type[]{typeof(bool),typeof(int),typeof(bool),typeof(int),typeof(BlessedState?)})]
static class FertEggLuckNarrowPatch{static void Prefix(Card __instance,ref int fertChance){if(!Plugin.FertEggLuck.Value||__instance==null||!__instance.IsPCFaction||fertChance<=1)return;fertChance=Plugin.ReduceDenom(fertChance,Plugin.RelativeBonus(Plugin.EggDiv,Plugin.EggCap));}}

[HarmonyPatch]
static class ScratchPrizeNarrowPatch
{
    static readonly MethodInfo Rnd=typeof(EClass).GetMethod(nameof(EClass.rnd),new Type[]{typeof(int)})!;static readonly MethodInfo Helper=typeof(ScratchPrizeNarrowPatch).GetMethod(nameof(ScratchRnd),BindingFlags.Static|BindingFlags.NonPublic)!;
    static IEnumerable<MethodBase> TargetMethods(){foreach(var m in Methods(typeof(TraitCrafter)))if(m.Name.Contains("g__Prize"))yield return m;}static IEnumerable<MethodBase> Methods(Type root){foreach(var m in root.GetMethods(BindingFlags.Static|BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))yield return m;foreach(var t in root.GetNestedTypes(BindingFlags.Public|BindingFlags.NonPublic))foreach(var m in Methods(t))yield return m;}
    static int ScratchRnd(int max){if(!Plugin.ScratchLuck.Value||max<=1)return EClass.rnd(max);return EClass.rnd(Plugin.ReduceDenom(max,Plugin.RelativeBonus(Plugin.ScratchDiv,Plugin.ScratchCap)));}
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions){foreach(var ci in instructions){if(ci.operand is MethodInfo m&&m==Rnd&&(ci.opcode==OpCodes.Call||ci.opcode==OpCodes.Callvirt))yield return new CodeInstruction(OpCodes.Call,Helper);else yield return ci;}}
}

[HarmonyPatch]
static class TreasureRarityNarrowPatch
{
    static readonly MethodInfo Rnd=typeof(EClass).GetMethod(nameof(EClass.rnd),new Type[]{typeof(int)})!;static readonly MethodInfo Helper=typeof(TreasureRarityNarrowPatch).GetMethod(nameof(TreasureRnd),BindingFlags.Static|BindingFlags.NonPublic)!;
    static IEnumerable<MethodBase> TargetMethods(){foreach(var m in Methods(typeof(ThingGen)))if(m.Name.Contains("g__SetRarity"))yield return m;}static IEnumerable<MethodBase> Methods(Type root){foreach(var m in root.GetMethods(BindingFlags.Static|BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))yield return m;foreach(var t in root.GetNestedTypes(BindingFlags.Public|BindingFlags.NonPublic))foreach(var m in Methods(t))yield return m;}
    static int TreasureRnd(int max){if(!Plugin.TreasureLuck.Value||(max!=100&&max!=20))return EClass.rnd(max);return EClass.rnd(Plugin.ReduceDenom(max,Plugin.RelativeBonus(Plugin.TreasureDiv,Plugin.TreasureCap)));}
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions){foreach(var ci in instructions){if(ci.operand is MethodInfo m&&m==Rnd&&(ci.opcode==OpCodes.Call||ci.opcode==OpCodes.Callvirt))yield return new CodeInstruction(OpCodes.Call,Helper);else yield return ci;}}
}

static class BonusDropCore
{
    internal static void Process(Chara c){if(!Plugin.PostDeathBonus.Value||c==null||c.IsPCFaction)return;var ap=AttackProcess.Current;if(ap==null||ap.TC!=c||ap.CC==null||!ap.CC.IsPCFactionOrMinion)return;Chara killer=ap.CC.Chara;if(killer==null)return;if(Plugin.GeneBonus.Value&&!HasAt(c,"gene"))TryGene(c,killer);if(Plugin.MaterialBonus.Value)TryMaterials(c);if(Plugin.UniqueBonus.Value)TryUnique(c);}
    static bool HasAt(Chara c,string id){foreach(Card x in c.pos.ListCards())if(x!=null&&x.isThing&&x.id==id)return true;return false;}
    static bool RollExtra(double baseP,int rel){if(baseP<=0||baseP>=1||rel<=0)return false;double target=Math.Min(.95,baseP*(1.0+rel/100.0));double q=(target-baseP)/(1.0-baseP);return q>0&&EClass.rnd(1000000)<(int)(q*1000000.0);}
    static void Drop(Chara c,string id){Thing t=ThingGen.Create(id,-1,c.LV);EClass._zone.AddCard(t,c.pos);}
    static void TryGene(Chara c,Chara killer){int sw=Math.Max(0,Plugin.AnatomyW.Value),lw=Math.Max(0,Plugin.AnatomyLuckW.Value),d=sw+lw;if(d<=0)return;int mix=(Math.Max(0,killer.Evalue(290))*sw+Plugin.Luck()*lw)/d;int rel=Math.Min(Math.Max(0,Plugin.GeneCap.Value),Math.Max(0,mix/Math.Max(1,Plugin.GeneDiv.Value)));if(RollExtra(1.0/200.0,rel)){Thing g=c.MakeGene();EClass._zone.AddCard(g,c.pos);}}
    static void TryMaterials(Chara c){int rel=Plugin.RelativeBonus(Plugin.MaterialDiv,Plugin.MaterialCap);if(c.IsMachine){TryMat(c,"memory_chip",200,rel);bool scrap=c.HasElement(1248);TryMat(c,scrap?"scrap":"microchip",20,rel);TryMat(c,scrap?"bolt":"battery",15,rel);}else{if(c.IsAnimal){TryMat(c,"fang",15,rel);TryMat(c,"skin",10,rel);}TryMat(c,"offal",20,rel);TryMat(c,"heart",20,rel);}switch(c.id){case "golem_wood":TryMat(c,"crystal_earth",30,rel);break;case "golem_fish":case "golem_stone":TryMat(c,"crystal_sun",30,rel);break;case "golem_steel":TryMat(c,"crystal_mana",30,rel);break;}}
    static void TryMat(Chara c,string id,int denom,int rel){if(!HasAt(c,id)&&RollExtra(1.0/denom,rel))Drop(c,id);}
    static void TryUnique(Chara c){int rel=Plugin.RelativeBonus(Plugin.UniqueDiv,Plugin.UniqueCap);var seen=new HashSet<string>();Action<string> one=entry=>{if(string.IsNullOrEmpty(entry))return;var p=entry.Split('/');if(p.Length<2||!int.TryParse(p[1],out int n)||n<=0||n>=1000)return;string id=p[0];if(!seen.Add(id)||HasAt(c,id))return;if(RollExtra(n/1000.0,rel))Drop(c,id);};if(c.sourceCard!=null&&c.sourceCard.loot!=null)foreach(var e in c.sourceCard.loot)one(e);if(c.race!=null&&c.race.loot!=null)foreach(var e in c.race.loot)one(e);}
}
[HarmonyPatch(typeof(ZoneEventManager),nameof(ZoneEventManager.OnCharaDie),new Type[]{typeof(Chara)})]
static class PostDeathBonusPatch{static void Postfix(Chara c){try{BonusDropCore.Process(c);}catch(Exception ex){Plugin.I.Logger.LogWarning("[Luck] 사망 후 보너스 드롭 런타임 예외: "+ex.GetType().Name+" "+ex.Message);}}}

static class ActivityOutputCore
{
    internal static void Apply(Thing t,Chara c,int skillId,bool enabled){if(!Plugin.ActivityBonus.Value||!enabled||t==null||c==null||!c.IsPC)return;int skill=skillId==0?Math.Max(c.Evalue(250),c.Evalue(286)):c.Evalue(skillId);int rolls=Plugin.ActivityRolls(skill);if(rolls<=0)return;int baseN=Math.Max(1,t.Num);t.ModNum(baseN*rolls);}
    internal static void PickMine(Map map,Point p,Thing t,Chara c){Apply(t,c,220,Plugin.MineBonus.Value);map.TrySmoothPick(p,t,c);}
    internal static void PickDig(Map map,Point p,Thing t,Chara c){Apply(t,c,230,Plugin.DigBonus.Value);map.TrySmoothPick(p,t,c);}
    internal static void PickChop(Map map,Point p,Thing t,Chara c){Apply(t,c,225,Plugin.ChopBonus.Value);map.TrySmoothPick(p,t,c);}
    internal static void PickHarvest(Map map,Point p,Thing t,Chara c){Apply(t,c,0,Plugin.HarvestBonus.Value);map.TrySmoothPick(p,t,c);}
}

static class ActivityTranspilerUtil
{
    internal static readonly MethodInfo Pick=typeof(Map).GetMethod(nameof(Map.TrySmoothPick),new Type[]{typeof(Point),typeof(Thing),typeof(Chara)})!;
    internal static IEnumerable<CodeInstruction> Replace(IEnumerable<CodeInstruction> instructions,MethodInfo helper){int n=0;foreach(var ci in instructions){if(ci.operand is MethodInfo m&&m==Pick&&(ci.opcode==OpCodes.Call||ci.opcode==OpCodes.Callvirt)){n++;yield return new CodeInstruction(OpCodes.Call,helper);}else yield return ci;}if(n==0)Plugin.I.Logger.LogWarning("[Luck] 직접 산출 패턴: TrySmoothPick 호출을 찾지 못함");}
    internal static bool CallsPick(MethodBase m){try{var il=m.GetMethodBody()?.GetILAsByteArray();if(il==null)return false;int token=Pick.MetadataToken;for(int i=0;i+4<il.Length;i++){byte op=il[i];if((op==0x28||op==0x6f)&&BitConverter.ToInt32(il,i+1)==token)return true;}}catch{}return false;}
    internal static IEnumerable<MethodBase> Methods(Type root){foreach(var m in root.GetMethods(BindingFlags.Static|BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))yield return m;foreach(var t in root.GetNestedTypes(BindingFlags.Public|BindingFlags.NonPublic))foreach(var m in Methods(t))yield return m;}
}

[HarmonyPatch(typeof(Map),nameof(Map.MineBlock),new Type[]{typeof(Point),typeof(bool),typeof(Chara),typeof(bool)})]
static class MineBlockActivityPatch{static readonly MethodInfo H=typeof(ActivityOutputCore).GetMethod(nameof(ActivityOutputCore.PickMine))!;static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> x)=>ActivityTranspilerUtil.Replace(x,H);}

[HarmonyPatch(typeof(Map),nameof(Map.MineFloor),new Type[]{typeof(Point),typeof(Chara),typeof(bool),typeof(bool)})]
static class MineFloorActivityPatch{static readonly MethodInfo H=typeof(ActivityOutputCore).GetMethod(nameof(ActivityOutputCore.PickDig))!;static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> x)=>ActivityTranspilerUtil.Replace(x,H);}

[HarmonyPatch]
static class ChopActivityPatch
{
    static readonly MethodInfo H=typeof(ActivityOutputCore).GetMethod(nameof(ActivityOutputCore.PickChop))!;
    static IEnumerable<MethodBase> TargetMethods(){foreach(var m in ActivityTranspilerUtil.Methods(typeof(TaskChopWood)))if(ActivityTranspilerUtil.CallsPick(m))yield return m;}
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> x)=>ActivityTranspilerUtil.Replace(x,H);
}

[HarmonyPatch(typeof(GrowSystem),nameof(GrowSystem.Harvest),new Type[]{typeof(Chara)})]
static class HarvestActivityPatch{static readonly MethodInfo H=typeof(ActivityOutputCore).GetMethod(nameof(ActivityOutputCore.PickHarvest))!;static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> x)=>ActivityTranspilerUtil.Replace(x,H);}


[HarmonyPatch(typeof(LayerCraft),nameof(LayerCraft.GetReqIngredient),new Type[]{typeof(int)})]
static class CraftRefundPatch
{
    static ConfigEntry<bool>? Enabled;
    static ConfigEntry<bool>? FlavorLog;
    static ConfigEntry<int>? RefundCap;
    static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("SkillAndLuckMatter 대체","제작/가공 재료 환급",true,"실제 제작 소비 단계에서만 스킬+운에 따라 일부 재료 소비를 줄입니다. UI 필요량과 제작 가능 판정은 원본 그대로입니다.");
        RefundCap=Plugin.I.Config.Bind("SkillAndLuckMatter 대체","제작 재료 환급률 상한",50,"재료 1개당 환급 확률의 상한(%)입니다. 안전을 위해 최소 1개는 항상 소비합니다.");
        FlavorLog=Plugin.I.Config.Bind("SkillAndLuckMatter 대체","제작 환급 플레이버 로그",true,"운으로 실제 재료 소비가 줄었을 때 게임 플레이 로그에 짧은 메시지를 남깁니다.");
        return true;
    }
    static void Postfix(LayerCraft __instance,int index,ref int __result)
    {
        if(Enabled==null||!Enabled.Value||__result<=1||EClass.pc==null||__instance==null||__instance.recipe==null)return;
        Element req=__instance.recipe.source?.GetReqSkill();
        int skill=req==null?0:EClass.pc.Evalue(req.id);
        double chance=Plugin.ActivityCurve(Plugin.ActivityScore(skill))/10.0;
        chance=Math.Min(Math.Max(0,RefundCap?.Value??50)/100.0,Math.Max(0,chance));
        if(chance<=0)return;
        int saved=0;
        int maxSave=__result-1;
        int threshold=(int)(chance*100000.0);
        for(int i=0;i<maxSave;i++)if(EClass.rnd(100000)<threshold)saved++;
        if(saved>0){__result=Math.Max(1,__result-saved);if(FlavorLog!=null&&FlavorLog.Value)Msg.SayRaw("손끝에 행운이 스쳤다. 재료 "+saved+"개를 아꼈다.");}
    }
}


[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]
static class FishRareRewardPatch
{
    static ConfigEntry<bool>? Enabled,AncientBook,Medal,CoinGroup,SpecialReward,BigCatch;
    static ConfigEntry<int>? LuckDiv,LuckCap;
    static readonly MethodInfo Rnd=typeof(EClass).GetMethod(nameof(EClass.rnd),new Type[]{typeof(int)})!;
    static readonly MethodInfo Helper=typeof(FishRareRewardPatch).GetMethod(nameof(LuckRnd),BindingFlags.Static|BindingFlags.NonPublic)!;

    static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("낚시 운","희귀 보상 운",true,"Makefish 내부에서 확인된 희귀 보상 첫 관문만 운으로 보정합니다.");
        AncientBook=Plugin.I.Config.Bind("낚시 운","고대책 운",true,"고대책의 원래 1/30 판정을 운으로 완화합니다.");
        Medal=Plugin.I.Config.Bind("낚시 운","메달 운",true,"메달의 첫 1/40 관문만 운으로 완화하며 낚시 스킬 조건은 원본 그대로 유지합니다.");
        CoinGroup=Plugin.I.Config.Bind("낚시 운","코인류 운",true,"플래티넘/스크래치/카지노/가챠 코인 묶음의 원래 1/35 관문만 운으로 완화합니다. 내부 보상 종류 비율은 바꾸지 않습니다.");
        SpecialReward=Plugin.I.Config.Bind("낚시 운","특수 희귀품 운",true,"코인류 관문 안의 특수 희귀품 1/50 판정을 운으로 완화합니다.");
        BigCatch=Plugin.I.Config.Bind("낚시 운","대어 운",true,"대어 판정의 rnd(100) 범위만 운으로 완화합니다. 지형/거점 보정값은 원본 그대로입니다.");
        LuckDiv=Plugin.I.Config.Bind("낚시 운","희귀 보상 운 분모",20,"운이 이 값만큼 오를 때 희귀 보상 상대 확률이 1% 증가합니다.");
        LuckCap=Plugin.I.Config.Bind("낚시 운","희귀 보상 상대 보너스 상한",100,"희귀 보상 상대 확률 증가 상한입니다.");
        return true;
    }

    static bool IsLdc(CodeInstruction c,int v)
    {
        if(c.opcode==OpCodes.Ldc_I4)return c.operand is int x&&x==v;
        if(c.opcode==OpCodes.Ldc_I4_S)return Convert.ToInt32(c.operand)==v;
        if(v>=0&&v<=8){OpCode[] a={OpCodes.Ldc_I4_0,OpCodes.Ldc_I4_1,OpCodes.Ldc_I4_2,OpCodes.Ldc_I4_3,OpCodes.Ldc_I4_4,OpCodes.Ldc_I4_5,OpCodes.Ldc_I4_6,OpCodes.Ldc_I4_7,OpCodes.Ldc_I4_8};return c.opcode==a[v];}
        return false;
    }
    static bool IsRnd(CodeInstruction c)=>c.operand is MethodInfo m&&m==Rnd&&(c.opcode==OpCodes.Call||c.opcode==OpCodes.Callvirt);
    static int LuckRnd(int max,Chara c,int kind)
    {
        if(Enabled==null||!Enabled.Value||c==null||!c.IsPC||max<=1)return EClass.rnd(max);
        bool on=kind switch{1=>AncientBook?.Value??false,2=>Medal?.Value??false,3=>CoinGroup?.Value??false,4=>SpecialReward?.Value??false,5=>BigCatch?.Value??false,_=>false};
        if(!on)return EClass.rnd(max);
        int rel=Plugin.RelativeBonus(LuckDiv!,LuckCap!);
        return EClass.rnd(Plugin.ReduceDenom(max,rel));
    }
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var src=new List<CodeInstruction>(instructions);
        int i30=-1,i35=-1,i50=-1,i100=-1,i40first=-1,c40=0;
        for(int i=1;i<src.Count;i++)
        {
            if(!IsRnd(src[i]))continue;
            if(IsLdc(src[i-1],30)&&i30<0)i30=i;
            else if(IsLdc(src[i-1],35)&&i35<0)i35=i;
            else if(IsLdc(src[i-1],50)&&i50<0)i50=i;
            else if(IsLdc(src[i-1],100)&&i100<0)i100=i;
            else if(IsLdc(src[i-1],40)){c40++;if(i40first<0)i40first=i;}
        }
        if(i30<0||i35<0||i50<0||i100<0||i40first<0||c40<2)
        {
            Plugin.I.Logger.LogWarning($"[Luck] 낚시 희귀 보상 IL 패턴 불일치: 30={i30}, 35={i35}, 50={i50}, 100={i100}, 40count={c40}. 희귀 보상 패치를 적용하지 않습니다.");
            foreach(var x in src)yield return x;
            yield break;
        }
        var map=new Dictionary<int,int>{{i30,1},{i40first,2},{i35,3},{i50,4},{i100,5}};
        for(int i=0;i<src.Count;i++)
        {
            if(map.TryGetValue(i,out int kind))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldc_I4,kind);
                yield return new CodeInstruction(OpCodes.Call,Helper);
            }
            else yield return src[i];
        }
        Plugin.I.Logger.LogInfo("[Luck] 낚시 희귀 보상 좁은 패치 적용: 고대책/메달 첫 관문/코인류/특수 희귀품/대어");
    }
}


static class BlackmarketContext
{
    [ThreadStatic] internal static int depth;
    [ThreadStatic] internal static bool active;
}

[HarmonyPatch]
static class BlackmarketContextPatch
{
    static MethodBase? target;
    static bool Prepare()
    {
        target=FindTarget();
        if(target==null)
        {
            Plugin.I.Logger.LogWarning("[Luck] 블랙마켓 희귀도: shop_blackmarket 생성 메서드를 찾지 못해 비활성화합니다.");
            return false;
        }
        return true;
    }
    static MethodBase TargetMethod()=>target!;

    static MethodBase? FindTarget()
    {
        foreach(var m in typeof(Trait).GetMethods(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))
        {
            if(HasString(m,"shop_blackmarket"))return m;
        }
        foreach(var t in typeof(Trait).GetNestedTypes(BindingFlags.Public|BindingFlags.NonPublic))
        foreach(var m in t.GetMethods(BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic))
        {
            if(HasString(m,"shop_blackmarket"))return m;
        }
        return null;
    }

    static bool HasString(MethodBase m,string value)
    {
        try
        {
            var il=m.GetMethodBody()?.GetILAsByteArray();
            if(il==null)return false;
            var mod=m.Module;
            for(int i=0;i<il.Length;)
            {
                ushort code=il[i++];
                if(code==0xFE){if(i>=il.Length)break;code=(ushort)(0xFE00|il[i++]);}
                OpCode op=OpCodes.Nop;
                foreach(var f in typeof(OpCodes).GetFields(BindingFlags.Public|BindingFlags.Static))
                {
                    if(f.GetValue(null) is OpCode o && (ushort)o.Value==code){op=o;break;}
                }
                int size=0;
                switch(op.OperandType)
                {
                    case OperandType.InlineNone:size=0;break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar:size=1;break;
                    case OperandType.InlineVar:size=2;break;
                    case OperandType.InlineI:
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineField:
                    case OperandType.InlineMethod:
                    case OperandType.InlineSig:
                    case OperandType.InlineString:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.ShortInlineR:size=4;break;
                    case OperandType.InlineI8:
                    case OperandType.InlineR:size=8;break;
                    case OperandType.InlineSwitch:
                        if(i+4>il.Length)return false;
                        int n=BitConverter.ToInt32(il,i);size=4+n*4;break;
                }
                if(op==OpCodes.Ldstr && i+4<=il.Length)
                {
                    int tok=BitConverter.ToInt32(il,i);
                    if(mod.ResolveString(tok)==value)return true;
                }
                i+=size;
            }
        }
        catch{}
        return false;
    }

    static void Prefix(object __instance)
    {
        BlackmarketContext.depth++;
        if(BlackmarketContext.depth!=1)return;
        BlackmarketContext.active=false;
        try
        {
            if(__instance is Trait tr)
                BlackmarketContext.active = tr.ShopType==ShopType.Blackmarket || tr.ShopType==ShopType.Exotic;
        }
        catch{}
    }
    static void Finalizer()
    {
        BlackmarketContext.depth=Math.Max(0,BlackmarketContext.depth-1);
        if(BlackmarketContext.depth==0)BlackmarketContext.active=false;
    }
}

[HarmonyPatch(typeof(CardBlueprint),nameof(CardBlueprint.SetRarity),new Type[]{typeof(Rarity)})]
static class BlackmarketRarityPatch
{
    static ConfigEntry<bool>? Enabled,FlavorLog;
    static ConfigEntry<int>? LuckDiv,LuckCap;
    static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("장비 운","블랙마켓 장비 희귀도 운",true,"블랙마켓/Exotic 상점의 장비 희귀도 결정 직후에만 Luck으로 한 단계 승급 기회를 줍니다. Thing.OnCreate는 패치하지 않습니다.");
        LuckDiv=Plugin.I.Config.Bind("장비 운","블랙마켓 희귀도 운 분모",20,"운/이 값(%)을 기본 승급 확률로 사용합니다.");
        LuckCap=Plugin.I.Config.Bind("장비 운","블랙마켓 희귀도 승급 상한",50,"한 번의 희귀도 승급 확률 상한(%)입니다.");
        FlavorLog=Plugin.I.Config.Bind("장비 운","블랙마켓 희귀도 플레이버 로그",true,"Luck으로 실제 희귀도가 승급됐을 때 게임 플레이 로그에 짧은 메시지를 표시합니다.");
        return true;
    }
    static void Prefix(ref Rarity q)
    {
        if(Enabled==null||!Enabled.Value||!BlackmarketContext.active)return;
        if(q>=Rarity.Mythical||q>=Rarity.Artifact)return;
        int chance=Math.Min(Math.Max(0,LuckCap?.Value??50),Math.Max(0,Plugin.Luck()/Math.Max(1,LuckDiv?.Value??20)));
        if(chance<=0||EClass.rnd(100)>=chance)return;
        Rarity old=q;
        if(q<=Rarity.Normal)q=Rarity.Superior;
        else if(q==Rarity.Crude)q=Rarity.Superior;
        else if(q==Rarity.Superior)q=Rarity.Legendary;
        else if(q==Rarity.Legendary)q=Rarity.Mythical;
        if(q!=old && FlavorLog!=null&&FlavorLog.Value)Msg.SayRaw("행운이 좋은 물건을 끌어당겼다. 블랙마켓 장비의 품질이 한층 높아졌다.");
    }
}


static class FinishKillContext
{
    [ThreadStatic] internal static int depth;
    [ThreadStatic] internal static Chara? killer;
    [ThreadStatic] internal static Chara? target;
    internal static bool Matches(Chara c)=>depth>0&&target==c&&killer!=null;
}

[HarmonyPatch(typeof(Chara),nameof(Chara.TryNeckHunt),new Type[]{typeof(Chara),typeof(int),typeof(bool)})]
static class FinishKillContextPatch
{
    static void Prefix(Chara __instance,Chara TC)
    {
        FinishKillContext.depth++;
        if(FinishKillContext.depth==1)
        {
            FinishKillContext.killer=__instance;
            FinishKillContext.target=TC;
        }
    }
    static Exception? Finalizer(Exception? __exception)
    {
        FinishKillContext.depth=Math.Max(0,FinishKillContext.depth-1);
        if(FinishKillContext.depth==0)
        {
            FinishKillContext.killer=null;
            FinishKillContext.target=null;
        }
        return __exception;
    }
}

static class CombatEquipmentBonusCore
{
    static ConfigEntry<bool>? Enabled,FlavorLog;
    static ConfigEntry<int>? LuckDiv,LuckCap,CritBonus,FinishBonus,ExecutionerPerLv,ExecutionerCap,OverkillCap,TotalCap;

    internal static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("사망 후 보너스 드롭","전투 장비 보너스 드롭",true,"SpawnLoot가 끝난 뒤에도 사망한 적 인벤토리에 남아 있는 장비 중 최대 1개에 Luck+크리티컬+Finish+처형자+오버킬 기반 추가 드롭 판정을 합니다.");
        LuckDiv=Plugin.I.Config.Bind("사망 후 보너스 드롭","전투 장비 Luck 분모",10,"Luck을 이 값으로 나눈 수치를 장비 추가 드롭 보너스 점수로 사용합니다.");
        LuckCap=Plugin.I.Config.Bind("사망 후 보너스 드롭","전투 장비 Luck 기여 상한",100,"Luck이 기여하는 보너스 점수 상한입니다. 100점은 약 추가 1%p에 해당합니다.");
        CritBonus=Plugin.I.Config.Bind("사망 후 보너스 드롭","크리티컬 처치 보너스",50,"마지막 실제 공격이 크리티컬이면 더하는 점수입니다. 50점은 약 추가 0.5%p입니다.");
        FinishBonus=Plugin.I.Config.Bind("사망 후 보너스 드롭","Finish 처치 보너스",100,"TryNeckHunt의 Finish 처치이면 더하는 점수입니다. 100점은 약 추가 1%p입니다.");
        ExecutionerPerLv=Plugin.I.Config.Bind("사망 후 보너스 드롭","처형자 레벨당 보너스",25,"처형자 피트(1420) 1레벨당 더하는 점수입니다.");
        ExecutionerCap=Plugin.I.Config.Bind("사망 후 보너스 드롭","처형자 보너스 상한",100,"처형자 피트가 기여하는 점수 상한입니다.");
        OverkillCap=Plugin.I.Config.Bind("사망 후 보너스 드롭","오버킬 보너스 상한",100,"최대 HP 대비 음수 HP 비율을 점수로 사용하며 이 값에서 제한합니다.");
        TotalCap=Plugin.I.Config.Bind("사망 후 보너스 드롭","전투 장비 총 보너스 상한",300,"모든 전투 보너스 점수의 합계 상한입니다. 300점은 약 추가 3%p입니다.");
        FlavorLog=Plugin.I.Config.Bind("사망 후 보너스 드롭","전투 장비 플레이버 로그",true,"Luck 보너스로 실제 장비가 추가 드롭됐을 때 게임 로그에 짧은 문구를 표시합니다.");
        return true;
    }

    static bool ResolveKiller(Chara victim,out Chara? killer,out bool crit,out bool finish)
    {
        killer=null;crit=false;finish=false;
        if(FinishKillContext.Matches(victim))
        {
            killer=FinishKillContext.killer;
            finish=true;
            var apf=AttackProcess.Current;
            if(apf!=null&&apf.TC==victim&&apf.CC==killer)crit=apf.crit;
            return killer!=null&&(killer.IsPCFaction||killer.IsPCFactionOrMinion);
        }
        var ap=AttackProcess.Current;
        if(ap==null||ap.TC!=victim||ap.CC==null)return false;
        killer=ap.CC.Chara;
        if(killer==null||(!ap.CC.IsPCFaction&&!ap.CC.IsPCFactionOrMinion))return false;
        crit=ap.crit;
        return true;
    }

    internal static void Process(Chara victim)
    {
        if(Enabled==null||!Enabled.Value||victim==null||victim.IsPCFaction)return;
        if(!ResolveKiller(victim,out Chara? killer,out bool crit,out bool finish)||killer==null)return;

        var candidates=new List<Thing>();
        foreach(Thing t in victim.things)
        {
            if(t==null||!t.IsEquipmentOrRanged)continue;
            if(t.rarity>=Rarity.Artifact||t.IsUnique||t.isGifted)continue;
            candidates.Add(t);
        }
        if(candidates.Count==0)return;

        int score=Math.Min(Math.Max(0,LuckCap?.Value??100),Math.Max(0,Plugin.Luck()/Math.Max(1,LuckDiv?.Value??10)));
        if(crit)score+=Math.Max(0,CritBonus?.Value??50);
        if(finish)score+=Math.Max(0,FinishBonus?.Value??100);
        int ex=Math.Max(0,killer.Evalue(1420))*Math.Max(0,ExecutionerPerLv?.Value??25);
        score+=Math.Min(Math.Max(0,ExecutionerCap?.Value??100),ex);
        int mh=Math.Max(1,victim.MaxHP);
        int over=Math.Max(0,-victim.hp*100/mh);
        score+=Math.Min(Math.Max(0,OverkillCap?.Value??100),over);
        score=Math.Min(Math.Max(0,TotalCap?.Value??300),score);
        if(score<=0||EClass.rnd(10000)>=score)return;

        Thing drop=candidates[EClass.rnd(candidates.Count)];
        drop.isHidden=false;
        drop.isNPCProperty=false;
        drop.SetInt(116);
        EClass._zone.AddCard(drop,victim.pos);
        if(FlavorLog!=null&&FlavorLog.Value)
        {
            string why=finish?"마무리의 행운이 전리품을 남겼다.":(crit?"결정적인 일격이 뜻밖의 전리품을 끌어냈다.":"행운이 적의 장비 하나를 놓치지 않았다.");
            Msg.SayRaw(why);
        }
    }
}

[HarmonyPatch(typeof(ZoneEventManager),nameof(ZoneEventManager.OnCharaDie),new Type[]{typeof(Chara)})]
static class CombatEquipmentBonusPatch
{
    static bool Prepare()=>CombatEquipmentBonusCore.Prepare();
    static void Postfix(Chara c)
    {
        try{CombatEquipmentBonusCore.Process(c);}
        catch(Exception ex){Plugin.I.Logger.LogWarning("[Luck] 전투 장비 보너스 런타임 예외: "+ex.GetType().Name+" "+ex.Message);}
    }
}


[HarmonyPatch]
static class MonsterEquipLuckPatch
{
    static MethodBase? target;
    static ConfigEntry<bool>? Enabled,EnemyOnly,FlavorLog;
    static ConfigEntry<int>? LuckDiv,LuckCap,DoubleUpgradeThreshold;

    static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("장비 운","몬스터 장비가 플레이어 운에 반응",false,"고위험/고보상 옵션입니다. 적이 장비를 생성할 때 플레이어 Luck으로 장비 희귀도를 올릴 수 있어 전투 난이도와 잠재 전리품 가치가 함께 증가합니다. 기본값은 꺼짐입니다.");
        EnemyOnly=Plugin.I.Config.Bind("장비 운","적대 몬스터만 적용",true,"플레이어 진영/중립 NPC를 제외하고 적대 개체의 장비 생성에만 적용합니다.");
        LuckDiv=Plugin.I.Config.Bind("장비 운","몬스터 장비 운 분모",40,"희귀도 1단계 승급 확률은 Luck/이 값(%)입니다.");
        LuckCap=Plugin.I.Config.Bind("장비 운","몬스터 장비 승급 확률 상한",35,"희귀도 1단계 승급 확률 상한(%)입니다.");
        DoubleUpgradeThreshold=Plugin.I.Config.Bind("장비 운","2단계 승급 시작 Luck",2000,"이 Luck 이상부터 첫 승급에 성공했을 때 두 번째 승급을 추가로 판정합니다. 0이면 2단계 승급을 사용하지 않습니다.");
        FlavorLog=Plugin.I.Config.Bind("장비 운","몬스터 장비 플레이버 로그",false,"몬스터 장비 희귀도가 실제로 상승했을 때 게임 플레이 로그에 메시지를 표시합니다. 몬스터 생성이 잦으면 로그가 많아질 수 있어 기본값은 꺼짐입니다.");
        target=typeof(Chara).GetMethod("SetEQQuality",BindingFlags.Instance|BindingFlags.NonPublic);
        if(target==null)
        {
            Plugin.I.Logger.LogWarning("[Luck] 몬스터 장비 운: Chara.SetEQQuality를 찾지 못해 비활성화합니다.");
            return false;
        }
        return true;
    }

    static MethodBase TargetMethod()=>target!;

    static void Postfix(Chara __instance)
    {
        try
        {
            if(Enabled==null||!Enabled.Value||__instance==null||EClass.pc==null)return;
            if(__instance.IsPCFaction)return;
            if(EnemyOnly!=null&&EnemyOnly.Value)
            {
                try{if(__instance.OriginalHostility!=Hostility.Enemy)return;}catch{return;}
            }
            var bp=CardBlueprint.current;
            if(bp==null||bp!=CardBlueprint.CharaGenEQ)return;
            var old=bp.rarity;
            if(old==Rarity.Artifact||old==Rarity.Mythical||old==Rarity.Random)return;
            int chance=Math.Min(Math.Max(0,LuckCap?.Value??35),Math.Max(0,Plugin.Luck()/Math.Max(1,LuckDiv?.Value??40)));
            if(chance<=0||EClass.rnd(100)>=chance)return;
            var now=Upgrade(old);
            int threshold=DoubleUpgradeThreshold?.Value??2000;
            if(threshold>0&&Plugin.Luck()>=threshold&&now!=Rarity.Mythical&&now!=Rarity.Artifact)
            {
                int extra=Math.Min(chance,Math.Max(1,(Plugin.Luck()-threshold)/Math.Max(1,(LuckDiv?.Value??40)*2)));
                if(EClass.rnd(100)<extra)now=Upgrade(now);
            }
            if(now==old)return;
            bp.rarity=now;
            if(FlavorLog!=null&&FlavorLog.Value)Msg.SayRaw("기묘한 행운이 적의 장비마저 날카롭게 벼렸다.");
        }
        catch(Exception ex)
        {
            Plugin.I.Logger.LogWarning("[Luck] 몬스터 장비 운 런타임 예외: "+ex.GetType().Name+" "+ex.Message);
        }
    }

    static Rarity Upgrade(Rarity r)
    {
        if(r==Rarity.Crude||r==Rarity.Normal)return Rarity.Superior;
        if(r==Rarity.Superior)return Rarity.Legendary;
        if(r==Rarity.Legendary)return Rarity.Mythical;
        return r;
    }
}

}
