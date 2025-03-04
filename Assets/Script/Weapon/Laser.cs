using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : WeaponBase
{
    protected override void Attack()
    {
        MergeWeaponAndPlayerStats(); // 스탯 동기화

        List<Transform> targets = player.scanner.GetTargetsInScanRange(combineProjectileCount);
        Transform parent = InGameManager.instance.PoolManager.transform.Find("Weapon").Find("Weapon2");

        for(int i = 0; i < combineProjectileCount; i++)
        {
            bool isNew;
            Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform;
            if(isNew)
            {
                weaponT.parent = parent;
            }
            weaponT.position = player.transform.position;
            float newScale = combineProjectileSize;
            weaponT.localScale = new Vector3(newScale, newScale, newScale);
            weaponT.GetComponent<WeaponSetting>().Init(combineDamage, -1, weapondata.Knockback, Vector3.zero, weaponname);

            Vector3 dir = GetDir(targets, i);

            // weaponT를 반대(dir의 반대방향) 방향으로 회전
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // dir 벡터의 각도 계산
            weaponT.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180)); // 반대 방향(180도 추가)으로 회전
            
            // 3초 뒤에 자동으로 비활성화
            StartCoroutine(weaponT.GetComponent<WeaponSetting>().ThrowWhileDuration(3f));
            
            // 조준한곳으로 발사
            Rigidbody2D rb = weaponT.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = dir * combineProjectileSpeed;
            }
        }
    }

    Vector3 GetDir(List<Transform> targets, int i)
    {
        Vector3 dir;
        
        if (i < targets.Count)
        {
            Vector3 targetPos = targets[i].position;
            // z값을 0으로 설정하여 방향을 계산(z값에따라 무기의 속도가 달라지기때문)
            dir = new Vector3(targetPos.x - transform.position.x, targetPos.y - transform.position.y, 0).normalized;
        }
        else
        {
            // 적이 없으면 i번 레이저를 랜덤 방향으로 조준
            dir = UnityEngine.Random.insideUnitCircle.normalized; // 2D 평면에서 랜덤 방향 계산
        }

        return dir;
    }
}
