using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ai : MonoBehaviour
{
    public float curhealth;

    public float speed;
    public Transform target;
    bool moveCheck = false;

    bool attackCheck;
    public float curDelay;
    public float maxDelay;

    public bool hit = false;

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
            curDelay += Time.deltaTime;
            if (dis <= 1.7 && curDelay > maxDelay) {
                Attack();
            } 
            else {
                if (dis <= 1.2 || hit == true) {
                    rigid.AddForce(Vector3.zero, ForceMode2D.Impulse);
                    anime.SetBool("isWalk", false);
                }
                else
                    Move();
                anime.SetBool("isEnemyAttack", false);
                attackCheck = false;
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
        if (!moveCheck) {
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
    }
 

    void Attack()
    {
        attackCheck = true;           
        if (spriteRenderer.flipX == false && attackCheck == true) {
            LeftEnemyAttackBox.SetActive(true);
            RightEnemyAttackBox.SetActive(false);
            anime.SetBool("isEnemyAttack", true);    
            Invoke("EndAttack", 0.5f);
        }    
        else if (spriteRenderer.flipX == true && attackCheck == true) {
            LeftEnemyAttackBox.SetActive(false);
            RightEnemyAttackBox.SetActive(true);
            anime.SetBool("isEnemyAttack", true);           
            Invoke("EndAttack", 0.5f);
        }
    }

    void EndAttack()
    {
        curDelay = 0;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Attack") {
            hit = true;
            Attack_Damage ondamage = collision.gameObject.GetComponent<Attack_Damage>();
            Vector3 reactVec = transform.position - collision.transform.position;
            OnHit(ondamage.damage, reactVec);         
        }
    }

    void OnHit(float damage, Vector3 reVec)
    {
        curDelay = 0;
        curhealth -= damage;
        reVec = reVec.normalized;
        reVec += Vector3.forward + Vector3.up;
        rigid.AddForce(reVec * 5f, ForceMode2D.Impulse);
        Die();
        Invoke("EndOnHit", 0.3f);
    }

    void EndOnHit()
    {
        hit = false;
    }

    void Die()
    {
        if(curhealth <= 0) {
            // 공격을 멈춤
            // 이동을 멈춤 
            attackCheck = false;
            moveCheck = true;
            anime.SetBool("isWalk", false);

            // 죽는 애니메이션 
            anime.SetTrigger("doDie");

            // 오브젝트 비활성화
            LeftEnemyAttackBox.SetActive(false);
            RightEnemyAttackBox.SetActive(false);
            Invoke("EndDie", 1.2f);

            // 아이템 드랍

        }
    }

    void EndDie()
    {
        gameObject.SetActive(false);
    }
}