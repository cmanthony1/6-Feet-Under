using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float stoppingDistance;
    public float retreatDistance;

    public Transform player;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float timeBetweenShots = 2f;
    private float shotTimer;
    public int health = 3;

    public GameObject door;
    private bool canAttack = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shotTimer = timeBetweenShots;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (door == null)
        {
            canAttack = true;
        }

        if (!canAttack)
        {
            return;
        }

        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        }
        else if (Vector2.Distance(transform.position, player.position) < retreatDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, -speed * Time.deltaTime);
        }

        if (shotTimer <= 0)
        {
            ShootAtPlayer();
            shotTimer = timeBetweenShots;
        }
        else
        {
            shotTimer -= Time.deltaTime;
        }
    }

    private void ShootAtPlayer()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = (player.position - firePoint.position).normalized;
            projectile.transform.right = direction;
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * 10f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        Debug.Log("TakeDamage");
        health -= 1;
        StartCoroutine(DamageEffect());

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DamageEffect()
    {
        spriteRenderer.color = Color.red;
        Vector3 originalPosition = transform.position;

        for (int i = 0; i < 5; i++)
        {
            transform.position = originalPosition + (Vector3)Random.insideUnitCircle * 0.1f;
            yield return new WaitForSeconds(0.05f);
        }

        transform.position = originalPosition;
        spriteRenderer.color = Color.white;
    }
}
