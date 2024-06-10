using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSetting : MonoBehaviour
{
    public float damage;
    public int per;
    public float knockbackForce;
    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, float knockbackForce, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;
        this.knockbackForce = knockbackForce;
        
        if(per > -1){ // 관통이 무한이 아니면 원거리 무기
            rigid.velocity = dir;
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy")){
            Enemy enemy = collision.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage, knockbackForce, transform.position);
            }
            
            if(per > -1){ // 관통이 무한이 아니면 수치를 1 줄이고
                per--;
                if(per == -1){ // 관통 수치가 끝나면 무기 사라짐
                    rigid.velocity = Vector2.zero;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public IEnumerator AttackWhileDuration(float duration)
    {
        gameObject.SetActive(true); // 활성화
        yield return new WaitForSeconds(duration); // duration 값만큼 기다렸다가
        //TODO 점차 사리지게끔
        gameObject.SetActive(false); // 비활성화
    }
}
