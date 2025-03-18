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
        Init();
    }
    #endregion

    [Header("# Panel")]
    public LevelUpPanel LevelUpPanel;
    public SettingPanel SettingPanel;
    public GameObject GameResultPanel;
    public GameObject OptionBtn;

    [Header("# ItemCount")]
    public int JewelCount;
    public int DropItemCount;

    [Header("# Player Situation")]
    public bool living = true;
    public bool OnSettings = false;
    public bool OnLevelUp = false;

    [Header("# Reference Data")]
    public Player Player;
    public WeaponManager WeaponManager;
    public PoolManager PoolManager;

    [Header("# Sub Component")]
    public EquipmentManager EquipmentManager;

    void Init() // Awake()에서 실행
    {
        LevelUpPanel.gameObject.SetActive(false);
        SettingPanel.gameObject.SetActive(false);
        GameResultPanel.SetActive(false);
        OptionBtn.SetActive(false);
        Player.JoyStick.gameObject.SetActive(false);
        if(GameManager.instance.IsMobile)
        {
            Player.JoyStick.gameObject.SetActive(true);
            OptionBtn.SetActive(true);
        }
    }

    void Start() // InGame으로 Scene이 전환되면 시작되는곳
    {
        GameManager.instance.TimerStart();
        AudioManager.instance.PlayBgm(Bgm.InGame);

        CreatePlayerCharacter();
    }

    void CreatePlayerCharacter() // Robby Scene에서 선택한 Character를 InGame Scene에 생성하는 함수
    {
        // 1번 인자(오브젝트)를 2번 인자 위치에, 3번 인자로 설정한 회전값으로 생성한다 부모 객체는 4번 인자
        GameObject character = Instantiate(GameManager.instance.SelectCharacter, Player.gameObject.transform.position, Quaternion.identity, Player.gameObject.transform);
        character.name = "character"; // 객체의 hierarchy상 이름을 character로 설정
        Player.Init();
        SetWeapon(GameManager.instance.SelectWeapon);
    }

    void SetWeapon(WeaponData data)
    {
        Transform weaponT = WeaponManager.transform.Find("Weapon"+data.itemId);
        weaponT.gameObject.SetActive(true);
        WeaponBase weapon = weaponT.GetComponent<WeaponBase>();
        weapon.Init(data);
        weapon.level++;
    }

    public void CheckGameResult(bool isClear) // 게임 승리시 사용
    {
        GameManager.instance.TimerStop();
        GameResultPanel.SetActive(true);
        GameResultPanel.transform.Find("ExitBtn").gameObject.SetActive(true);
        Player.JoyStick.gameObject.SetActive(false);

        if(isClear)
        {
            GameResultPanel.transform.Find("GameVictory").gameObject.SetActive(true);
        }
        else
        {   
            GameResultPanel.transform.Find("GameOver").gameObject.SetActive(true);
        }
    }
    
    #region "Btn"
    public void OnClickGameQuit()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        GameManager.instance.LoadLobbyScene();
    }
    #endregion
}
