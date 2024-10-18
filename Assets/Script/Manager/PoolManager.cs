using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header(" # All Item Data")]
    // Item을 보관할 변수
    public GameObject[] items;
    List<GameObject>[] pools;

    void Awake()
    {
        pools = new List<GameObject>[items.Length];

        for(int index = 0; index < pools.Length; index++){
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        // 선택한 pool의 비활성화된 게임오브젝트에 접근
        foreach(GameObject item in pools[index]){
            if(!item.activeSelf){// 비활성화된걸 발견하면 select 변수에 할당
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // 못찾았으면?
        if(!select){
            // 새롭게 생성하고 select 변수에 할당
            select = Instantiate(items[index], transform);
            pools[index].Add(select);
        }

        return select;
    }
}
