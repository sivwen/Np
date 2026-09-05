# Elona Luck for Elin v3.7

v3.4에서 포식/HP 0 사망 문제가 정상화된 것을 기준으로, 안전 구조를 유지하면서 SkillAndLuckMatter식 활동 보너스 일부를 직접 산출 패치로 복원한 버전입니다.

## 안전 원칙
다음은 계속 패치하지 않습니다.
- Card.Die
- Card.SpawnLoot
- Thing.OnCreate
- ThingGen.Create
- Card.Evalue
- 전역 Card.ChildrenAndSelfWeight
- 전역 EClass.rnd / rndf
- Dice.Roll
- 전역 Map.TrySmoothPick

또 Harmony.PatchAll()을 사용하지 않습니다. 각 기능은 개별 CreateClassProcessor(...).Patch()로 적용하며 실패하면 해당 기능만 비활성화합니다.

## SpawnLoot 없는 사망 후 보너스 드롭 유지
ZoneEventManager.OnCharaDie() Postfix에서만 추가분을 굴립니다.
- 유전자: 해부학+Luck 3:2, 원본 유전자가 이미 있으면 추가하지 않음
- 일반 소재: 원본 소재가 같은 칸에 있으면 추가하지 않음
- 몬스터 고유 loot: sourceCard.loot / race.loot 중 0~999 확률 항목만 추가분 판정

시체 및 몬스터가 들고 있던 장비/일반 소지품은 원본 실패 상태를 사망 후 정확히 재구성하기 어려워 아직 보류합니다.

## v3.5 직접 활동 보너스
과거처럼 Map.TrySmoothPick 전체를 후킹하고 StackTrace로 호출자를 추적하지 않습니다.
대신 실제 산출 메서드 내부의 TrySmoothPick 호출만 transpiler로 교체합니다.

### 채광
- 대상: Map.MineBlock
- 스킬: 채광 220
- MineBlock 내부에서 실제 산출물이 TrySmoothPick으로 넘어가는 순간만 수량 보정

### 땅파기
- 대상: Map.MineFloor
- 스킬: 땅파기 230
- MineFloor 내부의 실제 회수 산출물만 보정

### 벌목
- 대상: TaskChopWood 내부 완료 콜백 중 TrySmoothPick을 호출하는 메서드만 런타임 검색
- 스킬: 벌목 225
- 판자 산출물만 보정

### 작물 수확
- 대상: GrowSystem.Harvest(Chara)
- 스킬: 수확/농사 관련 250과 286 중 높은 값
- Harvest 내부의 실제 TrySmoothPick 산출물만 보정

### 공식
기본 가중치: 스킬 3 : Luck 2

활동 점수는 (스킬×3 + Luck×2) / 5 로 계산하고 기존 SkillAndLuckMatter의 비선형 곡선에 가깝게 보너스 롤 수를 계산합니다.
- 점수 10 부근: 약 15%
- 점수 40 부근: 약 50%
- 점수 100 부근: 약 75%
- 점수 200 부근: 약 100%
- 이후 100점마다 추가 1롤 증가

추가 롤 수 상한은 기본 5이며 모드 설정에서 변경할 수 있습니다.

## 낚시 감사 결과
AI_Fish.Makefish() 내부에는 실패, 고대책, 메달, 플래티넘/스크래치/카지노/가챠 코인, 특수 희귀품, fish tier, 대어, 65_gold가 서로 다른 RNG와 우선순서로 얽혀 있습니다.
현재 v3.5는 안정성을 위해 기존 fish tier Luck만 유지합니다. 희귀 보상 전체를 한 번에 보정하지 않습니다.

## 기타 유지 기능
- 범죄 목격 회피 Luck
- 자물쇠 따기 Luck
- 낚시 fish tier Luck
- 카지노 순이익 보너스
- 훔치기 tooHeavy 시도별 랜덤 우회
- 씨앗 회수 Luck
- 수정란 Luck
- 스크래치 Luck
- 보물상자 장비 희귀도 Luck

호환 기준: Elin EA 23.338 Patch 2


## 제작/가공 재료 환급
- AI_UseCrafter가 실제로 소비할 수량을 받는 LayerCraft.GetReqIngredient()만 보정합니다.
- UI 필요량과 제작 가능 판정은 원본 그대로입니다.
- Skill:Luck = 3:2 활동 점수와 기존 곡선의 1/10 확률로 재료 단위별 절약 판정을 합니다.
- 기본 환급률 상한 50%, 최소 1개는 항상 소비합니다.
- Recipe.Craft, 재료 전체 스택 복제, 전역 Thing.Split/Destroy는 패치하지 않습니다.
- 낚시 희귀 보상은 이번 안정 배치에서 보류합니다.


## v3.7 제작 환급 로그와 낚시 희귀 보상
- 실제 소비량이 줄었을 때 `Msg.SayRaw`로 게임 플레이 로그에 `손끝에 행운이 스쳤다. 재료 N개를 아꼈다.`를 표시합니다. 설정에서 끌 수 있습니다.
- AI_Fish.Makefish 전체 RNG를 바꾸지 않고, 코드상 식별이 명확한 희귀 보상 첫 관문만 좁게 보정합니다.
- 고대책: 원래 1/30
- 메달: 첫 1/40 관문만 보정. 뒤의 낚시 스킬 조건은 원본 유지
- 코인류: 원래 1/35 관문만 보정. plat/scratch/casino/gacha 내부 선택 1/2, 1/3, 1/3은 원본 유지
- 특수 희귀품: 코인 관문 내부 1/50
- 대어: `num6 >= rnd(100)`의 rnd 범위만 보정하며 거점/지형 보너스 num6는 원본 유지
- 65_gold는 동적 8192/819200 분기와 생성 교체가 얽혀 있어 이번 배치에서는 보류합니다.
- 낚시 실패율과 일반 어종 선택 확률도 건드리지 않습니다.
