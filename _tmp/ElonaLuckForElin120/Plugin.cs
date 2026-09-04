using BepInEx;using BepInEx.Configuration;using HarmonyLib;using System;using System.Collections.Generic;using System.Diagnostics;
namespace ElonaLuckForElin{
[BepInPlugin(G,N,V)]public sealed class Plugin:BaseUnityPlugin{
public const string G="sivwen.elin.elonaluck",N="Elona Luck for Elin",V="2.0.4";
internal static ConfigEntry<bool>Q=null!,D=null!,EC=null!,ET=null!,EV=null!,Craft=null!,Ammo=null!,Drop=null!,StealWeight=null!,WitnessLuck=null!,LockLuck=null!,FishLuck=null!,DismantleLuck=null!,ActivityBonus=null!,BonusMine=null!,BonusDig=null!,BonusHarvest=null!,BonusFish=null!,BonusCraft=null!,AutoDisableSALM=null!,NestLuck=null!,SeedLuck=null!,TreasureLuck=null!,ScratchLuck=null!,GachaLuck=null!,CasinoLuck=null!,CorpseLuck=null!,GeneLuck=null!,UniqueLootLuck=null!,CombatLootLuck=null!,GeneralMaterialLuck=null!,Log=null!;
internal static ConfigEntry<int>QD=null!,CountDiv=null!,CountCap=null!,TierDiv=null!,TierCap=null!,ValueDiv=null!,ValueCap=null!,DropDiv=null!,DropCap=null!,StealDiv=null!,StealCap=null!,WitnessDiv=null!,WitnessCap=null!,LockDiv=null!,LockCap=null!,FishDiv=null!,FishCap=null!,DismantleDiv=null!,DismantleCap=null!,ActSkillWeight=null!,ActLuckWeight=null!,ActSkillMod=null!,ActLuckMod=null!,NestDiv=null!,NestCap=null!,SeedDiv=null!,SeedCap=null!,TreasureDiv=null!,TreasureCap=null!,TreasureMythDiv=null!,ScratchDiv=null!,ScratchCap=null!,GachaDiv=null!,GachaCap=null!,CasinoDiv=null!,CasinoCap=null!,AnatomySkillWeight=null!,AnatomyLuckWeight=null!,GeneBonusCap=null!,UniqueLootDiv=null!,UniqueLootCap=null!,CombatLuckDiv=null!,CombatLuckCap=null!,CritKillBonus=null!,FinishKillBonus=null!,ExecutionerBonus=null!,OverkillCap=null!,CombatTotalCap=null!;
internal static Plugin I=null!; Harmony? h;
void Awake(){I=this;
Q=Config.Bind("엘로나식 운","장비 품질 운 적용",false,"엘로나식 장비 품질 1단계 상승을 적용합니다.");
D=Config.Bind("엘로나식 운","Elin 전역 운 재굴림 제거",false,"Elin 기본 전역 운 재굴림을 제거합니다.");
QD=Config.Bind("엘로나식 운","장비 품질 판정 분모",5000,"엘로나 기본값은 5000입니다.");
EC=Config.Bind("인챈트 운","인챈트 개수 운 적용",false,"운에 따라 랜덤 인챈트 개수를 늘립니다.");
CountDiv=Config.Bind("인챈트 운","인챈트 개수 운 분모",250,"운이 이 수치만큼 오를 때마다 추가 인챈트 확률이 1% 증가합니다.");
CountCap=Config.Bind("인챈트 운","인챈트 개수 확률 상한",35,"추가 인챈트 1회 판정의 최대 확률입니다.");
ET=Config.Bind("인챈트 운","인챈트 등급 운 적용",false,"운에 따라 추가 인챈트의 가상 생성 레벨을 높입니다.");
TierDiv=Config.Bind("인챈트 운","인챈트 등급 운 분모",20,"운을 이 값으로 나눈 만큼 생성 레벨에 더합니다.");
TierCap=Config.Bind("인챈트 운","인챈트 등급 레벨 보너스 상한",150,"운으로 얻을 수 있는 생성 레벨 보너스 상한입니다.");
EV=Config.Bind("인챈트 운","인챈트 수치 운 적용",false,"새로 생성되는 랜덤 인챈트의 수치를 운으로 강화합니다.");
ValueDiv=Config.Bind("인챈트 운","인챈트 수치 운 분모",2000,"운을 이 값으로 나눈 비율만큼 인챈트 수치가 증가합니다. 상한 적용 전 기준 운 2000은 +100%입니다.");
ValueCap=Config.Bind("인챈트 운","인챈트 수치 보너스 상한",50,"인챈트 수치 증가율의 최대값입니다.");
Drop=Config.Bind("드롭 운","드롭 운 전체 사용",true,"몬스터 드롭 운 보정 전체를 켜거나 끕니다. 피규어와 박제에는 적용되지 않습니다.");
DropDiv=Config.Bind("드롭 운","일반 소재 운 분모",50,"운이 이 수치만큼 오를 때마다 일반 몬스터 소재의 상대 드롭 확률이 1% 증가합니다.");
DropCap=Config.Bind("드롭 운","일반 소재 보너스 상한",100,"일반 몬스터 소재의 상대 드롭 확률 증가 상한입니다.");
GeneralMaterialLuck=Config.Bind("드롭 운","일반 소재 드롭 운",true,"송곳니, 가죽, 내장, 심장, 기계 부품 등 일반 소재 드롭에 운을 적용합니다.");
CorpseLuck=Config.Bind("드롭 운","시체 해부학+운",false,"시체 관련 판정에서 운이 해부학을 보조하도록 합니다.");
GeneLuck=Config.Bind("드롭 운","유전자 해부학+운",true,"해부학과 운을 함께 사용해 유전자 드롭 확률을 높입니다.");
AnatomySkillWeight=Config.Bind("드롭 운","해부학 가중치",3,"시체/유전자 판정에서 해부학(290)의 가중치입니다.");
AnatomyLuckWeight=Config.Bind("드롭 운","운 가중치",2,"시체/유전자 판정에서 운의 가중치입니다.");
GeneBonusCap=Config.Bind("드롭 운","유전자 상대 보너스 상한",300,"해부학+운으로 얻을 수 있는 유전자 상대 드롭 확률 보너스 상한입니다.");
UniqueLootLuck=Config.Bind("드롭 운","몬스터 고유 드롭 운",true,"몬스터 전용 희귀품과 아티팩트 등 sourceCard/race 고유 드롭에 운을 적용합니다.");
UniqueLootDiv=Config.Bind("드롭 운","고유 드롭 운 분모",10,"운이 이 수치만큼 오를 때마다 고유 드롭의 상대 확률이 1% 증가합니다.");
UniqueLootCap=Config.Bind("드롭 운","고유 드롭 상대 보너스 상한",300,"몬스터 고유 드롭의 상대 확률 증가 상한입니다.");
CombatLootLuck=Config.Bind("드롭 운","전투 마무리 드롭 운",true,"몬스터 소지품/장비 드롭을 운과 처치 품질로 보정합니다.");
CombatLuckDiv=Config.Bind("드롭 운","전투 드롭 운 분모",10,"운이 이 수치만큼 오를 때마다 소지품/장비의 상대 드롭 확률이 1% 증가합니다.");
CombatLuckCap=Config.Bind("드롭 운","전투 드롭 운 기여 상한",100,"전투 전리품 보너스에서 운이 기여할 수 있는 최대값입니다.");
CritKillBonus=Config.Bind("드롭 운","크리티컬 처치 보너스",50,"마지막 일격이 크리티컬일 때 적용할 상대 드롭 보너스입니다.");
FinishKillBonus=Config.Bind("드롭 운","Finish 처치 보너스",100,"Finish 판정으로 처치했을 때 적용할 상대 드롭 보너스입니다.");
ExecutionerBonus=Config.Bind("드롭 운","처형자 레벨당 보너스",25,"처형자 특성(1420) 1레벨당 적용할 상대 드롭 보너스입니다.");
OverkillCap=Config.Bind("드롭 운","오버킬 보너스 상한",100,"대상 최대 HP 대비 오버킬 비율에서 얻는 상대 드롭 보너스 상한입니다.");
CombatTotalCap=Config.Bind("드롭 운","전투 드롭 총 보너스 상한",300,"운·크리티컬·Finish·처형자·오버킬을 합친 전투 드롭 보너스 상한입니다.");
StealWeight=Config.Bind("훔치기 운","훔치기 중량 제한 운 우회",false,"운에 따라 훔치기 중량 제한을 확률적으로 무시합니다.");
StealDiv=Config.Bind("훔치기 운","중량 우회 운 분모",20,"초과 중량 보정 전 기본 우회 확률은 운/이 값(%)입니다.");
StealCap=Config.Bind("훔치기 운","중량 우회 확률 상한",75,"초과 중량 보정 전 우회 확률 상한입니다.");
WitnessLuck=Config.Bind("범죄/발각 운","범죄 목격 회피 운",true,"훔치기 등 플레이어 범죄를 목격자가 놓칠 확률에 운을 적용합니다.");
WitnessDiv=Config.Bind("범죄/발각 운","목격 회피 운 분모",25,"기본 목격 회피 확률은 운/이 값(%)입니다.");
WitnessCap=Config.Bind("범죄/발각 운","목격 회피 확률 상한",60,"목격 회피 확률 상한입니다.");
LockLuck=Config.Bind("자물쇠 운","자물쇠 따기 운",true,"자물쇠 따기 시 운으로 유효 자물쇠 레벨을 낮춥니다.");
LockDiv=Config.Bind("자물쇠 운","자물쇠 운 분모",20,"유효 자물쇠 레벨 감소량은 운/이 값입니다.");
LockCap=Config.Bind("자물쇠 운","자물쇠 레벨 감소 상한",100,"유효 자물쇠 레벨 감소량의 상한입니다.");
FishLuck=Config.Bind("낚시 운","낚시 품질 운",true,"운에 따라 잡은 물고기의 등급을 1단계 올립니다.");
FishDiv=Config.Bind("낚시 운","낚시 품질 운 분모",25,"물고기 등급 상승 확률은 운/이 값(%)입니다.");
FishCap=Config.Bind("낚시 운","낚시 품질 확률 상한",50,"물고기 등급 상승 확률 상한입니다.");
DismantleLuck=Config.Bind("수확/해체 운","해체 회수 운",false,"해체 시 소수점 재료 회수 확률을 운으로 높입니다.");
DismantleDiv=Config.Bind("수확/해체 운","해체 회수 운 분모",50,"운이 이 수치만큼 오를 때마다 소수점 재료의 유효 회수 확률이 1% 증가합니다.");
DismantleCap=Config.Bind("수확/해체 운","해체 회수 보너스 상한",100,"해체 소수점 재료 회수 보너스 상한입니다.");
ActivityBonus=Config.Bind("SkillAndLuckMatter 대체","활동 보너스 롤 사용",true,"SkillAndLuckMatter 방식의 활동 보너스 롤을 사용합니다.");
BonusMine=Config.Bind("SkillAndLuckMatter 대체","채광 보너스",true,"채광에 활동 보너스 롤을 적용합니다.");
BonusDig=Config.Bind("SkillAndLuckMatter 대체","땅파기 보너스",true,"땅파기에 활동 보너스 롤을 적용합니다.");
BonusHarvest=Config.Bind("SkillAndLuckMatter 대체","수확/채집/벌목 보너스",true,"수확·채집·베기·벌목에 활동 보너스 롤을 적용합니다.");
BonusFish=Config.Bind("SkillAndLuckMatter 대체","낚시 보너스",true,"낚시에 활동 보너스 롤을 적용합니다.");
BonusCraft=Config.Bind("SkillAndLuckMatter 대체","제작/가공 보너스",true,"SkillAndLuckMatter 곡선을 이용해 제작/가공 재료 환급을 적용합니다.");
AutoDisableSALM=Config.Bind("SkillAndLuckMatter 대체","원본 모드 감지 시 자동 비활성",true,"원본 SkillAndLuckMatter가 감지되면 중복 보상을 막기 위해 대체 보너스 롤을 자동으로 끕니다.");
ActSkillWeight=Config.Bind("SkillAndLuckMatter 대체","스킬 가중치",3,"원본 모드의 기본 스킬 가중치입니다.");
ActLuckWeight=Config.Bind("SkillAndLuckMatter 대체","운 가중치",2,"원본 모드의 기본 운 가중치입니다.");
ActSkillMod=Config.Bind("SkillAndLuckMatter 대체","스킬 배율",100,"원본 모드의 기본 배율은 100%입니다.");
ActLuckMod=Config.Bind("SkillAndLuckMatter 대체","운 배율",100,"원본 모드의 기본 배율은 100%입니다.");
NestLuck=Config.Bind("희귀 결과 운","수정란 운 적용",false,"새 둥지에서 수정란이 나올 확률에 운을 적용합니다.");
NestDiv=Config.Bind("희귀 결과 운","수정란 운 분모",50,"수정란 추가 판정 확률은 운/이 값(%)입니다.");
NestCap=Config.Bind("희귀 결과 운","수정란 추가 확률 상한",40,"수정란 추가 판정 확률 상한입니다.");
SeedLuck=Config.Bind("희귀 결과 운","씨앗 회수 운",false,"수동 수확의 씨앗 회수 판정에 운을 적용합니다.");
SeedDiv=Config.Bind("희귀 결과 운","씨앗 회수 운 분모",50,"운이 이 수치만큼 오를 때마다 씨앗 회수 판정 강도가 약 1% 증가합니다.");
SeedCap=Config.Bind("희귀 결과 운","씨앗 회수 보너스 상한",200,"씨앗 회수 판정 강도 보너스 상한입니다.");
TreasureLuck=Config.Bind("희귀 결과 운","보물상자 희귀도 운",false,"보물상자에서 생성되는 장비 희귀도에 운을 적용합니다.");
TreasureDiv=Config.Bind("희귀 결과 운","레전더리 운 분모",50,"운/이 값만큼 보물상자 희귀도 판정값을 낮춥니다.");
TreasureCap=Config.Bind("희귀 결과 운","레전더리 판정 감소 상한",50,"0~99 보물상자 희귀도 판정값의 최대 감소량입니다.");
TreasureMythDiv=Config.Bind("희귀 결과 운","미시컬 운 분모",500,"운이 이 수치만큼 오를 때마다 미시컬 1/N의 N이 1 감소하며 최소 N은 5입니다.");
ScratchLuck=Config.Bind("희귀 결과 운","스크래치 당첨 운",false,"스크래치의 상위 보상 당첨 판정에 운을 적용합니다.");
ScratchDiv=Config.Bind("희귀 결과 운","스크래치 운 분모",50,"운이 이 수치만큼 오를 때마다 스크래치 유효 당첨 확률이 1% 증가합니다.");
ScratchCap=Config.Bind("희귀 결과 운","스크래치 보너스 상한",150,"스크래치 유효 당첨 확률 보너스 상한입니다.");
GachaLuck=Config.Bind("가챠 운","가챠 Best-of 운",true,"캐릭터/아이템 가챠에서 운에 따라 후보를 추가로 뽑고 가장 좋은 결과를 선택합니다.");
GachaDiv=Config.Bind("가챠 운","추가 후보당 운",500,"운이 이 수치만큼 오를 때마다 후보가 1개 추가됩니다.");
GachaCap=Config.Bind("가챠 운","추가 후보 상한",5,"추가 후보 수의 상한입니다.");
CasinoLuck=Config.Bind("카지노 운","카지노 배당 운",true,"카지노 미니게임에서 순이익이 발생하면 운에 따라 추가 배당을 지급합니다.");
CasinoDiv=Config.Bind("카지노 운","카지노 보너스 운 분모",25,"추가 배당 확률은 운/이 값(%)입니다.");
CasinoCap=Config.Bind("카지노 운","카지노 보너스 확률 상한",50,"추가 배당 확률 상한입니다.");
Craft=Config.Bind("호환성","제작 장비에도 적용",false,"제작 장비에도 장비 운 보정을 적용합니다.");
Ammo=Config.Bind("호환성","탄약 포함",false,"탄약에도 장비 운 보정을 적용합니다.");
Log=Config.Bind("진단","디버그 로그",false,"운 보정 발생 내용을 로그에 기록합니다.");
h=new Harmony(G);h.PatchAll();Logger.LogInfo(N+" "+V+" Safe Hotfix loaded. 전역 고빈도 Harmony 패치를 제거한 진단/안정화 빌드입니다.");}
void OnDestroy(){h?.UnpatchSelf();}
internal static int Luck(){int l=EClass.pc==null?1:EClass.pc.Evalue(78);if(l<1)l=1;if(l>9999)l=9999;return l;}
internal static bool Eligible(Thing x){return x.IsEquipmentOrRangedOrAmmo&&(!x.IsAmmo||Ammo.Value)&&(x.bp==null||!x.bp.isCraft||Craft.Value)&&(x.sourceCard==null||x.sourceCard.quality==0)&&!x.HasTag(CTAG.noRandomEnc);}
internal static void Info(string s){if(Log.Value)I.Logger.LogInfo(s);}
internal static bool OriginalSALMLoaded(){foreach(var a in AppDomain.CurrentDomain.GetAssemblies()){string n=a.GetName().Name??"";if(n.IndexOf("SkillAndLuckMatter",StringComparison.OrdinalIgnoreCase)>=0)return true;}return false;}
internal static bool ActivityEnabled(){return ActivityBonus.Value&&(!AutoDisableSALM.Value||!OriginalSALMLoaded());}
internal static double ActivityScore(int skill){
int sw=Math.Max(0,ActSkillWeight.Value),lw=Math.Max(0,ActLuckWeight.Value);int den=sw+lw;if(den<=0)return 0;
double sv=Math.Max(0,skill)*(Math.Max(0,ActSkillMod.Value)/100.0);double lv=Luck()*(Math.Max(0,ActLuckMod.Value)/100.0);
return (sv*sw+lv*lw)/den;
}
internal static double BonusChance(double x){
if(x<=0)return 0;if(x<10)return x/10.0*0.15;if(x<40)return 0.15+(x-10)/30.0*0.35;if(x<100)return 0.50+(x-40)/60.0*0.25;if(x<200)return 0.75+(x-100)/100.0*0.25;return 1.0+(x-200)/100.0;
}
internal static int BonusRolls(int skill,bool crafting=false){
double p=BonusChance(ActivityScore(skill));if(crafting)p/=10.0;int n=(int)Math.Floor(p);double f=p-n;if(f>0&&EClass.rnd(100000)<(int)(f*100000.0))n++;return n;
}
}
static class DeathLootContext{
[ThreadStatic] internal static Card? victim;
[ThreadStatic] internal static Card? origin;
[ThreadStatic] internal static AttackSource attackSource;
[ThreadStatic] internal static bool critical;
[ThreadStatic] internal static int overkillPct;
[ThreadStatic] internal static bool active;
[ThreadStatic] internal static bool geneCreated;
[ThreadStatic] internal static HashSet<string>? createdIds;
[ThreadStatic] internal static List<Thing>? heldBefore;

internal static int AnatomyComposite(Card? killer){
 if(killer==null)return 0;int skill=Math.Max(0,killer.Evalue(290));int luck=Plugin.Luck();
 int sw=Math.Max(0,Plugin.AnatomySkillWeight.Value),lw=Math.Max(0,Plugin.AnatomyLuckWeight.Value),den=sw+lw;
 if(den<=0)return skill;int mix=(skill*sw+luck*lw)/den;return Math.Max(skill,mix);
}
internal static double IncrementalChance(double baseP,int relativeBonusPct){
 if(baseP<=0||baseP>=1||relativeBonusPct<=0)return 0;
 double target=Math.Min(0.95,baseP*(1.0+relativeBonusPct/100.0));
 if(target<=baseP)return 0;return (target-baseP)/(1.0-baseP);
}
internal static bool Roll(double p){if(p<=0)return false;if(p>=1)return true;return EClass.rnd(1000000)<(int)(p*1000000.0);}
internal static Point DropPoint(Card v){Point p=v.GetRootCard().pos;if(p.IsBlocked)p=p.GetNearestPoint()??p;return p;}
internal static void DropThing(Thing t,Card v){
 if(t==null)return;t.isHidden=false;t.isNPCProperty=false;t.SetInt(116);EClass._zone.AddCard(t,DropPoint(v));
}
internal static void TryBonusCreated(string id,int denom,int relativePct,Card v){
 if(denom<=1||relativePct<=0||createdIds==null||createdIds.Contains(id))return;
 double q=IncrementalChance(1.0/denom,relativePct);if(!Roll(q))return;
 Thing t=ThingGen.Create(id,-1,v.LV);DropThing(t,v);createdIds.Add(id);Plugin.Info($"Drop Luck: bonus {id}");
}
internal static int CombatBonus(Card killer){
 int b=Math.Min(Math.Max(0,Plugin.CombatLuckCap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.CombatLuckDiv.Value)));
 if(critical)b+=Math.Max(0,Plugin.CritKillBonus.Value);
 if(attackSource==AttackSource.Finish)b+=Math.Max(0,Plugin.FinishKillBonus.Value);
 b+=Math.Min(100,Math.Max(0,killer.Evalue(1420)*Math.Max(0,Plugin.ExecutionerBonus.Value)));
 b+=Math.Min(Math.Max(0,Plugin.OverkillCap.Value),Math.Max(0,overkillPct));
 return Math.Min(Math.Max(0,Plugin.CombatTotalCap.Value),b);
}
}

