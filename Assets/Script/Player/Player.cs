using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("# Player Info")]
    public int health;
    public int maxHealth = 100;
    public float moveSpeed = 4f;
    public int level = 0;
    public int exp = 0;
    public List<int> nextExp = new List<int> { 3, };

    [Header("# Player Input")]
    public Vector2 inputVec;
    public GameObject Character;

    Rigidbody2D rigid;

    void Awake()
    {
        health = maxHealth;

        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 플레이어의 방향키 이동값 받아오기
        inputVec.x = Input.GetAxis("Horizontal");
        inputVec.y = Input.GetAxis("Vertical");

        if(inputVec.x > 0){ // 오른쪽으로 이동할때 뒷모습 보여줌
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else { // 정지상태일때나 왼쪽으로 이동할때 앞을 바라봄
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void FixedUpdate()
    {
        Vector2 nextVec = inputVec.normalized * moveSpeed * Time.fixedDeltaTime; // 이동해야할 위치
        rigid.MovePosition(rigid.position + nextVec); // 이동
        // rigid.velocity = Vector2.zero; // 충돌로 밀려나는걸 없애기위함
    }
}
