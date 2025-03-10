using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public int exp;
    public int type;
    public bool jewel = false;
    bool BigJewel = false; // AddExp로 exp가 10이 넘어가서 RedJewel로 바뀌면 true로 변경
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    CapsuleCollider2D coll;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    public void Init(int index)
    {
        type = index;
        if(index == 0)
        {
            exp = 1;
            jewel = true;
        }
        else if (index == 1)
        {
            exp = 3;
            jewel = true;
        }
        else if (index == 2)
        {
            exp = 5;
            jewel = true;
        }
        else
        {
            exp = 0;
            jewel = false;
        }

        GameObject prefab = InGameManager.instance.PoolManager.Items[index];
        SpriteRenderer Pspriter = prefab.GetComponent<SpriteRenderer>();
        CapsuleCollider2D prefabcoll = prefab.GetComponent<CapsuleCollider2D>();

        // 오브젝트 이름, 크기 설정
        gameObject.name = "DropItem";
        gameObject.transform.localScale = prefab.transform.localScale;

        // 오브젝트 SpriteRenderer 설정
        spriter.sprite = Pspriter.sprite;
        spriter.color = Pspriter.color;
        spriter.sortingOrder = Pspriter.sortingOrder;

        // 오브젝트 Collider 설정
        coll.size = prefabcoll.size;
        coll.offset = prefabcoll.offset;
        coll.direction = prefabcoll.direction;
    }

    public void AddExp(int index)
    {
        if(index == 0)
        {
            exp += 1;
        }
        else if (index == 1)
        {
            exp += 3;
        }
        else if(index == 2)
        {
            exp += 5;
        }

        if(exp >= 10 && !BigJewel)
        {
            BigJewel = true;
            gameObject.transform.localScale = new Vector3(3.192213f, 3.192213f, 3.192213f);
            spriter.color = Color.red;
        }
    }

    public void PullToPlayer()
    {
        // 객체가 활성화 돼있으면
        if(gameObject.activeSelf)
        {
            // Coroutine 실행
            StartCoroutine(PullToPlayerCoroutine());
        }
    }

    IEnumerator PullToPlayerCoroutine()
    {
        Transform _player = InGameManager.instance.player.transform;
        // 이동 속도 설정
        float _speed = 8f;
        
        while (gameObject.activeSelf) // 객체가 활성화되어 있는 동안 계속 반복
        {
            // 플레이어에게 끌어당김
            gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, _player.position, _speed * Time.deltaTime);
            yield return null; // 다음 프레임까지 대기
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")){
            Player player = collision.GetComponent<Player>();
            if(player != null)
            {
                if(type >= 0 && type <= 2) // item이 경험치 보석이면
                {
                    InGameManager.instance.JewelCount--;
                    player.GetExp(exp);
                }
                else if(type == 3) // item이 gold면
                {
                    InGameManager.instance.DropItemCount--;
                    player.GetGold(1);
                }
                else if(type == 4) // item이 magnet이면
                {
                    InGameManager.instance.DropItemCount--;
                    player.ActiveMagnet();
                }
                else if(type == 5) // item이 potion이면
                {
                    InGameManager.instance.DropItemCount--;
                    player.GetHeal(20);
                }
                gameObject.SetActive(false);
            }
        }
    }
}
