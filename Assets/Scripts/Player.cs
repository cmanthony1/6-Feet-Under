using UnityEngine;

public class Player : MonoBehaviour
{
    public float MovementSpeed = 1;
    public GameObject bulletPrefab; // Bullet prefab to instantiate
    public Transform firePoint;     // FirePoint for bullets

    // Update is called once per frame
    private void Update()
    {
        // Movement
        var movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * MovementSpeed;

        if (!Mathf.Approximately(movement, 0))
            transform.rotation = movement < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

        // Shooting
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
}
