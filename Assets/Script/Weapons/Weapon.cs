using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponData
{
    public float damage = 0;
    public float coolTime = 0;
    public float area = 0;
    public float duration = 0;
    public int count = 0;
    public float speed = 0;
}

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    float lastATKtime;
    public WeaponData data;
    List<RotateSword> rotateSwords;
    List<Coroutine> coroutines;

    Status playerstat;

    void Start()
    {
        playerstat = GameManager.instance.player.GetComponent<Status>();
        rotateSwords = new List<RotateSword>();
        coroutines = new List<Coroutine>();
        // Test Code
        Init();
    }

    void Update()
    {
        switch(id){
            case 0:
                transform.Rotate(Vector3.back * data.speed * playerstat.AttackSpeed * Time.deltaTime);
                lastATKtime += Time.deltaTime;
                if (lastATKtime >= data.coolTime * playerstat.CoolTime)
                    {
                        AttackRotateSword();
                        lastATKtime = 0;
                    }
                break;
            default:
                break;
        }
        // Test Code
        if(Input.GetButtonDown("Jump")){
            var levelupdata = new WeaponData{ damage = 1, count = 1 };
            LevelUp(levelupdata);
        }
    }

    public void Init()
    {
        switch(id){
            case 0:
                transform.position += new Vector3(0, 0.7f, 0);
                BatchRotateSword();
                break;
            default:
                break;
        }
    }

    void BatchRotateSword() // RotateSword 1 사이클 실행
    {
        for(int index=0; index < data.count + playerstat.Amount; index++){
            Transform weaponT;
            
            if(index < transform.childCount){ // index값이 Weapon0의 자식 수보다 적으면 생성돼있는 rotatesword를 그대로 사용
                weaponT = transform.GetChild(index);
            } else { // index가 Weapon0의 자식 수보다 높으면 새로운 rotatesword 생성(count 갯수 추가)
                weaponT = GameManager.instance.weapon.Get(prefabId).transform;
                weaponT.parent = transform; // rotatesword 만든후 부모를 weapon0으로 설정
                var rotatesword = weaponT.GetComponent<RotateSword>();
                rotateSwords.Add(rotatesword);
            }
            weaponT.localPosition = Vector3.zero; // 레벨업하면 위치 초기화
            weaponT.localRotation = Quaternion.identity; // 레벨업하면 회전 초기화

            Vector3 rotVec = Vector3.forward * 360 * index / data.count; // 무기가 여러개여도 일정한 원을 그리며 회전하는 공식
            weaponT.Rotate(rotVec); // 위 공식대로 회전하게 만듦
            weaponT.Translate(weaponT.up * data.area * playerstat.Area, Space.World); // 플레이어와 무기 사이의 거리

            weaponT.GetComponent<RotateSword>().Init(data.damage * playerstat.Damage, -1); // 무한 관통이라 per는 -1로 설정
        }
        AttackRotateSword();
    }

    void AttackRotateSword()
    {
        foreach(var coroutine in coroutines)
        {
            StopCoroutine(coroutine);
        }
        coroutines.Clear();
        foreach(var rotateSword in rotateSwords)
        {
            coroutines.Add(StartCoroutine(rotateSword.AttackWhileDuration(data.duration * playerstat.Duration)));  // duration 값만큼 무기 지속시킴
        }
    }

    public void LevelUp(WeaponData weapon)
    {
        data.damage += weapon.damage;
        data.coolTime -= weapon.coolTime;
        data.area += weapon.area;
        data.duration += weapon.duration;
        data.count += weapon.count;
        data.speed += weapon.speed;

        if(id == 0){
            AttackRotateSword();
            BatchRotateSword();
        }
    }
}
