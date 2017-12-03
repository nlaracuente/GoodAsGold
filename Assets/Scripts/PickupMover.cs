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
    /// Where to go when button is active
    /// </summary>
    [SerializeField]
    Vector3 destinationPos = new Vector3(0f, -3f, 0);

    /// <summary>
    /// Starting position
    /// </summary>
    Vector3 startingPos;

    /// <summary>
    /// The position to move towards
    /// </summary>
    Vector3 targetPos;

    /// <summary>
    /// How fast to move
    /// </summary>
    [SerializeField]
    float moveSpeed = 5f;

    /// <summary>
    /// How close to get to the target before snapping into place
    /// </summary>
    [SerializeField]
    float distanceToTarget = 0.01f;

    /// <summary>
    /// True while the routine is running
    /// </summary>
    bool isRoutineRunning = false;

    /// <summary>
    /// Initialize
    /// Registers itself with a Trigger Button
    /// </summary>
    void Start ()
    {
        this.startingPos = this.transform.position;
        this.targetPos = this.startingPos;

        TriggerButton button = FindObjectOfType<TriggerButton>();
        if (button != null) {
            button.RegisterButtonEvents(this);
        }
    }

    /// <summary>
    /// Fires off a coroutine to move this object to the target position
    /// As long as there isn't one running already
    /// </summary>
    void Update()
    {
        // Wait for the coroutine to be done
        if (this.isRoutineRunning) {
            return;
        }

        // Trigger the coroutine
        if(this.transform.position != this.targetPos) {
            StartCoroutine(this.MoveToPosition());
        }
    }

    /// <summary>
    /// Sets the target position to destination position
    /// </summary>
    public void OnButtonPressed()
    {
        this.targetPos = this.destinationPos;
    }

    /// <summary>
    /// Sets the target position to starting position
    /// </summary>
    public void OnButtonReleased()
    {
        this.targetPos = this.startingPos;
    }

    /// <summary>
    /// Smoothlyh moves this object from its current position to the destination
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    IEnumerator MoveToPosition()
    {
        this.isRoutineRunning = true;

        while(Vector3.Distance(this.targetPos, this.transform.position) > this.distanceToTarget) {
            this.transform.position = Vector3.MoveTowards(
                this.transform.position,
                this.targetPos,
                this.moveSpeed * Time.deltaTime
            );
            yield return new WaitForEndOfFrame();
        }
        
        this.transform.position = this.targetPos;
        this.isRoutineRunning = false;
    }
}
