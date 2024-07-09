using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 게임의 중요 변수들을 갖고있는 스크립트
public class GameManager1 : MonoBehaviour
{
    public static GameManager1 instance;
    [Header("# Game Control")]
    public float gameTime; // 현재 게임 시간
    public float maxGameTime = 6 * 10f; // 최대 게임 시간
    public int kill; // 잡은 몬스터 수

    [Header("# GameObject")]
    public Player1 player;
    public PoolManager1 pool;
    public WeaponManager1 weapon;
    public GameObject LevelUpPanel;
    public Item1 item;

    void Awake(){
        instance = this;
        LevelUpPanel.SetActive(false);
    }
    
    void Update()
    {
        gameTime += Time.deltaTime;
        
        if(gameTime > maxGameTime){
            gameTime = maxGameTime;
        }
    }

}
