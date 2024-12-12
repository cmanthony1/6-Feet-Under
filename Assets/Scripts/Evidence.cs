using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Script to be attached to evidence objects
using UnityEngine;

public class Evidence : MonoBehaviour
{
    public void Collect()
    {
        // Log message to the console
        Debug.Log("Evidence collected");

        // Destroy the evidence object
        Destroy(gameObject);
    }
}
