using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    [Header("# Main Data")]
    [SerializeField] EquipmentData[] CommonEquips;
    [SerializeField] EquipmentData[] UnCommonEquips;
    [SerializeField] EquipmentData[] RareEquips;
    [SerializeField] EquipmentData[] UniqueEquips;
    [SerializeField] EquipmentData[] LegendaryEquips;

    public EquipmentData GetDropEquipData(EquipGrade grade, EquipPart part) // 등급과 부위로 생성 요청 들어오면 아이템 생성
    {
        // grade에 맞는 장비 목록 선택
        EquipmentData[] equipList = grade switch
        {
            EquipGrade.Common => CommonEquips,
            EquipGrade.UnCommon => UnCommonEquips,
            EquipGrade.Rare => RareEquips,
            EquipGrade.Unique => UniqueEquips,
            _ => LegendaryEquips
        };
        
        // part와 일치하는 EquipmentData 찾아오기
        EquipmentData equipData = equipList.FirstOrDefault(data => data.Part == part);

        return equipData;
    }
}
