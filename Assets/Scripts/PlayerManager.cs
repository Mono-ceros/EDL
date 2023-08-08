using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public InputHandler inputHandler;
    Animator anim;

    CameraHandler cameraHandler;
    PlayerLocomotion playerLocomotion;

    public bool isInteracting;
    public bool isInAir;
    public bool isGrounded;
    public bool canDoCombo;

    public float interractTime;

    private void Awake()
    {
        cameraHandler = FindObjectOfType<CameraHandler>();
    }

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    void Update()
    {
        float delta = Time.deltaTime;

        isInteracting = anim.GetBool("isInteracting");
        canDoCombo = anim.GetBool("canDoCombo");

        //플래이어 포지션
        inputHandler.TickInput(delta);
        playerLocomotion.HandleMovement(delta);
        playerLocomotion.HandlRollingAndSprinting(delta);
        playerLocomotion.HandleFalling(delta, playerLocomotion.moveDirection);
    }

    private void LateUpdate()
    {
        inputHandler.rollFlag = false;
        inputHandler.sprintFlag = false;
        inputHandler.leftClick_input = false;
        inputHandler.wheel_input = false;
        inputHandler.Up_Arrow = false;
        inputHandler.Down_Arrow = false;
        inputHandler.Left_Arrow = false;
        inputHandler.Right_Arrow = false;

        if(isInAir)
        {
            playerLocomotion.inAirTimer += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        float delta = Time.deltaTime;

        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
        }
    }

}
