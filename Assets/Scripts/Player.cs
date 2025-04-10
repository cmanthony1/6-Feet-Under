using UnityEngine;
using UnityEngine.UI; // Required for working with UI components
using TMPro; // Required for working with TextMeshPro
using UnityEngine.SceneManagement;
using System.Collections; // Required for IEnumerator (Coroutines)


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

    public AudioClip gunFireSound;  // Sound effect for gun firing
    public AudioClip outOfAmmoSound; // Sound effect for out of ammo
    public AudioClip reloadSound;   // Sound effect for reloading
    private AudioSource audioSource; // Audio source component

    private Vector3 originalScale;  // To store the original scale of the player
    private bool isCrouching = false; // To track crouching state
    private Evidence nearbyEvidence; // Reference to nearby evidence
    public Transform weaponTransform; // Assign in the Inspector

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

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        // Update health slider smoothly
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

        // Movement logic (adjust speed when crouched)
        float movementSpeedModifier = isCrouching ? 0.5f : 1f; // Reduce speed while crouching
        var movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * MovementSpeed * movementSpeedModifier;

        // Rotate player sprite based on movement direction
        if (!Mathf.Approximately(movement, 0))
        {
            transform.rotation = movement < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }
        // Rotate weapon to face the mouse cursor
        RotateWeaponTowardsMouse();

        // Shooting logic
        if (Input.GetMouseButtonDown(0))
        {
            if (currentChamberAmmo > 0)
            {
                Shoot();
            }
            else
            {
                PlayOutOfAmmoSound();
            }
        }

        // Reloading
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        // Collect evidence
        if (Input.GetKeyDown(KeyCode.E) && nearbyEvidence != null)
        {
            nearbyEvidence.Collect();
            nearbyEvidence = null;
        }
    }
    private void RotateWeaponTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - weaponTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        weaponTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void StartCrouching()
    {
        if (!isCrouching)
        {
            isCrouching = true;
            transform.localScale = new Vector3(originalScale.x, originalScale.y / 2, originalScale.z);

            // Move player down so they stay attached to the ground
            transform.position -= new Vector3(0, originalScale.y / 4, 0);
        }
    }

    void StopCrouching()
    {
        if (isCrouching)
        {
            isCrouching = false;
            transform.localScale = originalScale;

            // Move player back up to original position
            transform.position += new Vector3(0, originalScale.y / 4, 0);
        }
    }


    public bool IsCrouching()
    {
        return isCrouching;
    }

    void Shoot()
    {
        if (currentChamberAmmo > 0 && bulletPrefab != null && firePoint != null)
        {
            if (!IsFacingMouseDirection()) return; // Prevent shooting in the wrong direction

            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            currentChamberAmmo--;
            UpdateAmmoText();

            Vector3 recoilDirection = transform.rotation.y == 0 ? Vector3.left : Vector3.right;
            transform.position += recoilDirection * 0.025f;

            PlayGunFireSound();
        }
    }

    private bool IsFacingMouseDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 directionToMouse = mousePosition - transform.position;

        // If the player is facing right (y = 0), make sure mouse is to the right
        if (transform.rotation.y == 0 && directionToMouse.x < 0)
            return false;

        // If the player is facing left (y = 180), make sure mouse is to the left
        if (transform.rotation.y != 0 && directionToMouse.x > 0)
            return false;

        return true;
    }

    private void PlayGunFireSound()
    {
        if (gunFireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gunFireSound);
        }
    }

    private void PlayOutOfAmmoSound()
    {
        if (outOfAmmoSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(outOfAmmoSound);
        }
    }

    private void PlayReloadSound()
    {
        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound);
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

        // Check if the object has the Evidence script
        Evidence evidence = collision.GetComponent<Evidence>();
        if (evidence != null)
        {
            nearbyEvidence = evidence;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Clear the reference when the player leaves the trigger
        Evidence evidence = collision.GetComponent<Evidence>();
        if (evidence != null && evidence == nearbyEvidence)
        {
            nearbyEvidence = null;
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
        Health = Mathf.Clamp(Health, 0, MaxHealth);
        UpdateHealthSlider();

        StartCoroutine(PlayerShake(0.1f, 0.1f)); // Shake player
        CameraShake.Instance.ShakeCamera(0.2f, 0.2f); // Shake camera

        if (Health <= 0)
        {
            ReloadScene();
        }
    }

    IEnumerator PlayerShake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }


    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

            // Play reload sound
            PlayReloadSound();
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