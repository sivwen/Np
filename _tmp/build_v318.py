from pathlib import Path
import runpy,re

runpy.run_path('_tmp/build_v317.py', run_name='__main__')
root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.17", V="3.17.0"','N="Elona Luck for Elin v3.18", V="3.18.0"')

# Korean enum names are intentional: Mod Config GUIs can expose these values without
# another custom UI layer.
anchor='namespace ElonaLuckForElinV3\n{\n'
enum='''namespace ElonaLuckForElinV3\n{\npublic enum DeathLuckMode { 마지막가해자, 파티최고, 파티합산 }\n'''
if anchor not in s: raise SystemExit('namespace anchor not found')
s=s.replace(anchor,enum,1)

# Thread-scoped Luck override is used only while post-death material/gene/unique bonus
# processing runs, so those older helpers can use the newly resolved death Luck source
# without changing unrelated fishing/shop/crafting Luck calls.
old='    internal static int Luck(){int l=EClass.pc==null?1:EClass.pc.Evalue(78);return Math.Max(1,Math.Min(9999,l));}\n'
new='''    [ThreadStatic] internal static int LuckOverride;\n    internal static int Luck(){if(LuckOverride>0)return Math.Max(1,Math.Min(9999,LuckOverride));int l=EClass.pc==null?1:EClass.pc.Evalue(78);return Math.Max(1,Math.Min(9999,l));}\n'''
if old not in s: raise SystemExit('Luck helper pattern not found')
s=s.replace(old,new,1)

# Plugin-level death attribution settings.
field_anchor='    internal static ConfigEntry<int> SkillW=null!,LuckW=null!,ActivityCap=null!;\n'
fields='''    internal static ConfigEntry<int> SkillW=null!,LuckW=null!,ActivityCap=null!;\n    internal static ConfigEntry<DeathLuckMode> DeathLuckSource=null!;\n    internal static ConfigEntry<int> PartyLuckSumCap=null!,RecentDamageTTL=null!;\n    internal static ConfigEntry<bool> RecentDamageFallback=null!;\n'''
if field_anchor not in s: raise SystemExit('config field anchor not found')
s=s.replace(field_anchor,fields,1)

bind_anchor='        PostDeathBonus=Config.Bind("사망 후 보너스 드롭","안전 보너스 드롭 사용",true,"SpawnLoot를 건드리지 않고 ZoneEventManager.OnCharaDie 종료 후 보너스를 추가합니다.");\n'
binds='''        DeathLuckSource=Config.Bind("사망 후 보너스 드롭","사망 보너스 Luck 기준",DeathLuckMode.파티최고,"마지막가해자=최근 실제 가해자의 운, 파티최고=현재 플레이어 파티 중 가장 높은 운, 파티합산=플레이어+현재 파티원의 운 합계(별도 상한 적용)입니다.");\n        PartyLuckSumCap=Config.Bind("사망 후 보너스 드롭","파티 합산 Luck 상한",9999,"파티합산 모드의 최종 Luck 상한입니다. 합산은 강력하므로 밸런스가 과하면 낮추세요.");\n        RecentDamageFallback=Config.Bind("사망 후 보너스 드롭","최근 가해자 환경/도트 귀속",true,"직접 AttackProcess가 끊긴 환경사/도트/비전투 사망에서도 최근에 실제 피해를 준 PC 파티 측 캐릭터를 짧게 기억해 Luck 보너스만 귀속합니다.");\n        RecentDamageTTL=Config.Bind("사망 후 보너스 드롭","최근 가해자 유효 턴",20,"최근 가해자 기록이 유효한 피해자 행동 턴 수입니다. 오래된 공격을 환경사에 잘못 귀속하지 않도록 제한합니다.");\n        PostDeathBonus=Config.Bind("사망 후 보너스 드롭","안전 보너스 드롭 사용",true,"SpawnLoot를 건드리지 않고 ZoneEventManager.OnCharaDie 종료 후 보너스를 추가합니다.");\n'''
if bind_anchor not in s: raise SystemExit('post-death bind anchor not found')
s=s.replace(bind_anchor,binds,1)

