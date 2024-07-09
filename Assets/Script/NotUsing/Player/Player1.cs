using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    [Header("# Player Info")]
    public int health;
    public int maxHealth = 100;
    public float moveSpeed = 4f;
    public int level = 0;
    public int exp = 0;
    public List<int> nextExp = new List<int> { 3, };
    
    [Header("# Player Input")]
    public Vector2 inputVec;

    [Header("# Reference Object")]
    public GameObject weapon;
    public Scanner1 scanner;

    Animator ani;
    Rigidbody2D rigid;

    void NeedNextLevelExp(int exp)
    {
        int NeedNextLevelExp = exp * 2;
        nextExp.Add(NeedNextLevelExp);
    }

    void Awake()
    {
        health = maxHealth;

        rigid = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        scanner = GetComponent<Scanner1>();
        ani.SetBool("move_bool", false);
    }

    void Update()
    {
        inputVec.x = Input.GetAxis("Horizontal");
        inputVec.y = Input.GetAxis("Vertical");

        if(inputVec.x > 0){ // 오른쪽으로 이동할때 뒷모습 보여줌
            transform.rotation = Quaternion.Euler(0, 180, 0);
            weapon.transform.rotation = Quaternion.Euler(0, 0, weapon.transform.eulerAngles.z);
        }
        else { // 정지상태일때나 왼쪽으로 이동할때 앞을 바라봄
            transform.rotation = Quaternion.Euler(0, 0, 0);
            weapon.transform.rotation = Quaternion.Euler(0, 0, weapon.transform.eulerAngles.z);
        }

        // Vector의 크기가 0보다 크면(움직이는 상태면) move_bool 값을 true로 변경(walk 애니메이션 동작)
        if (inputVec.magnitude > 0)
        {
            ani.SetBool("move_bool", true);
        }
        else
        {
            ani.SetBool("move_bool", false);
        }
    }


    public void GetExp()
    {
        exp++;

        if(exp == nextExp[level]){
            LevelUp();
        }
    }

    public void LevelUp()
    {
        NeedNextLevelExp(exp);
        level++;
        GameManager1.instance.LevelUpPanel.SetActive(true);
        Time.timeScale = 0;
        

        exp = 0;
        //GameManager.instance.LevelUpPanel.SetActive(false);
        //Time.timeScale = 1;
    }
}
