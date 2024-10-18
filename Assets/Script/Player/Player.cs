using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("# Player Info")]
    public int health;
    public int maxHealth = 100;
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

    [Header("# Reference Object")]
    public Scanner scanner;
    public Status stat;

    GameObject Character;
    Rigidbody2D rigid;
    Animator anim;

    void Awake()
    {
        health = maxHealth;
    }
    
    void Start()
    {
        Character = transform.Find("character").gameObject;
        scanner = GetComponent<Scanner>();
        rigid = GetComponent<Rigidbody2D>();
        anim = Character.GetComponent<Animator>();
        stat = GetComponent<Status>();
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
        Vector2 nextVec = inputVec * moveSpeed * Time.fixedDeltaTime; // 이동해야할 위치
        transform.Translate(nextVec); // Player 객체를 이동
        rigid.velocity = Vector2.zero; // Enemy와 충돌시 밀림현상 방지
    }

    void LateUpdate()
    {
        anim.SetFloat("Speed", inputVec.magnitude); // inputVec의 값이 0보다 크면 walk 애니메이션 실행
        if(inputVec.x > 0)
        {
            Character.transform.rotation = Quaternion.Euler(0, 180, 0);
        } else {
            Character.transform.rotation = Quaternion.Euler(0, 0, 0);
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
    }

    void NeedNextLevelExp()
    {
        int NeedNextLevelExp;
        if(GameManager.instance.gameTime > 1200)
        {
            NeedNextLevelExp = nextExp[level] + 150;
        }
        else if(GameManager.instance.gameTime > 600)
        {
            NeedNextLevelExp = nextExp[level] + 70;
        }
        else if(GameManager.instance.gameTime > 300)
        {
            NeedNextLevelExp = nextExp[level] + 30;
        }
        else if(GameManager.instance.gameTime > 150)
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
        Time.timeScale = 0;
        InGameManager.instance.LevelUpPanel.SetActive(true);
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
}