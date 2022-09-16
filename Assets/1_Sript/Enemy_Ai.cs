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
        float dis = Vector3.Distance(transform.position, target.position);

        if (dis <= 5) 
            Move();
        else
            return;
    }

    void Move()
    {
        float dir = target.position.x - transform.position.x;
        if (dir < 0) {
            dir = -1;
            spriteRenderer.flipX = false;
        }
        else { 
            dir = 1;
            spriteRenderer.flipX = true;
        }

        if (dir != 0)
            anime.SetBool("isWalk", true);

        transform.Translate(new Vector2(dir, 0) * speed * Time.deltaTime);      
    }
}
