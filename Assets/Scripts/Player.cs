using UnityEngine;
using UnityEngine.UI; // Required for working with UI components
using TMPro; // Required for working with TextMeshPro

public class Player : MonoBehaviour
{
    public float MovementSpeed = 1; // Player's movement speed
    public GameObject bulletPrefab; // Bullet prefab to instantiate
    public Transform firePoint;     // FirePoint for bullets
    public float Health = 10;       // Current health
    public float MaxHealth = 10;    // Maximum health
    public Slider healthSlider;     // Reference to the health slider

    public int MaxChamberAmmo = 6;  // Maximum ammo in the chamber
    public int MaxReserveAmmo = 12; // Maximum ammo in reserve
    private int currentChamberAmmo; // Current ammo in the chamber
    private int currentReserveAmmo; // Current ammo in reserve
    public TMP_Text ammoText;       // Reference to the TMP UI text for ammo

    private Vector3 originalScale;  // To store the original scale of the player
    private bool isCrouching = false; // To track crouching state

    private void Start()
    {
        // Initialize the health slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = Health;
        }

        // Initialize ammo
        currentChamberAmmo = MaxChamberAmmo;
        currentReserveAmmo = MaxReserveAmmo;
        UpdateAmmoText();

        // Store the original scale of the player
        originalScale = transform.localScale;
    }

    private void Update()
    {
        // Update slider smoothly each frame
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, Health, Time.deltaTime * 10f);
        }

        // Handle crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartCrouching();
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            StopCrouching();
        }

        // If crouching, disable movement and shooting
        if (isCrouching) return;

        // Movement logic
        var movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * MovementSpeed;

        if (!Mathf.Approximately(movement, 0))
            transform.rotation = movement < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

        // Shooting with left mouse button (only) and when ammo is available
        if (Input.GetMouseButtonDown(0) && currentChamberAmmo > 0) // Left Mouse Button and ammo check
        {
            Shoot();
        }

        // Reloading
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void StartCrouching()
    {
        isCrouching = true;
        transform.localScale = new Vector3(originalScale.x, originalScale.y / 2, originalScale.z);
    }

    void StopCrouching()
    {
        isCrouching = false;
        transform.localScale = originalScale;
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    void Shoot()
    {
        if (currentChamberAmmo > 0 && bulletPrefab != null && firePoint != null)
        {
            // Instantiate the bullet and decrement chamber ammo
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            currentChamberAmmo--;
            UpdateAmmoText();
        }
        else
        {
            Debug.Log("Out of ammo in the chamber! Reload!");
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

    public void TakeDamage()
    {
        if (isCrouching)
        {
            Debug.Log("Player is crouching, no damage taken.");
            return;
        }

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

    void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            // Format ammo display as "Chamber/Reserve" (e.g., "6/12")
            ammoText.text = $"{currentChamberAmmo}/{currentReserveAmmo}";
        }
    }

    public void Reload()
    {
        if (currentReserveAmmo > 0 && currentChamberAmmo < MaxChamberAmmo)
        {
            int ammoNeeded = MaxChamberAmmo - currentChamberAmmo;
            int ammoToReload = Mathf.Min(ammoNeeded, currentReserveAmmo);

            currentChamberAmmo += ammoToReload;
            currentReserveAmmo -= ammoToReload;

            UpdateAmmoText();
        }
        else if (currentReserveAmmo == 0)
        {
            Debug.Log("No ammo left in reserve!");
        }
    }

    public void RefillAmmo(int chamberAmount, int reserveAmount)
    {
        // Refill the chamber ammo (but do not exceed max chamber capacity)
        currentChamberAmmo = Mathf.Min(currentChamberAmmo + chamberAmount, MaxChamberAmmo);

        // Refill the reserve ammo (but do not exceed max reserve capacity)
        currentReserveAmmo = Mathf.Min(currentReserveAmmo + reserveAmount, MaxReserveAmmo);

        UpdateAmmoText(); // Update the ammo display
    }
}
