using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class WeaponData
{
    public float damage = 0;
    public float coolTime = 0;
    public float area = 0;
    public float duration = 0;
    public int count = 0;
    public float speed = 0;
    public float knockback = 0;

    public WeaponData Clone()
    {
        return MemberwiseClone() as WeaponData;
    }
}

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
    public enum ItemType { Weapon, Accessories, Artifacts, ETC }

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public string itemDesc;
    public int maxlevel;
    public Sprite itemIcon;
    public WeaponData weaponData;
    public ELevelUpStat acceData;

    [Header("# Level Data")]
    // 최대레벨만큼 배열 늘리기
    public WeaponData[] levelupdata_weapon;
    public ELevelUpStat[] levelupdata_acce;
    public string[] descriptions;

    [Header("# Weapon")]
    public GameObject projectile;
    public GameObject projectile2;

    private void OnEnable()
    {
        SetMaxLevel();
    }

    private void SetMaxLevel()
    {
        switch(itemType)
        {
            case ItemType.Weapon:
                maxlevel = 8;
                break;
            case ItemType.Accessories:
                maxlevel = 5;
                break;
            case ItemType.Artifacts:
                maxlevel = 1;
                break;
            case ItemType.ETC:
                maxlevel = 0;
                break;
        }
    }
}
