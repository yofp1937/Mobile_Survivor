using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacter : MonoBehaviour
{
    [Header("# Character Prefab")]
    public List<GameObject> characters; // Hierarchy상에 존재하는 캐릭터 GameObject들
    public List<GameObject> prefabs; // Project상에 존재하는 캐릭터 Prefab들

    [Header("# Weapon Prefab")]
    public List<ItemData> weapons;

    public Button startbtn;
    public Image weaponImage;

    void Awake()
    {
        
    }

    void Select(int index)
    {
        foreach(GameObject character in characters)
        {
            character.SetActive(false);
        }
        characters[index].SetActive(true);
        GameManager.instance.SelectCharacter = prefabs[index];
        startbtn.interactable = true;
    }

    void SelectWeapon(int index)
    {
        GameManager.instance.SelectWeapon = weapons[index];
        weaponImage.sprite = weapons[index].itemIcon;
        weaponImage.SetNativeSize();
    }

    public void OnClickSelectKnight()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Select(0);
        SelectWeapon(0);
    }
    public void OnClickSelectMerchant()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Select(1);
        SelectWeapon(3);
    }
    public void OnClickSelectPeasant()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Select(2);
        SelectWeapon(5);
    }
    public void OnClickSelectPriest()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Select(3);
        SelectWeapon(4);
    }
    public void OnClickSelectSoldier()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Select(4);
        SelectWeapon(6);
    }
    public void OnClickSelectThief()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        Select(5);
        SelectWeapon(1);
    }
}
