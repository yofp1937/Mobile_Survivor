using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : WeaponBase
{
    // Scene에서 Laser의 공격 범위 파란색으로 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, combineAttackRange);
    }

    protected override void Attack()
    {
        Transform parent = poolManager.transform.Find("Weapon").Find("Weapon2");
        List<Transform> targets = player.Scanner.GetTargetsInScanRange(combineProjectileCount);

        for(int i = 0; i < combineProjectileCount; i++)
        {
            Transform weaponT = GetObjAndSetBase(PoolList.Laser, parent, combineProjectileSize, out bool isNew);
            weaponT = GetDir(weaponT, targets, i);
            weaponT.GetComponent<WeaponSetting>().Init(combineDamage, -1, weapondata.Knockback, Vector3.zero, weaponname);
            weaponT.GetComponent<WeaponSetting>().StartAttackWhileDuration(3f);
        }
    }

    Transform GetDir(Transform weaponT, List<Transform> targets, int num)
    {
        Vector3 dir;
        weaponT.position = player.transform.position;
        
        // 발사할 좌표 선정
        if (num < targets.Count)
        {
            Vector3 targetPos = targets[num].position;
            // z값을 0으로 설정하여 방향을 계산(z값에따라 무기의 속도가 달라지기때문)
            dir = new Vector3(targetPos.x - transform.position.x, targetPos.y - transform.position.y, 0).normalized;
        }
        else
        {
            dir = UnityEngine.Random.insideUnitCircle.normalized; // 2D 평면에서 랜덤 방향 계산
        }

        // 무기의 앞부분이 좌표를 바라보게 설정
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // dir 벡터의 각도 계산
        weaponT.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180)); // 반대 방향(180도 추가)으로 회전

        // 속도 부여
        Rigidbody2D rb = weaponT.GetComponent<Rigidbody2D>();
        rb.velocity = dir * combineProjectileSpeed;

        return weaponT;
    }
}
