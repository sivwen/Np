# Elona Luck for Elin v3.0.0 Core

v3는 v2.x 코드를 이어붙인 버전이 아니라 처음부터 다시 작성한 안전 기반입니다.

## v3 Core 원칙
다음 핵심/전역 함수에는 Harmony 패치를 걸지 않습니다.
- Card.Die
- Card.SpawnLoot
- Thing.OnCreate
- ThingGen.Create
- Card.Evalue
- Card.ChildrenAndSelfWeight
- EClass.rnd / rndf
- Dice.Roll

따라서 v2.x에서 발생한 HP 0 불사, 슬라임 포식 실패, 로드 후 UI/입력 정지 같은 생명주기 침범 경로를 제거했습니다.

## v3 Core에 우선 포함된 기능
- 범죄 목격 회피 Luck
- 자물쇠 따기 Luck
- 낚시 품질 Luck
- SkillAndLuckMatter 방식의 채광/땅파기/수확/벌목/낚시 활동 보너스
- 제작/가공 재료 환급
- 카지노 순이익 보너스

## 아직 복원하지 않은 기능
- 장비 품질/인챈트
- 몬스터 드롭
- 시체/유전자
- 훔치기 중량 제한
- 보물상자/스크래치/가챠

위 기능은 v3 Core가 실제 플레이에서 안전한 것이 확인된 뒤, 원본 판정식에 직접 삽입하는 좁은 패치로 하나씩 복원합니다.

설정은 전부 한글이며 별도 설정창을 만들지 않습니다.
Mod Options / Mod Config GUI에서 조절합니다.

호환: Elin EA 23.338 Patch 2