[HarmonyPatch(typeof(Card),nameof(Card.Die),new Type[]{typeof(Element),typeof(Card),typeof(AttackSource),typeof(Chara)})]static class DeathLootSignalPatch{
static void Prefix(Card __instance,Card origin,AttackSource attackSource){
 DeathLootContext.victim=__instance;DeathLootContext.origin=origin;DeathLootContext.attackSource=attackSource;
 DeathLootContext.critical=AttackProcess.Current!=null&&AttackProcess.Current.TC==__instance&&AttackProcess.Current.CC==origin&&AttackProcess.Current.crit;
 int mh=Math.Max(1,__instance.MaxHP);DeathLootContext.overkillPct=Math.Min(500,Math.Max(0,-__instance.hp*100/mh));
}
static Exception? Finalizer(Exception? __exception){
 DeathLootContext.victim=null;DeathLootContext.origin=null;DeathLootContext.attackSource=AttackSource.None;DeathLootContext.critical=false;DeathLootContext.overkillPct=0;return __exception;
}
}

[HarmonyPatch(typeof(Chara),nameof(Chara.MakeGene),new Type[]{typeof(Nullable<DNA.Type>)})]static class GeneCreatedTrackerPatch{
static void Postfix(){
 if(DeathLootContext.active)DeathLootContext.geneCreated=true;
}
}

