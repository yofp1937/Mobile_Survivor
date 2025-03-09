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

    public void Setup(float damage, bool isCritical) // 표시 데미지 설정, 사라지는 시간 설정
    {
        damageText.text = damage.ToString();
        if(isCritical) // 크리티컬 발생시 빨간색
        {
            damageText.color = new Color(1f, 0f, 41f / 255f);
            damageText.fontSize = 0.8f;
        }
        else
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
