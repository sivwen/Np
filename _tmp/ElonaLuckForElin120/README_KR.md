# Elona Luck for Elin 1.2.0

v1.2는 v1.1의 품질 상승 외에 Luck이 랜덤 인챈트에도 영향을 줍니다.

기본값:
- 품질: Luck / 5000 확률로 1단계 상승
- 인챈트 개수: Luck 250마다 추가 인챈트 시도 확률 +1%, 최대 35%
- 인챈트 등급: 추가 인챈트 생성 레벨에 Luck / 20 보너스, 최대 +150
- 인챈트 수치: 이번 생성에서 새로 붙은 랜덤 인챈트 수치에 Luck / 1000 비율 보너스, 최대 +100%
- 모든 효과는 config에서 개별 On/Off 및 수치 조절 가능
- 일반 몬스터 드롭률은 기본적으로 변경하지 않음
- Elin 기본 전역 Luck 주사위 재굴림은 기본 비활성화

설치: Elin/Package/ElonaLuckForElin/ 에 DLL과 package.xml을 넣습니다.
설정: BepInEx/config/sivwen.elin.elonaluck.cfg
EA 23.338 Patch 2 기준.