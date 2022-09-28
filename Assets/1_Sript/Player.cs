using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    // input ���� 
    float hAxis;                    // �̵� input ����
    bool jDown;                     // ���� input ����
    bool dDown;                     // ȸ�� input ����
    bool aDown;                     // ���� input ����
    public bool fDown;                     // ��ų input ���� 

    // �̵� ����
    bool moveCheck;

    // Turn ���� 
    bool isTurn;

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
    bool attackCheck;

    // �޺� ����  
    public float maxTime;
    public float curTime;
    int attackCount = 0;

    // ü��
    public float curhealth;

    // ��ų
    bool skillCheck;

    // ���� ���� 
    public GameObject RightAttackBox;
    public GameObject LeftAttackBox;
    
    // ��ų �ڽ� object
    public GameObject rSkillBox;
    public GameObject lSkillBox;

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
        Skill();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        jDown = Input.GetButtonDown("Jump");
        dDown = Input.GetButtonDown("Dodge");
        aDown = Input.GetButtonDown("Attack");
        fDown = Input.GetButtonDown("Fire1");
    }

    void Move()
    {
        // �ܼ� �̵� ����
        if (!attackCheck && !skillCheck)
            rigid.AddForce(Vector2.right * hAxis * speed, ForceMode2D.Impulse);
        else
            rigid.AddForce(Vector3.zero, ForceMode2D.Force);


        // �ִ� �ӵ� ���� 
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        // �̵� �ִϸ��̼� 
        if (rigid.velocity.normalized.x == 0) {
            anime.SetBool("isRun", false);
            moveCheck = false;
        }
        else {
            anime.SetBool("isRun", true);
            moveCheck = true;
        }
    }

    void Turn()
    {
        // ���� ���� ��ȯ 
        if (Input.GetButton("Horizontal") && !isTurn) {
            spriteRenderer.flipX = hAxis == -1;

            // ���� �ݶ��̴� ���� 
            if (hAxis == -1 && attackCheck == true) {
                LeftAttackBox.SetActive(true);
                RightAttackBox.SetActive(false);
                stopTrun();
            }
            else if (hAxis == 1 && attackCheck == true) {
                LeftAttackBox.SetActive(false);
                RightAttackBox.SetActive(true);
                stopTrun();
            }
            else if (hAxis == 0 && attackCheck == true) {
                LeftAttackBox.SetActive(false);
                RightAttackBox.SetActive(true);
                stopTrun();
            }
            // ��ų �ݶ��̴� ���� 
            if(hAxis == -1 && skillCheck == true) {
                lSkillBox.SetActive(true);
                rSkillBox.SetActive(false);
                isTurn = true;
                Invoke("EndTurn", 0.5f);
            }
            else if (hAxis == 1 && skillCheck == true) {
                lSkillBox.SetActive(false);
                rSkillBox.SetActive(true);
                isTurn = true;
                Invoke("EndTurn", 0.5f);
            }
        }
        else if (!(Input.GetButton("Horizontal")) && !isTurn) {
            if (spriteRenderer.flipX == false && attackCheck == true) {
                LeftAttackBox.SetActive(false);
                RightAttackBox.SetActive(true);
                stopTrun();
            }
            else if (spriteRenderer.flipX == true && attackCheck == true) {
                LeftAttackBox.SetActive(true);
                RightAttackBox.SetActive(false);
                stopTrun();
            }

            if (spriteRenderer.flipX == false && skillCheck == true) {
                lSkillBox.SetActive(false);
                rSkillBox.SetActive(true);
                isTurn = true;
                Invoke("EndTurn", 0.5f);
            }
            else if (spriteRenderer.flipX == true && skillCheck == true) {
                lSkillBox.SetActive(true);
                rSkillBox.SetActive(false);
                isTurn = true;
                Invoke("EndTurn", 0.5f);
            }
        }
    }

    void stopTrun()
    {
        isTurn = true;
        Invoke("EndTurn", 0.7f);
    }

    void EndTurn()
    {
        isTurn = false;
    }
    void Jump()
    {
        // 2�� ���� ���� 
        if (jDown && !isDodge && attackCheck == false) {
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
                attackCheck = true;
                anime.SetTrigger("doAttack_1");               
                attackDelay = 0;
                curTime = 0;     
                Invoke("AttackEnd", 0.7f);
            }
            else if (attackCount == 1) {
                attackCheck = true;
                anime.SetTrigger("doAttack_2");
                attackDelay = 0;
                curTime = 0;
                Invoke("AttackEnd", 0.7f);
            }
            else if (attackCount == 2) {
                attackCheck = true;
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
            attackCheck = true;
            anime.SetBool("isJumpAttack", true);
            attackDelay = 0;
            Invoke("AttackEnd", 0.3f);
        }
    }

    // ������ ������ ������ ���� 
    void AttackEnd()
    {
        attackCheck = false;
        anime.SetBool("isJumpAttack", false);
        LeftAttackBox.SetActive(false);
        RightAttackBox.SetActive(false);
    }

    void Skill()
    {
        if (fDown && !isDodge && attackCheck == false) {
            skillCheck = true;
            Invoke("EndSkill", 1f);
        }
    }

    void EndSkill()
    {
        skillCheck = false;
        lSkillBox.SetActive(false);
        rSkillBox.SetActive(false);
    }
    void OnTriggerEnter2D( Collider2D collision )
    {
        if (collision.gameObject.tag == "EnemyAttack") {
            Enemy_Damage_Range ondamage = collision.gameObject.GetComponent<Enemy_Damage_Range>();
            Vector3 reactVec = transform.position - collision.transform.position;
            OnHit(ondamage.damage, reactVec);
        }
    }
    void OnHit(float damage, Vector3 reVec)
    {
        rigid.AddForce(Vector3.zero, ForceMode2D.Impulse);
        curhealth -= damage;
        reVec = reVec.normalized;
        reVec += Vector3.forward + Vector3.up;
        rigid.AddForce(reVec * 8.2f, ForceMode2D.Impulse);
        OnDie();
    }
    
    void OnDie()
    {
        if(curhealth <= 0) {
            // ���� ��ǥ ���� 
            // gamemanager�� x,y ��ǥ���� �� ������ ��ġ �ҷ�����

            // �ִϸ��̼� 
            anime.SetTrigger("doDeath");
            attackCheck = true;
            isDodge = true;
            jumpCheck = true;
            // ���� ��ȭ ����
            // ü�� max 
            // ������Ʈ ��Ȱ��ȭ
            LeftAttackBox.SetActive(false);
            RightAttackBox.SetActive(false);
            Invoke("EndDie", 1.2f);
        }
    }

    void EndDie()
    {
        gameObject.SetActive(false);
    }
}
