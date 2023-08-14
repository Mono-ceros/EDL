using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �̱������� ���� ��ǲ�ڵ鷯���� ������Ʈ��
/// </summary>
public class CameraHandler : MonoBehaviour
{
    public Transform targetTransform;
    public Transform cameraTransform;
    public Transform cameraPivotTransform;
    Transform myTransform;
    Vector3 camerTransformPosition;
    LayerMask ignoreLayers;
    Vector3 cameraFollowVelocity = Vector3.zero;

    //�̱���
    public static CameraHandler singleton;

    public float lookSpeed = 0.05f;
    public float followSpeed = 0.05f;
    public float pivotSpeed = 0.02f;

    float targetPosition;
    float defaultPosition;
    float lookAngle;
    float pivotAngle;
    public float minimumPivot = -35;
    public float maximumPivot = 35;

    public float cameraSphereRadius = 0.3f;
    public float cameraCollisionOffset = 0.2f;
    public float minimumCollisionOffset = 0.2f;

    private void Awake()
    {
        singleton = this;
        myTransform = transform;
        //�ڽ� ������Ʈ ��ġ ������ ����������
        defaultPosition = cameraTransform.localPosition.z;
        //8,9,10��° ���̾ ������ ���̾�
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        targetTransform = FindObjectOfType<PlayerManager>().transform;
    }

    public void FollowTarget(float delta)
    {
        //ī�޶� Ÿ���� ������������ �������ϰ� ����
        Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
        myTransform.position = targetPosition;
        HandleCameraCollisions(delta);
    }

    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
        //���콺 x,y ����� �ٸ��� �޾Ƽ� ī�޶� �ε巴�� �����̴°�ó��
        lookAngle += (mouseXInput * lookSpeed) / delta;
        pivotAngle -= (mouseYInput * pivotSpeed) / delta;
        //�ޱ� y�� ���� ���� �����ؼ� ��õ�� �հ� ��������
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        myTransform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    public void HandleCameraCollisions(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position; ;
        direction.Normalize();
        //Mathf.Abs(targetPosition)
        //������ ���� ���� ù��° �ݶ��̴� ����
        //������, ������, ����, �浹��, �ִ�Ÿ�, ���̾�
        if (Physics.SphereCast
            (cameraPivotTransform.position,
             cameraSphereRadius, direction,
             out hit, 5f,
             ignoreLayers))
        {
            cameraTransform.localPosition = hit.point;
            //float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
            //targetPosition = -(dis - cameraCollisionOffset);

        }

        //ī�޶� �ʹ� ��������°� ����
        //if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        //{
        //    targetPosition = -minimumCollisionOffset;
        //}

        camerTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta * 10f);
        cameraTransform.localPosition = camerTransformPosition;
    }
}
