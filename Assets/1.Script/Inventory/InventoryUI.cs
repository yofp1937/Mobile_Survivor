using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    GameManager_Inventory inven;
    public InventorySlot[] slots;
    public Transform slotHolder;

    void Start()
    {
        inven = GameManager_Inventory.instance;
        slots = slotHolder.GetComponentsInChildren<InventorySlot>();
        Init();
    }

    // 플레이어의 가방 칸 로딩
    private void Init()
    {
        int count = inven.SlotCnt - slots.Length;

        if(count > 0) 
        {
            for(int i = 0; i < count; i++)
            {
                CreateSlot();
            }
        }
    }

    // 가방 칸 생성
    public void AddSlot()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        if(GameManager.instance.Gold < 50)
        {
            return;
        }
        GameManager.instance.Gold -= 50;
        inven.SlotCnt++;
        CreateSlot();
    }

    private void CreateSlot()
    {
        // SlotUI 생성
        GameObject newSlot = Instantiate(inven.SlotUI, slotHolder);
        // 생성한 슬롯 가방 확장 버튼 앞으로 이동
        int location = slotHolder.childCount - 2; // 가방 확장 버튼 앞 위치
        newSlot.transform.SetSiblingIndex(location); // 인덱스 재설정
    }
}
