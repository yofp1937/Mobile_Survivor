using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer1 : MonoBehaviour
{
    public float offsetZ = -10f;

    void Update()
    {
        GameObject player = GameManager1.instance.player.gameObject;

        if(player != null)
        {
            Vector3 newPosition = player.transform.position;
            newPosition.z = offsetZ;
            transform.position = newPosition;
        }

    }
}
