using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class Fireball : WeaponBase
{

    // Scene에서 Fireball의 공격 범위 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // 노란색으로 설정
        Gizmos.DrawWireSphere(transform.position, combineAttackRange); // 얇은 실선으로 원 그리기
    }

    protected override void Attack()
    {
        MergeWeaponAndPlayerStats(); // 스탯 동기화

        List<Transform> targets = player.scanner.GetTragetsInAttackRange(combineAttackRange, combineProjectileCount);
        Transform parent = InGameManager.instance.PoolManager.transform.Find("Weapon").Find("Weapon3");

        for(int i = 0; i < combineProjectileCount; i++)
        {
            bool isNew;
            Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform;
            Transform weaponC = weaponT.GetChild(0);
            if(isNew)
            {
                weaponT.parent = parent;
            }
            
            // 찾아낸 weaponT의 위치를 플레이어 위치로 조정후 크기, 데미지 세팅
            weaponT.position = player.transform.position;
            float newScale = combineProjectileSize;
            weaponT.localScale = new Vector3(newScale, newScale, newScale);
            weaponC.localScale = new Vector3(newScale, newScale, newScale);
            weaponC.GetComponent<WeaponSetting>().Init(combineDamage, -1, weapondata.Knockback, Vector3.zero, weaponname);

            Vector3 targetPos = GetDir(targets);
            float arcHeight = 2.0f;
            StartCoroutine(MoveInArc(weaponT, weaponC, weaponT.position, targetPos, arcHeight));
        }
    }

    Vector3 GetDir(List<Transform> list)
    {
        Vector3 dir;
        if (list.Count > 0) // 범위내에 적이 존재하면
        {
            // i번 무기를 랜덤 적에게 조준
            int randomenemy = UnityEngine.Random.Range(0, list.Count);
            dir = list[randomenemy].position;
            list.RemoveAt(randomenemy);
        }
        else
        {
            // 적이 없으면 i번 무기를 랜덤 방향으로 조준
            Vector2 randomPos = UnityEngine.Random.insideUnitCircle * combineAttackRange;
            dir = player.transform.position + new Vector3(randomPos.x, randomPos.y, 0);
        }
        return dir;
    }

    IEnumerator MoveInArc(Transform weaponT, Transform weaponC, Vector3 startPos, Vector3 targetPos, float arcHeight)
    {
        float time = 0;
        float baseDuration = 5f; // 기본 지속시간
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
        weaponC.parent = weaponT.parent;
        weaponT.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.75f); // 애니메이션이 끝날 때까지 대기

        weaponC.parent = weaponT;
        weaponC.gameObject.SetActive(false);
    }
}
