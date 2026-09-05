from pathlib import Path
import runpy

# Start from v3.17: this deliberately excludes the v3.18 Card.DamageHP tracker.
runpy.run_path('_tmp/build_v317.py', run_name='__main__')
root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.17", V="3.17.0"','N="Elona Luck for Elin v3.19", V="3.19.0"')

# Party-Luck mode only. No recent-damage metadata tracking and no DamageHP patch.
anchor='namespace ElonaLuckForElinV3\n{\n'
enum='''namespace ElonaLuckForElinV3\n{\npublic enum DeathLuckMode { 마지막가해자, 파티최고, 파티합산 }\n'''
if anchor not in s: raise SystemExit('namespace anchor not found')
s=s.replace(anchor,enum,1)

old='    internal static int Luck(){int l=EClass.pc==null?1:EClass.pc.Evalue(78);return Math.Max(1,Math.Min(9999,l));}\n'
new='''    [ThreadStatic] internal static int LuckOverride;\n    internal static int Luck(){if(LuckOverride>0)return Math.Max(1,Math.Min(9999,LuckOverride));int l=EClass.pc==null?1:EClass.pc.Evalue(78);return Math.Max(1,Math.Min(9999,l));}\n'''
if old not in s: raise SystemExit('Luck helper pattern not found')
s=s.replace(old,new,1)

field_anchor='    internal static ConfigEntry<int> SkillW=null!,LuckW=null!,ActivityCap=null!;\n'
fields='''    internal static ConfigEntry<int> SkillW=null!,LuckW=null!,ActivityCap=null!;\n    internal static ConfigEntry<DeathLuckMode> DeathLuckSource=null!;\n    internal static ConfigEntry<int> PartyLuckSumCap=null!;\n'''
if field_anchor not in s: raise SystemExit('config field anchor not found')
s=s.replace(field_anchor,fields,1)

bind_anchor='        PostDeathBonus=Config.Bind("사망 후 보너스 드롭","안전 보너스 드롭 사용",true,"SpawnLoot를 건드리지 않고 ZoneEventManager.OnCharaDie 종료 후 보너스를 추가합니다.");\n'
binds='''        DeathLuckSource=Config.Bind("사망 후 보너스 드롭","사망 보너스 Luck 기준",DeathLuckMode.파티최고,"마지막가해자=직접 처치한 캐릭터의 운, 파티최고=현재 플레이어 파티 중 가장 높은 운, 파티합산=플레이어+현재 파티원의 운 합계(별도 상한 적용)입니다. 환경/DOT 최근 가해자 추적은 성능과 호환성을 위해 사용하지 않습니다.");\n        PartyLuckSumCap=Config.Bind("사망 후 보너스 드롭","파티 합산 Luck 상한",9999,"파티합산 모드의 최종 Luck 상한입니다. 합산은 강력하므로 밸런스가 과하면 낮추세요.");\n        PostDeathBonus=Config.Bind("사망 후 보너스 드롭","안전 보너스 드롭 사용",true,"SpawnLoot를 건드리지 않고 ZoneEventManager.OnCharaDie 종료 후 보너스를 추가합니다.");\n'''
if bind_anchor not in s: raise SystemExit('post-death bind anchor not found')
s=s.replace(bind_anchor,binds,1)

# Route general post-death bonus through direct attribution + chosen party Luck source.
if 'BonusDropCore.Process(c);' not in s: raise SystemExit('BonusDropCore.Process call not found')
s=s.replace('BonusDropCore.Process(c);','DeathLuckResolver.ProcessGeneralBonus(c);',1)

# Replace v3.17 direct killer resolver with common resolver and selected Luck source.
old='''    static bool ResolveKiller(Chara victim,out Chara? killer,out bool crit)\n    {\n        killer=null;crit=false;\n        var ap=AttackProcess.Current;\n        if(ap==null||ap.TC!=victim||ap.CC==null)return false;\n        killer=ap.CC.Chara;\n        if(killer==null||(!ap.CC.IsPCFaction&&!ap.CC.IsPCFactionOrMinion))return false;\n        crit=ap.crit;\n        return true;\n    }'''
new='''    static bool ResolveKiller(Chara victim,out Chara? killer,out bool crit)\n        =>DeathLuckResolver.TryResolveDirect(victim,out killer,out crit);'''
if old not in s: raise SystemExit('v3.17 ResolveKiller pattern not found')
s=s.replace(old,new,1)

