using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    #region "Singleton"
    public static LobbyManager instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    [Header("# Main Data")]
    public Text HaveGold;
    public Button LoginBtn;

    [Header("# Reference Data")]
    public StartGamePanel StartGamePanel;
    public UpgradePanel UpgradePanel;
    public SettingPanel SettingPanel;
    public ScorePanel ScorePanel;
    public InventoryPanel InventoryPanel;
    public AdMobManager AdMobManager;
    public GameObject LoadingPanel;

    private void Init() // Awake에서 실행
    {
        StartGamePanel.gameObject.SetActive(false);
        UpgradePanel.gameObject.SetActive(false);
        SettingPanel.gameObject.SetActive(false);
        ScorePanel.gameObject.SetActive(false);
        InventoryPanel.gameObject.SetActive(true);
        InventoryPanel.gameObject.SetActive(false);
    }

    void Start()
    {
        LoadHaveGold();
        if(GameManager.instance.InGameDataManager.isQuit)
        {
            ScorePanel.ActiveScore();
        }
    }

    public void LoadHaveGold()
    {
        HaveGold.text = string.Format("{0:F0}", GameManager.instance.Gold);
    }

    void Update()
    {
        // Test Code - 숫자패드 + 누르면 골드 100원 증가
        if(Input.GetKeyDown(KeyCode.KeypadPlus) && GameManager.instance.IsDeveloperMode)
        {
            GameManager.instance.Gold += 100;
        }

        // Test Code - 숫자패드 - 누르면 골드 10000원 증가
        if(Input.GetKeyDown(KeyCode.KeypadMinus) && GameManager.instance.IsDeveloperMode)
        {
            GameManager.instance.Gold += 10000;
        }

        // Test Code - Space 누르면 GameManager의 UpgradeLevelDict 내용물 출력
        if(Input.GetKeyDown(KeyCode.Space) && GameManager.instance.IsDeveloperMode)
        {
            var test = GameManager.instance.StatusManager.UpgradeLevelDict;

            foreach(var _ in test)
            {
                Debug.Log("Key: " + _.Key  + ", Value: " + _.Value);
            }
        }
    }

    public void ShowLoadingPanel(float duration)
    {
        LoadingPanel.SetActive(true);
        StartCoroutine(ShowWhileDuration(duration));
    }

    IEnumerator ShowWhileDuration(float duration)
    {
        yield return new WaitForSecondsRealtime(duration); // duration 값만큼 기다렸다가

        LoadingPanel.SetActive(false); // 비활성화
    }

    #region "Btn"
    public void OnClickQuit()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void OnClickLoginPlayGames()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        GoogleManager.instance.LoginPlayGames();
    }
    #endregion
}
