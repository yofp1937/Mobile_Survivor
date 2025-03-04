using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : WeaponBase
{
    [Header("# Weapon Data")]
    private float attackrange; // 공격 범위

    void Awake()
    {
        attackrange = weapondata.AttackRange * player.stat.AttackRange;
    }

    // Scene에서 Spark의 공격 범위 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; // 노란색으로 설정
        Gizmos.DrawWireSphere(transform.position, attackrange); // 얇은 실선으로 원 그리기
    }

    protected override void Attack()
    {
        MergeWeaponAndPlayerStats(); // 스탯 동기화

        List<Transform> targets = player.scanner.GetAllTargetsInAttackRange(combineAttackRange); // 범위내 모든 Enemy 받음

        if(targets.Count > 0)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Saprk);
            foreach(Transform enemy in targets)
            {
                Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out bool isNew).transform;
                Transform parent = InGameManager.instance.PoolManager.transform.Find("Weapon").Find("Weapon5");
                if(isNew)
                {
                    weaponT.parent = parent;
                }
                weaponT.position = enemy.transform.position;
                enemy.GetComponent<Enemy>().TakeDamage(combineDamage, -1, transform.position, weaponname);

                weaponT.GetComponent<WeaponSetting>().AttackWhileDuration(0.35f);
            }
        }
    }
}
