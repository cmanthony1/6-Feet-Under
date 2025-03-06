using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float stoppingDistance;
    public float retreatDistance;

    public Transform player;
    public GameObject projectilePrefab; // The projectile prefab to be instantiated
    public Transform firePoint;         // The point from where projectiles will be fired
    public float timeBetweenShots = 2f; // Time interval between shots
    private float shotTimer;            // Timer to manage shooting intervals
    public int health = 3; // Enemy health

    public GameObject door; // Reference to the door object
    private bool canAttack = false; // Determines if the enemy can attack

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shotTimer = timeBetweenShots; // Initialize the timer
    }

    void Update()
    {
        // Check if the door is destroyed
        if (door == null)
        {
            canAttack = true;
        }

        // Prevent movement if the door is still present
        if (!canAttack)
        {
            return;
        }

        // Movement logic
        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else if (Vector2.Distance(transform.position, player.position) < stoppingDistance && Vector2.Distance(transform.position, player.position) > retreatDistance)
        {
            transform.position = this.transform.position;
        }
        else if (Vector2.Distance(transform.position, player.position) < retreatDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
        }

        // Shooting logic (only if the door is destroyed)
        if (shotTimer <= 0)
        {
            ShootAtPlayer();
            shotTimer = timeBetweenShots; // Reset the timer
        }
        else
        {
            shotTimer -= Time.deltaTime;
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = (player.position - firePoint.position);
            direction.y = 0; // Restrict to horizontal
            direction.Normalize();
            projectile.transform.right = direction;

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * 10f; // Adjust speed as necessary
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage();
            Destroy(collision.gameObject); // Destroy the bullet on impact
        }
    }

    void TakeDamage()
    {
        Debug.Log("TakeDamage");
        health -= 1;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
