using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    각종 enum(열거형 데이터) 초기화
*/

public enum UpgradeEnum {Hp, AttackPower, ProjectileCount, ProjectileSpeed, CoolTime, Duration, AttackRange, ObtainRange} // 로비 강화 탭에서 강화하는 능력치들

public enum PoolList // ObjectPoolling을 활용하는 객체 리스트
{
    None = -1,
    // Weapons
    RotateSword, ThrowWeapon, Laser, Fireball, Thunder, Spark, Wave,
    // Enemies
    FlyEye, Goblin, Mushroom, Skeleton,
    // Items
    ExpJewel_1, ExpJewel_3, ExpJewel_5, Gold, Magnet, Potion,
    // DmgPopUp
    DamagePopUp
}

public enum WeaponName { RotateSword, ThrowWeapon, Laser, Fireball, Thunder, Spark, Wave } // 무기 종류


// 아이템 등급
public enum EquipGrade
{
    Common, // 흔함
    UnCommon, // 안흔함
    Rare, // 특별
    Unique, // 희귀
    Legendary // 전설
}

// 아이템 옵션
public enum EquipOption
{
    Hp, HpRegen, Damage, AttackSpeed, Depense, MoveSpeed, AttackCount, CoolTime, Duration, AttackArea, GetArea, CriticalChance, CriticalDamage, Luck, Curse
}