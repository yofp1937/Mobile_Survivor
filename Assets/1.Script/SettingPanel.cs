using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [Header("# Main Data")]
    [SerializeField] Scene _scene;

    [Header("# InGame Data")]
    [SerializeField] GameObject[] _weaponSlots;
    [SerializeField] GameObject[] _acceSlots;
    [SerializeField] GameObject[] _equipSlots;
    [SerializeField] GameObject _volumePanel;
    [SerializeField] GameObject _quitPanel;

    [Header("# Reference Data")]
    [SerializeField] Slider _bgmSlider;
    [SerializeField] Slider _sfxSlider;


    void Start()
    {
        SettingVolume();   
        if(_scene == Scene.InGame)
        {
            _volumePanel.SetActive(false);
            _quitPanel.SetActive(false);
            SettingInGameListener();
        }
    }

    void SettingVolume()
    {
        _bgmSlider.value = AudioManager.instance.BgmVolume;
        _sfxSlider.value = AudioManager.instance.SfxVolume;
    }

    void SettingInGameListener() // InGame Scene 패널에 리스너 설정
    {
        _bgmSlider.onValueChanged.AddListener(AudioManager.instance.SetBgmVolume);
        _sfxSlider.onValueChanged.AddListener(AudioManager.instance.SetSfxVolume);
        _volumePanel.transform.Find("SfxGroup").Find("SfxTestBtn").GetComponent<Button>().onClick.AddListener(AudioManager.instance.OnClickTestSfx);
    }
    
    void SettingSlot()
    {
        List<int> weapons = InGameManager.instance.Player.WeaponList;
        List<int> acces = InGameManager.instance.Player.AcceList;
        List<EquipmentData> equips = GameManager.instance.InGameDataManager.GetEquip;

        for(int i = 0; i < weapons.Count; i++)
        {
            Image slotimage = _weaponSlots[i].transform.Find("Image").GetComponent<Image>();
            Text slottext = _weaponSlots[i].transform.Find("Text").GetComponent<Text>();

            WeaponBase weapon = GameObject.Find("Weapon" + weapons[i]).GetComponent<WeaponBase>();
            slotimage.gameObject.SetActive(true);
            slotimage.sprite = weapon.WeaponData.itemIcon;
            slotimage.SetNativeSize();

            slottext.text = "Lv." + weapon.level;
        }

        for(int i = 0; i < acces.Count; i++)
        {
            Image slotimage = _acceSlots[i].transform.Find("Image").GetComponent<Image>();
            Text slottext = _acceSlots[i].transform.Find("Text").GetComponent<Text>();

            WeaponBase acce = GameObject.Find("Acce" + acces[i]).GetComponent<WeaponBase>();
            slotimage.gameObject.SetActive(true);
            slotimage.sprite = acce.WeaponData.itemIcon;
            slotimage.SetNativeSize();

            slottext.text = "Lv." + acce.level;
        }

        for(int i = 0; i < equips.Count; i++)
        {
            Image slotimage = _equipSlots[i].transform.Find("Image").GetComponent<Image>();

            EquipmentData equip = equips[i];
            slotimage.gameObject.SetActive(true);
            slotimage.sprite = equip.Sprite;
            slotimage.SetNativeSize();
        }
    }

    #region "Btn"
    public void OnClickSettingPanel()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        if(_scene == Scene.InGame && InGameManager.instance.living && !InGameManager.instance.OnLevelUp)
        {
            GameManager.instance.TimerStop();
            InGameManager.instance.OnSettings = true;
            SettingSlot();
        }
        gameObject.SetActive(true);
    }
    public void OnClickExit()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        if(_scene == Scene.InGame)
        {
            GameManager.instance.TimerStart();
            InGameManager.instance.OnSettings = false;
        }
        gameObject.SetActive(false);
    }
    public void OnClickVolumePanel()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        _volumePanel.SetActive(true);
    }
    public void OnClickVolumePanelExit()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        _volumePanel.SetActive(false);
    }
    public void OnClickQuitPanel()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        _quitPanel.SetActive(true);
    }
    public void OnClickQuitPanelExit()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        _quitPanel.SetActive(false);
    }
    #endregion
}
