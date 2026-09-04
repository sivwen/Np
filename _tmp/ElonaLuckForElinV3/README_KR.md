# Elona Luck for Elin v3.1

v3.0 Core의 안전 원칙을 유지하면서 몬스터 드롭/시체/유전자를 다시 복원한 버전입니다.

## 가장 중요한 변경
v2.x와 달리 다음 전역/생명주기 함수에는 패치를 걸지 않습니다.
- Card.Die
- Thing.OnCreate
- ThingGen.Create
- Card.Evalue
- Card.ChildrenAndSelfWeight
- EClass.rnd / rndf
- Dice.Roll

Card.SpawnLoot에도 Prefix/Postfix를 걸지 않습니다.
대신 SpawnLoot의 IL을 한 번 검사한 뒤, 예상한 패턴이 모두 맞을 때만 개별 판정식에 좁은 transpiler를 적용합니다.
패턴이 하나라도 맞지 않으면 부분 적용하지 않고 드롭 확장 전체를 건너뜁니다.

## 시체
SpawnLoot 안에서 해부학(290)을 읽는 지점만 해부학+운 혼합값으로 교체합니다.
기본 가중치:
- 해부학 3
- 운 2
혼합값이 실제 해부학보다 낮으면 원래 해부학 값을 유지합니다.

## 유전자
원래 chance(200) 판정의 분모만 해부학+운으로 완화합니다.
별도 유전자를 사후 생성하지 않습니다.
기본:
- 혼합값 / 2 만큼 상대 드롭 보너스(%)
- 최대 +200%

## 일반 소재
피규어/박제는 건드리지 않습니다.
다음 기존 chance() 판정에만 Luck 보정을 넣습니다.
- memory_chip / scrap / microchip / bolt / battery
- fang / skin / offal / heart
- 골렘 결정

기본:
- Luck 50당 상대 확률 +1%
- 최대 +100%

## 몬스터 고유 드롭
sourceCard.loot / race.loot의 기존
num4 > rnd(1000)
판정 중 첫 희귀 드롭 판정만 보정합니다.

기본:
- Luck 10당 상대 확률 +1%
- 최대 +300%

토끼 꼬리 등 몬스터별 loot 테이블에 등록된 희귀품은 이 계층의 대상입니다.

## 몬스터 소지 장비/아이템
사망 이벤트를 패치하지 않고 SpawnLoot 실행 시점의 상태만 읽습니다.
- Luck
- AttackProcess.Current의 실제 크리티컬
- 처형자(1420)
- 현재 음수 HP 기준 오버킬

원래 DropChance / 1% 장비 / 20% 일반 소지품 판정의 확률만 완화합니다.
Finish 판정은 v3.1에서는 안전한 직접 신호가 없으므로 제외했습니다.

## 기존 v3 Core 기능
- 범죄 목격 회피
- 자물쇠 따기
- 낚시 품질
- SkillAndLuckMatter 방식 활동 보너스
- 제작/가공 재료 환급
- 카지노 순이익 보너스

설정은 전부 한글이며 별도 설정창 없이 Mod Options/Mod Config GUI에서 조절합니다.
호환: Elin EA 23.338 Patch 2
