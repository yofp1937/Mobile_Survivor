using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("# Monster Info")]
    public float moveSpeed = 3.8f; // 이동속도
    public Rigidbody2D target; // 목표
    public float health;
    public int damage;

    bool isLive; // 죽었는지 살았는지 체크용
    bool isKnockback; // 넉백 상태 체크용
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    CapsuleCollider2D coll;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    void OnEnable()
    {
        isLive = true;
        isKnockback = false;
        SetDamage();
    }    

    void FixedUpdate()
    {
        if(!isLive || isKnockback) 
            return; // 죽어있거나 넉백 적용중이면 리턴

        Vector2 dirVec = target.position - rigid.position; // 방향 구하기
        Vector2 nextVec = dirVec.normalized * moveSpeed * Time.fixedDeltaTime; // 이동해야할 위치
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if(!isLive)
            return; // 죽어있으면 리턴

        spriter.flipX = target.position.x < rigid.position.x; // 플레이어 위치에따라 좌우 모습 변경
    }

    public void Init(SpawnData data, int monstertype)
    {
        target = InGameManager.instance.player.GetComponent<Rigidbody2D>();
        moveSpeed = data.speed;
        health = data.health;
        gameObject.name = "Enemy";

        GameObject prefab = InGameManager.instance.EnemyPoolManager.prefabs[monstertype];
        Animator prefabanim = prefab.GetComponent<Animator>();
        CapsuleCollider2D prefabcoll = prefab.GetComponent<CapsuleCollider2D>();
        anim.runtimeAnimatorController = prefabanim.runtimeAnimatorController;
        coll.size = prefabcoll.size;
        coll.offset = prefabcoll.offset;
        coll.direction = prefabcoll.direction;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player")){
            Player player = collision.collider.GetComponent<Player>();
            if(player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(float damage, float knockbackForce, Vector3 attackerPosition, WeaponName weaponname)
    {
        if(!isLive)
            return;

        health -= damage;
        GameManager.instance.accumWeaponDamage += damage; // 총 누적 데미지 증가
        GameManager.instance.accumWeaponDamageDict[weaponname].AddDamage(damage); // 무기별 누적 데미지 증가
        ShowDamagePopup(damage); // 데미지 팝업 생성

        if (health > 0)
        {
            Knockback(knockbackForce, attackerPosition);
        }
        else
        {
            isLive = false;
            Dead();
            GameManager.instance.kill++;
        }
    }

    void SetDamage()
    {
        // 데미지 설정
        // 28분 이후 데미지 - 30
        // 24분 이후 데미지 - 25
        // 20분 이후 데미지 - 20
        // 15분 이후 데미지 - 15
        // 10분 이후 데미지 - 10
        // 초반 5
        if(GameManager.instance.gameTime >= 1680) // 28분 이후
        {
            damage = 30;
        }
        else if(GameManager.instance.gameTime >= 1440) // 24분 이후
        {
            damage = 25;
        }
        else if(GameManager.instance.gameTime >= 1200) // 20분 이후
        {
            damage = 20;
        }
        else if(GameManager.instance.gameTime >= 900) // 15분 이후
        {
            damage = 15;
        }
        else if(GameManager.instance.gameTime >= 600) // 10분 이후
        {
            damage = 10;
        }
        else
        {
            damage = 5;
        }
    }

    void Knockback(float knockbackForce, Vector3 attackerPosition)
    {
        Vector2 knockbackDirection = ((Vector2)transform.position - (Vector2)attackerPosition).normalized;
        StartCoroutine(ApplyKnockback(knockbackDirection, knockbackForce));
    }

    IEnumerator ApplyKnockback(Vector2 direction, float force)
    {
        isKnockback = true;
        rigid.velocity = Vector2.zero; // 기존 속도 제거
        rigid.AddForce(direction * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f); // 넉백 효과를 0.5초 동안 유지 (원하는 시간으로 조절 가능)
        isKnockback = false;
    }

    void ShowDamagePopup(float damage)
    {
        Vector3 popupPosition = transform.position + new Vector3(0, 0.3f, 0); // 데미지 팝업 생성 위치 조정
        PopupPoolManager.instance.Get(damage, popupPosition); // 데미지 팝업 생성
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
        int index;

        if(GameManager.instance.gameTime >= 1680) // 28분부턴 경험치 5
        {
            index = 2;
        }
        else if(GameManager.instance.gameTime >= 1200) // 20분부턴 경험치 3
        {
            index = 1;
        }
        else
        {
            index = 0;
        }

        if(JewelCount >= 200) // 보석의 갯수가 200개가 넘어가면 가장 가까운 보석의 경험치량을 증가시킴
        {
            DropItem nearjewel = null;
            float finddistance = Mathf.Infinity;

            foreach (Transform jewelT in InGameManager.instance.PoolManager.transform.Find("Item")) // 풀매니저 Item.Exp의 모든 자식객체들을 순회하면서 거리 측정
            {
                if(!jewelT.GetComponent<DropItem>().jewel) // jewelT가 경험치 보석이 아니면 아래 코드 실행 안함
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
                nearjewel.AddExp(index); // 경험치 증가 함수 호출
            }
        }
        else
        {
            Transform itemT = InGameManager.instance.PoolManager.Get(index).transform;
            itemT.position = gameObject.transform.position;
            itemT.parent = InGameManager.instance.PoolManager.transform.Find("Item");
            itemT.GetComponent<DropItem>().Init(index);
        }
    }

    void DropItem()
    {
        float randomValue = Random.Range(0f,100f);
        int index;

        if (randomValue < 0.1f)  // 0.1% 확률로 자석 드랍
        {
            index = 5;
        }
        else if (randomValue < 1.6f)  // 1.5% 확률로 골드 드랍 (0.1f 이상 1.6f 미만)
        {
            index = 3;
        }
        else if (randomValue < 4.6f)  // 3% 확률로 포션 드랍 (1.6f 이상 4.6f 미만)
        {
            index = 4;
        }
        else // 아이템이 안뜨면 index = 0
        {
            index = 0;
        }

        if(index > 0 && InGameManager.instance.DropItemCount <= 49) // index가 0보다 크고, 필드에 존재하는 아이템 개수가 50개 미만이면 아이템 소환
        {
            InGameManager.instance.DropItemCount++;

            Transform itemT = InGameManager.instance.PoolManager.Get(index).transform;
            itemT.position = gameObject.transform.position;
            itemT.parent = InGameManager.instance.PoolManager.transform.Find("Item");

            itemT.GetComponent<DropItem>().Init(index);
        }
    }
}
