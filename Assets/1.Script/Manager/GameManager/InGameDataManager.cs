using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameDataManager : MonoBehaviour
{
    [Header("# InGame Data")] // 게임 종료 후 표시되는 정보들
    public bool isClear = false;
    public int Kill; // 잡은 몬스터 수
    public int GetGold; // 획득한 골드량
    public int GetPotion; // 획득한 포션량
    public int GetMagnet; // 획득한 자석량
    public int AccumDamage; // 입은 데미지

    [Header("# Weapon, Acce, Equip Data")]
    public Dictionary<WeaponEnum, AccumWeaponData> AccumWeponData = new Dictionary<WeaponEnum, AccumWeaponData>();
    public Dictionary<AccesorriesEnum, AccumWeaponData> AccumAcceData = new Dictionary<AccesorriesEnum, AccumWeaponData>();
    public List<EquipmentData> GetEquip = new List<EquipmentData>();

    public void ResetData() // 데이터 초기화
    {
        Kill = 0;
        GetGold = 0;
        GetPotion = 0;
        GetMagnet = 0;
        AccumDamage = 0;
        AccumWeponData = new Dictionary<WeaponEnum, AccumWeaponData>();
        AccumAcceData = new Dictionary<AccesorriesEnum, AccumWeaponData>();
        GetEquip = new List<EquipmentData>();
    }
    
    public void SetAccumWeaponData() // 게임 종료할때 현재 무기레벨을 저장
    {
        isClear = true;
        Player player = InGameManager.instance.Player;
        for(int i = 0; i < player.WeaponList.Count; i++) // 무기 저장
        {
            WeaponBase weapon = InGameManager.instance.WeaponManager.transform.Find("Weapon" + player.WeaponList[i]).GetComponent<WeaponBase>();
            AccumWeponData[weapon.weaponname].Level = weapon.level;
        }

        for(int i = 0; i < player.AcceList.Count; i++) // 장신구 저장
        {
            WeaponBase acce = InGameManager.instance.WeaponManager.transform.Find("Acce" + player.AcceList[i]).GetComponent<WeaponBase>();
            AccumAcceData[acce.accename].Level = acce.level;
        }
    }
}
