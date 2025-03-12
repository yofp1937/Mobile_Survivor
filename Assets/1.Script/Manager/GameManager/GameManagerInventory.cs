using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerInventory : MonoBehaviour
{
    [Header("# Main Data")]
    private int slotCnt;
    public int SlotCnt
    {
        get => slotCnt;
        set
        {
            slotCnt = value;
            PlayerPrefs.SetInt("InvenSlot", slotCnt);
            PlayerPrefs.Save();
            onSlotCountChange?.Invoke(slotCnt);
        }
    }
    public GameObject[] Equips; // 장비 프리팹


    // 대리자를 사용하여 인벤토리 슬롯 변경 구현
    public delegate void OnSlotCountChange(int val);
    public OnSlotCountChange onSlotCountChange;


    void Awake()
    {
        slotCnt = PlayerPrefs.GetInt("InvenSlot", 15);
    }

    public void CreateEquip() // 장비 생성
    {

    }
}
