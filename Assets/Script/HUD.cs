using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 각 정보를 화면에 표시
public class HUD : MonoBehaviour
{
    public enum InfoType { Exp, Level, Kill, Time, Health, Gold }
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
                float curExp = InGameManager.instance.player.exp;
                float maxExp = InGameManager.instance.player.nextExp[InGameManager.instance.player.level];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", InGameManager.instance.player.level);
                break;
            case InfoType.Kill:
                myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Time:
                float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(remainTime / 60);
                int sec = Mathf.FloorToInt(remainTime % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health:
                float curHealth = InGameManager.instance.player.health;
                float maxHealth = InGameManager.instance.player.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
            case InfoType.Gold:
                myText.text = string.Format("{0:F0}", GameManager.instance.getGold);
                break;
        }
    }
}
