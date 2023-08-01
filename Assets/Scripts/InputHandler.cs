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

    public bool b_Input;
    public bool rollFlag;
    public bool isInteracting;

    //�̸� �������� ��ǲ�׼�
    PlayerControls inputActions;
    CameraHandler cameraHandler;

    Vector2 movementInput;
    Vector2 cameraInput;

    private void Awake()
    {
        cameraHandler = CameraHandler.singleton;
    }

    private void FixedUpdate()
    {
        float delta = Time.deltaTime;

        if (cameraHandler != null)
        {
            cameraHandler.FollowTarget(delta);
            cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
        }
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
    

    /// <summary>
    /// MoveInput���� �޾ƿ�
    /// </summary>
    /// <param name="delta"></param>
    public void TickInput(float delta)
    {
        MoveInput(delta);
        HandleRollInput(delta);
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
        b_Input = inputActions.PlayerActions.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;

        if(b_Input)
        {
            rollFlag = true;
        }
    }

}
