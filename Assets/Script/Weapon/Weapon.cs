using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 무기와 악세사리는 각 6개씩만 획득가능
public class Weapon : MonoBehaviour
{

    [Header(" # BaseData")]
    public int level;
    public ItemData itemdata;
    public WeaponName weaponname;

    [Header(" # DamageData")]
    public WeaponData weapondata;

    float lastATKtime;
    bool boolAttack = false;
    bool showSparkRange = false;
    List<WeaponSetting> weaponsetting;
    List<Coroutine> coroutines;
    Vector3 originalScale;

    void Awake()
    {
        weaponsetting = new List<WeaponSetting>();
        coroutines = new List<Coroutine>();
    }

    void Update()
    {
        if(itemdata.itemType == ItemData.ItemType.Weapon)
        {
            switch(itemdata.itemId){
                case 0:
                    lastATKtime += Time.deltaTime;
                    if (lastATKtime >= weapondata.CoolTime * InGameManager.instance.player.stat.CoolTime)
                    {
                        BatchRotateSword();
                        lastATKtime = 0;
                    }
                    break;
                case 1:
                    if(boolAttack)
                    {
                        lastATKtime += Time.deltaTime;
                        if(lastATKtime >= weapondata.CoolTime * InGameManager.instance.player.stat.CoolTime)
                        {
                            AttackThrowWeapon();
                            lastATKtime = 0;
                        }
                    }
                    break;
                case 2:
                    lastATKtime += Time.deltaTime;
                    if(lastATKtime >= weapondata.CoolTime * InGameManager.instance.player.stat.CoolTime)
                    {
                        AttackLaser();
                        lastATKtime = 0;
                    }
                    break;
                case 3:
                    lastATKtime += Time.deltaTime;
                    if(lastATKtime >= weapondata.CoolTime * InGameManager.instance.player.stat.CoolTime)
                    {
                        AttackFireBall();
                        lastATKtime = 0;
                    }
                    break;
                case 4:
                    lastATKtime += Time.deltaTime;
                    if(lastATKtime >= weapondata.CoolTime * InGameManager.instance.player.stat.CoolTime)
                    {
                        AttackThunder();
                        lastATKtime = 0;
                    }
                    break;
                case 5:
                    lastATKtime += Time.deltaTime;
                    if(lastATKtime >= weapondata.CoolTime * InGameManager.instance.player.stat.CoolTime)
                    {
                        AttackSpark();
                        lastATKtime = 0;
                    }
                    break;
                case 6:
                    lastATKtime += Time.deltaTime;
                    if(lastATKtime >= weapondata.CoolTime * InGameManager.instance.player.stat.CoolTime)
                    {
                        AttackWave();
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
                showSparkRange = true;
                break;
            case 6:
                BaseSetting(data);
                AttackWave();
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
        InGameManager.instance.player.stat.AddStatus(data.acceData);
    }


    void BaseSetting(ItemData data)
    {
        itemdata = data;
        weapondata = data.weaponData.Clone();
        InGameManager.instance.player.weapon.Add(data.itemId);
        boolAttack = false;
        weaponname = (WeaponName)data.itemId;

        GameManager.instance.InGameData.accumWeaponDamageDict[weaponname] = new AccumWeaponData();
        GameManager.instance.InGameData.accumWeaponDamageDict[weaponname].SetData(data);
    }

    // 무기 레벨업시 실행하는 함수
    public void LevelUp(WeaponData weapon)
    {
        weapondata.Damage += weapon.Damage;
        weapondata.CoolTime -= weapon.CoolTime;
        weapondata.AttackRange += weapon.AttackRange;
        weapondata.Duration += weapon.Duration;
        weapondata.ProjectileCount += weapon.ProjectileCount;
        weapondata.ProjectileSpeed += weapon.ProjectileSpeed;

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
                Debug.Log("ThrowWeapon 동작");
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
            case 6:
                AttackWave();
                break;
        }
    }

    IEnumerator WaitCoroutine(Func<IEnumerator> attackCoroutine)
    {
        boolAttack = false;
        // 무한 루프를 돌며 적을 찾을 때까지 대기
        while (InGameManager.instance.player.scanner == null || InGameManager.instance.player.scanner.nearestTarget == null)
        {
            // 대기 후 다시 확인 (0.1초 주기로 대기)
            yield return new WaitForSeconds(0.1f);
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

    void DeactiveWeaponSetting()
    {
        foreach(WeaponSetting _ in weaponsetting)
        {
            _.gameObject.SetActive(false);
        }
    }

    void BatchRotateSword() // RotateSword 레벨업할때 실행하는 함수(RotateSword 1 사이클 실행)
    {
        DeactiveWeaponSetting(); // 실행되고있는 RotateSword 전부 비활성화
        int totalweapons = weapondata.ProjectileCount + InGameManager.instance.player.stat.ProjectileCount;
        float anglestep = 360f / totalweapons;

        for(int index = 0; index < totalweapons; index++)
        {
            bool isNew;
            Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform;
            if(isNew)
            {
                NewWeaponObjectCreate(weaponT, transform, weaponsetting);
            }

            weaponT.localPosition = Vector3.zero; // 레벨업하면 위치 초기화
            weaponT.localRotation = Quaternion.identity; // 레벨업하면 회전 초기화

             // 각도 계산
            float currentAngle = anglestep * index; // 현재 무기의 각도
            // 회전 벡터 계산 (현재 각도를 라디안으로 변환)
            float radian = currentAngle * Mathf.Deg2Rad;
            Vector3 positionOffset = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0); // 원 주변에 위치시키기 위한 벡터
            // 무기를 해당 위치로 이동시키고 회전 적용
            weaponT.Translate(positionOffset * weapondata.AttackRange * InGameManager.instance.player.stat.AttackRange, Space.World); // 무기의 위치
            weaponT.up = positionOffset.normalized;

            // 무기 설정 초기화
            weaponT.GetComponent<WeaponSetting>().Init(weapondata.Damage * InGameManager.instance.player.stat.AttackPower, -1, weapondata.Knockback, Vector3.zero, weaponname); // 무한 관통이라 per는 -1로 설정
        }
        AttackRotateSword();
        transform.Rotate(Vector3.back * weapondata.ProjectileSpeed * InGameManager.instance.player.stat.ProjectileSpeed * Time.deltaTime);
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
            coroutines.Add(StartCoroutine(rotatesword.AttackWhileDuration(weapondata.Duration * InGameManager.instance.player.stat.Duration)));  // duration 값만큼 무기 지속시킴
        }
    }

    void AttackThrowWeapon() 
    {
        // 코루틴을 시작하고 적을 기다림
        StartCoroutine(WaitCoroutine(AttackThrowWeaponCoroutine));
    }


    IEnumerator AttackThrowWeaponCoroutine() // ThrowWeapon 1회 작동
    {
        int _per = 1;
        bool _levelcheck = false;
        if(level >= 8)
        {
            _levelcheck = true;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.ThrowWeapon);
        }

        int weaponcount = weapondata.ProjectileCount + InGameManager.instance.player.stat.ProjectileCount;
        float _cool = weapondata.CoolTime * InGameManager.instance.player.stat.CoolTime;
        float _speed = weapondata.ProjectileSpeed * InGameManager.instance.player.stat.ProjectileSpeed;
        float interval = _cool / weaponcount;

        for(int i = 0; i < weaponcount; i++)
        {
            Vector3 playerforward = InGameManager.instance.player.transform.right;
            float angle;
            Vector3 dir;
            if(_levelcheck)
            {
                _per = 2;
                interval = 0;
                float angleStep = 60f / (weaponcount - 1); // 부채꼴 범위 (60도) 내에서 균등 분배
                float currentAngle = -30f + (angleStep * i); // -30도 ~ +30도 범위에서 분배
                float radian = (currentAngle + Vector3.SignedAngle(Vector3.right, playerforward, Vector3.forward)) * Mathf.Deg2Rad;

                dir = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0).normalized;
            }
            else
            {
                dir = playerforward;
            }

            bool isNew;
            Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform;
            Transform parent = InGameManager.instance.PoolManager.transform.Find("Weapon").Find("Weapon1");
            if(isNew)
            {
                NewWeaponObjectCreate(weaponT, parent, weaponsetting);
            }
            weaponT.position = transform.position;

            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // dir 벡터의 각도 계산
            weaponT.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            weaponT.GetComponent<WeaponSetting>().Init(weapondata.Damage * InGameManager.instance.player.stat.AttackPower, _per, weapondata.Knockback, dir, weaponname);
            weaponT.GetComponent<WeaponSetting>().StartThrowWhileDuration(5f);

            Rigidbody2D rigid = weaponT.GetComponent<Rigidbody2D>();
            rigid.velocity = dir * _speed;

            yield return new WaitForSeconds(interval);
        }
    }