# Register metadata-only damage attribution patch before death consumers.
reg_anchor='        PatchClass("사망 후 보너스 드롭",typeof(PostDeathBonusPatch));\n'
reg='''        PatchClass("최근 피해 가해자 기록",typeof(LastDamageAttributionPatch));\n        PatchClass("사망 후 보너스 드롭",typeof(PostDeathBonusPatch));\n'''
if reg_anchor not in s: raise SystemExit('post-death patch registration anchor not found')
s=s.replace(reg_anchor,reg,1)

# Route general death bonuses through the same resolved attribution/Luck source.
if 'BonusDropCore.Process(c);' not in s: raise SystemExit('BonusDropCore.Process call not found')
s=s.replace('BonusDropCore.Process(c);','DeathLuckResolver.ProcessGeneralBonus(c);',1)

# Upgrade combat equipment resolver to support recent-attacker fallback and party Luck modes.
old='''    static bool ResolveKiller(Chara victim,out Chara? killer,out bool crit)\n    {\n        killer=null;crit=false;\n        var ap=AttackProcess.Current;\n        if(ap==null||ap.TC!=victim||ap.CC==null)return false;\n        killer=ap.CC.Chara;\n        if(killer==null||(!ap.CC.IsPCFaction&&!ap.CC.IsPCFactionOrMinion))return false;\n        crit=ap.crit;\n        return true;\n    }'''
new='''    static bool ResolveKiller(Chara victim,out Chara? killer,out bool direct,out bool crit)\n        =>DeathLuckResolver.TryResolve(victim,out killer,out direct,out crit);'''
if old not in s: raise SystemExit('v3.17 ResolveKiller pattern not found')
s=s.replace(old,new,1)

old='        if(!ResolveKiller(victim,out Chara? killer,out bool crit)||killer==null)return;\n'
new='        if(!ResolveKiller(victim,out Chara? killer,out bool direct,out bool crit)||killer==null)return;\n'
if old not in s: raise SystemExit('v3.17 ResolveKiller call not found')
s=s.replace(old,new,1)

old='        int killerLuck=Math.Max(1,Math.Min(9999,killer.Evalue(78)));\n'
new='        int killerLuck=DeathLuckResolver.GetLuck(killer);\n'
if old not in s: raise SystemExit('killerLuck pattern not found')
s=s.replace(old,new,1)

# Direct-hit bonuses must not be retroactively awarded to an environmental/DOT death.
old='''        if(crit)score+=Math.Max(0,CritBonus?.Value??50);\n        int ex=Math.Max(0,killer.Evalue(1420))*Math.Max(0,ExecutionerPerLv?.Value??25);\n        score+=Math.Min(Math.Max(0,ExecutionerCap?.Value??100),ex);\n        int mh=Math.Max(1,victim.MaxHP);\n        int over=Math.Max(0,-victim.hp*100/mh);\n        score+=Math.Min(Math.Max(0,OverkillCap?.Value??100),over);'''
new='''        if(direct)\n        {\n            if(crit)score+=Math.Max(0,CritBonus?.Value??50);\n            int ex=Math.Max(0,killer.Evalue(1420))*Math.Max(0,ExecutionerPerLv?.Value??25);\n            score+=Math.Min(Math.Max(0,ExecutionerCap?.Value??100),ex);\n            int mh=Math.Max(1,victim.MaxHP);\n            int over=Math.Max(0,-victim.hp*100/mh);\n            score+=Math.Min(Math.Max(0,OverkillCap?.Value??100),over);\n        }'''
if old not in s: raise SystemExit('direct bonus block pattern not found')
s=s.replace(old,new,1)

