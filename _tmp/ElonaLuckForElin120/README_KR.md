# Elona Luck for Elin 2.0.2 UI Hotfix

이 버전은 v2.0.1 설치 후 모드 뷰어/UI가 깨지는 문제를 우선 수정한 핫픽스입니다.

주요 수정:
- package.xml의 <version>을 모드 릴리스 번호로 잘못 사용하던 문제 수정
  - Elin에서 이 값은 게임 호환 버전이므로 0.23.338로 복구
- 모드 뷰어 설정 섹션/항목명/설명을 전부 한글화
- 별도 설정창은 추가하지 않음
- BepInEx/모드 뷰어 설정 화면에서 기존 방식 그대로 조절
- 캐릭터 가챠 best-of 중 UI 흐름에서 임시 Chara.Destroy()를 호출하던 부분 제거
  - 가챠 화면 객체 수명주기와 충돌할 가능성 차단
- v2.0.1의 드롭 재설계 유지
  - 피규어/박제 Luck 미적용
  - 시체/유전자 해부학+운
  - 몬스터 고유 드롭 Luck
  - 장비/소지품은 크리티컬/Finish/처형자/오버킬+운

중요:
기존 v2.0.1에서 생성된 영문 설정 파일이 남아 있다면,
BepInEx/config/sivwen.elin.elonaluck.cfg 를 삭제한 뒤 게임을 한 번 실행하는 것을 권장합니다.
새 한글 설정 파일이 자동 생성됩니다.

설치:
Elin/Package/ElonaLuckForElin/

호환 버전:
Elin EA 23.338 Patch 2
