# Elona Luck for Elin 2.0.4 Safe Hotfix

이번 버전은 로드 후 월드 렌더링/입력/인벤토리(Tab)가 멈추는 원인을 분리하기 위한 안전화 빌드입니다.

## 제거한 전역 고빈도 패치
- EClass.rnd(int) 전역 패치 제거
- Card.ChildrenAndSelfWeight 전역 getter 패치 제거
- Dice.Roll 전역 패치 제거
- Card.Evalue 전역 패치 제거
- Thing.OnCreate 전역 패치 제거
- ThingGen.Create 전역 패치 제거
- EClass.rndf 전역 패치 제거

이전 구현은 위 함수에서 StackTrace를 생성하거나 모든 호출을 후킹했기 때문에,
월드 갱신·UI·인벤토리 중량 계산·랜덤 처리와 섞여 메인 스레드를 막을 가능성이 있었습니다.

## Safe Hotfix에서 기본 비활성화한 기능
- 장비 품질 운
- 인챈트 개수/등급/수치 운
- Elin 전역 운 재굴림 제거
- 시체 해부학+운 직접 보정
- 훔치기 중량 제한 운 우회
- 해체 소수점 회수 운
- 수정란/씨앗/보물상자 희귀도/스크래치 운

위 기능은 설정 항목은 남아 있으나 이번 Safe Hotfix에서는 후킹 자체가 제거되어 적용되지 않습니다.

## 유지되는 기능
- 몬스터 고유 드롭 Luck 후처리
- 유전자 보너스 드롭
- 전투 마무리(크리티컬/Finish/처형자/오버킬)+Luck 소지품/장비 드롭
- 범죄 목격 회피
- 자물쇠 따기
- 낚시 품질
- SkillAndLuckMatter 대체 활동 보너스(채광/땅파기/수확/벌목/낚시/제작)
- 가챠 best-of
- 카지노 순이익 보너스

## 테스트 방법
1. 기존 ElonaLuckForElin 폴더를 완전히 삭제
2. BepInEx/config/sivwen.elin.elonaluck.cfg 삭제
3. v2.0.4 설치
4. 같은 세이브 로드
5. 월드 타일/캐릭터/오브젝트 렌더링 확인
6. Tab 인벤토리와 일반 입력 확인
7. 문제 재현 시 새 LogOutput.log 제공

설정은 한글이며 별도 설정창 없이 Mod Options / Mod Config GUI에서 조절합니다.
호환 버전: Elin EA 23.338 Patch 2
