using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public Transform target;
    public float yOffset = 3f;
    public float cursorFollowAmount = 3f; // How far the camera shifts toward the cursor
    private float fixedYPosition;
    private bool isRecentering = false;

    void Start()
    {
        if (target != null)
        {
            fixedYPosition = target.position.y + yOffset;
        }
    }

    void Update()
    {
        if (target == null) return;

        // Always lock Y position to follow target with offset
        fixedYPosition = target.position.y + yOffset;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 cursorOffset = mouseWorldPos - target.position;

        // Only apply X offset (ignore Y/Z)
        Vector3 cameraTargetPos = target.position + new Vector3(cursorOffset.x, 0f, 0f).normalized * cursorFollowAmount;
        cameraTargetPos.y = fixedYPosition;
        cameraTargetPos.z = -10f;

        // Recenter on 'C' key press
        if (Input.GetKeyDown(KeyCode.C))
        {
            isRecentering = true;
        }

        if (isRecentering)
        {
            cameraTargetPos = new Vector3(target.position.x, fixedYPosition, -10f);
            if (Vector3.Distance(transform.position, cameraTargetPos) < 0.05f)
            {
                isRecentering = false; // Stop recenting when close enough
            }
        }

        // Smoothly follow calculated position
        transform.position = Vector3.Lerp(transform.position, cameraTargetPos, FollowSpeed * Time.deltaTime);
    }
}
