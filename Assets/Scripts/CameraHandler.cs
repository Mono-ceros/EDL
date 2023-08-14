using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 싱글턴으로 만들어서 인풋핸들러에서 업데이트됨
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

    //싱글턴
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
        //자식 오브젝트 위치 만질때 로컬포지션
        defaultPosition = cameraTransform.localPosition.z;
        //8,9,10번째 레이어를 제외한 레이어
        ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        targetTransform = FindObjectOfType<PlayerManager>().transform;
    }

    public void FollowTarget(float delta)
    {
        //카메라가 타겟을 스무스댐프로 스무스하게 따라감
        Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
        myTransform.position = targetPosition;
        HandleCameraCollisions(delta);
    }

    public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
    {
        //마우스 x,y 배수를 다르게 받아서 카메라가 부드럽게 움직이는것처럼
        lookAngle += (mouseXInput * lookSpeed) / delta;
        pivotAngle -= (mouseYInput * pivotSpeed) / delta;
        //앵글 y축 각도 범위 지정해서 땅천장 뚫고 못보도록
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
        //가상의 구를 던져 첫번째 콜라이더 감지
        //시작점, 반지름, 방향, 충돌점, 최대거리, 레이어
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

        //카메라가 너무 가까워지는걸 방지
        //if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        //{
        //    targetPosition = -minimumCollisionOffset;
        //}

        camerTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta * 10f);
        cameraTransform.localPosition = camerTransformPosition;
    }
}
