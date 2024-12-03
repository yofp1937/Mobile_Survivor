using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 플레이어가 입힌 데미지를 화면에 띄우고 설정시간이 지나면 사라지게하는 스크립트
public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText; // 입은 데미지량 표시해주는 Text 객체
    float disappearTimer; // 텍스트가 사라지기까지의 시간
    Vector3 textposition = new Vector3(0, 1, 0); // 텍스트가 이동할 방향과 속도

    public void Setup(float damage) // 표시 데미지 설정, 사라지는 시간 설정
    {
        damageText.text = damage.ToString();
        if(damage > 90) // 데미지 90 이상이면 검정색
        {
            damageText.color = new Color(0f, 0f, 0f);
            damageText.fontSize = 0.8f;
        }
        else if(damage > 70) // 데미지 70 이상이면 빨간색
        {
            damageText.color = new Color(1f, 0f, 41f / 255f);
            damageText.fontSize = 0.7f;
        }
        else if(damage > 50) // 데미지 50 이상이면 금색
        {
            damageText.color = new Color(240f / 255f, 177f / 255f, 0f);
            damageText.fontSize = 0.6f;
        }
        else if(damage > 20) // 데미지 20 이상이면 초록색
        {
            damageText.color = new Color(0f, 177f / 255f, 230f / 255f);
            damageText.fontSize = 0.55f;
        }
        else // 데미지 20 미만이면 하얀색
        {
            damageText.color = Color.white;
            damageText.fontSize = 0.5f;
        }
        disappearTimer = 1f; // 시간 설정
    }

    void Update()
    {
        transform.position += textposition * Time.deltaTime;
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            gameObject.SetActive(false);
        }
    }
}