[HarmonyPatch(typeof(Card),nameof(Card.SpawnLoot),new Type[]{typeof(Card)})]static class SpawnLootRebalancedPatch{
static void Prefix(Card __instance,Card origin){
 DeathLootContext.active=true;DeathLootContext.geneCreated=false;
 DeathLootContext.createdIds=new HashSet<string>();DeathLootContext.heldBefore=new List<Thing>();
 foreach(Thing t in __instance.things)if(t!=null)DeathLootContext.heldBefore.Add(t);
}
static void Postfix(Card __instance,Card origin){
 if(!Plugin.Drop.Value||!__instance.isChara)return;
 var made=DeathLootContext.createdIds??new HashSet<string>();int luck=Plugin.Luck();

 if(Plugin.GeneralMaterialLuck.Value){
   int rel=Math.Min(Math.Max(0,Plugin.DropCap.Value),Math.Max(0,luck/Math.Max(1,Plugin.DropDiv.Value)));
   if(__instance.Chara.IsMachine){
     bool scrap=__instance.Chara.HasElement(1248);
     DeathLootContext.TryBonusCreated("memory_chip",200,rel,__instance);
     DeathLootContext.TryBonusCreated(scrap?"scrap":"microchip",20,rel,__instance);
     DeathLootContext.TryBonusCreated(scrap?"bolt":"battery",15,rel,__instance);
   }else{
     if(__instance.Chara.IsAnimal){
       DeathLootContext.TryBonusCreated("fang",15,rel,__instance);
       DeathLootContext.TryBonusCreated("skin",10,rel,__instance);
     }
     DeathLootContext.TryBonusCreated("offal",20,rel,__instance);
     DeathLootContext.TryBonusCreated("heart",20,rel,__instance);
   }
   switch(__instance.id){
     case "golem_wood":DeathLootContext.TryBonusCreated("crystal_earth",30,rel,__instance);break;
     case "golem_fish":case "golem_stone":DeathLootContext.TryBonusCreated("crystal_sun",30,rel,__instance);break;
     case "golem_steel":DeathLootContext.TryBonusCreated("crystal_mana",30,rel,__instance);break;
   }
 }

 if(Plugin.GeneLuck.Value&&!DeathLootContext.geneCreated&&origin!=null&&!__instance.IsPCFaction){
   int comp=DeathLootContext.AnatomyComposite(origin);
   int rel=Math.Min(Math.Max(0,Plugin.GeneBonusCap.Value),Math.Max(0,comp));
   double q=DeathLootContext.IncrementalChance(1.0/200.0,rel);
   if(DeathLootContext.Roll(q)){Thing g=__instance.Chara.MakeGene();DeathLootContext.DropThing(g,__instance);DeathLootContext.geneCreated=true;Plugin.Info($"Gene Luck: anatomy/luck composite {comp}");}
 }

 if(Plugin.UniqueLootLuck.Value&&!__instance.IsPCFaction){
   int rel=Math.Min(Math.Max(0,Plugin.UniqueLootCap.Value),Math.Max(0,luck/Math.Max(1,Plugin.UniqueLootDiv.Value)));
   Action<string> tryEntry=(string entry)=>{
     if(string.IsNullOrEmpty(entry))return;string[] p=entry.Split('/');if(p.Length<2)return;
     if(!int.TryParse(p[1],out int n)||n<=0||n>=1000)return;string id=p[0];if(made.Contains(id))return;
     double q=DeathLootContext.IncrementalChance(n/1000.0,rel);if(!DeathLootContext.Roll(q))return;
     Thing t=ThingGen.Create(id,-1,__instance.LV);DeathLootContext.DropThing(t,__instance);made.Add(id);Plugin.Info($"Unique Loot Luck: {id}");
   };
   if(__instance.sourceCard!=null&&__instance.sourceCard.loot!=null)foreach(string e in __instance.sourceCard.loot)tryEntry(e);
   if(__instance.Chara.race!=null&&__instance.Chara.race.loot!=null)foreach(string e in __instance.Chara.race.loot)tryEntry(e);
 }

 if(Plugin.CombatLootLuck.Value&&origin!=null&&DeathLootContext.heldBefore!=null){
   int rel=DeathLootContext.CombatBonus(origin);
   foreach(Thing item in DeathLootContext.heldBefore){
     if(item==null||item.parent!=__instance||item.isGifted||item.rarity>=Rarity.Artifact||item.HasTag(CTAG.gift)||item.trait is TraitChestMerchant)continue;
     double baseP;
     if(item.trait!=null&&item.trait.DropChance>0f)baseP=Math.Min(0.95,item.trait.DropChance);
     else if(item.IsEquipmentOrRanged)baseP=(item.rarity>=Rarity.Legendary)?0.05:0.01;
     else baseP=0.20;
     double q=DeathLootContext.IncrementalChance(baseP,rel);
     if(DeathLootContext.Roll(q)){DeathLootContext.DropThing(item,__instance);Plugin.Info($"Combat Loot: {item.id}, bonus {rel}%");}
   }
 }
}
static Exception? Finalizer(Exception? __exception){
 DeathLootContext.active=false;DeathLootContext.geneCreated=false;DeathLootContext.createdIds=null;DeathLootContext.heldBefore=null;return __exception;
}
}
[HarmonyPatch(typeof(Point),nameof(Point.TryWitnessCrime),new Type[]{typeof(Chara),typeof(Chara),typeof(int),typeof(Func<Chara,bool>)})]static class WitnessLuckPatch{
static void Prefix(Chara criminal,ref Func<Chara,bool> funcWitness){
if(!Plugin.WitnessLuck.Value||criminal==null||EClass.pc==null||criminal!=EClass.pc)return;
int avoid=Math.Min(Plugin.WitnessCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.WitnessDiv.Value)));if(avoid<=0)return;
var orig=funcWitness;
if(orig==null)funcWitness=(Chara c)=>EClass.rnd(10)==0&&EClass.rnd(100)>=avoid;
else funcWitness=(Chara c)=>orig(c)&&EClass.rnd(100)>=avoid;
}}