# Insert attribution core before CombatEquipmentBonusCore.
insert_anchor='static class CombatEquipmentBonusCore\n'
if insert_anchor not in s: raise SystemExit('CombatEquipmentBonusCore anchor not found')
block=r'''
sealed class LastDamageRecord
{
    internal Chara attacker=null!;
    internal int victimTurn;
    internal Zone zone=null!;
}

static class LastDamageAttribution
{
    static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Chara,LastDamageRecord> table=new System.Runtime.CompilerServices.ConditionalWeakTable<Chara,LastDamageRecord>();

    internal static void Observe(Card target,long dmg,Card? origin)
    {
        if(dmg<=0||target==null||!target.isChara)return;
        Chara victim=target.Chara;
        if(victim==null)return;
        Chara? attacker=origin?.Chara;
        var ap=AttackProcess.Current;
        if(attacker==null&&ap!=null&&ap.TC==victim&&ap.CC!=null)attacker=ap.CC.Chara;
        if(attacker==null)return; // environment/condition tick keeps the most recent character attribution
        table.Remove(victim);
        if(!attacker.IsPCFaction&&!attacker.IsPCFactionOrMinion)return; // a newer non-party character hit supersedes the old party hit
        table.Add(victim,new LastDamageRecord{attacker=attacker,victimTurn=victim.turn,zone=EClass._zone});
    }

    internal static bool TryGet(Chara victim,out Chara? attacker)
    {
        attacker=null;
        if(!Plugin.RecentDamageFallback.Value||victim==null)return false;
        if(!table.TryGetValue(victim,out LastDamageRecord r))return false;
        int age=victim.turn-r.victimTurn;
        int ttl=Math.Max(0,Plugin.RecentDamageTTL.Value);
        if(age<0||age>ttl||!ReferenceEquals(r.zone,EClass._zone))return false;
        if(r.attacker==null||(!r.attacker.IsPCFaction&&!r.attacker.IsPCFactionOrMinion))return false;
        attacker=r.attacker;
        return true;
    }
}

[HarmonyPatch(typeof(Card),nameof(Card.DamageHP),new Type[]{typeof(long),typeof(int),typeof(int),typeof(AttackSource),typeof(Card),typeof(bool),typeof(Thing),typeof(Chara),typeof(int)})]
static class LastDamageAttributionPatch
{
    static void Prefix(Card __instance,long dmg,Card origin)
    {
        try{LastDamageAttribution.Observe(__instance,dmg,origin);}
        catch(Exception ex){Plugin.I.Logger.LogWarning("[Luck] 최근 피해 가해자 기록 예외: "+ex.GetType().Name+" "+ex.Message);}
    }
}

static class DeathLuckResolver
{
    static int ClampLuck(Chara? c)=>Math.Max(1,Math.Min(9999,c==null?1:c.Evalue(78)));

    internal static int GetLuck(Chara attacker)
    {
        var mode=Plugin.DeathLuckSource.Value;
        if(mode==DeathLuckMode.마지막가해자)return ClampLuck(attacker);
        Chara pc=EClass.pc;
        if(pc==null)return ClampLuck(attacker);
        int best=ClampLuck(pc),sum=0;
        var seen=new HashSet<Chara>();
        seen.Add(pc); sum+=ClampLuck(pc);
        try
        {
            if(pc.party!=null&&pc.party.members!=null)
            {
                foreach(Chara m in pc.party.members)
                {
                    if(m==null||!seen.Add(m))continue;
                    int l=ClampLuck(m);
                    if(l>best)best=l;
                    sum=Math.Min(1000000,sum+l);
                }
            }
        }
        catch(Exception ex){Plugin.I.Logger.LogWarning("[Luck] 파티 Luck 집계 예외: "+ex.GetType().Name+" "+ex.Message);return ClampLuck(attacker);}
        if(mode==DeathLuckMode.파티최고)return best;
        return Math.Max(1,Math.Min(Math.Max(1,Plugin.PartyLuckSumCap.Value),sum));
    }

    internal static bool TryResolve(Chara victim,out Chara? killer,out bool direct,out bool crit)
    {
        killer=null;direct=false;crit=false;
        var ap=AttackProcess.Current;
        if(ap!=null&&ap.TC==victim&&ap.CC!=null)
        {
            Chara? a=ap.CC.Chara;
            if(a!=null&&(a.IsPCFaction||a.IsPCFactionOrMinion))
            {
                killer=a;direct=true;crit=ap.crit;return true;
            }
        }
        if(LastDamageAttribution.TryGet(victim,out Chara? recent)&&recent!=null)
        {
            killer=recent;direct=false;crit=false;return true;
        }
        return false;
    }

    internal static void ProcessGeneralBonus(Chara victim)
    {
        if(!TryResolve(victim,out Chara? killer,out bool direct,out bool crit)||killer==null)return;
        int old=Plugin.LuckOverride;
        try{Plugin.LuckOverride=GetLuck(killer);BonusDropCore.Process(victim);}
        finally{Plugin.LuckOverride=old;}
    }
}

'''
s=s.replace(insert_anchor,block+insert_anchor,1)

