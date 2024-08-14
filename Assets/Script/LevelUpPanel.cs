using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelUpPanel : MonoBehaviour
{
    public Item[] ItemLists;

    void BaseSetting()
    {
        ItemLists = GetComponentsInChildren<Item>();
    }

    void OnEnable()
    {
        BaseSetting();
        LoadLevelUpPanel();
    }

    public void HideLevelUpPanel()
    {
        InGameManager.instance.LevelUpPanel.SetActive(false);
        Time.timeScale = 1;
    }

    void LoadLevelUpPanel()
    {
        // 3개의 랜덤 아이템 선택
        ItemData[] randomItems = RandomItemSelect(3);
        
        for(int i = 0; i < randomItems.Length; i++)
        {
            ItemLists[i].SetItemData(randomItems[i]);
        }
    }

    bool CheckWeapon()
    {
        bool check;
        if(InGameManager.instance.player.weapon.Count > 5)
        {
            check = true;
        }
        else
        {
            check = false;
        }
        Debug.Log("CheckWeapon:" + InGameManager.instance.player.weapon.Count + ", check: " + check);
        return check;
    }

    bool CheckAcce()
    {
        bool check;
        if(InGameManager.instance.player.accesorries.Count > 5)
        {
            check = true;
        }
        else
        {
            check = false;
        }
        Debug.Log("CheckAcce:" + InGameManager.instance.player.accesorries.Count + ", check: " + check);
        return check;
    }

    bool CheckMaxLevel(ItemData data)
    {
        bool check;
        Weapon weapon;

        if(data.itemType == ItemData.ItemType.Weapon)
        {
            weapon = InGameManager.instance.WeaponManager.transform.Find("Weapon" + data.itemId).GetComponent<Weapon>();
        }
        else
        {
            weapon = InGameManager.instance.WeaponManager.transform.Find("Acce" + data.itemId).GetComponent<Weapon>();
        }

        if(weapon.level == data.maxlevel)
        {
            check = true;
        }
        else
        {
            check = false;
        }

        return check;
    }

    // 랜덤으로 count만큼 아이템을 골라주는 함수
    ItemData[] RandomItemSelect(int count)
    {
        List<ItemData> items = new List<ItemData>();
        List<int> usedNum = new List<int>();
        int RandomNum;
        int allweaponcount = InGameManager.instance.WeaponManager.weapons.Length; // 전체 무기 개수
        int allaccecount = InGameManager.instance.WeaponManager.accessories.Length; // 전체 장신구 개수
        int myweaponcount = InGameManager.instance.player.weapon.Count; // 유저 무기 개수
        int myaccecount = InGameManager.instance.player.accesorries.Count; // 유저 장신구 개수

        while(items.Count < count) // items List의 길이가 count보다 작으면 같아질때까지 계속 추가 
        {
            // 1.무기와 장신구 각각 6칸 꽉찼으면 갖고있는 무기들만 나와야함(안찼으면 랜덤으로 아이템 선택)
            //   무기와 장신구가 다 6칸인데 다 만렙이라 2개, 1개만 뜰때는 item1과 item2는 비활성화 해야함
            //   무기와 장신구가 6칸이고 전부 강화가 끝났으면 체력회복 or 골드 획득을 해야함
            // 2.받아온 아이템 번호의 weapon을 찾아서 레벨을 확인함
            // 3.레벨이 itemdata.maxlevel값과 같으면 RandomNum을 다시 찾아야함

            // 무기나 장신구가 6칸이면 랜덤무기나 랜덤장신구가 나오면 안됨
            // 무기와 장신구가 둘다 6칸이면 그중에서 랜덤
            if(CheckWeapon() && CheckAcce()) // 무기와 장신구가 둘다 꽉차있으면 if문 실행
            {
                RandomNum = Random.Range(0, myweaponcount + myaccecount);
                if(!usedNum.Contains(RandomNum))
                {
                    ItemData selectedItem;
                    if(RandomNum >= myweaponcount) // RandomNum이 장신구 번호가 나오면
                    {
                        int itemnum = InGameManager.instance.player.accesorries[RandomNum-myweaponcount]; // player의 인벤토리에 있는 장신구의 장신구번호를 받아옴
                        selectedItem = InGameManager.instance.WeaponManager.accessories[itemnum];
                    }
                    else // RandomNum이 무기 번호면
                    {
                        int itemnum = InGameManager.instance.player.weapon[RandomNum-myweaponcount]; // player의 인벤토리에 있는 무기의 무기번호를 받아옴
                        selectedItem = InGameManager.instance.WeaponManager.weapons[itemnum];
                    }

                    if (!CheckMaxLevel(selectedItem)) // 해당 무기나 장신구가 최고레벨이 아니면 추가
                    {
                        items.Add(selectedItem);
                        usedNum.Add(RandomNum);
                        Debug.Log("무기와 장신구 둘 다 꽉 차 있음 " + selectedItem + " 추가");
                    }
                }
            }
            else if(CheckWeapon()) // 무기칸만 꽉차있으면 else if문 실행
            {
                RandomNum = Random.Range(0, myweaponcount + allaccecount);
                if(!usedNum.Contains(RandomNum))
                {
                    ItemData selectedItem;
                    if(RandomNum >= myweaponcount) // RandomNum이 장신구 번호가 나오면
                    {
                        selectedItem = InGameManager.instance.WeaponManager.accessories[RandomNum - myweaponcount];
                    }
                    else // RandomNum이 무기번호면
                    {
                        int itemnum = InGameManager.instance.player.weapon[RandomNum];
                        selectedItem = InGameManager.instance.WeaponManager.weapons[itemnum];
                    }

                    if (!CheckMaxLevel(selectedItem)) // 최고 레벨이 아닌 경우만 추가
                    {
                        items.Add(selectedItem);
                        usedNum.Add(RandomNum);
                        Debug.Log("무기만 꽉 차 있음" + selectedItem + " 추가");
                    }
                }
            }
            else if(CheckAcce()) // 장신구칸만 꽉차있으면 else if문 실행
            {
                RandomNum = Random.Range(0, allweaponcount + myaccecount);
                Debug.Log(allweaponcount+myaccecount);
                if(!usedNum.Contains(RandomNum))
                {
                    ItemData selectedItem;
                    if(RandomNum >= allweaponcount) // RandomNum이 장신구 번호가 나오면
                    {
                        int itemnum = InGameManager.instance.player.accesorries[RandomNum-allweaponcount];
                        selectedItem = InGameManager.instance.WeaponManager.accessories[itemnum];
                    }
                    else
                    {
                        selectedItem = InGameManager.instance.WeaponManager.weapons[RandomNum];
                    }
                    
                    if (!CheckMaxLevel(selectedItem)) // 최고 레벨이 아닌 경우만 추가
                    {
                        items.Add(selectedItem);
                        usedNum.Add(RandomNum);
                        Debug.Log("장신구만 꽉 차 있음" + selectedItem + " 추가");
                    }
                }
            }
            else // 둘다 꽉차있지않으면 else문 실행
            {
                RandomNum = Random.Range(0, allweaponcount + allaccecount); // 랜덤 아이템 번호를 받아옴
                if(!usedNum.Contains(RandomNum))
                {
                    ItemData selectedItem;
                    if(RandomNum >= allweaponcount)
                    {
                        selectedItem = InGameManager.instance.WeaponManager.accessories[RandomNum - allweaponcount];
                    }
                    else
                    {
                        selectedItem = InGameManager.instance.WeaponManager.weapons[RandomNum];
                    }

                    if (!CheckMaxLevel(selectedItem)) // 최고 레벨이 아닌 경우만 추가
                    {
                        items.Add(selectedItem);
                        usedNum.Add(RandomNum);
                        Debug.Log("무기와 장신구 다 빈자리 있음");
                    }
                }
            }
        }

        return items.ToArray();;
    }
}