[HarmonyPatch(typeof(Trait),nameof(Trait.TryOpenLock),new Type[]{typeof(Chara),typeof(bool)})]static class LockLuckPatch{
public sealed class S{public int lv;public bool changed;}
static void Prefix(Trait __instance,Chara cc,out S __state){
__state=new S();if(!Plugin.LockLuck.Value||cc==null||!cc.IsPC||__instance.owner==null)return;
int cut=Math.Min(Plugin.LockCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.LockDiv.Value)));if(cut<=0)return;
__state.lv=__instance.owner.c_lockLv;int nl=Math.Max(0,__state.lv-cut);if(nl<__state.lv){__instance.owner.c_lockLv=nl;__state.changed=true;}
}
static void Postfix(Trait __instance,LockOpenState __result,S __state){if(__state!=null&&__state.changed&&__result!=LockOpenState.Success)__instance.owner.c_lockLv=__state.lv;}
}

[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]static class FishingLuckPatch{
static void Postfix(Chara c,ref Thing __result){
if(!Plugin.FishLuck.Value||c==null||!c.IsPC||__result==null||__result.category==null||!__result.category.IsChildOf("fish"))return;
int chance=Math.Min(Plugin.FishCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.FishDiv.Value)));if(chance<=0||__result.tier>=3)return;
if(EClass.rnd(100)<chance){int before=__result.tier;__result.SetTier(Math.Min(3,before+1));Plugin.Info($"Luck {Plugin.Luck()}: fishing tier {before}->{__result.tier}");}
}}

