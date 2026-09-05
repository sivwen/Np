from pathlib import Path
root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.9", V="3.9.0"','N="Elona Luck for Elin v3.11", V="3.11.0"')

old='static Exception? Finalizer(Trait __instance,S __state,Exception? __exception){if(__state!=null&&__state.changed&&__instance.owner.c_lockLv>0)__instance.owner.c_lockLv=__state.lv;return __exception;}'
new='static Exception? Finalizer(Trait __instance,S __state,Exception? __exception){if(__state!=null&&__state.changed&&__instance.owner!=null)__instance.owner.c_lockLv=__state.lv;return __exception;}'
if old not in s: raise SystemExit('Lock finalizer pattern not found')
s=s.replace(old,new)
anchor='    internal static int Luck(){int l=EClass.pc==null?1:EClass.pc.Evalue(78);return Math.Max(1,Math.Min(9999,l));}\n'
helper='    internal static bool HasAssembly(string token){try{foreach(var a in AppDomain.CurrentDomain.GetAssemblies()){string n=a.GetName().Name??"";if(n.IndexOf(token,StringComparison.OrdinalIgnoreCase)>=0)return true;}}catch{}return false;}\n'
if helper not in s:
    if anchor not in s: raise SystemExit('Luck helper anchor not found')
    s=s.replace(anchor,anchor+helper)
old='        PatchClass("낚시 희귀 보상",typeof(FishRareRewardPatch));'
new='        if(HasAssembly("LuckyFishing"))Logger.LogWarning("[Luck] LuckyFishing 감지: 낚시 희귀 보상 transpiler는 충돌 방지를 위해 자동 비활성화합니다. 물고기 tier Postfix는 유지합니다.");else PatchClass("낚시 희귀 보상",typeof(FishRareRewardPatch));'
if old not in s: raise SystemExit('Fish rare registration not found')
s=s.replace(old,new)
a='static class PostDeathBonusPatch{static void Postfix(Chara c){try{BonusDropCore.Process(c);}catch(Exception ex){Plugin.I.Logger.LogWarning("[Luck] 사망 후 보너스 드롭 런타임 예외: "+ex.GetType().Name+" "+ex.Message);}}}'
b='static class PostDeathBonusPatch{[HarmonyPriority(Priority.Last)] static void Postfix(Chara c){try{BonusDropCore.Process(c);}catch(Exception ex){Plugin.I.Logger.LogWarning("[Luck] 사망 후 보너스 드롭 런타임 예외: "+ex.GetType().Name+" "+ex.Message);}}}'
if a not in s: raise SystemExit('PostDeathBonusPatch pattern not found')
s=s.replace(a,b)
a='    static void Postfix(Chara c)\n    {\n        try{CombatEquipmentBonusCore.Process(c);}'
b='    [HarmonyPriority(Priority.Last)]\n    static void Postfix(Chara c)\n    {\n        try{CombatEquipmentBonusCore.Process(c);}'
if a not in s: raise SystemExit('CombatEquipmentBonusPatch pattern not found')
s=s.replace(a,b)
marker='        h=new Harmony(G);\n'
diag='        if(HasAssembly("KillDropEffect"))Logger.LogInfo("[Luck] KillDropEffect 감지: 사망 후 보너스는 Priority.Last로 실행합니다.");\n        if(HasAssembly("BetterSlime"))Logger.LogInfo("[Luck] Better Slime 계열 감지: 비전투/포식 사망은 공격자 검증 실패 시 보너스 드롭을 건너뜁니다.");\n'
if diag not in s:s=s.replace(marker,diag+marker)

reg='        PatchClass("블랙마켓 희귀도 승급",typeof(BlackmarketRarityPatch));\n'
add='        PatchClass("몬스터 장비 생성 컨텍스트",typeof(MonsterEquipContextPatch));\n        PatchClass("몬스터 장비 희귀도 난이도 연동",typeof(MonsterEquipRarityPatch));\n'
if reg not in s: raise SystemExit('monster equip registration anchor not found')
s=s.replace(reg,reg+add)

