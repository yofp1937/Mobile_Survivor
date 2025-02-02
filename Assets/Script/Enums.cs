using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    각종 enum(열거형 데이터) 초기화
*/

public enum PlayerData {Hp, AttackPower, AttackSpeed, Cooldown, AttackRange, Duration, Amount, Magnet} // 로비 강화 탭에서 강화하는 능력치들


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