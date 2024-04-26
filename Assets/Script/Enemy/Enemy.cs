using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 3.8f; // 이동속도
    public Rigidbody2D target; // 목표
    bool isLive = true; // 죽었는지 살았는지 체크용

    Rigidbody2D rigid;
    SpriteRenderer spriter;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if(!isLive) 
            return; // 죽어있으면 리턴

        Vector2 dirVec = target.position - rigid.position; // 방향 구하기
        Vector2 nextVec = dirVec.normalized * moveSpeed * Time.fixedDeltaTime; // 이동해야할 위치
        rigid.MovePosition(rigid.position + nextVec); // 이동
        rigid.velocity = Vector2.zero; // 충돌로 밀려나는걸 없애기위함
    }

    void LateUpdate()
    {
        if(!isLive)
            return; // 죽어있으면 리턴

        spriter.flipX = target.position.x < rigid.position.x; // 플레이어 위치에따라 좌우 모습 변경
    }
}
