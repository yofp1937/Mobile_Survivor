using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public float offsetZ = -10f;

    void Update()
    {
        GameObject player = GameManager.instance.player;

        if(player != null)
        {
            Vector3 newPosition = player.transform.position;
            newPosition.z = offsetZ;
            transform.position = newPosition;
        }

    }
}
