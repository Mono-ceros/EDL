using System; //Action쓰려면 달아야함
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데미지, 회복, 사망
/// </summary>
public class Life : MonoBehaviour, IDamage
{
    public float startHp = 100f; // 시작 체력
    public float hp; // 현재 체력
    public bool dead; // 사망 상태
    public event Action onDeath; // 사망시 발동할 이벤트

    //체력 초기화
    public virtual void OnEnable()
    {
        // 사망하지 않은 상태로 시작
        dead = false;
        // 체력을 시작 체력으로 초기화
        hp = startHp;
    }

    /// <summary>
    /// 데미지를 입는 기능
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="hitPoint"></param>
    /// <param name="hitNormal"></param>
    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        hp -= damage;

        if (hp <= 0 && !dead)
        {
            Die();
        }
    }

    /// <summary>
    /// 체력을 회복하는 기능
    /// </summary>
    /// <param name="healHealth"></param>
    public virtual void HealingHealth(float healHealth)
    {
        if (dead) return;
        hp += healHealth;
    }

    

    /// <summary>
    /// onDeath 이벤트 함수가 존재하면 실행, dead = true로 변경
    /// </summary>
    public virtual void Die()
    {
        // onDeath 이벤트에 등록된 메서드가 있다면 실행
        // Life를 상속하는 개체들의 다른 메소드들을 각자 담으면 
        // 같은 이름으로 편하게 실행시킬수있음.
        if (onDeath != null)
        {
            onDeath();
        }

        // 사망 상태를 참으로 변경
        dead = true;
    }
}
