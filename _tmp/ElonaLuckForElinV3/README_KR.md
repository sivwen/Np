# Elona Luck for Elin v3.2

v3.1의 안전한 SpawnLoot transpiler 구조를 유지하면서, 훔치기 중량 제한을 전역 getter 패치 없이 복원한 버전입니다.

## 전투 장비/소지품 드롭 용어 정리
v3.1에서 "Finish 처치"라고 표현한 것은 AttackSource.Finish를 의미하지 않습니다.
실제 v3.1/v3.2의 전투 전리품 보정값은 다음입니다.

- Luck
- 실제 마지막 공격의 크리티컬 여부(AttackProcess.Current.crit)
- 처형자 피트(1420)
- 오버킬: 현재 음수 HP / MaxHP 비율

AttackSource.Finish는 Card.Die를 건드리지 않는 v3 원칙 때문에 아직 사용하지 않습니다.

## 훔치기 tooHeavy
실제 Elin 코드는 AI_Steal 내부에서 다음 한 곳에서 중량 제한을 검사합니다.

아이템 중량 > 훔치기(281) × 200 + STR × 100 + 1000

v3.2는 Card.ChildrenAndSelfWeight 전역 getter를 패치하지 않습니다.
대신 AI_Steal의 컴파일러 생성 내부 메서드 중 실제로 ChildrenAndSelfWeight를 호출하는 메서드만 런타임에 찾아,
그 호출 1개를 안전한 보조 함수로 교체합니다.

기본 우회 확률:
- base = min(75%, Luck / 20)
- final = base × (기본 중량 한도 / 실제 중량)

예:
Luck 1000 → base 50%
- 한도의 1.25배: 약 40%
- 한도의 2배: 약 25%
- 한도의 5배: 약 10%

같은 대상/같은 게임 시각에서는 결정값을 고정해 진행 틱마다 재굴림되는 문제를 피합니다.

## 낚시 감사 결과
AI_Fish.Makefish에는 다음 판정이 독립적으로 존재합니다.
- 낚시 실패
- 고대책
- 메달
- 플래티넘/스크래치/카지노코인/가챠코인
- 특수 희귀품
- fish tier
- 대어
- 65_gold 특수 물고기

현재 v3.2는 그중 fish tier만 안전한 Postfix로 Luck 보정합니다.
희귀 보상/실패율까지 한 번에 건드리지는 않습니다. 다음 확장 시 Makefish 내부 개별 판정만 transpiler로 좁게 패치할 수 있습니다.

## 이미 Elin 자체에서 Luck을 쓰는 항목
저주 판정은 ActEffect에서 이미 LUC × 5가 직접 들어갑니다.
따라서 별도 Luck 패치를 넣지 않습니다. 이중 적용을 방지하기 위함입니다.

## v3.1 유지 기능
- SpawnLoot 좁은 transpiler
  - 시체: 해부학+운
  - 유전자
  - 일반 소재
  - 몬스터 고유 드롭
  - 장비/소지품: Luck+크리티컬+처형자+오버킬
- 범죄 목격 회피
- 자물쇠
- 낚시 품질
- SkillAndLuckMatter 활동 보너스
- 제작/가공 재료 환급
- 카지노 순이익 보너스

전역 EClass.rnd/rndf, Card.Die, Card.Evalue, Thing.OnCreate, ThingGen.Create, ChildrenAndSelfWeight에는 패치하지 않습니다.

설정은 한글이며 별도 설정창 없이 Mod Options/Mod Config GUI에서 조절합니다.
호환: Elin EA 23.338 Patch 2
