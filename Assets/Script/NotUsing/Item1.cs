using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ItemData를 받아와서 
public class Item1 : MonoBehaviour
{
    public ItemData1 data;
    public Sprite image;

    Image icon;
    Text textLevel;

    GameObject Item0, Item3, Item2;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[4]; // 0번째는 자기 자신
        icon.sprite = image;
    }

    void LateUpdate()
    {
        icon.SetNativeSize();
    }

    // Panel의 온클릭 이벤트
    public void OnClick()
    {

    }

    public void LoadLevelUpPanel()
    {
        // 3개의 랜덤 아이템 선택
        List<BaseData1> randomItems = RandomItemSelect(3);
        
        // 이 함수를 GameManager의 LevelUp 함수에서 불러오면 Item0, Item1, Item2에 랜덤으로 아이템 3개를 배치해야함 - RandomItemSelect 함수
        // 1. 모든 아이템의 정보를 가지고있고 지금 해당 아이템의 레벨정보까지 가지고있는 Script 필요
        // 2. 1번의 조건을 만족하는 스크립트를 읽어옴
        // 3. 해당 스크립트에서 랜덤으로 3개의 무기를 선택함(만약 이미 만렙인 아이템이 있다면 그 무기는 제외)
        // 4. 선택한 무기들을 Item0,1,2에 배치시킴
        // 5. 플레이어가 선택한 무기를 인식해서 해당 아이템 레벨업 시킴
        // 6. 선택이 끝나면 해당 함수는 종료되고 GameManager에서 SetActive(false)로 변경후 Time.timeScale 1로 바꿀거
    }

    // 랜덤으로 count만큼 아이템을 골라주는 함수
    List<BaseData1> RandomItemSelect(int count)
    {
        // 랜덤 숫자를 생성하기 위한 Random 객체 생성
        System.Random random = new System.Random();

        // 선택된 아이템을 저장할 리스트
        List<BaseData1> selectedItems = new List<BaseData1>();

        // 중복되지 않는 랜덤 인덱스를 생성하여 아이템 선택
        HashSet<int> chosenIndices = new HashSet<int>();
        while (chosenIndices.Count < count)
        {
            int index = random.Next(0, data.ItemList.Count); // 0부터 ItemList.Count - 1 사이의 랜덤 인덱스 생성
            chosenIndices.Add(index);
        }

        // 선택된 인덱스에 해당하는 아이템을 selectedItems 리스트에 추가
        foreach (int index in chosenIndices)
        {
            selectedItems.Add(data.ItemList[index]);
        }

        return selectedItems;
    }
}
