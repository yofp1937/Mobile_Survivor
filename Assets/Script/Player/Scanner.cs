using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 가장 가까운적을 찾아주는 스크립트 
public class Scanner : MonoBehaviour
{
    float activescanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public Transform nearestTarget;

    void FixedUpdate()
    {
        activescanRange = InGameManager.instance.player.Status.AttackRange + InGameManager.instance.player.Status.AttackRange;
        targets = Physics2D.CircleCastAll(transform.position, activescanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
    }

    Transform GetNearest() // 가장 가까운 적을 찾아서 return해주는 함수
    {
        Transform result = null;
        float diff = 100;

        foreach(RaycastHit2D target in targets){
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if(curDiff < diff){
                diff = curDiff;
                result = target.transform;
            }
        }

        return result;
    }

    public List<Transform> GetTargets(int count)
    {
        Vector3 myPos = transform.position;
        // Targets에 myPos와 target.transform.position의 거리가 작은 순서대로 배열에 정렬시킴
        // 이후 .ToList로 리스트 형식으로 변환
        var Targets = targets.OrderBy(target => Vector3.Distance(myPos, target.transform.position)).ToList();

        // Take 함수는 해당 리스트(Targets)의 앞에서부터 count만큼의 요소를 가져옴
        // 이후 Select로 Take로 고른 요소들의 transform만 추출하고
        // .ToList로 리스트화 시킴
        return Targets.Take(count).Select(target => target.transform).ToList();
    }

    // Scene에서 플레이어의 공격범위 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white; // 흰색으로 설정
        Gizmos.DrawWireSphere(transform.position, activescanRange); // 얇은 실선으로 원 그리기
    }
}
