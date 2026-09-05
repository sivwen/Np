from pathlib import Path
import runpy

runpy.run_path('_tmp/build_v315.py', run_name='__main__')
root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.15", V="3.15.0"','N="Elona Luck for Elin v3.16", V="3.16.0"')

# Deterministic Makefish Postfix pipeline:
# 1) LuckyFishingReplacementPatch (Priority.First)
# 2) FishingOutcomeLuckPatch (default priority)
# 3) FishPatch tier promotion (Priority.Last)
start=s.find('static class FishingOutcomeLuckPatch')
if start<0: raise SystemExit('FishingOutcomeLuckPatch not found')
end=s.find('\n[HarmonyPatch',start+1)
if end<0: raise SystemExit('FishingOutcomeLuckPatch end not found')
seg=s[start:end]
needle='    [HarmonyPriority(Priority.First)]\n    static void Postfix(Chara c,ref Thing __result)'
if needle not in seg: raise SystemExit('FishingOutcome priority pattern not found')
seg=seg.replace(needle,'    static void Postfix(Chara c,ref Thing __result)',1)
s=s[:start]+seg+s[end:]

old='static class FishPatch{static void Postfix(Chara c,ref Thing __result)'
new='static class FishPatch{[HarmonyPriority(Priority.Last)] static void Postfix(Chara c,ref Thing __result)'
if old not in s: raise SystemExit('FishPatch pattern not found')
s=s.replace(old,new,1)

# Improve startup diagnostics so runtime logs explicitly show the ordered pipeline.
marker='        h=new Harmony(G);\n'
diag='        Logger.LogInfo("[Luck] 낚시 후처리 순서: Lucky Fishing 특수 보상 -> 실패/정크 구조 -> 최종 물고기 tier (고정)");\n'
if diag not in s:
    if marker not in s: raise SystemExit('Harmony startup marker not found')
    s=s.replace(marker,diag+marker,1)

# External LuckyFishing is a hard duplicate-risk warning. Do not attempt to unpatch another mod.
oldwarn='if(HasAssembly("LuckyFishing"))Logger.LogWarning("[Luck] 외부 LuckyFishing 감지: 이 모드가 동일 기능을 내장 대체하므로 외부 LuckyFishing을 비활성화/구독 해제하는 것을 권장합니다. 내장 대체 기능은 계속 사용합니다.");'
newwarn='if(HasAssembly("LuckyFishing"))Logger.LogWarning("[Luck] 외부 LuckyFishing 감지: 특수 보상 롤이 중복될 수 있고 구버전 transpiler 오류도 발생할 수 있습니다. 외부 LuckyFishing은 비활성화/구독 해제하세요. 이 모드의 내장 대체 기능은 계속 사용합니다.");'
if oldwarn not in s: raise SystemExit('LuckyFishing warning pattern not found')
s=s.replace(oldwarn,newwarn,1)

p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj';cs.write_text(cs.read_text().replace('<Version>3.15.0</Version>','<Version>3.16.0</Version>'))
pkg=root/'package.xml';pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.15','Elona Luck for Elin v3.16'))
rd=root/'README_KR.md';t=rd.read_text().replace('# Elona Luck for Elin v3.15','# Elona Luck for Elin v3.16');t+='''\n\n## v3.16 안정성 감사 / 낚시 후처리 순서 고정\n- Makefish 후처리 순서를 명시적으로 고정했습니다.\n  1. Lucky Fishing 내장 대체 특수 보상 추가 롤 (Priority.First)\n  2. 빈 낚싯줄 구조 / 정크 -> 정상 어획 변환 (기본 우선순위)\n  3. 최종 결과가 물고기일 때 tier Luck (Priority.Last)\n- 이전 버전에서는 Lucky Fishing 대체와 실패/정크 패치가 둘 다 Priority.First라 런타임 정렬에 따라 구조/변환된 물고기가 tier Luck을 놓치거나 플레이버 로그 순서가 흔들릴 수 있었습니다.\n- 외부 LuckyFishing이 감지되면 중복 특수 보상과 구버전 transpiler 오류 위험을 명확히 경고합니다. 다른 모드의 Harmony 패치를 강제로 해제하지는 않습니다.\n- OnCharaDie 일반 보너스와 전투 장비 보너스는 서로 다른 품목군을 처리하며 둘 다 Priority.Last를 유지합니다. SpawnLoot/Card.Die는 계속 패치하지 않습니다.\n- 황금 물고기/에헤카틀 패치는 IL 패턴이 정확히 맞을 때만 적용되는 fail-closed 정책을 유지합니다.\n''';rd.write_text(t)
