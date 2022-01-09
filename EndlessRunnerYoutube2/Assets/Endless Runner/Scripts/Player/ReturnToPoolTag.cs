using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script only records the collision with the platform exit trigger so that the return to pool can be started
// Attached to the player so that when the player has moved off a pltform piece it can be re-allocated
public class ReturnToPoolTag : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collided with " + collision.gameObject.name);
    }
}
