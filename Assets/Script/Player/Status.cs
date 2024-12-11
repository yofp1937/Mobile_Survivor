using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public float Damage;
    public int DamageLev; // 레벨당 데미지 + 5%

    public float AttackSpeed;
    public int AttackSpeedLev; // 레벨당 투사체 속도 + 5%

    public float CoolTime;
    public int CoolTimeLev; // 레벨당 무기 쿨타임 5% 감소 

    public float Area;
    public int AreaLev; // 레벨당 무기 범위 7% 증가

    public float Duration;
    public int DurationLev; // 레벨당 무기 지속시간 10% 증가
    
    public int Amount;
    public int AmountLev; // 레벨당 투사체 개수 +1

    public float Magnet;
    public int MagnetLev; // 레벨당 아이템 획득 범위 25% 증가

    public List<int> PD_list;
    public float upgradeDamage = 1f;
    public float upgradeAttackSpeed = 1f;
    public float upgradeCoolTime = 1f;
    public float upgradeArea = 1f;
    public float upgradeDuration = 1f;
    public int upgradeAmount = 0;
    public float upgradeMagnet = 1f;

    void Awake()
    {
        DamageLev = 1;
        AttackSpeedLev = 1;
        CoolTimeLev = 1;
        AreaLev = 1;
        DurationLev = 1;
        AmountLev = 1;
        MagnetLev = 1;

        UpdateStatus();
        UpdateUpgrade();
    }

    void UpdateStatus()
    {
        Damage = Mathf.Pow(1.07f, DamageLev - 1) * upgradeDamage;
        AttackSpeed = Mathf.Pow(1.06f, AttackSpeedLev - 1) * upgradeAttackSpeed;
        CoolTime = Mathf.Pow(0.94f, CoolTimeLev - 1) * upgradeCoolTime;
        Area = Mathf.Pow(1.06f, AreaLev - 1) * upgradeArea;
        Duration = Mathf.Pow(1.06f, DurationLev - 1) * upgradeDuration;
        Amount = AmountLev - 1 + upgradeAmount;
        Magnet = Mathf.Pow(1.20f, MagnetLev - 1) * upgradeMagnet;
    }

    void UpdateUpgrade()
    {
        PD_list = GameManager.instance.PD_List;

        // 0.HP - 레벨당 10퍼
        float maxhealth = InGameManager.instance.player.maxHealth;
        InGameManager.instance.player.maxHealth = maxhealth * Mathf.Pow(1.1f, PD_list[0]);
        // 1.AttackDamage - 레벨당 7퍼
        upgradeDamage = Mathf.Pow(1.07f, PD_list[1]);
        // 2.AttackSpeed - 레벨당 4퍼
        upgradeAttackSpeed = Mathf.Pow(1.04f, PD_list[2]);
        // 3.Colldown - 레벨당 2퍼
        upgradeCoolTime = Mathf.Pow(0.98f, PD_list[3]);
        // 4.AttackRange - 레벨당 4퍼
        upgradeArea = Mathf.Pow(1.04f, PD_list[4]);
        // 5.Duration - 레벨당 2퍼
        upgradeDuration = Mathf.Pow(1.02f, PD_list[5]);
        // 6.Amount - 레벨당 1개
        upgradeAmount = PD_list[6];
        // 7.Magnet - 레벨당 10퍼
        upgradeMagnet = Mathf.Pow(1.1f, PD_list[7]);

        UpdateStatus();
    }

    public void LevelUp(ELevelUpStat stat)
    {
        switch(stat){
            case ELevelUpStat.Damage:
                DamageLev += 1;
                break;
            case ELevelUpStat.AttackSpeed:
                AttackSpeedLev += 1;
                break;
            case ELevelUpStat.CoolTime:
                CoolTimeLev += 1;
                break;
            case ELevelUpStat.Area:
                AreaLev += 1;
                break;
            case ELevelUpStat.Duration:
                DurationLev += 1;
                break;
            case ELevelUpStat.Amount:
                AmountLev += 1;
                break;
            case ELevelUpStat.Magnet:
                MagnetLev += 1;
                break;
            default:
                break;
        }
        UpdateStatus();
    }

    // Scene에서 플레이어의 획득범위 표시
    void OnDrawGizmos()
    {
        float _range = Magnet * transform.GetComponent<Player>().magnetRange;
        Gizmos.color = Color.red; // 빨간색으로 설정
        Gizmos.DrawWireSphere(transform.position, _range); // 얇은 실선으로 원 그리기
    }
}
public enum ELevelUpStat
{
    Damage = 0,
    AttackSpeed = 1,
    CoolTime = 2,
    Area = 3,
    Duration = 4,
    Amount = 5,
    Magnet = 6
}