using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAngle : MonoBehaviour
{
    //���ϴ� ��ġ�� ���ϴ� ������ �� ������ ��°� ��ȯ�ؼ� 

    public float viewRange = 15f; //�þ߰Ÿ�
    public float viewAngle = 120f; //�þ߰�

    Transform monsterTr; //����Ʈ������
    Transform playerTr; //�÷��̾�Ʈ������

    int playerLayer; //�÷��̾�̾�
    int obstacleLayer; //��ֹ����̾�
    int layerMask; // ���̾� ����ũ

    private void Start()
    {
        monsterTr = GetComponent<Transform>();
        //�±׷� �÷��̾� Ʈ������ ã���ϱ� �±� �̸� ���� ���ϱ�
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;

        //���̾��̸��� ����
        playerLayer = LayerMask.NameToLayer("Player");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        //���̾��ũ = ��Ʈ
        //��Ʈ �̵� �����ڸ� ����� ���̾ �ڱ� ���̾� ��ȣ��ŭ �о ã��
        layerMask = 1 << playerLayer | 1 << obstacleLayer;
    }

    /// <summary>
    /// �þ߰Ÿ� �þ߰� �ȿ� �÷��̾� Ž��
    /// </summary>
    /// <returns></returns>
    public bool isTracePlayer()
    {
        bool isTrace = false;

        //OverlapSphere�޼���� �����ص� ���� �þ߰Ÿ� ���� ���ο� �ִ�
        //�ݶ��̴� �߿��� �÷��̾� ���̾ ���� �༮�� �迭�� ��ȯ
        Collider[] colls = Physics.OverlapSphere(monsterTr.position,
                                                 viewRange,
                                                 1 << playerLayer);
        //��ȯ�� �迭�� 1���϶� = �÷��̾ �� �ȿ� Ž��������
        if (colls.Length == 1)
        {
            //���� �������� ���Ͱ� �÷��̾ �ٶ󺸴� ���͸� ����ȭ�� ���Ⱚ�� ����
            Vector3 lookPlayer = (playerTr.position - monsterTr.position).normalized;
            //���Ͱ� �÷��̾ �ٶ󺸴� ������ ���� ������� ���� ������ 60�� �� = 120�� �ȿ� ������ �߰�
            if (Vector3.Angle(monsterTr.forward, lookPlayer) < viewAngle * 0.5)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }

    /// <summary>
    /// �÷��̾� ���� ĳġ
    /// </summary>
    /// <returns></returns>
    public bool isViewPlayer()
    {
        bool isView = false;
        RaycastHit hit;

        //���Ͱ� �÷��̾ �ٶ󺸴� ����
        Vector3 lookPlayer = (playerTr.position - monsterTr.position).normalized;
        //���� ��ġ���� �÷��̾� �������� �þ߰Ÿ���ŭ ���� �߻�. ���� ���̾ ������
        if (Physics.Raycast(monsterTr.position, lookPlayer, out hit, viewRange, layerMask))
        {
            //����Ȱ� �±װ� �÷��̾�� isView �� ��ȯ
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
