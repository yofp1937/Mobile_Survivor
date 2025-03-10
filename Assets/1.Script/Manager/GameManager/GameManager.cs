using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region "Singleton"
    public static GameManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Application.targetFrameRate = 60;
            Init();
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 값이 유지되도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("# Game Control")]
    public float gameSpeed = 1f; // 게임 속도
    public float maxGameTime = 30 * 60f; // 최대 게임 시간
    public float gameTime; // 현재 게임 시간
    [SerializeField] private bool DEVELOPER_MODE = false;
    public bool IsDeveloperMode => DEVELOPER_MODE;
    public bool IsMobile { get; private set; }
    
    [Header("# Player Data")]
    [SerializeField] private int gold; // 소지 골드
    public int Gold
    {
        get => gold;
        set
        {
            gold = value;
            PlayerPrefs.SetInt("Gold", gold);
            PlayerPrefs.Save();
        }
    }

    [Header("# Player Select")]
    public GameObject SelectCharacter;
    public int CharacterCode;
    public ItemData SelectWeapon;
    public int SelectArtifactId;

    [Header("# Sub Component")]
    public GameManager_InGameData InGameData;
    public GameManager_Status Status;

    private bool timerrunning = false; // InGame Scene로 이동하면 시간을 측정하기위함

    void Init() // Awake()에서 실행
    {
        gold = PlayerPrefs.GetInt("Gold");
        CheckPlatform();
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

    void CheckPlatform()
    {
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IsMobile = true;
        }
        else
        {
            IsMobile = false;
        }
    }

    public void DataReset() // 게임 종료 후 이전 게임 데이터 Reset
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

    public void LoadInGameScene()
    {
        if(SelectCharacter == null)
        {
            return;
        }
        else
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
            SceneManager.LoadScene("InGame");
        }
    }

    public void LoadLobbyScene(bool clear) // 게임 끝나고 나가기 버튼 누르면 동작
    {
        AudioManager.instance.PlayBgm(AudioManager.Bgm.Lobby);
        if(clear)
        {
            Gold += InGameData.getGold;
        }
        SceneManager.LoadScene("Lobby");
    }
}
