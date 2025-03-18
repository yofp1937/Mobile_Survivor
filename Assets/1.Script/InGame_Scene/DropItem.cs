using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [Header("# Main Data")]
    public int Exp;
    DropItemEnum _item;
    bool isBigJewel = false; // AddExp로 exp가 10이 넘어가서 RedJewel로 바뀌면 true로 변경
    public EquipmentData EquipmentData; 

    [Header("# Reference Data")]
    SpriteRenderer spriter;
    CapsuleCollider2D coll;

    void Awake()
    {
        spriter = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    public void Init(DropItemEnum dropItem)
    {
        _item = dropItem;
        Exp = 0;

        switch(dropItem)
        {
            case DropItemEnum.ExpJewel_1:
                Exp = 1;
                break;
            case DropItemEnum.ExpJewel_3:
                Exp = 3;
                break;
            case DropItemEnum.ExpJewel_5:
                Exp = 5;
                break;
        }

        GameObject prefab = InGameManager.instance.PoolManager.Items[(int)dropItem];
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

    public void AddExp(DropItemEnum dropItem)
    {
        if(dropItem == DropItemEnum.ExpJewel_1)
        {
            Exp += 1;
        }
        else if (dropItem == DropItemEnum.ExpJewel_3)
        {
            Exp += 3;
        }
        else if(dropItem == DropItemEnum.ExpJewel_5)
        {
            Exp += 5;
        }

        if(Exp >= 10 && !isBigJewel)
        {
            isBigJewel = true;
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
        Transform _player = InGameManager.instance.Player.transform;
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
        if(collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if(player != null)
            {
                switch(_item)
                {
                    case DropItemEnum.ExpJewel_1:
                    case DropItemEnum.ExpJewel_3:
                    case DropItemEnum.ExpJewel_5:
                        InGameManager.instance.JewelCount--;
                        player.Exp += Exp;
                        break;
                    case DropItemEnum.Gold:
                        InGameManager.instance.DropItemCount--;
                        player.GetGold(1);
                        break;
                    case DropItemEnum.Magnet:
                        InGameManager.instance.DropItemCount--;
                        player.ActiveMagnet();
                        break;
                    case DropItemEnum.Potion:
                        InGameManager.instance.DropItemCount--;
                        player.GetPotion(20);
                        break;
                    case DropItemEnum.Equipment:
                        GameManager.instance.InGameDataManager.GetEquip.Add(EquipmentData);
                        break;
                }
                gameObject.SetActive(false);
            }
        }
    }
}
