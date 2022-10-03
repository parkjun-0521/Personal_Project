using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Damage : MonoBehaviour
{
    public float damage;
    public Transform target;
    public Vector3 offset;
    Player player;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }
    void Update()
    {
        transform.position = target.position + offset;

        int rand = Random.Range(1, 101);
        Debug.Log(rand);
        if (rand <= 90)
            damage = 10 + player.power;
        else
            damage = (10 + player.power) * 2;
    }

}