[HarmonyPatch(typeof(Map),nameof(Map.TrySmoothPick),new Type[]{typeof(Point),typeof(Thing),typeof(Chara)})]static class ActivityDropBonusPatch{
static void Prefix(Thing t,Chara c){
if(!Plugin.ActivityEnabled()||t==null||c==null||!c.IsPC)return;
var fs=new StackTrace(1,false).GetFrames();if(fs==null)return;int skill=-1;bool enabled=false;
for(int i=0;i<fs.Length&&i<14;i++){string dn=fs[i].GetMethod()?.DeclaringType?.FullName??"";
if(dn.Contains("TaskMine")){skill=c.Evalue(220);enabled=Plugin.BonusMine.Value;break;}
if(dn.Contains("TaskDig")){skill=c.Evalue(230);enabled=Plugin.BonusDig.Value;break;}
if(dn.Contains("TaskChopWood")){skill=c.Evalue(225);enabled=Plugin.BonusHarvest.Value;break;}
if(dn.Contains("TaskHarvest")||dn.Contains("GrowSystem")){skill=Math.Max(c.Evalue(250),c.Evalue(286));enabled=Plugin.BonusHarvest.Value;break;}}
if(!enabled||skill<0)return;int rolls=Plugin.BonusRolls(skill);if(rolls<=0)return;int baseNum=Math.Max(1,t.Num);t.ModNum(baseNum*rolls);Plugin.Info($"Activity bonus x{rolls}: {t.id}");
}}

