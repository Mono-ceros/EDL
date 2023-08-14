using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using System.ComponentModel;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;

/// <summary>
/// 몬스터가 상속할 fsm.
/// 부모인 life와 idamage, monsterdata
/// nav 소리 애니메이터 파라미터등이 알맞아야 동작
/// </summary>
public class Monster : Life
{
    //뭔가 문제 생기면 자식들 레이어가 몬스터가 아니라서 그럴수도

    //달리기랑 걷기
    //달리기를 하면 일정범위 감지해서 바로 추적
    public int playerLayer; // 추적 대상 레이어
    [SerializeField]
    Life target; // 추적 대상
    NavMeshAgent navMeshAgent; // 경로 계산 AI 에이전트

    //자식으로 넣은 파티클 참조
    public ParticleSystem hitEffect; // 피격 시 재생할 파티클 효과

    public AudioClip deathSound; // 사망 시 재생할 소리
    public AudioClip hitSound; // 피격 시 재생할 소리
    Animator monsterAnimater; // 애니메이터 컴포넌트
    AudioSource monsterAudioPlayer; // 오디오 소스 컴포넌트
    MonsterPatrol monsterPatrol;

    public float damage; // 공격력
    public float timeBetAttack = 0.5f; // 공격 간격
    private float lastAttackTime; // 마지막 공격 시점
    public float viewAngle = 120f; //시야각
    WaitForSeconds ws;

    [Header("공격 사거리")]
    public float attackDist = 1f;
    [Header("감지 거리")]
    public float traceDist = 1f;

    GameObject player;

    public IObjectPool<Monster> poolToReturn;

    readonly int hashMove = Animator.StringToHash("isMove");
    readonly int hashSpeed = Animator.StringToHash("Speed");
    readonly int hashDie = Animator.StringToHash("Die");
    readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    readonly int hashOffset = Animator.StringToHash("Offset");
    readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    readonly int hashAttack = Animator.StringToHash("Attack");
    readonly int hashHasTarget = Animator.StringToHash("HasTarget");

    /// <summary>
    /// 3초뒤에 오브젝트 풀로 몬스터 리턴
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyMonster()
    {
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(3f);
        poolToReturn.Release(this);
    }

    public enum State   //몬스터 상태
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    public State state = State.PATROL; //초기 상태는 순찰 상태로

    bool isLookPlayer
    {
        get
        {
            RaycastHit hit;

            //몬스터가 플래이어를 바라보는 방향
            Vector3 lookPlayer = (target.transform.position - transform.position).normalized;
            //몬스터 위치에서 플래이어 방향으로 시야거리만큼 레이 발사. 맞은 레이어가 있으면
            if (Physics.Raycast(transform.position, lookPlayer, out hit, attackDist, 1 << playerLayer))
            {
                //검출된거 태그가 플래이어면 isView 참 반환
                return true;
            }
            return false;
        }
        
        
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        monsterAnimater = GetComponentInChildren<Animator>();
        monsterAudioPlayer = GetComponent<AudioSource>();
        monsterPatrol = GetComponent<MonsterPatrol>();
        playerLayer = LayerMask.NameToLayer("Player");
        ws = new WaitForSeconds(0.2f);
        navMeshAgent.updateRotation = false;
    }

    // 몬스터 스탯을 몬스터 데이터 스크립터블 오브젝트에 넣은 값으로 셋업
    public void Setup(MonsterData monsterData)
    {
        startHp = monsterData.health;
        hp = monsterData.health;
        damage = monsterData.damage;
        navMeshAgent.speed = monsterData.speed;
        timeBetAttack = monsterData.timeBetAttack;
        attackDist = monsterData.attackDist;
        traceDist = monsterData.traceDist;
    }

