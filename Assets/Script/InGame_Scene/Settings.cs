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
        List<int> weapons = InGameManager.instance.player.Weapon;
        List<int> acces = InGameManager.instance.player.Accesorries;

        for(int i = 0; i < weapons.Count; i++)
        {
            Image slotimage = WeaponSlots[i].transform.Find("Image").GetComponent<Image>();
            Text slottext = WeaponSlots[i].transform.Find("Text").GetComponent<Text>();

            Weapon weapon = GameObject.Find("Weapon" + weapons[i]).GetComponent<Weapon>(); // weapons[i]에 들어있는 weapon 객체를 찾음
            slotimage.gameObject.SetActive(true);
            slotimage.sprite = weapon.itemdata.itemIcon;
            slotimage.SetNativeSize();

            slottext.text = "Lv." + weapon.level;
        }

        for(int i = 0; i < acces.Count; i++)
        {
            Image slotimage = AcceSlots[i].transform.Find("Image").GetComponent<Image>();
            Text slottext = AcceSlots[i].transform.Find("Text").GetComponent<Text>();

            Weapon acce = GameObject.Find("Acce" + acces[i]).GetComponent<Weapon>(); // weapons[i]에 들어있는 weapon 객체를 찾음
            slotimage.gameObject.SetActive(true);
            slotimage.sprite = acce.itemdata.itemIcon;
            slotimage.SetNativeSize();

            slottext.text = "Lv." + acce.level;
        }
    }
}
