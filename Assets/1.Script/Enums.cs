/*
    각종 enum(열거형 데이터) 초기화
*/
public enum Scene { Lobby, InGame }
public enum Bgm { Lobby, InGame } // Bgm 종류
public enum Sfx { Click, Heal, ThrowWeapon, Explosion, Laser, Thunder, Wave, Saprk } // 효과음 종류
public enum DifficultyLevels{ Normal, Hard, Hell, God, Nightmare} // 난이도 종류
public enum UpgradeEnum {Hp, AttackPower, ProjectileCount, ProjectileSpeed, CoolTime, Duration, AttackRange, ObtainRange} // 강화 능력치 종류
public enum WeaponEnum{ RotateSword, ThrowWeapon, Laser, Fireball, Thunder, Spark, Wave} // 무기 종류
public enum AccesorriesEnum { Ruby, Feather, Book, Candle, Gear, Belt, Coin } // 장신구 종류
public enum DropItemEnum { ExpJewel_1, ExpJewel_3, ExpJewel_5, Gold, Magnet, Potion, Equipment} // 몬스터 드랍 아이템 종류

public enum PoolEnum // ObjectPoolling을 활용하는 객체 리스트
{
    None = -1,
    // Weapons
    RotateSword, ThrowWeapon, Laser, Fireball, Thunder, Spark, Wave,
    // Enemies
    Boss, FlyEye, Goblin, Mushroom, Skeleton,
    // Items
    ExpJewel_1, ExpJewel_3, ExpJewel_5, Gold, Magnet, Potion,
    // Equipment
    Equipment,
    // DmgPopUp
    DamagePopUp
}
public enum HudType // InGameScene에서 화면에 표시할 데이터들의 이름
{
    Exp, // 경험치
    Level, // 플레이어 레벨
    Kill, // 킬수
    Time, // 남은 시간
    Health, // 플레이어 체력
    Gold // 획득 골드
}

public enum EquipGrade // 아이템 등급
{
    Common, // 흔한
    UnCommon, // 드문
    Rare, // 특별한
    Unique, // 희귀한
    Legendary // 전설적인
}

public enum EquipPart // 아이템 부위
{
    Hat, Armor, Ring, Necklace
}

public enum StatusEnum // 열거형 스탯
{
    Hp, // 체력(percent)
    HpRegen, // 체력 재생(value) - 3초동안 HpRegen의 값을 리젠시킴
    AttackPower, // 공격력(percent)
    Defense, // 방어력(percent)
    MoveSpeed, // 이동속도(value)
    ProjectileCount, // 투사체 개수(value)
    ProjectileSpeed, // 투사체 속도(percent)
    ProjectileSize, // 투사체 크기(percent)
    CoolTime, // 쿨타임(percent)
    Duration, // 지속시간(percent)
    AttackRange, // 공격 범위(percent)
    ObtainRange, // 아이템 획득 범위(percent)
    CriticalChance, // 치명타 확률(기본 5%)
    CriticalDamage, // 치명타 데미지(기본 50%)
    Luck, // 아이템 획득 확률, 골드 추가 획득 확률, 아이템 등급 업그레이드 확률, 치명타 확률 등 게임 내 모든 확률 요소에 영향
                       // Luck 1당 치명타 확률 + 2.5퍼
    Curse // 몬스터 스폰 속도(Curse%만큼 빨리 스폰, 최대 50퍼로 제한)
}