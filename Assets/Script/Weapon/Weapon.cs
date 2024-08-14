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
    List<WeaponSetting> rotateSwords;
    List<Coroutine> coroutines;

    Player player;
    Status playerstat;

    void Awake()
    {
        player = GetComponentInParent<Player>();
        playerstat = InGameManager.instance.player.GetComponent<Status>();
        rotateSwords = new List<WeaponSetting>();
        coroutines = new List<Coroutine>();
    }

    void Update()
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
                lastATKtime += Time.deltaTime;

                if(lastATKtime >= weapondata.coolTime * playerstat.CoolTime){
                    AttackThrowWeapon();
                    lastATKtime = 0;
                }
                break;
            default:
                break;
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
            default:
                break;
        }
    }

    void BaseSetting(ItemData data)
    {
        itemdata = data;
        weapondata = data.weaponData.Clone();
        level++;
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

        switch(itemdata.itemId)
        {
            case 0:
                BatchRotateSword();
                break;
            case 1:
                AttackThrowWeapon();
                break;
        }
    }

    void BatchRotateSword() // RotateSword 레벨업할때 실행하는 함수(RotateSword 1 사이클 실행)
    {
        Debug.Log(InGameManager.instance);
        for(int index=0; index < weapondata.count + playerstat.Amount; index++){
            Transform weaponT;
            
            if(index < transform.childCount){ // index값이 Weapon0의 자식 수보다 적으면 생성돼있는 rotatesword를 그대로 사용
                weaponT = transform.GetChild(index);
            } else { // index가 Weapon0의 자식 수보다 높으면 새로운 rotatesword 생성(count 갯수 추가)
                weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId).transform;
                weaponT.parent = transform; // rotatesword 만든후 부모를 weapon0으로 설정
                var rotatesword = weaponT.GetComponent<WeaponSetting>();
                rotateSwords.Add(rotatesword);
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
        foreach(var rotateSword in rotateSwords) // rotatesword의 각 객체(검)마다 지속시간, 쿨타임 부여하고 동작시킴
        {
            coroutines.Add(StartCoroutine(rotateSword.AttackWhileDuration(weapondata.duration * playerstat.Duration)));  // duration 값만큼 무기 지속시킴
        }
    }

    void AttackThrowWeapon() // ThrowWeapon 레벨업 할때 실행하는 함수
    {
        if(!player.scanner.nearestTarget) // 대상 없으면 실행 X
            return;

        StartCoroutine(AttackThrowWeaponCoroutine());
    }

    IEnumerator AttackThrowWeaponCoroutine() // ThrowWeapon 1회 작동
    {
        for(int i = 0; i < weapondata.count + playerstat.Amount; i++)
        {
            Vector3 targetPos = player.scanner.nearestTarget.position;
            Vector3 dir = targetPos - transform.position;
            dir = dir.normalized;

            Transform weaponT = InGameManager.instance.WeaponManager.Get(itemdata.itemId).transform;
            weaponT.parent = InGameManager.instance.PoolParent.transform.Find("Weapon");
            weaponT.position = transform.position;

            Quaternion rotation = Quaternion.FromToRotation(Vector2.up, dir);
            weaponT.rotation = Quaternion.Euler(0, 0, rotation.eulerAngles.z);

            weaponT.GetComponent<WeaponSetting>().Init(weapondata.damage * playerstat.Damage, 0, weapondata.knockback, dir);

            Rigidbody2D rb = weaponT.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = dir * weapondata.speed * playerstat.AttackSpeed;
            }

            yield return new WaitForSeconds(0.35f);
        }
    }
}
