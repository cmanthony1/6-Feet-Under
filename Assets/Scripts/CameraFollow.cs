using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public Transform target;
    public float yOffset = 3f;
    private float fixedYPosition; // Store fixed Y position of camera
    private bool allowFollow = true; // Allow camera to follow after teleportation

    void Start()
    {
        if (target != null)
        {
            fixedYPosition = target.position.y + yOffset; // Set initial Y position
        }
    }

    void Update()
    {
        if (target != null)
        {
            if (allowFollow)
            {
                fixedYPosition = target.position.y + yOffset; // Allow Y position updates when needed
            }
            Vector3 newPos = new Vector3(target.position.x, fixedYPosition, -10f);
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
        }
    }

    public void AllowCameraFollow() // Call this method after teleportation
    {
        allowFollow = true;
    }

    public void LockCameraYPosition() // Call this method to lock camera Y when needed
    {
        allowFollow = false;
    }
}