[HarmonyPatch(typeof(AI_Fish),nameof(AI_Fish.Makefish),new Type[]{typeof(Chara)})]static class ActivityFishingBonusPatch{
static void Postfix(Chara c,ref Thing __result){
if(!Plugin.ActivityEnabled()||!Plugin.BonusFish.Value||c==null||!c.IsPC||__result==null)return;
int rolls=Plugin.BonusRolls(c.Evalue(245));if(rolls<=0)return;int baseNum=Math.Max(1,__result.Num);__result.ModNum(baseNum*rolls);Plugin.Info($"Fishing activity bonus x{rolls}: {__result.id}");
}}

static class CraftRefundCore{
internal static List<Thing>? Prepare(Recipe r,List<Thing> ings,bool model){
if(!Plugin.ActivityEnabled()||!Plugin.BonusCraft.Value||model||r==null||ings==null||EClass.pc==null)return null;
Element e=r.source?.GetReqSkill();int skill=e==null?0:EClass.pc.Evalue(e.id);int rolls=Plugin.BonusRolls(skill,true);if(rolls<=0)return null;
var list=new List<Thing>();foreach(var ing in ings){if(ing==null)continue;int n=Math.Max(1,ing.Num)*rolls;Thing d=ing.Duplicate(n);if(d!=null)list.Add(d);}return list;
}
internal static void Refund(List<Thing>? list){if(list==null||EClass.pc==null)return;foreach(var t in list)EClass.pc.AddCard(t);Plugin.Info($"Crafting material refund: {list.Count} stacks");}
}
[HarmonyPatch(typeof(Recipe),nameof(Recipe.Craft),new Type[]{typeof(BlessedState),typeof(bool),typeof(List<Thing>),typeof(TraitCrafter),typeof(bool)})]static class RecipeCraftRefundPatch{
static void Prefix(Recipe __instance,List<Thing> ings,bool model,out List<Thing>? __state){__state=CraftRefundCore.Prepare(__instance,ings,model);}
static void Postfix(List<Thing>? __state,Thing __result){if(__result!=null)CraftRefundCore.Refund(__state);}
}
[HarmonyPatch(typeof(RecipeCard),nameof(RecipeCard.Craft),new Type[]{typeof(BlessedState),typeof(bool),typeof(List<Thing>),typeof(TraitCrafter),typeof(bool)})]static class RecipeCardCraftRefundPatch{
static void Prefix(RecipeCard __instance,List<Thing> ings,bool model,out List<Thing>? __state){__state=CraftRefundCore.Prepare(__instance,ings,model);}
static void Postfix(List<Thing>? __state,Thing __result){if(__result!=null)CraftRefundCore.Refund(__state);}
}

