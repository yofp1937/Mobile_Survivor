using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("# Main Data")]
    float _health;
    int _damage;
    float _moveSpeed = 3.8f; // 이동속도
    Rigidbody2D _target; // 목표
    bool _isLive;
    bool _isKnockback;

    [Header("# Reference Data")]
    Rigidbody2D _rigid;
    SpriteRenderer _spriter;
    Animator _anim;
    CapsuleCollider2D _coll;

    void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _spriter = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _coll = GetComponent<CapsuleCollider2D>();
    }

    void OnEnable()
    {
        _isLive = true;
        _isKnockback = false;
        SetDamage();
    }    

    void FixedUpdate()
    {
        if(!_isLive || _isKnockback) 
            return; // 죽어있거나 넉백 적용중이면 리턴

        Vector2 dirVec = _target.position - _rigid.position; // 방향 구하기
        Vector2 nextVec = dirVec.normalized * _moveSpeed * Time.fixedDeltaTime; // 이동해야할 위치
        _rigid.MovePosition(_rigid.position + nextVec);
        _rigid.velocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if(!_isLive)
            return; // 죽어있으면 리턴

        _spriter.flipX = _target.position.x < _rigid.position.x; // 플레이어 위치에따라 좌우 모습 변경
    }

    public void Init(SpawnData data, PoolEnum monstertype)
    {
        _target = InGameManager.instance.player.GetComponent<Rigidbody2D>();
        _moveSpeed = data.speed;
        _health = data.health;
        gameObject.name = "Enemy";
        gameObject.transform.parent = InGameManager.instance.PoolManager.transform.Find("Enemy");

        GameObject prefab = InGameManager.instance.PoolManager.GetPrefab(monstertype);
        Animator prefabanim = prefab.GetComponent<Animator>();
        CapsuleCollider2D prefabcoll = prefab.GetComponent<CapsuleCollider2D>();
        _anim.runtimeAnimatorController = prefabanim.runtimeAnimatorController;
        _coll.size = prefabcoll.size;
        _coll.offset = prefabcoll.offset;
        _coll.direction = prefabcoll.direction;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player")){
            Player player = collision.collider.GetComponent<Player>();
            if(player != null)
            {
                player.TakeDamage(_damage);
            }
        }
    }

    public void TakeDamage(float damage, float knockbackForce, Vector3 attackerPosition, WeaponEnum weaponname)
    {
        var InGameData = GameManager.instance.InGameData;

        if(!_isLive)
            return;

        // 치명타 검사
        bool isCritical = Random.Range(0f, 100f) < InGameManager.instance.player.Status.CriticalChance;
        if(isCritical)
        {
            float CriticalDamagePer = InGameManager.instance.player.Status.CriticalDamage;
            damage *= 1 + (CriticalDamagePer / 100f); // 백분율로 계산
        }

        _health -= damage;
        InGameData.accumWeaponDamage += damage; // 총 누적 데미지 증가
        InGameData.accumWeaponDamageDict[weaponname].AddDamage(damage); // 무기별 누적 데미지 증가
        ShowDamagePopup(damage, isCritical); // 데미지 팝업 생성

        if (_health > 0)
        {
            Knockback(knockbackForce, attackerPosition);
        }
        else
        {
            _isLive = false;
            Dead();
            InGameData.kill++;
        }
    }

    void SetDamage()
    {
        if(GameManager.instance.gameTime >= 1680) // 28분 이후
        {
            _damage = 40;
        }
        else if(GameManager.instance.gameTime >= 1440) // 24분 이후
        {
            _damage = 30;
        }
        else if(GameManager.instance.gameTime >= 1200) // 20분 이후
        {
            _damage = 20;
        }
        else if(GameManager.instance.gameTime >= 900) // 15분 이후
        {
            _damage = 15;
        }
        else if(GameManager.instance.gameTime >= 600) // 10분 이후
        {
            _damage = 10;
        }
        else
        {
            _damage = 5;
        }
    }

    void Knockback(float knockbackForce, Vector3 attackerPosition)
    {
        Vector2 knockbackDirection = ((Vector2)transform.position - (Vector2)attackerPosition).normalized;
        StartCoroutine(ApplyKnockback(knockbackDirection, knockbackForce));
    }

    IEnumerator ApplyKnockback(Vector2 direction, float force)
    {
        _isKnockback = true;
        _rigid.velocity = Vector2.zero; // 기존 속도 제거
        _rigid.AddForce(direction * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f); // 넉백 효과를 0.5초 동안 유지 (원하는 시간으로 조절 가능)
        _isKnockback = false;
    }

    void ShowDamagePopup(float damage, bool isCritical)
    {
        Vector3 popupPosition = transform.position + new Vector3(0, 0.3f, 0); // 데미지 팝업 생성 위치 조정
        InGameManager.instance.PoolManager.GetDmgPopup(damage, popupPosition, isCritical); // 데미지 팝업 생성
    }

    void Dead()
    {
        DropJewel();
        DropItem();
        gameObject.SetActive(false);
    }

    void DropJewel()
    {
        int JewelCount = InGameManager.instance.JewelCount;
        DropItemEnum dropItem;
        PoolEnum poolItem;

        if(GameManager.instance.gameTime >= 1680) // 28분부턴 경험치 5
        {
            dropItem = DropItemEnum.ExpJewel_5;
            poolItem = PoolEnum.ExpJewel_5;
        }
        else if(GameManager.instance.gameTime >= 1200) // 20분부턴 경험치 3
        {
            dropItem = DropItemEnum.ExpJewel_3;
            poolItem = PoolEnum.ExpJewel_3;
        }
        else
        {
            dropItem = DropItemEnum.ExpJewel_1;
            poolItem = PoolEnum.ExpJewel_1;
        }

        if(JewelCount >= 150) // 보석의 갯수가 150개가 넘어가면 가장 가까운 보석의 경험치량을 증가시킴
        {
            DropItem nearjewel = null;
            float finddistance = Mathf.Infinity;

            foreach (Transform jewelT in InGameManager.instance.PoolManager.transform.Find("Item")) // 풀매니저 Item.Exp의 모든 자식객체들을 순회하면서 거리 측정
            {
                if(jewelT.GetComponent<DropItem>().Exp < 1) // 경험치 보석 아니면 건너뜀
                {
                    continue;
                }

                float distance = Vector3.Distance(gameObject.transform.position, jewelT.position); // 현재 객체에서 jewelT까지의 거리 계산
                if (distance < finddistance) // dinstance가 finddistance보다 작으면 더 가까운거니깐 finddistance 갱신
                {
                    finddistance = distance;
                    nearjewel = jewelT.GetComponent<DropItem>(); // 이번 jewelT가 제일 가까운 jewel이므로 nearjewel에 등록
                }
            }

            // 가장 가까운 보석의 경험치 증가
            if (nearjewel != null)
            {
                nearjewel.AddExp(dropItem); // 경험치 증가 함수 호출
            }
        }
        else
        {
            Transform itemT = InGameManager.instance.PoolManager.Get(poolItem, out bool isNew).transform;
            itemT.position = gameObject.transform.position;
            itemT.parent = InGameManager.instance.PoolManager.transform.Find("Item");
            itemT.GetComponent<DropItem>().Init(dropItem);
        }
    }

    void DropItem()
    {
        float randomValue = Random.Range(0f,100f);
        PoolEnum poolItem;
        DropItemEnum dropItem = DropItemEnum.Potion;

        if (randomValue < 0.1f)  // 0.1% 확률로 자석 드랍
        {
            poolItem = PoolEnum.Magnet;
            dropItem = DropItemEnum.Magnet;
        }
        else if (randomValue < 1.6f)  // 1.5% 확률로 골드 드랍 (0.1f 이상 1.6f 미만)
        {
            poolItem = PoolEnum.Gold;
            dropItem = DropItemEnum.Gold;
        }
        else if (randomValue < 4.1f)  // 2.5% 확률로 포션 드랍 (1.6f 이상 4.1f 미만)
        {
            poolItem = PoolEnum.Potion;
            dropItem = DropItemEnum.Potion;
        }
        else
        {
            poolItem = PoolEnum.None;
        }

        if(poolItem != PoolEnum.None && InGameManager.instance.DropItemCount < 50) // None이 아니고, 필드에 존재하는 아이템 개수가 50개 미만이면 아이템 소환
        {
            InGameManager.instance.DropItemCount++;

            Transform itemT = InGameManager.instance.PoolManager.Get(poolItem, out bool isNew).transform;
            itemT.position = gameObject.transform.position;
            itemT.parent = InGameManager.instance.PoolManager.transform.Find("Item");

            itemT.GetComponent<DropItem>().Init(dropItem);
        }
    }
}
