using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    // input 변수 
    float hAxis;                    // 이동 input 변수
    bool jDown;                     // 점프 input 변수
    bool dDown;                     // 회피 input 변수
    bool aDown;                     // 공격 input 변수
    bool fDown;                     // 스킬 input 변수 
    bool iDown;                     // 상호작용 input 변수

    bool moveCheck;

    // Turn 변수 
    bool isTurn;

    // 점프 변수 
    public int jump_Power;          // 점프 파워 
    int jumpCount = 0;              // 2단 점프를 확인하기 
    bool jumpStop = false;          // 무한 점프를 막기  
    bool jumpCheck;

    // 스피드 변수 
    public float speed;             // 기본 스피드 
    public float maxSpeed;          // 최대 스피드 
    public float maxJumpSpeed;      // 최대 점프 스피드 

    // 회피     
    bool isDodge;                   // 무한 회피 막기 
    bool dodgeCheck;

    // 공격 
    public float attackDelay;
    public float maxDelay;
    bool attackCheck;

    // 콤보 공격  
    public float maxTime;
    public float curTime;
    int attackCount = 0;

    // 체력
    public float curhealth;

    public int power = 10;

    // 스킬
    bool skillCheck;
    public float skillkDelay;
    public float maxskillDelay;

    // 아이템 
    public int point;

    bool isShop;

    // 변수 선언 
    public GameObject RightAttackBox;
    public GameObject LeftAttackBox;
    
    // 스킬 박스 object
    public GameObject rSkillBox;
    public GameObject lSkillBox;

    GameObject nearObject;
    Rigidbody2D rigid;              // rigidbody 변수 선언
    public SpriteRenderer spriteRenderer;  // spriteRenderer 변수 선언 
    Animator anime;
    BoxCollider2D boxCollider2D;

    void Awake()
    {
        // 변수 초기화 
        rigid = GetComponent<Rigidbody2D>();                    // Rigidbody2D 변수 초기화 
        spriteRenderer = GetComponent<SpriteRenderer>();        // SpriteRenderer 변수 초기화 
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
        Interation();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        jDown = Input.GetButtonDown("Jump");
        dDown = Input.GetButtonDown("Dodge");
        aDown = Input.GetButtonDown("Attack");
        fDown = Input.GetButtonDown("Fire1");
        iDown = Input.GetButtonDown("Interation");
    }

    void Move()
    {
        if (!moveCheck) {
            // 단순 이동 로직
             rigid.AddForce(Vector2.right * hAxis * speed, ForceMode2D.Impulse);


            // 최대 속도 제한 
            if (rigid.velocity.x > maxSpeed)
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            else if (rigid.velocity.x < maxSpeed * (-1))
                rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

            // 이동 애니메이션 
            if (rigid.velocity.normalized.x == 0) {
                anime.SetBool("isRun", false);
            }
            else {
                anime.SetBool("isRun", true);
            }
        }
    }

    void Turn()
    {
        // 보는 방향 전환 
        if (Input.GetButton("Horizontal") && !isTurn) {
            if(!attackCheck && !skillCheck)
                spriteRenderer.flipX = hAxis == -1;

            // 공격 콜라이더 생성 
            if (hAxis == -1 && attackCheck == true) {
                LAttackSetAtive();
            }
            else if (hAxis == 1 && attackCheck == true) {
                RAttackSetAtive();
            }
            else if (hAxis == 0 && attackCheck == true) {
                RAttackSetAtive();
            }
            // 스킬 콜라이더 생성 
            if(hAxis == -1 && skillCheck == true) {
                LSkillkSetAtive();
            }
            else if (hAxis == 1 && skillCheck == true) {
                RSkillkSetAtive();
            }
        }
        else if (!(Input.GetButton("Horizontal")) && !isTurn) {
            if (spriteRenderer.flipX == false && attackCheck == true) {
                RAttackSetAtive();
            }
            else if (spriteRenderer.flipX == true && attackCheck == true) {
                LAttackSetAtive();
            }

            if (spriteRenderer.flipX == false && skillCheck == true) {
                RSkillkSetAtive();
            }
            else if (spriteRenderer.flipX == true && skillCheck == true) {
                LSkillkSetAtive();
            }
        }
    }

    void LAttackSetAtive()
    {
        LeftAttackBox.SetActive(true);
        RightAttackBox.SetActive(false);
        stopTrun(0.7f);
    } 
    void RAttackSetAtive()
    {
        LeftAttackBox.SetActive(false);
        RightAttackBox.SetActive(true);
        stopTrun(0.7f);
    }
    void LSkillkSetAtive()
    {
        lSkillBox.SetActive(true);
        rSkillBox.SetActive(false);
        stopTrun(1f);
    }
    void RSkillkSetAtive()
    {
        lSkillBox.SetActive(false);
        rSkillBox.SetActive(true);
        stopTrun(1f);
    }

    void stopTrun(float sec)
    {
        isTurn = true;
        Invoke("EndTurn", sec);
    }

    void EndTurn()
    {
        isTurn = false;
    }
    void Jump()
    {
        // 2단 점프 로직 
        if (jDown && !isDodge && attackCheck == false) {
            if (jumpCount >= 0 && jumpStop == false) {
                rigid.AddForce(Vector2.up * jump_Power, ForceMode2D.Impulse);
                jumpCount++;

                jumpCheck = true;
                // 점프 애니메이션 
                anime.SetBool("isJump", true);

                if (rigid.velocity.y > maxJumpSpeed && jumpCount == 2)
                    rigid.velocity = new Vector2(rigid.velocity.x, maxJumpSpeed);
                if (jumpCount == 2)
                    jumpStop = true;
            }
        }

        // Ray로 오브젝트 판별 
        if (rigid.velocity.y < 0) {
            Debug.DrawRay(rigid.position, Vector2.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Floor"));
            if (rayHit.collider != null) {
                if (rayHit.distance < 0.7f) {
                    jumpStop = false;
                    jumpCount = 0;
                    jumpCheck = false;
                    dodgeCheck = false;
                    // 점프 애니메이션 
                    anime.SetBool("isJump", false);
                }
            }
        }
    }

    // 회피 구현 
    void Dodge()
    {
        if (dDown && !isDodge && hAxis != 0 && jumpCheck == false && !isShop) {
            speed *= 2;
            maxSpeed *= 2;
            anime.SetTrigger("doDash");
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            isDodge = true;
            Invoke("DodgeOut", 0.3f);
        }
        // 공중에서는 회피가 한번만 가능하도록 조건 추가 
        else if (dDown && !isDodge && hAxis != 0 && jumpCheck == true && dodgeCheck == false) {
            speed *= 2;
            maxSpeed *= 2;
            anime.SetTrigger("doDash");
            // 공중 회피시 y위치 고정 
            rigid.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
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

    // 공격관련 함수 
    void Attack()
    {
        // 지상 공격 
        attackDelay += Time.deltaTime;

        // 콤보공격을 위한 시간 지정 
        curTime += Time.deltaTime;

        // 일정 시간이 되면 콤보 초기화 
        if (curTime > maxTime)
            attackCount = 0;

        // 연속 공격 
        if (aDown && !isDodge && attackDelay > maxDelay && !jumpCheck && !isShop) {
            moveCheck = true;
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
        // 공중 공격 
        else if(aDown && !isDodge && attackDelay > maxDelay && jumpCheck && !isShop) {
            attackCheck = true;
            anime.SetBool("isJumpAttack", true);
            attackDelay = 0;
            Invoke("AttackEnd", 0.3f);
        }
    }

    // 공격이 끝나고 원상태 복귀 
    void AttackEnd()
    {
        moveCheck = false;
        attackCheck = false;
        anime.SetBool("isJumpAttack", false);
        LeftAttackBox.SetActive(false);
        RightAttackBox.SetActive(false);
    }

    void Skill()
    {
        skillkDelay += Time.deltaTime;
        if (fDown && !isDodge && attackCheck == false && skillkDelay > maxskillDelay && !jumpCheck && !isShop) {
            moveCheck = true;
            skillCheck = true;
            anime.SetTrigger("doSkill");
            skillkDelay = 0;
            Invoke("EndSkill", 1f);
        }
        else if (fDown && !isDodge && !attackCheck && skillkDelay > maxskillDelay && jumpCheck && !isShop) {
            moveCheck = true;
            skillCheck = true;
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            anime.SetTrigger("jumpSkill");
            skillkDelay = 0;
            Invoke("EndSkill", 1f);
            Invoke("End", 0.9f);
        }
    }

    void End()
    {
        rigid.constraints = RigidbodyConstraints2D.None;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void EndSkill()
    {
        moveCheck = false;
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
            // 스폰 좌표 지정 
            // gamemanager에 x,y 좌표저장 후 리스폰 위치 불러오기

            // 애니메이션 
            anime.SetTrigger("doDeath");
            attackCheck = true;
            isDodge = true;
            jumpCheck = true;
            // 일정 제화 감소
            // 체력 max 
            // 오브젝트 비활성화
            LeftAttackBox.SetActive(false);
            RightAttackBox.SetActive(false);
            Invoke("EndDie", 1.2f);
        }
    }

    void EndDie()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "NPC") {
            nearObject = collision.gameObject;
        }
    }

    void Interation()
    {
        if (iDown && nearObject != null && !jumpCheck && !attackCheck && !isDodge) {
            if(nearObject.tag == "NPC") {
                Debug.Log("상점");
                NPC_Shop shop = nearObject.GetComponent<NPC_Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "NPC") {
            NPC_Shop shop = nearObject.GetComponent<NPC_Shop>();

            // Exit 함수 작동 
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
}
