using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText; // 입은 데미지량 표시
    private float disappearTimer = 1f; // 텍스트가 사라지기까지의 시간
    private Vector3 textposition = new Vector3(0, 1, 0); // 텍스트가 이동할 방향과 속도
    Transform popupT;

    public void Setup(float damage)
    {
        damageText.text = damage.ToString();
    }

    void Update()
    {
        transform.position += textposition * Time.deltaTime;
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            Destroy(gameObject);
        }
    }
}
