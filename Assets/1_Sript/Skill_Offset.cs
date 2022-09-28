using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Offset : MonoBehaviour
{
    public float damage;
    public Transform target;
    public Vector3 offset;

    Rigidbody2D rigid;
    Player player;

    void Awake()
    {
        player = new Player();
        rigid = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        transform.position = target.position + offset;
        Move();
    }

    void Move()
    {
        if (player.fDown == true) {
            rigid.AddForce(Vector3.forward * 10, ForceMode2D.Impulse);
        }
    }
}
