# Elona Luck for Elin 2.0.3 Load/Render Hotfix

v2.0.2에서 모드가 실제 로드된 뒤 월드 타일/카드 렌더링이 무너질 수 있는 문제를 우선 수정한 핫픽스입니다.

핵심 수정:
- 장비 품질/인챈트 Luck 패치를 더 이상 모든 Thing.OnCreate에 적용하지 않습니다.
- ThingGen.Create()로 실제 신규 Thing을 만드는 중에만 Thing.OnCreate Luck 보정이 활성화됩니다.
- 저장 불러오기, 기존 월드 카드 복원, UI/모드 뷰어가 만든 임시 Thing에는 개입하지 않습니다.
- 기존 장비를 로드하면서 ApplyMaterial(true/false), AddEnchant(), rarity 변경이 실행될 가능성을 차단했습니다.
- package.xml 호환 버전은 0.23.338 유지.
- 설정 섹션/항목/설명은 한글 유지.
- 별도 설정창은 만들지 않으며 기존 Mod Options/Mod Config GUI에서 조절합니다.

권장 테스트:
1. 기존 sivwen.elin.elonaluck.cfg 삭제
2. v2.0.3 폴더로 완전히 교체
3. 게임 실행 후 같은 세이브 로드
4. 월드 타일/캐릭터/오브젝트가 정상 렌더링되는지 확인
5. 새 장비 생성 시에만 Luck 품질/인챈트가 적용되는지 확인

주의:
- 이전 v2.0.2 로그에는 본 모드가 로드되지 않은 상태가 확인됐으므로, 문제 재현 시에는 v2.0.3이 실제 로드된 새 LogOutput.log가 필요합니다.
