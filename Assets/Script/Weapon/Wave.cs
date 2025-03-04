using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : WeaponBase
{
    protected override void Attack()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Wave);

        bool isNew;
        Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform;
        if(isNew)
        {
            weaponT.parent = transform;
        }
        float newScale = combineProjectileSize;
        weaponT.localScale = new Vector3(newScale, newScale, newScale);
        weaponT.GetComponent<WeaponSetting>().Init(combineDamage, -1, weapondata.Knockback, Vector3.zero, weaponname);
        weaponT.GetComponent<WeaponSetting>().AttackWhileDuration(0.45f);
    }
}
