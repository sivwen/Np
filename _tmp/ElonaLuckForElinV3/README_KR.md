# Elona Luck for Elin v3.3

## v3.3 핵심

### 훔치기 중량 제한
- `AI_Steal.Perform()`가 시작될 때 Luck 판정용 난수 1개를 새로 뽑습니다.
- 같은 훔치기 시도 안에서는 그 값을 유지하므로 진행 틱마다 재굴림되어 결국 성공하는 문제가 없습니다.
- 다음 훔치기 시도에서는 다시 새로 굴립니다.
- 전역 `ChildrenAndSelfWeight` getter는 패치하지 않습니다.
- 실제 `AI_Steal` 내부의 tooHeavy 비교에서 사용하는 getter 호출 1개만 교체합니다.

기본 확률:
- base = min(75%, Luck / 20)
- final = base × (중량 한도 / 실제 중량)

따라서 Luck 1000이면 기본 50%이며,
- 한도의 1.25배 무게: 약 40%
- 2배: 약 25%
- 5배: 약 10%

### 씨앗 회수
- `GrowSystem.TryPopSeed(Chara)` 내부의 첫 번째 `EClass.rnd(num)` 호출만 교체합니다.
- 자동농장 경로는 원본 코드에서 그 RNG 이전에 반환되므로 영향이 없습니다.
- 별도 씨앗을 사후 생성하지 않습니다.
- 원래 씨앗 생성/드롭 경로를 그대로 사용합니다.

### 보물상자 장비 희귀도
- `ThingGen.CreateTreasureContent()`의 로컬 `SetRarity()` 메서드만 찾아 패치합니다.
- 그 안의 `rnd(100)`과 `rnd(20)` 분모만 Luck으로 완화합니다.
- 장비 생성 후 rarity를 억지로 다시 쓰지 않습니다.
- 돈/소모품/기타 상자 내용물 RNG에는 영향이 없습니다.
- 보스 보물상자도 원래 `SetRarity()`를 거치는 장비만 동일 규칙을 적용받습니다.

### 스크래치
- `TraitCrafter.Craft()`의 로컬 `Prize()` 메서드만 찾아 패치합니다.
- `Prize(chance, ...)` 내부의 기존 `EClass.rnd(chance)` 분모만 Luck으로 완화합니다.
- 기존 상품 우선순서와 메시지/색상/아이템 생성 흐름을 그대로 유지합니다.
- 꽝 이후 별도 아이템을 사후 생성하지 않습니다.

### 수정란
- `Card.MakeEgg(..., fertChance, ...)`에 들어오는 `fertChance` 값만 완화합니다.
- 플레이어 진영의 알 생성에만 적용합니다.
- 원래 `EClass.rnd(fertChance)==0` 판정을 그대로 사용하므로 매 알 생성마다 원본 RNG가 정상적으로 굴러갑니다.
- NPC/야생 생태 전체의 수정란 비율은 변경하지 않습니다.

## 기존 v3 기능
- 시체: 해부학+Luck
- 유전자: 해부학+Luck
- 일반 몬스터 소재 드롭
- 몬스터 고유 희귀 드롭
- 몬스터 소지 장비/아이템: Luck + 실제 크리티컬 + 처형자 + 오버킬
- 범죄 목격 회피
- 자물쇠 따기
- 낚시 fish tier
- SkillAndLuckMatter 방식 활동 보너스
- 제작/가공 재료 환급
- 카지노 순이익 보너스

## 안전 원칙
다음 전역/생명주기 함수에는 패치를 걸지 않습니다.
- `Card.Die`
- `Thing.OnCreate`
- `ThingGen.Create`
- `Card.Evalue`
- `Card.ChildrenAndSelfWeight`
- `EClass.rnd/rndf`
- `Dice.Roll`

저주 저항은 Elin 원본이 이미 `LUC × 5`를 직접 사용하므로 추가 보정하지 않습니다.

호환 기준: Elin EA 23.338 Patch 2
