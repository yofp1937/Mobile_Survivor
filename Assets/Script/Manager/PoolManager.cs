using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header(" # Prefab Data")]
    public GameObject[] Enemies;
    public GameObject[] Items;
    public GameObject DmgPopUp;

    private Dictionary<PoolList, List<GameObject>> pools;

    void Awake()
    {
        pools = new Dictionary<PoolList, List<GameObject>>();

        // Dictionary 초기화
        foreach(PoolList obj in Enum.GetValues(typeof(PoolList)))
        {
            pools[obj] = new List<GameObject>();
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
        
        foreach(GameObject item in pools[obj])
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
            pools[obj].Add(result);

            // 서브 프리팹이 있는 객체들은 여기서 추가 - 나중에 코드 개선해야함
            if(obj == PoolList.Fireball)
            {
                prefab = InGameManager.instance.WeaponManager.Weapons[3].projectile2;
                GameObject child = Instantiate(prefab, result.transform);
            }
        }

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
            case PoolList.DamagePopUp: return DmgPopUp;

            default: return null;
        }
    }
}
