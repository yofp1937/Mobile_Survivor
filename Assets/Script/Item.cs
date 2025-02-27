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
        if(gameObject.activeSelf == true)
        {
            this.data = data;
            BaseSetting();
            ItemSetting();
        }
    }

    void ItemSetting()
    {
        image.sprite = data.itemIcon;
        image.SetNativeSize();

        textLevel.text = "Lv. " + weapon.level + " / " + data.maxlevel;
        textName.text = data.itemName;

        if(weapon.level == 0)
        {
            textDesc.text = data.itemDesc;
        }
        else if(data.itemType == ItemData.ItemType.Weapon || data.itemType == ItemData.ItemType.Accessories)
        {
            textDesc.text = data.descriptions[weapon.level-1];
        }
    }

    public void MaxLevelSetting(int num)
    {
        switch(num)
        {
            case 0:
                this.data = InGameManager.instance.WeaponManager.etcs[0];
                image.sprite = data.itemIcon;
                image.SetNativeSize();
                textLevel.text = data.itemName;
                textName.text = data.itemName;
                textDesc.text = data.itemDesc;
                break;
            case 1:
                this.data = InGameManager.instance.WeaponManager.etcs[1];
                image.sprite = data.itemIcon;
                image.SetNativeSize();
                textLevel.text = data.itemName;
                textName.text = data.itemName;
                textDesc.text = data.itemDesc;
                break;
            default:
                break;
        }
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
                    weapon.gameObject.SetActive(true);
                    weapon.InitAcce(data);
                }
                else if(weapon.level < 5)
                {
                    if(weapon.itemdata.itemType == ItemData.ItemType.Accessories && weapon.level == 4)
                    {
                        InGameManager.instance.player.maxlevelcount++;
                    }
                    InGameManager.instance.player.stat.AddStatus(data.levelupdata_acce[weapon.level-1]);
                }
                weapon.level++;
                break;
            case ItemData.ItemType.ETC:
                if(data.itemId == 0)
                {
                    InGameManager.instance.player.GetHeal(30);
                }
                else if(data.itemId == 1)
                {
                    InGameManager.instance.player.GetGold(10);
                }
                break;
            default:
                break;
        }
        InGameManager.instance.OnLevelUp = false;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        AudioManager.instance.EffectBgm(false);
    }
}
