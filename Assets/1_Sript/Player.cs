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

    // ���� ���� 
    Rigidbody2D rigid;              // rigidbody ���� ����
    SpriteRenderer spriteRenderer;  // spriteRenderer ���� ���� 
    Animator anime;

    void Awake()
    {
        // ���� �ʱ�ȭ 
        rigid = GetComponent<Rigidbody2D>();                    // Rigidbody2D ���� �ʱ�ȭ 
        spriteRenderer = GetComponent<SpriteRenderer>();        // SpriteRenderer ���� �ʱ�ȭ 
        anime = GetComponent<Animator>();
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
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = hAxis == -1;
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
        else if (dDown && !isDodge && hAxis != 0 && jumpCheck == true && dodgeCheck == false) {
            speed *= 2;
            maxSpeed *= 2;
            anime.SetTrigger("doDash");
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

    void Attack()
    {
        attackDelay += Time.deltaTime;
        if (aDown && !isDodge && attackDelay > maxDelay) {
            attaackCheck = true;
            anime.SetTrigger("doAttack");
            attackDelay = 0;
            Invoke("AttackEnd", 0.7f);
        }
    }

    void AttackEnd()
    {
        attaackCheck = false;
    }
}
