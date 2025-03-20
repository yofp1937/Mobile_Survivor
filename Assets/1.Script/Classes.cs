using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public class WeaponStatusData // 무기의 기본 능력치
{
    public float Damage = 0; // 공격력(percent)
    public float CoolTime = 0; // 쿨타임(seconds)
    public float AttackRange = 0; // 공격 범위(value)
    public float Duration = 0; // 지속시간(seconds)
    public int ProjectileCount = 0; // 투사체 갯수
    public float ProjectileSpeed = 0; // 투사체 속도(value)
    public float ProjectileSize = 0; // 투사체 크기(value)
    public float Knockback = 0; // 넉백(value)

    public WeaponStatusData Clone()
    {
        return MemberwiseClone() as WeaponStatusData;
    }
}

[Serializable] public class Status // 악세서리 능력치 목록
{
    public float Hp; // 체력(percent)
    public float HpRegen; // 체력 재생(value) - 3초동안 HpRegen의 값을 리젠시킴
    public float AttackPower; // 공격력(percent)
    public float Defense; // 방어력(percent)
    public float MoveSpeed; // 이동속도(value)
    public int ProjectileCount; // 투사체 개수(value)
    public float ProjectileSpeed; // 투사체 속도(percent)
    public float ProjectileSize; // 투사체 크기(percent)
    public float CoolTime; // 쿨타임(percent)
    public float Duration; // 지속시간(percent)
    public float AttackRange; // 공격 범위(percent)
    public float ObtainRange; // 아이템 획득 범위(percent)
    public float CriticalChance; // 치명타 확률(기본 5%)
    public float CriticalDamage; // 치명타 데미지(기본 50%)
    public float Luck; // 아이템 획득 확률, 골드 추가 획득 확률, 아이템 등급 업그레이드 확률, 치명타 확률 등 게임 내 모든 확률 요소에 영향
                       // Luck 1당 치명타 확률 + 2.5퍼
    public float Curse; // 몬스터 스폰 속도(Curse%만큼 빨리 스폰, 최대 50퍼로 제한)

    public void CloneStatus(Status param)
    {
        Hp = param.Hp;
        HpRegen = param.HpRegen;
        AttackPower = param.AttackPower;
        Defense = param.Defense;
        MoveSpeed = param.MoveSpeed;
        ProjectileCount = param.ProjectileCount;
        ProjectileSpeed = param.ProjectileSpeed;
        ProjectileSize = param.ProjectileSize;
        CoolTime = param.CoolTime;
        Duration = param.Duration;
        AttackRange = param.AttackRange;
        ObtainRange = param.ObtainRange;
        CriticalChance = param.CriticalChance;
        CriticalDamage = param.CriticalDamage;
        Luck = param.Luck;
        Curse = param.Curse;
    }

    public void AddStatus(Status param) // Status 합산 메서드
    {
        Hp += param.Hp;
        HpRegen += param.HpRegen;
        AttackPower += param.AttackPower;
        Defense += param.Defense;
        MoveSpeed += param.MoveSpeed;
        ProjectileCount += param.ProjectileCount;
        ProjectileSpeed += param.ProjectileSpeed;
        ProjectileSize += param.ProjectileSize;
        CoolTime -= param.CoolTime;
        Duration += param.Duration;
        AttackRange += param.AttackRange;
        ObtainRange += param.ObtainRange;
        CriticalChance += param.CriticalChance;
        CriticalDamage += param.CriticalDamage;
        Luck += param.Luck;
        Curse -= param.Curse;
    }
}


public class AccumWeaponData // 통계창에서 표시할 무기별 데미지 데이터 저장 클래스
{
    public WeaponData Data;
    public int Level;
    public float TotalDamage;
}

[Serializable] public class SpawnData // Spawner 동작할때 필요한 데이터들
{
    public float spawnTime; // 리젠 시간
    public int health; // 몬스터 체력
    public float moveSpeed; // 몬스터 속도

    public SpawnData(float spawnTime, int health, float moveSpeed)
    {
        this.spawnTime = spawnTime;
        this.health = health;
        this.moveSpeed = moveSpeed;
    }
}