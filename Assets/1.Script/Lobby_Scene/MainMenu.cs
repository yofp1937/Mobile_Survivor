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
    public GameObject Settings;
    public UpgradePanel Upgrade;
    public GameObject Score;
    public GameObject Inventory;
    public GameObject PanelParent;
    public Text HaveGold;

    [Header("# StartGame Panel")]
    public List<GameObject> SG_character_Slots;
    public Button GameSpeedBtn;
    public GameObject GameSpeedImage;

    [Header("# Inventory Panel")]
    public GameObject inven;

    [Header("# Settings Panel")]
    Slider bgm_Slider;
    Slider sfx_Slider;

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
        Score.SetActive(false);
        Inventory.SetActive(false);
        Upgrade.gameObject.SetActive(false);
        GameSpeedReset();

        killtext = Sc_kill.GetComponent<Text>();
        goldtext = Sc_gold.GetComponent<Text>();
        potiontext = Sc_potion.GetComponent<Text>();
        magnettext = Sc_magnet.GetComponent<Text>();
    }

    void Start()
    {
        VolumeSetting();
        Settings.SetActive(false);
        LoadHaveGold();

        if(GameManager.instance.InGameData.boolScore)
        {
            ActiveScore();
        }
    }

    public void GameSpeedUp()
    {
        GameManager.instance.gameSpeed = 1.5f;
        GameSpeedBtn.interactable = false;
        GameSpeedImage.SetActive(true);
    }

    public void GameSpeedReset()
    {
        GameManager.instance.gameSpeed = 1f;
        GameSpeedBtn.interactable = true;
        GameSpeedImage.SetActive(false);
    }

    void ActiveScore()
    {
        var InGameData = GameManager.instance.InGameData;

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
        killtext.text = string.Format("{0:F0}", InGameData.kill);
        goldtext.text = string.Format("{0:F0}", InGameData.getGold);
        potiontext.text = string.Format("{0:F0}", InGameData.getPotion);
        magnettext.text = string.Format("{0:F0}", InGameData.getMagnet);

        // 무기 이미지, 무기별 데미지 표시
        int index = 0;
        foreach(var dict in InGameData.accumWeaponDamageDict)
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

        InGameData.boolScore = false;
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
        GameManager.instance.DataReset();
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

    public void LoadHaveGold()
    {
        HaveGold.text = string.Format("{0:F0}", GameManager.instance.Gold);
    }

    #region "Btn"

    public void OnClickStartGame()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        StartGame.SetActive(true);
    }

    public void OnClickSettings()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Settings.SetActive(true);
    }
    public void OnClickExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnClickStartGame_Start()
    {
        GameManager.instance.LoadInGameScene();
    }

    public void OnClickStartGame_Exit()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        StartGame.SetActive(false);
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

    public void OnClickInventory()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Inventory.SetActive(true);
    }

    public void OnClickInventory_Exit()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Inventory.SetActive(false);
    }

    #endregion
}
