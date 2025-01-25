using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentData : MonoBehaviour
{
    public enum EquipGrade { Common, UnCommon, Rare, Unique, Legendary } // 흔함, 평범, 희귀, 특별, 전설
    public enum EquipOption
    {
        Hp, HpRegen, Damage, AttackSpeed, Depense, MoveSpeed, AttackCount, CoolTime, Duration, AttackArea, GetArea, CriticalChance, CriticalDamage, Luck, Curse
    }
    public EquipGrade Grade;
    public EquipOption MainOption;
    public EquipOption[] SubOption;
    private int subOptionCount; // Grade에 따라 1~5까지 정해짐
    private int equipLevel; // 장비의 레벨(4의 배수마다 1강화)
    public int[] SubOptionStrengthLevels; // 각 서브옵션이 몇번 강화됐는지 표시
}
