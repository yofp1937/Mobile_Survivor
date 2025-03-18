using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("# Main Data")]
    [SerializeField] float _health;
    [SerializeField] int _damage;
    float _moveSpeed;
    bool _isLive;
    bool _isKnockback;
    float _knockbackResis;
    int[] _difficultyStat = { 1, 2, 4, 8, 16 };

    [Header("# Reference Data")]
    [SerializeField] Rigidbody2D _target;
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

    void FixedUpdate() // _target 방향으로 이동
    {
        if(!_isLive || _isKnockback) return;

        Vector2 dirVec = _target.position - _rigid.position;
        Vector2 nextVec = dirVec.normalized * _moveSpeed * Time.fixedDeltaTime;
        _rigid.MovePosition(_rigid.position + nextVec);
        _rigid.velocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if(!_isLive) return;

        _spriter.flipX = _target.position.x < _rigid.position.x; // 플레이어 위치에따라 좌우 모습 변경
    }

    void SetDamage()
    {
        if(GameManager.instance.GameTime >= 1680) // 28분 이후
        {
            _damage = 40;
        }
        else if(GameManager.instance.GameTime >= 1440) // 24분 이후
        {
            _damage = 30;
        }
        else if(GameManager.instance.GameTime >= 1200) // 20분 이후
        {
            _damage = 20;
        }
        else if(GameManager.instance.GameTime >= 900) // 15분 이후
        {
            _damage = 15;
        }
        else if(GameManager.instance.GameTime >= 600) // 10분 이후
        {
            _damage = 10;
        }
        else
        {
            _damage = 5;
        }
        _damage *= _difficultyStat[(int)GameManager.instance.DifficultyLevel];
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

    public void Init(SpawnData data, PoolEnum monstertype) // 외부에서 Enemy 데이터 설정
    {
        if(monstertype == PoolEnum.Boss)
        {
            gameObject.name = "Boss";
            _knockbackResis = 0.333f;
        }
        else
        {
            gameObject.name = "Enemy";
            _knockbackResis = 0f;
        }
        _target = InGameManager.instance.Player.GetComponent<Rigidbody2D>();
        _moveSpeed = data.moveSpeed;
        _health = data.health * _difficultyStat[(int)GameManager.instance.DifficultyLevel];
        gameObject.transform.parent = InGameManager.instance.PoolManager.transform.Find("Enemy");

        GameObject prefab = InGameManager.instance.PoolManager.GetPrefab(monstertype);
        Animator prefabanim = prefab.GetComponent<Animator>();
        CapsuleCollider2D prefabcoll = prefab.GetComponent<CapsuleCollider2D>();
        _anim.runtimeAnimatorController = prefabanim.runtimeAnimatorController;
        _coll.size = prefabcoll.size;
        _coll.offset = prefabcoll.offset;
        _coll.direction = prefabcoll.direction;
    }

    public void TakeDamage(float damage, float knockbackForce, Vector3 attackerPosition, WeaponEnum weaponname)
    {
        var InGameData = GameManager.instance.InGameDataManager;

        if(!_isLive)
            return;

        // 치명타 검사
        bool isCritical = Random.Range(0f, 100f) < InGameManager.instance.Player.Status.CriticalChance;
        if(isCritical)
        {
            float CriticalDamagePer = InGameManager.instance.Player.Status.CriticalDamage;
            damage *= 1 + (CriticalDamagePer / 100f); // 백분율로 계산
        }

        _health -= damage;
        InGameData.AccumWeponData[weaponname].TotalDamage += damage; // 무기별 누적 데미지 증가
        ShowDamagePopup(damage, isCritical); // 데미지 팝업 생성

        if (_health > 0)
        {
            Knockback(knockbackForce, attackerPosition);
        }
        else
        {
            _isLive = false;
            Dead();
            InGameData.Kill++;
        }
    }

    void Knockback(float knockbackForce, Vector3 attackerPosition)
    {
        Vector2 knockbackDirection = ((Vector2)transform.position - (Vector2)attackerPosition).normalized;
        StartCoroutine(ApplyKnockback(knockbackDirection, knockbackForce));
    }

    IEnumerator ApplyKnockback(Vector2 direction, float force)
    {
        float adjustKnockback = force * (1 - _knockbackResis); // 넉백저항 적용
        _isKnockback = true;
        _rigid.velocity = Vector2.zero; // 기존 속도 제거
        _rigid.AddForce(direction * adjustKnockback, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f); // 넉백 효과를 0.3초 동안 유지
        _isKnockback = false;
    }

    void ShowDamagePopup(float damage, bool isCritical)
    {
        Vector3 popupPosition = transform.position + new Vector3(0, 0.3f, 0); // 데미지 팝업 생성 위치 조정
        InGameManager.instance.PoolManager.GetDmgPopup(damage, popupPosition, isCritical); // 데미지 팝업 생성
    }

    protected virtual void Dead()
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

        if(GameManager.instance.GameTime >= 1680) // 28분부턴 경험치 5
        {
            dropItem = DropItemEnum.ExpJewel_5;
            poolItem = PoolEnum.ExpJewel_5;
        }
        else if(GameManager.instance.GameTime >= 1200) // 20분부턴 경험치 3
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
