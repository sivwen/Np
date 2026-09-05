from pathlib import Path
import runpy

runpy.run_path('_tmp/build_v313.py', run_name='__main__')
root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.13", V="3.13.0"','N="Elona Luck for Elin v3.14", V="3.14.0"')

anchor='        PatchClass("낚시 실패/정크 운",typeof(FishingOutcomeLuckPatch));\n'
add='        PatchClass("에헤카틀 물고기 도주 회피 운",typeof(EhekatlFishEscapeLuckPatch));\n'
if add not in s:
    if anchor not in s: raise SystemExit('fishing outcome registration anchor not found')
    s=s.replace(anchor,anchor+add)

block=r'''

[HarmonyPatch(typeof(AI_Fish.ProgressFish),nameof(AI_Fish.ProgressFish.OnProgressComplete),new Type[]{})]
static class EhekatlFishEscapeLuckPatch
{
    static ConfigEntry<bool>? Enabled;
    static ConfigEntry<int>? LuckDiv,LuckCap;
    static readonly MethodInfo Rnd=typeof(EClass).GetMethod(nameof(EClass.rnd),new Type[]{typeof(int)})!;
    static readonly MethodInfo Helper=typeof(EhekatlFishEscapeLuckPatch).GetMethod(nameof(EscapeRnd),BindingFlags.Static|BindingFlags.NonPublic)!;

    static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("낚시 운","에헤카틀 물고기 도주 회피 운",true,"잡은 물고기가 에헤카틀에게 1/1000로 사라지는 원본 판정의 분모만 Luck으로 늘립니다. 해당 IL 패턴이 정확히 하나일 때만 적용됩니다.");
        LuckDiv=Plugin.I.Config.Bind("낚시 운","에헤카틀 도주 회피 운 분모",20,"운이 이 값만큼 오를 때 도주 확률 분모가 1% 증가합니다.");
        LuckCap=Plugin.I.Config.Bind("낚시 운","에헤카틀 도주 회피 보너스 상한",200,"도주 확률 분모 증가 상한(%)입니다. 100이면 1/1000이 약 1/2000이 됩니다.");
        if(Plugin.HasAssembly("InstantFishing"))
        {
            Plugin.I.Logger.LogWarning("[Luck] InstantFishing 계열 감지: ProgressFish.OnProgressComplete 충돌 방지를 위해 에헤카틀 도주 회피 패치를 비활성화합니다.");
            return false;
        }
        return true;
    }

    static bool IsLdc(CodeInstruction c,int v)
    {
        if(c.opcode==OpCodes.Ldc_I4)return c.operand is int x&&x==v;
        if(c.opcode==OpCodes.Ldc_I4_S)return Convert.ToInt32(c.operand)==v;
        return false;
    }
    static bool IsRnd(CodeInstruction c)=>c.operand is MethodInfo m&&m==Rnd&&(c.opcode==OpCodes.Call||c.opcode==OpCodes.Callvirt);

    static int EscapeRnd(int max,AI_Fish.ProgressFish self)
    {
        if(Enabled==null||!Enabled.Value||self==null||self.owner==null||!self.owner.IsPC||max!=1000)return EClass.rnd(max);
        int bonus=Plugin.RelativeBonus(LuckDiv!,LuckCap!);
        long boosted=(long)max*(100L+bonus)/100L;
        int denom=(int)Math.Min(int.MaxValue,Math.Max(max,boosted));
        return EClass.rnd(denom);
    }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var src=new List<CodeInstruction>(instructions);
        var hits=new List<int>();
        for(int i=0;i<src.Count;i++)
        {
            if(!IsRnd(src[i]))continue;
            bool has1000=false;
            for(int j=Math.Max(0,i-8);j<i;j++)if(IsLdc(src[j],1000)){has1000=true;break;}
            if(has1000)hits.Add(i);
        }
        if(hits.Count!=1)
        {
            Plugin.I.Logger.LogWarning("[Luck] 에헤카틀 도주 IL 패턴 불일치: rnd(1000) 후보="+hits.Count+". 패치를 적용하지 않습니다.");
            foreach(var x in src)yield return x;
            yield break;
        }
        int target=hits[0];
        for(int i=0;i<src.Count;i++)
        {
            if(i==target)
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Call,Helper);
            }
            else yield return src[i];
        }
        Plugin.I.Logger.LogInfo("[Luck] 에헤카틀 물고기 도주 1/1000 좁은 패치 적용");
    }
}
'''
pos=s.rfind('\n}')
if pos<0: raise SystemExit('namespace end not found')
s=s[:pos]+block+s[pos:]
p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj';cs.write_text(cs.read_text().replace('<Version>3.13.0</Version>','<Version>3.14.0</Version>'))
pkg=root/'package.xml';pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.13','Elona Luck for Elin v3.14'))
rd=root/'README_KR.md';t=rd.read_text().replace('# Elona Luck for Elin v3.13','# Elona Luck for Elin v3.14');t+='''\n\n## v3.14 에헤카틀 물고기 도주 회피\n- ProgressFish.OnProgressComplete의 물고기 1/1000 도주 RNG만 좁게 식별해 Luck으로 분모를 증가시킵니다.\n- 기본은 Luck/20%만큼 분모 증가, 상한 +200%. Luck 2000이면 1/1000이 약 1/2000 수준입니다.\n- rnd(1000) 후보가 정확히 하나가 아니면 원본 IL을 그대로 반환하고 기능만 비활성화합니다.\n- InstantFishing 계열 어셈블리가 감지되면 이 패치만 자동 비활성화합니다.\n- v3.13의 Lucky Fishing 내장 대체와 v3.12의 실패/정크/tier Luck은 그대로 유지합니다.\n''';rd.write_text(t)

st=root/'refs/Elin/Stub.cs';h=st.read_text()
if 'class AI_FishProgressBase' not in h:
    h=h.replace('public class AI_Fish{public static Thing Makefish(Chara c)=>null;}', 'public class AI_Fish{public class ProgressFish{public Chara owner=new Chara();public void OnProgressComplete(){}}public static Thing Makefish(Chara c)=>null;}')
st.write_text(h)
