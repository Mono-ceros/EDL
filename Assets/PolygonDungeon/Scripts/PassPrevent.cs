using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassPrevent : MonoBehaviour
{
    bool isBorder;


    void FixedUpdate()
    {
        stopToWall();  
    }

    void stopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Obstacle"));
    }
}
