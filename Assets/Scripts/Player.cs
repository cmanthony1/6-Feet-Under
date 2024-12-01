using UnityEngine;
using UnityEngine.UI; // Required for working with UI components

public class Player : MonoBehaviour
{
    public float MovementSpeed = 1; // Player's movement speed
    public GameObject bulletPrefab; // Bullet prefab to instantiate
    public Transform firePoint;     // FirePoint for bullets
    public float Health = 10;       // Current health
    public float MaxHealth = 10;    // Maximum health
    public Slider healthSlider;     // Reference to the health slider

    private void Start()
    {
        // Initialize the health slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = Health;
        }
    }

    private void Update()
    {
        // Update slider smoothly each frame
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, Health, Time.deltaTime * 10f);
        }

        // Other update logic (movement, shooting, etc.)
        var movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * MovementSpeed;

        if (!Mathf.Approximately(movement, 0))
            transform.rotation = movement < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }


    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
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

        Health -= 1;
        Health = Mathf.Clamp(Health, 0, MaxHealth); // Ensure health doesn't drop below 0
        UpdateHealthSlider();

        if (Health <= 0)
        {
            Destroy(gameObject); // Destroy the player
        }
    }

    void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            // Immediately update the slider to match the health
            healthSlider.value = Health;
        }
    }


}
