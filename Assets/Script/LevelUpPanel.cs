using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelUpPanel : MonoBehaviour
{
    public Item[] ItemLists;

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
        ItemLists = GetComponentsInChildren<Item>();
    }

    void BaseSetting()
    {
        GetMyEquipSituation(CheckWeapon(), CheckAcce());
        GetSelectItemCount();
    }

    void OnEnable()
    {
        BaseSetting();
        LoadLevelUpPanel();
    }

    public void HideLevelUpPanel()
    {
        InGameManager.instance.LevelUpPanel.SetActive(false);
        GameManager.instance.TimerStart();
    }

    void GetSelectItemCount()
    {
        // 아이템을 가져올 개수를 구해주는 함수
        if(mysitu == ItemSituation.Full && InGameManager.instance.player.maxlevelcount > weaponcount - 1)
        {
            selectcount = 0;
            ItemLists[0].gameObject.SetActive(false);
        }
        else if(mysitu == ItemSituation.Full && InGameManager.instance.player.maxlevelcount > weaponcount - 2)
        {
            selectcount = 1;
            ItemLists[1].gameObject.SetActive(false);
        }
        else if(mysitu == ItemSituation.Full && InGameManager.instance.player.maxlevelcount > weaponcount - 3)
        {
            selectcount = 2;
            ItemLists[2].gameObject.SetActive(false);
        }
        else
        {
            selectcount = 3;
        }       
    }

    void LoadLevelUpPanel()
    {
        if(selectcount == 0)
        {
            ItemLists[0].MaxLevelSetting(0);
            ItemLists[0].gameObject.SetActive(true);
            ItemLists[1].MaxLevelSetting(1);
            ItemLists[1].gameObject.SetActive(true);
        }
        else
        {
            // 3개의 랜덤 아이템 선택
            ItemData[] randomItems = RandomItemSelect(selectcount);
            
            for(int i = 0; i < randomItems.Length; i++)
            {
                ItemLists[i].SetItemData(randomItems[i]);
            }
        }
    }

    void GetMyEquipSituation(bool myweapon, bool myacce)
    {
        if(myweapon && myacce)
        {
            mysitu = ItemSituation.Full;
            weaponcount = InGameManager.instance.player.weapon.Count + InGameManager.instance.player.accesorries.Count;
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
        return InGameManager.instance.player.weapon.Count > 5;
    }

    bool CheckAcce()
    {
        return InGameManager.instance.player.accesorries.Count > 5;
    }

    bool CheckMaxLevel(ItemData data)
    {
        Weapon weapon;

        if(data.itemType == ItemData.ItemType.Weapon)
        {
            weapon = InGameManager.instance.WeaponManager.transform.Find("Weapon" + data.itemId).GetComponent<Weapon>();
        }
        else
        {
            weapon = InGameManager.instance.WeaponManager.transform.Find("Acce" + data.itemId).GetComponent<Weapon>();
        }

        return weapon.level == data.maxlevel;
    }

    ItemData getSelectItem(List<int> usedNum)
    {
        List<ItemData> availableItems = new List<ItemData>();

        switch (mysitu)
        {
            case ItemSituation.Full:
                availableItems.AddRange(InGameManager.instance.player.weapon.Select(id => InGameManager.instance.WeaponManager.weapons[id]));
                availableItems.AddRange(InGameManager.instance.player.accesorries.Select(id => InGameManager.instance.WeaponManager.accessories[id]));
                break;

            case ItemSituation.OnlyWeapon:
                availableItems.AddRange(InGameManager.instance.player.weapon.Select(id => InGameManager.instance.WeaponManager.weapons[id]));
                availableItems.AddRange(InGameManager.instance.WeaponManager.accessories);
                break;

            case ItemSituation.OnlyAcce:
                availableItems.AddRange(InGameManager.instance.WeaponManager.weapons);
                availableItems.AddRange(InGameManager.instance.player.accesorries.Select(id => InGameManager.instance.WeaponManager.accessories[id]));
                break;

            case ItemSituation.Available:
                availableItems.AddRange(InGameManager.instance.WeaponManager.weapons);
                availableItems.AddRange(InGameManager.instance.WeaponManager.accessories);
                break;
        }

        ItemData item = null;
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
    ItemData[] RandomItemSelect(int count)
    {
        List<ItemData> items = new List<ItemData>();
        List<int> usedNum = new List<int>();

        while(items.Count < count) // items List의 길이가 count보다 작으면 같아질때까지 계속 추가 
        {
            ItemData selectedItem = getSelectItem(usedNum);
            if (selectedItem != null)
            {
                items.Add(selectedItem);
            }
        }
        return items.ToArray();;
    }
}