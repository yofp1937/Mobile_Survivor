using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("# Player Info")]
    public float health;
    public float maxHealth = 100;
    public float moveSpeed = 4f;
    public float scanRange;
    public float magnetRange;
    public int level;
    public int exp;
    public List<int> weapon;
    public List<int> accesorries;
    public int maxlevelcount;
    public List<int> nextExp = new List<int> { 3, };

    [Header("# Player Input")]
    public Vector2 inputVec;
    public VariableJoystick joy;

    [Header("# Reference Object")]
    public Scanner scanner;
    public Status stat;

    GameObject Character;
    Rigidbody2D rigid;
    Animator anim;
    
    void Start()
    {
        Character = transform.Find("character").gameObject;
        scanner = GetComponent<Scanner>();
        rigid = GetComponent<Rigidbody2D>();
        anim = Character.GetComponent<Animator>();
        stat = GetComponent<Status>();
        health = maxHealth;
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    void Update()
    {
        PullItems();
    }

    void FixedUpdate()
    {
        if(InGameManager.instance.living)
        {
            Vector2 nextVec;
            if(GameManager.instance.IsMobile)
            {
                float x = joy.Horizontal;
                float y = joy.Vertical;

                inputVec = new Vector2(x, y);
                nextVec = inputVec * moveSpeed * Time.fixedDeltaTime;
            }
            else
            {
                nextVec = inputVec * moveSpeed * Time.fixedDeltaTime; // 이동해야할 위치
            }
            transform.Translate(nextVec); // Player 객체를 이동
            rigid.velocity = Vector2.zero; // Enemy와 충돌시 밀림현상 방지
        }
    }

    void LateUpdate()
    {
        if(InGameManager.instance.living)
        {
            anim.SetFloat("Speed", inputVec.magnitude); // inputVec의 값이 0보다 크면 walk 애니메이션 실행
            if(inputVec.x > 0)
            {
                Character.transform.rotation = Quaternion.Euler(0, 180, 0);
            } else {
                Character.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    public void GetExp(int _exp)
    {
        exp += _exp;

        if(exp >= nextExp[level]){
            LevelUp();
        }
    }

    public void GetHeal(int count)
    {
        if(health + count > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += count;
        }
        GameManager.instance.getPotion++;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Heal);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        GameManager.instance.accumDamage += damage;
        if(health <= 0)
        {
            InGameManager.instance.GameOver();
            anim.SetTrigger("die");
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    public void GetGold(int count)
    {
        GameManager.instance.getGold += count;
        GameManager.instance.getGold++;
    }

    void NeedNextLevelExp()
    {
        int NeedNextLevelExp;
        // 게임 시간별로 다음레벨까지 필요한 경험치량 조절
        // 기본적으로 다음 레벨까지 필요한 경험치는 - 현재 레벨까지 필요했던 경험치 + 추가로 필요한 경험치
        if(GameManager.instance.gameTime > 1200) // 25분 이후부턴 경험치+180 필요
        {
            NeedNextLevelExp = nextExp[level] + 180;
        }
        else if(GameManager.instance.gameTime > 1200) // 20분 이후부턴 경험치+80 필요
        {
            NeedNextLevelExp = nextExp[level] + 80;
        }
        else if(GameManager.instance.gameTime > 900) // 15분 이후부턴 경험치+35 필요
        {
            NeedNextLevelExp = nextExp[level] + 35;
        }
        else if(GameManager.instance.gameTime > 600) // 10분 이후부턴 경험치+15 필요
        {
            NeedNextLevelExp = nextExp[level] + 15;
        }
        else
        {
            NeedNextLevelExp = nextExp[level] + 5;
        }

        nextExp.Add(NeedNextLevelExp);
    }

    public void LevelUp()
    {
        NeedNextLevelExp();
        level++;
        GameManager.instance.TimerStop();
        AudioManager.instance.EffectBgm(true);
        InGameManager.instance.LevelUpPanel.SetActive(true);
        if(GameManager.instance.IsMobile)
        {
            joy.gameObject.SetActive(false);
        }
        exp = 0;
    }

    void PullItems()
    {
        float _range = magnetRange * stat.Magnet;
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
        GameManager.instance.getMagnet++;

        // 모든 Item 태그 객체 탐색
        GameObject[] allItems = GameObject.FindGameObjectsWithTag("Item");

        // 모든 Item 태그 객체의 PullToPlayer 함수 실행
        foreach (GameObject item in allItems)
        {
            item.GetComponent<DropItem>().PullToPlayer();
        }
    }

}