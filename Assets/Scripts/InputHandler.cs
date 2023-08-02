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

    public bool rollFlag;
    public bool sprintFlag;
    public float rollInputTimer;

    //�̸� �������� ��ǲ�׼�
    PlayerControls inputActions;

    Vector2 movementInput;
    Vector2 cameraInput;


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

    public void HandleRollInput(float delta)
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

    public void HandleSprintInput(float delta)
    {
        leftShift_input = inputActions.PlayerActions.Sprint.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        if(leftShift_input)
        sprintFlag = true;
    }

}
