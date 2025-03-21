using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [Header("# Inventory Data")]
    [SerializeField] GameObject _slotUI; // 인벤토리 Slot 프리팹
    [SerializeField] Transform _slotHolder; // _slots의 부모 객체
    [SerializeField] List<InventorySlot> _slots; // 인벤토리 공간
    [SerializeField] List<InventorySlot> _equipSlots; // 장비 장착 공간

    [Header("# Player Select")]
    [SerializeField] InventorySlot _selectSlot; // 선택한 슬롯
    [SerializeField] GameObject _selectObject; // 선택한 슬롯의 장비 오브젝트
    [SerializeField] Equipment _selectEquipData; // 선택한 장비의 Equipment 컴포넌트 데이터

    [Header("# Inventory Panel")]
    [SerializeField] Text _upgradeGoldText; // 강화 비용 텍스트
    [SerializeField] Button _upgradeBtn; // 강화 버튼
    [SerializeField] Text _sellGoldText; // 판매금 텍스트
    [SerializeField] Button _sellBtn; // 판매 버튼
    [SerializeField] Button _equipBtn; // 장착 버튼

    [Header("# Status Panel")]
    [SerializeField] InventorySlot _statusSlot; // 스테이터스창 슬롯
    [SerializeField] Text _nameText; // 이름 텍스트들
    [SerializeField] List<GameObject> _textGroup; // 옵션, 값 텍스트 그룹

    [Header("# Sell Panel")]
    [SerializeField] GameObject _sellPanel;
    [SerializeField] InventorySlot _sellSlot;
    [SerializeField] Text _sellText;

    void Awake()
    {
        Init();
        _sellPanel.SetActive(false);
        _upgradeBtn.interactable = false;
        _sellBtn.interactable = false;
        _equipBtn.interactable = false;
        ResetEquipStatus();
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
        newSlot.GetComponent<Button>().onClick.AddListener(OnClickSlot);
        int location = _slotHolder.childCount - 2;
        newSlot.transform.SetSiblingIndex(location);
        _slots.Add(newSlot.GetComponent<InventorySlot>());
    }

    void ResetSlot() // 모든 InventorySlot 데이터 초기화
    {
        foreach(var slot in _slots)
        {
            slot.ResetData();
        }
        foreach(var slot in _equipSlots)
        {
            slot.ResetData();
        }
    }

    public void LoadEquipmentInInvetorySlot() // _slots 내부의 데이터 리셋하고 재설정
    {
        GameManager.instance.InventoryManager.DeleteNullInEquips();
        ResetSlot();
        foreach(GameObject equip in GameManager.instance.InventoryManager.EquippedEquips) // 장착중인 아이템 세팅
        {
            Equipment data = equip.GetComponent<Equipment>();
            _equipSlots[(int)data.Part].SetItemSlot(equip);
        }
        foreach(GameObject equip in GameManager.instance.InventoryManager.Equips) // 미장착 아이템 세팅
        {
            Equipment data = equip.GetComponent<Equipment>();
            InventorySlot slot = GetNullInventorySlot();
            slot.SetItemSlot(equip);
        }
    }

    InventorySlot GetNullInventorySlot() // 비어있는 Slot 반환
    {
        InventorySlot result = null;
        foreach(var slot in _slots)
        {
            if(slot.Data == null)
            {
                result = slot;
                break;
            }
        }
        return result;
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
    }
    public void OnClickSlot() // 슬롯 버튼을 클릭했을때 - Slot에 데이터가 있으면 _upgradeGoldText와 _equipBtn 세팅
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        ResetEquipStatus();
        // 선택한 슬롯과 내부 데이터를 임시저장
        _selectSlot = EventSystem.current.currentSelectedGameObject.GetComponent<InventorySlot>();
        if(_selectSlot.Data == null) return;
        _selectObject = _selectSlot.Data;
        _selectEquipData = _selectObject.GetComponent<Equipment>();

        // Status 세팅
        SetEquipStatus();

        // 강화 골드 세팅
        _upgradeBtn.interactable = true;
        SetUpgradeCost();

        // 판매 세팅
        _sellBtn.interactable = true;
        SetSellCost();

        // 장착 버튼 세팅
        _equipBtn.interactable = true;
        ChangeEquipBtnText(_selectEquipData.IsEquip);
    }
    public void OnClickUpgradeBtn() // 강화 버튼을 클릭했을때
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        _selectEquipData.UpgradeEquip();
        _selectSlot.SetLevelText();
        _statusSlot.SetLevelText();
        SetUpgradeCost();
        SetEquipStatusText();
        _selectEquipData = _selectObject.GetComponent<Equipment>();
    }
    public void OnClickSellPanel() // 판매 버튼을 클릭했을때 등급이 높으면 Panel 염
    {
        if(_selectEquipData.Grade == EquipGrade.Legendary || _selectEquipData.Grade == EquipGrade.Unique)
        {
            AudioManager.instance.PlaySfx(Sfx.Click);
            _sellSlot.SetItemSlot(_selectObject);
            _sellText.text = _selectEquipData.EquipName;
            _sellPanel.SetActive(true);
        }
        else
        {
            OnClickSellBtn();
        }
    }
    public void OnClickSellBtn() // 판매하면 파괴하고 인벤토리 재정렬
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        ResetSellPanel();
        GameManager.instance.Gold += _selectEquipData.SellCost;
        _selectEquipData.SubEquipStatus();
        StartCoroutine(WaitDestroying(_selectObject));
    }
    public void OnClickSellCancleBtn()
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        ResetSellPanel();
    }
    public void OnClickEquipBtn() // 장착 or 해제 버튼을 클릭했을때
    {
        AudioManager.instance.PlaySfx(Sfx.Click);
        if(_selectEquipData.IsEquip) // 해제 버튼이면
        {
            MoveEquippedEquipment(_selectObject);
        }
        else // 장착 안돼있으면
        {
            EquippedEquipment(); // 장착 버튼이면
        }
        ResetSelectData();
        ResetEquipStatus();
    }
    // 이 아래론 Btn region 내부에서 쓰이는 함수
    void SetEquipStatus() // 선택한 장비의 옵션을 Status Panel에 표시
    {
        // 슬롯 설정
        _statusSlot.SetItemSlot(_selectObject);

        // 이름과 옵션 설정
        SetEquipStatusText();
    }
    void SetEquipStatusText() // 이름과 옵션 설정
    {
        // 이름 표시
        _nameText.gameObject.SetActive(true);
        _nameText.text = _selectEquipData.EquipName;

        for(int i = 0; i < _selectEquipData.Options.Count; i++) // 옵션 증가량 표시
        {
            _textGroup[i].SetActive(true);

            List<Text> valueTexts = _textGroup[i].GetComponentsInChildren<Text>().ToList();
            string upgradeLevel = _selectEquipData.OptionUpgradeCounts[i] > 0 ? $"(+{_selectEquipData.OptionUpgradeCounts[i]})" : "" ;
            valueTexts[0].text = _selectEquipData.GetOptionString(_selectEquipData.Options[i]) + upgradeLevel;

            string per = null;
            float statusValue = _selectEquipData.OptionsValue[i] * (_selectEquipData.OptionUpgradeCounts[i] + 1);
            if(statusValue < 1)
            {
                statusValue *= 100;
                per = "%";
            }
            else if(_selectEquipData.Options[i] == StatusEnum.Defense ||
                    _selectEquipData.Options[i] == StatusEnum.CriticalChance ||
                    _selectEquipData.Options[i] == StatusEnum.CriticalDamage)
            {
                per = "%";
            }
            if(_selectEquipData.Options[i] == StatusEnum.MoveSpeed) per = "";
            valueTexts[1].text = "+" + statusValue + per;
        }

        // 텍스트 컬러 설정
        SetTextColr();
    }
    void ResetEquipStatus() // Status Panel 초기화
    {
        _statusSlot.ResetData();
        _nameText.gameObject.SetActive(false);

        // 옵션 초기화
        foreach(var group in _textGroup)
        {
            group.SetActive(false);
        }
    }
    void SetTextColr() // 등급에 맞게 텍스트 컬러 변환
    {
        Color32 color = new Color32(139, 245, 255, 255); // 흔한 장비 색
        switch(_selectEquipData.Grade)
        {
            case EquipGrade.UnCommon:
                color = new Color32(113, 229, 100, 255);
                break;
            case EquipGrade.Rare:
                color = new Color32(36, 90, 182, 255);
                break;
            case EquipGrade.Unique:
                color = new Color32(196, 9, 177, 255);
                break;
            case EquipGrade.Legendary:
                color = new Color32(255, 250, 21, 255);
                break;
        }
        // 이름 텍스트
        _nameText.color = color;

        // 옵션 텍스트
        foreach(GameObject obj in _textGroup)
        {
            List<Text> texts = obj.GetComponentsInChildren<Text>().ToList();
            foreach(Text text in texts)
            {
                text.color = color;
            }
        }
    }
    void SetUpgradeCost() // 강화 비용 설정
    {
        if(_selectEquipData.MaxLevel == _selectEquipData.EquipLevel)
        {
            _upgradeGoldText.text = "";
            _upgradeBtn.interactable = false;
            return;
        }
        _upgradeGoldText.text = _selectEquipData.UpgradeCost[_selectEquipData.EquipLevel].ToString();
    }
    void SetSellCost() // 판매금 설정
    {
        _sellGoldText.text = _selectEquipData.SellCost.ToString();
    }
    void ChangeEquipBtnText(bool isEquip) // 아이템의 IsEquip에따라 _equipBtn의 텍스트 변경
    {
        Text btnText = _equipBtn.transform.Find("Text").GetComponent<Text>();
        if(isEquip)
        {
            btnText.text = "해제";
            btnText.color = new Color32(107, 107, 107, 255);
        }
        else
        {
            btnText.text = "장착";
            btnText.color = new Color32(1, 141, 0, 255);
        }
    }
    void MoveEquippedEquipment(GameObject obj) // 장착된 장비를 _slots로 이동(장비 해제)
    {
        Equipment data = obj.GetComponent<Equipment>();
        // 인벤토리 남은 공간 검사
        InventorySlot inven = GetNullInventorySlot();
        if(inven == null) return;

        // 장비칸에서 아이템 제거후 인벤토리로 이동
        _equipSlots[(int)data.Part].ResetData();
        inven.SetItemSlot(obj);
        data.IsEquip = false;
        GameManager.instance.InventoryManager.EquippedEquips.Remove(obj);
        GameManager.instance.InventoryManager.Equips.Add(obj);

        // Status 반영
        data.SubEquipStatus();

        ChangeEquipBtnText(_selectEquipData.IsEquip);
    }
    void EquippedEquipment() // 장비를 장착(이미 장착중이면 빼고 새로 장착)
    {
        if(_equipSlots[(int)_selectEquipData.Part].Data != null) // 이미 해당 슬롯에 장비가 장착중이면 장비를 인벤토리로 이동
        {
            _selectSlot.ResetData(); // 내 장비공간 1칸 마련(인벤토리 꽉찼을때 오류 안나게)
            MoveEquippedEquipment(_equipSlots[(int)_selectEquipData.Part].Data);
            _selectSlot = null;
        }

        // 장비 장착
        _equipSlots[(int)_selectEquipData.Part].SetItemSlot(_selectObject);
        _selectEquipData.IsEquip = true;
        GameManager.instance.InventoryManager.Equips.Remove(_selectObject);
        GameManager.instance.InventoryManager.EquippedEquips.Add(_selectObject);
        if(_selectSlot != null) _selectSlot.ResetData();

        // Status 반영
        _selectEquipData.AddEquipStatus();
    }
    void ResetSellPanel()
    {
        _sellSlot.ResetData();
        _sellText.text = "";
        _sellPanel.SetActive(false);
    }
    IEnumerator WaitDestroying(GameObject obj) // Destroy는 작동에 1프레임이 소모돼서 실행후 1프레임 대기해주는 코루틴
    {
        Destroy(obj);
        yield return null;
        ResetSelectData();
        ResetEquipStatus();
        LoadEquipmentInInvetorySlot();
    }
    void ResetSelectData()
    {
        _selectSlot = null;
        _selectObject = null;
        _selectEquipData = null;
        _upgradeGoldText.text = "";
        _upgradeBtn.interactable = false;
        _sellGoldText.text = "";
        _sellBtn.interactable = false;
        _equipBtn.interactable = false;
    }
    #endregion
}
