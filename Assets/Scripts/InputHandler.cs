using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public float moveAmount;
    public float mouseX;
    public float mouseY;

    public bool space_input;
    public bool leftShift_input;
    public bool rb_input;
    public bool rt_input;
    public bool Up_Arrow;
    public bool Down_Arrow;
    public bool Left_Arrow;
    public bool Right_Arrow;

    public bool rollFlag;
    public bool sprintFlag;
    public bool comboFlag;
    public float rollInputTimer;

    //미리 만들어놓은 인풋액션
    PlayerControls inputActions;
    PlayerAttacker playerAttacker;
    PlayerInventory playerInventory;
    PlayerManager playerManager;

    Vector2 movementInput;
    Vector2 cameraInput;

    private void Awake()
    {
        playerAttacker = GetComponent<PlayerAttacker>();
        playerInventory = GetComponent<PlayerInventory>();
        playerManager = GetComponent<PlayerManager>();
    }

    public void OnEnable()
    {
        //input action에서 적용한 설정이 playercontrols 스크립트에 저장되어있음
        //거기에다 input을 관리하는 이 스크립트에서 들어온 값을 전해줌
          if(inputActions == null)
        {
            inputActions = new PlayerControls();
            //인풋이벤트가 발생했을때 밸류값을 콜백하도록 콜백컨텍스트인 퍼폼드에 람다식으로 등록해줌
            inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
            inputActions.PlayerMovement.Camera.performed += inputActions => cameraInput = inputActions.ReadValue<Vector2>();
        }
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    

    public void TickInput(float delta)
    {
        MoveInput(delta);
        HandleRollInput(delta);
        HandleSprintInput(delta);
        HandleAttackInput(delta);
        HandleQuickSlotsInput(delta);
    }

    void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        //clamp01(최소0 최대1범위의 값으로 설정)
        //abs 절댓값
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
        mouseX = cameraInput.x;
        mouseY = cameraInput.y;
    }

    void HandleRollInput(float delta)
    {
        space_input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        if (space_input)
        {
            rollInputTimer += delta;
        }
        else
        {
            if(rollInputTimer > 0 && rollInputTimer < 0.4f)
            {
                sprintFlag = false;
                rollFlag = true;
            }

            rollInputTimer = 0;
        }
    }

    void HandleSprintInput(float delta)
    {
        leftShift_input = inputActions.PlayerActions.Sprint.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        if(leftShift_input)
        sprintFlag = true;
    }

    void HandleAttackInput(float delta)
    {
        inputActions.PlayerActions.RB.performed += i => rb_input = true;
        inputActions.PlayerActions.RT.performed += i => rt_input = true;

        if(rb_input)
        {
            if(playerManager.canDoCombo)
            {
                comboFlag = true;
                playerAttacker.HandleWeaponCombo(playerInventory.rightWeapon);
                comboFlag = false;
            }
            else
            {
                if (playerManager.isInteracting)
                    return;

                if (playerManager.canDoCombo)
                    return;

                playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
            }
        }

        if(rt_input)
        {
            playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
        }
    }

    void HandleQuickSlotsInput(float delta)
    {
        inputActions.PlayerQuickSlots.RightArrow.performed += i => Right_Arrow = true;
        inputActions.PlayerQuickSlots.LeftArrow.performed += i => Left_Arrow = true;
        
        if (Right_Arrow)
        {
            playerInventory.ChangeRightWeapon();
        }
        else if (Left_Arrow)
        {
            playerInventory.ChangeLeftWeapon();
        }
    }

}
