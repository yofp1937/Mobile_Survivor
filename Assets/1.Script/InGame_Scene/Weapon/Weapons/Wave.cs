using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : WeaponBase
{
    protected override void Attack()
    {
        AudioManager.instance.PlaySfx(Sfx.Wave);

        Transform weaponT = GetObjAndSetBase(PoolEnum.Wave, transform, combineAttackRange, out bool isNew);
        weaponT.position = player.transform.position;
        
        weaponT.GetComponent<WeaponSetting>().Init(combineDamage, -1, _wStatusData.Knockback, Vector3.zero, weaponname);
        weaponT.GetComponent<WeaponSetting>().StartAttackWhileDuration(0.4f);
    }
}
