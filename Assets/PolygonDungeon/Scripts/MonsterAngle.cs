using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAngle : MonoBehaviour
{
    //원하는 위치에 원하는 각도의 원 범위에 닿는걸 반환해서 

    public float viewRange = 15f; //시야거리
    public float viewAngle = 120f; //시야각

    Transform monsterTr; //몬스터트랜스폼
    Transform playerTr; //플래이어트랜스폼

    int playerLayer; //플래이어레이어
    int obstacleLayer; //장애물레이어
    int layerMask; // 레이어 마스크

    private void Start()
    {
        monsterTr = GetComponent<Transform>();
        //태그로 플래이어 트랜스폼 찾으니까 태그 이름 지정 잘하기
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;

        //레이어이름도 지정
        playerLayer = LayerMask.NameToLayer("Player");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        //레이어마스크 = 비트
        //비트 이동 연산자를 사용해 레이어를 자기 레이어 번호만큼 밀어서 찾음
        layerMask = 1 << playerLayer | 1 << obstacleLayer;
    }

    /// <summary>
    /// 시야거리 시야각 안에 플래이어 탐지
    /// </summary>
    /// <returns></returns>
    public bool isTracePlayer()
    {
        bool isTrace = false;

        //OverlapSphere메서드로 설정해둔 몬스터 시야거리 구면 내부에 있는
        //콜라이더 중에서 플레이어 레이어를 가진 녀석을 배열로 반환
        Collider[] colls = Physics.OverlapSphere(monsterTr.position,
                                                 viewRange,
                                                 1 << playerLayer);
        //반환된 배열이 1개일때 = 플래이어가 원 안에 탐지됐을때
        if (colls.Length == 1)
        {
            //벡터 뺄샘으로 몬스터가 플래이어를 바라보는 벡터를 정규화로 방향값만 추출
            Vector3 lookPlayer = (playerTr.position - monsterTr.position).normalized;
            //몬스터가 플래이어를 바라보는 방향이 몬스터 정면부터 양쪽 각도가 60도 밑 = 120도 안에 있으면 추격
            if (Vector3.Angle(monsterTr.forward, lookPlayer) < viewAngle * 0.5)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }

    /// <summary>
    /// 플래이어 레이 캐치
    /// </summary>
    /// <returns></returns>
    public bool isViewPlayer()
    {
        bool isView = false;
        RaycastHit hit;

        //몬스터가 플래이어를 바라보는 방향
        Vector3 lookPlayer = (playerTr.position - monsterTr.position).normalized;
        //몬스터 위치에서 플래이어 방향으로 시야거리만큼 레이 발사. 맞은 레이어가 있으면
        if (Physics.Raycast(monsterTr.position, lookPlayer, out hit, viewRange, layerMask))
        {
            //검출된거 태그가 플래이어면 isView 참 반환
            isView = hit.collider.CompareTag("Player");
        }
        return isView;
    }

    //public Vector3 CirclePoint(float angle)
    //{
    //    angle += transform.eulerAngles.y;
    //    return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    //}
}
