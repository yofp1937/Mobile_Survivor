using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D collision)
    {
        if(!collision.CompareTag("Area"))
            return;

        GameObject playerObj = GameManager.instance.player;
        Move playerMove = playerObj.GetComponent<Move>();

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
                } else if(diffX < diffY){
                    transform.Translate(Vector3.up * dirY * 136);
                }
                break;
            case "Enemy":

                break;
        }
    }
}
