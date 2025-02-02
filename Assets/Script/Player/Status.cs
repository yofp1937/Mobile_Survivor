using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public float Hp; // 체력(Hp만큼 증가(%도 가능))
    public float HpRegen; // 체력 재생(3초동안 HpRegen의 값을 리젠시킴)
    public float AttackPower; // 공격력(AttackPower만큼 증가(%도 가능))
    public float Defense; // 방어력(Defense%만큼 데미지를 덜입음)
    public float MoveSpeed; // 이동속도(MoveSpeed만큼 증가)
    public int ProjectileCount; // 투사체 개수(ProjectileCount만큼 증가)
    public float ProjectileSpeed; // 투사체 속도(ProjectileSpeed%만큼 빨라짐)
    public float CoolDown; // 쿨타임(CoolDown%만큼 줄어듬)
    public float Duration; // 지속시간(Duration%만큼 늘어남)
    public float AttackRange; // 공격 범위(AttackRange%만큼 늘어남)
    public float ObtainRange; // 아이템 획득 범위(ObtainRange%만큼 늘어남)
    public float CriticalChance; // 치명타 확률(기본 5%)
    public float CriticalDamage; // 치명타 데미지(기본 50%)
    public float Luck; // 아이템 획득 확률, 골드 추가 획득 확률, 아이템 등급 업그레이드 확률, 치명타 확률 등 게임 내 모든 확률 요소에 영향
                       // Luck 1당 치명타 확률 + 2.5퍼
    public float Curse; // 몬스터 스폰 속도(Curse%만큼 빨리 스폰, 최대 50퍼로 제한)

    public void AddStatus(Status param) // Status 합산 메서드
    {
        Hp += param.Hp;
        HpRegen += param.HpRegen;
        AttackPower += param.AttackPower;
        Defense += param.Defense;
        MoveSpeed += param.MoveSpeed;
        ProjectileCount += param.ProjectileCount;
        ProjectileSpeed += param.ProjectileSpeed;
        CoolDown += param.CoolDown;
        Duration += param.Duration;
        AttackRange += param.AttackRange;
        ObtainRange += param.ObtainRange;
        CriticalChance += param.CriticalChance;
        CriticalDamage += param.CriticalDamage;
        Luck += param.Luck;
        Curse += param.Curse;
    }
}