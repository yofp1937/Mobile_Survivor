using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("# Base Data")]
    [SerializeField] public int level; // 장비의 현재 레벨
    public ItemData itemdata; // 장비의 전체 정보

    [Header("# Weapon Data")]
    [SerializeField] protected WeaponData weapondata; // 무기의 Stat 정보
    public WeaponName weaponname; // 무기 이름

    [Header("# Acce Data")]
    [SerializeField] protected AcceData accedata; // 악세의 Stat 정보

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
        player = InGameManager.instance.player.GetComponent<Player>();
        poolManager = InGameManager.instance.PoolManager;
        MergeWeaponAndPlayerStats();
    }

    public void Init(ItemData data) // 외부에서 무기 획득시 호출해서 무기의 정보를 설정하는 함수
    {
        level++;

        // 기본 정보 설정
        itemdata = data;
        weapondata = data.weaponData.Clone();
        player.weapon.Add(data.itemId);
        weaponname = (WeaponName)data.itemId;

        // 데미지 통계 정보 설정
        GameManager.instance.InGameData.accumWeaponDamageDict[weaponname] = new AccumWeaponData();
        GameManager.instance.InGameData.accumWeaponDamageDict[weaponname].SetData(data);
    }

    public void InitAcce(ItemData data)
    {
        itemdata = data;
        player.accesorries.Add(data.itemId);
        player.stat.AddStatus(data.acceData);
    }

    public void WeaponLevelUp() // 무기 레벨업시 호출하는 함수
    {
        var data = itemdata.levelupdata_weapon[level-1];

        weapondata.Damage += data.Damage;
        weapondata.CoolTime -= data.CoolTime;
        weapondata.AttackRange += data.AttackRange;
        weapondata.Duration += data.Duration;
        weapondata.ProjectileCount += data.ProjectileCount;
        weapondata.ProjectileSpeed += data.ProjectileSpeed;
        weapondata.ProjectileSize += data.ProjectileSize;
        weapondata.Knockback += data.Knockback;

        if(itemdata.itemType == ItemData.ItemType.Weapon && level == itemdata.maxlevel - 1)
        {
            player.maxlevelcount++;
        }

        lastATKtime = 100;
    }

    protected void MergeWeaponAndPlayerStats() // 무기 스탯과 플레이어 스탯 결합
    {
        combineDamage = player.stat.AttackPower * (weapondata.Damage / 100f);
        combineCoolTime = weapondata.CoolTime * player.stat.CoolTime;
        combineAttackRange = weapondata.AttackRange * player.stat.AttackRange;
        combineDuration = weapondata.Duration * player.stat.Duration;
        combineProjectileCount = weapondata.ProjectileCount + player.stat.ProjectileCount;
        combineProjectileSpeed = weapondata.ProjectileSpeed * player.stat.ProjectileSpeed;
        combineProjectileSize = weapondata.ProjectileSize * player.stat.ProjectileSize;
    }

    protected virtual void Update() // 쿨타임마다 공격 실행
    {
        if(itemdata.itemType == ItemData.ItemType.Weapon)
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

    protected virtual Transform GetObjAndSetBase(PoolList type, Transform parent, float scaleparam, out bool isNew)
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
