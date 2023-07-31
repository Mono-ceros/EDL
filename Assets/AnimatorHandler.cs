using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    public Animator anim;
    int vertical;
    int horizontal;
    public bool canRotate;

    public void Initialize()
    {
        anim = GetComponent<Animator>();
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorValues(float verticalMovenemt, float horizontalMovenemt)
    {
        #region Vertical
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

    public void CanRotate()
    {
        canRotate = true;
    }

    public void StopRotation()
    {
        canRotate = false;
    }
}
