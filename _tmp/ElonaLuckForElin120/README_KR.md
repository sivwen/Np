# Elona Luck for Elin 2.0.0 Stable

v2.0은 v1.7까지 추가된 Luck 기능을 유지하면서 안정화한 버전입니다.

주요 수정:
- SkillAndLuckMatter 대체 계층의 채광/땅파기/벌목에서 스킬 ID를 값처럼 쓰던 버그 수정
  - 이제 Evalue(220/230/225)의 실제 숙련도를 사용
- SpawnLoot 일반 드롭 Luck 보정에서 전역 StackTrace 감시 제거
  - Card.SpawnLoot 진입/탈출 컨텍스트로 제한
- 해체 Luck 보정에서 전역 StackTrace 감시 제거
  - TaskHarvest.HarvestThing 컨텍스트로 제한
- 보물상자/둥지 Luck에 명시적 컨텍스트 추가
  - 다른 EClass.rnd 호출에 번질 가능성 축소
- 캐릭터 가챠 best-of 후보 교체 시 버려진 임시 캐릭터 객체 정리
- 예외 발생 시에도 ThreadStatic 컨텍스트가 복구되도록 Finalizer 사용
- 신규 설치 기본 인챈트 수치 보정 완화
  - EnchantValueLuckDivisor 2000
  - EnchantValueBonusCapPercent 50

기능:
- Elona식 장비 품질 상승
- 인챈트 개수 / 후보 레벨 / 수치 Luck
- 일반 SpawnLoot 드롭 Luck
- 훔치기 중량 제한 우회 / 범죄 목격 회피
- 자물쇠 따기 Luck
- 낚시 tier 및 SkillAndLuckMatter식 추가 보상
- 해체 소수점 회수
- SkillAndLuckMatter 대체: 채광/땅파기/채집/벌목/낚시/제작
- 둥지 수정란 / 씨앗 회수 / 보물상자 희귀도 / 스크래치
- 아이템/캐릭터 가챠 best-of
- 카지노 순이익 보너스

호환:
- SkillAndLuckMatter가 함께 로드되면 대체 Activity Bonus Roll 계층은 기본 자동 비활성화
- SkillAndLuckMatter 제거 시 본 모드가 해당 기능을 단독 대체
- 각 기능은 config에서 개별 On/Off 가능

주의:
- 기존 v1.x config가 있으면 사용자가 설정한 값을 보존합니다.
- v2.0의 완화된 기본 인챈트 수치를 적용하려면 기존 config에서
  EnchantValueLuckDivisor=2000,
  EnchantValueBonusCapPercent=50
  로 직접 바꾸거나 해당 config를 새로 생성하면 됩니다.

설치:
Elin/Package/ElonaLuckForElin/

설정:
BepInEx/config/sivwen.elin.elonaluck.cfg

EA 23.338 Patch 2 기준 안정화 빌드.
