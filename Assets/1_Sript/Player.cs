using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // input ���� 
    float hAxis;                    // �̵� input ����
    bool jDown;                     // ���� input ���� 
    bool dDown;                     // ȸ�� input ����

    // ���� ���� 
    public int jump_Power;          // ���� �Ŀ� 
    int jumpCount = 0;              // 2�� ������ Ȯ���ϱ� 
    bool jumpStop = false;          // ���� ������ ����  

    // ���ǵ� ���� 
    public float speed;             // �⺻ ���ǵ� 
    public float maxSpeed;          // �ִ� ���ǵ� 
    public float masJumpSpeed;      // �ִ� ���� ���ǵ� 

    // ȸ��     
    bool isDodge;                   // ���� ȸ�� ���� 
    int DodgeCount;                 // ȸ�� 1�� 

    // ���� ���� 
    Rigidbody2D rigid;              // rigidbody ���� ����
    SpriteRenderer spriteRenderer;  // spriteRenderer ���� ���� 

    void Awake()
    {
        // ���� �ʱ�ȭ 
        rigid = GetComponent<Rigidbody2D>();                    // Rigidbody2D ���� �ʱ�ȭ 
        spriteRenderer = GetComponent<SpriteRenderer>();        // SpriteRenderer ���� �ʱ�ȭ 
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
        // �ܼ� �̵� ����
        rigid.AddForce(Vector2.right * hAxis * speed, ForceMode2D.Impulse);

        // �ִ� �ӵ� ���� 
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        
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
                if (rigid.velocity.y > masJumpSpeed && jumpCount == 2)
                    rigid.velocity = new Vector2(rigid.velocity.x, masJumpSpeed);
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
