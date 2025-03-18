using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 각 정보를 화면에 표시
public class HUD : MonoBehaviour
{
    public HudType type;

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
            case HudType.Exp:
                UpdateExp();
                break;
            case HudType.Level:
                UpdateLevel();
                break;
            case HudType.Kill:
                UpdateKill();
                break;
            case HudType.Time:
                UpdateTime();
                break;
            case HudType.Health:
                UpdateHealth();
                break;
            case HudType.Gold:
                UpdateGold();
                break;
        }
    }

    void UpdateExp() // 경험치 표시
    {
        float curExp = InGameManager.instance.Player.Exp;
        float maxExp = InGameManager.instance.Player.NextExp[InGameManager.instance.Player.Level];
        mySlider.value = curExp / maxExp;
    }

    void UpdateLevel() // 플레이어 레벨 표시
    {
        myText.text = string.Format("Lv.{0:F0}", InGameManager.instance.Player.Level);
    }

    void UpdateKill() // 킬수 표시
    {
        myText.text = string.Format("{0:F0}", GameManager.instance.InGameDataManager.Kill);
    }

    void UpdateTime() // 남은 시간 표시
    {
        float remaintime = GameManager.instance.MaxGameTime - GameManager.instance.GameTime;
        if (remaintime < 0)
            {
                remaintime = 0;
            }
        int min = Mathf.FloorToInt(remaintime / 60);
        int sec = Mathf.FloorToInt(remaintime % 60);
        myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
    }

    void UpdateHealth() // 플레이어 체력 표시
    {
        float curHealth = InGameManager.instance.Player.Health;
        float maxHealth = InGameManager.instance.Player.Status.Hp;
        mySlider.value = curHealth / maxHealth;
    }

    void UpdateGold() // 획득 골드 표시
    {
        myText.text = string.Format("{0:F0}", GameManager.instance.InGameDataManager.GetGold);
    }
}
