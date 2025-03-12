using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    [Header("# Main Data")]
    [SerializeField] GameObject _slotUI; // 인벤토리 Slot 프리팹
    public Transform slotHolder; // _slots의 부모 객체
    InventorySlot[] _slots;

    [Header("# Reference Data")]
    GameManagerInventory Inventory;

    void Start()
    {
        Inventory = GameManager.instance.Inventory;
        _slots = slotHolder.GetComponentsInChildren<InventorySlot>();
        Init();
    }

    private void Init() // 플레이어의 가방 칸 로딩
    {
        int count = Inventory.SlotCnt - _slots.Length; // 플레이어의 최대 슬롯 - 생성돼있는 슬롯

        if(count > 0) 
        {
            for(int i = 0; i < count; i++)
            {
                CreateSlot();
            }
        }
    }

    void CreateSlot() // 슬롯 1번 생성후 구매버튼 위치 이동
    {
        GameObject newSlot = Instantiate(_slotUI, slotHolder);
        int location = slotHolder.childCount - 2;
        newSlot.transform.SetSiblingIndex(location);
    }

    #region "Button"

    public void OnClickBuyAddSlot() // 가방 칸 생성
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Click);
        if(GameManager.instance.Gold < 50)
        {
            return;
        }
        GameManager.instance.Gold -= 50;
        Inventory.SlotCnt++;
        CreateSlot();
        LobbyManager.instance.mainMenu.LoadHaveGold();
    }

    #endregion
}
