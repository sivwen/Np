from pathlib import Path
root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.6", V="3.6.0"','N="Elona Luck for Elin v3.7", V="3.7.0"')
s=s.replace('PatchClass("낚시 품질",typeof(FishPatch));','PatchClass("낚시 품질",typeof(FishPatch));\n        PatchClass("낚시 희귀 보상",typeof(FishRareRewardPatch));')
s=s.replace('static ConfigEntry<bool>? Enabled;\n    static ConfigEntry<int>? RefundCap;', 'static ConfigEntry<bool>? Enabled;\n    static ConfigEntry<bool>? FlavorLog;\n    static ConfigEntry<int>? RefundCap;')
s=s.replace('RefundCap=Plugin.I.Config.Bind("SkillAndLuckMatter 대체","제작 재료 환급률 상한",50,"재료 1개당 환급 확률의 상한(%)입니다. 안전을 위해 최소 1개는 항상 소비합니다.");', 'RefundCap=Plugin.I.Config.Bind("SkillAndLuckMatter 대체","제작 재료 환급률 상한",50,"재료 1개당 환급 확률의 상한(%)입니다. 안전을 위해 최소 1개는 항상 소비합니다.");\n        FlavorLog=Plugin.I.Config.Bind("SkillAndLuckMatter 대체","제작 환급 플레이버 로그",true,"운으로 실제 재료 소비가 줄었을 때 게임 플레이 로그에 짧은 메시지를 남깁니다.");')
s=s.replace('if(saved>0)__result=Math.Max(1,__result-saved);', 'if(saved>0){__result=Math.Max(1,__result-saved);if(FlavorLog!=null&&FlavorLog.Value)Msg.SayRaw("손끝에 행운이 스쳤다. 재료 "+saved+"개를 아꼈다.");}')
if 'static class FishRareRewardPatch' not in s:
    block=r'''

[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]
static class FishRareRewardPatch
{
    static ConfigEntry<bool>? Enabled,AncientBook,Medal,CoinGroup,SpecialReward,BigCatch;
    static ConfigEntry<int>? LuckDiv,LuckCap;
    static readonly MethodInfo Rnd=typeof(EClass).GetMethod(nameof(EClass.rnd),new Type[]{typeof(int)})!;
    static readonly MethodInfo Helper=typeof(FishRareRewardPatch).GetMethod(nameof(LuckRnd),BindingFlags.Static|BindingFlags.NonPublic)!;

    static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("낚시 운","희귀 보상 운",true,"Makefish 내부에서 확인된 희귀 보상 첫 관문만 운으로 보정합니다.");
        AncientBook=Plugin.I.Config.Bind("낚시 운","고대책 운",true,"고대책의 원래 1/30 판정을 운으로 완화합니다.");
        Medal=Plugin.I.Config.Bind("낚시 운","메달 운",true,"메달의 첫 1/40 관문만 운으로 완화하며 낚시 스킬 조건은 원본 그대로 유지합니다.");
        CoinGroup=Plugin.I.Config.Bind("낚시 운","코인류 운",true,"플래티넘/스크래치/카지노/가챠 코인 묶음의 원래 1/35 관문만 운으로 완화합니다. 내부 보상 종류 비율은 바꾸지 않습니다.");
        SpecialReward=Plugin.I.Config.Bind("낚시 운","특수 희귀품 운",true,"코인류 관문 안의 특수 희귀품 1/50 판정을 운으로 완화합니다.");
        BigCatch=Plugin.I.Config.Bind("낚시 운","대어 운",true,"대어 판정의 rnd(100) 범위만 운으로 완화합니다. 지형/거점 보정값은 원본 그대로입니다.");
        LuckDiv=Plugin.I.Config.Bind("낚시 운","희귀 보상 운 분모",20,"운이 이 값만큼 오를 때 희귀 보상 상대 확률이 1% 증가합니다.");
        LuckCap=Plugin.I.Config.Bind("낚시 운","희귀 보상 상대 보너스 상한",100,"희귀 보상 상대 확률 증가 상한입니다.");
        return true;
    }

    static bool IsLdc(CodeInstruction c,int v)
    {
        if(c.opcode==OpCodes.Ldc_I4)return c.operand is int x&&x==v;
        if(c.opcode==OpCodes.Ldc_I4_S)return Convert.ToInt32(c.operand)==v;
        if(v>=0&&v<=8){OpCode[] a={OpCodes.Ldc_I4_0,OpCodes.Ldc_I4_1,OpCodes.Ldc_I4_2,OpCodes.Ldc_I4_3,OpCodes.Ldc_I4_4,OpCodes.Ldc_I4_5,OpCodes.Ldc_I4_6,OpCodes.Ldc_I4_7,OpCodes.Ldc_I4_8};return c.opcode==a[v];}
        return false;
    }
    static bool IsRnd(CodeInstruction c)=>c.operand is MethodInfo m&&m==Rnd&&(c.opcode==OpCodes.Call||c.opcode==OpCodes.Callvirt);
    static int LuckRnd(int max,Chara c,int kind)
    {
        if(Enabled==null||!Enabled.Value||c==null||!c.IsPC||max<=1)return EClass.rnd(max);
        bool on=kind switch{1=>AncientBook?.Value??false,2=>Medal?.Value??false,3=>CoinGroup?.Value??false,4=>SpecialReward?.Value??false,5=>BigCatch?.Value??false,_=>false};
        if(!on)return EClass.rnd(max);
        int rel=Plugin.RelativeBonus(LuckDiv!,LuckCap!);
        return EClass.rnd(Plugin.ReduceDenom(max,rel));
    }
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var src=new List<CodeInstruction>(instructions);
        int i30=-1,i35=-1,i50=-1,i100=-1,i40first=-1,c40=0;
        for(int i=1;i<src.Count;i++)
        {
            if(!IsRnd(src[i]))continue;
            if(IsLdc(src[i-1],30)&&i30<0)i30=i;
            else if(IsLdc(src[i-1],35)&&i35<0)i35=i;
            else if(IsLdc(src[i-1],50)&&i50<0)i50=i;
            else if(IsLdc(src[i-1],100)&&i100<0)i100=i;
            else if(IsLdc(src[i-1],40)){c40++;if(i40first<0)i40first=i;}
        }
        if(i30<0||i35<0||i50<0||i100<0||i40first<0||c40<2)
        {
            Plugin.I.Logger.LogWarning($"[Luck] 낚시 희귀 보상 IL 패턴 불일치: 30={i30}, 35={i35}, 50={i50}, 100={i100}, 40count={c40}. 희귀 보상 패치를 적용하지 않습니다.");
            foreach(var x in src)yield return x;
            yield break;
        }
        var map=new Dictionary<int,int>{{i30,1},{i40first,2},{i35,3},{i50,4},{i100,5}};
        for(int i=0;i<src.Count;i++)
        {
            if(map.TryGetValue(i,out int kind))
            {
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldc_I4,kind);
                yield return new CodeInstruction(OpCodes.Call,Helper);
            }
            else yield return src[i];
        }
        Plugin.I.Logger.LogInfo("[Luck] 낚시 희귀 보상 좁은 패치 적용: 고대책/메달 첫 관문/코인류/특수 희귀품/대어");
    }
}
'''
    pos=s.rfind('\n}')
    s=s[:pos]+block+s[pos:]
