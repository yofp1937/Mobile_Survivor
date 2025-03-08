using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : WeaponBase
{
    Vector3 originalchildscale;
    Vector3 childscale;

    // Scene에서 Fireball의 공격 범위 빨간색으로 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, combineAttackRange);
    }

    protected override void Attack()
    {
        Transform parent = poolManager.transform.Find("Weapon").Find("Weapon3");
        List<Transform> targets = player.scanner.GetTragetsInAttackRange(combineAttackRange, combineProjectileCount);

        for(int i = 0; i < combineProjectileCount; i++)
        {
            Transform weaponT = GetObjAndSetBase(PoolList.Fireball, parent, combineProjectileSize, out bool isNew);
            Transform weaponC = weaponT.GetChild(0);
            originalchildscale = weaponC.localScale;
            weaponC = SetWeaponC(weaponC);
            
            Vector3 targetPos = GetDir(weaponT, targets);
            StartCoroutine(MoveInArc(weaponT, weaponC, weaponT.position, targetPos));
        }
    }

    Transform SetWeaponC(Transform weaponC)
    {
        weaponC.gameObject.SetActive(false);
        weaponC.localScale = weaponC.localScale * combineProjectileSize;
        childscale = weaponC.localScale;
        weaponC.GetComponent<WeaponSetting>().Init(combineDamage, -1, weapondata.Knockback, Vector3.zero, weaponname);

        return weaponC;
    }

    Vector3 GetDir(Transform weaponT,List<Transform> list)
    {
        weaponT.position = player.transform.position;
        Vector3 dir;
        if (list.Count > 0) // 범위내에 적이 존재하면
        {
            // i번 무기를 랜덤 적에게 조준
            int randomenemy = Random.Range(0, list.Count);
            dir = list[randomenemy].position;
            list.RemoveAt(randomenemy);
        }
        else
        {
            // 적이 없으면 i번 무기를 랜덤 방향으로 조준
            Vector2 randomPos = Random.insideUnitCircle * combineAttackRange;
            dir = player.transform.position + new Vector3(randomPos.x, randomPos.y, 0);
        }
        return dir;
    }

    IEnumerator MoveInArc(Transform weaponT, Transform weaponC, Vector3 startPos, Vector3 targetPos)
    {
        float time = 0;
        float arcHeight = 2f; // 포물선 높이
        float baseDuration = 4.75f; // 기본 지속시간
        float speedFactor = combineProjectileSpeed; // 속도 비율
        float duration = baseDuration / speedFactor; // 속도가 높을수록 적에게 날아가는 시간이 짧아짐

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // 포물선 운동 계산
            Vector3 currentPosition = Vector3.Lerp(startPos, targetPos, t);
            currentPosition.y += Mathf.Sin(t * Mathf.PI) * arcHeight; // 포물선 궤적 추가

            weaponT.position = currentPosition;

            yield return null;
        }
        // weaponT가 부모일때 weaponT가 비활성화되면 weaponC도 같이 비활성화돼서 잠깐 부모 객체를 바꾸고 동작이 끝나면 다시 weaponT를 부모 객체로 지정
        weaponC.position = weaponT.position + new Vector3(0,1,0);
        weaponC.gameObject.SetActive(true);
        //부모가 변경돼서 localScale 설정
        weaponC.parent = weaponT.parent;
        weaponC.localScale = childscale;
        weaponT.position = player.transform.position + new Vector3(100,100,0);

        yield return new WaitForSeconds(0.7f); // 애니메이션이 끝날 때까지 대기

        //부모가 변경돼서 localScale 설정
        weaponC.parent = weaponT;
        weaponC.localScale = originalchildscale;
        weaponT.gameObject.SetActive(false);
    }
}
