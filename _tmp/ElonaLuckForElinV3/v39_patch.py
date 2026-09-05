from pathlib import Path

root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.8", V="3.8.0"','N="Elona Luck for Elin v3.9", V="3.9.0"')

anchor='        PatchClass("사망 후 보너스 드롭",typeof(PostDeathBonusPatch));'
insert='''        PatchClass("Finish 처치 컨텍스트",typeof(FinishKillContextPatch));\n        PatchClass("전투 장비 보너스 드롭",typeof(CombatEquipmentBonusPatch));'''
if 'CombatEquipmentBonusPatch' not in s:
    s=s.replace(anchor,anchor+'\n'+insert)

if 'static class FinishKillContext' not in s:
    block=r'''

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
'''
    pos=s.rfind('\n}')
    s=s[:pos]+block+s[pos:]

p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj'
cs.write_text(cs.read_text().replace('<Version>3.8.0</Version>','<Version>3.9.0</Version>'))

pkg=root/'package.xml'
pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.8','Elona Luck for Elin v3.9'))

rd=root/'README_KR.md'
t=rd.read_text().replace('# Elona Luck for Elin v3.8','# Elona Luck for Elin v3.9')
if '## v3.9 전투 장비 보너스 드롭' not in t:
    t += '''\n\n## v3.9 전투 장비 보너스 드롭\n- `Card.SpawnLoot`와 `Card.Die`는 여전히 패치하지 않습니다.\n- 원본 SpawnLoot가 끝난 뒤 `ZoneEventManager.OnCharaDie`에서 사망한 적의 인벤토리에 남아 있는 장비만 추가 드롭 후보로 봅니다. 이미 원본에서 떨어진 장비는 후보에 들어오지 않습니다.\n- Artifact/Unique/선물 장비는 제외합니다.\n- 한 처치에서 이 기능으로 추가되는 장비는 최대 1개입니다.\n- 보너스 점수는 Luck + 실제 크리티컬 + Finish(TryNeckHunt) + 처형자(1420) + 오버킬로 계산합니다.\n- 기본 설정에서 점수 100 = 약 추가 1%p, 총 상한 300 = 약 추가 3%p입니다. 원본 일반 장비 1% 드롭에 상대 +300%를 적용한 것과 비슷한 최대 증가폭입니다.\n- Finish는 `TryNeckHunt`가 실행되는 동안에만 짧은 컨텍스트를 잡아 식별하며 `Card.Die`의 attackSource를 읽거나 변경하지 않습니다.\n- 공격자를 확정할 수 없는 환경사/상태이상 사망에는 전투 장비 보너스를 적용하지 않습니다.\n- 실제 추가 장비 드롭이 발생하면 게임 로그에 플레이버 문구를 표시하며 설정에서 끌 수 있습니다.\n- 몬스터 장비의 생성 품질 자체는 건드리지 않습니다. 적이 살아 있을 때 더 강해지는 부작용을 피하기 위해서입니다.\n'''
rd.write_text(t)

# compile stubs for v3.9 only; production assembly uses real Elin types
stub=root/'refs/Elin/Stub.cs'
x=stub.read_text()
if 'public class ThingContainer' not in x:
    x=x.replace('public class Category{public bool IsChildOf(string s)=>false;}','public class Category{public bool IsChildOf(string s)=>false;}\npublic class ThingContainer:System.Collections.Generic.List<Thing>{}')
x=x.replace(' public Category category=new Category();public Point pos=new Point();public string id="";public bool isThing=true;', ' public Category category=new Category();public Point pos=new Point();public string id="";public bool isThing=true;public ThingContainer things=new ThingContainer();')
x=x.replace(' public bool IsEquipment=>false;public bool IsPC=>true;public bool IsPCFaction=>true;public bool IsPCFactionOrMinion=>true;public Rarity rarity;', ' public bool IsEquipment=>false;public bool IsEquipmentOrRanged=>false;public bool IsUnique=>false;public bool isGifted;public bool isHidden;public bool isNPCProperty;public bool IsPC=>true;public bool IsPCFaction=>true;public bool IsPCFactionOrMinion=>true;public Rarity rarity;')
x=x.replace(' public virtual int Evalue(int id)=>0;public Thing MakeEgg', ' public virtual int Evalue(int id)=>0;public void SetInt(int id,int v=0){}public Thing MakeEgg')
x=x.replace('public class Chara:Card{public int DEX;public int STR;', 'public class Chara:Card{public int DEX;public int STR;public bool TryNeckHunt(Chara TC,int power,bool harvest=false)=>false;')
stub.write_text(x)
