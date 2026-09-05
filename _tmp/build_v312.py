from pathlib import Path
import runpy

# Start from the already-audited v3.11 transform.
runpy.run_path('_tmp/build_v311.py', run_name='__main__')
root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.11", V="3.11.0"','N="Elona Luck for Elin v3.12", V="3.12.0"')

# Register safe outcome-level fishing luck. This is a Postfix only; no Makefish IL edit.
anchor='        PatchClass("낚시 품질",typeof(FishPatch));\n'
add='        PatchClass("낚시 실패/정크 운",typeof(FishingOutcomeLuckPatch));\n'
if add not in s:
    if anchor not in s: raise SystemExit('fish patch anchor not found')
    s=s.replace(anchor,anchor+add)

block=r'''

[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]
static class FishingOutcomeLuckPatch
{
    static ConfigEntry<bool>? RescueEnabled,JunkConvertEnabled,FlavorLog;
    static ConfigEntry<int>? RescueDiv,RescueCap,JunkDiv,JunkCap;
    static readonly HashSet<string> JunkIds=new HashSet<string>{"233","235","236","1170","1143","1144","727","728","237","869","1178","1179","1180","1243","1244","1245"};

    static bool Prepare()
    {
        RescueEnabled=Plugin.I.Config.Bind("낚시 운","빈 낚싯줄 구조 운",true,"Makefish가 원래 null을 반환한 경우에만 Luck으로 일반 어획 1회를 구조합니다. 원본 실패 판정식은 수정하지 않습니다.");
        RescueDiv=Plugin.I.Config.Bind("낚시 운","실패 구조 운 분모",25,"실패 구조 확률은 운/이 값(%)입니다.");
        RescueCap=Plugin.I.Config.Bind("낚시 운","실패 구조 확률 상한",50,"빈 낚싯줄 구조 확률 상한(%)입니다.");
        JunkConvertEnabled=Plugin.I.Config.Bind("낚시 운","정크 어획 전환 운",true,"원본 기타/정크 어획 풀이 선택된 뒤에만 Luck으로 정상 물고기로 전환할 기회를 줍니다. 원본 정크 분기 확률은 수정하지 않습니다.");
        JunkDiv=Plugin.I.Config.Bind("낚시 운","정크 전환 운 분모",20,"정크를 정상 어획으로 전환할 확률은 운/이 값(%)입니다.");
        JunkCap=Plugin.I.Config.Bind("낚시 운","정크 전환 확률 상한",60,"정크 전환 확률 상한(%)입니다.");
        FlavorLog=Plugin.I.Config.Bind("낚시 운","낚시 운 플레이버 로그",true,"Luck이 빈 낚싯줄을 구조하거나 정크를 정상 어획으로 바꿨을 때 짧은 게임 로그를 표시합니다.");
        return true;
    }

    static int Chance(ConfigEntry<int>? div,ConfigEntry<int>? cap)
        =>Math.Min(Math.Max(0,cap?.Value??0),Math.Max(0,Plugin.Luck()/Math.Max(1,div?.Value??1)));

    static Thing MakeNormalFish(Chara c)
    {
        int skill=Math.Max(0,c.Evalue(245));
        int lv=Math.Max(1,EClass.rnd(Math.Max(1,skill*2))+1);
        string id=(EClass._zone!=null&&EClass._zone.id=="startVillage2")?"65":"fish";
        Thing t=ThingGen.Create(id,-1,lv);
        t.SetBlessedState(BlessedState.Normal);
        return t;
    }

    [HarmonyPriority(Priority.First)]
    static void Postfix(Chara c,ref Thing __result)
    {
        if(c==null||!c.IsPC)return;
        try
        {
            if(__result==null)
            {
                if(RescueEnabled==null||!RescueEnabled.Value)return;
                int chance=Chance(RescueDiv,RescueCap);
                if(chance<=0||EClass.rnd(100)>=chance)return;
                __result=MakeNormalFish(c);
                if(FlavorLog!=null&&FlavorLog.Value)Msg.SayRaw("행운이 빈 낚싯줄을 그냥 돌려보내지 않았다.");
                return;
            }
            if(JunkConvertEnabled==null||!JunkConvertEnabled.Value||!JunkIds.Contains(__result.id))return;
            int junkChance=Chance(JunkDiv,JunkCap);
            if(junkChance<=0||EClass.rnd(100)>=junkChance)return;
            try{__result.Destroy();}catch{}
            __result=MakeNormalFish(c);
            if(FlavorLog!=null&&FlavorLog.Value)Msg.SayRaw("행운이 잡동사니 대신 제대로 된 어획을 끌어냈다.");
        }
        catch(Exception ex){Plugin.I.Logger.LogWarning("[Luck] 낚시 실패/정크 후처리 예외: "+ex.GetType().Name+" "+ex.Message);}
    }
}
'''
pos=s.rfind('\n}')
if pos<0: raise SystemExit('namespace end not found')
s=s[:pos]+block+s[pos:]
p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj';cs.write_text(cs.read_text().replace('<Version>3.11.0</Version>','<Version>3.12.0</Version>'))
pkg=root/'package.xml';pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.11','Elona Luck for Elin v3.12'))
rd=root/'README_KR.md';t=rd.read_text().replace('# Elona Luck for Elin v3.11','# Elona Luck for Elin v3.12');t+='''\n\n## v3.12 낚시 결과 Luck 확장\n- 원본 Makefish 실패식과 정크 분기식을 IL로 수정하지 않습니다.\n- Makefish가 null을 반환했을 때만 Luck 기반 1회 구조를 합니다. 기본 운/25%, 상한 50%.\n- 원본 기타/정크 어획 풀(코드상 18칸, 중복 ID 포함)의 결과가 나온 경우 Luck으로 정상 물고기로 전환할 수 있습니다. 기본 운/20%, 상한 60%.\n- startVillage2에서는 구조/전환 물고기도 지역 어종 65를 사용합니다.\n- 결과 후처리 Postfix이므로 LuckyFishing이 있어도 동작합니다. 기존 희귀 보상 IL transpiler는 LuckyFishing 감지 시 계속 자동 비활성화합니다.\n- 기존 물고기 tier Luck은 유지합니다.\n- 고대책/메달/코인/희귀품/대어 보정은 LuckyFishing이 없을 때만 기존 좁은 transpiler가 담당합니다.\n- 황금 물고기(65_gold), 낚싯줄 진행 루프, bait 소비는 건드리지 않습니다.\n''';rd.write_text(t)

# Compile stubs for new safe Postfix only.
st=root/'refs/Elin/Stub.cs';h=st.read_text()
h=h.replace('public class Thing:Card{public int tier;public int Num=1;public void SetTier(int t){tier=t;}public void ModNum(int n,bool notify=true){Num+=n;}public Thing Duplicate(int n)=>new Thing{Num=n,id=id};}', 'public class Thing:Card{public int tier;public int Num=1;public void SetTier(int t){tier=t;}public void ModNum(int n,bool notify=true){Num+=n;}public Thing Duplicate(int n)=>new Thing{Num=n,id=id};public void Destroy(){}public void SetBlessedState(BlessedState s){}}')
h=h.replace('public class Zone{public Card AddCard(Card c,Point p)=>c;}', 'public class Zone{public string id="";public Card AddCard(Card c,Point p)=>c;}')
st.write_text(h)
hs=root/'refs/Harmony/Stub.cs';h=hs.read_text();h=h.replace('public static class Priority{public const int Last=0;}', 'public static class Priority{public const int Last=0;public const int First=800;}');hs.write_text(h)
