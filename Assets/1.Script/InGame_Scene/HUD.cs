using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 각 정보를 화면에 표시
public class HUD : MonoBehaviour
{
    public enum InfoType // 화면에서 표시할 데이터들의 이름
    {
        Exp, // 경험치
        Level, // 플레이어 레벨
        Kill, // 킬수
        Time, // 남은 시간
        Health, // 플레이어 체력
        Gold // 획득 골드
    }
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
                UpdateExp();
                break;
            case InfoType.Level:
                UpdateLevel();
                break;
            case InfoType.Kill:
                UpdateKill();
                break;
            case InfoType.Time:
                UpdateTime();
                break;
            case InfoType.Health:
                UpdateHealth();
                break;
            case InfoType.Gold:
                UpdateGold();
                break;
        }
    }

    void UpdateExp() // 경험치 표시
    {
        float curExp = InGameManager.instance.player.Exp;
        float maxExp = InGameManager.instance.player.NextExp[InGameManager.instance.player.Level];
        mySlider.value = curExp / maxExp;
    }

    void UpdateLevel() // 플레이어 레벨 표시
    {
        myText.text = string.Format("Lv.{0:F0}", InGameManager.instance.player.Level);
    }

    void UpdateKill() // 킬수 표시
    {
        myText.text = string.Format("{0:F0}", GameManager.instance.InGameData.kill);
    }

    void UpdateTime() // 남은 시간 표시
    {
        float remaintime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
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
        float curHealth = InGameManager.instance.player.Health;
        float maxHealth = InGameManager.instance.player.Status.Hp;
        mySlider.value = curHealth / maxHealth;
    }

    void UpdateGold() // 획득 골드 표시
    {
        myText.text = string.Format("{0:F0}", GameManager.instance.InGameData.getGold);
    }
}
