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

    //private void OnBecameInvisible()
   // {
   //     Destroy(gameObject); // Destroy the bullet if it leaves the screen
   // }
}
