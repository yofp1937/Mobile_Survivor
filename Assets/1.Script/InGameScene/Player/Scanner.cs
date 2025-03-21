using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 가장 가까운적을 찾아주는 스크립트 
public class Scanner : MonoBehaviour
{
    public float activescanRange; // 스캔 범위
    public LayerMask targetLayer;
    public Collider2D[] targets;
    public Transform nearestTarget;

    // Scene에서 플레이어의 EnemyScan 범위 초록색으로 표시
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activescanRange);
    }

    public Transform GetNearestTarget() // activescanRange 내부의 가장 가까운 Enemy 레이어를 찾아서 return해주는 함수
    {
        Vector3 myPos = transform.position;
        targets = Physics2D.OverlapCircleAll(myPos, activescanRange, targetLayer);

        Transform result = null;
        float diff = 100;

        foreach(Collider2D target in targets){
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if(curDiff < diff){
                diff = curDiff;
                result = target.transform;
            }
        }

        return result;
    }

    public List<Transform> GetTragetsInAttackRange(float attackrange) // attackrange로 전달받은 범위 내부의 Enemy 레이어를 골라서 return
    {
        Vector3 myPos = transform.position;
        targets = Physics2D.OverlapCircleAll(myPos, attackrange, targetLayer);

        // Targets에 myPos와 target.transform.position의 거리가 작은 순서대로 배열에 정렬시킴
        // 이후 .ToList로 리스트 형식으로 변환
        var Targets = targets.Where(target => Vector3.Distance(myPos, target.transform.position) <= attackrange)
                             .OrderBy(target => Vector3.Distance(myPos, target.transform.position))
                             .ToList();

        // Select로 Take로 고른 요소들의 transform만 추출하고
        // .ToList로 리스트화 시킴
        return Targets.Select(target => target.transform).ToList();
    }

    public List<Transform> GetTargetsInScanRange(int count) // Scanner에 설정된 activescanRange 범위 내부의 Enemy 레이어를 골라서 return
    {
        Vector3 myPos = transform.position;
        targets = Physics2D.OverlapCircleAll(myPos, activescanRange, targetLayer);

        // Targets에 myPos와 target.transform.position의 거리가 작은 순서대로 배열에 정렬시킴
        // 이후 .ToList로 리스트 형식으로 변환
        var Targets = targets.Where(target => Vector3.Distance(myPos, target.transform.position) <= activescanRange)
                             .OrderBy(target => Vector3.Distance(myPos, target.transform.position))
                             .ToList();

        // Take 함수는 해당 리스트(Targets)의 앞에서부터 count만큼의 요소를 가져옴
        // 이후 Select로 Take로 고른 요소들의 transform만 추출하고
        // .ToList로 리스트화 시킴
        return Targets.Take(count).Select(target => target.transform).ToList();
    }

    public List<Transform> GetAllTargetsInAttackRange(float attackrange)
    {
        Vector3 myPos = transform.position;
        targets = Physics2D.OverlapCircleAll(myPos, attackrange, targetLayer);

        return targets.Select(collider => collider.transform) // Transform만 추출
                        .OrderBy(transform => Vector3.Distance(myPos, transform.position)) // 거리순 정렬
                        .ToList();
    }

    public List<Transform> GetAllTargets() // Enemy 스크립트를 가진 모든 오브젝트 반환
    {
        Vector3 myPos = transform.position;

        return GameObject.FindObjectsOfType<EnemyBase>()
                         .Select(target => target.transform)
                         .OrderBy(target => Vector3.Distance(myPos, target.transform.position))
                         .ToList();
    }
}
