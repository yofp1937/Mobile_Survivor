using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public float gameSpeed = 1f; // 게임 속도
    public float maxGameTime = 30 * 60f; // 최대 게임 시간

    [Header("# Player Select")]
    public GameObject SelectCharacter;
    public ItemData SelectWeapon;
    public int SelectArtifactId;

    private bool timerrunning = false; // InGame Scene로 이동하면 시간을 측정하기위함

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

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 값이 유지되도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 새로운 객체는 파괴
        }
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

    public void LoadLobbyScene()
    {
        AudioManager.instance.PlayBgm(AudioManager.Bgm.Lobby);
        SceneManager.LoadScene("Lobby");
    }
}
