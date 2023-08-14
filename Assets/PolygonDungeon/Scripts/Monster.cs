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
/// ���Ͱ� ����� fsm.
/// �θ��� life�� idamage, monsterdata
/// nav �Ҹ� �ִϸ����� �Ķ���͵��� �˸¾ƾ� ����
/// </summary>
public class Monster : Life
{
    //���� ���� ����� �ڽĵ� ���̾ ���Ͱ� �ƴ϶� �׷�����

    //�޸���� �ȱ�
    //�޸��⸦ �ϸ� �������� �����ؼ� �ٷ� ����
    public int playerLayer; // ���� ��� ���̾�
    [SerializeField]
    Life target; // ���� ���
    NavMeshAgent navMeshAgent; // ��� ��� AI ������Ʈ

    //�ڽ����� ���� ��ƼŬ ����
    public ParticleSystem hitEffect; // �ǰ� �� ����� ��ƼŬ ȿ��

    public AudioClip deathSound; // ��� �� ����� �Ҹ�
    public AudioClip hitSound; // �ǰ� �� ����� �Ҹ�
    Animator monsterAnimater; // �ִϸ����� ������Ʈ
    AudioSource monsterAudioPlayer; // ����� �ҽ� ������Ʈ
    MonsterPatrol monsterPatrol;

    public float damage; // ���ݷ�
    public float timeBetAttack = 0.5f; // ���� ����
    private float lastAttackTime; // ������ ���� ����
    public float viewAngle = 120f; //�þ߰�
    WaitForSeconds ws;

    [Header("���� ��Ÿ�")]
    public float attackDist = 1f;
    [Header("���� �Ÿ�")]
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

    bool isLookPlayer
    {
        get
        {
            RaycastHit hit;

            //���Ͱ� �÷��̾ �ٶ󺸴� ����
            Vector3 lookPlayer = (target.transform.position - transform.position).normalized;
            //���� ��ġ���� �÷��̾� �������� �þ߰Ÿ���ŭ ���� �߻�. ���� ���̾ ������
            if (Physics.Raycast(transform.position, lookPlayer, out hit, attackDist, 1 << playerLayer))
            {
                //����Ȱ� �±װ� �÷��̾�� isView �� ��ȯ
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

    // ���� ������ ���� ������ ��ũ���ͺ� ������Ʈ�� ���� ������ �¾�
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
            //���� �������� ���Ͱ� �÷��̾ �ٶ󺸴� ���͸� ����ȭ�� ���Ⱚ�� ����
            Vector3 lookPlayer = (target.transform.position - transform.position).normalized;
            //���Ͱ� �÷��̾ �ٶ󺸴� ������ ���� ������� ���� ������ ���� ������ �� ���� = ���� ���� �ȿ� ������ �߰�
            if (Vector3.Angle(transform.forward, lookPlayer) < viewAngle * 0.5)
            {
                return true;
            }
            return false;
        }
    }



    /// <summary>
    /// �ֱ������� Ÿ�� �˻�, ������ Ÿ�� ����
    /// Ÿ���� �����ϸ� Ÿ���� �þ� �ȿ� �����ִ��� �˻�
    /// </summary>
    /// <returns></returns>
    //private IEnumerator UpdatePath()
    //{
    //    // ��� �ִ� ���� ���� ����
    //    while (!dead)
    //    {
    //        if(!hasTarget)
    //        {
    //            //traceDist ��ŭ�� �������� ���� ������ ���� �׷��� �� ���� ��ġ�� ��� �ݶ��̴��� ������
    //            //��, targetLayer ���̾ ���� �ݶ��̴��� ���������� ���͸�
    //            Collider[] colliders =
    //                Physics.OverlapSphere(transform.position, traceDist, 1 << playerLayer);

    //            if(colliders.Length > 0)
    //            {
    //                //�ݶ��̴��κ��� LivingEntity ������Ʈ ��������
    //                Life livingEntity = colliders[0].GetComponent<Life>();

    //                //���� ����� �ش� LivingEntity�� ����
    //                target = livingEntity;
    //            }
    //        }
    //        yield return ws;
    //    }
    //}

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

    //private void OnTriggerStay(Collider other)
    //{
    //    // Ʈ���� �浹�� ���� ���� ������Ʈ�� ���� ����̶�� ���� ����
    //    //�ڽ��� ������� �ʾ�����
    //    //�ֱ� ���� �������� timeBetAttack �̻� �ð��� �����ٸ� ���� ����
    //    if (!dead && Time.time >= lastAttackTime + timeBetAttack)
    //    {
    //        //������ LifeŸ�� �������� �õ�
    //        Life attackTarget =
    //            other.GetComponent<Life>();

    //        //������ Life�� �ڽ��� ���� ����̶�� ���� ����
    //        if (attackTarget != null && attackTarget == target)
    //        {
    //            //�ֱ� ���� �ð� ����   
    //            lastAttackTime = Time.time;

    //            //������ �ǰ� ��ġ�� �ǰ� ������ �ٻ����� ���
    //            Vector3 hitPoint =
    //                other.ClosestPoint(transform.position);
    //            Vector3 hitNormal =
    //                transform.position = other.transform.position;

    //            //���� ����
    //            attackTarget.OnDamage(damage, hitPoint, hitNormal);
    //        }
    //    }
    //}

    IEnumerator CheckState()
    {

        //�÷��̾�� �� ������ �Ÿ� ���
        while (!dead)
        {
            if (state == State.DIE)
                yield break;

            float dist = Vector3.Distance(transform.position,
                                           target.transform.position);
            if (isTrace && dist <= attackDist)
            {
                //���� �ȿ��� ������ ���ڸ����� �����ϴ� �̽��� ����
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
            yield return ws;    //�ڷ�ƾ ��ȯ
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

                    //�װ��� �ݶ��̴� ��Ȱ��ȭ
                    GetComponent<Collider>().enabled = false;

                    break;
            }
        }
    }
}
