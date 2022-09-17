using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ai : MonoBehaviour
{
    public float speed;
    public Transform target;

    SpriteRenderer spriteRenderer;
    Animator anime;
    Rigidbody2D rigid;


    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anime = GetComponent<Animator>();
    }

    void Update()
    {
        // 플레이어와의 거리 계산 
        float dis = Vector3.Distance(transform.position, target.position);
        if (dis <= 5) {
            if (dis <= 1)
                Attack();
            else {
                anime.SetBool("isEnemyAttack", false);
                Move();
            }
        }
        else {
            anime.SetBool("isWalk", false);
            return;
        }
    }

    void Move()
    {
        float move = target.position.x - transform.position.x;
        if (move < 0) {
            move = -1;
            spriteRenderer.flipX = false;
        }
        else {
            move = 1;
            spriteRenderer.flipX = true;
        }

        if (move != 0)
            anime.SetBool("isWalk", true);

        transform.Translate(new Vector2(move, 0) * speed * Time.deltaTime);      
    }

    void Attack()
    {
        anime.SetBool("isEnemyAttack", true);
    }
}
