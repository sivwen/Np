# Elona Luck for Elin v3.3.1 Safe

이번 버전은 v3.3의 Mod Doctor 경고와 포식/사망 연계 문제를 우선 해결하기 위한 안정화 빌드입니다.

## 핵심 변경
- `Card.SpawnLoot()` 패치를 **완전히 제거**했습니다.
- `Card.Die()`는 여전히 패치하지 않습니다.
- 따라서 v3.3의 시체/유전자/일반 몬스터 소재/몬스터 고유 드롭/전투 마무리 장비 드롭 Luck은 이번 안정화 빌드에서 일시 비활성화됩니다.
- 모든 Harmony 패치를 `PatchAll()`로 한꺼번에 설치하지 않고 기능별로 개별 적용합니다.
- 한 기능이 실제 게임 DLL과 맞지 않아 패치에 실패해도 그 기능만 비활성화되고 모드 전체 `Awake()`는 계속 진행합니다.
- 로그에는 `[Luck] 기능명: 적용` 또는 `[Luck] 기능명: 비활성 (...)` 형태로 남습니다.

## 유지 기능
- 범죄 목격 회피 Luck
- 자물쇠 따기 Luck
- 낚시 품질 Luck
- SkillAndLuckMatter 방식 활동 보너스
- 제작/가공 재료 환급
- 카지노 순이익 보너스
- 훔치기 tooHeavy 우회
- 씨앗 회수 Luck
- 수정란 Luck
- 스크래치 Luck
- 보물상자 장비 희귀도 Luck

## 훔치기
`AI_Steal.Perform()`이 시작될 때 시도용 난수 하나를 새로 뽑습니다. 같은 훔치기 시도에서는 그 값을 유지하고, 다음 시도에서는 다시 뽑습니다. 전역 `ChildrenAndSelfWeight` getter는 패치하지 않고 `AI_Steal` 내부 tooHeavy 비교에 사용되는 getter 호출만 바꿉니다.

## 중요한 안전 원칙
다음 함수에는 패치를 걸지 않습니다.
- `Card.Die`
- `Card.SpawnLoot`
- `Thing.OnCreate`
- `ThingGen.Create`
- `Card.Evalue`
- 전역 `Card.ChildrenAndSelfWeight`
- 전역 `EClass.rnd/rndf`
- `Dice.Roll`

포식/사망이 정상화되는 것이 이번 버전의 최우선 목표입니다. 이 빌드가 안정적인 것을 확인한 뒤 드롭 Luck은 `SpawnLoot`가 아닌 다른 안전 경로가 실제로 가능한지 재검토합니다.

호환 기준: Elin EA 23.338 Patch 2
