using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Moves the collectables away in a given direction onButtonPress
/// Returns to starting point when the button is no longer pressed
/// </summary>
public class PickupMover : MonoBehaviour, ITriggerable
{
    /// <summary>
    /// Starting position
    /// </summary>
    Vector3 startingPos;

    /// <summary>
    /// Where to go when button is active
    /// </summary>
    [SerializeField]
    Vector3 destinationPos = new Vector3(0f, -3f, 0);

    /// <summary>
    /// How fast to move
    /// </summary>
    [SerializeField]
    float moveSpeed = 5f;

    /// <summary>
    /// Initialize
    /// Registers itself with a Trigger Button
    /// </summary>
    void Start ()
    {
        this.startingPos = this.transform.position;

        TriggerButton button = FindObjectOfType<TriggerButton>();
        if (button != null) {
            button.RegisterButtonEvents(this);
        }
    }
    
    /// <summary>
    /// Hides pickups by sinking into the ground
    /// </summary>
    public void OnButtonPressed()
    {
        StopCoroutine("MoveToPosition");
        StartCoroutine(this.MoveToPosition(this.destinationPos, this.moveSpeed));
    }

    /// <summary>
    /// Revels pickup by rising from the ground
    /// </summary>
    public void OnButtonReleased()
    {
        StopCoroutine("MoveToPosition");
        StartCoroutine(this.MoveToPosition(this.startingPos, this.moveSpeed));
    }

    /// <summary>
    /// Smoothlyh moves this object from its current position to the destination
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    IEnumerator MoveToPosition(Vector3 destination, float speed)
    {
        while(Vector3.Distance(destination, this.transform.position) > 1f) {
            this.transform.position = Vector3.MoveTowards(
                this.transform.position,
                destination,
                speed * Time.deltaTime
            );
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Done Moving");
        this.transform.position = destination;
    }
}
