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
        player = GameObject.Find("Player").GetComponent<Player>();
        rigid = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        transform.position = target.position + offset;

        Invoke("Move", 0.5f);
    }

    void Move()
    {
        if (player.spriteRenderer.flipX == true) {
            offset.x -= 0.03f;
            Invoke("lMoveEnd", 0.8f);
        }
        else {
            offset.x += 0.03f;
            Invoke("rMoveEnd", 0.8f);
        }
    }

    void rMoveEnd()
    {
        offset.x = 1;
    }
    void lMoveEnd()
    {
        offset.x = -1;
    }
}
