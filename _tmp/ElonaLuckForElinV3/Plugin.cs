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
    public const string G="sivwen.elin.elonaluck", N="Elona Luck for Elin v3.4", V="3.4.0";
    internal static Plugin I=null!;
    Harmony? h;

    internal static ConfigEntry<bool> Witness=null!,Lock=null!,Fish=null!,Casino=null!,StealWeight=null!;
    internal static ConfigEntry<bool> SeedLuck=null!,TreasureLuck=null!,ScratchLuck=null!,FertEggLuck=null!;
    internal static ConfigEntry<bool> PostDeathBonus=null!,GeneBonus=null!,MaterialBonus=null!,UniqueBonus=null!;
    internal static ConfigEntry<int> WitnessDiv=null!,WitnessCap=null!,LockDiv=null!,LockCap=null!,FishDiv=null!,FishCap=null!,CasinoDiv=null!,CasinoCap=null!;
    internal static ConfigEntry<int> StealWeightDiv=null!,StealWeightCap=null!,SeedDiv=null!,SeedCap=null!,TreasureDiv=null!,TreasureCap=null!,ScratchDiv=null!,ScratchCap=null!,EggDiv=null!,EggCap=null!;
    internal static ConfigEntry<int> AnatomyW=null!,AnatomyLuckW=null!,GeneDiv=null!,GeneCap=null!,MaterialDiv=null!,MaterialCap=null!,UniqueDiv=null!,UniqueCap=null!;

    void Awake()
    {
        I=this;
        Witness=Config.Bind("범죄/발각 운","범죄 목격 회피 운",true,"범죄 목격 판정이 성공했을 때 운에 따라 추가 회피 판정을 합니다.");
        WitnessDiv=Config.Bind("범죄/발각 운","목격 회피 운 분모",25,"기본 회피 확률은 운/이 값(%)입니다.");
        WitnessCap=Config.Bind("범죄/발각 운","목격 회피 확률 상한",60,"목격 회피 확률의 최대값입니다.");

        Lock=Config.Bind("자물쇠 운","자물쇠 따기 운",true,"자물쇠 따기 시에만 유효 자물쇠 레벨을 운으로 낮춥니다.");
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

        SeedLuck=Config.Bind("채집/농사 운","씨앗 회수 운",true,"TryPopSeed의 원래 수동 씨앗 회수 RNG 한 곳만 보정합니다.");
        SeedDiv=Config.Bind("채집/농사 운","씨앗 운 분모",25,"운이 이 값만큼 오를 때 씨앗 회수 상대 확률이 1% 증가합니다.");
        SeedCap=Config.Bind("채집/농사 운","씨앗 보너스 상한",60,"씨앗 회수 상대 확률 증가 상한입니다.");

        TreasureLuck=Config.Bind("보물상자 운","보물 장비 희귀도 운",true,"CreateTreasureContent의 로컬 SetRarity RNG만 보정합니다.");
        TreasureDiv=Config.Bind("보물상자 운","보물 희귀도 운 분모",25,"운이 이 값만큼 오를 때 희귀도 판정의 상대 보정이 1% 증가합니다.");
        TreasureCap=Config.Bind("보물상자 운","보물 희귀도 보너스 상한",75,"보물상자 장비 희귀도 상대 보정 상한입니다.");

        ScratchLuck=Config.Bind("스크래치 운","스크래치 당첨 운",true,"TraitCrafter의 로컬 Prize() RNG만 보정합니다.");
        ScratchDiv=Config.Bind("스크래치 운","스크래치 운 분모",25,"운이 이 값만큼 오를 때 각 Prize 상대 당첨 확률이 1% 증가합니다.");
        ScratchCap=Config.Bind("스크래치 운","스크래치 보너스 상한",75,"스크래치 상대 당첨 확률 증가 상한입니다.");

        FertEggLuck=Config.Bind("생산 운","수정란 운",true,"플레이어 진영 Card.MakeEgg의 fertChance 분모만 보정합니다.");
        EggDiv=Config.Bind("생산 운","수정란 운 분모",25,"운이 이 값만큼 오를 때 수정란 상대 확률이 1% 증가합니다.");
        EggCap=Config.Bind("생산 운","수정란 보너스 상한",75,"수정란 상대 확률 증가 상한입니다.");

        PostDeathBonus=Config.Bind("사망 후 보너스 드롭","안전 보너스 드롭 사용",true,"SpawnLoot를 건드리지 않고 Chara.Die가 끝난 뒤 ZoneEventManager.OnCharaDie 통지 후 보너스를 추가합니다.");
        GeneBonus=Config.Bind("사망 후 보너스 드롭","유전자 보너스",true,"원본 유전자가 나오지 않은 경우에만 해부학+운으로 추가 유전자 판정을 합니다.");
        MaterialBonus=Config.Bind("사망 후 보너스 드롭","일반 소재 보너스",true,"원본 소재가 같은 칸에 없을 때만 Luck 기반 추가 판정을 합니다.");
        UniqueBonus=Config.Bind("사망 후 보너스 드롭","몬스터 고유 드롭 보너스",true,"sourceCard/race loot 항목이 원본에서 나오지 않은 경우에만 Luck 기반 추가 판정을 합니다.");
        AnatomyW=Config.Bind("사망 후 보너스 드롭","해부학 가중치",3,"유전자 추가 판정에서 해부학의 가중치입니다.");
        AnatomyLuckW=Config.Bind("사망 후 보너스 드롭","운 가중치",2,"유전자 추가 판정에서 운의 가중치입니다.");
        GeneDiv=Config.Bind("사망 후 보너스 드롭","유전자 보너스 강도 분모",2,"해부학+운 혼합값을 이 값으로 나눈 %만큼 원래 유전자 확률을 상대적으로 높입니다.");
        GeneCap=Config.Bind("사망 후 보너스 드롭","유전자 상대 보너스 상한",200,"유전자 추가 확률 계산용 상대 보너스 상한입니다.");
        MaterialDiv=Config.Bind("사망 후 보너스 드롭","일반 소재 운 분모",50,"운이 이 값만큼 오를 때 원래 소재 확률이 상대적으로 1% 증가합니다.");
        MaterialCap=Config.Bind("사망 후 보너스 드롭","일반 소재 보너스 상한",100,"일반 소재 상대 보너스 상한입니다.");
        UniqueDiv=Config.Bind("사망 후 보너스 드롭","고유 드롭 운 분모",10,"운이 이 값만큼 오를 때 원래 고유 드롭 확률이 상대적으로 1% 증가합니다.");
        UniqueCap=Config.Bind("사망 후 보너스 드롭","고유 드롭 보너스 상한",300,"고유 드롭 상대 보너스 상한입니다.");

        h=new Harmony(G);
        PatchClass("범죄 목격",typeof(WitnessPatch));
        PatchClass("자물쇠",typeof(LockPatch));
        PatchClass("낚시 품질",typeof(FishPatch));
        PatchClass("카지노",typeof(CasinoPatch));
        PatchClass("훔치기 시도 난수",typeof(StealAttemptRollPatch));
        PatchClass("훔치기 중량",typeof(StealWeightNarrowPatch));
        PatchClass("씨앗",typeof(SeedLuckNarrowPatch));
        PatchClass("수정란",typeof(FertEggLuckNarrowPatch));
        PatchClass("스크래치",typeof(ScratchPrizeNarrowPatch));
        PatchClass("보물상자",typeof(TreasureRarityNarrowPatch));
        PatchClass("사망 후 보너스 드롭",typeof(PostDeathBonusPatch));
        Logger.LogInfo(N+" "+V+" loaded. SpawnLoot/Card.Die/PatchAll 사용 안 함.");
    }

    void PatchClass(string name,Type t)
    {
        try{h!.CreateClassProcessor(t).Patch();Logger.LogInfo("[Luck] "+name+": 적용");}
        catch(Exception ex){Logger.LogWarning("[Luck] "+name+": 비활성 ("+ex.GetType().Name+") "+ex.Message);}
    }
    void OnDestroy(){h?.UnpatchSelf();}
    internal static int Luck(){int l=EClass.pc==null?1:EClass.pc.Evalue(78);return Math.Max(1,Math.Min(9999,l));}
    internal static int RelativeBonus(ConfigEntry<int> div,ConfigEntry<int> cap){return Math.Min(Math.Max(0,cap.Value),Math.Max(0,Luck()/Math.Max(1,div.Value)));}
    internal static int ReduceDenom(int d,int bonus){if(d<=1||bonus<=0)return d;long n=(long)d*100L;int r=(int)((n+99+bonus)/(100+bonus));return Math.Max(1,r);}
}

