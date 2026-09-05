from pathlib import Path
import runpy

runpy.run_path('_tmp/build_v316.py', run_name='__main__')
root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.16", V="3.16.0"','N="Elona Luck for Elin v3.17", V="3.17.0"')

# v3.17: remove the TryNeckHunt-based pseudo-"Finish" attribution entirely.
# It is not a reliable representation of the final damaging hit and can over-credit
# non-standard executions. Keep combat equipment bonus strictly tied to the live
# AttackProcess context that actually points at this victim and attacker.
reg='        PatchClass("Finish 처치 컨텍스트",typeof(FinishKillContextPatch));\n'
if reg not in s: raise SystemExit('FinishKillContext registration not found')
s=s.replace(reg,'',1)

start=s.find('static class FinishKillContext\n')
end=s.find('static class CombatEquipmentBonusCore\n',start)
if start<0 or end<0: raise SystemExit('FinishKillContext block not found')
s=s[:start]+s[end:]

old='''    static ConfigEntry<bool>? Enabled,FlavorLog;\n    static ConfigEntry<int>? LuckDiv,LuckCap,CritBonus,FinishBonus,ExecutionerPerLv,ExecutionerCap,OverkillCap,TotalCap;'''
new='''    static ConfigEntry<bool>? Enabled,FlavorLog;\n    static ConfigEntry<int>? LuckDiv,LuckCap,CritBonus,ExecutionerPerLv,ExecutionerCap,OverkillCap,TotalCap;'''
if old not in s: raise SystemExit('combat config declaration pattern not found')
s=s.replace(old,new,1)

old='        FinishBonus=Plugin.I.Config.Bind("사망 후 보너스 드롭","Finish 처치 보너스",100,"TryNeckHunt의 Finish 처치이면 더하는 점수입니다. 100점은 약 추가 1%p입니다.");\n'
if old not in s: raise SystemExit('Finish bonus config bind not found')
s=s.replace(old,'',1)

old='''    static bool ResolveKiller(Chara victim,out Chara? killer,out bool crit,out bool finish)\n    {\n        killer=null;crit=false;finish=false;\n        if(FinishKillContext.Matches(victim))\n        {\n            killer=FinishKillContext.killer;\n            finish=true;\n            var apf=AttackProcess.Current;\n            if(apf!=null&&apf.TC==victim&&apf.CC==killer)crit=apf.crit;\n            return killer!=null&&(killer.IsPCFaction||killer.IsPCFactionOrMinion);\n        }\n        var ap=AttackProcess.Current;\n        if(ap==null||ap.TC!=victim||ap.CC==null)return false;\n        killer=ap.CC.Chara;\n        if(killer==null||(!ap.CC.IsPCFaction&&!ap.CC.IsPCFactionOrMinion))return false;\n        crit=ap.crit;\n        return true;\n    }'''
new='''    static bool ResolveKiller(Chara victim,out Chara? killer,out bool crit)\n    {\n        killer=null;crit=false;\n        var ap=AttackProcess.Current;\n        if(ap==null||ap.TC!=victim||ap.CC==null)return false;\n        killer=ap.CC.Chara;\n        if(killer==null||(!ap.CC.IsPCFaction&&!ap.CC.IsPCFactionOrMinion))return false;\n        crit=ap.crit;\n        return true;\n    }'''
if old not in s: raise SystemExit('ResolveKiller pattern not found')
s=s.replace(old,new,1)

old='        if(!ResolveKiller(victim,out Chara? killer,out bool crit,out bool finish)||killer==null)return;\n'
new='        if(!ResolveKiller(victim,out Chara? killer,out bool crit)||killer==null)return;\n'
if old not in s: raise SystemExit('ResolveKiller call pattern not found')
s=s.replace(old,new,1)

# Use the actual killer's Luck for combat equipment drops. This prevents a PC's
# Luck from being silently borrowed by a low-Luck minion kill, while still allowing
# PC-faction/minion kills when their own Luck and Executioner values justify it.
old='        int score=Math.Min(Math.Max(0,LuckCap?.Value??100),Math.Max(0,Plugin.Luck()/Math.Max(1,LuckDiv?.Value??10)));\n'
new='        int killerLuck=Math.Max(1,Math.Min(9999,killer.Evalue(78)));\n        int score=Math.Min(Math.Max(0,LuckCap?.Value??100),Math.Max(0,killerLuck/Math.Max(1,LuckDiv?.Value??10)));\n'
if old not in s: raise SystemExit('combat Luck score pattern not found')
s=s.replace(old,new,1)

