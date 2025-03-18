using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("# Player Info")]
    [SerializeField] float health;
    public float Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0, MaxHealth);
        }
    }
    public float MaxHealth = 100;
    [SerializeField] float _movespeed;
    [SerializeField] float _magnetrange = 3.5f;
    public int Level;
    int exp;
    public int Exp
    {
        get => exp;
        set
        {
            exp = value;

            if(exp >= NextExp[Level]){
                LevelUp();
            };
        }
    }
    public List<int> WeaponList;
    public List<int> AcceList;
    public int MaxLevelCount;
    public List<int> NextExp = new List<int> { 3, };

    [Header("# Player Input")]
    [SerializeField] Vector2 inputVec;
    public VariableJoystick JoyStick;
    public Vector2 MoveDirection;

    [Header("# Reference Object")]
    public Scanner Scanner;
    public Status Status;
    
    GameObject _character;
    Rigidbody2D _rigid;
    Animator _animator;

    void Awake()
    {
        WeaponList = new List<int>();
        AcceList = new List<int>();
        _rigid = GetComponent<Rigidbody2D>();
        MoveDirection = transform.right;
    }

    public void Init()
    {
        _character = transform.Find("character").gameObject;
        StatusSetting();
        _animator = _character.GetComponent<Animator>();
    }

    void StatusSetting()
    {
        Status.CloneStatus(GameManager.instance.StatusManager.StatusDataList[GameManager.instance.CharacterCode].Stat);
        GameManager.instance.StatusManager.CombineUpgradeStat(Status);
        MaxHealth = Status.Hp;
        Health = MaxHealth;
        _movespeed = Status.MoveSpeed;
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    void Update()
    {
        PullItems();
        RegenHp();
    }

    void FixedUpdate()
    {
        if(InGameManager.instance.living)
        {
            Vector2 nextVec;
            if(GameManager.instance.IsMobile)
            {
                float x = JoyStick.Horizontal;
                float y = JoyStick.Vertical;

                inputVec = new Vector2(x, y);

                if(inputVec.sqrMagnitude > 0.01f) // 조이스틱이 조금만 움직여도 moveSpeed값만큼 이동할수 있게끔 정규화
                {
                    inputVec = inputVec.normalized;
                }
            }
            nextVec = inputVec * _movespeed * Time.fixedDeltaTime; // 이동해야할 위치
            transform.Translate(nextVec); // Player 객체를 이동
            _rigid.velocity = Vector2.zero; // Enemy와 충돌시 밀림현상 방지

            if(inputVec.sqrMagnitude > 0.01f) // 움직이는 방향 저장
            {
                MoveDirection = inputVec;
            }
        }
    }

    void LateUpdate()
    {
        if(InGameManager.instance.living)
        {
            _animator.SetFloat("Speed", inputVec.magnitude); // inputVec의 값이 0보다 크면 walk 애니메이션 실행
            if(inputVec.x > 0)
            {
                _character.transform.rotation = Quaternion.Euler(0, 180, 0);
            } else {
                _character.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        float reducedamage = Mathf.Max(1, Mathf.RoundToInt(damage * (1 - Status.Defense / 100f)));
        Health -= reducedamage;
        GameManager.instance.InGameDataManager.AccumDamage += damage;
        if(Health <= 0)
        {
            InGameManager.instance.CheckGameResult(false);
            _animator.SetTrigger("die");
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    public void GetGold(int count)
    {
        GameManager.instance.InGameDataManager.GetGold += count;
        GameManager.instance.InGameDataManager.GetGold++;
    }

    public void GetPotion(int heal)
    {
        Health += heal;
        GameManager.instance.InGameDataManager.GetPotion++;
        AudioManager.instance.PlaySfx(Sfx.Heal);
    }

    void NeedNextLevelExp()
    {
        int NeedNextLevelExp;
        // 게임 시간별로 다음레벨까지 필요한 경험치량 조절
        // 기본적으로 다음 레벨까지 필요한 경험치는 - 현재 레벨까지 필요했던 경험치 + 추가로 필요한 경험치
        if(GameManager.instance.GameTime > 1500) // 25분 이후부턴 경험치+180 필요
        {
            NeedNextLevelExp = NextExp[Level] + 180;
        }
        else if(GameManager.instance.GameTime > 1200) // 20분 이후부턴 경험치+80 필요
        {
            NeedNextLevelExp = NextExp[Level] + 80;
        }
        else if(GameManager.instance.GameTime > 900) // 15분 이후부턴 경험치+35 필요
        {
            NeedNextLevelExp = NextExp[Level] + 35;
        }
        else if(GameManager.instance.GameTime > 600) // 10분 이후부턴 경험치+15 필요
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
        InGameManager.instance.LevelUpPanel.gameObject.SetActive(true);
        if(GameManager.instance.IsMobile)
        {
            JoyStick.gameObject.SetActive(false);
        }
        exp = 0;
    }

    void PullItems()
    {
        float _range = _magnetrange * Status.ObtainRange;
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

    void RegenHp()
    {
        if(Health >= MaxHealth)
        {
            return;
        }

        float regenhpPerSecond = Status.HpRegen / 3f;
        Health += regenhpPerSecond * Time.deltaTime;
        Health = Mathf.Min(Health, MaxHealth); // 최대체력 초과 방지
    }

    public void ActiveMagnet()
    {
        GameManager.instance.InGameDataManager.GetMagnet++;

        // 모든 Item 태그 객체 탐색
        GameObject[] allItems = GameObject.FindGameObjectsWithTag("Item");

        // 모든 Item 태그 객체의 PullToPlayer 함수 실행
        foreach (GameObject item in allItems)
        {
            item.GetComponent<DropItem>().PullToPlayer();
        }
    }

    // Scene에서 아이템 획득 범위 회색으로 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        if(Status != null)
        {
            Gizmos.DrawWireSphere(transform.position, _magnetrange * Status.ObtainRange);
        }
    }
}