p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj'
cs.write_text(cs.read_text().replace('<Version>3.6.0</Version>','<Version>3.7.0</Version>'))

pkg=root/'package.xml'
pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.6','Elona Luck for Elin v3.7'))

rd=root/'README_KR.md'
t=rd.read_text().replace('# Elona Luck for Elin v3.6','# Elona Luck for Elin v3.7')
marker='## v3.7 제작 환급 로그와 낚시 희귀 보상'
if marker not in t:
    t += '\n\n## v3.7 제작 환급 로그와 낚시 희귀 보상\n'
    t += '- 실제 소비량이 줄었을 때 `Msg.SayRaw`로 게임 플레이 로그에 `손끝에 행운이 스쳤다. 재료 N개를 아꼈다.`를 표시합니다. 설정에서 끌 수 있습니다.\n'
    t += '- AI_Fish.Makefish 전체 RNG를 바꾸지 않고, 코드상 식별이 명확한 희귀 보상 첫 관문만 좁게 보정합니다.\n'
    t += '- 고대책: 원래 1/30\n'
    t += '- 메달: 첫 1/40 관문만 보정. 뒤의 낚시 스킬 조건은 원본 유지\n'
    t += '- 코인류: 원래 1/35 관문만 보정. plat/scratch/casino/gacha 내부 선택 1/2, 1/3, 1/3은 원본 유지\n'
    t += '- 특수 희귀품: 코인 관문 내부 1/50\n'
    t += '- 대어: `num6 >= rnd(100)`의 rnd 범위만 보정하며 거점/지형 보너스 num6는 원본 유지\n'
    t += '- 65_gold는 동적 8192/819200 분기와 생성 교체가 얽혀 있어 이번 배치에서는 보류합니다.\n'
    t += '- 낚시 실패율과 일반 어종 선택 확률도 건드리지 않습니다.\n'
rd.write_text(t)

st=root/'refs/Elin/Stub.cs'
t=st.read_text()
if 'public static class Msg' not in t:
    t += '\npublic static class Msg{public static string SayRaw(string text)=>text;}\n'
st.write_text(t)
