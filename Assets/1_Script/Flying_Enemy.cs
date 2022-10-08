using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Flying_Enemy : Enemy_Ai
{
    bool moveCheck;

    Rigidbody2D rigid;
    Animator anime;
    SpriteRenderer spriteRenderer;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        // 플레이어와의 거리 계산 
        float dis = Vector3.Distance(transform.position, target.position);
        if (dis <= 7) {
            Move(dis);
        }
    }

    void Move(float d)
    {
        if (!moveCheck) {
            float move = target.position.x - transform.position.x;
            if (move < 0) {
                move = -1;
                spriteRenderer.flipX = true;
            }
            else {
                move = 1;
                spriteRenderer.flipX = false;

            }

            if (d > 3) {
                transform.Translate(new Vector2(move, 0) * speed * Time.deltaTime);
                rigid.constraints = RigidbodyConstraints2D.None;            
            }
            else {
                transform.Translate(new Vector2(move, 0) * 0 * Time.deltaTime);
                rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
                rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
            }
        }
    }

    void Attack()
    {

    }
}
