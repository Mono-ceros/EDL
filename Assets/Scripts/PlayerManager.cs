using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public InputHandler inputHandler;
    Animator anim;

    PlayerLocomotion playerLocomotion;
    PlayerStats playerStats;
    StaminaBar staminaBar;

    public bool isInteracting;
    public bool isInAir;
    public bool isGrounded;
    public bool canDoCombo;

    public float interractTime;

    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        anim = GetComponentInChildren<Animator>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerStats = GetComponent<PlayerStats>();
        staminaBar = FindObjectOfType<StaminaBar>();
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
        playerLocomotion.HandleGroundCheck();
    }

    private void LateUpdate()
    {
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

    //private void FixedUpdate()
    //{
    //    float delta = Time.deltaTime;

    //    if (cameraHandler != null)
    //    {
    //        cameraHandler.FollowTarget(delta);
    //        cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
    //    }
    //}

}
