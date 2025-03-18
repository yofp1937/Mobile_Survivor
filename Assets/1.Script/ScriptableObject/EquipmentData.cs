using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
    1.장비는 등급과 부위가 정해져있음

    2.등급에따라 강화 횟수와 옵션 갯수가 달라짐
    2-1.등급에따라 옵션의 스테이터스 증가량이 달라짐
        ex) Common: HP+5, UnCommon: HP+10, Rare: HP+25, Unique: HP+50, Legendary: HP+100
    2-2.등급에따라 생성될때의 출현 옵션 갯수가 다름
        Common: 기본 1개, 50%로 2개
        UnCommon: 기본 2개, 33%로 3개
        Rare: 기본 2개, 50%로 3개
        Unique: 기본 3개, 33%로 4개
        Legendary: 기본 4개, 25%로 5개

    3.부위에따라 옵션에 출현하는 능력치가 다름

    * _maxLevel과 _maxObtionCount 값
    Common, UnCommon = 3
    Rare, Unique = 4
    Legendary = 5
*/
[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Object/EquipmentData", order = 1)]
public class EquipmentData : ScriptableObject // Name, Grade, Part, Sprite만 넣고 CreateEquip 실행시 나머지 옵션은 자동 생성
{
    public string Name; // 장비 이름
    public EquipGrade Grade; // 장비 등급
    public EquipPart Part; // 장비 부위
    public Sprite Sprite; // 장비의 이미지
}