using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class WeaponData1
{
    public float damage = 0;
    public float coolTime = 0;
    public float area = 0;
    public float duration = 0;
    public int count = 0;
    public float speed = 0;
    public float knockback = 0;
}

[Serializable]
public class BaseData1
{
    public enum ItemType { Weapon, Accessories, Artifacts }

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    public string itemDesc;
    public Sprite itemIcon;
    public WeaponData1 weaponData;

    [Header("# Level Data")]
    // 최대레벨만큼 배열 늘리기
    public float[] damages;
    public float[] counts;
    public string[] descriptions;

    [Header("# Weapon")]
    public GameObject projectile;
}

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData", order = 1)]
public class ItemData1 : ScriptableObject
{
    [Header("★중요한 무기 기본정보이니 반드시 스크립트 내에서 값 변경할것\nInspector 우측 위 Reset을 이용해서 내용 갱신 가능\n아이콘과 프리팹은 직접 넣어줘야함")]
    public List<BaseData1> ItemList;

    public ItemData1()
    {
        ItemList = new List<BaseData1>();

        var item0 = new BaseData1
        {
            itemType = BaseData1.ItemType.Weapon,
            itemId = 0,
            itemName = "RotateSword",
            itemDesc = "플레이어 주변을 회전하며 데미지를 입힙니다.",

            weaponData = new WeaponData1
            {
                damage = 3,
                coolTime = 8,
                area = 2,
                duration = 3.5f,
                count = 3,
                speed = 150,
                knockback = 10
            },

            damages = new float[] { 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 1.5f, 2 },
            counts = new float[] { 0, 1, 0, 0, 1, 0, 2 },
            descriptions = new string[]
            {
                "공격력 1.5 증가",
                "공격력 1.5 증가, 무기 개수 1개 증가",
                "공격력 1.5 증가",
                "공격력 1.5 증가",
                "공격력 1.5 증가, 무기 개수 1개 증가",
                "공격력 1.5 증가",
                "공격력 2 증가, 무기 개수 2개 증가"
            },
        };
        ItemList.Add(item0);


        var item1 = new BaseData1
        {
            itemType = BaseData1.ItemType.Weapon,
            itemId = 1,
            itemName = "ThrowWeapon",
            itemDesc = "가장 가까운적에게 무기를 던집니다.",

            weaponData = new WeaponData1
            {
                damage = 5,
                coolTime = 3,
                area = 0,
                duration = 0,
                count = 1,
                speed = 5,
                knockback = 1.5f
            },

            damages = new float[] { 1, 2, 1, 2, 1, 2, 3 },
            descriptions = new string[]
            {
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 3 증가"
            },
        };
        ItemList.Add(item1);

        var item2 = new BaseData1
        {
            itemType = BaseData1.ItemType.Weapon,
            itemId = 2,
            itemName = "Test2",
            itemDesc = "Test2",

            weaponData = new WeaponData1
            {
                damage = 2,
                coolTime = 2,
                area = 2,
                duration = 2,
                count = 2,
                speed = 2,
                knockback = 2
            },

            damages = new float[] { 1, 2, 1, 2, 1, 2, 3 },
            descriptions = new string[]
            {
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 3 증가"
            },
        };
        ItemList.Add(item2);

        var item3 = new BaseData1
        {
            itemType = BaseData1.ItemType.Weapon,
            itemId = 2,
            itemName = "Test3",
            itemDesc = "Test3",

            weaponData = new WeaponData1
            {
                damage = 2,
                coolTime = 2,
                area = 2,
                duration = 2,
                count = 2,
                speed = 2,
                knockback = 2
            },

            damages = new float[] { 1, 2, 1, 2, 1, 2, 3 },
            descriptions = new string[]
            {
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 3 증가"
            },
        };
        ItemList.Add(item3);

        var item4 = new BaseData1
        {
            itemType = BaseData1.ItemType.Weapon,
            itemId = 2,
            itemName = "Test4",
            itemDesc = "Test4",

            weaponData = new WeaponData1
            {
                damage = 2,
                coolTime = 2,
                area = 2,
                duration = 2,
                count = 2,
                speed = 2,
                knockback = 2
            },

            damages = new float[] { 1, 2, 1, 2, 1, 2, 3 },
            descriptions = new string[]
            {
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 3 증가"
            },
        };
        ItemList.Add(item4);

        var item5 = new BaseData1
        {
            itemType = BaseData1.ItemType.Weapon,
            itemId = 2,
            itemName = "Test5",
            itemDesc = "Test5",

            weaponData = new WeaponData1
            {
                damage = 2,
                coolTime = 2,
                area = 2,
                duration = 2,
                count = 2,
                speed = 2,
                knockback = 2
            },

            damages = new float[] { 1, 2, 1, 2, 1, 2, 3 },
            descriptions = new string[]
            {
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 1 증가",
                "공격력 2 증가",
                "공격력 3 증가"
            },
        };
        ItemList.Add(item5);
    }
    private void OnEnable()
    {
        foreach(var item in ItemList)
        {
            switch(item.itemId)
            {
                case 0:
                    item.itemIcon = Resources.Load<Sprite>("Assets/Aseets/Weapons/Medieval_Weapons/Image/Sword/Sword_10.png");
                    break;
                case 1:
                    item.itemIcon = Resources.Load<Sprite>("Assets/Aseets/Weapons/Medieval_Weapons/Image/Other/Other_11.png");
                    break;
            }
        }
    }
}
