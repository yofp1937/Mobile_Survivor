using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("# Panel")]
    public GameObject StartGame;
    public GameObject Upgrade;
    public GameObject Settings;
    public GameObject Score;
    public GameObject PanelParent;
    public Text HaveGold;

    [Header("# StartGame Panel")]
    public List<GameObject> SG_character_Slots;
    public Button GameSpeedBtn;
    public GameObject GameSpeedImage;

    [Header("# Upgrade Panel")]
    public List<GameObject> Ug_Slots;
    public Text Ug_Desc;
    int Ug_cost;
    public Text Ug_CostText;
    public GameObject Ug_BuyBtn;
    public GameObject Ug_ResetBtn;
    public Sprite level_image;
    public Sprite empty_image;
    public PlayerData Ug_target;

    [Header("# Inventory Panel")]
    public GameObject inven;

    [Header("# Settings Panel")]
    public Slider bgm_Slider;
    public Slider sfx_Slider;

    [Header("# Score Panel")]
    public GameObject Sc_character_Image;
    public GameObject Sc_kill;
    Text killtext;
    public GameObject Sc_gold;
    Text goldtext;
    public GameObject Sc_potion;
    Text potiontext;
    public GameObject Sc_magnet;
    Text magnettext;
    public List<GameObject> Sc_Weapons;

    void Awake()
    {
        StartGame.SetActive(false);
        Upgrade.SetActive(false);
        Score.SetActive(false);
        Ug_BuyBtn.GetComponent<Button>().interactable = false;

        killtext = Sc_kill.GetComponent<Text>();
        goldtext = Sc_gold.GetComponent<Text>();
        potiontext = Sc_potion.GetComponent<Text>();
        magnettext = Sc_magnet.GetComponent<Text>();
    }

    void Start()
    {
        VolumeSetting();
        Settings.SetActive(false);
        HaveGold.text = string.Format("{0:F0}", GameManager.instance.GetGold());
        SetUpgradeSlots();
        GameSpeedReset();

        if(GameManager.instance.boolScore)
        {
            ActiveScore();
        }
    }

    public void GameSpeedReset()
    {
        GameManager.instance.gameSpeed = 1f;
        GameSpeedBtn.interactable = true;
        GameSpeedImage.SetActive(false);
    }

    public void GameSpeedUp()
    {
        GameManager.instance.gameSpeed = 1.5f;
        GameSpeedBtn.interactable = false;
        GameSpeedImage.SetActive(true);
    }

    public void OnClickStartGame()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        StartGame.SetActive(true);
    }

    public void OnClickUpgrades()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Upgrade.SetActive(true);
    }

    public void OnClickSettings()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Settings.SetActive(true);
    }
    public void OnClickExit()
    {
        // #if라는 전처리기 지시문 사용
        // UnityEditor에서 실행중이면 실행을 취소하고
        // 다른 환경에서 실행중이면 어플리케이션을 끝낸다
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnClickStartGame_Start()
    {
        if(GameManager.instance.SelectCharacter == null)
        {
            return;
        } else {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
            SceneManager.LoadScene("InGame");
        }
    }

    public void OnClickStartGame_Exit()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        StartGame.SetActive(false);
    }

    public void OnClickUpgrade_Exit()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Upgrade.SetActive(false);
        Ug_BuyBtn.GetComponent<Button>().interactable = false;
        Ug_cost = 0;
        Ug_Desc.text = "";
        Ug_CostText.text = "";
    }

    public void OnClickSettings_Exit()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Settings.SetActive(false);
    }

    public void OnClickGameSpeed()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        LobbyManager.instance.admobManager.ShowRewardedAd();
    }

    public void OnClickUpgradeSlots(int num)
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        switch(num)
        {
            case 0: // HP
                Ug_target = PlayerData.Hp;
                Ug_Desc.text = "체력 + 10%";
                break;
            case 1: // AttackPower
                Ug_target = PlayerData.AttackPower;
                Ug_Desc.text = "공격력 + 7%";
                break;
            case 2: // AttackSpeed
                Ug_target = PlayerData.AttackSpeed;
                Ug_Desc.text = "공격속도 + 4%";
                break;
            case 3: // Cooldown
                Ug_target = PlayerData.Cooldown;
                Ug_Desc.text = "쿨타임 + 2%";
                break;
            case 4: // AttackRange
                Ug_target = PlayerData.AttackRange;
                Ug_Desc.text = "공격범위 + 4%";
                break;
            case 5: // Duration
                Ug_target = PlayerData.Duration;
                Ug_Desc.text = "지속시간 + 2%";
                break;
            case 6: // Amount
                Ug_target = PlayerData.Amount;
                Ug_Desc.text = "투사체 개수 + 1";
                break;
            case 7: // Magnet
                Ug_target = PlayerData.Magnet;
                Ug_Desc.text = "아이템 획득범위 + 10%";
                break;
        }
        Ug_BuyBtn.GetComponent<Button>().interactable = true;
        SetCost();
    }

    // 필요한 골드 표시
    public void SetCost()
    {
        int _level = GameManager.instance.GetPlayerData(Ug_target);

        if(_level == 0)
        {
            Ug_cost = 10;
            Ug_CostText.text = Ug_cost.ToString();
        }
        else if(_level == 1)
        {
            Ug_cost = 50;
            Ug_CostText.text = Ug_cost.ToString();
        }
        else if(_level == 2)
        {
            Ug_cost = 100;
            Ug_CostText.text = Ug_cost.ToString();
        }
        else if(_level == 3)
        {
            Ug_cost = 200;
            Ug_CostText.text = Ug_cost.ToString();
        }
        else if(_level == 4)
        {
            Ug_cost = 500;
            Ug_CostText.text = Ug_cost.ToString();
        }
        else if(_level == 5)
        {
            Ug_CostText.text = "";
        }
    }

    public void OnClickResetBtn()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        // 강화에 사용된 골드 반환
        int _ = PlayerPrefs.GetInt("UseUpgradeGold", 0);
        GameManager.instance.SetHaveGold(_);

        // 보유 골드 텍스트 갱신
        HaveGold.text = string.Format("{0:F0}", GameManager.instance.GetGold());
        
        // UseUpgradeGold 초기화
        PlayerPrefs.SetInt("UseUpgradeGold", 0);
        PlayerPrefs.Save();

        // 레벨에따라 슬롯별 체크되는거 전부 체크 해제
        ResetUpgradeSlots();

        // 데이터 리셋
        GameManager.instance.ResetPD();

        // 설명, 필요 골드 텍스트 갱신
        Ug_Desc.text = "";
        Ug_CostText.text = "";
    }

    public void OnClickBuyBtn()
    {
        List<int> _list = GameManager.instance.PD_List;
        if(_list[(int)Ug_target] == 5) // 레벨이 5면 버튼 안눌림
        {
            return;
        }
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);

        // 강화에 사용된 골드 누적
        int _ = PlayerPrefs.GetInt("UseUpgradeGold", 0);
        _ += Ug_cost;
        PlayerPrefs.SetInt("UseUpgradeGold", _);
        PlayerPrefs.Save();

        // 골드 사용
        GameManager.instance.UseHaveGold(Ug_cost);

        // 보유 골드 텍스트 갱신
        HaveGold.text = string.Format("{0:F0}", GameManager.instance.GetGold());

        // 레벨업
        GameManager.instance.LevelUpPlayerData(Ug_target);

        // 필요 골드 텍스트 갱신
        SetCost();

        // 레벨에따라 슬롯별 체크되는거 다음레벨 체크
        LevelUpgradeSlots(Ug_target);
    }

    void ActiveScore()
    {
        // 플레이한 캐릭터 표시
        String charcname = GameManager.instance.SelectCharacter.name;
        GameObject charc = null;
        foreach(GameObject slot in SG_character_Slots)
        {
            Transform _ = slot.transform.Find(charcname);
            if(_ != null && _.gameObject.name == charcname)
            {
                charc = _.gameObject;
                break;
            }
        }
        Sc_character_Image.GetComponent<Image>().sprite = charc.GetComponent<Image>().sprite;

        // 킬수, 획득 골드 표시
        killtext.text = string.Format("{0:F0}", GameManager.instance.kill);
        goldtext.text = string.Format("{0:F0}", GameManager.instance.getGold);
        potiontext.text = string.Format("{0:F0}", GameManager.instance.getPotion);
        magnettext.text = string.Format("{0:F0}", GameManager.instance.getMagnet);

        // 무기 이미지, 무기별 데미지 표시
        int index = 0;
        foreach(var dict in GameManager.instance.accumWeaponDamageDict)
        {
            AccumWeaponData data = dict.Value;
            // 이미지 설정
            Image wimage = Sc_Weapons[index].transform.Find("Image").GetComponent<Image>();
            wimage.sprite = data.Weapon.itemIcon;
            wimage.SetNativeSize();
            wimage.gameObject.SetActive(true);

            // 레벨 텍스트 설정
            Text text = Sc_Weapons[index].transform.Find("LvText").GetComponent<Text>();
            text.text = "Lv." + data.Level;
            text.gameObject.SetActive(true);

            // 데미지 텍스트 설정
            text = Sc_Weapons[index].transform.Find("DamageText").GetComponent<Text>();
            text.text = FormatDamage(data.TotalDamage);
            text.gameObject.SetActive(true);

            index++;
        }

        GameManager.instance.boolScore = false;
        Score.SetActive(true);
    }

    string FormatDamage(float damage)
    {
        if (damage >= 1_000_000_000_000) // 1T 이상
            return $"{damage / 1_000_000_000_000:0.#}T";
        else if (damage >= 1_000_000_000) // 1B 이상
            return $"{damage / 1_000_000_000:0.#}B";
        else if (damage >= 1_000_000) // 1M 이상
            return $"{damage / 1_000_000:0.#}M";
        else if (damage >= 1_000) // 1K 이상
            return $"{damage / 1_000:0.#}K";
        else // 1K 미만
            return $"{damage:0}";
    }

    public void HideScore()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Score.SetActive(false);
        GameManager.instance.GameDataReset();
    }

    void VolumeSetting()
    {
        // Btn 설정
        Settings.transform.Find("Settings_Exit").GetComponent<Button>().onClick.AddListener(OnClickSettings_Exit);
        Settings.transform.Find("Sfx_Group").Find("Sfx_TestBtn").GetComponent<Button>().onClick.AddListener(AudioManager.instance.TestSfx);

        // Slider 설정
        bgm_Slider = Settings.transform.Find("Bgm_Group").Find("Bgm_Slider").GetComponent<Slider>();
        bgm_Slider.value = AudioManager.instance.bgmVolume;
        bgm_Slider.onValueChanged.AddListener(AudioManager.instance.SetBgmVolume);

        sfx_Slider = Settings.transform.Find("Sfx_Group").Find("Sfx_Slider").GetComponent<Slider>();
        sfx_Slider.value = AudioManager.instance.sfxVolume;
        sfx_Slider.onValueChanged.AddListener(AudioManager.instance.SetSfxVolume);
    }

    void SetUpgradeSlots()
    {
        List<int> _list = GameManager.instance.PD_List;

        for(int i = 0; i < _list.Count; i++)
        {
            for(int j = 1; j <= _list[i]; j++)
            {
                Ug_Slots[i].transform.Find("Level_Panel").Find(j.ToString()).GetComponent<Image>().sprite = level_image;
            }
        }
    }

    void ResetUpgradeSlots()
    {
        List<int> _list = GameManager.instance.PD_List;

        for(int i = 0; i < _list.Count; i++)
        {
            if(_list[i] > 0)
            {
                for(int j = 1; j <= _list[i]; j++)
                {
                    Ug_Slots[i].transform.Find("Level_Panel").Find(j.ToString()).GetComponent<Image>().sprite = empty_image;
                }
            }
        }
    }

    void LevelUpgradeSlots(PlayerData data)
    {
        int index = GameManager.instance.PD_List[(int)data];

        Ug_Slots[(int)data].transform.Find("Level_Panel").Find(index.ToString()).GetComponent<Image>().sprite = level_image;
    }
}
