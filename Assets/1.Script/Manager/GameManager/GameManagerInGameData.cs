using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerInGameData : MonoBehaviour
{
    [Header("# InGame Data")] // 게임 종료 후 표시되는 정보들
    public bool boolScore = false;
    public int kill; // 잡은 몬스터 수
    public int getGold; // 획득한 골드량
    public int getPotion; // 획득한 포션량
    public int getMagnet; // 획득한 자석량
    public int accumDamage; // 입은 데미지

    [Header("# Accum Weapon Damage Data")]
    public float accumWeaponDamage;
    public Dictionary<WeaponEnum, AccumWeaponData> accumWeaponDamageDict = new Dictionary<WeaponEnum, AccumWeaponData>();

    public void DataReset() // 데이터 초기화
    {
        kill = 0;
        getGold = 0;
        getPotion = 0;
        getMagnet = 0;
        accumDamage = 0;
        accumWeaponDamage = 0f;
        accumWeaponDamageDict = new Dictionary<WeaponEnum, AccumWeaponData>();
    }
}
