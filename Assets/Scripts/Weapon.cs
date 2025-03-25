using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform player; // Reference to the player's transform
    public float bulletSpeed = 10f; // Adjust bullet speed

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; // Ensure it's on the same plane as the player

        // Calculate direction from player to mouse
        Vector3 direction = (mousePos - player.position).normalized;

        // Calculate rotation to face the mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Instantiate bullet and apply velocity
        GameObject bullet = Instantiate(bulletPrefab, player.position, rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
    }
}
