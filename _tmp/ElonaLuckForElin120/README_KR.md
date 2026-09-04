# Elona Luck for Elin 1.4.0

v1.4 추가:
- 범죄/훔치기 발각 회피: PC의 범죄 목격 판정이 성공한 뒤 Luck에 따라 한 번 더 회피
  - 기본: Luck/25%, 최대 60%
- 자물쇠 따기 보조: 시도 중 유효 자물쇠 레벨을 Luck/20만큼 낮춤
  - 최대 100 감소, 성공하지 못하면 원래 자물쇠 레벨로 복원
- 낚시 품질 보정: 정상적으로 잡힌 물고기의 tier를 Luck 확률로 +1
  - 기본: Luck/25%, 최대 50%, tier 3 상한
- 해체 재료 회수 보정: 소수점 재료 회수 판정 확률을 Luck으로 증가
  - 기본: Luck 50당 유효 확률 +1%, 최대 +100%

기존 v1.3 기능:
- 일반 SpawnLoot 드롭률 Luck 보정
- 훔치기 중량 제한 Luck 우회
- 장비 품질 1단계 상승
- 인챈트 개수 / 상위 후보 레벨 / 인챈트 수치 보정
- Elin 전역 Luck 주사위 재굴림 비활성화

주의:
- 퀘스트/스토리성 랜덤 이벤트는 진행 파손 위험 때문에 이번 버전에서는 직접 변경하지 않음.
- 모든 신규 효과는 config에서 개별 On/Off 및 상한 조절 가능.

설치: Elin/Package/ElonaLuckForElin/
설정: BepInEx/config/sivwen.elin.elonaluck.cfg
EA 23.338 Patch 2 기준.