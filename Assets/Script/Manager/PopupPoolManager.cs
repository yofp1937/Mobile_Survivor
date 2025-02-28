using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 데미지 팝업 오브젝트풀링 스크립트
public class PopupPoolManager : MonoBehaviour
{
    public static PopupPoolManager instance;
    public GameObject prefab; // 데미지 팝업 프리팹
    public int initpoolsize = 5;
    List<GameObject> pool;

    void Awake()
    {
        instance = this;
        pool = new List<GameObject>();
        for(int i = 0; i < initpoolsize; i++){ // 초기 5개의 팝업창 생성
            GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject Get(float damage, Vector3 popupP)
    {
        GameObject obj = FindFalsePopup(); // obj에 비활성화된 Popup 가져옴

        if(obj != null){ // 비활성화된 팝업이 존재하면
            obj = ActivePopup(obj, damage); // 팝업 데미지 세팅 후 활성화
            obj.transform.position = popupP; // 위치 조정
            return obj;
        }
        // 팝업이 전부 활성화 상태면
        GameObject newobj = Instantiate(prefab, popupP, Quaternion.identity, transform); // 팝업 하나 생성후 위치 조정
        newobj = ActivePopup(newobj, damage); // 팝업 데미지 세팅 후 활성화
        pool.Add(newobj);
        return newobj;
    }

    GameObject FindFalsePopup() // 비활성화된 Popup 찾아주는 함수
    {
        GameObject obj = null;
        foreach(GameObject item in pool)
        {
            if(!item.activeInHierarchy)
                obj = item;
        }
        return obj;
    }

    GameObject ActivePopup(GameObject obj, float damage) // Popup의 표시 Damage 수정, 객체 활성화까지 시켜주는 함수
    {
        DamagePopup DPcomponent = obj.GetComponent<DamagePopup>();
        int _damage = Mathf.RoundToInt(damage);
        DPcomponent.Setup(_damage);
        obj.SetActive(true);
        return obj;
    }
}
