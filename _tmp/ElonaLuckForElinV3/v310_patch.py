from pathlib import Path

root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.9", V="3.9.0"','N="Elona Luck for Elin v3.10", V="3.10.0"')

anchor='PatchClass("블랙마켓 희귀도 승급",typeof(BlackmarketRarityPatch));'
if 'MonsterEquipLuckPatch' not in s:
    s=s.replace(anchor,anchor+'\n        PatchClass("몬스터 장비 난이도 운",typeof(MonsterEquipLuckPatch));')

block=r'''

[HarmonyPatch]
static class MonsterEquipLuckPatch
{
    static MethodBase? target;
    static ConfigEntry<bool>? Enabled,EnemyOnly,FlavorLog;
    static ConfigEntry<int>? LuckDiv,LuckCap,DoubleUpgradeThreshold;

    static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("장비 운","몬스터 장비가 플레이어 운에 반응",false,"고위험/고보상 옵션입니다. 적이 장비를 생성할 때 플레이어 Luck으로 장비 희귀도를 올릴 수 있어 전투 난이도와 잠재 전리품 가치가 함께 증가합니다. 기본값은 꺼짐입니다.");
        EnemyOnly=Plugin.I.Config.Bind("장비 운","적대 몬스터만 적용",true,"플레이어 진영/중립 NPC를 제외하고 적대 개체의 장비 생성에만 적용합니다.");
        LuckDiv=Plugin.I.Config.Bind("장비 운","몬스터 장비 운 분모",40,"희귀도 1단계 승급 확률은 Luck/이 값(%)입니다.");
        LuckCap=Plugin.I.Config.Bind("장비 운","몬스터 장비 승급 확률 상한",35,"희귀도 1단계 승급 확률 상한(%)입니다.");
        DoubleUpgradeThreshold=Plugin.I.Config.Bind("장비 운","2단계 승급 시작 Luck",2000,"이 Luck 이상부터 첫 승급에 성공했을 때 두 번째 승급을 추가로 판정합니다. 0이면 2단계 승급을 사용하지 않습니다.");
        FlavorLog=Plugin.I.Config.Bind("장비 운","몬스터 장비 플레이버 로그",false,"몬스터 장비 희귀도가 실제로 상승했을 때 게임 플레이 로그에 메시지를 표시합니다. 몬스터 생성이 잦으면 로그가 많아질 수 있어 기본값은 꺼짐입니다.");
        target=AccessTools.Method(typeof(Chara),"SetEQQuality");
        if(target==null)
        {
            Plugin.I.Logger.LogWarning("[Luck] 몬스터 장비 운: Chara.SetEQQuality를 찾지 못해 비활성화합니다.");
            return false;
        }
        return true;
    }

    static MethodBase TargetMethod()=>target!;

    static void Postfix(Chara __instance)
    {
        try
        {
            if(Enabled==null||!Enabled.Value||__instance==null||EClass.pc==null)return;
            if(__instance.IsPCFaction)return;
            if(EnemyOnly!=null&&EnemyOnly.Value)
            {
                try{if(__instance.OriginalHostility!=Hostility.Enemy)return;}catch{return;}
            }
            var bp=CardBlueprint.current;
            if(bp==null||bp!=CardBlueprint.CharaGenEQ)return;
            var old=bp.rarity;
            if(old==Rarity.Artifact||old==Rarity.Mythical)return;
            if(old==Rarity.Random)return;
            int chance=Math.Min(Math.Max(0,LuckCap?.Value??35),Math.Max(0,Plugin.Luck()/Math.Max(1,LuckDiv?.Value??40)));
            if(chance<=0||EClass.rnd(100)>=chance)return;
            var now=Upgrade(old);
            int threshold=DoubleUpgradeThreshold?.Value??2000;
            if(threshold>0&&Plugin.Luck()>=threshold&&now!=Rarity.Mythical&&now!=Rarity.Artifact)
            {
                int extra=Math.Min(chance,Math.Max(1,(Plugin.Luck()-threshold)/Math.Max(1,(LuckDiv?.Value??40)*2)));
                if(EClass.rnd(100)<extra)now=Upgrade(now);
            }
            if(now==old)return;
            bp.rarity=now;
            if(FlavorLog!=null&&FlavorLog.Value)Msg.SayRaw("기묘한 행운이 적의 장비마저 날카롭게 벼렸다.");
        }
        catch(Exception ex)
        {
            Plugin.I.Logger.LogWarning("[Luck] 몬스터 장비 운 런타임 예외: "+ex.GetType().Name+" "+ex.Message);
        }
    }

    static Rarity Upgrade(Rarity r)
    {
        if(r==Rarity.Crude||r==Rarity.Normal)return Rarity.Superior;
        if(r==Rarity.Superior)return Rarity.Legendary;
        if(r==Rarity.Legendary)return Rarity.Mythical;
        return r;
    }
}
'''

if 'static class MonsterEquipLuckPatch' not in s:
    pos=s.rfind('\n}')
    s=s[:pos]+block+s[pos:]

p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj'
cs.write_text(cs.read_text().replace('<Version>3.9.0</Version>','<Version>3.10.0</Version>'))

pkg=root/'package.xml'
t=pkg.read_text().replace('Elona Luck for Elin v3.9','Elona Luck for Elin v3.10')
pkg.write_text(t)

rd=root/'README_KR.md'
t=rd.read_text().replace('# Elona Luck for Elin v3.9','# Elona Luck for Elin v3.10')
if '## v3.10 몬스터 장비 난이도 운' not in t:
    t += '''\n\n## v3.10 몬스터 장비 난이도 운\n- 기본값 OFF인 고위험/고보상 옵션입니다.\n- `Chara.SetEQQuality()` 완료 직후, 그 메서드가 이미 준비한 `CardBlueprint.CharaGenEQ`의 rarity만 좁게 보정합니다.\n- `Thing.OnCreate`, `ThingGen.Create`, `Card.SpawnLoot`, `Card.Die`는 패치하지 않습니다.\n- 기본은 적대 개체만 적용하며 플레이어 진영/중립 NPC에는 적용하지 않습니다.\n- 기본 승급 확률은 `Luck / 40 (%)`, 상한 35%입니다.\n- Crude/Normal -> Superior -> Legendary -> Mythical 순으로 최대 1단계 승급합니다.\n- 기본 Luck 2000 이상에서는 첫 승급이 성공한 경우 매우 낮은 추가 확률로 2단계 승급을 한 번 더 판정합니다. 설정에서 시작 Luck을 0으로 두면 비활성화할 수 있습니다.\n- 장비 희귀도가 올라간 상태로 원본 장비 생성이 이어지므로 실제 적 전투력이 올라갈 수 있고, 사망 후 원본/보너스 드롭으로 얻을 수 있는 장비 가치도 함께 높아질 수 있습니다.\n- 몬스터 생성이 많은 구역에서 로그가 과해질 수 있어 플레이버 로그는 기본 OFF입니다.\n'''
rd.write_text(t)
