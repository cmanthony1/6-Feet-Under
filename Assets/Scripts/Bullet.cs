using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 80f;
    public Rigidbody2D rb;

    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Destroy the bullet when it collides with any 2D collider
        if (collision.collider is BoxCollider2D || collision.collider is CircleCollider2D)
        {
            Destroy(gameObject);
        }
        // Check if the bullet collided with the player
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            // Only damage the player if they are not crouching
            if (!player.IsCrouching())
            {
                player.TakeDamage();
            }
        }
    }

    // Uncomment if you want the bullet to be destroyed when it leaves the screen
    //private void OnBecameInvisible()
    //{
    //    Destroy(gameObject);
    //}
}
