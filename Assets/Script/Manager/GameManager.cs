using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("# Game Control")]
    public float gameSpeed = 1f; // 게임 속도
    public float gameTime; // 현재 게임 시간
    public float maxGameTime = 30 * 60f; // 최대 게임 시간
    public int kill; // 잡은 몬스터 수
    public int gold; // 획득한 골드량

    [Header("# Player Select")]
    public GameObject SelectCharacter;
    public int SelectWeaponId;
    public int SelectArtifactId;

    private bool timerrunning = false; // InGame Scene로 이동하면 시간을 측정하기위함


    void Awake()
    {
        instance = this; // 싱글톤 패턴 구현

        var obj = FindObjectsOfType<GameManager>(); // obj로 GameManager를 전부 찾아와 배열로 변환
        if(obj.Length == 1) // obj가 1개면 씬 전환에도 값이 유지되게 설정
        {
            DontDestroyOnLoad(gameObject);
        }
        else // obj가 1개보다 많으면 현재 obj 파괴
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if(timerrunning)
        {
            gameTime += Time.deltaTime;
        }
    }

    public void TimerStart() // InGame Scene에 입장하면 실행됨
    {
        timerrunning = true;
        Time.timeScale = gameSpeed;
    }

    public void TimerStop() // InGame Scene에서 Player의 hp가 0이되면 실행됨
    {
        timerrunning = false;
        Time.timeScale = 0;
    }
}
