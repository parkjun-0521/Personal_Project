using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ai : MonoBehaviour
{
    public float curhealth;

    public float speed;
    public Transform target;

    bool attackCheck;

    public GameObject RightEnemyAttackBox;
    public GameObject LeftEnemyAttackBox;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anime;
    

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
            if (dis <= 1.2)
                Attack();
            else {
                anime.SetBool("isEnemyAttack", false);
                attackCheck = false;
                Move();
                LeftEnemyAttackBox.SetActive(false);
                RightEnemyAttackBox.SetActive(false);
            }
        }
        else {
            anime.SetBool("isWalk", false);
            LeftEnemyAttackBox.SetActive(false);
            RightEnemyAttackBox.SetActive(false);
            attackCheck = false;
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
        attackCheck = true;
        Turn(attackCheck);
    }

    void Turn(bool atkCheck)
    {
        if (spriteRenderer.flipX == false && atkCheck == true) {
            LeftEnemyAttackBox.SetActive(true);
            RightEnemyAttackBox.SetActive(false);
        }
        else if (spriteRenderer.flipX == true && atkCheck == true) {
            LeftEnemyAttackBox.SetActive(false);
            RightEnemyAttackBox.SetActive(true);
       }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Attack") {
            Attack_Damage ondamage = collision.gameObject.GetComponent<Attack_Damage>();
            Vector3 reactVec = transform.position - collision.transform.position;
            OnHit(ondamage.damage, reactVec);         
        }
    }

    void OnHit(float damage, Vector3 reVec)
    {
        curhealth -= damage;
        reVec = reVec.normalized;
        reVec += Vector3.up;
        rigid.AddForce(reVec * 2f, ForceMode2D.Impulse);
    }
}
