using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : EnemyBase
{
    [Header("# Main Data")]
    public int BossNum;

    [Header("# Drop Equip Data")]
    EquipGrade _dropEquipGrade;
    EquipPart _dropEquipPart;

    protected override void Dead()
    {
        DropEquip();
        gameObject.SetActive(false);
    }

    void DropEquip() // EquipmentData 들고있는 아이템을 드랍함
    {
        CheckDropEquipGrade();
        CheckDropEquipPart();

        EquipmentData Equip = InGameManager.instance.EquipmentManager.GetDropEquipData(_dropEquipGrade, _dropEquipPart);
        Transform obj = InGameManager.instance.PoolManager.Get(PoolEnum.Equipment, out bool isNew).transform;
        obj.position = gameObject.transform.position;
        obj.parent = InGameManager.instance.PoolManager.transform.Find("Item");
        obj.GetComponent<DropItem>().Init(DropItemEnum.Equipment);
        obj.GetComponent<DropItem>().EquipmentData = Equip;
        obj.GetComponent<SpriteRenderer>().sprite = Equip.Sprite;
    }

    void CheckDropEquipGrade() // 난이도와 보스 레벨에따라 장비의 등급 결정
    {
        DifficultyLevels difficult = GameManager.instance.DifficultyLevel;
        float random = Random.Range(0, 100);
        
        Dictionary<(DifficultyLevels, int), List<(float, EquipGrade)>> dropChance = new()
        {
            // Normal 난이도
            { (DifficultyLevels.Normal, 0), new() { (100, EquipGrade.Common) } },
            { (DifficultyLevels.Normal, 1), new() { (100, EquipGrade.Common) } },
            { (DifficultyLevels.Normal, 2), new() { (75, EquipGrade.Common), (100, EquipGrade.UnCommon) } },

            // Hard 난이도
            { (DifficultyLevels.Hard, 0), new() { (75, EquipGrade.Common), (100, EquipGrade.UnCommon) } },
            { (DifficultyLevels.Hard, 1), new() { (50, EquipGrade.Common), (100, EquipGrade.UnCommon) } },
            { (DifficultyLevels.Hard, 2), new() { (37.5f, EquipGrade.Common), (75, EquipGrade.UnCommon), (100, EquipGrade.Rare) } },

            // Hell 난이도
            { (DifficultyLevels.Hell, 0), new() { (75, EquipGrade.UnCommon), (100, EquipGrade.Rare) } },
            { (DifficultyLevels.Hell, 1), new() { (37.5f, EquipGrade.UnCommon), (75, EquipGrade.Rare), (100, EquipGrade.Unique) } },
            { (DifficultyLevels.Hell, 2), new() { (25, EquipGrade.UnCommon), (75, EquipGrade.Rare), (100, EquipGrade.Unique) } },

            // God 난이도
            { (DifficultyLevels.God, 0), new() { (75, EquipGrade.Rare), (100, EquipGrade.Unique) } },
            { (DifficultyLevels.God, 1), new() { (50, EquipGrade.Rare), (100, EquipGrade.Unique) } },
            { (DifficultyLevels.God, 2), new() { (25, EquipGrade.Rare), (100, EquipGrade.Unique) } },

            // Nightmare 난이도
            { (DifficultyLevels.Nightmare, 0), new() { (100, EquipGrade.Unique) } },
            { (DifficultyLevels.Nightmare, 1), new() { (85, EquipGrade.Unique), (100, EquipGrade.Legendary) } },
            { (DifficultyLevels.Nightmare, 2), new() { (70, EquipGrade.Unique), (100, EquipGrade.Legendary) } },
        };

        if(dropChance.TryGetValue((difficult, BossNum), out var chances)) // 위 Dictionary에서 difficult, BossNum의 값과 일치하는 List를 chances에 넣음
        {
            foreach(var (threshold, grade) in chances) // chances 내의 값을 반복하면서 random과 비교
            {
                if(random < threshold) // 비교하여 일치하면 Grade 설정 후 함수 종료
                {
                    _dropEquipGrade = grade;
                    return;
                }
            }
        }
    }

    void CheckDropEquipPart() // 장비의 부위 결정
    {
        int random = Random.Range(0, 100);
        _dropEquipPart = random < 25 ? EquipPart.Hat :
                         random < 50 ? EquipPart.Armor :
                         random < 75 ? EquipPart.Ring : EquipPart.Necklace;
    }
}
