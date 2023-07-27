using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using System.ComponentModel;
using Random = UnityEngine.Random;

/// <summary>
/// ���Ͱ� ����� fsm.
/// �θ��� life�� idamage, monsterdata
/// nav �Ҹ� �ִϸ����� �Ķ���͵��� �˸¾ƾ� ����
/// </summary>
public class Monster : Life
{
    //���� ���� ����� �ڽĵ� ���̾ ���Ͱ� �ƴ϶� �׷�����

    //�޸���� �ȱ�
    //�޸��⸦ �ϸ� �������� �����ؼ� �ٷ� ����
    public LayerMask targetLayer; // ���� ��� ���̾�
    private Life target; // ���� ���
    NavMeshAgent navMeshAgent; // ��� ��� AI ������Ʈ

    //�ڽ����� ���� ��ƼŬ ����
    public ParticleSystem hitEffect; // �ǰ� �� ����� ��ƼŬ ȿ��

    public AudioClip deathSound; // ��� �� ����� �Ҹ�
    public AudioClip hitSound; // �ǰ� �� ����� �Ҹ�
    private Animator monsterAnimater; // �ִϸ����� ������Ʈ
    private AudioSource monsterAudioPlayer; // ����� �ҽ� ������Ʈ
    MonsterAngle monsterAngle;
    MonsterPatrol monsterPatrol;

    public float damage; // ���ݷ�
    public float timeBetAttack = 0.5f; // ���� ����
    private float lastAttackTime; // ������ ���� ����

    WaitForSeconds ws;

    [Header("���� ��Ÿ�")]
    public float attackDist = 5f;
    [Header("���� �Ÿ�")]
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
    /// 3�ʵڿ� ������Ʈ Ǯ�� ���� ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator DestroyMonster()
    {
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(3f);
        poolToReturn.Release(this);
    }

    public enum State   //���� ����
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    public State state = State.PATROL; //�ʱ� ���´� ���� ���·�

    /// <summary>
    /// ������Ƽ�� ������ ����� �����ϰ� ����־�߸� ����
    /// </summary>
    private bool hasTarget
    {
        get
        {
            // ������ ����� �����ϰ�, ����� ������� �ʾҴٸ� true
            if (target != null && !target.dead) return true;
            // �׷��� �ʴٸ� false
            return false;
        }
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        monsterAnimater = GetComponent<Animator>();
        monsterAudioPlayer = GetComponent<AudioSource>();
        monsterPatrol = GetComponent<MonsterPatrol>();
        ws = new WaitForSeconds(0.3f);
        navMeshAgent.updateRotation = false;
    }

    // ���� ������ ���� ������ ��ũ���ͺ� ������Ʈ�� ���� ������ �¾�
    public void Setup(MonsterData monsterData)
    {
        startHp = monsterData.health;
        hp = monsterData.health;
        damage = monsterData.damage;
        navMeshAgent.speed = monsterData.speed;
        timeBetAttack = monsterData.timeBetAttack;
        attackDist = monsterData.attackDist;
    }

    private new void OnEnable()
    {
        StartCoroutine(UpdatePath());
        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }

    private void FixedUpdate()
    {
        
    }

