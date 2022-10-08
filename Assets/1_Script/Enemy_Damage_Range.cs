using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Damage_Range : MonoBehaviour
{
    public float damage;
    public Transform target;
    public Vector3 offset;

    void Update()
    {
        transform.position = target.position + offset;
    }
}
