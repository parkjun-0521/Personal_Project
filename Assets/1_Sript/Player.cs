using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 이동속도 및 점프 파워 
    public int speed;
    public int jump_Power;

    // 변수 선언 
    Rigidbody2D rigid;
    void Awake()
    {
        // 변수 초기화 
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
         // 단순 점프 로직 
        if (Input.GetButtonDown("Jump")){
            rigid.AddForce(Vector2.up * jump_Power, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        // 단순 이동 로직
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h * speed, ForceMode2D.Impulse);
    }
}
