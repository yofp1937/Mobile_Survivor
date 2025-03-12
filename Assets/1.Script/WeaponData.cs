using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public enum ItemType { Weapon, Accessories, Artifacts, ETC }

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public string itemDesc;
    public int maxlevel;
    public Sprite itemIcon;
    public WeaponStatusData weaponData;
    public AcceStatusData acceData;

    [Header("# Level Data")]
    // 최대레벨만큼 배열 늘리기
    public WeaponStatusData[] levelupdata_weapon;
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
            case ItemType.Artifacts:
                maxlevel = 1;
                break;
            case ItemType.ETC:
                maxlevel = 0;
                break;
        }
    }
}
