using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �÷��̾��� �ڽ� ������Ʈ�� �پ��־
/// �����Ҷ� �����ؾ���
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
        //���ڸ�� ��ŸƮ������ �ִϸ����� ������ �ʱ�ȭ����
        anim = GetComponent<Animator>();
        playerManager = GetComponentInParent<PlayerManager>();
        inputHandler = GetComponentInParent<InputHandler>();
        playerLocomotion = GetComponentInParent<PlayerLocomotion>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorValues(float verticalMovenemt, float horizontalMovenemt, bool isSprinting)
    {
        //���ڸ�� ������Ʈ������ �ִϸ����Ͱ� ��� Ȯ��

        //��� ���� region���� ����
        #region Vertical
        //�ִϸ����� ��Ʈ�ѷ� ���� Ʈ���� �����س���
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

        //������ �����ʰ� ���������� Ʈ��� �޸�
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
        //�� �Լ��� �����Ҷ� ���� �ִϸ��̼ǿ��� targetAnim���� 0.2�ʵ��� �ε巴�� ��ȯ 
        anim.CrossFade(targetAnim, 0.2f);
    }

    //�÷��̾ ���� �ȵɶ��� �־ �̰ɷ� ����
    public void CanRotate()
    {
        canRotate = true;
    }

    public void StopRotation()
    {
        canRotate = false;
    }

    //�̰͵� �ִϸ��̼� �׼ǿ��� ó���ϸ� �Ǵ°ǵ� �Ҽ��� ����..
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
        //������ٵ� ������ 0����
        playerLocomotion.rigidbody.drag = 0;
        //��Ÿ���������� �ִϸ��̼� ���ν�Ƽ���� ����
        Vector3 deltaPosition = anim.deltaPosition;
        deltaPosition.y = 0;
        //���� ������
        Vector3 velocity = deltaPosition * Time.deltaTime;
        playerLocomotion.rigidbody.velocity = velocity;
    }

}
