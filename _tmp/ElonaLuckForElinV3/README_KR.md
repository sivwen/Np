# Elona Luck for Elin v3.3

## v3.3 변경
- 훔치기 tooHeavy: 같은 시각/대상 고정값을 제거하고 **실제 훔치기 시도마다 EClass.rnd(100)** 을 굴립니다.
  - 기본 = min(상한, Luck/분모)
  - 최종 = 기본 × 중량한도/실제중량
  - 따라서 Luck은 성공을 보장하는 중량 증가가 아니라 매 시도 변동하는 '행운의 우회'입니다.
- 씨앗: 원본 TryPopSeed가 실패했을 때만 PC 수동 채집에 Luck 추가 판정. 성공 시 원본 TraitSeed.MakeSeed/TryPick 경로 사용.
- 보물상자: CreateTreasureContent가 끝난 뒤 **장비(IsEquipment)만** Luck 판정. Superior→Legendary, Legendary→Mythical 한 단계 승급. 돈/소모품/스크러버 등 비장비는 제외.
- 스크래치: 원본 Scratch가 꽝(__result == null)일 때만 추가 Luck 판정. 원본 상품 순서(메달→플래티넘→가구→프라모델 상자→음식→카지노 코인)를 유지.
- 수정란: MakeEgg 진입 직전에 Luck 추가 판정이 성공하면 해당 1회에 한해 fertChance=1로 만들어 원본 생성/출산 경로를 그대로 사용.

## 중복/안전 원칙
- 전역 EClass.rnd/rndf 패치 없음
- Card.Die 패치 없음
- ChildrenAndSelfWeight 전역 패치 없음
- 원본 판정이 성공한 경우 씨앗/스크래치/수정란 추가 판정 없음
- 저주 저항은 Elin 원본이 이미 LUC×5를 사용하므로 추가 패치 없음
- 보물은 장비만 후처리하며 Artifact 이상은 올리지 않음

## 기존 v3.2 기능
SpawnLoot 개별 드롭, 범죄 목격, 자물쇠, 낚시 tier, 활동 보너스, 제작 재료 환급, 카지노 보너스 유지.

호환 기준: Elin EA 23.338 Patch 2
