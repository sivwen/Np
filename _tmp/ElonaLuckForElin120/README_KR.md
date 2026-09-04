# Elona Luck for Elin 1.7.0

v1.7 추가:

## Gacha Luck - best-of
- 캐릭터 가챠
  - 기존 Draw 결과 + Luck 기반 추가 후보를 생성
  - quality → LV → 원본 chance(낮을수록 희귀) 순으로 더 좋은 후보 선택
  - 기본: Luck 500당 추가 후보 1개, 최대 +5
- 아이템 가챠
  - TraitGachaBall에서 SpawnList.Select() 호출 시에만 추가 후보 생성
  - item value와 LV가 높은 후보를 우선
  - 일반 SpawnList 사용에는 영향 없음
- 후보를 여러 번 평가하되 원래 가챠 호출 범위에서만 동작

## Casino Luck
- MiniGame 종료 정산 시 순이익(changeCoin)이 양수일 때만 Luck 보너스 판정
- 기본: Luck/25%, 최대 50% 확률
- 성공 시 해당 세션 순이익의 +50%를 추가 지급
- 손실 세션에는 보정 없음
- Blackjack/Basket 및 MiniGame 공통 정산을 사용하는 플러그인형 미니게임에도 적용 가능
- 게임 내부 카드/슬롯 RNG를 직접 변조하지 않음

기존 v1.6 희귀 결과 Luck과 SkillAndLuckMatter 대체 계층 포함.
모든 기능은 config에서 개별 On/Off 및 수치 조절 가능.

설치: Elin/Package/ElonaLuckForElin/
설정: BepInEx/config/sivwen.elin.elonaluck.cfg
EA 23.338 Patch 2 기준 빌드.