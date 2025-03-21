using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSetting : MonoBehaviour
{
    public float damage;
    public int per;
    public float knockbackForce;
    public WeaponEnum weaponname;
    Rigidbody2D rigid;
    private Coroutine weaponCoroutine;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, float knockbackForce, Vector3 dir, WeaponEnum weaponname)
    {
        this.damage = damage;
        this.per = per;
        this.knockbackForce = knockbackForce;
        this.weaponname = weaponname;

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
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage, knockbackForce, transform.position, weaponname);
                Applyper();
            }
        }
    }

    public IEnumerator AttackWhileDuration(float duration)
    {
        yield return new WaitForSeconds(duration); // duration 값만큼 기다렸다가

        gameObject.SetActive(false); // 비활성화
    }

    // 코루틴을 실행하고 저장
    public void StartAttackWhileDuration(float cool)
    {
        if (weaponCoroutine != null)
        {
            StopCoroutine(weaponCoroutine); // 기존 코루틴이 있으면 중지
            weaponCoroutine = null;
        }
        weaponCoroutine = StartCoroutine(AttackWhileDuration(cool));
    }
}
