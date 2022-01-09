using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroy : MonoBehaviour
{
    [SerializeField] float delay = 10f;                              // How long to wait before returning object to pool

    public delegate void ExitAction();                              // Define the exit action
    public static event ExitAction OnLevelExited;                   // call the event in Level manager to enable return to pool



    private void OnTriggerExit(Collider other)                      // if the player tag returnToPoolTag touches collider, mark it as no longer needed and deactivate
    {
        ReturnToPoolTag returnToPoolTag = other.GetComponent<ReturnToPoolTag>();    // Does the collided object have a return to pool script
        if (returnToPoolTag != null)                                                // If the script exists
        {
            StartCoroutine(WaitAndDeactivate());                                    // Ren the CoRoutine WaitandDeactivate
        }
    }

    IEnumerator WaitAndDeactivate()
    {
        yield return new WaitForSeconds(delay);                                 // Wait for the time defined in delay to elapse
        OnLevelExited();                                                         // Send the event trigger OnLevelExit, which will be picked up in the LevelManager script to re-allocate platform pieces
        transform.root.gameObject.SetActive(false);                              // Set the gameobject that was collided with to an inactive state
    }
}
