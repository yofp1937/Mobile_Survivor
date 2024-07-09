using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status1 : MonoBehaviour
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
    public int DurationLev; // 레벨당 무기 지속시간 15% 증가
    
    public int Amount;
    public int AmountLev; // 레벨당 투사체 개수 +1

    public float Magnet;
    public int MagnetLev; // 레벨당 아이템 획득 범위 25% 증가

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
    }

    void UpdateStatus()
    {
        Damage = Mathf.Pow(1.05f, DamageLev - 1);
        AttackSpeed = Mathf.Pow(1.05f, AttackSpeedLev - 1);
        CoolTime = Mathf.Pow(0.95f, CoolTimeLev - 1);
        Area = Mathf.Pow(1.07f, AreaLev - 1);
        Duration = Mathf.Pow(1.15f, DurationLev - 1);
        Amount = AmountLev - 1;
        Magnet = Mathf.Pow(1.25f, MagnetLev - 1);
    }

    public void LevelUp(ELevelUpStat1 stat)
    {
        switch(stat){
            case ELevelUpStat1.Damage:
                DamageLev += 1;
                break;
            case ELevelUpStat1.AttackSpeed:
                AttackSpeedLev += 1;
                break;
            case ELevelUpStat1.CoolTime:
                CoolTimeLev += 1;
                break;
            case ELevelUpStat1.Area:
                AreaLev += 1;
                break;
            case ELevelUpStat1.Duration:
                DurationLev += 1;
                break;
            case ELevelUpStat1.Amount:
                AmountLev += 1;
                break;
            case ELevelUpStat1.Magnet:
                MagnetLev += 1;
                break;
            default:
                break;
        }
        UpdateStatus();
    }
}
public enum ELevelUpStat1
{
    Damage = 0,
    AttackSpeed = 1,
    CoolTime = 2,
    Area = 3,
    Duration = 4,
    Amount = 5,
    Magnet = 6
}