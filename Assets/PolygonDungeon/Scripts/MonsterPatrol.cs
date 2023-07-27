using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterPatrol : MonoBehaviour
{
    public List<Transform> wayPoints;   //순찰 지점 저장용 List
    public int nextIdx; //다음 순찰지점 지정 변수

    NavMeshAgent agent;

    readonly float patrolSpeed = 1.5f;
    readonly float traceSpeed = 4.0f;

    float damping = 1f; //회전속도 조절 계수
    Transform enemyTr;

    bool patrolling;
    public bool PATROLLING
    {
        get { return patrolling; }
        set
        {
            patrolling = value;
            if (patrolling)
            {
                agent.speed = patrolSpeed;
                damping = 1f;
                MoveWayPoint();
            }
        }
    }

    Vector3 traceTarget;
    public Vector3 TRACETARGET
    {
        get { return traceTarget; }
        set
        {
            traceTarget = value;
            agent.speed = traceSpeed;
            damping = 7f;
            //추적 대상 지정 함수 호출
            TraceTarget(traceTarget);
        }
    }

    public float SPEED
    {
        get { return agent.velocity.magnitude; }
    }

    void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale)
            return;

        agent.destination = pos;
        agent.isStopped = false;
    }

    public void Stop()
    {
        agent.isStopped = true;
        //제자리에 멈추기 위하여 속도 0으로
        agent.velocity = Vector3.zero;
        patrolling = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyTr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        //목적지에 도착함에 따라 감속하는 옵션 비활성화
        agent.autoBraking = false;
        agent.updateRotation = false;

        //Find는 이름으로
        //하이어라키에 있는 오브젝트를 탐색하는 메서드
        //하나하나 확인해서 성능저하를 일으킴 남발 ㄴㄴ
        var group = GameObject.Find("WayPointGroup");
        if (group != null)
        {
            //자식 오브젝트들을 가지고 와서 waypoints 리스트에 할당
            group.GetComponentsInChildren<Transform>(wayPoints);
            //리스트의 제일 처음 항목을 제거
            //부모오브젝트까지 들고와버려서 0번 삭제
            wayPoints.RemoveAt(0);

            //순찰 포인트 랜덤
            nextIdx = Random.Range(0, wayPoints.Count);
        }

        //순찰지점으로 움직이는 메소드 호출
        //MoveWayPoint();
        this.patrolling = true;
    }



    void MoveWayPoint()
    {
        //경로 계산중이면 참 끝나면 거짓
        if (agent.isPathStale)
            return;
        //다음 목적지를 리스트에서 추출해서 설정
        agent.destination = wayPoints[nextIdx].position;
        //네비게이션 기능 활성화 = 움직임
        agent.isStopped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.isStopped)    //캐릭이 이동중이면
        {
            //NavMeshAgent가 진행해야될 방향 벡터를 회전값으로 변환
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation,
                                                rot,
                                                Time.deltaTime * damping);
        }


        //순찰 상태가 아닐 경우에는 아래 코드 실행안함
        if (!patrolling)
            return;

        //이동속도가 0.2 * 0.2 이상이면서 남은거리가 0.5이하
        //sqrMagnitude는 성능때문에 제곱근연산을 함
        if (agent.velocity.sqrMagnitude > -0.2f * 0.2f &&
            agent.remainingDistance <= 0.5f)
        {
            //nextIdx++;
            //순찰지점의 로테이션을 위해서 나머지 연산
            // 5로 나누면 0,1,2,3,4,5=0,6=1, 이런식으로 순환
            //nextIdx = nextIdx % wayPoints.Count;
            nextIdx = Random.Range(0, wayPoints.Count);

            MoveWayPoint();
        }
    }
}
