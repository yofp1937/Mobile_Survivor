using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowWeapon : WeaponBase
{
    [Header("# Weapon Data")]
    private int per; // 무기의 관통력
    private float interval; // 한사이클에서 무기들의 투척 간격
    Vector3 playerforward;

    [Header("# Tools")]
    private bool levelcheck;

    void Awake()
    {
    }

    protected override void Attack()
    {
        MergeWeaponAndPlayerStats(); // 스탯 동기화
        WeaponLevelCheck(); // 이번 사이클에서 사용할 Weapon Data 리셋
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.ThrowWeapon);
        for(int i = 0; i < combineProjectileCount; i++)
        {
            // ThrowWeapon 객체 재사용 or 생성
            bool isNew;
            Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform;
            Transform parent = poolManager.transform.Find("Weapon").Find("Weapon1"); // 부모를 Player 외부의 객체로 설정(Player가 부모면 플레이어의 움직임에따라 단검도 같이 움직임)
            if(isNew)
            {
                weaponT.parent = parent;
            }
            weaponT.position = transform.position; // 위치 설정

            playerforward = player.moveDirection.normalized; // 플레이어가 바라보는 방향
            Vector3 dir = CalcThrowDirection(i); // 무기 각도 계산

            float angle; // 무기가 적을 바라보게 설정하기위한 변수
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; // dir 벡터의 각도 계산
            weaponT.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // 적을 바라보게 설정

            weaponT.GetComponent<WeaponSetting>().Init(combineDamage, per, weapondata.Knockback, dir, weaponname); // 객체 기본 정보 설정
            weaponT.GetComponent<WeaponSetting>().StartThrowWhileDuration(5f); // 객체 지속시간 설정(5초 후 비활성화)

            Rigidbody2D rigid = weaponT.GetComponent<Rigidbody2D>();
            
            rigid.velocity = dir * combineProjectileSpeed; // 날아가는 속도 부여
            
            if(!levelcheck && i > 0) 
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.ThrowWeapon); // 최대레벨 아니면 무기 던질때마다 사운드 출력
            }

            yield return new WaitForSeconds(interval);
        }
    }

    void WeaponLevelCheck()
    {
        if(level >= 8) // 8이 최고 레벨
        {
            levelcheck = true;
            per = 2;
            interval = 0;
        }
        else
        {
            levelcheck = false;
            per = 1;
            interval = 0.15f;
        }
    }

    Vector3 CalcThrowDirection(int count) // 무기 각도 계산
    {
        Vector3 dir; // 무기 던지는 방향

        if(levelcheck) // 무기가 최대레벨일때 작동방식 변경(부채꼴로 _weaponcount만큼의 단검을 투척)
        {
            float anglestep = 60f / combineProjectileCount; // 부채꼴 범위 (60도) 내에서 균등 분배
            float currentAngle = -30f + (anglestep * count); // -30도 ~ +30도 범위에서 분배
            float radian = (currentAngle + Vector3.SignedAngle(Vector3.right, playerforward, Vector3.forward)) * Mathf.Deg2Rad; // 각도를 라디안으로 변환

            dir = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0).normalized; // 방향 정규화
        }
        else // 무기가 최대레벨이 아닐때
        {
            dir = playerforward;
        }

        return dir;
    }
}
