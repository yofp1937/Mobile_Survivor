using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerStatus : MonoBehaviour
{
    [Header("# Main Data")]
    public List<StatusData> StatusDataList; // 캐릭터별 Status Data List
    public List<UpgradeData> UpgradeDataList; // Upgrade Data List

    [Header("# Player Data")]
    public Dictionary<UpgradeEnum, int> UpgradeLevelDict = new Dictionary<UpgradeEnum, int>(); // Upgrade 레벨 저장용

    void Awake()
    {
        LoadUpgradeLevel();
    }

    void LoadUpgradeLevel() // 게임 실행시 1회만 실행 - 유저의 저장된 UpgradeData 로드
    {
        foreach (UpgradeEnum type in Enum.GetValues(typeof(UpgradeEnum)))
        {
            SetUpgradeLevel(type, PlayerPrefs.GetInt(type.ToString(), 0));
        }
    }

    public int GetUpgradeLevel(UpgradeEnum data) => UpgradeLevelDict[data]; // Upgrade 레벨 가져오기

    public void SetUpgradeLevel(UpgradeEnum data, int level) // Upgrade 레벨 변경 및 저장
    {
        UpgradeLevelDict[data] = level;
        PlayerPrefs.SetInt(data.ToString(), level);
        PlayerPrefs.Save();
    }

    public void ResetUpgrade() // UpgradePanel에서 Upgrade Reset버튼 동작시 실행
    {
        foreach (UpgradeEnum type in Enum.GetValues(typeof(UpgradeEnum))) // 유저의 UpgradeData를 초기화
        {
            SetUpgradeLevel(type, 0);
            PlayerPrefs.SetInt(type.ToString(), 0);
            PlayerPrefs.Save();
        }
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
}
