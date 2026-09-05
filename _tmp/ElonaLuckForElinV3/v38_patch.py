from pathlib import Path

root=Path('_tmp/ElonaLuckForElinV3')
p=root/'Plugin.cs'
s=p.read_text()
s=s.replace('N="Elona Luck for Elin v3.7", V="3.7.0"','N="Elona Luck for Elin v3.8", V="3.8.0"')

s=s.replace('PatchClass("보물상자",typeof(TreasureRarityNarrowPatch));','PatchClass("보물상자",typeof(TreasureRarityNarrowPatch));\n        PatchClass("블랙마켓 희귀도 컨텍스트",typeof(BlackmarketContextPatch));\n        PatchClass("블랙마켓 희귀도 승급",typeof(BlackmarketRarityPatch));')

if 'static class BlackmarketContext' not in s:
    block=r'''

static class BlackmarketContext
{
    [ThreadStatic] internal static int depth;
    [ThreadStatic] internal static bool active;
}

[HarmonyPatch]
static class BlackmarketContextPatch
{
    static MethodBase? target;
    static bool Prepare()
    {
        target=FindTarget();
        if(target==null)
        {
            Plugin.I.Logger.LogWarning("[Luck] 블랙마켓 희귀도: shop_blackmarket 생성 메서드를 찾지 못해 비활성화합니다.");
            return false;
        }
        return true;
    }
    static MethodBase TargetMethod()=>target!;

    static MethodBase? FindTarget()
    {
        foreach(var m in typeof(Trait).GetMethods(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))
        {
            if(HasString(m,"shop_blackmarket"))return m;
        }
        foreach(var t in typeof(Trait).GetNestedTypes(BindingFlags.Public|BindingFlags.NonPublic))
        foreach(var m in t.GetMethods(BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic))
        {
            if(HasString(m,"shop_blackmarket"))return m;
        }
        return null;
    }

    static bool HasString(MethodBase m,string value)
    {
        try
        {
            var il=m.GetMethodBody()?.GetILAsByteArray();
            if(il==null)return false;
            var mod=m.Module;
            for(int i=0;i<il.Length;)
            {
                ushort code=il[i++];
                if(code==0xFE){if(i>=il.Length)break;code=(ushort)(0xFE00|il[i++]);}
                OpCode op=OpCodes.Nop;
                foreach(var f in typeof(OpCodes).GetFields(BindingFlags.Public|BindingFlags.Static))
                {
                    if(f.GetValue(null) is OpCode o && (ushort)o.Value==code){op=o;break;}
                }
                int size=0;
                switch(op.OperandType)
                {
                    case OperandType.InlineNone:size=0;break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar:size=1;break;
                    case OperandType.InlineVar:size=2;break;
                    case OperandType.InlineI:
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineField:
                    case OperandType.InlineMethod:
                    case OperandType.InlineSig:
                    case OperandType.InlineString:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.ShortInlineR:size=4;break;
                    case OperandType.InlineI8:
                    case OperandType.InlineR:size=8;break;
                    case OperandType.InlineSwitch:
                        if(i+4>il.Length)return false;
                        int n=BitConverter.ToInt32(il,i);size=4+n*4;break;
                }
                if(op==OpCodes.Ldstr && i+4<=il.Length)
                {
                    int tok=BitConverter.ToInt32(il,i);
                    if(mod.ResolveString(tok)==value)return true;
                }
                i+=size;
            }
        }
        catch{}
        return false;
    }

    static void Prefix(object __instance)
    {
        BlackmarketContext.depth++;
        if(BlackmarketContext.depth!=1)return;
        BlackmarketContext.active=false;
        try
        {
            if(__instance is Trait tr)
                BlackmarketContext.active = tr.ShopType==ShopType.Blackmarket || tr.ShopType==ShopType.Exotic;
        }
        catch{}
    }
    static void Finalizer()
    {
        BlackmarketContext.depth=Math.Max(0,BlackmarketContext.depth-1);
        if(BlackmarketContext.depth==0)BlackmarketContext.active=false;
    }
}

[HarmonyPatch(typeof(CardBlueprint),nameof(CardBlueprint.SetRarity),new Type[]{typeof(Rarity)})]
static class BlackmarketRarityPatch
{
    static ConfigEntry<bool>? Enabled,FlavorLog;
    static ConfigEntry<int>? LuckDiv,LuckCap;
    static bool Prepare()
    {
        Enabled=Plugin.I.Config.Bind("장비 운","블랙마켓 장비 희귀도 운",true,"블랙마켓/Exotic 상점의 장비 희귀도 결정 직후에만 Luck으로 한 단계 승급 기회를 줍니다. Thing.OnCreate는 패치하지 않습니다.");
        LuckDiv=Plugin.I.Config.Bind("장비 운","블랙마켓 희귀도 운 분모",20,"운/이 값(%)을 기본 승급 확률로 사용합니다.");
        LuckCap=Plugin.I.Config.Bind("장비 운","블랙마켓 희귀도 승급 상한",50,"한 번의 희귀도 승급 확률 상한(%)입니다.");
        FlavorLog=Plugin.I.Config.Bind("장비 운","블랙마켓 희귀도 플레이버 로그",true,"Luck으로 실제 희귀도가 승급됐을 때 게임 플레이 로그에 짧은 메시지를 표시합니다.");
        return true;
    }
    static void Prefix(ref Rarity q)
    {
        if(Enabled==null||!Enabled.Value||!BlackmarketContext.active)return;
        if(q>=Rarity.Mythical||q>=Rarity.Artifact)return;
        int chance=Math.Min(Math.Max(0,LuckCap?.Value??50),Math.Max(0,Plugin.Luck()/Math.Max(1,LuckDiv?.Value??20)));
        if(chance<=0||EClass.rnd(100)>=chance)return;
        Rarity old=q;
        if(q<=Rarity.Normal)q=Rarity.Superior;
        else if(q==Rarity.Crude)q=Rarity.Superior;
        else if(q==Rarity.Superior)q=Rarity.Legendary;
        else if(q==Rarity.Legendary)q=Rarity.Mythical;
        if(q!=old && FlavorLog!=null&&FlavorLog.Value)Msg.SayRaw("행운이 좋은 물건을 끌어당겼다. 블랙마켓 장비의 품질이 한층 높아졌다.");
    }
}
'''
    pos=s.rfind('\n}')
    s=s[:pos]+block+s[pos:]