[HarmonyPatch(typeof(Point),nameof(Point.TryWitnessCrime),new Type[]{typeof(Chara),typeof(Chara),typeof(int),typeof(Func<Chara,bool>)})]
static class WitnessPatch
{
    static void Prefix(Chara criminal,ref Func<Chara,bool> funcWitness)
    {
        if(!Plugin.Witness.Value||criminal==null||EClass.pc==null||criminal!=EClass.pc)return;
        int a=Math.Min(Plugin.WitnessCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.WitnessDiv.Value)));
        if(a<=0)return;var old=funcWitness;
        if(old==null)funcWitness=c=>EClass.rnd(10)==0&&EClass.rnd(100)>=a;else funcWitness=c=>old(c)&&EClass.rnd(100)>=a;
    }
}

[HarmonyPatch(typeof(Trait),nameof(Trait.TryOpenLock),new Type[]{typeof(Chara),typeof(bool)})]
static class LockPatch
{
    public sealed class S{public int lv;public bool changed;}
    static void Prefix(Trait __instance,Chara cc,out S __state){__state=new S();if(!Plugin.Lock.Value||cc==null||!cc.IsPC||__instance.owner==null)return;int cut=Math.Min(Plugin.LockCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.LockDiv.Value)));__state.lv=__instance.owner.c_lockLv;int n=Math.Max(0,__state.lv-cut);if(n<__state.lv){__instance.owner.c_lockLv=n;__state.changed=true;}}
    static Exception? Finalizer(Trait __instance,S __state,Exception? __exception){if(__state!=null&&__state.changed&&__instance.owner.c_lockLv>0)__instance.owner.c_lockLv=__state.lv;return __exception;}
}

