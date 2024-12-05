using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public Rigidbody2D rb;

    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy the bullet when it collides with any collider
        Destroy(gameObject);
    }

    // Uncomment if you want the bullet to be destroyed when it leaves the screen
    //private void OnBecameInvisible()
    //{
    //    Destroy(gameObject);
    //}
}
