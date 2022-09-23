using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // input ���� 
    float hAxis;                    // �̵� input ����
    bool jDown;                     // ���� input ����
    bool dDown;                     // ȸ�� input ����
    bool aDown;                     // ���� input ����

    // ���� ���� 
    public int jump_Power;          // ���� �Ŀ� 
    int jumpCount = 0;              // 2�� ������ Ȯ���ϱ� 
    bool jumpStop = false;          // ���� ������ ����  
    bool jumpCheck;

    // ���ǵ� ���� 
    public float speed;             // �⺻ ���ǵ� 
    public float maxSpeed;          // �ִ� ���ǵ� 
    public float maxJumpSpeed;      // �ִ� ���� ���ǵ� 

    // ȸ��     
    bool isDodge;                   // ���� ȸ�� ���� 
    bool dodgeCheck;

    // ���� 
    public float attackDelay;
    public float maxDelay;
    bool attaackCheck;

    // �޺� ����  
    public float maxTime;
    public float curTime;
    public int attackCount = 0;

    

    // ���� ���� 
    Rigidbody2D rigid;              // rigidbody ���� ����
    SpriteRenderer spriteRenderer;  // spriteRenderer ���� ���� 
    Animator anime;
    BoxCollider2D boxCollider2D;

    void Awake()
    {
        // ���� �ʱ�ȭ 
        rigid = GetComponent<Rigidbody2D>();                    // Rigidbody2D ���� �ʱ�ȭ 
        spriteRenderer = GetComponent<SpriteRenderer>();        // SpriteRenderer ���� �ʱ�ȭ 
        anime = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Attack();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        jDown = Input.GetButtonDown("Jump");
        dDown = Input.GetButtonDown("Dodge");
        aDown = Input.GetButtonDown("Attack");
    }

    void Move()
    {
        // �ܼ� �̵� ����
        if (!attaackCheck)
            rigid.AddForce(Vector2.right * hAxis * speed, ForceMode2D.Impulse);
        else
            rigid.AddForce(Vector3.zero, ForceMode2D.Impulse);


        // �ִ� �ӵ� ���� 
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        // �̵� �ִϸ��̼� 
        if(rigid.velocity.normalized.x == 0)
            anime.SetBool("isRun", false);
        else
            anime.SetBool("isRun", true);
    }

    void Turn()
    {
        // ���� ���� ��ȯ 
        if (Input.GetButton("Horizontal")) {
            spriteRenderer.flipX = hAxis == -1;
            //gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    void Jump()
    {
        // 2�� ���� ���� 
        if (jDown && !isDodge) {
            if (jumpCount >= 0 && jumpStop == false) {
                rigid.AddForce(Vector2.up * jump_Power, ForceMode2D.Impulse);
                jumpCount++;

                jumpCheck = true;
                // ���� �ִϸ��̼� 
                anime.SetBool("isJump", true);

                if (rigid.velocity.y > maxJumpSpeed && jumpCount == 2)
                    rigid.velocity = new Vector2(rigid.velocity.x, maxJumpSpeed);
                if (jumpCount == 2)
                    jumpStop = true;
            }
        }

        // Ray�� ������Ʈ �Ǻ� 
        if (rigid.velocity.y < 0) {
            Debug.DrawRay(rigid.position, Vector2.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Floor"));
            if (rayHit.collider != null) {
                if (rayHit.distance < 0.7f) {
                    jumpStop = false;
                    jumpCount = 0;
                    jumpCheck = false;
                    dodgeCheck = false;
                    // ���� �ִϸ��̼� 
                    anime.SetBool("isJump", false);
                }
            }
        }
    }

    // ȸ�� ���� 
    void Dodge()
    {
        if (dDown && !isDodge && hAxis != 0 && jumpCheck == false) {
            speed *= 2;
            maxSpeed *= 2;
            anime.SetTrigger("doDash");
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            isDodge = true;
            Invoke("DodgeOut", 0.3f);
        }
        // ���߿����� ȸ�ǰ� �ѹ��� �����ϵ��� ���� �߰� 
        else if (dDown && !isDodge && hAxis != 0 && jumpCheck == true && dodgeCheck == false) {
            speed *= 2;
            maxSpeed *= 2;
            anime.SetTrigger("doDash");
            // ���� ȸ�ǽ� y��ġ ���� 
            rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
            isDodge = true;
            dodgeCheck = true;
            Invoke("DodgeOut", 0.3f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        maxSpeed *= 0.5f;
        rigid.constraints = RigidbodyConstraints2D.None;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        isDodge = false;
    }

    // ���ݰ��� �Լ� 
    void Attack()
    {
        // ���� ���� 
        attackDelay += Time.deltaTime;

        // �޺������� ���� �ð� ���� 
        curTime += Time.deltaTime;
        // ���� �ð��� �Ǹ� �޺� �ʱ�ȭ 
        if (curTime > maxTime)
            attackCount = 0;

        // ���� ���� 
        if (aDown && !isDodge && attackDelay > maxDelay && !jumpCheck) {
            if (attackCount == 0) {
                attaackCheck = true;
                anime.SetTrigger("doAttack_1");
                
                attackDelay = 0;
                curTime = 0;     
                Invoke("AttackEnd", 0.7f);
            }
            else if (attackCount == 1) {
                attaackCheck = true;
                anime.SetTrigger("doAttack_2");
                attackDelay = 0;
                curTime = 0;
                Invoke("AttackEnd", 0.7f);
            }
            else if (attackCount == 2) {
                attaackCheck = true;
                anime.SetTrigger("doAttack_3");
                attackDelay = 0;
                curTime = 0;
                Invoke("AttackEnd", 0.5f);
            }
            attackCount++;

            if (attackCount == 3) 
                attackCount = 0;
        }
        // ���� ���� 
        else if(aDown && !isDodge && attackDelay > maxDelay && jumpCheck) {
            attaackCheck = true;
            anime.SetBool("isJumpAttack", true);
            attackDelay = 0;
            Invoke("AttackEnd", 0.3f);
        }
    }

    // ������ ������ ������ ���� 
    void AttackEnd()
    {
        attaackCheck = false;
        anime.SetBool("isJumpAttack", false);
    }
}
