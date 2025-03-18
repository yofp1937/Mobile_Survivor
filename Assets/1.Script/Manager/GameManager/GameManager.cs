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
    public float GameSpeed = 1f; // 게임 속도
    public float MaxGameTime = 30 * 60f; // 최대 게임 시간
    public float GameTime; // 현재 게임 시간
    [SerializeField] DifficultyLevels _difficultyLevel;
    public DifficultyLevels DifficultyLevel
    {
        get => _difficultyLevel;
        set
        {
            _difficultyLevel = value;
            PlayerPrefs.SetString("DifficultyLevel", _difficultyLevel.ToString());
            PlayerPrefs.Save();
        }
    }
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
    public GameObject SelectCharacter;
    public int CharacterCode;
    public WeaponData SelectWeapon;
    public int SelectArtifactId;

    [Header("# Sub Component")]
    public InGameDataManager InGameDataManager;
    public StatusManager StatusManager;
    public InventoryManager InventoryManager;

    private bool timerrunning = false; // InGame Scene로 이동하면 시간을 측정하기위함

    void Init() // Awake()에서 실행
    {
        Application.targetFrameRate = 60;
        gold = PlayerPrefs.GetInt("Gold");
        CheckPlatform();
        LoadDifficulty();
    }

    void Update()
    {
        if(timerrunning)
        {
            GameTime += Time.deltaTime;

            if(GameTime >= MaxGameTime)
            {
                InGameManager.instance.CheckGameResult(true);
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

    void LoadDifficulty()
    {
        string savedDif = PlayerPrefs.GetString("DifficultyLevel", DifficultyLevels.Normal.ToString());
        _difficultyLevel = Enum.TryParse(savedDif, out DifficultyLevels difficulty) ? difficulty : DifficultyLevels.Normal;
    }

    public void LoadInGameScene()
    {
        if(SelectCharacter == null)
        {
            return;
        }
        else
        {
            AudioManager.instance.PlaySfx(Sfx.Click);
            SceneManager.LoadScene("InGame");
        }
    }

    public void TimerStart() // InGame Scene에 입장하면 실행됨
    {
        timerrunning = true;
        Time.timeScale = GameSpeed;
    }

    public void TimerStop() // InGame Scene에서 게임 일시정지할때 실행됨
    {
        timerrunning = false;
        Time.timeScale = 0;
    }

    public void LoadLobbyScene() // InGame Scene에서 게임 종료할때 동작
    {
        AudioManager.instance.PlayBgm(Bgm.Lobby);
        InGameDataManager.SetAccumWeaponData();
        Gold += InGameDataManager.GetGold;
        SceneManager.LoadScene("Lobby");
    }

    public void ResetInGameData() // 게임 종료 후 이전 게임 데이터 Reset
    {
        SelectCharacter = null;
        SelectWeapon = null;
        GameTime = 0;
        InGameDataManager.ResetData();
    }
}
