using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 무기의 기본 스테이터스
[Serializable] public class WeaponData
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

// 통계창에서 띄울 무기별 데미지 데이터 저장 클래스
public class AccumWeaponData
{
    public ItemData Weapon { get; set; }
    public int Level { get; set; }
    public float TotalDamage { get; set; }

    public void SetData(ItemData weapon)
    {
        Weapon = weapon;
    }

    public void SetLevel(int level)
    {
        Level = level;
    }

    public void AddDamage(float damage)
    {
        TotalDamage += damage;
    }
}
