using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header(" # Prefab Data")]
    public GameObject[] Enemies;
    public GameObject[] Items;
    public GameObject DmgPopup;

    Dictionary<PoolList, List<GameObject>> _pools;

    void Awake()
    {
        _pools = new Dictionary<PoolList, List<GameObject>>();

        // Dictionary 초기화
        foreach(PoolList name in Enum.GetValues(typeof(PoolList)))
        {
            _pools[name] = new List<GameObject>();
        }
    }

    public GameObject Get(PoolList obj, out bool isNew)
    {
        GameObject result = null;
        isNew = false;
        if(obj == PoolList.None)
        {
            return result;
        }
        
        foreach(GameObject item in _pools[obj])
        {
            if(!item.activeSelf)
            {
                result = item;
                result.SetActive(true);
                break;
            }
        }

        if(!result)
        {
            GameObject prefab = GetPrefab(obj);
            isNew = true;
            result = Instantiate(prefab, transform);
            _pools[obj].Add(result);

            // 서브 프리팹이 있는 객체들은 여기서 추가 - 나중에 코드 개선해야함
            if(obj == PoolList.Fireball)
            {
                prefab = InGameManager.instance.WeaponManager.Weapons[3].projectile2;
                GameObject child = Instantiate(prefab, result.transform);
            }
        }

        return result;
    }

    public GameObject GetDmgPopup(float damage, Vector3 position, bool isCritical)
    {
        GameObject result = null;
        Transform parent = transform.Find("DamagePopup");

        foreach(GameObject item in _pools[PoolList.DamagePopUp]) // 비활성화된 DamagePopup 찾음
        {
            if(!item.activeSelf)
            {
                result = item;
                result.transform.position = position;
            }
        }
        if(!result)
        {
            result = Instantiate(DmgPopup, position, Quaternion.identity, parent);
            _pools[PoolList.DamagePopUp].Add(result);
        }
        result = ActivePopup(result, damage, isCritical);

        return result;
    }

    public GameObject GetPrefab(PoolList obj) // PoolList -> Gameobject(Prefab) 변환
    {
        switch (obj)
        {
            // Weapons
            case PoolList.RotateSword: return InGameManager.instance.WeaponManager.Weapons[0].projectile;
            case PoolList.ThrowWeapon: return InGameManager.instance.WeaponManager.Weapons[1].projectile;
            case PoolList.Laser: return InGameManager.instance.WeaponManager.Weapons[2].projectile;
            case PoolList.Fireball: return InGameManager.instance.WeaponManager.Weapons[3].projectile;
            case PoolList.Thunder: return InGameManager.instance.WeaponManager.Weapons[4].projectile;
            case PoolList.Spark: return InGameManager.instance.WeaponManager.Weapons[5].projectile;
            case PoolList.Wave: return InGameManager.instance.WeaponManager.Weapons[6].projectile;

            // Enemies
            case PoolList.FlyEye: return Enemies[0];
            case PoolList.Goblin: return Enemies[1];
            case PoolList.Mushroom: return Enemies[2];
            case PoolList.Skeleton: return Enemies[3];

            // Items
            case PoolList.ExpJewel_1: return Items[0];
            case PoolList.ExpJewel_3: return Items[1];
            case PoolList.ExpJewel_5: return Items[2];
            case PoolList.Gold: return Items[3];
            case PoolList.Magnet: return Items[4];
            case PoolList.Potion: return Items[5];

            // DmgPopUp
            case PoolList.DamagePopUp: return DmgPopup;

            default: return null;
        }
    }
    GameObject ActivePopup(GameObject obj, float damage, bool isCritical) // Popup의 표시 Damage 수정, 객체 활성화까지 시켜주는 함수
    {
        DamagePopup damagepopup = obj.GetComponent<DamagePopup>();
        int roundeddamage = Mathf.RoundToInt(damage);
        damagepopup.Setup(roundeddamage, isCritical);
        obj.SetActive(true);
        return obj;
    }
}
