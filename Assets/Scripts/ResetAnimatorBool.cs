using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 기본이동 이외의 동작 애니메이션을 실행할때 애니메이터 파라미터인 isInteracting값을 바꿔줌
/// empty 애니메이터 스테이트에 참조되있음
/// </summary>
public class ResetAnimatorBool : StateMachineBehaviour
{
    public string targetBool;
    public bool status;

    //애니메이션이 특정상태로 진입할때 호출되는 메서드
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(targetBool, status);
    }
}
