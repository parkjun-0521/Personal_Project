using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int speed;
    Rigidbody2D rigid;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 단순 이동 로직 
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h * speed, ForceMode2D.Force);
    }
}
