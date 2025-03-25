using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    [Header("# Constant Data")]
    [SerializeField] public string GUID;
    [SerializeField] string equipName;
    public string EquipName
    {
        get => equipName;
        private set => equipName = value;
    }
    EquipGrade grade; // 장비 등급
    public EquipGrade Grade
    {
        get => grade;
        private set => grade = value;
    }
    EquipPart part; // 장비 부위
    public EquipPart Part
    {
        get => part;
        private set => part = value;
    }
    Sprite sprite; // 장비의 이미지
    public Sprite Sprite
    {
        get => sprite;
        private set => sprite = value;
    }
    int maxLevel;
    public int MaxLevel // 장비의 최고 레벨(Grade에따라 1~5)
    {
        get => maxLevel;
        private set => maxLevel = value;
    }

    [Header("# Main Data")]
    public int EquipLevel; // 장비의 레벨
    public int[] UpgradeCost; // 강화에 필요한 골드
    public int SellCost; // 판매금

    [Header("# Option Data")]
    int _maxObtionCount; // 옵션 최대갯수
    public List<StatusEnum> Options = new List<StatusEnum>(); // 옵션 종류
    public List<int> OptionUpgradeCounts = new List<int>(); // n번째 옵션이 몇번 강화됐는지 표시
    public List<float> OptionsValue = new List<float>(); // n번째 옵션의 값(Options[0]이 HP + 50 두번이면 OptionsValue[0]은 100)
    public bool IsCurse;
    public float CurseValue;

    [Header("# State Data")]
    public bool IsEquip;

    public void CreateEquip(EquipmentData data) // data를 기반으로 새로운 장비 객체 생성
    {
        SetGUID();
        Grade = data.Grade;
        Part = data.Part;
        Sprite = data.Sprite;
        EquipName = data.Name;
        SettingOptions();
        CreateInDataBase();
    }

    void SetGUID()
    {
        GUID = Guid.NewGuid().ToString();
    }

    void SettingOptions()
    {
        SettingCursedEquip();
        SetMaxLevelAndStatusCount();
        SetRandomOption();
        SettingUpgradeCost();
    }

    void CreateInDataBase()
    {
        EquipmentDataClass data = new EquipmentDataClass(Grade, Part, EquipLevel, Options, OptionUpgradeCounts, IsCurse, IsEquip);
        string json = JsonConvert.SerializeObject(data);
        Debug.Log("CreateInDataBase-json:" + json);
        DBManager.instance.CreateEquipInDB(GUID, json);
    }

    void SettingCursedEquip() // 10% 확률로 저주 아이템 설정
    {
        float[] curseValues = { 0.025f, 0.05f, 0.075f, 0.1f, 0.125f };
        int randomPer = UnityEngine.Random.Range(0, 100);

        if(randomPer < 10)
        {
            IsCurse = true;
            CurseValue = curseValues[(int)Grade];
            EquipName = "저주받은 " + EquipName;
        }
    }

    void SetMaxLevelAndStatusCount() // 등급에따라 최대레벨과 최대옵션수 정하고 배열 크기 설정
    {
        switch(Grade)
        {
            case EquipGrade.Rare:
            case EquipGrade.Unique:
                MaxLevel = 4;
                _maxObtionCount = 4;
                break;
            case EquipGrade.Legendary:
                MaxLevel = 5;
                _maxObtionCount = 5;
                break;
            default:
                MaxLevel = 3;
                _maxObtionCount = 3;
                break;
        }
        UpgradeCost = new int[MaxLevel];
    }

    void SetRandomOption() // 아이템 생성시 옵션 초기 생성 갯수만큼 랜덤 옵션 부여
    {
        int[] baseOptionCounts = { 1, 2, 2, 3, 4 }; // 등급에따른 기본 옵션 생성 갯수
        int[] addOptionPer = { 50, 33, 50, 33, 25 }; // 등급에따른 추가 옵션 생성 확률
        int optionCount = baseOptionCounts[(int)Grade]; // 생성해야할 옵션 갯수
        
        if(UnityEngine.Random.Range(0, 100) < addOptionPer[(int)Grade])
        {
            optionCount++;
        }

        GenerateOptions(optionCount);
    }

    void GenerateOptions(int generateCount) // 옵션 부여하고 List에 추가
    {
        List<StatusEnum> appearOptions = new() // 무조건 출현하는 옵션
        {
            StatusEnum.Hp, StatusEnum.AttackPower, StatusEnum.Defense,
            StatusEnum.ObtainRange, StatusEnum.CriticalChance, StatusEnum.CriticalDamage
        };
        
        switch(Part) // 부위별 추가 등장 옵션 설정
        {
            case EquipPart.Hat:
                appearOptions.Add(StatusEnum.CoolTime);
                appearOptions.Add(StatusEnum.Duration);
                break;
            case EquipPart.Armor:
                appearOptions.Add(StatusEnum.AttackRange);
                appearOptions.Add(StatusEnum.ProjectileCount);
                break;
            case EquipPart.Ring:
                appearOptions.Add(StatusEnum.ProjectileSpeed);
                appearOptions.Add(StatusEnum.ProjectileSize);
                break;
            case EquipPart.Necklace:
                appearOptions.Add(StatusEnum.MoveSpeed);
                break;
        }

        List<StatusEnum> filteredOptions = appearOptions.Except(Options).ToList();

        for(int i = 0; i < generateCount; i++)
        {
            int random = UnityEngine.Random.Range(0, filteredOptions.Count);
            StatusEnum selectedOption = filteredOptions[random];

            Options.Add(selectedOption);
            OptionUpgradeCounts.Add(0);
            OptionsValue.Add(SetOptionsValue(selectedOption));

            filteredOptions.RemoveAt(random);
        }
    }

    float SetOptionsValue(StatusEnum option) // Option에따라 OptionValue 추가
    {
        float result = 0;
        float[] Hp = { 20, 50, 100, 250, 500 };
        float[] AttackPower = { 70, 140, 125, 200, 265 };
        float[] Defense = { 1, 2, 3, 4, 5 };
        float[] VariousValue = { 0.1f, 0.1f, 0.15f, 0.15f, 0.2f }; // AttackRange, ObtainRange, ProjectileSize, ProjectileSpeed, MoveSpeed
        float[] CriticalChance = { 1.5f, 3f, 4.5f, 6f, 7.5f };
        float[] CriticalDamage = { 3, 6, 9, 12, 15 };
        float[] CoolTimeAndDuration = { 0.01f, 0.02f, 0.03f, 0.04f, 0.05f };
        int ProjectileCount = 1;

        switch(option)
        {
            case StatusEnum.Hp:
                result += Hp[(int)Grade];
                break;
            case StatusEnum.AttackPower:
                result += AttackPower[(int)Grade];
                break;
            case StatusEnum.Defense:
                result += Defense[(int)Grade];
                break;
            case StatusEnum.AttackRange:
            case StatusEnum.ObtainRange:
            case StatusEnum.ProjectileSize:
            case StatusEnum.ProjectileSpeed:
            case StatusEnum.MoveSpeed:
                result += VariousValue[(int)Grade];
                break;
            case StatusEnum.CriticalChance:
                result += CriticalChance[(int)Grade];
                break;
            case StatusEnum.CriticalDamage:
                result += CriticalDamage[(int)Grade];
                break;
            case StatusEnum.CoolTime:
            case StatusEnum.Duration:
                result += CoolTimeAndDuration[(int)Grade];
                break;
            case StatusEnum.ProjectileCount:
                result += ProjectileCount;
                break;
        }

        return result;
    }

    void SettingUpgradeCost() // 강화비용, 판매금 설정
    {
        int[] needCosts = { 500, 750, 1250, 1500, 2000 };

        int cost = 0;
        for(int i=0; i < MaxLevel; i++)
        {
            cost += needCosts[(int)Grade];
            UpgradeCost[i] = cost;
        }
        SellCost = needCosts[(int)Grade];
    }

    public void UpgradeEquip() // 강화
    {
        if(EquipLevel == MaxLevel || GameManager.instance.Gold < UpgradeCost[EquipLevel])
            return;

        if(IsEquip) SubEquipStatus();

        GameManager.instance.Gold -= UpgradeCost[EquipLevel];
        EquipLevel++;

        if(_maxObtionCount > Options.Count)
        {
            GenerateOptions(1);
            // DB에 EquipLevel, Options, OptionUpgradeCounts 세개 전송
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["EquipLevel"] = EquipLevel;
            dict["Options"] = Options;
            dict["OptionUpgradeCounts"] = OptionUpgradeCounts;
            DBManager.instance.UpdateEquipInDB(GUID, dict);
        }
        else
        {
            UpgradeOption();
            // DB에 EquipLevel, OptionUpgradeCounts 두개 전송
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict["EquipLevel"] = EquipLevel;
            dict["OptionUpgradeCounts"] = OptionUpgradeCounts;
            DBManager.instance.UpdateEquipInDB(GUID, dict);
        }

        if(IsEquip) AddEquipStatus();
    }

    public void AddEquipStatus()
    {
        if(IsCurse) GameManager.instance.StatusManager.AddEquipStatus(StatusEnum.Curse, CurseValue);
        for(int i = 0; i < Options.Count; i++)
        {
            GameManager.instance.StatusManager.AddEquipStatus(Options[i], OptionsValue[i] * (OptionUpgradeCounts[i] + 1));
        }
    }

    public void SubEquipStatus()
    {
        if(IsCurse) GameManager.instance.StatusManager.SubEquipStatus(StatusEnum.Curse, CurseValue);
        for(int i = 0; i < Options.Count; i++)
        {
            GameManager.instance.StatusManager.SubEquipStatus(Options[i], OptionsValue[i] * (OptionUpgradeCounts[i] + 1));
        }
    }

    void UpgradeOption() // 강화로인한 옵션 업그레이드 발생시 호출
    {
        int randomNum = UnityEngine.Random.Range(0, Options.Count);
        StatusEnum target = Options[randomNum];

        OptionUpgradeCounts[randomNum]++;
        OptionsValue[randomNum] = SetOptionsValue(target);
    }

    public string GetOptionString(StatusEnum status)
    {
        string result = null;

        switch(status)
        {
            case StatusEnum.Hp:
                result = "체력";
                break;
            case StatusEnum.AttackPower:
                result = "공격력";
                break;
            case StatusEnum.Defense:
                result = "방어력";
                break;
            case StatusEnum.MoveSpeed:
                result = "이동속도";
                break;
            case StatusEnum.ProjectileCount:
                result = "투사체 개수";
                break;
            case StatusEnum.ProjectileSpeed:
                result = "투사체 속도";
                break;
            case StatusEnum.ProjectileSize:
                result = "투사체 크기";
                break;
            case StatusEnum.CoolTime:
                result = "쿨타임";
                break;
            case StatusEnum.Duration:
                result = "지속시간";
                break;
            case StatusEnum.AttackRange:
                result = "공격범위";
                break;
            case StatusEnum.ObtainRange:
                result = "획득범위";
                break;
            case StatusEnum.CriticalChance:
                result = "치명타 확률";
                break;
            case StatusEnum.CriticalDamage:
                result = "치명타 데미지";
                break;
        }

        return result;
    }

    public void SetInfo(Dictionary<string, object> data) // DB에서 읽어온 장비 생성하는 함수
    {
        // Grade, Part, Sprite 설정
        int grade = Convert.ToInt32(data["EquipGrade"]);
        Grade = (EquipGrade)grade;
        int part = Convert.ToInt32(data["EquipPart"]);
        Part = (EquipPart)part;
        gameObject.name = Grade.ToString() + "_" + Part.ToString();
        Image img = gameObject.AddComponent<Image>();
        switch(Grade)
        {
            case EquipGrade.Common:
                Sprite = GameManager.instance.InventoryManager.Common[(int)Part].Sprite;
                img.sprite = Sprite;
                equipName = GameManager.instance.InventoryManager.Common[(int)Part].Name;
                break;
            case EquipGrade.UnCommon:
                Sprite = GameManager.instance.InventoryManager.UnCommon[(int)Part].Sprite;
                img.sprite = Sprite;
                equipName = GameManager.instance.InventoryManager.UnCommon[(int)Part].Name;
                break;
            case EquipGrade.Rare:
                Sprite = GameManager.instance.InventoryManager.Rare[(int)Part].Sprite;
                img.sprite = Sprite;
                equipName = GameManager.instance.InventoryManager.Rare[(int)Part].Name;
                break;
            case EquipGrade.Unique:
                Sprite = GameManager.instance.InventoryManager.Unique[(int)Part].Sprite;
                img.sprite = Sprite;
                equipName = GameManager.instance.InventoryManager.Unique[(int)Part].Name;
                break;
            case EquipGrade.Legendary:
                Sprite = GameManager.instance.InventoryManager.Legendary[(int)Part].Sprite;
                img.sprite = Sprite;
                equipName = GameManager.instance.InventoryManager.Legendary[(int)Part].Name;
                break;
        }

        // MaxLevel, MaxOptionCount 설정
        SetMaxLevelAndStatusCount();

        // UpgradeCost, SellCost 설정
        SettingUpgradeCost();

        // EquipLevel 설정
        EquipLevel = Convert.ToInt32(data["EquipLevel"]);

        // Options Data 설정
        List<int> options = ((JArray)data["Options"]).ToObject<List<int>>();
        foreach(int op in options)
        {
            Options.Add((StatusEnum)op);
            OptionsValue.Add(SetOptionsValue((StatusEnum)op));
        }
        List<int> levels = ((JArray)data["OptionUpgradeCounts"]).ToObject<List<int>>();
        foreach(int lev in levels)
        {
            OptionUpgradeCounts.Add(lev);
        }

        // Curse 설정
        IsCurse = Convert.ToBoolean(data["IsCurse"]);
        if(IsCurse)
        {
            float[] curseValues = { 0.025f, 0.05f, 0.075f, 0.1f, 0.125f };
            CurseValue = curseValues[(int)Grade];
            equipName = "저주받은 " + equipName;
        }

        // Equip 설정
        IsEquip = Convert.ToBoolean(data["IsEquip"]);
        if(IsEquip)
        {
            GameManager.instance.InventoryManager.EquippedEquips.Add(gameObject);
        }
        else
        {
            GameManager.instance.InventoryManager.Equips.Add(gameObject);
        }
    }
}
