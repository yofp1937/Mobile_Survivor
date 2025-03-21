using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartGamePanel : MonoBehaviour
{
    [Header("# Main Data")]
    [SerializeField] Button _startBtn;
    [SerializeField] Button _speedBtn;
    [SerializeField] GameObject _speedImage;
    [SerializeField] Image _weaponImage;

    [Header("# Character Data")]
    public List<GameObject> Characters; // In Hierarchy
    [SerializeField] List<GameObject> _prefabs; // In Project

    [Header("# Status Data")]
    [SerializeField] StatusData _standardStatus;
    [SerializeField] Status _tempStatus;
    [SerializeField] List<GameObject> _statuses;
    Dictionary<string, FieldInfo> _statusFields = new Dictionary<string, FieldInfo>();

    [Header("# Weapon Data")]
    [SerializeField] List<WeaponData> _weapons;

    [Header("# Difficult Data")]
    [SerializeField] Text _difficultText;
    [SerializeField] Button _difficultBtn;
    [SerializeField] GameObject _difficultPanel;
    [SerializeField] Text _difficultPanelText;
    [SerializeField] Text[] _difficultPanelEquipText;

    [Header("# Check Panel")]
    [SerializeField] GameObject _checkPanel;

    [Header("# Reference Data")]
    StatusManager StatusManager;

    void Awake()
    {
        StatusManager = GameManager.instance.StatusManager;
        _difficultPanel.SetActive(false);
        _checkPanel.SetActive(false);
        ResetGameSpeed();
        SetDifficultText();
        LoadStatusField();
    }

    void Select(int index) // 캐릭터 선택
    {
        foreach(GameObject character in Characters)
        {
            character.SetActive(false);
        }
        Characters[index].SetActive(true);
        GameManager.instance.SelectCharacter = _prefabs[index];
        GameManager.instance.CharacterCode = index;
        _startBtn.interactable = true;
        SetStatusPanel();
    }

    void SelectWeapon(int index) // 무기 선택
    {
        GameManager.instance.SelectWeapon = _weapons[index];
        _weaponImage.sprite = _weapons[index].itemIcon;
        _weaponImage.SetNativeSize();
    }

    public void SetDifficultText() // 난이도별 텍스트 설정
    {
        DifficultyLevels difficult = GameManager.instance.DifficultyLevel;
        _difficultText.text = difficult.ToString();
        string textString = "몬스터 체력, 공격력 ";

        switch(difficult)
        {
            case DifficultyLevels.Normal:
                _difficultText.text = "보통";
                _difficultText.color = new Color32(136, 129, 129, 255);
                _difficultPanelText.text = textString + "변동 없음";
                _difficultPanelText.color = new Color32(136, 129, 129, 255);

                _difficultPanelEquipText[0].text = "흔한";
                _difficultPanelEquipText[0].color = new Color32(139, 245, 255, 255); // 흔한 장비 색
                _difficultPanelEquipText[1].color = new Color32(136, 129, 129, 255);
                _difficultPanelEquipText[2].text = "드문";
                _difficultPanelEquipText[2].color = new Color32(113, 229, 100, 255); // 드문 장비 색
                _difficultPanelEquipText[3].color = new Color32(136, 129, 129, 255);
                break;
            case DifficultyLevels.Hard:
                _difficultText.text = "어려움";
                _difficultText.color = new Color32(188, 155, 140, 255);
                _difficultPanelText.text = textString + "2배 증가";
                _difficultPanelText.color = new Color32(188, 155, 140, 255);
                
                _difficultPanelEquipText[0].text = "흔한";
                _difficultPanelEquipText[0].color = new Color32(139, 245, 255, 255); // 흔한 장비 색
                _difficultPanelEquipText[1].color = new Color32(188, 155, 140, 255);
                _difficultPanelEquipText[2].text = "특별한";
                _difficultPanelEquipText[2].color = new Color32(36, 90, 182, 255); // 특별한 장비 색
                _difficultPanelEquipText[3].color = new Color32(188, 155, 140, 255);
                break;
            case DifficultyLevels.Hell:
                _difficultText.text = "지옥";
                _difficultText.color = new Color32(168, 24, 36, 255);
                _difficultPanelText.text = textString + "4배 증가";
                _difficultPanelText.color = new Color32(168, 24, 36, 255);

                _difficultPanelEquipText[0].text = "드문";
                _difficultPanelEquipText[0].color = new Color32(113, 229, 100, 255); // 드문 장비 색
                _difficultPanelEquipText[1].color = new Color32(168, 24, 36, 255);
                _difficultPanelEquipText[2].text = "희귀한";
                _difficultPanelEquipText[2].color = new Color32(196, 9, 177, 255); // 희귀한 장비 색
                _difficultPanelEquipText[3].color = new Color32(168, 24, 36, 255);
                break;
            case DifficultyLevels.God:
                _difficultText.text = "신";
                _difficultText.color = new Color32(255, 183, 0, 255);
                _difficultPanelText.text = textString + "8배 증가";
                _difficultPanelText.color = new Color32(255, 183, 0, 255);

                _difficultPanelEquipText[0].text = "특별한";
                _difficultPanelEquipText[0].color = new Color32(36, 90, 182, 255); // 특별한 장비 색
                _difficultPanelEquipText[1].color = new Color32(255, 183, 0, 255);
                _difficultPanelEquipText[2].text = "희귀한";
                _difficultPanelEquipText[2].color = new Color32(196, 9, 177, 255); // 희귀한 장비 색
                _difficultPanelEquipText[3].color = new Color32(255, 183, 0, 255);
                break;
            case DifficultyLevels.Nightmare:
                _difficultText.text = "악몽";
                _difficultText.color = new Color32(76, 0, 255, 255);
                _difficultPanelText.text = textString + "16배 증가";
                _difficultPanelText.color = new Color32(76, 0, 255, 255);

                _difficultPanelEquipText[0].text = "희귀한";
                _difficultPanelEquipText[0].color = new Color32(196, 9, 177, 255); // 희귀한 장비 색
                _difficultPanelEquipText[1].color = new Color32(76, 0, 255, 255);
                _difficultPanelEquipText[2].text = "전설적인";
                _difficultPanelEquipText[2].color = new Color32(255, 250, 21, 255); // 전설적인 장비 색
                _difficultPanelEquipText[3].color = new Color32(76, 0, 255, 255);
                break;
        }
    }

    public void SetGameSpeedUp() // 광고 시청후 게임속도 증가
    {
        GameManager.instance.GameSpeed = 1.5f;
        _speedBtn.interactable = false;
        _speedImage.SetActive(true);
    }

    public void ResetGameSpeed()
    {
        GameManager.instance.GameSpeed = 1f;
        _speedBtn.interactable = true;
        _speedImage.SetActive(false);
    }

    public void SetStatusPanel()
    {
        // 캐릭터, 강화, 장비 스탯 적용
        ApplyStatusToPanel(StatusManager.StatusDataList[GameManager.instance.CharacterCode].Stat, "Value");
        SetCharacterStatusColor();
        ApplyStatusToPanel(StatusManager.GetUpgradeStatus(), "Upgrade");
        ApplyStatusToPanel(StatusManager.EquipStatus, "Equip");
    }

    void ApplyStatusToPanel(Status sourceStatus, string targetTextName) // 입력받은 Status 종류 Panel에 표시
    {
        _tempStatus = new Status();
        _tempStatus.CloneStatus(sourceStatus);
        
        foreach(GameObject group in _statuses)
        {
            string optionName = group.name;
            Text text = group.transform.Find("Option").Find(targetTextName).GetComponent<Text>();
            
            if (_statusFields.TryGetValue(optionName, out FieldInfo field))
            {
                float value = Convert.ToSingle(field.GetValue(_tempStatus));
                if(value == 0 || (targetTextName == "Value" && value == 1 && optionName != "ProjectileCount")) // 값이 없으면 패스
                {
                    text.text = "";
                    continue;
                }
                
                bool isPercent = optionName switch // * 100 && 뒤에 % 붙여야하는 옵션들
                {
                    "ProjectileSpeed" or "ProjectileSize" or "Duration" or "CoolTime" or "AttackRange" or "ObtainRange" or "Curse" => true,
                    _ => false
                };
                
                bool isCriticalOrDefense = optionName switch
                {
                    "Defense" or "CriticalChance" or "CriticalDamage" => true,
                    _ => false
                };
                
                string prefix = targetTextName == "Value" ? "" : "+";
                string suffix = isPercent ? "%" : "";
                string suffix2 = isCriticalOrDefense ? "%" : "";
                float displayValue = isPercent ? value * 100 : value;
                if(optionName == "MoveSpeed") displayValue = value * 100;
                else if(targetTextName == "Value" && isPercent)
                {
                    displayValue -= 100;
                    prefix = "+";
                }
                else if(optionName == "ProjectileCount" && targetTextName == "Value") prefix = "+";
                if (optionName == "CoolTime" && (targetTextName == "Upgrade" || targetTextName == "Value")) prefix = "";
                else if(optionName == "CoolTime" && targetTextName == "Equip") prefix = "-";

                text.text = $"{prefix}{displayValue:0}{suffix}{suffix2}";
            }
            else
            {
                text.text = "";
            }
        }
    }

    void LoadStatusField() // Status 클래스의 구조를 가져옴(무슨 변수가 있는지 읽어옴)
    {
        foreach(var field in typeof(Status).GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            _statusFields[field.Name] = field;
        }
    }

    void SetCharacterStatusColor() // 캐릭터별 증가 스테이터스 색상 표시
    {
        ResetCharacterStatusColor();
        foreach(GameObject group in _statuses)
        {
            string optionName = group.name;
            Text text = group.transform.Find("Option").Find("Value").GetComponent<Text>();
            
            if (_statusFields.TryGetValue(optionName, out FieldInfo field))
            {
                float standardValue = Convert.ToSingle(field.GetValue(_standardStatus.Stat));
                float value = Convert.ToSingle(field.GetValue(_tempStatus));

                if(value != standardValue)
                {
                    text.color = new Color32(0, 198, 12, 255);
                }
            }
        }
    }

    void ResetCharacterStatusColor() // 캐릭터 스테이터스 색깔 초기화
    {
        foreach(GameObject group in _statuses)
        {
            Text text = group.transform.Find("Option").Find("Value").GetComponent<Text>();
            text.color = new Color32(174, 174, 174, 255);
        }
    }

    #region "Btn"
    public void OnClickStartGamePanel()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        gameObject.SetActive(true);
    }
    public void OnClickExit()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        gameObject.SetActive(false);
    }
    public void OnClickSelectCharacter(int index)
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        int[] weaponList = { 0, 3, 5, 4, 6, 1 };
        Select(index);
        SelectWeapon(weaponList[index]);
    }
    public void OnClickGameSpeed()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        LobbyManager.instance.AdMobManager.ShowRewardedAd();
    }
    public void OnClickGameStart()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        if(GameManager.instance.InventoryManager.Equips.Count + 3 > GameManager.instance.InventoryManager.SlotCnt)
        {
            _checkPanel.SetActive(true);
            return;
        }
        StartGame();
    }
    public void OnClickDifficultPanel()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        _difficultPanel.SetActive(true);
    }
    public void OnClickDifficultSet(int index)
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        DifficultyLevels[] levels = (DifficultyLevels[])Enum.GetValues(typeof(DifficultyLevels));
        GameManager.instance.DifficultyLevel = levels[index];
        SetDifficultText();
    }
    public void OnClickDifficultSelect()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        _difficultPanel.SetActive(false);
    }
    public void OnClickCheckPanelStart()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        StartGame();
    }
    public void OnClickCheckPanelInven()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        _checkPanel.SetActive(false);
        LobbyManager.instance.InventoryPanel.gameObject.SetActive(true);
    }
    void StartGame()
    {
        GameManager.instance.LoadInGameScene();
    }
    #endregion
}
