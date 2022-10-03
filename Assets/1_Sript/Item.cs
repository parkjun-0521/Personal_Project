using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int money;
    Player player;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.gameObject.tag == "Player") {
            player.point += money;
            Destroy(gameObject);
       }
    }
}
