using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    Transform cameraObject;
    public InputHandler inputHandler;
    public  Vector3 moveDirection;

    [HideInInspector]
    public Transform myTransform;

    public AnimatorHandler animatorHandler;

    public new Rigidbody rigidbody;
    public GameObject normalCamera;

    //낙하를 위해 사용하는 애들
    [SerializeField]
    float groundDetectionRayStartPoint = 0.5f;
    [SerializeField]
    float minimumDistanceNeededToBeginFall = 1f;
    [SerializeField]
    float groundDirectionRayDistance = 0.2f;
    LayerMask ignoreForGroundCheck;
    public float inAirTimer;

    [SerializeField]
    float movementSpeed = 4;
    [SerializeField]
    float sprintSpeed = 6;
    [SerializeField]
    float rotationSpeed = 10;
    [SerializeField]
    float fallingSpeed = 80;

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        rigidbody = GetComponent<Rigidbody>();
        inputHandler = GetComponent<InputHandler>();
        animatorHandler = GetComponentInChildren<AnimatorHandler>();
        cameraObject = Camera.main.transform;
        myTransform = transform;
        animatorHandler.Initialize();

        playerManager.isGrounded = true;
        ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
    }



    #region Movement
    Vector3 normalVector;
    Vector3 targetPosition;

    void HandleRotation(float delta)
    {
        Vector3 targetDir = Vector3.zero;
        float moveOverride = inputHandler.moveAmount;

        targetDir = (cameraObject.forward * inputHandler.vertical) + (cameraObject.right * inputHandler.horizontal);

        targetDir.Normalize();
        targetDir.y = 0;

        if(targetDir == Vector3.zero)
        {
            targetDir = myTransform.forward;
        }

        float rs = rotationSpeed;

        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

        myTransform.rotation = targetRotation;

    }

    public void HandleMovement(float delta)
    {
        if (inputHandler.rollFlag)
            return;

        moveDirection = (cameraObject.forward * inputHandler.vertical) + (cameraObject.right * inputHandler.horizontal);
        moveDirection.Normalize();
        moveDirection.y = 0;

        float speed = movementSpeed;

        if (inputHandler.sprintFlag)
        {
            speed = sprintSpeed;
        }

        //movementSpeed *= speed;
        Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
        rigidbody.velocity = projectedVelocity * speed;

        animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.inputHandler.sprintFlag);

        //canRotate가 참일때 플래이어 로테이션
        if (animatorHandler.canRotate)
        {
            HandleRotation(delta);
        }
    }

    public void HandlRollingAndSprinting(float delta)
    {
        if (animatorHandler.anim.GetBool("isInteracting"))
            return;

        if(inputHandler.rollFlag)
        {
            moveDirection = (cameraObject.forward * inputHandler.vertical) + (cameraObject.right * inputHandler.horizontal);

            if(inputHandler.moveAmount > 0)
            {
                animatorHandler.PlayTargetAnimation("Rolling", true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                myTransform.rotation = rollRotation;
            }
        }

    }

    public void HandleFalling(float delta, Vector3 moveDirection)
    {
        playerManager.isGrounded = false;
        RaycastHit hit;
        Vector3 origin = myTransform.position;
        origin.y += groundDetectionRayStartPoint;

        if(Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
        {
            moveDirection = Vector3.zero;
        }

        if(playerManager.isInAir)
        {
            rigidbody.AddForce(-Vector3.up * fallingSpeed * 3);
            rigidbody.AddForce(moveDirection * fallingSpeed);
        }

        Vector3 dir = moveDirection + Vector3.back;
        dir.Normalize();
        origin = origin + dir * groundDirectionRayDistance;

        targetPosition = myTransform.position;

        //떨어지는 중인데 아래에 땅이 검출될때 == 땅에 다으려할떄
        if(Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignoreForGroundCheck))
        {
            normalVector = hit.normal;
            Vector3 tp = hit.point;
            playerManager.isGrounded = true;
            targetPosition.y = tp.y;

            //0.5초 이상 떠있어야 랜드 애니메이션 실행
            if(playerManager.isInAir)
            {
                if(inAirTimer > 0.5f)
                {
                    animatorHandler.PlayTargetAnimation("Land", true);
                    inAirTimer = 0;
                }
                else
                {
                    animatorHandler.PlayTargetAnimation("Land", false);
                    inAirTimer = 0;
                }

                playerManager.isInAir = false;
            }
        }
        //떨어지는 중일때
        else
        {
            if(playerManager.isGrounded)
            {
                playerManager.isGrounded = false;
            }

            if(playerManager.isInAir == false)
            {
                if(playerManager.isInteracting == false)
                {
                    StartCoroutine(Fallani());
                }

                Vector3 vel = rigidbody.velocity;
                vel.Normalize();
                rigidbody.velocity = vel * movementSpeed;
                playerManager.isInAir = true;
            }


        }

        if(playerManager.isGrounded)
        {
            if (playerManager.isInteracting || inputHandler.moveAmount> 0)
            {
                myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime);
            }
            else
            {
                myTransform.position = targetPosition;
            }
        }

    }

    //떨어지는 모션이 0.1초 있다 재생되게
    IEnumerator Fallani()
    {
        yield return new WaitForSeconds(0.1f);
        animatorHandler.PlayTargetAnimation("Falling", true);
    }

    #endregion
}
