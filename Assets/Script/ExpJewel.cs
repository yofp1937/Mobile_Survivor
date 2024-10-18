using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpJewel : MonoBehaviour
{
    public int exp;
    Rigidbody2D rigid;
    CapsuleCollider2D coll;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    public void Init(int index)
    {
        if(index == 1)
        {
            exp = 3;
        }
        else if (index == 2)
        {
            exp = 5;
        }
        else
        {
            exp = 1;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")){
            Player player = collision.GetComponent<Player>();
            if(player != null)
            {
                player.GetExp(exp);
                gameObject.SetActive(false);
            }
        }
    }
}
