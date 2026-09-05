from pathlib import Path
import runpy

runpy.run_path('_tmp/build_v314.py', run_name='__main__')
root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.14", V="3.14.0"','N="Elona Luck for Elin v3.15", V="3.15.0"')

anchor='        PatchClass("에헤카틀 물고기 도주 회피 운",typeof(EhekatlFishEscapeLuckPatch));\n'
add='        PatchClass("황금 물고기 운",typeof(GoldenFishLuckPatch));\n'
if add not in s:
    if anchor not in s: raise SystemExit('v3.14 fishing anchor not found')
    s=s.replace(anchor,anchor+add)

block=r'''

[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]
static class GoldenFishLuckPatch
{
    static ConfigEntry<bool>? Enabled;
    static ConfigEntry<int>? LuckDiv,LuckCap;
    static readonly MethodInfo Helper=typeof(GoldenFishLuckPatch).GetMethod(nameof(PlayerPartyGoldDenom),BindingFlags.Static|BindingFlags.NonPublic)!;

    static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("낚시 운","황금 물고기 운",true,"지역 어종 65가 황금 물고기 65_gold로 변하는 PC 파티의 원본 1/8192 판정만 Luck으로 완화합니다. NPC용 1/819200은 변경하지 않습니다.");
        LuckDiv=Plugin.I.Config.Bind("낚시 운","황금 물고기 운 분모",20,"운이 이 값만큼 오를 때 황금 물고기 상대 확률이 1% 증가합니다.");
        LuckCap=Plugin.I.Config.Bind("낚시 운","황금 물고기 상대 보너스 상한",300,"황금 물고기 상대 확률 증가 상한(%)입니다. 300이면 원본 대비 최대 약 4배입니다.");
        return true;
    }

    static bool IsLdc(CodeInstruction c,int v)
    {
        if(c.opcode==OpCodes.Ldc_I4)return c.operand is int x&&x==v;
        if(c.opcode==OpCodes.Ldc_I4_S)return Convert.ToInt32(c.operand)==v;
        return false;
    }

    static int PlayerPartyGoldDenom(Chara c)
    {
        const int original=8192;
        if(Enabled==null||!Enabled.Value||c==null||!c.IsPCParty)return original;
        int bonus=Plugin.RelativeBonus(LuckDiv!,LuckCap!);
        long denom=(long)original*100L/Math.Max(100,100+bonus);
        return (int)Math.Max(2,Math.Min(original,denom));
    }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var src=new List<CodeInstruction>(instructions);
        var p8192=new List<int>();
        var p819200=new List<int>();
        for(int i=0;i<src.Count;i++)
        {
            if(IsLdc(src[i],8192))p8192.Add(i);
            if(IsLdc(src[i],819200))p819200.Add(i);
        }
        if(p8192.Count!=1||p819200.Count!=1)
        {
            Plugin.I.Logger.LogWarning("[Luck] 황금 물고기 IL 패턴 불일치: 8192="+p8192.Count+", 819200="+p819200.Count+". 황금 물고기 패치를 적용하지 않습니다.");
            foreach(var x in src)yield return x;
            yield break;
        }
        int target=p8192[0];
        for(int i=0;i<src.Count;i++)
        {
            if(i==target)
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call,Helper);
            }
            else yield return src[i];
        }
        Plugin.I.Logger.LogInfo("[Luck] 황금 물고기 PC 파티 1/8192 좁은 패치 적용");
    }
}
'''
pos=s.rfind('\n}')
if pos<0: raise SystemExit('namespace end not found')
s=s[:pos]+block+s[pos:]
p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj';cs.write_text(cs.read_text().replace('<Version>3.14.0</Version>','<Version>3.15.0</Version>'))
pkg=root/'package.xml';pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.14','Elona Luck for Elin v3.15'))
rd=root/'README_KR.md';t=rd.read_text().replace('# Elona Luck for Elin v3.14','# Elona Luck for Elin v3.15');t+='''\n\n## v3.15 황금 물고기 Luck\n- AI_Fish.Makefish의 지역 어종 65 -> 65_gold 변환에서 PC 파티용 원본 1/8192 분모만 Luck으로 완화합니다.\n- NPC용 1/819200 판정은 원본 그대로 둡니다.\n- 기본은 Luck/20% 상대 확률 증가, 상한 +300%. Luck 2000이면 약 1/4096, 상한에서는 약 1/2048 수준입니다.\n- 8192와 819200 상수가 각각 정확히 한 번 확인되지 않으면 원본 IL을 그대로 반환하고 기능만 비활성화합니다.\n- 물고기 생성, tier, 수량 1 고정, bait 소비, fished 카운트는 원본 코드에 맡깁니다.\n- 고빈도 ProgressFish.OnProgress 입질/끌어올리기 루프는 안정성을 위해 수정하지 않습니다.\n- 외부 Lucky Fishing은 v3.13부터 내장 대체되므로 중복 설치하지 않는 것을 권장합니다.\n''';rd.write_text(t)