[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]
static class FishPatch
{
    static void Postfix(Chara c,ref Thing __result){if(!Plugin.Fish.Value||c==null||!c.IsPC||__result==null||__result.category==null||!__result.category.IsChildOf("fish"))return;int a=Math.Min(Plugin.FishCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.FishDiv.Value)));if(__result.tier<3&&EClass.rnd(100)<a)__result.SetTier(Math.Min(3,__result.tier+1));}
}

[HarmonyPatch(typeof(MiniGame),nameof(MiniGame.Deactivate),new Type[]{})]
static class CasinoPatch
{
    static void Prefix(MiniGame __instance){if(!Plugin.Casino.Value||__instance?.balance==null||__instance.balance.changeCoin<=0)return;int a=Math.Min(Plugin.CasinoCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.CasinoDiv.Value)));if(EClass.rnd(100)<a)__instance.balance.changeCoin+=Math.Max(1,__instance.balance.changeCoin/2);}
}

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
    static readonly MethodInfo Rnd=typeof(EClass).GetMethod(nameof(EClass.rnd),new Type[]{typeof(int)})!;
    static readonly MethodInfo Helper=typeof(SeedLuckNarrowPatch).GetMethod(nameof(SeedRnd),BindingFlags.Static|BindingFlags.NonPublic)!;
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
    internal static void Process(Chara c)
    {
        if(!Plugin.PostDeathBonus.Value||c==null||c.IsPCFaction)return;
        var ap=AttackProcess.Current;if(ap==null||ap.TC!=c||ap.CC==null||!ap.CC.IsPCFactionOrMinion)return;
        Chara killer=ap.CC.Chara;if(killer==null)return;
        if(Plugin.GeneBonus.Value&&!HasAt(c,"gene"))TryGene(c,killer);
        if(Plugin.MaterialBonus.Value)TryMaterials(c);
        if(Plugin.UniqueBonus.Value)TryUnique(c);
    }
    static bool HasAt(Chara c,string id){foreach(Card x in c.pos.ListCards())if(x!=null&&x.isThing&&x.id==id)return true;return false;}
    static bool RollExtra(double baseP,int rel){if(baseP<=0||baseP>=1||rel<=0)return false;double target=Math.Min(.95,baseP*(1.0+rel/100.0));double q=(target-baseP)/(1.0-baseP);return q>0&&EClass.rnd(1000000)<(int)(q*1000000.0);}
    static void Drop(Chara c,string id){Thing t=ThingGen.Create(id,-1,c.LV);EClass._zone.AddCard(t,c.pos);}
    static void TryGene(Chara c,Chara killer){int sw=Math.Max(0,Plugin.AnatomyW.Value),lw=Math.Max(0,Plugin.AnatomyLuckW.Value),d=sw+lw;if(d<=0)return;int mix=(Math.Max(0,killer.Evalue(290))*sw+Plugin.Luck()*lw)/d;int rel=Math.Min(Math.Max(0,Plugin.GeneCap.Value),Math.Max(0,mix/Math.Max(1,Plugin.GeneDiv.Value)));if(RollExtra(1.0/200.0,rel)){Thing g=c.MakeGene();EClass._zone.AddCard(g,c.pos);}}
    static void TryMaterials(Chara c){int rel=Plugin.RelativeBonus(Plugin.MaterialDiv,Plugin.MaterialCap);if(c.IsMachine){TryMat(c,"memory_chip",200,rel);bool scrap=c.HasElement(1248);TryMat(c,scrap?"scrap":"microchip",20,rel);TryMat(c,scrap?"bolt":"battery",15,rel);}else{if(c.IsAnimal){TryMat(c,"fang",15,rel);TryMat(c,"skin",10,rel);}TryMat(c,"offal",20,rel);TryMat(c,"heart",20,rel);}switch(c.id){case "golem_wood":TryMat(c,"crystal_earth",30,rel);break;case "golem_fish":case "golem_stone":TryMat(c,"crystal_sun",30,rel);break;case "golem_steel":TryMat(c,"crystal_mana",30,rel);break;}}
    static void TryMat(Chara c,string id,int denom,int rel){if(!HasAt(c,id)&&RollExtra(1.0/denom,rel))Drop(c,id);}
    static void TryUnique(Chara c){int rel=Plugin.RelativeBonus(Plugin.UniqueDiv,Plugin.UniqueCap);var seen=new HashSet<string>();Action<string> one=entry=>{if(string.IsNullOrEmpty(entry))return;var p=entry.Split('/');if(p.Length<2||!int.TryParse(p[1],out int n)||n<=0||n>=1000)return;string id=p[0];if(!seen.Add(id)||HasAt(c,id))return;if(RollExtra(n/1000.0,rel))Drop(c,id);};if(c.sourceCard!=null&&c.sourceCard.loot!=null)foreach(var e in c.sourceCard.loot)one(e);if(c.race!=null&&c.race.loot!=null)foreach(var e in c.race.loot)one(e);}
}

[HarmonyPatch(typeof(ZoneEventManager),nameof(ZoneEventManager.OnCharaDie),new Type[]{typeof(Chara)})]
static class PostDeathBonusPatch
{
    static void Postfix(Chara c){try{BonusDropCore.Process(c);}catch(Exception ex){Plugin.I.Logger.LogWarning("[Luck] 사망 후 보너스 드롭 런타임 예외: "+ex.GetType().Name+" "+ex.Message);}}
}

}
