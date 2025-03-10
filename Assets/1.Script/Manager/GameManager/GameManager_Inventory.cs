using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Inventory : MonoBehaviour
{
    #region "Singleton"
    public static GameManager_Inventory instance;
    
    void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {
            instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    public GameObject SlotUI; // 인벤토리 Slot 프리팹

    // 대리자를 사용하여 인벤토리 슬롯 변경 구현
    public delegate void OnSlotCountChange(int val);
    public OnSlotCountChange onSlotCountChange;
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
    
    void Init() // Awake() 내부에서 실행
    {
        // InvenSlot 값이 존재하지 않으면 15로 설정(처음 플레이하는 유저 데이터 설정)
        if(!PlayerPrefs.HasKey("InvenSlot"))
        {
            PlayerPrefs.SetInt("InvenSlot", 15);
            PlayerPrefs.Save();
        }

        slotCnt = PlayerPrefs.GetInt("InvenSlot");
    }

    public void CreateEquip()
    {
        Equipment equip;
        EquipmentData data;
    }


}
