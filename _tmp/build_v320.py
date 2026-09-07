from pathlib import Path
import runpy

# Keep v3.19 Performance Safe feature set; v3.20 fixes only build/reference metadata.
runpy.run_path('_tmp/build_v319.py', run_name='__main__')
root=Path('_tmp/ElonaLuckForElinV3')

p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.19", V="3.19.0"','N="Elona Luck for Elin v3.20", V="3.20.0"')
marker='        Logger.LogInfo("[Luck] 성능 안전 모드: Card.DamageHP Harmony 패치 없음; 환경/DOT 귀속은 직접 AttackProcess가 남아 있을 때만 적용");\n'
extra='        Logger.LogInfo("[Luck] BepInEx 참조 ABI: 6.0.0.0 (Elin BepInEx 6 계열에 맞춤)");\n'
if marker not in s: raise SystemExit('v3.19 diagnostic marker not found')
s=s.replace(marker,marker+extra,1)
p.write_text(s)

# Match Elin's BepInEx 6 runtime assembly ABI instead of SDK default 1.0.0.0.
# These projects are compile-only stubs; only ElonaLuckForElin.dll is packaged.
for rel,name in [
    ('refs/BepInExCore/BepInExCore.csproj','BepInEx.Core'),
    ('refs/BepInExUnity/BepInExUnity.csproj','BepInEx.Unity'),
]:
    cp=root/rel
    txt=cp.read_text()
    if '<AssemblyVersion>' not in txt:
        txt=txt.replace('</PropertyGroup>', '<AssemblyVersion>6.0.0.0</AssemblyVersion><FileVersion>6.0.0.0</FileVersion><Version>6.0.0</Version></PropertyGroup>',1)
    else:
        raise SystemExit(rel+' already has AssemblyVersion; audit manually')
    cp.write_text(txt)

cs=root/'ElonaLuckForElinV3.csproj'
t=cs.read_text().replace('<Version>3.19.0</Version>','<Version>3.20.0</Version>')
cs.write_text(t)

pkg=root/'package.xml'
t=pkg.read_text().replace('Elona Luck for Elin v3.19','Elona Luck for Elin v3.20')
pkg.write_text(t)

rd=root/'README_KR.md'
t=rd.read_text().replace('# Elona Luck for Elin v3.19','# Elona Luck for Elin v3.20')
t+='''\n\n## v3.20 BepInEx Reference Fix\n- 기능 세트는 v3.19 Performance Safe와 동일합니다.\n- CI용 BepInEx.Core / BepInEx.Unity stub의 AssemblyVersion이 SDK 기본 1.0.0.0으로 남아 최종 플러그인에 잘못된 BepInEx 대상 버전 경고가 발생하던 문제를 수정했습니다.\n- Elin의 BepInEx 6 런타임 ABI에 맞춰 두 compile-only stub의 AssemblyVersion/FileVersion을 6.0.0.0으로 고정합니다.\n- stub DLL은 배포 ZIP에 포함하지 않습니다. 실제 게임의 BepInEx DLL을 사용합니다.\n- Card.DamageHP 최근 가해자 패치는 v3.19와 동일하게 없습니다.\n- Card.Die/SpawnLoot/Thing.OnCreate/ThingGen 전역 패치/PatchAll/전역 TrySmoothPick/StackTrace 금지 정책도 유지합니다.\n'''
rd.write_text(t)
