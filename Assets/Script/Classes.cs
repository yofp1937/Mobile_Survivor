using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 무기의 기본 스테이터스
[Serializable] public class WeaponData
{
    public float Damage = 0; // 공격력
    public float CoolTime = 0; // 쿨타임
    public float AttackRange = 0; // 공격 범위
    public float Duration = 0; // 지속시간
    public int ProjectileCount = 0; // 투사체 갯수
    public float ProjectileSpeed = 0; // 투사체 속도
    public float ProjectileSize = 0; // 투사체 크기
    public float Knockback = 0; // 넉백

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
