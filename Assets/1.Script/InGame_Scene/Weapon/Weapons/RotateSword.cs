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
        transform.Rotate(Vector3.back * combineProjectileSpeed * Time.fixedDeltaTime);
    }

    protected override void Attack()
    {
        DeactiveSword();
        for(int i = 0; i < combineProjectileCount; i++)
        {
            Transform weaponT = GetObjAndSetBase(PoolList.RotateSword, transform, combineProjectileSize, out bool isNew);
            if(isNew)
            {
                weaponlist.Add(weaponT.GetComponent<WeaponSetting>());
            }
            weaponT = SetDir(weaponT, i);
            weaponT.GetComponent<WeaponSetting>().Init(combineDamage, -1, weapondata.Knockback, Vector3.zero, weaponname);
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
    
    private Transform SetDir(Transform weaponT, int num)
    {
        weaponT.localPosition = Vector3.zero; // 위치 초기화
        weaponT.localRotation = Quaternion.identity; // 회전 초기화
        
        // 각도 계산
        float anglestep = 360f / combineProjectileCount; // sword 객체별 위치 계산
        float currentAngle = anglestep * num; // 현재 sword의 위치 측정
        float radian = currentAngle * Mathf.Deg2Rad; // 현재 sword 위치 radian으로 변환
        Vector3 positionOffset = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0); // 현재 sword 위치 Vector3로 변환
        
        weaponT.Translate(positionOffset * combineAttackRange, Space.World); // 무기의 위치 설정
        weaponT.up = positionOffset.normalized; // 무기 회전 설정

        return weaponT;
    }

    private void RotateSwordCoroutine() // sowrd 객체들이 지속시간만큼 동작하도록 Coroutine 부여
    {
        foreach(var coroutine in coroutines) // 진행중인 코루틴 전부 취소
        {
            StopCoroutine(coroutine);
        }
        coroutines.Clear();

        foreach(var sword in weaponlist) // sword의 각 객체마다 지속시간 부여하고 동작시킴
        {
            coroutines.Add(StartCoroutine(sword.AttackWhileDuration(combineDuration)));
        }
    }
}
