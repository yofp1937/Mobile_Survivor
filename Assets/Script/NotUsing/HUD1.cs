using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 게임 UI에 표시되는(경험치, 게임시간 등) 값들을 관리하는 스크립트
public class HUD1 : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health }
    public InfoType type;

    Text myText;
    Slider mySlider;

    void Awake()
    {
        myText = GetComponent<Text>();
        mySlider = GetComponent<Slider>();
    }
    
    void LateUpdate()
    {
        switch(type){
            case InfoType.Exp:
                float curExp = GameManager1.instance.player.exp;
                float maxExp = GameManager1.instance.player.nextExp[GameManager1.instance.player.level];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager1.instance.player.level);
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager1.instance.kill);
                break;
            case InfoType.Time:
                float remainTime = GameManager1.instance.maxGameTime - GameManager1.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health:
                float curHealth = GameManager1.instance.player.health;
                float maxHealth = GameManager1.instance.player.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
        }
    }
}