static class RareOutcomeContext{
[ThreadStatic] internal static int seedDepth;
[ThreadStatic] internal static int seedRndIndex;
[ThreadStatic] internal static int lootDepth;
[ThreadStatic] internal static int dismantleDepth;
[ThreadStatic] internal static int treasureDepth;
[ThreadStatic] internal static int nestDepth;
internal static int PercentBonus(ConfigEntry<int> div,ConfigEntry<int> cap){return Math.Min(Math.Max(0,cap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,div.Value)));}
}

static class GachaLuckCore{
[ThreadStatic] internal static bool rerolling;
internal static int Extra(){return Math.Min(Math.Max(0,Plugin.GachaCap.Value),Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.GachaDiv.Value)));}
internal static long Score(CardRow r){if(r==null)return long.MinValue;long v=0;try{v=((long)r.LV*100000L)-Math.Max(1,r.chance);}catch{}try{var st=r as SourceThing.Row;if(st!=null)v+=(long)st.value*100L;}catch{}return v;}
internal static long Score(Chara c){if(c==null||c.source==null)return long.MinValue;long q=c.source.quality;long lv=c.source.LV;long ch=Math.Max(1,c.source.chance);return q*1000000000L+lv*100000L-ch;}
}

[HarmonyPatch(typeof(SpawnList),nameof(SpawnList.Select),new Type[]{typeof(int),typeof(int)})]static class ItemGachaBestOfPatch{
static void Postfix(SpawnList __instance,ref CardRow __result){
if(!Plugin.GachaLuck.Value||GachaLuckCore.rerolling||__result==null)return;
var fs=new StackTrace(1,false).GetFrames();bool ctx=false;if(fs!=null)for(int i=0;i<fs.Length&&i<12;i++){string dn=fs[i].GetMethod()?.DeclaringType?.FullName??"";if(dn.Contains("TraitGachaBall")){ctx=true;break;}}
if(!ctx)return;int extra=GachaLuckCore.Extra();if(extra<=0)return;
GachaLuckCore.rerolling=true;try{CardRow best=__result;long score=GachaLuckCore.Score(best);for(int i=0;i<extra;i++){CardRow r=__instance.Select();long sc=GachaLuckCore.Score(r);if(sc>score){best=r;score=sc;}}__result=best;}finally{GachaLuckCore.rerolling=false;}
}}

