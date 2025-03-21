using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelUpPanel : MonoBehaviour
{
    LevelUpPanelItem[] _itemLists;

    ItemSituation mysitu;
    int weaponcount;
    int selectcount;

    public enum ItemSituation
    {
        Full = 0, // 무기, 장신구 가득참
        OnlyWeapon = 1, // 무기만 가득참
        OnlyAcce = 2, // 장신구만 가득참
        Available = 3 // 둘다 자리 있음
    }

    void Awake()
    {
        _itemLists = GetComponentsInChildren<LevelUpPanelItem>();
    }

    void OnEnable()
    {
        BaseSetting();
        LoadLevelUpPanel();
    }

    void BaseSetting()
    {
        GetMyEquipSituation(CheckWeapon(), CheckAcce());
        GetSelectItemCount();
    }

    public void HideLevelUpPanel()
    {
        gameObject.SetActive(false);
        if(GameManager.instance.IsMobile)
        {
            InGameManager.instance.Player.JoyStick.gameObject.SetActive(true);
        }
        GameManager.instance.TimerStart();
    }

    void GetSelectItemCount()
    {
        // 아이템을 가져올 개수를 구해주는 함수
        if(mysitu == ItemSituation.Full && InGameManager.instance.Player.MaxLevelCount > weaponcount - 1)
        {
            selectcount = 0;
            _itemLists[0].gameObject.SetActive(false);
        }
        else if(mysitu == ItemSituation.Full && InGameManager.instance.Player.MaxLevelCount > weaponcount - 2)
        {
            selectcount = 1;
            _itemLists[1].gameObject.SetActive(false);
        }
        else if(mysitu == ItemSituation.Full && InGameManager.instance.Player.MaxLevelCount > weaponcount - 3)
        {
            selectcount = 2;
            _itemLists[2].gameObject.SetActive(false);
        }
        else
        {
            selectcount = 3;
        }       
    }

    void LoadLevelUpPanel()
    {
        InGameManager.instance.OnLevelUp = true;
        
        if(selectcount == 0)
        {
            _itemLists[0].MaxLevelSetting(0);
            _itemLists[0].gameObject.SetActive(true);
            _itemLists[1].MaxLevelSetting(1);
            _itemLists[1].gameObject.SetActive(true);
        }
        else
        {
            // 3개의 랜덤 아이템 선택
            WeaponData[] randomItems = RandomItemSelect(selectcount);
            
            for(int i = 0; i < randomItems.Length; i++)
            {
                _itemLists[i].SetItemData(randomItems[i]);
            }
        }
    }

    void GetMyEquipSituation(bool myweapon, bool myacce)
    {
        if(myweapon && myacce)
        {
            mysitu = ItemSituation.Full;
            weaponcount = InGameManager.instance.Player.WeaponList.Count + InGameManager.instance.Player.AcceList.Count;
        }
        else if(myweapon)
        {
            mysitu = ItemSituation.OnlyWeapon;
        }
        else if(myacce)
        {
            mysitu = ItemSituation.OnlyAcce;
        }
        else
        {
            mysitu = ItemSituation.Available;
        }
    }

    bool CheckWeapon()
    {
        return InGameManager.instance.Player.WeaponList.Count > 5;
    }

    bool CheckAcce()
    {
        return InGameManager.instance.Player.AcceList.Count > 5;
    }

    bool CheckMaxLevel(WeaponData data)
    {
        WeaponBase weapon;

        if(data.itemType == WeaponData.ItemType.Weapon)
        {
            weapon = InGameManager.instance.WeaponManager.transform.Find("Weapon" + data.itemId).GetComponent<WeaponBase>();
        }
        else
        {
            weapon = InGameManager.instance.WeaponManager.transform.Find("Acce" + data.itemId).GetComponent<WeaponBase>();
        }

        return weapon.level == data.maxlevel;
    }

    WeaponData GetSelectItem(List<int> usedNum)
    {
        List<WeaponData> availableItems = new List<WeaponData>();

        switch (mysitu)
        {
            case ItemSituation.Full:
                availableItems.AddRange(InGameManager.instance.Player.WeaponList.Select(id => InGameManager.instance.WeaponManager.Weapons[id]));
                availableItems.AddRange(InGameManager.instance.Player.AcceList.Select(id => InGameManager.instance.WeaponManager.Accessories[id]));
                break;

            case ItemSituation.OnlyWeapon:
                availableItems.AddRange(InGameManager.instance.Player.WeaponList.Select(id => InGameManager.instance.WeaponManager.Weapons[id]));
                availableItems.AddRange(InGameManager.instance.WeaponManager.Accessories);
                break;

            case ItemSituation.OnlyAcce:
                availableItems.AddRange(InGameManager.instance.WeaponManager.Weapons);
                availableItems.AddRange(InGameManager.instance.Player.AcceList.Select(id => InGameManager.instance.WeaponManager.Accessories[id]));
                break;

            case ItemSituation.Available:
                availableItems.AddRange(InGameManager.instance.WeaponManager.Weapons);
                availableItems.AddRange(InGameManager.instance.WeaponManager.Accessories);
                break;
        }

        WeaponData item = null;
        int randomnum = Random.Range(0, availableItems.Count);

        if (!usedNum.Contains(randomnum))
        {
            item = availableItems[randomnum];
            if (CheckMaxLevel(item)) // 해당 무기나 장신구가 최고 레벨이면 거름
            {
                item = null;
            }
            usedNum.Add(randomnum);
        }

        return item;
    }

    // 랜덤으로 count만큼 아이템을 골라주는 함수
    WeaponData[] RandomItemSelect(int count)
    {
        List<WeaponData> items = new List<WeaponData>();
        List<int> usedNum = new List<int>();

        while(items.Count < count) // items List의 길이가 count보다 작으면 같아질때까지 계속 추가 
        {
            WeaponData selectedItem = GetSelectItem(usedNum);
            if (selectedItem != null)
            {
                items.Add(selectedItem);
            }
        }
        return items.ToArray();;
    }
}