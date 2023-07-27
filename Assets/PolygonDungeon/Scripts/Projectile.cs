using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    Rigidbody rb;
    Transform tr;

    public float damage = 10;
    public float speed = 1000;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
    }
    private void OnEnable()
    {

        rb.AddForce(transform.forward * speed);
    }

    private void OnDisable()
    {
        tr.position = Vector3.zero;
        tr.rotation = Quaternion.identity;
        rb.Sleep();
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    Life attackTarget =
    //        other.GetComponent<Life>();

    //    //상대방의 Life가 자신의 추적 대상이라면 공격 실행
    //    if (attackTarget != null && attackTarget == target)
    //    {
    //        //상대방의 피격 위치와 피격 방향을 근삿값으로 계산
    //        Vector3 hitPoint =
    //            other.ClosestPoint(transform.position);
    //        Vector3 hitNormal =
    //            transform.position = other.transform.position;

    //        //공격 실행
    //        attackTarget.OnDamage(damage, hitPoint, hitNormal);
    //    }
    //}
}
