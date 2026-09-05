# Elona Luck for Elin v3.4

v3.4는 SpawnLoot를 전혀 패치하지 않고 사망이 완전히 처리된 뒤 호출되는 `ZoneEventManager.OnCharaDie(Chara)` 후속 통지를 이용해 보너스 드롭만 추가하는 안전 경로 검증 버전입니다.

## 핵심 원칙
다음은 패치하지 않습니다.
- Card.Die
- Card.SpawnLoot
- Thing.OnCreate
- ThingGen.Create
- Card.Evalue
- 전역 Card.ChildrenAndSelfWeight
- 전역 EClass.rnd / rndf
- Dice.Roll

또 `Harmony.PatchAll()`도 사용하지 않습니다. 각 기능을 개별 `CreateClassProcessor(...).Patch()`로 적용하고 실패하면 해당 기능만 비활성화합니다.

## SpawnLoot 없는 드롭 복원
`Chara.Die()`의 마지막 단계에서 `RefreshDeathSentense()` 이후 `EClass._zone.events.OnCharaDie(this)`가 호출됩니다. v3.4는 `ZoneEventManager.OnCharaDie()`의 Postfix에서만 보너스 드롭을 처리합니다.

### 복원한 항목
- 유전자 보너스
  - 원본 유전자가 같은 위치에 이미 있으면 추가하지 않음
  - 해부학+Luck 가중치 3:2
  - 원본 1/200 확률에 대한 상대 보너스의 '추가분'만 굴림
- 일반 몬스터 소재
  - memory_chip / microchip / scrap / battery / bolt
  - fang / skin / offal / heart
  - 일부 골렘 결정
  - 같은 위치에 원본 드롭이 있으면 추가하지 않음
- 몬스터 고유 드롭
  - sourceCard.loot / race.loot의 0~999 확률 항목만 대상
  - 원본 드롭이 같은 위치에 있으면 추가하지 않음
  - Luck으로 높아지는 확률 중 원본 확률을 제외한 '추가 확률'만 굴림

### 아직 복원하지 않은 항목
- 시체: 원본 시체 생성에는 종족/해부학/도축 조건과 개체 참조가 얽혀 있어 사망 후 재생성이 안전하지 않음
- 몬스터 소지 장비/일반 아이템: 원본 SpawnLoot가 실패한 아이템 상태를 OnCharaDie 시점에서 정확히 구분하기 어려워 보류
- 크리티컬/처형자/오버킬 장비 드롭: 위 이유로 함께 보류

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

## 이번 버전에서 일시 제외
과거 `Map.TrySmoothPick + StackTrace` 기반 SkillAndLuckMatter 활동 보너스와 제작 환급은 고빈도 경로 안정성 때문에 이번 v3.4에서 제외했습니다. 다음 단계에서 채광/수확/벌목/제작 각각의 직접 메서드로 재작성합니다.

호환 기준: Elin EA 23.338 Patch 2
