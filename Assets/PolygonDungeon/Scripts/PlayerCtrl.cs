using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] //����ȭ //Attribute(�Ӽ�), �ٷ� ������ ���� Ŭ������ ������ ������ ��
public class PlayerAnim
{
    //public AnimationClip idle;
    //public AnimationClip runF;
    //public AnimationClip runB;
    //public AnimationClip runL;
    //public AnimationClip runR;
}
public class PlayerCtrl : MonoBehaviour
{

    bool isBorder = false;

    float h = 0f;
    float v = 0f;
    float r = 0f; //ȸ���� ����

    Transform tr;
    public float moveSpeed = 10f;
    public float rotSpeed = 200f; //�����̼�
    int obstacleLayer;

    //�ν����ͺ信 ǥ���� �ִϸ��̼� Ŭ���� ����
    // public PlayerAnim playerAnim;
    //public Animation anim;

    private void OnEnable()
    {
        tr = GetComponent<Transform>();
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
        //anim = GetComponent<Animation>();
        //anim.clip = playerAnim.idle;
        //anim.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //if (v >= 0.1f) //��
        //{
        //    //CrossFade(������ �ִϸ��̼� Ŭ�� �̸�, ���� �ð�)
        //    anim.CrossFade(playerAnim.runF.name, 0.3f);
        //}
        //else if(v <= -0.1f) //�Ʒ�
        //{
        //    anim.CrossFade(playerAnim.runB.name, 0.3f);
        //}
        //else if(h >= 0.1f) //������
        //{
        //    anim.CrossFade(playerAnim.runR.name, 0.3f);
        //}
        //else if(h<=-0.1f) //����
        //{
        //    anim.CrossFade(playerAnim.runL.name, 0.3f);
        //}
        //else //������
        //{
        //    anim.CrossFade(playerAnim.idle.name, 0.3f);
        //}
    }
    void FixedUpdate()
    {
        h = Input.GetAxis("Horizontal");    //Ű���� �����Է�
        v = Input.GetAxis("Vertical");     //Ű���� �����Է�
        r = Input.GetAxis("Mouse X"); //���콺 X��ǥ�� ���콺 �¿������

        //���� ���� �̵� ������ ������ ������ ���� ���� ���
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        //Translate ��ġ �̵� �Լ�
        //����ȭ �޼���� ���� ����ȭ
        //�밢�� ���� ����
        stopToWall();
        if (isBorder) moveSpeed = 0;
        else moveSpeed = 10;
        Debug.Log(moveSpeed);
        //Debug.Log(moveSpeed);
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime, Space.Self);
        //Rotate ȸ�� �Լ�
        tr.Rotate(Vector3.up * rotSpeed * r * Time.deltaTime);
    }

    void stopToWall()
    {
        isBorder = Physics.Raycast(transform.position, transform.forward, 1, 1 << obstacleLayer) && Physics.Raycast(transform.position, transform.forward, -1, 1 << obstacleLayer);
    }
}
