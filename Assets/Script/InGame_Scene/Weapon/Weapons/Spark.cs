using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : WeaponBase
{
    // Scene에서 Spark의 공격 범위 노란색으로 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, combineAttackRange);
    }

    protected override void Attack()
    {
        Transform parent = poolManager.transform.Find("Weapon").Find("Weapon5");
        List<Transform> targets = player.scanner.GetAllTargetsInAttackRange(combineAttackRange);

        if(targets.Count > 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Saprk);
            foreach(Transform enemy in targets)
            {
                Transform weaponT = GetObjAndSetBase(PoolList.Spark, parent, 1, out bool isNew);
                weaponT.position = enemy.transform.position;
                enemy.GetComponent<Enemy>().TakeDamage(combineDamage, -1, transform.position, weaponname);
                weaponT.GetComponent<WeaponSetting>().StartAttackWhileDuration(0.3f);
            }
        }
    }
}
