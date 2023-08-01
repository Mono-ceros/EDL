using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    public Animator anim;
    public InputHandler inputHandler;
    public PlayerLocomotion playerLocomotion;
    int vertical;
    int horizontal;
    public bool canRotate;

    public void Initialize()
    {
        //로코모션 스타트문에서 애니메이터 변수들 초기화해줌
        anim = GetComponent<Animator>();
        inputHandler = GetComponentInParent<InputHandler>();
        playerLocomotion = GetComponentInParent<PlayerLocomotion>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorValues(float verticalMovenemt, float horizontalMovenemt)
    {
        //로코모션 업데이트문에서 애니메이터값 계속 확인

        //기능 마다 region으로 정리
        #region Vertical
        //애니메이터 컨트롤러 블렌드 트리에 정의해놨음
        float v = 0;

        if (verticalMovenemt > 0.55f)
        {
            v = 1;
        }
        else if (verticalMovenemt > 0)
        {
            v = 0.5f;
        }
        else if(verticalMovenemt > - 0.55f)
        {
            v = -0.5f;
        }
        else if(verticalMovenemt < -0.55f)
        {
            v = -1;
        }
        else
        {
            v = 0;
        }
        #endregion

        #region Horizontal
        float h = 0;

        if (horizontalMovenemt > 0.55f)
        {
            h = 1;
        }
        else if (horizontalMovenemt > 0)
        {
            h = 0.5f;
        }
        else if (horizontalMovenemt > -0.55f)
        {
            h = -0.5f;
        }
        else if (horizontalMovenemt < -0.55f)
        {
            h = -1;
        }
        else
        {
            h = 0;
        }
        #endregion

        anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
        anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
    }

    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        anim.applyRootMotion = isInteracting;
        anim.SetBool("isInteracting", isInteracting);
        anim.CrossFade(targetAnim, 0.2f);
    }

    //플래이어가 돌면 안될때가 있어서 이걸로 제어
    public void CanRotate()
    {
        canRotate = true;
    }

    public void StopRotation()
    {
        canRotate = false;
    }

    private void OnAnimatorMove()
    {
        if (inputHandler.isInteracting == false)
            return;

        float delta = Time.deltaTime;
        playerLocomotion.rigidbody.drag = 0;
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        Vector3 velocity = deltaPosition / delta;
        playerLocomotion.rigidbody.velocity = velocity;
    }

}