# Runtime diagnostics.
marker='        Logger.LogInfo("[Luck] 전투 장비 드롭 귀속: AttackProcess.Current가 피해자/공격자와 정확히 일치할 때만 적용; TryNeckHunt/Finish 추정 사용 안 함");\n'
diag='        Logger.LogInfo("[Luck] 사망 Luck 기준: "+DeathLuckSource.Value+", 최근 가해자 TTL="+RecentDamageTTL.Value+"턴; 환경/도트 fallback은 Luck만 승계");\n'
if marker not in s: raise SystemExit('v3.17 attribution diagnostic marker not found')
s=s.replace(marker,marker+diag,1)

p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj';cs.write_text(cs.read_text().replace('<Version>3.17.0</Version>','<Version>3.18.0</Version>'))
pkg=root/'package.xml';pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.17','Elona Luck for Elin v3.18'))
rd=root/'README_KR.md';t=rd.read_text().replace('# Elona Luck for Elin v3.17','# Elona Luck for Elin v3.18');t+='''\n\n## v3.18 파티 Luck / 최근 피해 가해자 귀속\n- 사망 후 보너스 드롭의 Luck 기준을 설정에서 마지막가해자 / 파티최고 / 파티합산 중 선택할 수 있습니다. 기본값은 파티최고입니다.\n- 파티합산은 플레이어와 현재 party.members의 Luck을 중복 없이 더하며 기본 최종 상한은 9999입니다. 합산은 강력하므로 별도 상한을 제공합니다.\n- Card.DamageHP의 9인자 본체 오버로드에는 피해 계산을 바꾸지 않는 Prefix만 추가해 최근 PC 파티 측 가해자 메타데이터를 기록합니다. 단순 DamageHP 오버로드는 원본에서 이 본체를 호출합니다.\n- 최신 피해를 다른 비파티 캐릭터가 주면 이전 파티 귀속을 제거합니다. 가해자가 없는 환경/Condition 피해는 최근 기록을 유지합니다.\n- 최근 기록은 같은 Zone + 피해자 turn 기준 TTL(기본 20턴)을 통과할 때만 사용합니다.\n- 환경사/도트/비전투 사망에서 recent fallback으로 귀속된 경우 Luck만 사용합니다. 크리티컬/처형자/오버킬은 직접 AttackProcess가 사망 대상과 정확히 일치할 때만 적용합니다.\n- 유전자/일반 소재/고유 드롭 보너스도 같은 사망 Luck 기준을 사용하도록 짧은 ThreadStatic Luck scope 안에서 실행합니다. 다른 낚시/상점/제작 Luck에는 영향을 주지 않습니다.\n- DamageHP 패치 자체가 런타임 시그니처 차이로 실패하면 PatchClass 단위로 비활성화되고, 직접 AttackProcess 귀속은 계속 동작합니다.\n- Card.Die/SpawnLoot/Thing.OnCreate/ThingGen 전역 패치/PatchAll/전역 TrySmoothPick/StackTrace는 계속 사용하지 않습니다.\n''';rd.write_text(t)

# Compile stubs for v3.18-only read-only attribution structures.
st=root/'refs/Elin/Stub.cs';h=st.read_text()
h=h.replace('public int c_lockLv;public int hp;public int MaxHP=100;public int LV=1;public int uid=1;', 'public int c_lockLv;public int hp;public int MaxHP=100;public int LV=1;public int uid=1;public int turn;')
h=h.replace('public bool IsEquipment=>false;', 'public bool isChara=>this is Chara;public bool IsEquipment=>false;')
h=h.replace('public virtual int Evalue(int id)=>0;public void SetInt', 'public virtual int Evalue(int id)=>0;public void DamageHP(long dmg,int ele,int eleP=100,AttackSource attackSource=AttackSource.None,Card origin=null,bool showEffect=true,Thing weapon=null,Chara originalTarget=null,int resistPenetrationLevel=0){}public void SetInt')
h=h.replace('public class Chara:Card{public int DEX;', 'public class Party{public List<Chara> members=new List<Chara>();}\npublic class Chara:Card{public Party party=new Party();public int DEX;')
if 'public enum AttackSource' not in h:
    h=h.replace('public enum ShopType{None,Blackmarket,Exotic}\n', 'public enum ShopType{None,Blackmarket,Exotic}\npublic enum AttackSource{None,Condition,Trap,Finish}\n')
st.write_text(h)
