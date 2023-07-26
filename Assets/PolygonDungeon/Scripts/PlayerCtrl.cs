using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] //직렬화 //Attribute(속성), 바로 직전에 오는 클래스나 변수에 영향을 줌
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runF;
    public AnimationClip runB;
    public AnimationClip runL;
    public AnimationClip runR;
}
public class PlayerCtrl : MonoBehaviour
{
    float h = 0f;
    float v = 0f;
    float r = 0f; //회전값 변수

    Transform tr;
    public float moveSpeed = 10f;
    public float rotSpeed = 200f; //로테이션

    //인스펙터뷰에 표시할 애니메이션 클래스 변수
    public PlayerAnim playerAnim;
    public Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();

        anim = GetComponent<Animation>();
        anim.clip = playerAnim.idle;
        anim.Play();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");    //키보드 수평입력
        v = Input.GetAxis("Vertical");     //키보드 수직입력
        r = Input.GetAxis("Mouse X"); //마우스 X좌표값 마우스 좌우움직임

        //수직 수평 이동 벡터의 벡터의 합으로 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //Translate 위치 이동 함수
        //정규화 메서드로 벡터 정규화
        //대각선 사기맵 ㄴㄴ
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
        //Rotate 회전 함수
        tr.Rotate(Vector3.up * rotSpeed * r * Time.deltaTime);

        //if (v >= 0.1f) //위
        //{
        //    //CrossFade(변경할 애니메이션 클립 이름, 변경 시간)
        //    anim.CrossFade(playerAnim.runF.name, 0.3f);
        //}
        //else if(v <= -0.1f) //아래
        //{
        //    anim.CrossFade(playerAnim.runB.name, 0.3f);
        //}
        //else if(h >= 0.1f) //오른쪽
        //{
        //    anim.CrossFade(playerAnim.runR.name, 0.3f);
        //}
        //else if(h<=-0.1f) //왼쪽
        //{
        //    anim.CrossFade(playerAnim.runL.name, 0.3f);
        //}
        //else //가만히
        //{
        //    anim.CrossFade(playerAnim.idle.name, 0.3f);
        //}
    }
}
