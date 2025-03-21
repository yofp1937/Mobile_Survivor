using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : WeaponBase
{
    protected override void Attack()
    {
        Transform parent = poolManager.transform.Find("Weapon").Find("Weapon4");
        List<Transform> targets = player.Scanner.GetAllTargetsInAttackRange(player.Scanner.activescanRange * combineAttackRange);
        AudioManager.instance.PlaySfx(Sfx.Thunder);

        for(int i = 0; i < combineProjectileCount; i++)
        {
            Transform weaponT = GetObjAndSetBase(PoolEnum.Thunder, parent, combineAttackRange, out bool isNew);

            weaponT = SetDir(weaponT, targets); // 번개 타격 좌표 설정

            weaponT.GetComponent<WeaponSetting>().Init(combineDamage, -1, _wStatusData.Knockback, Vector3.zero, weaponname);
            weaponT.GetComponent<WeaponSetting>().StartAttackWhileDuration(0.45f);
        }
    }
    
    Transform SetDir(Transform weaponT, List<Transform> enemylist)
    {
        Vector3 targetPos;
        CapsuleCollider2D weaponCollider = weaponT.GetComponent<CapsuleCollider2D>();

        if(enemylist.Count > combineProjectileCount)
        {
            int randomnum = Random.Range(0, enemylist.Count);
            targetPos = enemylist[randomnum].position;
            enemylist.RemoveAt(randomnum);
        }
        else
        {
            // 플레이어 화면 내 랜덤한 방향으로 조준
            Vector2 screenPos = new Vector2(
                Random.Range(0, Screen.width),
                Random.Range(0, Screen.height)
            );
                
            // 화면 좌표를 월드 좌표로 변환 (z값은 카메라에서 적당히 떨어진 값으로 설정)
            targetPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane + 10f));
        }

        // thunder의 랜덤한 부분을 targetPos로 이동시키기 위한 계산
        // thunder의 크기 범위 내에서 랜덤한 부분을 선택
        float randomXOffset = Random.Range(-weaponCollider.bounds.extents.x, weaponCollider.bounds.extents.x);
        float randomYOffset = Random.Range(-weaponCollider.bounds.extents.y, weaponCollider.bounds.extents.y);

        // thunder의 랜덤한 위치 (thunder의 중심에서 랜덤 오프셋 위치를 추가한 위치)
        Vector3 randomPartOfWeapon = weaponT.position + new Vector3(randomXOffset, randomYOffset, 0);

        // 랜덤한 부분을 targetPos로 이동시키기 위한 오프셋 계산
        Vector3 dir = targetPos - randomPartOfWeapon;

        weaponT.position += dir;

        return weaponT;
    }
}
