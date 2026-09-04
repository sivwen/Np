# Elona Luck for Elin 1.5.0

## SkillAndLuckMatter 완전 대체 계층
Steam Workshop SkillAndLuckMatter(3386957797)의 공개 설명에 나온 핵심 기능을 이 모드에 흡수했습니다.

원 모드 공식:
(Activity Score) =
(Skill × SkillModifier × SkillWeight + Luck × LuckModifier × LuckWeight)
÷ (SkillWeight + LuckWeight)

기본값:
- SkillWeight = 3
- LuckWeight = 2
- Skill/Luck modifier = 100%
- 점수 10 → 15%, 40 → 50%, 100 → 75%, 200 → 100%, 300 → 200%
- 200 이상에서는 확정 보너스 롤이 누적되고 나머지는 확률 롤
- 제작/가공은 위 보너스 확률의 1/10로 재료를 환급

적용 활동:
- 채광 (Mining / skill 220)
- 땅파기 (Digging / skill 230)
- 수확·채집·베기 (Gathering/Farming/Lumberjack 계열)
- 낚시 (Fishing / skill 245)
- 제작/가공 (해당 레시피 요구 스킬)

호환:
- 원본 SkillAndLuckMatter 어셈블리가 감지되면 Activity Bonus Roll 계층은 기본 자동 비활성화됩니다.
- 원본 모드를 제거하면 본 모드가 자동으로 대체 기능을 수행합니다.
- AutoDisableWhenOriginalDetected=false로 두 모드를 강제로 중첩할 수도 있지만 권장하지 않습니다.

## 기존 기능과의 차이/중복
SkillAndLuckMatter는 개별 희귀품 확률을 바꾸지 않고 같은 활동의 추가 보상 롤을 제공합니다.
Elona Luck for Elin의 기존 기능은 별도 계층이므로 각각 독립적으로 끌 수 있습니다.

- SpawnLoot 일반 드롭 확률 Luck 보정: 별도
- 장비 품질 Luck/5000: 별도
- 인챈트 개수/등급/수치: 별도
- 훔치기 중량 제한/발각 회피: 별도
- 자물쇠 따기: 별도
- 낚시 tier 상승: 별도
- 해체 소수점 회수: 별도
- SkillAndLuckMatter Activity Bonus Roll: v1.5 신규

## 안전 처리
- 원 모드에서 과거 문제가 있었던 grindstone 장비 복제 방식은 사용하지 않습니다.
- 활동 드롭은 최종 산출물 수량 계층에서 보너스를 적용하며 장비 가공 결과를 직접 복제하지 않습니다.
- 제작은 결과물 복제 대신 입력 재료의 복제본을 환급합니다.
- 스토리/퀘스트 RNG는 건드리지 않습니다.

설치: Elin/Package/ElonaLuckForElin/
설정: BepInEx/config/sivwen.elin.elonaluck.cfg
EA 23.338 Patch 2 기준 빌드.