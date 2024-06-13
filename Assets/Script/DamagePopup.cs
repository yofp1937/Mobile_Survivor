using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText; // 입은 데미지량 표시해주는 Text 객체
    float disappearTimer; // 텍스트가 사라지기까지의 시간
    Vector3 textposition = new Vector3(0, 1, 0); // 텍스트가 이동할 방향과 속도

    public void Setup(float damage) // 표시 데미지 설정, 사라지는 시간 설정
    {
        damageText.text = damage.ToString();
        disappearTimer = 1f;
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
