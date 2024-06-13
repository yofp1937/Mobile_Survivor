using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3.8f; // 이동속도
    public Rigidbody2D target; // 목표
    public float health;
    public float maxHealth;

    bool isLive; // 죽었는지 살았는지 체크용
    bool isKnockback; // 넉백 상태 체크용

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if(!isLive || isKnockback) 
            return; // 죽어있거나 넉백 적용중이면 리턴

        Vector2 dirVec = target.position - rigid.position; // 방향 구하기
        Vector2 nextVec = dirVec.normalized * moveSpeed * Time.fixedDeltaTime; // 이동해야할 위치
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        if(!isLive)
            return; // 죽어있으면 리턴

        spriter.flipX = target.position.x < rigid.position.x; // 플레이어 위치에따라 좌우 모습 변경
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        moveSpeed = data.speed;
        maxHealth = data.health;
        health = data.health;
    }

    public void TakeDamage(float damage, float knockbackForce, Vector3 attackerPosition)
    {
        health -= damage;
        ShowDamagePopup(damage); // 데미지 팝업 생성

        if (health > 0)
        {
            Knockback(knockbackForce, attackerPosition);
        }
        else
        {
            // anim.SetTrigger("hp_zero");
            Dead();
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
        PopupManager.instance.Get(damage, popupPosition); // 데미지 팝업 생성
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
