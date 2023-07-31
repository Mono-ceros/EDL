using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public GameObject player;
    Transform tr;
    Vector3 po;

    void Awake()
    {
        tr = GetComponent<Transform>();
        po = tr.position + new Vector3(-50f, 5f, -11f);
        Instantiate(player, po, Quaternion.identity);
    }

}
