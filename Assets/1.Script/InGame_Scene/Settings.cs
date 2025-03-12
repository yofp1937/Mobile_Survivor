using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject[] WeaponSlots;
    public GameObject[] AcceSlots;

    public void SettingSlot()
    {
        List<int> weapons = InGameManager.instance.player.WeaponList;
        List<int> acces = InGameManager.instance.player.AcceList;

        for(int i = 0; i < weapons.Count; i++)
        {
            Image slotimage = WeaponSlots[i].transform.Find("Image").GetComponent<Image>();
            Text slottext = WeaponSlots[i].transform.Find("Text").GetComponent<Text>();

            WeaponBase weapon = GameObject.Find("Weapon" + weapons[i]).GetComponent<WeaponBase>();
            slotimage.gameObject.SetActive(true);
            slotimage.sprite = weapon.WeaponData.itemIcon;
            slotimage.SetNativeSize();

            slottext.text = "Lv." + weapon.level;
        }

        for(int i = 0; i < acces.Count; i++)
        {
            Image slotimage = AcceSlots[i].transform.Find("Image").GetComponent<Image>();
            Text slottext = AcceSlots[i].transform.Find("Text").GetComponent<Text>();

            WeaponBase acce = GameObject.Find("Acce" + acces[i]).GetComponent<WeaponBase>();
            slotimage.gameObject.SetActive(true);
            slotimage.sprite = acce.WeaponData.itemIcon;
            slotimage.SetNativeSize();

            slottext.text = "Lv." + acce.level;
        }
    }
}
