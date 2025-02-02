using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("# Player Info")]
    public float Health;
    public int Level;
    public int Exp;
    public List<int> Weapon;
    public List<int> Accesorries;
    public int MaxLevelCount;
    public List<int> NextExp = new List<int> { 3, };

    [Header("# Player Input")]
    public Vector2 InputVec;

    [Header("# Reference Object")]
    public Scanner Scanner;
    public Status Status;

    GameObject character;
    Rigidbody2D rigid;
    Animator anim;
    
    void Start() // 객체 연결
    {
        character = transform.Find("character").gameObject;
        Scanner = GetComponent<Scanner>();
        rigid = GetComponent<Rigidbody2D>();
        anim = character.GetComponent<Animator>();
        Status = GetComponent<Status>();
        Init();
    }

    void Init() // 변수 초기화
    {
        Status.AddStatus(character.GetComponent<Status>());
        // 체력 초기화
        Health = Status.Hp;
    }
    
    void SynchronizeStatus() // 스테이터스 동기화(캐릭터 기본 스테이터스 + 강화 스테이터스 + 장비 스테이터스)
    {
        Status characstat = character.GetComponent<Status>();

        Status.AddStatus(characstat);
        // 강화와 장비 스텟도 추가해야함
    }

    void OnMove(InputValue value)
    {
        InputVec = value.Get<Vector2>();
    }

    void Update()
    {
        PullItems();
    }

    void FixedUpdate()
    {
        if(InGameManager.instance.living)
        {
            Vector2 nextVec = InputVec * Status.MoveSpeed * Time.fixedDeltaTime; // 이동해야할 위치
            transform.Translate(nextVec); // Player 객체를 이동
            rigid.velocity = Vector2.zero; // Enemy와 충돌시 밀림현상 방지
        }
    }

    void LateUpdate()
    {
        if(InGameManager.instance.living)
        {
            anim.SetFloat("Speed", InputVec.magnitude); // inputVec의 값이 0보다 크면 walk 애니메이션 실행
            if(InputVec.x > 0)
            {
                character.transform.rotation = Quaternion.Euler(0, 180, 0);
            } else {
                character.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    public void GetExp(int _exp)
    {
        Exp += _exp;

        if(Exp >= NextExp[Level]){
            LevelUp();
        }
    }

    public void GetHeal(int count)
    {
        if(Health + count > Status.Hp)
        {
            Health = Status.Hp;
        }
        else
        {
            Health += count;
        }
        GameManager.instance.InGameData.getPotion++;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Heal);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        GameManager.instance.InGameData.accumDamage += damage;
        if(Health <= 0)
        {
            InGameManager.instance.GameOver();
            anim.SetTrigger("die");
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    public void GetGold(int count)
    {
        GameManager.instance.InGameData.getGold += count;
        GameManager.instance.InGameData.getGold++;
    }

    void NeedNextLevelExp()
    {
        int NeedNextLevelExp;
        // 게임 시간별로 다음레벨까지 필요한 경험치량 조절
        // 기본적으로 다음 레벨까지 필요한 경험치는 - 현재 레벨까지 필요했던 경험치 + 추가로 필요한 경험치
        if(GameManager.instance.gameTime > 1200) // 25분 이후부턴 경험치+180 필요
        {
            NeedNextLevelExp = NextExp[Level] + 180;
        }
        else if(GameManager.instance.gameTime > 1200) // 20분 이후부턴 경험치+80 필요
        {
            NeedNextLevelExp = NextExp[Level] + 80;
        }
        else if(GameManager.instance.gameTime > 900) // 15분 이후부턴 경험치+35 필요
        {
            NeedNextLevelExp = NextExp[Level] + 35;
        }
        else if(GameManager.instance.gameTime > 600) // 10분 이후부턴 경험치+15 필요
        {
            NeedNextLevelExp = NextExp[Level] + 15;
        }
        else
        {
            NeedNextLevelExp = NextExp[Level] + 5;
        }

        NextExp.Add(NeedNextLevelExp);
    }

    public void LevelUp()
    {
        NeedNextLevelExp();
        Level++;
        GameManager.instance.TimerStop();
        AudioManager.instance.EffectBgm(true);
        InGameManager.instance.LevelUpPanel.SetActive(true);
        Exp = 0;
    }

    void PullItems()
    {
        float _range = Status.ObtainRange * Status.ObtainRange;
        // 스캔 범위 내 모든 2D 콜라이더 탐색
        Collider2D[] itemsInRange = Physics2D.OverlapCircleAll(transform.position, _range);

        foreach (Collider2D item in itemsInRange)
        {
            // 아이템 오브젝트인지 확인
           if (item.CompareTag("Item"))
            {
                float _speed = 8f; // 이동 속도
                item.transform.position = Vector2.MoveTowards(item.transform.position, transform.position, _speed * Time.deltaTime);
            }
        }
    }

    public void ActiveMagnet()
    {
        GameManager.instance.InGameData.getMagnet++;

        // 모든 Item 태그 객체 탐색
        GameObject[] allItems = GameObject.FindGameObjectsWithTag("Item");

        // 모든 Item 태그 객체의 PullToPlayer 함수 실행
        foreach (GameObject item in allItems)
        {
            item.GetComponent<DropItem>().PullToPlayer();
        }
    }

}