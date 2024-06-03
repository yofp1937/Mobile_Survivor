using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float gameTime; // 현재 게임 시간
    public float maxGameTime = 6 * 10f; // 최대 게임 시간
    public GameObject player;
    public PoolManager pool;
    public WeaponManager weapon;

    void Awake(){
        instance = this;
    }
    
    void Update()
    {
        gameTime += Time.deltaTime;
        
        if(gameTime > maxGameTime){
            gameTime = maxGameTime;
        }
    }
}
