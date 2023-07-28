using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public GameObject player;
    Transform tr;
    Vector3 po;

    void Start()
    {
        tr = GetComponent<Transform>();
        po = tr.position + new Vector3(3f, -4.5f, 50f);
        Instantiate(player, po, Quaternion.identity);
    }

}
