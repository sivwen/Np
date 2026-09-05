from pathlib import Path
import runpy

runpy.run_path('_tmp/build_v312.py', run_name='__main__')
root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.12", V="3.12.0"','N="Elona Luck for Elin v3.13", V="3.13.0"')

# Replace the old conditional transpiler registration with a self-contained Postfix replacement.
old='        if(HasAssembly("LuckyFishing"))Logger.LogWarning("[Luck] LuckyFishing 감지: 낚시 희귀 보상 transpiler는 충돌 방지를 위해 자동 비활성화합니다. 물고기 tier Postfix는 유지합니다.");else PatchClass("낚시 희귀 보상",typeof(FishRareRewardPatch));'
new='        if(HasAssembly("LuckyFishing"))Logger.LogWarning("[Luck] 외부 LuckyFishing 감지: 이 모드가 동일 기능을 내장 대체하므로 외부 LuckyFishing을 비활성화/구독 해제하는 것을 권장합니다. 내장 대체 기능은 계속 사용합니다.");\n        PatchClass("Lucky Fishing 대체 특별 아이템 추가 롤",typeof(LuckyFishingReplacementPatch));'
if old not in s: raise SystemExit('old LuckyFishing registration not found')
s=s.replace(old,new)

block=r'''

[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]
static class LuckyFishingReplacementPatch
{
    static ConfigEntry<bool>? Enabled,FlavorLog;
    static ConfigEntry<int>? LuckPerRoll,MaxExtraRolls;
    static readonly HashSet<string> SpecialIds=new HashSet<string>{
        "book_ancient","medal","plat","scratchcard","casino_coin","gacha_coin",
        "659","758","759","806","828","1190","1191"
    };
    static readonly string[] RareIds={"659","758","759","806","828","1190","1191"};

    static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("낚시 운","Lucky Fishing 내장 대체",true,"외부 Lucky Fishing의 핵심 기능인 '1 + LUC/100 특수 아이템 롤'을 Makefish IL 수정 없이 안전한 후처리로 대체합니다.");
        LuckPerRoll=Plugin.I.Config.Bind("낚시 운","특수 아이템 추가 롤당 운",100,"운이 이 값만큼 오를 때 특수 아이템 추가 롤을 1회 얻습니다. 원본 Lucky Fishing 기본값은 100입니다.");
        MaxExtraRolls=Plugin.I.Config.Bind("낚시 운","특수 아이템 추가 롤 상한",99,"한 번 낚시에 추가할 최대 특수 아이템 롤 수입니다. 기본값 99는 모드의 Luck 상한 9999와 원본 LUC/100 공식을 그대로 수용합니다.");
        FlavorLog=Plugin.I.Config.Bind("낚시 운","특수 아이템 행운 로그",true,"추가 Luck 롤로 특수 아이템을 낚았을 때 짧은 게임 로그를 표시합니다.");
        return true;
    }

    static string RollSpecial(Chara c)
    {
        int skill=Math.Max(0,c.Evalue(245));
        string id="";
        if(EClass.rnd(30)==0)id="book_ancient";
        if(EClass.rnd(40)==0&&EClass.rnd(40)<skill/3+10)id="medal";
        if(EClass.rnd(35)==0)
        {
            id="plat";
            if(EClass.rnd(2)==0)id="scratchcard";
            if(EClass.rnd(3)==0)id="casino_coin";
            if(EClass.rnd(3)==0)id="gacha_coin";
            if(EClass.rnd(50)==0)id=RareIds[EClass.rnd(RareIds.Length)];
        }
        return id;
    }

    [HarmonyPriority(Priority.First)]
    static void Postfix(Chara c,ref Thing __result)
    {
        if(Enabled==null||!Enabled.Value||c==null||!c.IsPC)return;
        try
        {
            // The base game already performed the first special roll. If it succeeded, keep it.
            if(__result!=null&&SpecialIds.Contains(__result.id))return;
            int per=Math.Max(1,LuckPerRoll?.Value??100);
            int extra=Math.Min(Math.Max(0,MaxExtraRolls?.Value??99),Math.Max(0,Plugin.Luck()/per));
            if(extra<=0)return;
            string found="";
            for(int i=0;i<extra;i++)
            {
                string id=RollSpecial(c);
                if(id!=""){found=id;break;}
            }
            if(found=="")return;
            if(__result!=null){try{__result.Destroy();}catch{}}
            __result=ThingGen.Create(found,-1,EClass._zone.ContentLv);
            __result.SetBlessedState(BlessedState.Normal);
            if(FlavorLog!=null&&FlavorLog.Value)Msg.SayRaw("행운이 물밑의 특별한 보상을 끌어올렸다.");
        }
        catch(Exception ex){Plugin.I.Logger.LogWarning("[Luck] Lucky Fishing 내장 대체 예외: "+ex.GetType().Name+" "+ex.Message);}
    }
}
'''
pos=s.rfind('\n}')
if pos<0: raise SystemExit('namespace end not found')
s=s[:pos]+block+s[pos:]
p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj';cs.write_text(cs.read_text().replace('<Version>3.12.0</Version>','<Version>3.13.0</Version>'))
pkg=root/'package.xml';pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.12','Elona Luck for Elin v3.13'))
rd=root/'README_KR.md';t=rd.read_text().replace('# Elona Luck for Elin v3.12','# Elona Luck for Elin v3.13');t+='''\n\n## v3.13 Lucky Fishing 내장 대체\n- Steam Workshop Lucky Fishing의 공개 설명상 핵심 기능인 `1 + (LUC / 100)` 특수 아이템 롤을 내장합니다.\n- 원본 Makefish가 이미 수행한 1회는 그대로 두고 `LUC / 100`회만 추가로 후처리합니다.\n- 각 추가 롤은 현재 게임의 고대책 1/30, 메달 1/40 + 스킬 조건, 코인 묶음 1/35, 묶음 내부 scratch/casino/gacha 및 희귀품 1/50 순서를 그대로 재현합니다.\n- Makefish IL/transpiler를 사용하지 않습니다. 외부 LuckyFishing은 더 이상 필요하지 않으며 중복 방지를 위해 비활성화/구독 해제를 권장합니다.\n- v3.12의 빈 낚싯줄 구조, 정크→정상 어획 전환, 물고기 tier Luck도 유지합니다.\n- 65_gold와 에헤카틀 1/1000 도주는 이번 버전에서도 원본 유지합니다.\n''';rd.write_text(t)

st=root/'refs/Elin/Stub.cs';h=st.read_text();h=h.replace('public class Zone{public string id="";public Card AddCard(Card c,Point p)=>c;}', 'public class Zone{public string id="";public int ContentLv=1;public Card AddCard(Card c,Point p)=>c;}');st.write_text(h)
