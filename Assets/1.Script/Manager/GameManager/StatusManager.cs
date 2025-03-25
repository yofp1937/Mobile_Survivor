using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [Header("# Main Data")]
    public List<StatusData> StatusDataList; // 캐릭터별 Status Data List
    public List<UpgradeData> UpgradeDataList; // Upgrade Data List

    [Header("# Upgrade Data")]
    [SerializeField] Status _upgradeStatus;
    public Dictionary<UpgradeEnum, int> UpgradeLevelDict = new Dictionary<UpgradeEnum, int>(); // Upgrade 레벨 저장용
    public int UpgradeCost;

    [Header("# Equipment Data")]
    public Status EquipStatus;

    public void LoadUpgradeLevelInDB(UpgradeLevelData data) // 게임 실행시 1회만 실행 - 유저의 저장된 UpgradeData 로드
    {
        foreach(var pair in data.dict)
        {
            UpgradeEnum key = (UpgradeEnum)Enum.Parse(typeof(UpgradeEnum), pair.Key);
            UpgradeLevelDict[key] = Convert.ToInt32(pair.Value);
        }
    }

    public int GetUpgradeLevel(UpgradeEnum data) => UpgradeLevelDict[data]; // Upgrade 레벨 가져오기

    public void SetUpgradeLevel(UpgradeEnum data, int level) // Upgrade 레벨 변경 및 저장
    {
        UpgradeLevelDict[data] = level;
        UpdateUpgradeLevelInDB(data, level);
    }

    void UpdateUpgradeLevelInDB(UpgradeEnum data, int level) // 특정 Upgrade 값만 DB에 저장
    {
        Dictionary<string, object> updateData = new Dictionary<string, object>();
        updateData[data.ToString()] = level;

        DBManager.instance.UpdateUpgradeDataInUserData(updateData);
    }

    public void ResetUpgrade() // UpgradePanel에서 Upgrade Reset버튼 동작시 실행
    {
        DBManager.instance.CreateUserUpgradeData();
    }
    public void CombineUpgradeStat(Status stat) // stat을 UpgradeData에 기반하여 증가시킴
    {
        List<UpgradeEnum> keys = new List<UpgradeEnum>(UpgradeLevelDict.Keys);
        List<int> values = new List<int>(UpgradeLevelDict.Values);

        foreach(UpgradeData data in UpgradeDataList)
        {
            int _index = keys.IndexOf(data.EnumName);

            if(_index != -1)
            {
                int _level = values[_index];
                for(int i = 0; i < _level; i++)
                {
                    stat.AddStatus(data.Data);
                }
            }
        }
    }

    public Status GetUpgradeStatus()
    {
        _upgradeStatus = new Status();
        CombineUpgradeStat(_upgradeStatus);
        return _upgradeStatus;
    }

    public void CombineEquipStat(Status stat)
    {
        stat.AddStatus(EquipStatus);
    }

    public void AddEquipStatus(StatusEnum option, float value)
    {
        switch(option)
        {
            case StatusEnum.Hp:
                EquipStatus.Hp += value;
                break;
            case StatusEnum.AttackPower:
                EquipStatus.AttackPower += value;
                break;
            case StatusEnum.Defense:
                EquipStatus.Defense += value;
                break;
            case StatusEnum.ObtainRange:
                EquipStatus.ObtainRange += value;
                break;
            case StatusEnum.CriticalChance:
                EquipStatus.CriticalChance += value;
                break;
            case StatusEnum.CriticalDamage:
                EquipStatus.CriticalDamage += value;
                break;
            case StatusEnum.CoolTime:
                EquipStatus.CoolTime += value;
                break;
            case StatusEnum.Duration:
                EquipStatus.Duration += value;
                break;
            case StatusEnum.AttackRange:
                EquipStatus.AttackRange += value;
                break;
            case StatusEnum.ProjectileCount:
                EquipStatus.ProjectileCount += (int)value;
                break;
            case StatusEnum.ProjectileSize:
                EquipStatus.ProjectileSize += value;
                break;
            case StatusEnum.ProjectileSpeed:
                EquipStatus.ProjectileSpeed += value;
                break;
            case StatusEnum.MoveSpeed:
                EquipStatus.MoveSpeed += value;
                break;
            case StatusEnum.Curse:
                EquipStatus.Curse += value;
                break;
        }
    }

    public void SubEquipStatus(StatusEnum option, float value)
    {
        switch(option)
        {
            case StatusEnum.Hp:
                EquipStatus.Hp -= value;
                break;
            case StatusEnum.AttackPower:
                EquipStatus.AttackPower -= value;
                break;
            case StatusEnum.Defense:
                EquipStatus.Defense -= value;
                break;
            case StatusEnum.ObtainRange:
                EquipStatus.ObtainRange -= value;
                break;
            case StatusEnum.CriticalChance:
                EquipStatus.CriticalChance -= value;
                break;
            case StatusEnum.CriticalDamage:
                EquipStatus.CriticalDamage -= value;
                break;
            case StatusEnum.CoolTime:
                EquipStatus.CoolTime -= value;
                break;
            case StatusEnum.Duration:
                EquipStatus.Duration -= value;
                break;
            case StatusEnum.AttackRange:
                EquipStatus.AttackRange -= value;
                break;
            case StatusEnum.ProjectileCount:
                EquipStatus.ProjectileCount -= (int)value;
                break;
            case StatusEnum.ProjectileSize:
                EquipStatus.ProjectileSize -= value;
                break;
            case StatusEnum.ProjectileSpeed:
                EquipStatus.ProjectileSpeed -= value;
                break;
            case StatusEnum.MoveSpeed:
                EquipStatus.MoveSpeed -= value;
                break;
            case StatusEnum.Curse:
                EquipStatus.Curse -= value;
                break;
        }
    }
}