old='        int killerLuck=Math.Max(1,Math.Min(9999,killer.Evalue(78)));\n'
new='        int killerLuck=DeathLuckResolver.GetLuck(killer);\n'
if old not in s: raise SystemExit('killerLuck pattern not found')
s=s.replace(old,new,1)

insert_anchor='static class CombatEquipmentBonusCore\n'
if insert_anchor not in s: raise SystemExit('CombatEquipmentBonusCore anchor not found')
block=r'''
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
        seen.Add(pc);sum+=ClampLuck(pc);
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
        catch(Exception ex)
        {
            Plugin.I.Logger.LogWarning("[Luck] 파티 Luck 집계 예외: "+ex.GetType().Name+" "+ex.Message);
            return ClampLuck(attacker);
        }
        if(mode==DeathLuckMode.파티최고)return best;
        return Math.Max(1,Math.Min(Math.Max(1,Plugin.PartyLuckSumCap.Value),sum));
    }

    internal static bool TryResolveDirect(Chara victim,out Chara? killer,out bool crit)
    {
        killer=null;crit=false;
        var ap=AttackProcess.Current;
        if(ap==null||ap.TC!=victim||ap.CC==null)return false;
        Chara? a=ap.CC.Chara;
        if(a==null||(!a.IsPCFaction&&!a.IsPCFactionOrMinion))return false;
        killer=a;crit=ap.crit;return true;
    }

    internal static void ProcessGeneralBonus(Chara victim)
    {
        if(!TryResolveDirect(victim,out Chara? killer,out bool crit)||killer==null)return;
        int old=Plugin.LuckOverride;
        try{Plugin.LuckOverride=GetLuck(killer);BonusDropCore.Process(victim);}
        finally{Plugin.LuckOverride=old;}
    }
}

'''
s=s.replace(insert_anchor,block+insert_anchor,1)

marker='        Logger.LogInfo("[Luck] 전투 장비 드롭 귀속: AttackProcess.Current가 피해자/공격자와 정확히 일치할 때만 적용; TryNeckHunt/Finish 추정 사용 안 함");\n'
diag='''        Logger.LogInfo("[Luck] 사망 Luck 기준: "+DeathLuckSource.Value+"; 파티최고/파티합산 지원, DamageHP 최근가해자 추적 없음");\n        Logger.LogInfo("[Luck] 성능 안전 모드: Card.DamageHP Harmony 패치 없음; 환경/DOT 귀속은 직접 AttackProcess가 남아 있을 때만 적용");\n'''
if marker not in s: raise SystemExit('v3.17 diagnostic marker not found')
s=s.replace(marker,marker+diag,1)

p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj';cs.write_text(cs.read_text().replace('<Version>3.17.0</Version>','<Version>3.19.0</Version>'))
pkg=root/'package.xml';pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.17','Elona Luck for Elin v3.19'))
rd=root/'README_KR.md';t=rd.read_text().replace('# Elona Luck for Elin v3.17','# Elona Luck for Elin v3.19');t+='''\n\n## v3.19 Performance Safe\n- v3.18에서 추가했던 Card.DamageHP 최근 가해자 추적 Prefix를 완전히 제거했습니다. DamageHP는 전투/상태이상/환경 피해가 매우 자주 지나는 핵심 경로이므로 성능과 타 모드 호환성을 우선했습니다.\n- ConditionalWeakTable 기반 최근 가해자 기록, 환경/DOT TTL 설정, DamageHP PatchClass 등록도 모두 없습니다.\n- 사망 보너스 Luck 기준은 마지막가해자 / 파티최고 / 파티합산 설정을 그대로 유지합니다. 기본값은 파티최고입니다.\n- 파티합산은 중복 없는 현재 party.members + 플레이어 Luck을 합산하고 별도 상한을 적용합니다.\n- 직접 처치 귀속은 AttackProcess.Current가 정확히 사망한 victim(TC)과 PC 진영 attacker(CC)를 가리킬 때만 인정합니다.\n- 환경사/DOT/비전투 사망은 최근 가해자를 억지로 추적하지 않습니다. AttackProcess가 이미 끊긴 경우 보너스를 지급하지 않는 fail-closed 정책입니다.\n- Card.Die/SpawnLoot/Thing.OnCreate/ThingGen 전역 패치/PatchAll/전역 TrySmoothPick/StackTrace도 계속 사용하지 않습니다.\n''';rd.write_text(t)
