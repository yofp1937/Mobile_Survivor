using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    
    Weapon weapon;
    Image image;
    Text textLevel;
    Text textName;
    Text textDesc;

    void BaseSetting()
    {
        if(data.itemType == ItemData.ItemType.Weapon)
        {
            weapon = InGameManager.instance.WeaponManager.transform.Find("Weapon"+data.itemId).GetComponent<Weapon>();
        }
        else if(data.itemType == ItemData.ItemType.Accessories)
        {
            weapon = InGameManager.instance.WeaponManager.transform.Find("Acce"+data.itemId).GetComponent<Weapon>();
        }
        Image[] images = GetComponentsInChildren<Image>();
        image = images[3];

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
    }

    public void SetItemData(ItemData data)
    {
        this.data = data;
        BaseSetting();
        ItemSetting();
    }

    void ItemSetting()
    {
        image.sprite = data.itemIcon;
        image.SetNativeSize();
        textLevel.text = "Lv. " + weapon.level + " / " + data.maxlevel;
        textName.text = data.itemName;
        textDesc.text = data.itemDesc;
    }

    // Panel의 온클릭 이벤트
    public void OnClick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Weapon:
                if(weapon.level == 0)
                {
                    weapon.gameObject.SetActive(true);
                    weapon.Init(data);
                }
                else if(weapon.level < 8)
                {
                    weapon.LevelUp(data.levelupdata_weapon[weapon.level-1]);
                }
                weapon.level++;
                break;
            case ItemData.ItemType.Accessories:
                if(weapon.level == 0)
                {
                    InGameManager.instance.player.stat.LevelUp(data.acceData);
                    InGameManager.instance.player.accesorries.Add(data.itemId);
                }
                else if(weapon.level < 5)
                {
                    InGameManager.instance.player.stat.LevelUp(data.levelupdata_acce[weapon.level-1]);
                }
                weapon.level++;
                break;
            default:

                break;
        }
    }
}
