using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class RotateSword : WeaponBase
{
    private List<WeaponSetting> weaponlist;
    private List<Coroutine> coroutines;

    void Awake()
    {
        weaponlist = new List<WeaponSetting>();
        coroutines = new List<Coroutine>();
    }

    protected override void Update()
    {
        base.Update();
        transform.Rotate(Vector3.back * weapondata.speed * player.stat.ProjectileSpeed * Time.fixedDeltaTime);
    }

    protected override void Attack()
    {
        DeactiveSword(); // 실행되고있는 RotateSword 전부 비활성화
        int totalweapons = weapondata.count + player.stat.ProjectileCount; // 무기의 총 개수 설정
        float anglestep = 360f / totalweapons; // 회전 설정

        for(int index = 0; index < totalweapons; index++)
        {
            bool isNew;
            Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId, out isNew).transform; // 현재 InGame Scene에 존재하는 RotateSword 객체를 받아옴
            if(isNew) // 객체가 새로 만들어졌으면 swordlist에 추가
            {
                weaponT.parent = transform;
                weaponlist.Add(weaponT.GetComponent<WeaponSetting>());
            }

            weaponT.localPosition = Vector3.zero; // 위치 초기화
            weaponT.localRotation = Quaternion.identity; // 회전 초기화

             // 각도 계산
            float currentAngle = anglestep * index; // 현재 무기의 각도
            float radian = currentAngle * Mathf.Deg2Rad; // 회전 벡터 계산 (현재 각도를 라디안으로 변환)
            Vector3 positionOffset = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0); // 원 주변에 위치시키기 위한 벡터

            // 무기를 해당 위치로 이동시키고 회전 적용
            weaponT.Translate(positionOffset * weapondata.area * player.stat.AttackRange, Space.World); // 무기의 위치 설정
            weaponT.up = positionOffset.normalized;

            // sword 객체별 정보 재설정
            weaponT.GetComponent<WeaponSetting>().Init(weapondata.damage * player.stat.AttackPower, -1, weapondata.knockback, Vector3.zero, weaponname); // 무한 관통이라 per는 -1로 설정
        }
        RotateSwordCoroutine();
    }

    private void DeactiveSword() // swordlist에 존재하는 모든 sword 객체를 비활성화
    {
        foreach(WeaponSetting list in weaponlist)
        {
            list.gameObject.SetActive(false);
        }
    }

    private void RotateSwordCoroutine() // sowrd 객체들이 지속시간만큼 동작하도록 Coroutine 부여
    {
        foreach(var coroutine in coroutines) // 진행중인 코루틴 전부 취소
        {
            StopCoroutine(coroutine);
        }
        coroutines.Clear();

        float duration = weapondata.duration * player.stat.Duration;
        foreach(var sword in weaponlist) // sword의 각 객체마다 지속시간 부여하고 동작시킴
        {
            coroutines.Add(StartCoroutine(sword.AttackWhileDuration(duration)));
        }
    }
}
