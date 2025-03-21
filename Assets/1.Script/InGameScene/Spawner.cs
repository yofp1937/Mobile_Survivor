using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("# Main Data")]
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    float timer;
    int level; // 몬스터 소환 레벨

    [Header("# Boss Data")] // 보스를 4번 이상 소환할거면 Boss 스크립트의 DropEquip도 손봐야함
    bool[] isBossSpawn = {false, false, false};
    readonly float[] bossSpawnTimes = { 450f, 900f, 1500f }; // 보스 등장 시간 (7분30초, 15분, 25분)
    readonly int[] bossHealth = { 5000, 10000, 20000 };
    readonly float[] bossMoveSpeeds = { 3f, 3.2f, 3.4f };

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 시간에 따라 변수 설정
        CheckGameTimeAndSetting();

        // 보스 소환 체크
        CheckAndSpawnBoss();
        
        // 몬스터 스폰
        CheckSpawnTime();
    }

    void CheckGameTimeAndSetting() // GameTime에따른 설정 변경
    {
        // 게임 시간이 30분 이상이면 게임 정지
        if (GameManager.instance.GameTime >= 1800f)
        {
            GameManager.instance.TimerStop();
            return;
        }

        // 게임 시간에 따른 spawnData 레벨 증가 규칙
        if (GameManager.instance.GameTime < 1200f) // 20분 이하
        {
            level = Mathf.FloorToInt(GameManager.instance.GameTime / 120f); // 2분마다 레벨 1 증가 (0~10레벨)
        }
        else if (GameManager.instance.GameTime >= 1200f && GameManager.instance.GameTime < 1800f) // 20분 이상 30분 미만
        {
            level = 10 + Mathf.FloorToInt((GameManager.instance.GameTime - 1200f) / 240f); // 4분마다 레벨 1 증가 (11, 12레벨)
        }
    }

    void CheckAndSpawnBoss()
    {
        for(int i = 0; i < bossSpawnTimes.Length; i++)
        {
            if(!isBossSpawn[i] && GameManager.instance.GameTime >= bossSpawnTimes[i])
            {
                SpawnBoss(i);
                isBossSpawn[i] = true;
            }
        }
    }

    void SpawnBoss(int num)
    {
        GameObject boss = InGameManager.instance.PoolManager.Get(PoolEnum.Boss, out bool isNew);
        boss.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position;

        SpawnData spawndata = new SpawnData(0f, bossHealth[num], bossMoveSpeeds[num]);
        boss.GetComponent<Boss>().BossNum = num;
        boss.GetComponent<Boss>().Init(spawndata, PoolEnum.Boss);
    }

    void CheckSpawnTime()
    {
        float spawntime = spawnData[level].spawnTime * InGameManager.instance.Player.Status.Curse;

        if(timer > spawntime){
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        PoolEnum[] monsters = { PoolEnum.FlyEye, PoolEnum.Goblin, PoolEnum.Mushroom, PoolEnum.Skeleton };
        PoolEnum randommonster;
        switch (level) // 시간대별 스폰되는 몬스터 변경
        {
            case int n when (n <= 2): // 30~24분 사이
                randommonster = PoolEnum.FlyEye;
                break;
            case int n when (n <= 4): // 24~20분 사이
                randommonster = PoolEnum.Goblin;
                break;
            case int n when (n <= 6): // 20~16분 사이
                randommonster = monsters[Random.Range(0, 1)];
                break;
            case int n when (n <= 7): // 16~14분
                randommonster = PoolEnum.Mushroom;
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
                randommonster = PoolEnum.Skeleton;
                break;
        }
        SpawnMonster(randommonster);
    }

    void SpawnMonster(PoolEnum monstertype)
    {
        GameObject enemy = InGameManager.instance.PoolManager.Get(monstertype, out bool isNew);
        // Range()안에 1부터 하는이유는 플레이어와 겹쳐져있는 자신(0번 Spawner)을 제외하기위해
        enemy.transform.position = spawnPoint[Random.Range(1,spawnPoint.Length)].position;
        enemy.GetComponent<EnemyBase>().Init(spawnData[level], monstertype);
    }
}