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
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Prevents fast-moving bullets from passing through objects
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ensure the bullet stops when hitting any Rigidbody2D object or BoxCollider2D
        if (collision.rigidbody != null || collision.collider is BoxCollider2D)
        {
            rb.velocity = Vector2.zero; // Stop movement immediately
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

}
