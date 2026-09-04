# Elona Luck for Elin 1.3.0

v1.3 추가:
- 일반 드롭률 Luck 보정: Card.SpawnLoot() 내부 공통 chance(n) 계열에만 적용
- 기본값: Luck 50당 유효 드롭률 +1%, 최대 +100%
- 시체 판정, 수량, 희귀도 선택 등 SpawnLoot의 다른 랜덤 로직은 변경하지 않음
- 훔치기 중량 제한 Luck 우회
- 현재 기본 중량 제한: 훔치기×200 + STR×100 + 1000
- 기본 우회확률: min(75%, Luck/20) × (기본 중량 한도 / 실제 중량)
  예) 기본 성공률이 50%이고 물건이 한도의 2배 무거우면 최종 약 25%
- 실패하면 정상적으로 "tooHeavy" 처리. 성공한 동일 물건은 진행 중 재판정으로 취소되지 않도록 유지

기존 기능:
- 품질 Luck/5000 1단계 상승
- 인챈트 개수/상위 후보 레벨/수치 Luck 보정
- Elin 전역 Luck 주사위 재굴림 비활성화
- 각 기능 config 개별 On/Off 및 상한 조절 가능

설치: Elin/Package/ElonaLuckForElin/
설정: BepInEx/config/sivwen.elin.elonaluck.cfg
EA 23.338 Patch 2 기준.