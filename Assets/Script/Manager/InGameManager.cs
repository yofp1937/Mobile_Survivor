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
    public GameObject[] expjewel;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GameManager.instance.TimerStart();
        LevelUpPanel.SetActive(false);
        CreatePlayerCharacter();
    }

    public void CreatePlayerCharacter()
    {
        // 1번 인자(오브젝트)를 2번 인자 위치에, 3번 인자로 설정한 회전값으로 생성한다 부모 객체는 4번 인자
        GameObject character = Instantiate(GameManager.instance.SelectCharacter, player.gameObject.transform.position, Quaternion.identity, player.gameObject.transform);
        character.name = "character"; // 객체의 hierarchy상 이름을 character로 설정
    }
}
