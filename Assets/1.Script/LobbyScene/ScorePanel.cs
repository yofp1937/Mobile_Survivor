using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    [Header("# Main Data")]
    [SerializeField] Image _playCharacterImage;
    [SerializeField] Text _killText;
    [SerializeField] Text _goldText;
    [SerializeField] Text _potionText;
    [SerializeField] Text _magnetText;
    [SerializeField] Text _difficultText;
    [SerializeField] Text _clearText;
    [SerializeField] List<GameObject> _weaponSlots;
    [SerializeField] List<GameObject> _acceSlots;
    [SerializeField] List<GameObject> _equipSlots;

    [Header("# Reference Data")]
    InGameDataManager InGameData;
    [SerializeField] List<GameObject> _characterImageSlots;

    public void ActiveScore() // 게임 종료후 통계창 설정
    {
        InGameData = GameManager.instance.InGameDataManager;
        GameManager.instance.Gold += InGameData.GetGold;
        SettingPlayCharacter();
        SettingGetItemText();
        SettingWeaponAndAcceData();
        SettingGetEquip();

        InGameData.isQuit = false;
        gameObject.SetActive(true);
        // InventoryManager.획득아이템가져오기
        GameManager.instance.ResetInGameData();
    }

    void SettingPlayCharacter() // 플레이한 캐릭터 표시
    {
        string characName = GameManager.instance.SelectCharacter.name;
        GameObject character = null;
        foreach(GameObject slot in _characterImageSlots)
        {
            Transform charac = slot.transform.Find(characName);
            if(charac != null)
            {
                character = charac.gameObject;
                break;
            }
        }
        _playCharacterImage.sprite = character.GetComponent<Image>().sprite;
    }
    
    void SettingGetItemText() // 킬수, 획득 골드 표시
    {
        _killText.text = string.Format("{0:F0}", InGameData.Kill);
        _goldText.text = string.Format("{0:F0}", InGameData.GetGold);
        _potionText.text = string.Format("{0:F0}", InGameData.GetPotion);
        _magnetText.text = string.Format("{0:F0}", InGameData.GetMagnet);
        SetDifficultText();
        if(InGameData.isClear)
        {
            _clearText.text = "승리!";
            _clearText.color = new Color32(255, 175, 0, 255);
        }
        else
        {
            _clearText.text = "패배..";
            _clearText.color = new Color32(136, 129, 129, 255);
        }
    }
    void SetDifficultText()
    {
        switch(GameManager.instance.DifficultyLevel)
        {
            case DifficultyLevels.Normal:
                _difficultText.text = "보통";
                _difficultText.color = new Color32(136, 129, 129, 255);
                break;
            case DifficultyLevels.Hard:
                _difficultText.text = "어려움";
                _difficultText.color = new Color32(188, 155, 140, 255);
                break;
            case DifficultyLevels.Hell:
                _difficultText.text = "지옥";
                _difficultText.color = new Color32(168, 24, 36, 255);
                break;
            case DifficultyLevels.God:
                _difficultText.text = "신";
                _difficultText.color = new Color32(255, 183, 0, 255);
                break;
            case DifficultyLevels.Nightmare:
                _difficultText.text = "악몽";
                _difficultText.color = new Color32(76, 0, 255, 255);
                break;
        }
    }
    void SettingWeaponAndAcceData() // 획득 무기 또는 장신구의 정보 표시
    {
        int index = 0;
        foreach(var dict in InGameData.AccumWeponData) // 무기 설정
        {
            AccumWeaponData data = dict.Value;
            // Laser, Thunder는 이미지 크기가 커서 scale 조정
            Vector3 scale = (dict.Key == WeaponEnum.Laser || dict.Key == WeaponEnum.Thunder) ? new Vector3(0.6f, 0.6f, 0.6f) : Vector3.one;
            SettingSlots(data, _weaponSlots[index], scale);
            index++;
        }

        index = 0;
        foreach(var dict in InGameData.AccumAcceData) // 장신구 설정
        {
            AccumWeaponData data = dict.Value;
            SettingSlots(data, _acceSlots[index]);
            index++;
        }
    }

    void SettingSlots(AccumWeaponData data, GameObject slot, Vector3 scale = default) // 각 슬롯의 이미지와 텍스트 설정
    {
        // 이미지 설정
        Image image = slot.transform.Find("Image").GetComponent<Image>();
        image.sprite = data.Data.itemIcon;
        image.SetNativeSize();
        image.gameObject.SetActive(true);

        // 레벨 텍스트 설정
        Text text = slot.transform.Find("LvText").GetComponent<Text>();
        text.text = "Lv." + data.Level;
        text.gameObject.SetActive(true);

        if(scale != default) // scale 입력을 받았으면 Weapon 설정이란 뜻
        {
            image.transform.localScale = scale;

            // 데미지 텍스트 설정
            text = slot.transform.Find("DamageText").GetComponent<Text>();
            text.text = FormatDamage(data.TotalDamage);
            text.gameObject.SetActive(true);
        }
    }

    string FormatDamage(float damage) // 데미지 단위 설정
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

    void SettingGetEquip() // 획득 장비 생성후 표시하고 Inventory로 이동
    {
        for(int i = 0; i < InGameData.GetEquip.Count; i++)
        {
            // 데이터 불러오기
            EquipmentData equipData = InGameData.GetEquip[i];

            // 장비 생성
            GameObject equip = new GameObject(equipData.Grade.ToString() + "_" + equipData.Part.ToString());
            equip.transform.parent = GameManager.instance.InventoryManager.transform;
            equip.AddComponent<Image>().sprite = equipData.Sprite;
            equip.AddComponent<Equipment>().CreateEquip(equipData);

            GameManager.instance.InventoryManager.GetEquipment(equip);

            Image image = _equipSlots[i].transform.Find("Image").GetComponent<Image>();
            image.sprite = equip.GetComponent<Equipment>().Sprite;
            image.gameObject.SetActive(true);
            image.SetNativeSize();
        }
        GameManager.instance.InventoryManager.SortEquipment();
    }

    #region "Btn"
    public void OnClickExit()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        gameObject.SetActive(false);
    }
    #endregion
}
