using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Offset : MonoBehaviour
{
    public float damage;
    public Transform target;
    public Vector3 offset;

    void Update()
    {
        transform.position = target.position + offset;
    }
}
