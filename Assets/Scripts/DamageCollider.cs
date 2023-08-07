using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    Collider damageCollider;

    public int currentWeaponDamage = 25;

    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        //damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        //damageCollider.enabled = false;
    }

    //이거를 애니메이션 이벤트로 제어하고 싶은데
    //애니메이션 에셋이 읽기 전용이라 다른 방법을 찾아야할듯
    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    //데미지가 여러번 들어감
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();

            if(playerStats != null )
            {
                playerStats.TakeDamage(currentWeaponDamage);
            }
        }

        if(other.tag == "Enemy")
        {
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();

            if (enemyStats != null)
            {
                enemyStats.TakeDamage(currentWeaponDamage);
            }
        }
    }
}