p.write_text(s)

cs=root/'ElonaLuckForElinV3.csproj'
cs.write_text(cs.read_text().replace('<Version>3.7.0</Version>','<Version>3.8.0</Version>'))

pkg=root/'package.xml'
pkg.write_text(pkg.read_text().replace('Elona Luck for Elin v3.7','Elona Luck for Elin v3.8'))

rd=root/'README_KR.md'
t=rd.read_text().replace('# Elona Luck for Elin v3.7','# Elona Luck for Elin v3.8')
if '## v3.8 장비 희귀도 안전 복원' not in t:
    t += '''\n\n## v3.8 장비 희귀도 안전 복원\n- v2의 전역 `Thing.OnCreate` 후킹은 복원하지 않습니다.\n- 원본에서 블랙마켓/Exotic 상점 재고가 `CardBlueprint.SetRarity`로 희귀도를 고른 뒤 장비를 생성하는 경로만 컨텍스트로 감지합니다.\n- 그 컨텍스트 안에서만 Luck으로 이미 선택된 희귀도를 최대 한 단계 승급합니다.\n- 기본 확률은 `Luck / 20 (%)`, 상한 50%입니다.\n- Normal/Crude -> Superior, Superior -> Legendary, Legendary -> Mythical. Mythical/Artifact 이상은 건드리지 않습니다.\n- 희귀도 승급 뒤에는 원본 `Thing.OnCreate`가 해당 희귀도에 맞춰 인챈트 개수/소켓을 자연스럽게 생성하므로 후처리 재계산을 하지 않습니다.\n- 승급이 실제 발생했을 때만 플레이버 로그를 표시하며 설정에서 끌 수 있습니다.\n- 보물상자 장비 희귀도 Luck은 기존 좁은 패치를 그대로 유지합니다.\n- 일반 전역 장비 생성, 몬스터 소지 장비, 제작 장비의 희귀도는 아직 건드리지 않습니다. 안전한 생성 컨텍스트가 확인될 때만 추가합니다.\n'''
rd.write_text(t)