    /// <summary>
    /// �ֱ������� Ÿ�� �˻�, ������ Ÿ�� ����
    /// ������ ����� ��ġ�� ã�� ��� ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdatePath()
    {
        // ��� �ִ� ���� ���� ����
        while (!dead)
        {
            if (hasTarget)
            {
                //���� ��� ���� : ��θ� �����ϰ� AI �̵��� ��� ����
                navMeshAgent.isStopped = false;
                transform.LookAt(target.transform);
                navMeshAgent.SetDestination(
                    target.transform.position);
            }
            else
            {
                //���� ��� ���� : AI �̵� ����
                navMeshAgent.isStopped = true;

                //traceDist ��ŭ�� �������� ���� ������ ���� �׷��� �� ���� ��ġ�� ��� �ݶ��̴��� ������
                //��, targetLayer ���̾ ���� �ݶ��̴��� ���������� ���͸�
                Collider[] colliders =
                    Physics.OverlapSphere(transform.position, traceDist, targetLayer);

                //��� �ݶ��̴��� ��ȸ�ϸ鼭 ��� �ִ� LivingEntity ã��
                for (int i = 0; i < colliders.Length; i++)
                {
                    //�ݶ��̴��κ��� LivingEntity ������Ʈ ��������
                    Life livingEntity = colliders[i].GetComponent<Life>();

                    //LivingEntity ������Ʈ�� �����ϸ�, �ش� LivingEntity�� ��� ������
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        //���� ����� �ش� LivingEntity�� ����
                        target = livingEntity;

                        break;
                    }
                }
            }
            yield return ws;
        }
    }

    // monster�� �µ������� �ǰ� ��ƼŬ�� ����ϰ�
    //base�� �µ������� ȣ���� ������ ����
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            //���ݹ��� ������ �������� ��ƼŬ ȿ�� ���
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            //�ǰ� ȿ���� ���
            monsterAudioPlayer.PlayOneShot(hitSound);
        }
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    // ��� ó��
    public override void Die()
    {
        onDeath += () => StartCoroutine(DestroyMonster());

        // Life�� Die()�� ����
        base.Die();

        //�ٸ� AI�� �������� �ʵ��� �ڽ��� ��� �ݶ��̴��� ��Ȱ��ȭ
        Collider[] monsterColliders = GetComponents<Collider>();
        for (int i = 0; i < monsterColliders.Length; i++)
        {
            monsterColliders[i].enabled = false;
        }

        //AI ������ �����ϰ� ����޽� ������Ʈ�� ��Ȱ��ȭ
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;

        //��� �ִϸ��̼� ���
        monsterAnimater.SetTrigger("Die");
        //��� ȿ���� ���
        monsterAudioPlayer.PlayOneShot(deathSound);
    }

    private void OnTriggerStay(Collider other)
    {
        // Ʈ���� �浹�� ���� ���� ������Ʈ�� ���� ����̶�� ���� ����
        //�ڽ��� ������� �ʾ�����
        //�ֱ� ���� �������� timeBetAttack �̻� �ð��� �����ٸ� ���� ����
        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            //������ LifeŸ�� �������� �õ�
            Life attackTarget =
                other.GetComponent<Life>();

            //������ Life�� �ڽ��� ���� ����̶�� ���� ����
            if (attackTarget != null && attackTarget == target)
            {
                //�ֱ� ���� �ð� ����   
                lastAttackTime = Time.time;

                //������ �ǰ� ��ġ�� �ǰ� ������ �ٻ����� ���
                Vector3 hitPoint =
                    other.ClosestPoint(transform.position);
                Vector3 hitNormal =
                    transform.position = other.transform.position;

                //���� ����
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }

 
    IEnumerator CheckState()
    {
        //�ٸ� ��ũ��Ʈ �ʱ�ȭ�� ���� ���ð�
        yield return new WaitForSeconds(1);

        //�÷��̾�� �� ������ �Ÿ� ���
        while (!dead)
        {
            if (state == State.DIE)
                yield break;

            float dist = Vector3.Distance(this.transform.position,
                                           target.transform.position);
            if (dist <= attackDist)
            {
                if (monsterAngle.isViewPlayer())
                    state = State.ATTACK;
                else
                    state = State.TRACE;
            }
            else if (monsterAngle.isTracePlayer())
            {
                state = State.TRACE;
            }
            else if (dist <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }
            yield return ws;    //�ڷ�ƾ ��ȯ
        }
    }

    /// <summary>
    /// CheckState�� ���°� ���������� ���¿� ���� �׼��� ��������
    /// </summary>
    /// <returns></returns>
    IEnumerator Action()
    {
        while (!dead)
        {
            yield return ws;

            //enumŸ������ ������������ switch�� �����ϰ� ���º�ȯ����
            switch (state)
            {
                case State.PATROL:
                    //enemyFire.isFire = false;
                    monsterPatrol.PATROLLING = true;
                    monsterAnimater.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    //enemyFire.isFire = false;
                    monsterPatrol.TRACETARGET = target.transform.position;
                    monsterAnimater.SetBool(hashHasTarget, hasTarget);
                    break;
                case State.ATTACK:
                    monsterPatrol.Stop();
                    monsterAnimater.SetBool(hashMove, false);
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

                    //�װ��� �ݶ��̴� ��Ȱ��ȭ
                    GetComponent<Collider>().enabled = false;

                    break;
            }
        }
    }
}
