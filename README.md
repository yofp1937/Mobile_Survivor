# 플레이 영상
https://youtu.be/ID2S7tLrfuc

# PlayStore 주소
https://play.google.com/store/apps/details?id=com.yofp.MobileSurvivors

게임방법
 - 30분을 버티면 게임 승리, HP가 0이되면 게임 패배
 - 적을 죽여서 레벨업하여 무기 획득 가능
 - 게임중 골드를 획득하여 캐릭터 강화 가능
 - 게임중 3번 등장하는 보스몬스터를 사냥하여 장비를 획득할수있고, 좋은 장비를 착용해 더 높은 난이도에 도전할수있음

# Mobile Survivor
Unity 개인 프로젝트
<br>

## 🖥프로젝트 소개
뱀파이어 서바이버와 비슷한 게임을 만들었습니다.
<br>

## 🕰개발 기간
- 2024.04.25 ~ 2024.12.15 - ver1.0(테스트 버전)
- 2024.12.16 ~ 2024.03.26 - ver2.0(정식 출시)
<br>

## ⚙ 개발 환경
- Unity
<br>

## 📌 주요 기능
### ① 로비 씬

![로비씬](https://github.com/user-attachments/assets/6b5fce78-c2dc-43ad-aa2f-1ed80a76c46c)

 - 로비 화면에서 게임 시작과 강화, 볼륨 설정, 인벤토리 열기 가능
 
 - 로그인 버튼 클릭시 Google PlayGames로 로그인 가능하며 이때부터 다른 기기로 접속해도 로그인하면 개인 데이터를 불러올수있음
 - 로그인하지않고 플레이하면 GUID를 생성하여 PlayerPrefs에 저장한뒤 로그인할때마다 PlayerPrefs를 확인해서 DB에 접근하는 방식


#### ⓐ 게임 시작

https://github.com/user-attachments/assets/a29c931a-68e1-41e5-9e68-80da23b7781b

 - 캐릭터 선택 기능(캐릭터의 모습은 Render Camera 이용)
 - 캐릭터별 스텟 차별화(캐릭터별 차별 스텟은 옆 패널에서 초록색 글씨로 표시)

https://github.com/user-attachments/assets/74892f32-ce8f-4ab9-a486-97ffd87f5229

 - 강화로인한 스텟 적용

https://github.com/user-attachments/assets/79d79815-2ff9-4a6a-8428-3974bc981fe8

 - 장비 착용으로인한 스텟 적용

![난이도 선택](https://github.com/user-attachments/assets/7664d337-dbdc-4a3b-961f-88d2f90281ea)

 - 난이도 설정 기능
 - 난이도에따라 출현하는 장비의 등급이 다름
 
 - 게임 속도 1.5배 기능(Admob 보상형 광고 시청 후 적용)

#### ⓑ 강화

https://github.com/user-attachments/assets/a8c31721-1493-46f6-95f2-c8190cdbe9c0

 - 골드를 사용해 능력치 강화
 - 초기화 버튼으로 재분배 가능

#### ⓒ 설정

https://github.com/user-attachments/assets/49dc79de-1f06-4271-bdf9-c8570896c4fa

 - 배경음과 효과음 설정(PlayerPrefs 이용하여 설정값 저장)

#### ⓓ 장비

https://github.com/user-attachments/assets/50fb44e3-0aae-43fd-af0a-3e76590b995e

 - 보스를 사냥하여 장비 획득 가능
 - 장비 획득시 등급에따라 랜덤으로 옵션 생성(옵션 갯수와 스텟옵션, 옵션별 스텟 증가량, 최대 강화 횟수는 등급별로 다름)
 - 장비 획득시 15퍼 확률로 저주받은 장비가 될수있고 몬스터의 스폰율을 증가시킴

<br>

### ② 인게임 씬

#### ⓐ 게임 화면

![치명타](https://github.com/user-attachments/assets/7dbecd33-c6b4-4ba5-9825-f663cf71cf72)

 - 적, 드랍아이템, 데미지팝업은 전부 ObjectPooling으로 관리
 - 시간이 지날수록 적의 체력, 데미지, 이동속도, 스폰 주기가 증가함
 - 적을 죽이면 경험치 보석이 떨어지고 캐릭터와 경험치 보석이 가까워지면 자동으로 캐릭터에게 흡수됨
 - 빨간포션 획득시 체력 회복, 파란포션 획득시 자석 기능 발동
 - 파란포션 획득시 Item 태그의 모든 객체를 플레이어에게 끌어당김
 - 치명타 발생시 데미지 글씨가 빨간색으로 변하고 크기가 좀 더 커짐

https://github.com/user-attachments/assets/3d985f41-7684-4630-9b8c-f37cd040cb1d

 - 보스 몬스터는 게임당 총 3번 출현하며 일반 몬스터보다 크기가 크고 체력, 데미지, 이동속도가 높음
 - 보스 몬스터 처치시 장비를 드랍하는데 난이도에따라 장비의 등급이 달라짐

#### ⓑ 설정

![인게임 설정창](https://github.com/user-attachments/assets/a8394333-77f1-48d5-abcd-35ee75adee66)

 - 설정창 진입시 자신의 무기, 장신구, 획득한 장비 정보를 확인 가능
 - 배경음과 효과음 설정(UserPrefs 이용하여 설정값 저장)

https://github.com/user-attachments/assets/849fbe4b-87e7-40ac-b5c5-3c6e7d60f020

 - 게임 중도 포기(중도 포기해도 데이터 저장됨)


<br>

### ③ 통계 창

![스코어 패널](https://github.com/user-attachments/assets/4efe54eb-0f2f-4fd1-8280-c43e37b0a63a)

 - 게임이 끝나면 플레이한 게임의 통계가 표시됨
 - 플레이한 캐릭터와 킬수, 포션, 골드, 자석 획득량 표시
 - 게임 난이도와 클리어 여부 표시
 - 획득한 장비 표시
 - 획득한 무기와 장신구의 레벨과 가한 데미지 표시
<br>

### 추후 업데이트 예정
1. 다양한 무기와 악세사리
2. 사운드 없는 무기들 사운드 추가
3. 프레임 드랍이 발생하는데 수정 예정