old='        if(finish)score+=Math.Max(0,FinishBonus?.Value??100);\n'
if old not in s: raise SystemExit('finish score line not found')
s=s.replace(old,'',1)

old='            string why=finish?"마무리의 행운이 전리품을 남겼다.":(crit?"결정적인 일격이 뜻밖의 전리품을 끌어냈다.":"행운이 적의 장비 하나를 놓치지 않았다.");\n'
new='            string why=crit?"결정적인 일격이 뜻밖의 전리품을 끌어냈다.":"행운이 적의 장비 하나를 놓치지 않았다.";\n'
if old not in s: raise SystemExit('combat flavor line not found')
s=s.replace(old,new,1)

# Update user-facing description so it no longer claims a generic Finish signal.
old='SpawnLoot가 끝난 뒤에도 사망한 적 인벤토리에 남아 있는 장비 중 최대 1개에 Luck+크리티컬+Finish+처형자+오버킬 기반 추가 드롭 판정을 합니다.'
new='SpawnLoot가 끝난 뒤에도 사망한 적 인벤토리에 남아 있는 장비 중 최대 1개에 실제 공격 컨텍스트의 공격자 Luck+크리티컬+처형자+오버킬 기반 추가 드롭 판정을 합니다.'
if old not in s: raise SystemExit('combat description pattern not found')
s=s.replace(old,new,1)

# Startup diagnostics: explicitly document the stricter attribution rule.
marker='        Logger.LogInfo("[Luck] 낚시 후처리 순서: Lucky Fishing 특수 보상 -> 실패/정크 구조 -> 최종 물고기 tier (고정)");\n'
diag='        Logger.LogInfo("[Luck] 전투 장비 드롭 귀속: AttackProcess.Current가 피해자/공격자와 정확히 일치할 때만 적용; TryNeckHunt/Finish 추정 사용 안 함");\n'
if diag not in s:
    if marker not in s: raise SystemExit('startup diagnostics marker not found')
    s=s.replace(marker,marker+diag,1)

p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj';cs.write_text(cs.read_text().replace('<Version>3.16.0</Version>','<Version>3.17.0</Version>'))
pkg=root/'package.xml';pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.16','Elona Luck for Elin v3.17'))
rd=root/'README_KR.md';t=rd.read_text().replace('# Elona Luck for Elin v3.16','# Elona Luck for Elin v3.17');t+='''\n\n## v3.17 전투 장비 드롭 귀속 감사\n- TryNeckHunt를 일반적인 Finish/마지막 일격으로 간주하던 보조 컨텍스트 패치를 제거했습니다.\n- 전투 장비 보너스는 이제 ZoneEventManager.OnCharaDie 시점에 AttackProcess.Current가 정확히 해당 피해자(TC)와 PC 진영 공격자(CC)를 가리킬 때만 적용됩니다.\n- 크리티컬은 같은 AttackProcess의 crit 값만 사용합니다. AttackSource.Finish 같은 별도 신호를 실제 크리티컬/처형 판정으로 오인하지 않습니다.\n- 장비 보너스의 Luck은 전역 PC Luck이 아니라 실제 공격자의 Evalue(78)를 사용합니다. 동료/미니언 처치가 플레이어 Luck을 빌려 쓰지 않습니다.\n- 처형자(1420)는 실제 공격자의 값을 계속 사용하고, 오버킬은 사망 시점 피해자의 음수 HP/MaxHP 비율을 사용합니다.\n- 환경사/도트/비전투 사망처럼 AttackProcess.Current가 일치하지 않으면 전투 장비 보너스를 지급하지 않습니다. 이는 잘못된 귀속보다 보너스를 누락하는 쪽을 택하는 fail-closed 정책입니다.\n- Card.Die/SpawnLoot/Thing.OnCreate/ThingGen 전역 패치/PatchAll/전역 TrySmoothPick/StackTrace는 계속 사용하지 않습니다.\n''';rd.write_text(t)
