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
            Time.timeScale = 0; // 게임 정지
            Debug.Log("게임이 정지되었습니다.");
            return; // 아래 코드를 실행하지 않음
        }

        // 게임 시간에 따른 레벨 증가 규칙
        if (GameManager.instance.gameTime <= 600f) // 10분 이하
        {
            level = Mathf.FloorToInt(GameManager.instance.gameTime / 60f); // 1분마다 레벨 1 증가
        }
        else if (GameManager.instance.gameTime <= 1200f) // 10~20분 사이 (600~1200초)
        {
            level = 10 + Mathf.FloorToInt((GameManager.instance.gameTime - 600f) / 120f); // 2분마다 레벨 1 증가
        }
        else if (GameManager.instance.gameTime <= 1800f) // 20~30분 사이 (1200~1800초)
        {
            level = 15 + Mathf.FloorToInt((GameManager.instance.gameTime - 1200f) / 240f); // 4분마다 레벨 1 증가
        }

        if(timer > spawnData[level].spawnTime){
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        GameObject enemy = InGameManager.instance.EnemyPoolManager.Get(spawnData[level].spriteType);
        // Range()안에 1부터 하는이유는 플레이어와 겹쳐져있는 자신(0번 Spawner)을 제외하기위해
        enemy.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[level]);
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