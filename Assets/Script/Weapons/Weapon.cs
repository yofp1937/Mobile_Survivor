using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;

    void Start()
    {
        Init();
    }

    void Update()
    {
        switch(id){
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                break;
        }
    }

    public void Init()
    {
        switch(id){
            case 0:
                speed = 150;
                Batch();
                break;
            default:
                break;
        }
    }

    void Batch()
    {
        for(int index=0; index < count; index++){
            Transform rotatesword = GameManager.instance.weapon.Get(prefabId).transform;
            rotatesword.parent = transform;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            rotatesword.Rotate(rotVec);
            rotatesword.Translate(rotatesword.up * 2f, Space.World);

            rotatesword.GetComponent<RotateSword>().Init(damage, -1); // 무한 관통이라 per는 -1로 설정
        }
        transform.position += new Vector3(0, 0.7f, 0);
    }
}
