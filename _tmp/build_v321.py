from pathlib import Path
import runpy

runpy.run_path('_tmp/build_v320.py', run_name='__main__')
root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.20", V="3.20.0"','N="Elona Luck for Elin v3.21", V="3.21.0"')

old='''    void PatchClass(string name,Type t){try{h!.CreateClassProcessor(t).Patch();Logger.LogInfo("[Luck] "+name+": 적용");}catch(Exception ex){Logger.LogWarning("[Luck] "+name+": 비활성 ("+ex.GetType().Name+") "+ex.Message);}}'''
new='''    void PatchClass(string name,Type t)
    {
        Logger.LogInfo("[Luck][PATCH-BEGIN] "+name+" => "+t.FullName);
        try
        {
            h!.CreateClassProcessor(t).Patch();
            Logger.LogInfo("[Luck][PATCH-OK] "+name);
        }
        catch(Exception ex)
        {
            Logger.LogWarning("[Luck][PATCH-FAIL] "+name+" => "+ex.ToString());
        }
    }'''
if old not in s: raise SystemExit('PatchClass body pattern not found')
s=s.replace(old,new,1)
marker='        Logger.LogInfo("[Luck] BepInEx 참조 ABI: 6.0.0.0 (Elin BepInEx 6 계열에 맞춤)");\n'
extra='        Logger.LogInfo("[Luck] v3.21 진단 모드: 각 Harmony 클래스 등록의 BEGIN/OK/FAIL을 기록합니다. 기능 동작은 v3.20과 동일합니다.");\n'
if marker not in s: raise SystemExit('v3.20 ABI log marker not found')
s=s.replace(marker,marker+extra,1)
p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj';cs.write_text(cs.read_text().replace('<Version>3.20.0</Version>','<Version>3.21.0</Version>'))
pkg=root/'package.xml';pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.20','Elona Luck for Elin v3.21'))
rd=root/'README_KR.md';t=rd.read_text().replace('# Elona Luck for Elin v3.20','# Elona Luck for Elin v3.21');t+='''\n\n## v3.21 Patch Isolation Diagnostic\n- 기능 세트는 v3.20과 동일합니다.\n- 각 Harmony PatchClass 등록 직전에 [PATCH-BEGIN], 성공 시 [PATCH-OK], 예외 시 [PATCH-FAIL] + 전체 예외 문자열을 기록합니다.\n- Mod Doctor가 Plugin.Awake()를 반복적으로 지목하는 원인이 특정 Harmony 클래스 등록에서 발생하는 내부/first-chance 예외인지 한 번의 startup log로 좁히기 위한 진단 배치입니다.\n- BepInEx.Core/BepInEx.Unity compile-only stub ABI는 6.0.0.0을 유지합니다.\n- Card.DamageHP/Card.Die/Card.SpawnLoot/Thing.OnCreate/전역 ThingGen/PatchAll/StackTrace 금지 정책도 유지합니다.\n''';rd.write_text(t)
