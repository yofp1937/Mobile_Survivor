using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 무기의 오브젝트 풀링을 담당하는 스크립트
public class WeaponManager : MonoBehaviour
{
    [Header(" # All Weapon Data")]
    // ItemData를 보관할 변수
    public ItemData[] weapons;
    public ItemData[] accessories;
    public ItemData[] etcs;

    // 풀을 담당하는 변수
    List<GameObject>[] pools;

    void Awake()
    {
        pools = new List<GameObject>[weapons.Length];

        for(int index = 0; index < pools.Length; index++){
            pools[index] = new List<GameObject>();
        }
    }

    // 무기 오브젝트 풀링 담당하는 함수
    public GameObject Get(int index, out bool isNew)
    {
        GameObject select = null;
        isNew = false;

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
            isNew = true;
            select = Instantiate(weapons[index].projectile, transform);
            pools[index].Add(select);
            if(weapons[index].projectile2 != null)
            {
                GameObject childweapon = Instantiate(weapons[index].projectile2, select.transform);
                childweapon.SetActive(false);
            }
        }

        return select;
    }
}
