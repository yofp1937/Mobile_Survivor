using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    #region "Singleton"
    public static InGameManager instance;
    
    void Awake()
    {
        instance = this;
    }
    #endregion

    [Header("# GameObject")]
    public Player player;
    public GameObject LevelUpPanel;
    public WeaponManager WeaponManager;
    public PoolManager PoolManager;
    public GameObject Settings;
    GameObject SettingsBtn;
    public GameObject VolumeSettings;
    GameObject VolumeSettingsBtn;
    public GameObject ExitPanel;
    GameObject ExitBtn;
    public GameObject GameResultPanel;
    GameObject GameExitBtn;
    public GameObject OptionBtn;

    [Header("# Drop Item")]
    public GameObject[] ExpJewel;

    [Header("# ItemCount")]
    public int JewelCount;
    public int DropItemCount;

    [Header("# Player Situation")]
    public bool living = true;
    public bool OnSettings = false;
    public bool OnLevelUp = false;

    void Start() // InGame으로 Scene이 전환되면 시작되는곳
    {
        GameManager.instance.TimerStart();
        AudioManager.instance.PlayBgm(AudioManager.Bgm.InGame);
        LevelUpPanel.SetActive(false);
        Settings.SetActive(false);
        VolumeSettings.SetActive(false);
        ExitPanel.SetActive(false);
        GameResultPanel.SetActive(false);
        player.joy.gameObject.SetActive(false);
        OptionBtn.SetActive(false);
        if(GameManager.instance.IsMobile)
        {
            player.joy.gameObject.SetActive(true);
            OptionBtn.SetActive(true);
        }

        SettingsBtn = Settings.transform.Find("SettingExitBtn").gameObject;
        VolumeSettingsBtn = Settings.transform.Find("VolumeBtn").gameObject;
        ExitBtn = Settings.transform.Find("InGameExitBtn").gameObject;
        GameExitBtn = GameResultPanel.transform.Find("ExitBtn").gameObject;

        CreatePlayerCharacter();
        AudioManager.instance.InGameInit();
    }

    void CreatePlayerCharacter() // Robby Scene에서 선택한 Character를 InGame Scene에 생성하는 함수
    {
        // 1번 인자(오브젝트)를 2번 인자 위치에, 3번 인자로 설정한 회전값으로 생성한다 부모 객체는 4번 인자
        GameObject character = Instantiate(GameManager.instance.SelectCharacter, player.gameObject.transform.position, Quaternion.identity, player.gameObject.transform);
        character.name = "character"; // 객체의 hierarchy상 이름을 character로 설정
        player.Init();
        SetWeapon(GameManager.instance.SelectWeapon);
    }

    void SetWeapon(ItemData data)
    {
        Transform weaponT = WeaponManager.transform.Find("Weapon"+data.itemId);
        weaponT.gameObject.SetActive(true);
        WeaponBase weapon = weaponT.GetComponent<WeaponBase>();
        weapon.Init(data);
    }

    public void GameOver() // 게임 패배시 사용
    {
        living = false;
        GameManager.instance.TimerStop();
        GameResultPanel.SetActive(true);
        GameResultPanel.transform.Find("GameOver").gameObject.SetActive(true);
        GameExitBtn.SetActive(true);
        if(GameManager.instance.IsMobile)
        {
            player.joy.gameObject.SetActive(false);
        }
    }

    public void GameVictory() // 게임 승리시 사용
    {
        GameManager.instance.TimerStop();
        GameResultPanel.SetActive(true);
        GameResultPanel.transform.Find("GameVictory").gameObject.SetActive(true);
        GameExitBtn.SetActive(true);
        if(GameManager.instance.IsMobile)
        {
            player.joy.gameObject.SetActive(false);
        }
    }

    public void ActiveSettings()
    {
        OnSettings = true;
        Settings.GetComponent<Settings>().SettingSlot();
        Settings.SetActive(true);
        SettingsBtn.SetActive(true);
        VolumeSettingsBtn.SetActive(true);
        ExitBtn.SetActive(true);
        if(GameManager.instance.IsMobile)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
            player.joy.gameObject.SetActive(false);
        }
        GameManager.instance.TimerStop();
    }

    public void HideSettings()
    {
        OnSettings = false;
        Settings.SetActive(false);
        VolumeSettings.SetActive(false);
        ExitPanel.SetActive(false);
        if(GameManager.instance.IsMobile)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
            player.joy.gameObject.SetActive(true);
        }
        GameManager.instance.TimerStart();
    }

    public void ActiveVolumeSettings()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        VolumeSettings.SetActive(true);
        SettingsBtn.SetActive(false);
        VolumeSettingsBtn.SetActive(false);
        ExitBtn.SetActive(false);
    }

    public void HideVolumeSettings()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        VolumeSettings.SetActive(false);
        SettingsBtn.SetActive(true);
        VolumeSettingsBtn.SetActive(true);
        ExitBtn.SetActive(true);
    }

    public void ActiveExitPanel()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        ExitPanel.SetActive(true);
        SettingsBtn.SetActive(false);
    }
    
    public void HideExitPanel()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        ExitPanel.SetActive(false);
        SettingsBtn.SetActive(true);
    }
    
    public void GameResultPanelBtn(bool check)
    {
        SetAccumWeaponData();
        GameManager.instance.InGameData.boolScore = true;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        GameManager.instance.LoadLobbyScene(check);
    }

    // GameOver에서 통계창에서 보여줄 무기의 레벨을 적용하기위해 실행
    public void SetAccumWeaponData()
    {
        for(int i = 0; i < player.weapon.Count ; i++)
        {
            WeaponBase weapon = player.transform.Find("Weapon").Find("Weapon" + player.weapon[i]).GetComponent<WeaponBase>();

            GameManager.instance.InGameData.accumWeaponDamageDict[weapon.weaponname].SetLevel(weapon.level);
        }
    }

    // 모바일에서 톱니바퀴모양 버튼 누르면 작동
    public void PressOptionBtn()
    {
        if(living && !OnLevelUp)
        {
            ActiveSettings();
        }
    }
}
