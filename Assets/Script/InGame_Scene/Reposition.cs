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
        Player playerMove = playerObj.GetComponent<Player>();

        Vector3 playerPos = playerObj.transform.position;
        Vector3 myPos = transform.position;

        float diffX = Mathf.Abs(playerPos.x - myPos.x);
        float diffY = Mathf.Abs(playerPos.y - myPos.y);
        Vector3 playerDir = playerMove.inputVec;
        float dirX = playerDir.x < 0 ? -1 : 1;
        float dirY = playerDir.y < 0 ? -1 : 1;

        switch(transform.tag){
            case "Ground":
                if(diffX > diffY){
                    transform.Translate(Vector3.right * dirX * 136);
                } else {
                    transform.Translate(Vector3.up * dirY * 136);
                }
                break;

            case "Enemy":
                if(coll.enabled){ // Enemy의 isLive가 true면
                    transform.Translate(playerDir * 64 + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f)); // 플레이어의 카메라 밖 랜덤위치에 재생성
                }
                break;
        }
    }
}
