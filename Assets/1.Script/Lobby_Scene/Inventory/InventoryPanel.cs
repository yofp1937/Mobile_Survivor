using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryPanel : MonoBehaviour
{
    [Header("# Main Data")]
    [SerializeField] GameObject _slotUI; // 인벤토리 Slot 프리팹
    [SerializeField] Transform _slotHolder; // _slots의 부모 객체
    [SerializeField] List<InventorySlot> _slots;

    void Awake()
    {
        Init();
    }

    void Init() // 플레이어의 가방 칸 초기생성
    {
        int count = GameManager.instance.InventoryManager.SlotCnt - _slots.Count; // 플레이어의 최대 슬롯 - 생성돼있는 슬롯

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
        GameObject newSlot = Instantiate(_slotUI, _slotHolder);
        int location = _slotHolder.childCount - 2;
        newSlot.transform.SetSiblingIndex(location);
        _slots.Add(newSlot.GetComponent<InventorySlot>());
    }

    void ReloadSlot()
    {
        foreach(var slot in _slots)
        {
            slot.Reload();
        }
    }

    public void LoadEquipPanelInInvetorySlot() // _slots 내부의 데이터 리셋하고 재설정
    {
        ReloadSlot();
        int equipCount = GameManager.instance.InventoryManager.Equips.Count;
        int setCount = 0;

        foreach(var slot in _slots)
        {
            if(setCount >= equipCount) break;
            if(slot.Data == null)
            {
                slot.SetItemSlot(GameManager.instance.InventoryManager.Equips[setCount]);
                setCount++;
            }
        }
    }

    #region "Button"
    public void OnClickInventoryPanel()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        gameObject.SetActive(true);
    }
    public void OnClickExit()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        gameObject.SetActive(false);
    }
    public void OnClickBuyAddSlot() // 가방 칸 생성
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        if(GameManager.instance.Gold < 50)
        {
            return;
        }
        GameManager.instance.Gold -= 50;
        GameManager.instance.InventoryManager.SlotCnt++;
        CreateSlot();
        LobbyManager.instance.LoadHaveGold();
    }
    #endregion
}