    void AttackLaser()
    {
        StartCoroutine(AttackLaserCoroutine());
    }

    IEnumerator AttackLaserCoroutine()
    {
        int lasercount = weapondata.ProjectileCount + InGameManager.instance.player.stat.ProjectileCount;
        List<Transform> targets = InGameManager.instance.player.scanner.GetTargetsInScanRange(lasercount);
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
            weaponT.position = InGameManager.instance.player.transform.position;
            float newScale = weapondata.AttackRange * InGameManager.instance.player.stat.AttackRange;
            weaponT.localScale = new Vector3(newScale, newScale, newScale);
            weaponT.GetComponent<WeaponSetting>().Init(weapondata.Damage * InGameManager.instance.player.stat.AttackPower, -1, weapondata.Knockback, Vector3.zero, weaponname); // 무한 관통이라 per는 -1로 설정

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
            
            // 5초 뒤에 자동으로 비활성화
            StartCoroutine(weaponT.GetComponent<WeaponSetting>().ThrowWhileDuration(3f));
            
            // 조준한곳으로 발사
            Rigidbody2D rb = weaponT.GetComponent<Rigidbody2D>();
            float _speed = weapondata.ProjectileSpeed * InGameManager.instance.player.stat.ProjectileSpeed;
            if (rb != null)
            {
                rb.velocity = dir * _speed;
            }
            yield return null;
        }
    }

    void AttackFireBall()
    {
        if(!boolAttack)
        {
            boolAttack = true;
            StartCoroutine(WaitCoroutine(AttackFireBallCoroutine));
        }
    }

    IEnumerator AttackFireBallCoroutine()
    {
        int firballcount = weapondata.ProjectileCount + InGameManager.instance.player.stat.ProjectileCount;
        List<Transform> targets = InGameManager.instance.player.scanner.GetTargetsInScanRange(firballcount);
        Transform parent = InGameManager.instance.PoolManager.transform.Find("Weapon").Find("Weapon3");
        float range = InGameManager.instance.player.stat.AttackRange * InGameManager.instance.player.stat.AttackRange;

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
            weaponT.position = InGameManager.instance.player.transform.position;
            float newScale = weapondata.AttackRange * InGameManager.instance.player.stat.AttackRange;
            weaponT.localScale = new Vector3(newScale, newScale, newScale);
            weaponC.localScale = new Vector3(newScale, newScale, newScale);
            weaponC.GetComponent<WeaponSetting>().Init(weapondata.Damage * InGameManager.instance.player.stat.AttackPower, -1, weapondata.Knockback, Vector3.zero, weaponname);

            Vector3 targetPos;
            if (targets.Count > 0) // 범위내에 적이 존재하면
            {
                // i번 무기를 랜덤 적에게 조준
                int randomenemy = UnityEngine.Random.Range(0, targets.Count);
                targetPos = targets[randomenemy].position;
                targets.RemoveAt(randomenemy);
            }
            else
            {
                // 적이 없으면 i번 무기를 랜덤 방향으로 조준
                Vector2 randomPos = UnityEngine.Random.insideUnitCircle * range;
                targetPos = InGameManager.instance.player.transform.position + new Vector3(randomPos.x, randomPos.y, 0);
            }
            float arcHeight = 2.0f;
            StartCoroutine(MoveInArc(weaponT, weaponC, weaponT.position, targetPos, arcHeight));
        }

        yield return null;
    }


    IEnumerator MoveInArc(Transform weaponT, Transform weaponC, Vector3 startPos, Vector3 targetPos, float arcHeight)
    {
        float time = 0;
        float baseDuration = 5f; // 기본 지속시간
        float speedFactor = weapondata.ProjectileSpeed * InGameManager.instance.player.stat.ProjectileSpeed; // 속도 비율
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
        boolAttack = false;
    }

    void AttackThunder()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Thunder);

        int thundercount = weapondata.ProjectileCount + InGameManager.instance.player.stat.ProjectileCount;
        List<Transform> targets = InGameManager.instance.player.scanner.GetTargetsInScanRange(thundercount);

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
            float newScale = weapondata.AttackRange * InGameManager.instance.player.stat.AttackRange;
            weaponT.localScale = originalScale * newScale;
            weaponT.GetComponent<WeaponSetting>().Init(weapondata.Damage * InGameManager.instance.player.stat.AttackPower, -1, weapondata.Knockback, Vector3.zero, weaponname);

            // 플레이어 화면 내 랜덤한 방향으로 조준
            Vector2 screenPos = new Vector2(
                UnityEngine.Random.Range(0, Screen.width),
                UnityEngine.Random.Range(0, Screen.height)
            );
                
            // 화면 좌표를 월드 좌표로 변환 (z값은 카메라에서 적당히 떨어진 값으로 설정)
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane + 10f));

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
        float _range = weapondata.AttackRange * InGameManager.instance.player.stat.AttackRange;
        // 스캔 범위 내 모든 2D 콜라이더 탐색
        Collider2D[] enemyInRange = Physics2D.OverlapCircleAll(transform.position, _range);

        bool hasEnemy = false;

        foreach (Collider2D enemy in enemyInRange)
        {
            // 적 오브젝트인지 확인
           if (enemy.CompareTag("Enemy"))
            {
                hasEnemy = true;
                Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out bool isNew).transform;
                Transform parent = InGameManager.instance.PoolManager.transform.Find("Weapon").Find("Weapon5");
                if(isNew)
                {
                    NewWeaponObjectCreate(weaponT, parent, weaponsetting);
                }
                weaponT.position = enemy.transform.position;
                enemy.GetComponent<Enemy>().TakeDamage(weapondata.Damage * InGameManager.instance.player.stat.AttackPower, -1, transform.position, WeaponName.Spark);
                
                StartCoroutine(SetActiveWeapon(weaponT.gameObject, 0.3f));
            }
        }
        
        if(hasEnemy)
        {
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Saprk);
        }
    }

    void AttackWave()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Wave);

        bool isNew;
        Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform;
        if(isNew)
        {
            originalScale = weaponT.localScale;
            NewWeaponObjectCreate(weaponT, transform, weaponsetting);
        }
        float newScale = weapondata.AttackRange * InGameManager.instance.player.stat.AttackRange;
        weaponT.localScale = originalScale * newScale;
        weaponT.GetComponent<WeaponSetting>().Init(weapondata.Damage * InGameManager.instance.player.stat.AttackPower, -1, weapondata.Knockback, Vector3.zero, weaponname);

        StartCoroutine(SetActiveWeapon(weaponT.gameObject, 0.45f));
    }

    void OnDrawGizmos()
    {
        float _range = weapondata.AttackRange * InGameManager.instance.player.stat.AttackRange;
        
        // Spark 범위를 파란색으로 표시
        if (showSparkRange)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _range);
        }
    }
}
