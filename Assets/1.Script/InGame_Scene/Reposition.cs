using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(!collision.CompareTag("Area"))
            return;

        GameObject playerObj = InGameManager.instance.player.gameObject;
        Vector3 playerPos = playerObj.transform.position;
        Vector3 myPos = transform.position;

        switch(transform.tag){
            case "Ground":
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if(diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 136);
                } else {
                    transform.Translate(Vector3.up * dirY * 136);
                }
                break;

            case "Enemy":
                if(coll.enabled) // Enemy가 활성화상태면
                {
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
                    transform.Translate(ran + dist * 2); // 플레이어의 카메라 밖 랜덤위치에 재생성
                }
                break;
        }
    }
}
