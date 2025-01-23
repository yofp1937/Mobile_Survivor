using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerData {Hp, AttackPower, AttackSpeed, Cooldown, AttackRange, Duration, Amount, Magnet}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public float gameSpeed = 1f; // 게임 속도
    public float maxGameTime = 30 * 60f; // 최대 게임 시간
    [SerializeField]
    private bool DEVELOPE_MODE = false;
    
    [Header("# Player Data")]
    // 소지골드와 강화 8개 - 1.체력, 2.공격력, 3.공격속도, 4.쿨타임, 5.공격범위, 6.지속시간, 7.투사체 개수, 8.아이템 획득범위
    [SerializeField]
    private int Gold; // 소지 골드
    [SerializeField]
    public List<int> PD_List; // PlayerData의 데이터 개수만큼 초기화
    int maxlevel = 5;

    [Header("# Player Select")]
    public GameObject SelectCharacter;
    public ItemData SelectWeapon;
    public int SelectArtifactId;

    [Header("# Accum Data")]
    public bool boolScore = false;
    public float gameTime; // 현재 게임 시간
    public int kill; // 잡은 몬스터 수
    public int getGold; // 획득한 골드량
    public int getPotion; // 획득한 포션량
    public int getMagnet; // 획득한 자석량
    public int accumDamage; // 입은 데미지

    [Header("# Accum Weapon Damage Data")]
    public float accumWeaponDamage;
    public Dictionary<WeaponName, AccumWeaponData> accumWeaponDamageDict = new Dictionary<WeaponName, AccumWeaponData>();

    private bool timerrunning = false; // InGame Scene로 이동하면 시간을 측정하기위함

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

    void Init()
    {
        Gold = PlayerPrefs.GetInt("Gold");
        PD_List = new List<int>(new int[System.Enum.GetValues(typeof(PlayerData)).Length]);
        LoadPlayerData();
    }

    void Start()
    {
        GameDataReset();
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

    public void GameDataReset()
    {
        SelectCharacter = null;
        SelectWeapon = null;
        gameTime = 0;
        kill = 0;
        getGold = 0;
        getPotion = 0;
        getMagnet = 0;
        accumDamage = 0;
        accumWeaponDamage = 0f;
        accumWeaponDamageDict = new Dictionary<WeaponName, AccumWeaponData>();
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

    // 게임 끝나고 나가기 버튼 누르면 동작
    public void LoadLobbyScene(bool clear)
    {
        AudioManager.instance.PlayBgm(AudioManager.Bgm.Lobby);
        if(clear)
        {
            SetHaveGold(getGold);
        }
        SceneManager.LoadScene("Lobby");
    }

    public void SetHaveGold(int gold)
    {
        Gold = Gold + gold;
        PlayerPrefs.SetInt("Gold", Gold);
        PlayerPrefs.Save();
    }

    public void UseHaveGold(int usegold)
    {
        if(Gold >= usegold)
        {
            Gold = Gold - usegold;
            PlayerPrefs.SetInt("Gold", Gold);
            PlayerPrefs.Save();
        }
    }

    public int GetGold()
    {
        return Gold;
    }

    // 레벨 가져오기
    public int GetPlayerData(PlayerData type)
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
