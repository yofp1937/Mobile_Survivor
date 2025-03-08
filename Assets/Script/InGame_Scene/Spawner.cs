using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    float timer;
    int level; // 몬스터 소환 레벨

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 게임 시간이 30분 이상이면 게임 정지
        if (GameManager.instance.gameTime >= 1800f)
        {
            GameManager.instance.TimerStop();
            return;
        }

        // 게임 시간에 따른 레벨 증가 규칙
        if (GameManager.instance.gameTime < 1200f) // 20분 이하
        {
            level = Mathf.FloorToInt(GameManager.instance.gameTime / 120f); // 2분마다 레벨 1 증가 (0~10레벨)
        }
        else if (GameManager.instance.gameTime >= 1200f && GameManager.instance.gameTime < 1800f) // 20분 이상 30분 미만
        {
            level = 10 + Mathf.FloorToInt((GameManager.instance.gameTime - 1200f) / 240f); // 4분마다 레벨 1 증가 (11, 12레벨)
        }

        if(timer > spawnData[level].spawnTime){
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        PoolList[] monsters = { PoolList.FlyEye, PoolList.Goblin, PoolList.Mushroom, PoolList.Skeleton };
        PoolList randommonster;
        switch (level) // 시간대별 스폰되는 몬스터 변경
        {
            case int n when (n <= 2): // 30~24분 사이
                randommonster = PoolList.FlyEye;
                break;
            case int n when (n <= 4): // 24~20분 사이
                randommonster = PoolList.Goblin;
                break;
            case int n when (n <= 6): // 20~16분 사이
                randommonster = monsters[Random.Range(0, 1)];
                break;
            case int n when (n <= 7): // 16~14분
                randommonster = PoolList.Mushroom;
                break;
            case int n when (n <= 9): // 14~10분
                randommonster = monsters[Random.Range(1, 2)];
                break;
            case int n when (n <= 10): // 10~6분
                randommonster = monsters[Random.Range(0, 2)];
                break;
            case int n when (n <= 11): // 6~2분
                randommonster = monsters[Random.Range(0, 3)];
                break;
            default: // 2~0분
                randommonster = PoolList.Skeleton;
                break;
        }

        GameObject enemy = InGameManager.instance.PoolManager.Get(randommonster, out bool isNew);
        // Range()안에 1부터 하는이유는 플레이어와 겹쳐져있는 자신(0번 Spawner)을 제외하기위해
        enemy.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level], randommonster);
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime; // 리젠 시간
    public int health; // 몬스터 체력
    public float speed; // 몬스터 속도
}