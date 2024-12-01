using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    [Header("# GameObject")]
    public Player player;
    public EnemyPoolManager EnemyPoolManager;
    public WeaponManager WeaponManager;
    public GameObject LevelUpPanel;
    public PoolManager PoolManager;

    [Header("# Drop Item")]
    public GameObject[] ExpJewel;

    [Header("# ItemCount")]
    public int JewelCount;
    public int DropItemCount;

    [Header("# Score")]
    public int AccumJewelCount;
    public int HealCount;
    public int GoldCount;
    public int MagnetCount;

    void Awake()
    {
        instance = this;
    }

    void Start() // InGame으로 Scene이 전환되면 시작되는곳
    {
        GameManager.instance.TimerStart();
        AudioManager.instance.PlayBgm(AudioManager.Bgm.InGame);
        LevelUpPanel.SetActive(false);
        CreatePlayerCharacter();
        AudioManager.instance.InGameInit();
    }

    void CreatePlayerCharacter() // Robby Scene에서 선택한 Character를 InGame Scene에 생성하는 함수
    {
        // 1번 인자(오브젝트)를 2번 인자 위치에, 3번 인자로 설정한 회전값으로 생성한다 부모 객체는 4번 인자
        GameObject character = Instantiate(GameManager.instance.SelectCharacter, player.gameObject.transform.position, Quaternion.identity, player.gameObject.transform);
        character.name = "character"; // 객체의 hierarchy상 이름을 character로 설정
        SetWeapon(GameManager.instance.SelectWeapon);
    }

    void SetWeapon(ItemData data)
    {
        Weapon weapon = InGameManager.instance.WeaponManager.transform.Find("Weapon"+data.itemId).GetComponent<Weapon>();
        weapon.gameObject.SetActive(true);
        weapon.Init(data);
        weapon.level++;
    }

    public void GameOver() // 게임 패배시 사용
    {
        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        Debug.Log("게임 패배");
        GameManager.instance.TimerStop();
    }

    public void GameVictory() // 게임 승리시 사용
    {
        StartCoroutine(GameVictoryCoroutine());
    }
    IEnumerator GameVictoryCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        Debug.Log("게임 승리");
        GameManager.instance.TimerStop();
    }
}
