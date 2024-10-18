using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("# Monster Info")]
    public float moveSpeed = 3.8f; // 이동속도
    public Rigidbody2D target; // 목표
    public float health;

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

    void Start()
    {
        target = InGameManager.instance.player.GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        isLive = true;
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

    public void Init(SpawnData data)
    {
        moveSpeed = data.speed;
        health = data.health;
        gameObject.name = "Enemy";

        GameObject prefab = InGameManager.instance.EnemyPoolManager.prefabs[data.spriteType];
        Animator prefabanim = prefab.GetComponent<Animator>();
        CapsuleCollider2D prefabcoll = prefab.GetComponent<CapsuleCollider2D>();
        anim.runtimeAnimatorController = prefabanim.runtimeAnimatorController;
        coll.size = prefabcoll.size;
        coll.offset = prefabcoll.offset;
        coll.direction = prefabcoll.direction;
    }

    public void TakeDamage(float damage, float knockbackForce, Vector3 attackerPosition)
    {
        if(!isLive)
            return;

        health -= damage;
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
        gameObject.SetActive(false);
    }

    void DropJewel()
    {
        int index;

        if(GameManager.instance.gameTime > 1200)
        {
            index = 2;
        }
        else if(GameManager.instance.gameTime > 600)
        {
            index = 1;
        }
        else
        {
            index = 0;
        }

        Transform itemT = InGameManager.instance.PoolManager.Get(index).transform;
        itemT.position = gameObject.transform.position;
        itemT.parent = InGameManager.instance.PoolManager.transform.Find("Item");
        itemT.GetComponent<ExpJewel>().Init(index);
    }

}
