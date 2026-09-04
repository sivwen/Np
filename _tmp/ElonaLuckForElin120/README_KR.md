# Elona Luck for Elin 1.6.0

v1.6 추가: 희귀 결과(Quality/Rare Outcome) Luck 계층

- 새 둥지 수정란
  - SurvivalManager의 bird nest 결과에서 기본 10% 수정란 판정 실패 시 Luck 기반 추가 재굴림
  - 기본: Luck/50%, 최대 추가 40%

- 씨앗 회수
  - GrowSystem.TryPopSeed의 첫 seed recovery RNG 강도를 Luck으로 보정
  - 기본: Luck 50당 약 +1% 유효 판정 강도, 최대 +200%
  - 자동농장은 기존 고정 처리 유지

- 보물상자 장비 희귀도
  - ThingGen.CreateTreasureContent 내부 로컬 SetRarity에만 Luck 적용
  - Legendary 계열 0~99 rarity roll을 Luck/50만큼 낮춤, 최대 -50
  - Mythical 1/20 판정은 Luck 500마다 분모 -1, 최소 1/5
  - 상자의 화폐, 잡동사니, 스토리/퀘스트 보상 RNG는 변경하지 않음

- 스크래치
  - TraitCrafter Scratch의 Prize() 판정에만 Luck 적용
  - 보상 판정 순서(메달→플래티넘→가구→플라모→음식→카지노 코인)는 유지
  - Luck 50당 유효 당첨 확률 +1%, 최대 +150%

- 가챠/카지노
  - 이번 버전에서는 직접 변경하지 않음.
  - 가챠는 UID/date 기반 seeded RNG와 SpawnList/재질 선택이 묶여 있어 결과 비교 후 best-of 방식이 더 안전함.
  - TraitGamble은 PC 핵심 카지노 보상 판정이 아니므로 억지로 조정하지 않음.

SkillAndLuckMatter 대체 계층(v1.5)과 기존 기능은 모두 유지됩니다.
수량 보너스(Activity Bonus Roll)와 희귀 결과 Luck은 서로 독립 설정입니다.

설치: Elin/Package/ElonaLuckForElin/
설정: BepInEx/config/sivwen.elin.elonaluck.cfg
EA 23.338 Patch 2 기준 빌드.