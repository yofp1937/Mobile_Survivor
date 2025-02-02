using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    GameManager 객체의 메인 스크립트

    주요기능
    1. 게임 내의 중요 기능 초기화
    2. Lobby Scene, InGame Scene의 연결

    ※GameManager의 하위 스크립트들은 스크립트명 앞에 GameManager_를 붙여서 사용
*/

public class GameManager : MonoBehaviour // MonoBehaviour는 게임 사이클 함수(Awake(), Start(), Update() 등을 사용할수있게 해주는 Unity의 기본 상속 클래스)
{
    #region "Singleton Pattern"
        public static GameManager instance;
        void Awake()
        {
            // 싱글톤 패턴 구현
            if (instance == null)
            {
                instance = this;
                Init();
                DontDestroyOnLoad(gameObject); // 씬 전환 시에도 값이 유지되도록 설정
            }
            else
            {
                Destroy(gameObject); // 이미 인스턴스가 존재하면 새로운 객체는 파괴
            }
        }
    #endregion

    [Header("# Game Control")]
    public float gameSpeed = 1f; // 게임 속도
    public float maxGameTime = 30 * 60f; // 최대 게임 시간
    public float gameTime; // 게임 진행 시간
    private bool timerrunning = false; // InGame Scene로 이동하면 시간을 측정하기위함
    [SerializeField]
    private bool DEVELOPE_MODE = false; // 개발자 모드
    
    [Header("# Player Data")] // 로비 강화 탭에서 플레이어 스탯 강화하는 부분(따로 별개의 Script화 진행 예정)
    // 소지골드와 강화 8개 - 1.체력, 2.공격력, 3.공격속도, 4.쿨타임, 5.공격범위, 6.지속시간, 7.투사체 개수, 8.아이템 획득범위
    [SerializeField]
    private int Gold; // 소지 골드
    [SerializeField]
    public List<int> PD_List; // PlayerData의 데이터 개수만큼 초기화
    int maxlevel = 5;

    [Header("# Player Select")] // 게임 시작 패널에서의 플레이어 선택 데이터
    public GameObject SelectCharacter;
    public ItemData SelectWeapon;
    public int SelectArtifactId;

    [Header("# SubScript")]
    public GameManager_InGameData InGameData;

    void Init()
    {
        Gold = PlayerPrefs.GetInt("Gold");
        PD_List = new List<int>(new int[System.Enum.GetValues(typeof(PlayerData)).Length]);
        LoadPlayerData();
    }

    void Start()
    {
        DataReset();
        AudioManager.instance.PlayBgm(AudioManager.Bgm.Lobby);
    }

    void Update()
    {
        if(timerrunning)
        {
            gameTime += Time.deltaTime;

            if(gameTime >= maxGameTime)
            {
                InGameManager.instance.GameVictory();
            }
        }
    }

    public void DataReset()
    {
        SelectCharacter = null;
        SelectWeapon = null;
        gameTime = 0;
        InGameData.DataReset();
    }

    public void TimerStart() // InGame Scene에 입장하면 실행됨
    {
        timerrunning = true;
        Time.timeScale = gameSpeed;
    }

    public void TimerStop() // InGame Scene에서 게임 일시정지할때 실행됨
    {
        timerrunning = false;
        Time.timeScale = 0;
    }

    public void LoadLobbyScene(bool clear) // 게임 끝나고 나가기 버튼 누르면 동작 (InGame Scene 마지막에 동작)
    {
        AudioManager.instance.PlayBgm(AudioManager.Bgm.Lobby);
        if(clear)
        {
            SetHaveGold(InGameData.getGold);
        }
        SceneManager.LoadScene("Lobby");
    }

    public void SetHaveGold(int gold) // 골드 획득 함수(아래거랑 통합 예정)
    {
        Gold = Gold + gold;
        PlayerPrefs.SetInt("Gold", Gold);
        PlayerPrefs.Save();
    }

    public void UseHaveGold(int usegold) // 골드 사용 함수(위에거랑 통합 예정)
    {
        if(Gold >= usegold)
        {
            Gold = Gold - usegold;
            PlayerPrefs.SetInt("Gold", Gold);
            PlayerPrefs.Save();
        }
    }

    public int GetGold() // 소지 골드가 얼마인지 반환
    {
        return Gold;
    }

    public int GetPlayerData(PlayerData type) // 강화탭의 강화 레벨 가져오기
    {
        return PD_List[(int)type];
    }

    // 레벨업
    public void LevelUpPlayerData(PlayerData type)
    {
        int index = (int)type;

        if(PD_List[index] < maxlevel)
        {
            PD_List[index]++;
            PlayerPrefs.SetInt(type.ToString(), PD_List[index]); // PlayerPrefs 적용
            PlayerPrefs.Save(); // PlayerPrefs 저장
        }
    }

    // 리스트 설정
    public void SetPlayerDataList(PlayerData type, int value)
    {
        PD_List[(int)type] = value;
    }

    void LoadPlayerData()
    {
        foreach (PlayerData type in System.Enum.GetValues(typeof(PlayerData))) // PlayerData의 데이터를 전부 가져와 배열로 변환해서 모든 값을 순회
        {
            int value = PlayerPrefs.GetInt(type.ToString());
            SetPlayerDataList(type, value);
        }
    }

    public void ResetPD()
    {
        foreach (PlayerData type in System.Enum.GetValues(typeof(PlayerData))) // PlayerData의 데이터를 전부 가져와 배열로 변환해서 모든 값을 순회
        {
            PlayerPrefs.SetInt(type.ToString(), 0); // 0으로 설정
            PlayerPrefs.Save();
            SetPlayerDataList(type, 0);
        }
    }

    public bool IsDevelopMode()
    {
        return DEVELOPE_MODE;
    }
}
