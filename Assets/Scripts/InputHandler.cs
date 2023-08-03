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

    public bool rollFlag;
    public bool sprintFlag;
    public float rollInputTimer;

    //�̸� �������� ��ǲ�׼�
    PlayerControls inputActions;

    PlayerAttacker playerAttacker;
    PlayerInventory playerInventory;

    Vector2 movementInput;
    Vector2 cameraInput;

    private void Awake()
    {
        playerAttacker = GetComponent<PlayerAttacker>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    public void OnEnable()
    {
        //input action���� ������ ������ playercontrols ��ũ��Ʈ�� ����Ǿ�����
        //�ű⿡�� input�� �����ϴ� �� ��ũ��Ʈ���� ���� ���� ������
          if(inputActions == null)
        {
            inputActions = new PlayerControls();
            //��ǲ�̺�Ʈ�� �߻������� ������� �ݹ��ϵ��� �ݹ����ؽ�Ʈ�� �����忡 ���ٽ����� �������
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
    }

    void MoveInput(float delta)
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;
        //clamp01(�ּ�0 �ִ�1������ ������ ����)
        //abs ����
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
            playerAttacker.HandleLightAttack(playerInventory.rightWeapon);
        }

        if(rt_input)
        {
            playerAttacker.HandleHeavyAttack(playerInventory.rightWeapon);
        }
    }

}
