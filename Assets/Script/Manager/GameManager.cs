using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float gameTime; // 현재 게임 시간
    public int kill; // 잡은 몬스터 수
    public int getGold; // 획득한 골드량
    public int getPotion;
    public int getMagnet;
    public int accumDamage;

    [Header("# Accum Weapon Damage Data")]
    public float accumWeaponDamage;
    public Dictionary<WeaponName, float> accumWeaponDamageDict = new Dictionary<WeaponName, float>();

    void Awake()
    {
        instance = this; // 싱글톤 패턴 구현

        var obj = FindObjectsOfType<GameManager>(); // obj로 GameManager를 전부 찾아와 배열로 변환
        if(obj.Length == 1) // obj가 1개면 씬 전환에도 값이 유지되게 설정
        {
            DontDestroyOnLoad(gameObject);
        }
        else // obj가 1개보다 많으면 현재 obj 파괴
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Reset();
        LoadLobbyScene();
    }

    void Update()
    {
        if(timerrunning)
        {
            gameTime += Time.deltaTime;
        }
    }

    void Reset()
    {
        gameTime = 0;
        kill = 0;
        getGold = 0;
        getPotion = 0;
        getMagnet = 0;
        accumDamage = 0;
        accumWeaponDamage = 0f;
        foreach(WeaponName weapon in System.Enum.GetValues(typeof(WeaponName)))
        {
            accumWeaponDamageDict[weapon] = 0f;
        }
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
    }
}
