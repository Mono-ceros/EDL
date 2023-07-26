using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using System.ComponentModel;

/// <summary>
/// 몬스터가 상속할 fsm.
/// 부모인 life와 idamage, monsterdata
/// nav 소리 애니메이터 파라미터등이 알맞아야 동작
/// </summary>
public class Monster : Life
{
    public LayerMask targetLayer; // 추적 대상 레이어
    private Life target; // 추적 대상
    NavMeshAgent navMeshAgent; // 경로 계산 AI 에이전트

    //자식으로 넣은 파티클 참조
    public ParticleSystem hitEffect; // 피격 시 재생할 파티클 효과

    public AudioClip deathSound; // 사망 시 재생할 소리
    public AudioClip hitSound; // 피격 시 재생할 소리
    private Animator monsterAnimater; // 애니메이터 컴포넌트
    private AudioSource monsterAudioPlayer; // 오디오 소스 컴포넌트

    public event Action onAttack; // 공격 이벤트, 몬스터 상속한 놈들이 자기 공격 넣어서 쓸것

    public float damage; // 공격력
    public float timeBetAttack = 0.5f; // 공격 간격
    private float lastAttackTime; // 마지막 공격 시점

    [Header("공격 사거리")]
    public float attackDist = 5f;
    [Header("감지 거리")]
    public float traceDist = 10f;

    public IObjectPool<Monster> poolToReturn;

    readonly int hashMove = Animator.StringToHash("isMove");
    readonly int hashSpeed = Animator.StringToHash("Speed");
    readonly int hashDie = Animator.StringToHash("Die");
    readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    readonly int hashOffset = Animator.StringToHash("Offset");
    readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
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

    /// <summary>
    /// 추적할 대상이 존재하는지 알려주는 프로퍼티
    /// </summary>
    private bool hasTarget
    {
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (target != null && !target.dead) return true;
            // 그렇지 않다면 false
            return false;
        }
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        monsterAnimater = GetComponent<Animator>();
        monsterAudioPlayer = GetComponent<AudioSource>();
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
    }

    private new 
        void OnEnable()
    {
        StartCoroutine(UpdatePath());
        StartCoroutine(Targetanim());
    }

    /// <summary>
    /// 타켓 발견하면 추격 애니메이션
    /// </summary>
    /// <returns></returns>
    IEnumerator Targetanim()
    {
        while (!base.dead)
        {
            monsterAnimater.SetBool(hashHasTarget, hasTarget);
            yield return new WaitForSeconds(0.3f);
        }
    }


    /// <summary>
    /// 주기적으로 타겟 검사, 없으면 타겟 지정
    /// 추적할 대상의 위치를 찾아 경로 갱신
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdatePath()
    {
        // 살아 있는 동안 무한 루프
        while (!dead)
        {
            if (hasTarget)
            {
                //추적 대상 존재 : 경로를 갱신하고 AI 이동을 계속 진행
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(
                    target.transform.position);
            }
            else
            {
                //추적 대상 없음 : AI 이동 중지
                navMeshAgent.isStopped = true;

                //traceDist 만큼의 반지름을 가진 가상의 구를 그렸을 때 구와 겹치는 모든 콜라이더를 가져옴
                //단, targetLayer 레이어를 가진 콜라이더만 가져오도록 필터링
                Collider[] colliders =
                    Physics.OverlapSphere(transform.position, traceDist, targetLayer);

                //모든 콜라이더를 순회하면서 살아 있는 LivingEntity 찾기
                for (int i = 0; i < colliders.Length; i++)
                {
                    //콜라이더로부터 LivingEntity 컴포넌트 가져오기
                    Life livingEntity = colliders[i].GetComponent<Life>();

                    //LivingEntity 컴포넌트가 존재하며, 해당 LivingEntity가 살아 있으면
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        //추적 대상을 해당 LivingEntity로 설정
                        target = livingEntity;

                        //for 문 루프 즉시 정지
                        break;
                    }
                }
            }

            // 0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }

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

    private void OnTriggerStay(Collider other)
    {
        // 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행
        //자신이 사망하지 않았으며
        //최근 공격 시점에서 timeBetAttack 이상 시간이 지났다면 공격 가능
        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            //상대방의 Life타입 가져오기 시도
            Life attackTarget =
                other.GetComponent<Life>();

            //상대방의 Life가 자신의 추적 대상이라면 공격 실행
            if (attackTarget != null && attackTarget == target)
            {
                //최근 공격 시간 갱신   
                lastAttackTime = Time.time;

                //상대방의 피격 위치와 피격 방향을 근삿값으로 계산
                Vector3 hitPoint =
                    other.ClosestPoint(transform.position);
                Vector3 hitNormal =
                    transform.position = other.transform.position;

                //공격 실행
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }

    /// <summary>
    /// onAttack 이벤트 함수가 존재하면 실행
    /// </summary>
    public virtual void Attack()
    {
        if (onAttack != null)
        {
            onAttack();
        }
    }

    //IEnumerator CheckState()
    //{
    //    //다른 스크립트 초기화를 위한 대기시간
    //    yield return new WaitForSeconds(1);

    //    //플레이어와 적 사이의 거리 계산
    //    while (!isDie)
    //    {
    //        if (state == State.DIE)
    //            yield break;

    //        float dist = Vector3.Distance(playerTr.position,
    //                                       enemyTr.position);
    //        if (dist <= attackDist)
    //        {
    //            if (enemyFOV.isViewPlayer())
    //                state = State.ATTACK;
    //            else
    //                state = State.TRACE;
    //        }
    //        else if (enemyFOV.isTracePlayer())
    //        {
    //            state = State.TRACE;
    //        }
    //        else if (dist <= traceDist)
    //        {
    //            state = State.TRACE;
    //        }
    //        else
    //        {
    //            state = State.PATROL;
    //        }
    //        yield return ws;    //코루틴 반환
    //    }
    //}

    //IEnumerator Action()
    //{
    //    while (!isDie)
    //    {
    //        yield return ws;

    //        switch (state)
    //        {
    //            case State.PATROL:
    //                enemyFire.isFire = false;
    //                moveAgent.PATROLLING = true;
    //                animator.SetBool(hashMove, true);
    //                break;
    //            case State.TRACE:
    //                enemyFire.isFire = false;
    //                moveAgent.TRACETARGET = playerTr.position;
    //                animator.SetBool(hashMove, true);
    //                break;
    //            case State.ATTACK:
    //                moveAgent.Stop();
    //                animator.SetBool(hashMove, false);
    //                if (!enemyFire.isFire)
    //                {
    //                    enemyFire.isFire = true;
    //                }
    //                break;
    //            case State.DIE:
    //                this.gameObject.tag = "Untagged";

    //                isDie = true;
    //                enemyFire.isFire = false;
    //                moveAgent.Stop();
    //                animator.SetInteger(hashDieIdx, Random.Range(0, 3));
    //                animator.SetTrigger(hashDie);

    //                //죽고나서 콜라이더 비활성화
    //                GetComponent<CapsuleCollider>().enabled = false;

    //                break;
    //        }
    //    }
    //}
}