[HarmonyPatch(typeof(LayerGachaResult),nameof(LayerGachaResult.Draw),new Type[]{typeof(string)})]static class CharaGachaBestOfPatch{
static void Postfix(string id,ref Chara __result){
if(!Plugin.GachaLuck.Value||GachaLuckCore.rerolling||__result==null)return;int extra=GachaLuckCore.Extra();if(extra<=0)return;
GachaLuckCore.rerolling=true;try{Chara best=__result;long score=GachaLuckCore.Score(best);for(int i=0;i<extra;i++){Chara c=LayerGachaResult.Draw(id);long sc=GachaLuckCore.Score(c);if(sc>score){best=c;score=sc;}}__result=best;}finally{GachaLuckCore.rerolling=false;}
}}

[HarmonyPatch(typeof(MiniGame),nameof(MiniGame.Deactivate),new Type[]{})]static class CasinoLuckSettlementPatch{
static void Prefix(MiniGame __instance){
if(!Plugin.CasinoLuck.Value||__instance==null||__instance.balance==null||__instance.balance.changeCoin<=0)return;
int chance=Math.Min(Plugin.CasinoCap.Value,Math.Max(0,Plugin.Luck()/Math.Max(1,Plugin.CasinoDiv.Value)));if(chance<=0)return;
if(EClass.rnd(100)<chance){int bonus=Math.Max(1,__instance.balance.changeCoin/2);__instance.balance.changeCoin+=bonus;Plugin.Info($"Casino Luck bonus +{bonus}");}
}}

}
