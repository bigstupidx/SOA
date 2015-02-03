﻿using UnityEngine;
using System.Collections;

public class NetworkedCannonBall : MonoBehaviour
{
    public int health;

    private CashBasherManager manager;

    void Start()
    {
        manager = FindObjectOfType<CashBasherManager>() as CashBasherManager;
    }

    [RPC]
    public void SetVelocity(Vector3 velocity)
    {
        rigidbody2D.velocity = velocity;
    }

    [RPC]
    public void Damage(Vector3 blockPos, float speedDamper)
    {
        health--;

        Vector3 direction = blockPos - transform.position;

        Vector2 velocity = rigidbody2D.velocity;

        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            velocity.x *= speedDamper;
        }
        else
        {
            velocity.y *= speedDamper;
        }

        rigidbody2D.velocity = velocity;

        if (health == 0)
        {
            if (networkView.isMine)
            {        
                Network.Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        if (networkView.isMine)
        {
            manager.ReadyNextTurn();
        }
    }
}
