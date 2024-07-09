using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 몬스터의 소환을 담당하는 스크립트
public class Spawner1 : MonoBehaviour
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
        level = Mathf.FloorToInt(GameManager1.instance.gameTime / 10f);

        if(timer > spawnData[level].spawnTime){
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        GameObject enemy = GameManager1.instance.pool.Get(level);
        // Range()안에 1부터 하는이유는 플레이어와 겹쳐져있는 자신(0번 Spawner)을 제외하기위해
        enemy.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position;
        enemy.GetComponent<Enemy1>().Init(spawnData[level]);
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime; // 리젠 시간
    public int spriteType; // 몬스터 종류 변경용
    public int health; // 몬스터 체력
    public float speed; // 몬스터 속도
}