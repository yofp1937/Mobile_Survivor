using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float moveSpeed = 4f;
    public Vector2 inputVec;
    Rigidbody2D rigid;
    private Animator ani;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        inputVec.x = Input.GetAxis("Horizontal");
        inputVec.y = Input.GetAxis("Vertical");

        if(inputVec.x > 0){ // 왼쪽으로 이동할때 뒷모습 보여줌
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else { // 정지상태일때나 오른쪽으로 이동할때 앞을 바라봄
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // Vector의 크기가 0보다 크면(움직이는 상태면) move_bool 값을 true로 변경(walk 애니메이션 동작)
        if (inputVec.magnitude > 0)
        {
            ani.SetBool("move_bool", true);
        }
        else
        {
            ani.SetBool("move_bool", false);
        }
    }

    void FixedUpdate()
    {
        rigid.velocity = inputVec * moveSpeed;
    }
}