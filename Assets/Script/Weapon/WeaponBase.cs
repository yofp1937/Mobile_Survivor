using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("# Base Data")]
    [SerializeField] public int level; // 무기의 현재 레벨
    [SerializeField] protected WeaponData weapondata; // 무기의 현재 Stat
    protected ItemData itemdata; // 무기의 전체 정보
    protected WeaponName weaponname; // 무기 이름

    [Header("# Essential Data")]
    protected float combineDamage;
    protected float combineCoolTime;
    protected float combineAttackRange;
    protected float combineDuration;
    protected int combineProjectileCount;
    protected float combineProjectileSpeed;
    protected float combineProjectileSize;
    private float lastATKtime;

    [Header("# Referenced")]
    protected Player player;
    protected PoolManager poolManager;

    void OnEnable()
    {
        player = InGameManager.instance.player.GetComponent<Player>();
        poolManager = InGameManager.instance.PoolManager;
    }

    public void Init(ItemData data) // 외부에서 무기 획득시 호출해서 무기의 정보를 설정하는 함수
    {
        // 기본 정보 설정
        itemdata = data;
        weapondata = data.weaponData.Clone();
        player.weapon.Add(data.itemId);
        weaponname = (WeaponName)data.itemId;

        // 데미지 통계 정보 설정
        GameManager.instance.InGameData.accumWeaponDamageDict[weaponname] = new AccumWeaponData();
        GameManager.instance.InGameData.accumWeaponDamageDict[weaponname].SetData(data);

        Attack();
    }

    public void WeaponLevelUp(WeaponData weapon) // 무기 레벨업시 호출하는 함수
    {
        weapondata.Damage += weapon.Damage;
        weapondata.CoolTime -= weapon.CoolTime;
        weapondata.AttackRange += weapon.AttackRange;
        weapondata.Duration += weapon.Duration;
        weapondata.ProjectileCount += weapon.ProjectileCount;
        weapondata.ProjectileSpeed += weapon.ProjectileSpeed;
        weapondata.ProjectileSize += weapon.ProjectileSize;
        weapondata.Knockback += weapon.Knockback;

        if(itemdata.itemType == ItemData.ItemType.Weapon && level == 7)
        {
            player.maxlevelcount++;
        }

        Attack();
    }

    protected void MergeWeaponAndPlayerStats() // 무기 스탯과 플레이어 스탯 결합
    {
        combineDamage = weapondata.Damage * player.stat.AttackPower;
        combineCoolTime = weapondata.CoolTime * player.stat.CoolTime;
        combineAttackRange = weapondata.AttackRange * player.stat.AttackRange;
        combineDuration = weapondata.Duration * player.stat.Duration;
        combineProjectileCount = weapondata.ProjectileCount * player.stat.ProjectileCount;
        combineProjectileSpeed = weapondata.ProjectileSpeed * player.stat.ProjectileSpeed;
        combineProjectileSize = weapondata.ProjectileSize * player.stat.ProjectileSize;
    }

    protected virtual void Update() // 쿨타임마다 공격 실행
    {
        lastATKtime += Time.deltaTime;

        if(lastATKtime >= combineCoolTime)
        {
            Attack();
            lastATKtime = 0;
        }
    }

    protected abstract void Attack(); // 오버라이딩해서 사용할것
}
