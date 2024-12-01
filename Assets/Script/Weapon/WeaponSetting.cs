using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSetting : MonoBehaviour
{
    public float damage;
    public int per;
    public float knockbackForce;
    Rigidbody2D rigid;
    private Coroutine weaponCoroutine;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, float knockbackForce, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;
        this.knockbackForce = knockbackForce;

        if(per > -1)
        {
            rigid.velocity = dir;
        }
    }

    void Applyper()
    {
        per--;
        if(per == 0) // 관통이 0이되면
        {
            if (rigid != null) // null 체크 추가
            {
                rigid.velocity = Vector2.zero;
            }

            if(weaponCoroutine != null)
            {
                StopCoroutine(weaponCoroutine);
                weaponCoroutine = null;
            }
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy")){
            Enemy enemy = collision.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage, knockbackForce, transform.position);
                Applyper();
            }
        }
    }

    public IEnumerator AttackWhileDuration(float duration)
    {
        gameObject.SetActive(true); // 활성화
        yield return new WaitForSeconds(duration); // duration 값만큼 기다렸다가

        gameObject.SetActive(false); // 비활성화
    }

    // 코루틴을 실행하고 저장
    public void StartThrowWhileDuration(float cool)
    {
        if (weaponCoroutine != null)
        {
            StopCoroutine(weaponCoroutine); // 기존 코루틴이 있으면 중지
        }
        weaponCoroutine = StartCoroutine(ThrowWhileDuration(cool));
    }

    // 무기를 일정 시간 후 비활성화하는 코루틴
    public IEnumerator ThrowWhileDuration(float cool)
    {
        yield return new WaitForSeconds(cool);
        gameObject.SetActive(false);
    }
}
