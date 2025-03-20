using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Object/UpgradeData", order = 1)]
public class UpgradeData : ScriptableObject
{
    [Header("# Main Info")]
    public UpgradeEnum EnumName;
    public Status Data; // 증가시켜주는 Stat 종류
    public int MaxLevel; // 최고레벨
    public List<int> CostList; // 레벨별 가격
    public string Desc; //증가시켜주는 Stat 설명
}
