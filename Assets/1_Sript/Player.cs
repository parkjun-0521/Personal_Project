using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // input 변수 
    float hAxis;                    // 이동 input 변수
    bool jDown;                     // 점프 input 변수 
    bool dDown;                     // 회피 input 변수

    // 점프 변수 
    public int jump_Power;          // 점프 파워 
    int jumpCount = 0;              // 2단 점프를 확인하기 
    bool jumpStop = false;          // 무한 점프를 막기  

    // 스피드 변수 
    public float speed;             // 기본 스피드 
    public float maxSpeed;          // 최대 스피드 
    public float masJumpSpeed;      // 최대 점프 스피드 

    // 회피     
    bool isDodge;                   // 무한 회피 막기 
    int DodgeCount;                 // 회피 1번 

    // 변수 선언 
    Rigidbody2D rigid;              // rigidbody 변수 선언
    SpriteRenderer spriteRenderer;  // spriteRenderer 변수 선언 

    void Awake()
    {
        // 변수 초기화 
        rigid = GetComponent<Rigidbody2D>();                    // Rigidbody2D 변수 초기화 
        spriteRenderer = GetComponent<SpriteRenderer>();        // SpriteRenderer 변수 초기화 
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();              
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        jDown = Input.GetButtonDown("Jump");
        dDown = Input.GetButtonDown("Dodge");
    }

    void Move()
    {
        // 단순 이동 로직
        rigid.AddForce(Vector2.right * hAxis * speed, ForceMode2D.Impulse);

        // 최대 속도 제한 
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        
    }

    void Turn()
    {
        // 보는 방향 전환 
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = hAxis == -1;
    }
    void Jump()
    {
        // 2단 점프 로직 
        if (jDown && !isDodge) {
            if (jumpCount >= 0 && jumpStop == false) {
                rigid.AddForce(Vector2.up * jump_Power, ForceMode2D.Impulse);
                jumpCount++;
                if (rigid.velocity.y > masJumpSpeed && jumpCount == 2)
                    rigid.velocity = new Vector2(rigid.velocity.x, masJumpSpeed);
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
                    DodgeCount = 0;
                }
            }
        }
    }

    void Dodge()
    {
        if (dDown && !isDodge && hAxis != 0 && DodgeCount == 0) {
            speed *= 2;
            maxSpeed *= 2;
            DodgeCount++;
            rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
            isDodge = true;
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
}
