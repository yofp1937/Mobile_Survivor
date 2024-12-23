다운로드 링크: <http://naver.me/xjgAJbhb> <br>
조작키: WASD, 방향키, ESC <br>
아직 컴퓨터로만 가능합니다. <br><br>
게임방법
 - 30분을 버티면 게임 승리, HP가 0이되면 게임 패배
 - 적을 죽여서 레벨업하여 무기 획득 가능
 - 게임중 골드를 획득하여 캐릭터 강화 가능

# Mobile Survivor
Unity 개인 프로젝트
<br>

## 🖥프로젝트 소개
뱀파이어 서바이버와 비슷한 게임을 만들었습니다.
<br>

## 🕰개발 기간
- 2024.04.25 ~ 2024.12.15 - ver1.0
<br>

## ⚙ 개발 환경
- Unity
<br>

## 📌 주요 기능
### ① 로비 씬
![image](https://github.com/user-attachments/assets/eea3431e-edb3-4c0f-b39c-580432304624)

 - 로비 화면에서 게임 시작과 강화, 볼륨 설정 가능

#### ⓐ 게임 시작

https://github.com/user-attachments/assets/08b05561-585f-4adc-ab7f-e48fd9c1f32e

 - 캐릭터 선택 기능(캐릭터의 모습은 Render Camera 이용)
 - 게임 속도 1.5배 기능

#### ⓑ 강화

https://github.com/user-attachments/assets/a8c31721-1493-46f6-95f2-c8190cdbe9c0

 - 골드를 사용해 능력치 강화
 - 초기화 버튼으로 재분배 가능

#### ⓒ 설정

https://github.com/user-attachments/assets/fce29d74-e6ce-4a45-a39a-b273b31816ea

 - 배경음과 효과음 설정(UserPrefs 이용하여 설정값 저장)

<br>

### ② 인게임 씬

#### ⓐ 게임 화면
![인게임](https://github.com/user-attachments/assets/d63d889f-b8f2-4227-977d-e6f488184509)

 - 적, 드랍아이템, 데미지팝업은 전부 오브젝트풀링으로 관리

#### ⓑ 설정

https://github.com/user-attachments/assets/6a0b57b8-1269-48d5-955d-a0ee348ccee1

 - 배경음과 효과음 설정(UserPrefs 이용하여 설정값 저장)
 - 게임 중도 포기(중도 포기시 골드 획득 불가능)

#### ⓒ 플레이
https://youtu.be/NvTBS_JlCl8
 - 적을 죽이면 경험치 보석이 떨어지고 캐릭터와 경험치보석이 가까워지면 자동으로 캐릭터에게 흡수됨
 - 빨간포션 획득시 체력 회복, 파란포션 획득시 자석 기능 발동
 - 파란포션 획득시 FindGameObjectsWithTag 함수 활용하여 Item 태그의 모든 객체를 플레이어에게 끌어당김

<br>

### ③ 통계 창
<br>

### 추후 업데이트 예정
1. 모바일 지원
2. 캐릭터별 스텟 추가, 무기도 공격력 스텟에따라 데미지 들어가게 변경
3. 스텟 올려주는 장비(인벤토리 기능)
4. 다양한 무기와 악세사리
6. 보스 몬스터
7. 사운드 없는 무기들 사운드 추가
