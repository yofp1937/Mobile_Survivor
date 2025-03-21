using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("# Main Data")]
    public List<GameObject> EquippedEquips = new List<GameObject>(); // 장착한 장비
    public List<GameObject> Equips = new List<GameObject>(); // 갖고있는 장비

    [Header("# Inventory Slot Data")]
    int slotCnt;
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

    // 대리자를 사용하여 인벤토리 슬롯 변경 구현
    public delegate void OnSlotCountChange(int val);
    public OnSlotCountChange onSlotCountChange;


    void Awake()
    {
        SlotCnt = PlayerPrefs.GetInt("InvenSlot", 15);
        // 여기서 DB 아이템정보를 불러왔을때 장착된 장비들 옵션만큼 GameManager.instance.StatusManager.AddEquipStatus() 호출해야함
    }

    public void GetEquipment(GameObject equipment) // 아이템 획득시 호출
    {
        if(Equips.Count < SlotCnt)
        {
            Equips.Add(equipment);
        }
        else
        {
            Debug.Log("인벤토리가 가득 참");
        }
    }

    public void SortEquipment() // 등급 -> 부위 순으로 정렬후 인벤토리창 리로딩 ★Equips에 데이터를 전부 집어넣은후 한번 호출할것
    {
        Equips = Equips.OrderByDescending(e => e.GetComponent<Equipment>().Grade)
                       .ThenBy(e => e.GetComponent<Equipment>().Part)
                       .ToList();
        LobbyManager.instance.InventoryPanel.LoadEquipmentInInvetorySlot();
    }

    public void DeleteNullInEquips()
    {
        EquippedEquips.RemoveAll(equip => equip == null);
        Equips.RemoveAll(equip => equip == null);
    }
}
