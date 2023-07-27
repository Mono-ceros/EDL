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

    //    //������ Life�� �ڽ��� ���� ����̶�� ���� ����
    //    if (attackTarget != null && attackTarget == target)
    //    {
    //        //������ �ǰ� ��ġ�� �ǰ� ������ �ٻ����� ���
    //        Vector3 hitPoint =
    //            other.ClosestPoint(transform.position);
    //        Vector3 hitNormal =
    //            transform.position = other.transform.position;

    //        //���� ����
    //        attackTarget.OnDamage(damage, hitPoint, hitNormal);
    //    }
    //}
}
