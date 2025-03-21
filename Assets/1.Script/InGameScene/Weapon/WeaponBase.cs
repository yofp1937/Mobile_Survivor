using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("# Base Data")]
    [SerializeField] public int level; // 장비의 현재 레벨
    public WeaponData WeaponData; // 장비의 전체 정보

    [Header("# Weapon Data")]
    [SerializeField] protected WeaponStatusData _wStatusData; // 무기의 Stat 정보
    public WeaponEnum weaponname; // 무기 이름
    public AccesorriesEnum accename; // 장신구 이름

    [Header("# Acce Data")]
    [SerializeField] protected Status accedata; // 악세의 Stat 정보

    [Header("# Combine Status")]
    protected float combineDamage;
    protected float combineCoolTime;
    protected float combineAttackRange;
    protected float combineDuration;
    protected int combineProjectileCount;
    protected float combineProjectileSpeed;
    protected float combineProjectileSize;
    
    [Header("# Secondary Data")]
    private float lastATKtime;
    protected Vector3 scale;

    [Header("# Referenced")]
    protected Player player;
    protected PoolManager poolManager;

    void OnEnable()
    {
        player = InGameManager.instance.Player.GetComponent<Player>();
        poolManager = InGameManager.instance.PoolManager;
        MergeWeaponAndPlayerStats();
    }

    public void Init(WeaponData data) // 외부에서 무기 획득시 호출해서 무기의 정보를 설정하는 함수
    {
        // 기본 정보 설정
        WeaponData = data;
        _wStatusData = data.weaponData.Clone();
        player.WeaponList.Add(data.itemId);
        weaponname = (WeaponEnum)data.itemId;

        // 데미지 통계 정보 설정
        GameManager.instance.InGameDataManager.AccumWeponData[weaponname] = new AccumWeaponData();
        GameManager.instance.InGameDataManager.AccumWeponData[weaponname].Data = data;
    }

    public void InitAcce(WeaponData data)
    {
        WeaponData = data;
        player.AcceList.Add(data.itemId);
        player.Status.AddStatus(data.acceData);
        accename = (AccesorriesEnum)data.itemId;

        GameManager.instance.InGameDataManager.AccumAcceData[accename] = new AccumWeaponData();
        GameManager.instance.InGameDataManager.AccumAcceData[accename].Data = data;
    }

    public void WeaponLevelUp() // 무기 레벨업시 호출하는 함수
    {
        var data = WeaponData.levelupdata_weapon[level-1];

        _wStatusData.Damage += data.Damage;
        _wStatusData.CoolTime -= data.CoolTime;
        _wStatusData.AttackRange += data.AttackRange;
        _wStatusData.Duration += data.Duration;
        _wStatusData.ProjectileCount += data.ProjectileCount;
        _wStatusData.ProjectileSpeed += data.ProjectileSpeed;
        _wStatusData.ProjectileSize += data.ProjectileSize;
        _wStatusData.Knockback += data.Knockback;

        if(WeaponData.itemType == WeaponData.ItemType.Weapon && level == WeaponData.maxlevel - 1)
        {
            player.MaxLevelCount++;
        }

        lastATKtime = 100;
    }

    protected void MergeWeaponAndPlayerStats() // 무기 스탯과 플레이어 스탯 결합
    {
        combineDamage = player.Status.AttackPower * (_wStatusData.Damage / 100f);
        combineCoolTime = _wStatusData.CoolTime * player.Status.CoolTime;
        combineAttackRange = _wStatusData.AttackRange * player.Status.AttackRange;
        combineDuration = _wStatusData.Duration * player.Status.Duration;
        combineProjectileCount = _wStatusData.ProjectileCount + player.Status.ProjectileCount;
        combineProjectileSpeed = _wStatusData.ProjectileSpeed * player.Status.ProjectileSpeed;
        combineProjectileSize = _wStatusData.ProjectileSize * player.Status.ProjectileSize;
    }

    protected virtual void Update() // 쿨타임마다 공격 실행
    {
        if(WeaponData.itemType == WeaponData.ItemType.Weapon)
        {
            lastATKtime += Time.deltaTime;

            if(lastATKtime >= combineCoolTime)
            {
                MergeWeaponAndPlayerStats(); // 스탯 동기화
                Attack();
                lastATKtime = 0;
            }
        }
    }

    protected abstract void Attack(); // 오버라이딩해서 사용할것

    protected virtual Transform GetObjAndSetBase(PoolEnum type, Transform parent, float scaleparam, out bool isNew)
    {
        isNew = false;
        Transform weaponT = poolManager.Get(type, out bool _isnew).transform;

        if(_isnew)
        {
            isNew = true;
            weaponT.parent = parent;
            scale = weaponT.localScale;
        }
        weaponT.localScale = scale * scaleparam;

        return weaponT;
    }
}
