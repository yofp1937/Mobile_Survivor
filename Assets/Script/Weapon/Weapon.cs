using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header(" # BaseData")]
    public int level;
    public ItemData itemdata;

    [Header(" # DamageData")]
    public WeaponData weapondata;

    float lastATKtime;
    bool boolAttack;
    List<WeaponSetting> weaponsetting;
    List<Coroutine> coroutines;
    Vector3 originalScale;

    Player player;
    Status playerstat;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        playerstat = InGameManager.instance.player.GetComponent<Status>();
        weaponsetting = new List<WeaponSetting>();
        coroutines = new List<Coroutine>();
    }

    void Update()
    {
        if(itemdata.itemType == ItemData.ItemType.Weapon)
        {
            switch(itemdata.itemId){
                case 0:
                    transform.Rotate(Vector3.back * weapondata.speed * playerstat.AttackSpeed * Time.deltaTime);
                    lastATKtime += Time.deltaTime;
                    if (lastATKtime >= weapondata.coolTime * playerstat.CoolTime)
                    {
                        AttackRotateSword();
                        lastATKtime = 0;
                    }
                    break;
                case 1:
                    if(boolAttack)
                    {
                        lastATKtime += Time.deltaTime;
                        if(lastATKtime >= weapondata.coolTime * playerstat.CoolTime)
                        {
                            AttackThrowWeapon();
                            lastATKtime = 0;
                        }
                    }
                    break;
                case 2:
                    lastATKtime += Time.deltaTime;
                    if(lastATKtime >= weapondata.coolTime * playerstat.CoolTime)
                    {
                        AttackLaser();
                        lastATKtime = 0;
                    }
                    break;
                case 3:
                    lastATKtime += Time.deltaTime;
                    if(lastATKtime >= weapondata.coolTime * playerstat.CoolTime)
                    {
                        AttackFireBall();
                        lastATKtime = 0;
                    }
                    break;
                case 4:
                    lastATKtime += Time.deltaTime;
                    if(lastATKtime >= weapondata.coolTime * playerstat.CoolTime)
                    {
                        AttackThunder();
                        lastATKtime = 0;
                    }
                    break;
                case 5:
                    lastATKtime += Time.deltaTime;
                    if(lastATKtime >= weapondata.coolTime * playerstat.CoolTime)
                    {
                        AttackSpark();
                        lastATKtime = 0;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    // 무기 획득시 실행하는 함수
    public void Init(ItemData data)
    {
        switch(data.itemId){
            case 0:
                BaseSetting(data);
                transform.position += new Vector3(0, -0.1f, 0);
                BatchRotateSword();
                break;
            case 1:
                BaseSetting(data);
                AttackThrowWeapon();
                break;
            case 2:
                BaseSetting(data);
                AttackLaser();
                break;
            case 3:
                BaseSetting(data);
                AttackFireBall();
                break;
            case 4:
                BaseSetting(data);
                AttackThunder();
                break;
            case 5:
                BaseSetting(data);
                AttackSpark();
                break;
            default:
                BaseSetting(data);
                break;
        }
    }

    public void InitAcce(ItemData data)
    {
        itemdata = data;
        InGameManager.instance.player.accesorries.Add(data.itemId);
        InGameManager.instance.player.stat.LevelUp(data.acceData);
    }


    void BaseSetting(ItemData data)
    {
        itemdata = data;
        weapondata = data.weaponData.Clone();
        InGameManager.instance.player.weapon.Add(data.itemId);
    }

    // 무기 레벨업시 실행하는 함수
    public void LevelUp(WeaponData weapon)
    {
        weapondata.damage += weapon.damage;
        weapondata.coolTime -= weapon.coolTime;
        weapondata.area += weapon.area;
        weapondata.duration += weapon.duration;
        weapondata.count += weapon.count;
        weapondata.speed += weapon.speed;

        if(itemdata.itemType == ItemData.ItemType.Weapon && level == 7)
        {
            InGameManager.instance.player.maxlevelcount++;
        }

        switch(itemdata.itemId)
        {
            case 0:
                BatchRotateSword();
                break;
            case 1:
                AttackThrowWeapon();
                break;
            case 2:
                AttackLaser();
                break;
            case 3:
                AttackFireBall();
                break;
            case 4:
                AttackThunder();
                break;
            case 5:
                AttackSpark();
                break;
        }
    }

    IEnumerator WaitCoroutine(Func<IEnumerator> attackCoroutine)
    {
        boolAttack = false;
        // 무한 루프를 돌며 적을 찾을 때까지 대기
        while (!player.scanner.nearestTarget)
        {
            // 대기 후 다시 확인 (0.1초 주기로 대기)
            yield return null;
        }

        // 적을 찾으면 공격을 시작
        boolAttack = true;
        yield return StartCoroutine(attackCoroutine());
    }

    // 무기들의 부모를 매개변수 parent로 따로 지정해줌
    void NewWeaponObjectCreate(Transform weaponT, Transform parent, List<WeaponSetting> weaponsetting)
    {
        weaponT.parent = parent;
        var _ = weaponT.GetComponent<WeaponSetting>();
        weaponsetting.Add(_);
    }

    void BatchRotateSword() // RotateSword 레벨업할때 실행하는 함수(RotateSword 1 사이클 실행)
    {
        for(int index=0; index < weapondata.count + playerstat.Amount; index++)
        {
            bool isNew;
            Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform;
            if(isNew)
            {
                NewWeaponObjectCreate(weaponT, transform, weaponsetting);
            }

            weaponT.localPosition = Vector3.zero; // 레벨업하면 위치 초기화
            weaponT.localRotation = Quaternion.identity; // 레벨업하면 회전 초기화

            Vector3 rotVec = Vector3.forward * 360 * index / weapondata.count; // 무기가 여러개여도 일정한 원을 그리며 회전하는 공식
            weaponT.Rotate(rotVec); // 위 공식대로 회전하게 만듦
            weaponT.Translate(weaponT.up * weapondata.area * playerstat.Area, Space.World); // 플레이어와 무기 사이의 거리

            weaponT.GetComponent<WeaponSetting>().Init(weapondata.damage * playerstat.Damage, -1, weapondata.knockback, Vector3.zero); // 무한 관통이라 per는 -1로 설정
        }
        AttackRotateSword();
    }

    void AttackRotateSword() // RotateSword 1회 작동
    {
        foreach(var coroutine in coroutines) // 이미 무기공격이 진행중일경우 전부 취소
        {
            StopCoroutine(coroutine);
        }
        coroutines.Clear();
        foreach(var rotatesword in weaponsetting) // laser의 각 객체마다 지속시간, 쿨타임 부여하고 동작시킴
        {
            coroutines.Add(StartCoroutine(rotatesword.AttackWhileDuration(weapondata.duration * playerstat.Duration)));  // duration 값만큼 무기 지속시킴
        }
    }

    void AttackThrowWeapon() 
    {
        // 코루틴을 시작하고 적을 기다림
        StartCoroutine(WaitCoroutine(AttackThrowWeaponCoroutine));
    }


    IEnumerator AttackThrowWeaponCoroutine() // ThrowWeapon 1회 작동
    {
        for(int i = 0; i < weapondata.count + playerstat.Amount; i++)
        {
            Vector3 targetPos = player.scanner.nearestTarget.position;
            Vector3 dir = targetPos - transform.position;
            dir = dir.normalized;

            bool isNew;
            Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform;
            Transform parent = InGameManager.instance.PoolManager.transform.Find("Weapon").Find("Weapon1");
            if(isNew)
            {
                NewWeaponObjectCreate(weaponT, parent, weaponsetting);
            }
            weaponT.position = transform.position;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // dir 벡터의 각도 계산
            weaponT.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            weaponT.GetComponent<WeaponSetting>().Init(weapondata.damage * playerstat.Damage, 0, weapondata.knockback, dir);
            StartCoroutine(weaponT.GetComponent<WeaponSetting>().ThrowWhileDuration(5f));

            Rigidbody2D rb = weaponT.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = dir * weapondata.speed * playerstat.AttackSpeed;
            }
            yield return new WaitForSeconds(0.35f / playerstat.AttackSpeed);
        }
    }

    void AttackLaser()
    {
        StartCoroutine(AttackLaserCoroutine());
    }

    IEnumerator AttackLaserCoroutine()
    {
        int lasercount = weapondata.count + playerstat.Amount;
        List<Transform> targets = player.scanner.GetTargets(lasercount);
        Transform parent = InGameManager.instance.PoolManager.transform.Find("Weapon").Find("Weapon2");

        for(int i = 0; i < lasercount; i++)
        {
            bool isNew;
            Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform;
            if(isNew)
            {
                NewWeaponObjectCreate(weaponT, parent, weaponsetting);
            }

            // 찾아낸 weaponT의 위치를 플레이어 위치로 조정후 크기, 데미지 세팅
            weaponT.position = player.transform.position;
            float newScale = weapondata.area * playerstat.Area;
            weaponT.localScale = new Vector3(newScale, newScale, newScale);
            weaponT.GetComponent<WeaponSetting>().Init(weapondata.damage * playerstat.Damage, -1, weapondata.knockback, Vector3.zero); // 무한 관통이라 per는 -1로 설정

            Vector3 dir;
            if (i < targets.Count) // 범위내에 적이 존재하면
            {
                // i번 무기를 적 방향으로 조준
                Vector3 targetPos = targets[i].position;
                // z값을 0으로 설정하여 방향을 계산(z값에따라 무기의 속도가 달라지기때문)
                dir = new Vector3(targetPos.x - transform.position.x, targetPos.y - transform.position.y, 0).normalized;
            }
            else
            {
                // 적이 없으면 i번 레이저를 랜덤 방향으로 조준
                dir = UnityEngine.Random.insideUnitCircle.normalized; // 2D 평면에서 랜덤 방향 계산
            }

            // weaponT를 반대(dir의 반대방향) 방향으로 회전
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // dir 벡터의 각도 계산
            weaponT.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180)); // 반대 방향(180도 추가)으로 회전
            
            // 조준한곳으로 발사
            Rigidbody2D rb = weaponT.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = dir * weapondata.speed;
            }
            yield return null;
        }
    }

    void AttackFireBall()
    {
        int firballcount = weapondata.count + playerstat.Amount;
        List<Transform> targets = player.scanner.GetTargets(firballcount);
        Transform parent = InGameManager.instance.PoolManager.transform.Find("Weapon").Find("Weapon3");
        float range = InGameManager.instance.player.scanRange * playerstat.Area;

        for(int i = 0; i < firballcount; i++)
        {
            bool isNew;
            Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform;
            Transform weaponC = weaponT.GetChild(0);
            if(isNew)
            {
                NewWeaponObjectCreate(weaponT, parent, weaponsetting);
            }
            
            // 찾아낸 weaponT의 위치를 플레이어 위치로 조정후 크기, 데미지 세팅
            weaponT.position = player.transform.position;
            float newScale = weapondata.area * playerstat.Area;
            weaponT.localScale = new Vector3(newScale, newScale, newScale);
            weaponC.localScale = new Vector3(newScale, newScale, newScale);
            weaponC.GetComponent<WeaponSetting>().Init(weapondata.damage * playerstat.Damage, -1, weapondata.knockback, Vector3.zero);

            Vector3 targetPos;
            if (i < targets.Count) // 범위내에 적이 존재하면
            {
                // i번 무기를 적 방향으로 조준
                targetPos = targets[i].position;
            }
            else
            {
                // 적이 없으면 i번 무기를 랜덤 방향으로 조준
                Vector2 randomPos = UnityEngine.Random.insideUnitCircle * range;
                targetPos = player.transform.position + new Vector3(randomPos.x, randomPos.y, 0);
            }
            float arcHeight = 2.0f;
            StartCoroutine(MoveInArc(weaponT, weaponC, weaponT.position, targetPos, arcHeight));
        }
    }
    IEnumerator MoveInArc(Transform weaponT, Transform weaponC, Vector3 startPos, Vector3 targetPos, float arcHeight)
    {
        float time = 0;
        float baseDuration = 5f; // 기본 지속시간
        float speedFactor = weapondata.speed * playerstat.AttackSpeed; // 속도 비율
        float duration = baseDuration / speedFactor; // 속도가 높을수록 지속시간이 짧아짐

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

        weaponC.position = weaponT.position + new Vector3(0,1,0);
        weaponC.gameObject.SetActive(true);
        weaponC.parent = weaponT.parent;
        weaponT.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.75f); // 애니메이션이 끝날 때까지 대기

        weaponC.parent = weaponT;
        weaponC.gameObject.SetActive(false);
    }

    void AttackThunder()
    {
        int thundercount = weapondata.count + playerstat.Amount;
        List<Transform> targets = player.scanner.GetTargets(thundercount);

        for(int i = 0; i < thundercount; i++)
        {
            bool isNew;
            Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform;
            Transform parent = InGameManager.instance.PoolManager.transform.Find("Weapon").Find("Weapon4");
            if(isNew)
            {
                originalScale = weaponT.localScale;
                NewWeaponObjectCreate(weaponT, parent, weaponsetting);
            }
            float newScale = weapondata.area * playerstat.Area;
            weaponT.localScale = originalScale * newScale;
            weaponT.GetComponent<WeaponSetting>().Init(weapondata.damage * playerstat.Damage, -1, weapondata.knockback, Vector3.zero);

            Vector3 targetPos;
            if (i < targets.Count) // 범위내에 적이 존재하면
            {
                // i번 무기를 적 방향으로 조준
                targetPos = targets[i].position;
            }
            else
            {
                // 적이 없으면 플레이어 화면 내 랜덤한 방향으로 조준
                Vector2 screenPos = new Vector2(
                    UnityEngine.Random.Range(0, Screen.width),
                    UnityEngine.Random.Range(0, Screen.height)
                );
                
                // 화면 좌표를 월드 좌표로 변환 (z값은 카메라에서 적당히 떨어진 값으로 설정)
                targetPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane + 10f));
            }

            // weaponT의 랜덤한 부분을 targetPos로 이동시키기 위한 계산
            CapsuleCollider2D weaponCollider = weaponT.GetComponent<CapsuleCollider2D>();

            // weaponT의 크기 범위 내에서 랜덤한 부분을 선택
            float randomYOffset = UnityEngine.Random.Range(-weaponCollider.bounds.extents.y, weaponCollider.bounds.extents.y);
            float randomXOffset = UnityEngine.Random.Range(-weaponCollider.bounds.extents.x, weaponCollider.bounds.extents.x);

            // weaponT의 랜덤한 위치 (중심점에서 랜덤한 오프셋을 추가한 위치)
            Vector3 randomPartOfWeapon = weaponT.position + new Vector3(randomXOffset, randomYOffset, 0);

            // 랜덤한 부분을 targetPos로 이동시키기 위한 오프셋 계산
            Vector3 offset = targetPos - randomPartOfWeapon;

            // 오프셋을 적용하여 weaponT 이동
            weaponT.position += offset;
            StartCoroutine(SetActiveWeapon(weaponT.gameObject, 0.45f));
        }
    }

    IEnumerator SetActiveWeapon(GameObject obj, float delay)
    {
        // delay 만큼 대기
        yield return new WaitForSeconds(delay);

        obj.SetActive(false);
    }

    void AttackSpark()
    {
        float _range = weapondata.area * InGameManager.instance.player.stat.Area;
        // 스캔 범위 내 모든 2D 콜라이더 탐색
        Collider2D[] enemyInRange = Physics2D.OverlapCircleAll(transform.position, _range);

        foreach (Collider2D enemy in enemyInRange)
        {
            // 적 오브젝트인지 확인
           if (enemy.CompareTag("Enemy"))
            {
                Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out bool isNew).transform;
                Transform parent = InGameManager.instance.PoolManager.transform.Find("Weapon").Find("Weapon5");
                if(isNew)
                {
                    NewWeaponObjectCreate(weaponT, parent, weaponsetting);
                }
                enemy.GetComponent<Enemy>().TakeDamage(weapondata.damage * InGameManager.instance.player.stat.Damage, 0, transform.position);
                weaponT.position = enemy.transform.position;
                
                StartCoroutine(SetActiveWeapon(weaponT.gameObject, 0.3f));
            }
        }
    }
}
