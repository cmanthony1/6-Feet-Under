using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : MonoBehaviour
{
    public float MovementSpeed = 1;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float Health = 10;
    public float MaxHealth = 10;
    public Slider healthSlider;

    public int MaxChamberAmmo = 6;
    public int MaxReserveAmmo = 12;
    private int currentChamberAmmo;
    private int currentReserveAmmo;
    public TMP_Text ammoText;

    public AudioClip gunFireSound;
    public AudioClip outOfAmmoSound;
    public AudioClip reloadSound;
    private AudioSource audioSource;

    private Vector3 originalScale;
    private bool isCrouching = false;
    private Evidence nearbyEvidence;
    public Transform weaponTransform;

    private bool isPaused = false;

    private Animator animator;

    private void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = MaxHealth;
            healthSlider.value = Health;
        }

        currentChamberAmmo = MaxChamberAmmo;
        currentReserveAmmo = MaxReserveAmmo;
        UpdateAmmoText();

        originalScale = transform.localScale;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isPaused) return;

        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, Health, Time.deltaTime * 10f);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl)) StartCrouching();
        else if (Input.GetKeyUp(KeyCode.LeftControl)) StopCrouching();

        float movement = Input.GetAxis("Horizontal");
        float movementSpeedModifier = isCrouching ? 0.5f : 1f;

        // Animation control
        animator.SetFloat("Move", Mathf.Abs(movement));
        Debug.Log("movement:" + movement);

        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * MovementSpeed * movementSpeedModifier;

        if (!Mathf.Approximately(movement, 0))
        {
            transform.rotation = movement < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }

        RotateWeaponTowardsMouse();

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

        if (Input.GetKeyDown(KeyCode.R)) Reload();

        if (Input.GetKeyDown(KeyCode.E) && nearbyEvidence != null)
        {
            nearbyEvidence.Collect();
            nearbyEvidence = null;
        }

        
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
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
            transform.position -= new Vector3(0, originalScale.y / 4, 0);
            // animator.SetBool("IsWalking", false);
            // animator.SetBool("Idle", false);
            // animator.SetBool("IsCrouch", true);
        }
    }

    void StopCrouching()
    {
        if (isCrouching)
        {
            isCrouching = false;  // <-- MUST BE HERE
            transform.localScale = originalScale;
            transform.position += new Vector3(0, originalScale.y / 4, 0);
        }
    }

    public bool IsCrouching() => isCrouching;

    void Shoot()
    {
        if (currentChamberAmmo > 0 && bulletPrefab != null && firePoint != null)
        {
            if (!IsFacingMouseDirection()) return;

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

        if (transform.rotation.y == 0 && directionToMouse.x < 0) return false;
        if (transform.rotation.y != 0 && directionToMouse.x > 0) return false;

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
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage();
            Destroy(collision.gameObject);
        }

        Evidence evidence = collision.GetComponent<Evidence>();
        if (evidence != null)
        {
            nearbyEvidence = evidence;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
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

        StartCoroutine(PlayerShake(0.1f, 0.1f));
        CameraShake.Instance.ShakeCamera(0.2f, 0.2f);

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
            healthSlider.value = Health;
        }
    }

    void UpdateAmmoText()
    {
        if (ammoText != null)
        {
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
            PlayReloadSound();
        }
        else if (currentReserveAmmo == 0)
        {
            Debug.Log("No ammo left in reserve!");
        }
    }

    public void RefillAmmo(int chamberAmount, int reserveAmount)
    {
        currentChamberAmmo = Mathf.Min(currentChamberAmmo + chamberAmount, MaxChamberAmmo);
        currentReserveAmmo = Mathf.Min(currentReserveAmmo + reserveAmount, MaxReserveAmmo);
        UpdateAmmoText();
    }
}
