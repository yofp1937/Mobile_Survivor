using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임의 중요 변수들을 갖고있는 스크립트
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public float gameTime; // 현재 게임 시간
    public float maxGameTime = 6 * 10f; // 최대 게임 시간

    [Header("# Player Info")]
    public int health;
    public int maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

    [Header("# GameObject")]
    public GameObject player;
    public PoolManager pool;
    public WeaponManager weapon;
    public GameObject LevelUpPanel;

    void Awake(){
        instance = this;
        health = maxHealth;
        LevelUpPanel.SetActive(false);
    }
    
    void Update()
    {
        gameTime += Time.deltaTime;
        
        if(gameTime > maxGameTime){
            gameTime = maxGameTime;
        }
    }

    public void GetExp()
    {
        exp++;

        if(exp == nextExp[level]){
            level++;
            LevelUpPanel.SetActive(true);
            Time.timeScale = 0;
            exp = 0;

        }
    }
}
