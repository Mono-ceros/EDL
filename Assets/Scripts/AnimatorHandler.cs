using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플래이어의 자식 오브젝트에 붙어있어서
/// 참조할때 유의해야함
/// </summary>
public class AnimatorHandler : MonoBehaviour
{
    public AnimationClip clip;
    PlayerManager playerManager;
    public Animator anim;
    InputHandler inputHandler;
    PlayerLocomotion playerLocomotion;
    int vertical;
    int horizontal;
    public bool canRotate;

    public void Initialize()
    {
        //로코모션 스타트문에서 애니메이터 변수들 초기화해줌
        anim = GetComponent<Animator>();
        playerManager = GetComponentInParent<PlayerManager>();
        inputHandler = GetComponentInParent<InputHandler>();
        playerLocomotion = GetComponentInParent<PlayerLocomotion>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorValues(float verticalMovenemt, float horizontalMovenemt, bool isSprinting)
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

        //가만히 있지않고 스프린팅이 트루면 달림
        if(isSprinting && inputHandler.moveAmount > 0)
        {
                v = 2;
                h = horizontalMovenemt;
        }

        anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
        anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
    }

    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        anim.applyRootMotion = isInteracting;
        anim.SetBool("isInteracting", isInteracting);
        //이 함수가 동작할때 원래 애니메이션에서 targetAnim으로 0.2초동안 부드럽게 전환 
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

    //이것도 애니메이션 액션에서 처리하면 되는건데 할수가 없네..
    public void EnableCombo()
    {
        anim.SetBool("canDoCombo", true);
    }

    public void DisableCombo()
    {
        anim.SetBool("canDoCombo", false);
    }

    private void OnAnimatorMove()
    {
        if (playerManager.isInteracting == false)
            return;
        //리지드바디 저항을 0으로
        playerLocomotion.rigidbody.drag = 0;
        //델타포지션으로 애니메이션 벨로시티값을 따라감
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        //원래 나눴음
        Vector3 velocity = deltaPosition * Time.deltaTime;
        playerLocomotion.rigidbody.velocity = velocity;
    }

}
