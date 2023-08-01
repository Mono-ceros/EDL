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

    //미리 만들어놓은 인풋액션
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
    

    /// <summary>
    /// MoveInput값을 받아옴
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
        //clamp01(최소0 최대1범위의 값으로 설정)
        //abs 절댓값
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
