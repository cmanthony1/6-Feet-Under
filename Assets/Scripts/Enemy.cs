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

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shotTimer = timeBetweenShots; // Initialize the timer
    }

    // Update is called once per frame
    void Update()
    {
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

        // Shooting logic
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
            // Instantiate the projectile
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            // Calculate direction to the player
            Vector2 direction = (player.position - firePoint.position).normalized;

            // Ensure the projectile faces the correct direction
            projectile.transform.right = direction;

            // Apply velocity to the projectile
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * 10f; // Adjust speed as necessary
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if hit by a bullet
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage();
            Destroy(collision.gameObject); // Destroy the bullet on impact
        }
    }

    void TakeDamage()
    {
        Debug.Log("TakeDamage");
        //health == 2;
        health -= 1;
        if (health <= 0)
        {
            Destroy(gameObject); // Destroy the enemy
        }
    }
}