block='''

static class MonsterEquipLuckContext
{
    [ThreadStatic] internal static int depth;
    [ThreadStatic] internal static Chara? actor;
    [ThreadStatic] internal static bool active;
}

[HarmonyPatch(typeof(Chara),nameof(Chara.RestockEquip),new Type[]{typeof(bool)})]
static class MonsterEquipContextPatch
{
    static void Prefix(Chara __instance,bool onCreate)
    {
        MonsterEquipLuckContext.depth++;
        if(MonsterEquipLuckContext.depth!=1)return;
        MonsterEquipLuckContext.actor=__instance;
        MonsterEquipLuckContext.active=false;
        try{MonsterEquipLuckContext.active=onCreate&&__instance!=null&&!__instance.IsPCFactionOrMinion&&__instance.IsHostile();}
        catch{MonsterEquipLuckContext.active=false;}
    }
    static Exception? Finalizer(Exception? __exception)
    {
        MonsterEquipLuckContext.depth=Math.Max(0,MonsterEquipLuckContext.depth-1);
        if(MonsterEquipLuckContext.depth==0){MonsterEquipLuckContext.actor=null;MonsterEquipLuckContext.active=false;}
        return __exception;
    }
}

[HarmonyPatch]
static class MonsterEquipRarityPatch
{
    static ConfigEntry<bool>? Enabled,LevelScale;
    static ConfigEntry<int>? LuckDiv,LuckCap;
    static MethodBase TargetMethod()=>typeof(Chara).GetMethod("SetEQQuality",BindingFlags.Instance|BindingFlags.NonPublic)!;
    static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("장비 운","적 장비 난이도 연동",true,"플레이어의 운이 높을수록 초기 생성되는 적대 몬스터의 일반 장비가 한 단계 높은 희귀도로 생성될 수 있습니다. 더 강한 적이 더 좋은 전리품을 들고 나오는 위험/보상 옵션입니다.");
        LuckDiv=Plugin.I.Config.Bind("장비 운","적 장비 운 분모",50,"기본 승급 확률은 운/이 값(%)입니다.");
        LuckCap=Plugin.I.Config.Bind("장비 운","적 장비 승급 확률 상한",25,"레벨 보정 전 장비 1개당 희귀도 승급 확률 상한(%)입니다.");
        LevelScale=Plugin.I.Config.Bind("장비 운","적 레벨 난이도 보정",true,"낮은 레벨 적에게는 승급 확률을 줄이고 높은 레벨 적에서 원래 확률에 가까워지게 합니다.");
        return true;
    }
    static void Postfix(Chara __instance)
    {
        if(Enabled==null||!Enabled.Value||!MonsterEquipLuckContext.active||MonsterEquipLuckContext.actor!=__instance||__instance==null)return;
        var bp=CardBlueprint.current;
        if(bp==null)return;
        Rarity q=bp.rarity;
        if(q>=Rarity.Mythical||q>=Rarity.Artifact)return;
        int chance=Math.Min(Math.Max(0,LuckCap?.Value??25),Math.Max(0,Plugin.Luck()/Math.Max(1,LuckDiv?.Value??50)));
        if(LevelScale!=null&&LevelScale.Value){int factor=Math.Min(100,25+Math.Max(0,__instance.LV)/2);chance=chance*factor/100;}
        if(chance<=0||EClass.rnd(100)>=chance)return;
        if(q<=Rarity.Normal||q==Rarity.Crude)bp.rarity=Rarity.Superior;
        else if(q==Rarity.Superior)bp.rarity=Rarity.Legendary;
        else if(q==Rarity.Legendary)bp.rarity=Rarity.Mythical;
    }
}
'''
pos=s.rfind('\n}')
if pos<0: raise SystemExit('namespace end not found')
s=s[:pos]+block+s[pos:]
p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj';cs.write_text(cs.read_text().replace('<Version>3.9.0</Version>','<Version>3.11.0</Version>'))
pkg=root/'package.xml';pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.9','Elona Luck for Elin v3.11'))
rd=root/'README_KR.md';t=rd.read_text().replace('# Elona Luck for Elin v3.9','# Elona Luck for Elin v3.11');t+='\n\n## v3.11 적 장비 난이도 연동\n- 초기 생성되는 적대 몬스터의 RestockEquip -> SetEQQuality 경로에만 적용합니다.\n- PC 진영/미니언/비적대 NPC/재입고에는 적용하지 않습니다.\n- 고정 희귀도를 직접 지정하는 EQ_ID 장비는 원본 코드가 이후 다시 희귀도를 덮어쓰므로 영향을 받지 않습니다.\n- 한 장비당 최대 1단계, Mythical까지만 승급하며 Artifact는 만들지 않습니다.\n- 기본 확률은 Luck/50%, 상한 25%, 낮은 레벨 적은 추가 감쇠됩니다.\n- 더 강한 적이 더 좋은 장비를 들고 나올 수 있고 안전한 전투 장비 보너스 드롭과 위험/보상이 연결됩니다.\n- v3.10 안정화도 모두 포함합니다.\n';rd.write_text(t)

st=root/'refs/Elin/Stub.cs';h=st.read_text();h=h.replace('public class Chara:Card{public int DEX;public int STR;','public class Chara:Card{public int DEX;public int STR;public bool IsHostile()=>true;public void RestockEquip(bool onCreate){}');h=h.replace('public static class CardBlueprint{public static void SetRarity(Rarity q=Rarity.Normal){}}','public class CardBlueprintState{public Rarity rarity;}public static class CardBlueprint{public static CardBlueprintState current=new CardBlueprintState();public static void SetRarity(Rarity q=Rarity.Normal){current.rarity=q;}}');st.write_text(h)
hs=root/'refs/Harmony/Stub.cs';h=hs.read_text();
if 'class HarmonyPriority' not in h:h=h.replace('namespace HarmonyLib{','namespace HarmonyLib{public static class Priority{public const int Last=0;}[AttributeUsage(AttributeTargets.Method|AttributeTargets.Class)]public sealed class HarmonyPriority:Attribute{public HarmonyPriority(int p){}}')
hs.write_text(h)
