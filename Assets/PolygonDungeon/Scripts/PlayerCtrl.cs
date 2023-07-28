using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] //직렬화 //Attribute(속성), 바로 직전에 오는 클래스나 변수에 영향을 줌
public class PlayerAnim
{
    //public AnimationClip idle;
    //public AnimationClip runF;
    //public AnimationClip runB;
    //public AnimationClip runL;
    //public AnimationClip runR;
}
public class PlayerCtrl : Life
{

    bool isBorder = false;

    public ParticleSystem hitEffect;
    public AudioClip hitSound;
    AudioSource playerAudioPlayer;

    float h = 0f;
    float v = 0f;
    float r = 0f; //회전값 변수

    Transform tr;
    public float moveSpeed = 8f;
    public float rotSpeed = 200f; //로테이션
    int obstacleLayer;

    //인스펙터뷰에 표시할 애니메이션 클래스 변수
    // public PlayerAnim playerAnim;
    //public Animation anim;

    new void OnEnable()
    {
        tr = GetComponent<Transform>();
        playerAudioPlayer = GetComponent<AudioSource>();
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
        //anim = GetComponent<Animation>();
        //anim.clip = playerAnim.idle;
        //anim.Play();
    }

    // Update is called once per frame
    void Update()
    {
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
    void FixedUpdate()
    {
        h = Input.GetAxis("Horizontal");    //키보드 수평입력
        v = Input.GetAxis("Vertical");     //키보드 수직입력
        r = Input.GetAxis("Mouse X"); //마우스 X좌표값 마우스 좌우움직임

        //수직 수평 이동 벡터의 벡터의 합으로 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        stopToWall();
        if (isBorder) moveDir = (Vector3.forward * 0) + (Vector3.right * h);
        //정규화 메서드로 벡터 정규화
        //대각선 거리 맞추기
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
        //Rotate 회전 함수
        tr.Rotate(Vector3.up * rotSpeed * r * Time.deltaTime);
    }

    void stopToWall()
    {
        isBorder = Physics.Raycast(transform.position, transform.forward, 1, 1 << obstacleLayer);
    }

    // monster의 온데미지는 피격 파티클을 재생하고
    //base의 온데미지를 호출해 데미지 적용
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (!dead)
        {
            //공격받은 지점과 방향으로 파티클 효과 재생
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            //피격 효과음 재생
            playerAudioPlayer.PlayOneShot(hitSound);
        }
        base.OnDamage(damage, hitPoint, hitNormal);
    }
}
