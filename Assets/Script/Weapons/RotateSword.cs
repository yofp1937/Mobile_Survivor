using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSword : MonoBehaviour
{
    public float damage;
    public int per;

    public void Init(float damage, int per)
    {
        this.damage = damage;
        this.per = per;
    }

    public IEnumerator AttackWhileDuration(float duration)
    {
        gameObject.SetActive(true); // 활성화
        yield return new WaitForSeconds(duration); // duration 값만큼 기다렸다가
        //TODO 점차 사리지게끔
        gameObject.SetActive(false); // 비활성화
    }
}
