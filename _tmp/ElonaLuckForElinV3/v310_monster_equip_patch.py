from pathlib import Path

root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.9", V="3.9.0"','N="Elona Luck for Elin v3.10", V="3.10.0"')
needle='PatchClass("전투 장비 보너스 드롭",typeof(CombatEquipmentBonusPatch));'
if 'MonsterEquipmentLuckPatch' not in s:
    s=s.replace(needle, needle+'\n        PatchClass("몬스터 장비 Luck 난이도",typeof(MonsterEquipmentLuckPatch));')
if 'static class MonsterEquipmentLuckPatch' not in s:
    block=r'''

[HarmonyPatch]
static class MonsterEquipmentLuckPatch
{
    static MethodBase? target;
    static ConfigEntry<bool>? Enabled,FlavorLog;
    static ConfigEntry<int>? LuckDiv,LuckCap,MaxSteps;
    static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("장비 운","몬스터 장비 Luck 난이도",false,"플레이어 Luck이 높을수록 새로 생성되는 비플레이어 진영 몬스터의 랜덤 장비 희귀도가 상승할 수 있습니다. 적이 실제로 강해질 수 있는 고위험/고보상 옵션입니다.");
        LuckDiv=Plugin.I.Config.Bind("장비 운","몬스터 장비 희귀도 운 분모",20,"몬스터 장비 한 단계 승급의 기본 확률은 Luck/이 값(%)입니다.");
        LuckCap=Plugin.I.Config.Bind("장비 운","몬스터 장비 희귀도 승급 상한",50,"각 장비의 한 단계 승급 확률 상한(%)입니다.");
        MaxSteps=Plugin.I.Config.Bind("장비 운","몬스터 장비 최대 승급 단계",1,"한 장비 생성에서 Luck으로 추가될 수 있는 최대 희귀도 단계입니다. 안전상 0~2로 제한됩니다.");
        FlavorLog=Plugin.I.Config.Bind("장비 운","몬스터 장비 Luck 로그",false,"몬스터 장비 희귀도가 Luck으로 상승했을 때 게임 로그에 표시합니다. 몬스터 생성이 잦으면 로그가 많아질 수 있어 기본값은 꺼져 있습니다.");
        target=typeof(Chara).GetMethod("SetEQQuality",BindingFlags.Instance|BindingFlags.NonPublic);
        if(target==null){Plugin.I.Logger.LogWarning("[Luck] 몬스터 장비 Luck: Chara.SetEQQuality를 찾지 못해 비활성화합니다.");return false;}
        return true;
    }
    static MethodBase TargetMethod()=>target!;
    static void Postfix(Chara __instance)
    {
        if(Enabled==null||!Enabled.Value||__instance==null||EClass.pc==null||__instance.IsPCFaction)return;
        var bp=CardBlueprint.current;
        if(bp==null||!object.ReferenceEquals(bp,CardBlueprint.CharaGenEQ))return;
        Rarity old=bp.rarity;
        if(old>=Rarity.Mythical||old>=Rarity.Artifact)return;
        int chance=Math.Min(Math.Max(0,LuckCap?.Value??50),Math.Max(0,Plugin.Luck()/Math.Max(1,LuckDiv?.Value??20)));
        int steps=Math.Min(2,Math.Max(0,MaxSteps?.Value??1));
        if(chance<=0||steps<=0)return;
        Rarity q=old;int raised=0;
        for(int i=0;i<steps;i++)
        {
            if(EClass.rnd(100)>=chance)break;
            if(q<=Rarity.Normal)q=Rarity.Superior;
            else if(q==Rarity.Superior)q=Rarity.Legendary;
            else if(q==Rarity.Legendary)q=Rarity.Mythical;
            else break;
            raised++;
            if(q>=Rarity.Mythical)break;
        }
        if(raised<=0||q==old)return;
        bp.rarity=q;
        if(FlavorLog!=null&&FlavorLog.Value)Msg.SayRaw("기묘한 행운이 적의 장비를 더 위협적으로 만들었다.");
    }
}
'''
    pos=s.rfind('\n}')
    s=s[:pos]+block+s[pos:]
p.write_text(s)
cs=root/'ElonaLuckForElinV3.csproj'
cs.write_text(cs.read_text().replace('<Version>3.9.0</Version>','<Version>3.10.0</Version>'))
pkg=root/'package.xml'
pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.9','Elona Luck for Elin v3.10'))
rd=root/'README_KR.md'
t=rd.read_text().replace('# Elona Luck for Elin v3.9','# Elona Luck for Elin v3.10')
if '## v3.10 몬스터 장비 Luck 난이도' not in t:
    t += '''\n\n## v3.10 몬스터 장비 Luck 난이도\n- 기본값은 OFF입니다.\n- `Chara.SetEQQuality()`가 원본 장비 품질을 결정한 직후, 실제 장비 생성 전에만 `CardBlueprint.current`를 검사합니다.\n- `CardBlueprint.current`가 정확히 `CardBlueprint.CharaGenEQ`일 때만 적용하여 다른 아이템 생성 컨텍스트를 건드리지 않습니다.\n- 플레이어 진영은 제외하고 비플레이어 진영 몬스터의 랜덤 생성 장비만 대상입니다.\n- 기본 확률은 `Luck / 20 (%)`, 단계당 상한 50%, 기본 최대 1단계 승급입니다.\n- Normal/Crude -> Superior -> Legendary -> Mythical 순으로만 올리며 Artifact는 만들지 않습니다.\n- 고정 희귀도를 명시하는 장비는 호출자에서 SetEQQuality 이후 rarity를 다시 지정할 수 있으므로 원본 의도를 우선합니다.\n- 켜면 적이 실제로 더 좋은 장비를 착용해 난이도가 올라갈 수 있고, v3.9의 전투 장비 보너스 드롭과 결합해 고위험/고보상으로 작동합니다.\n- `Thing.OnCreate`, `ThingGen.Create`, `Card.SpawnLoot`, `Card.Die`는 패치하지 않습니다.\n'''
rd.write_text(t)