    private new void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.GetComponent<Life>();

        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }

    bool hasTarget
    {
        get
        {
            if (target != null && !target.dead)
            {
                return true;
            }
            return false;
        }
    }

    bool isTrace
    {
        get
        {
            //벡터 뺄샘으로 몬스터가 플래이어를 바라보는 벡터를 정규화로 방향값만 추출
            Vector3 lookPlayer = (target.transform.position - transform.position).normalized;
            //몬스터가 플래이어를 바라보는 방향이 몬스터 정면부터 양쪽 각도가 원래 각도의 반 이하 = 원래 각도 안에 있으면 추격
            if (Vector3.Angle(transform.forward, lookPlayer) < viewAngle * 0.5)
            {
                return true;
            }
            return false;
        }
    }



    /// <summary>
    /// 주기적으로 타겟 검사, 없으면 타겟 지정
    /// 타겟이 존재하면 타겟이 시야 안에 들어와있는지 검사
    /// </summary>
    /// <returns></returns>
    //private IEnumerator UpdatePath()
    //{
    //    // 살아 있는 동안 무한 루프
    //    while (!dead)
    //    {
    //        if(!hasTarget)
    //        {
    //            //traceDist 만큼의 반지름을 가진 가상의 구를 그렸을 때 구와 겹치는 모든 콜라이더를 가져옴
    //            //단, targetLayer 레이어를 가진 콜라이더만 가져오도록 필터링
    //            Collider[] colliders =
    //                Physics.OverlapSphere(transform.position, traceDist, 1 << playerLayer);

    //            if(colliders.Length > 0)
    //            {
    //                //콜라이더로부터 LivingEntity 컴포넌트 가져오기
    //                Life livingEntity = colliders[0].GetComponent<Life>();

    //                //추적 대상을 해당 LivingEntity로 설정
    //                target = livingEntity;
    //            }
    //        }
    //        yield return ws;
    //    }
    //}

    // monster의 온데미지는 피격 파티클을 재생하고
    //base의 온데미지를 호출해 데미지 적용
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            //공격받은 지점과 방향으로 파티클 효과 재생
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            //피격 효과음 재생
            monsterAudioPlayer.PlayOneShot(hitSound);
        }
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    // 사망 처리
    public override void Die()
    {
        onDeath += () => StartCoroutine(DestroyMonster());

        // Life의 Die()를 실행
        base.Die();

        //다른 AI를 방해하지 않도록 자신의 모든 콜라이더를 비활성화
        Collider[] monsterColliders = GetComponents<Collider>();
        for (int i = 0; i < monsterColliders.Length; i++)
        {
            monsterColliders[i].enabled = false;
        }

        //AI 추적을 중지하고 내비메시 컴포넌트를 비활성화
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;

        //사망 애니메이션 재생
        monsterAnimater.SetTrigger("Die");
        //사망 효과음 재생
        monsterAudioPlayer.PlayOneShot(deathSound);
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    // 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행
    //    //자신이 사망하지 않았으며
    //    //최근 공격 시점에서 timeBetAttack 이상 시간이 지났다면 공격 가능
    //    if (!dead && Time.time >= lastAttackTime + timeBetAttack)
    //    {
    //        //상대방의 Life타입 가져오기 시도
    //        Life attackTarget =
    //            other.GetComponent<Life>();

    //        //상대방의 Life가 자신의 추적 대상이라면 공격 실행
    //        if (attackTarget != null && attackTarget == target)
    //        {
    //            //최근 공격 시간 갱신   
    //            lastAttackTime = Time.time;

    //            //상대방의 피격 위치와 피격 방향을 근삿값으로 계산
    //            Vector3 hitPoint =
    //                other.ClosestPoint(transform.position);
    //            Vector3 hitNormal =
    //                transform.position = other.transform.position;

    //            //공격 실행
    //            attackTarget.OnDamage(damage, hitPoint, hitNormal);
    //        }
    //    }
    //}

    IEnumerator CheckState()
    {

        //플레이어와 적 사이의 거리 계산
        while (!dead)
        {
            if (state == State.DIE)
                yield break;

            float dist = Vector3.Distance(transform.position,
                                           target.transform.position);
            if (isTrace && dist <= attackDist)
            {
                //원뿔 안에만 있으면 제자리보고 공격하는 이슈가 있음
                state = State.ATTACK;
            }
            else if (hasTarget && dist <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }
            yield return ws;    //코루틴 반환
        }
    }
    bool attackAnim
    {
        get
        {
            if(state == State.ATTACK)
            { return true; }
            return false;
        }
    }

    /// <summary>
    /// CheckState로 상태가 결정됐을때 상태에 따라 액션을 실행해줌
    /// </summary>
    /// <returns></returns>
    IEnumerator Action()
    {
        while (!dead)
        {
            yield return ws;

            //enum타입으로 설정해줬으니 switch로 간편하게 상태변환가능
            switch (state)
            {
                case State.PATROL:
                    monsterPatrol.Stop();
                    //enemyFire.isFire = false;
                    //monsterPatrol.PATROLLING = true;
                    //monsterAnimater.SetBool(hashMove, true);
                    monsterAnimater.SetBool(hashHasTarget, false);
                    monsterAnimater.SetBool(hashAttack, attackAnim);
                    break;
                case State.TRACE:
                    //enemyFire.isFire = false;
                    monsterPatrol.TRACETARGET = target.transform.position;
                    monsterAnimater.SetBool(hashHasTarget, true);
                    monsterAnimater.SetBool(hashAttack, attackAnim);
                    break;
                case State.ATTACK:
                    //transform.LookAt(target.transform.position);
                    monsterPatrol.Stop();
                    monsterAnimater.SetBool(hashAttack, attackAnim);
                    //if (!enemyFire.isFire)
                    //{
                    //    enemyFire.isFire = true;
                    //}
                    break;
                case State.DIE:
                    this.gameObject.tag = "Untagged";

                    dead = true;
                    //enemyFire.isFire = false;
                    monsterPatrol.Stop();
                    monsterAnimater.SetInteger(hashDieIdx, Random.Range(0, 3));
                    monsterAnimater.SetTrigger(hashDie);
                    StopAllCoroutines();

                    //죽고나서 콜라이더 비활성화
                    GetComponent<Collider>().enabled = false;

                    break;
            }
        }
    }
}
