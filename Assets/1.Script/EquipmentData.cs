using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
    1.장비는 등급과 부위가 정해져있음

    2.등급에따라 강화 횟수와 옵션 갯수가 달라짐
    2-1.등급에따라 옵션의 스테이터스 증가량이 달라짐
        ex) Common: HP+5, UnCommon: HP+10, Rare: HP+25, Unique: HP+50, Legendary: HP+100
    2-2.등급에따라 생성될때의 출현 옵션 갯수가 다름
        Common: 기본 1개, 50%로 2개
        UnCommon: 기본 2개, 33%로 3개
        Rare: 기본 2개, 50%로 3개
        Unique: 기본 3개, 33%로 4개
        Legendary: 기본 4개, 25%로 5개

    3.부위에따라 옵션에 출현하는 능력치가 다름

    * _maxLevel과 _maxObtionCount 값
    Common, UnCommon = 3
    Rare, Unique = 4
    Legendary = 5
*/
[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Object/Equipment", order = 1)]
public class EquipmentData : ScriptableObject // Name, Grade, Part, Sprite만 넣고 CreateEquip 실행시 나머지 옵션은 자동 생성
{
    [Header("# Main Data")]
    public string Name; // 장비 이름
    public EquipGrade Grade; // 장비 등급
    public EquipPart Part; // 장비 부위
    public Sprite Sprite; // 장비의 이미지
    int _maxLevel; // 장비의 최고 레벨(Grade에따라 1~5)
    public int EquipLevel; // 장비의 레벨
    public List<int> UpgradeCost; // 강화에 필요한 골드

    [Header("# Equip Obtion")]
    int _maxObtionCount; // 옵션 최대갯수
    public List<StatusEnum> Options; // 옵션 배열
    public List<int> OptionUpgradeCounts; // 배열에 있는 n번째 옵션이 몇번 강화됐는지 표시
    public List<float> OptionsValue; // 옵션의 수치

    [Header("# Curse")]
    bool isCurse;
    float _curseValue;

    public void CreateEquip()
    {
        SettingEquip();
    }

    void SettingEquip()
    {
        SettingCursedItem();
        (_maxLevel, _maxObtionCount) = SetMaxLevelAndStatusCount(Grade);
        OptionsValue = new List<float>(Enumerable.Repeat(0f, _maxObtionCount));
        SetRandomOption();
        SettingUpgradeCost();
    }

    void SettingCursedItem() // 10% 확률로 저주 아이템 설정
    {
        float[] curseValues = { 2.5f, 5f, 7.5f, 10f, 12.5f };
        int randomPer = Random.Range(0, 100);

        if(randomPer < 10)
        {
            isCurse = true;
            _curseValue = curseValues[(int)Grade];
            Name = "저주받은 " + Name;
        }
    }

    (int maxLevel, int maxObtionCount) SetMaxLevelAndStatusCount(EquipGrade grade) // 등급에따라 최대레벨과 최대옵션수 설정
    {
        return grade switch
        {
            EquipGrade.Rare or EquipGrade.Unique => (4, 4),
            EquipGrade.Legendary => (5, 5),
            _ => (3, 3) // _는 Default로 동작(Common과 UnCommon) 
        };
    }

    void SetRandomOption() // 아이템 생성시 옵션 초기 생성 갯수만큼 랜덤 옵션 부여
    {
        int[] baseOptionCounts = { 1, 2, 2, 3, 4 }; // 등급에따른 기본 옵션 생성 갯수
        int[] addOptionPer = { 50, 33, 50, 33, 25 }; // 등급에따른 추가 옵션 생성 확률
        int optionCount = baseOptionCounts[(int)Grade]; // 생성해야할 옵션 갯수
        
        int random = Random.Range(0, 100);
        if(random < addOptionPer[(int)Grade])
        {
            optionCount++;
        }

        GenerateOptions(optionCount);
    }

    void GenerateOptions(int generateCount) // 옵션 부여하고 List에 추가
    {
        List<StatusEnum> appearStatus = new List<StatusEnum> { StatusEnum.Hp, StatusEnum.AttackPower, StatusEnum.Defense,
                                                               StatusEnum.ObtainRange, StatusEnum.CriticalChance, StatusEnum.CriticalDamage }; // 무조건 출현하는 옵션
        switch(Part) // 부위별 추가 등장 옵션 설정
        {
            case EquipPart.Hat:
                appearStatus.Add(StatusEnum.CoolTime);
                appearStatus.Add(StatusEnum.Duration);
                break;
            case EquipPart.Armor:
                appearStatus.Add(StatusEnum.AttackRange);
                appearStatus.Add(StatusEnum.ProjectileCount);
                break;
            case EquipPart.Leggings:
                appearStatus.Add(StatusEnum.ProjectileSpeed);
                appearStatus.Add(StatusEnum.ProjectileSize);
                break;
            case EquipPart.Boots:
                appearStatus.Add(StatusEnum.MoveSpeed);
                break;
        }

        for(int i=0; i < generateCount; i++)
        {
            int random = Random.Range(0, appearStatus.Count);
            Options.Add(appearStatus[random]);
            OptionUpgradeCounts.Add(0);
            OptionsValue.Add(SetOptionsValue(appearStatus[random], i));
        }
    }

    float SetOptionsValue(StatusEnum option, int optionNum) // Option에따라 OptionValue 추가
    {
        float result = OptionsValue[optionNum];
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

    void SettingUpgradeCost() // 강화비용 설정
    {
        UpgradeCost = new List<int>();
        int[] costs = { 500, 750, 1250, 1500, 2000 };
        SettingUpgradeCostLoop(costs[(int)Grade]);
    }

    void SettingUpgradeCostLoop(int costIncrement) // 강화 비용 자동 기입
    {
        int cost = 0;
        for(int i=0; i < _maxLevel; i++)
        {
            cost += costIncrement;
            UpgradeCost.Add(cost);
        }
    }

    public void UpgradeEquip() // 강화
    {
        if(EquipLevel == _maxLevel || GameManager.instance.Gold < UpgradeCost[EquipLevel])
            return;

        GameManager.instance.Gold -= UpgradeCost[EquipLevel];
        EquipLevel++;

        if(_maxObtionCount > Options.Count)
        {
            GenerateOptions(1);
        }
        else
        {
            UpgradeOption();
        }
    }

    void UpgradeOption() // 강화로인한 옵션 업그레이드 발생시 호출
    {
        // OptionTypes에서 랜덤으로 하나 가져온후 업그레이드
        int randomNum = Random.Range(0, Options.Count);
        StatusEnum target = Options[randomNum];

        OptionUpgradeCounts[randomNum]++;
        OptionsValue[randomNum] = SetOptionsValue(target, randomNum);
    }
}
