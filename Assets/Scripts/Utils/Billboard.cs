using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        transform.LookAt(transform.position + target.forward);
    }
}