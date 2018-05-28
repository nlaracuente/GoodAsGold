using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shifts the parent contain from the current position to the target position
/// at given intervals
/// </summary>
public class CoinShifter : MonoBehaviour
{
    /// <summary>
    /// How often to shift
    /// </summary>
    [SerializeField]
    float rate = 5f;

    /// <summary>
    /// How fast to move to destination
    /// </summary>
    [SerializeField]
    float speed = 5f;

    /// <summary>
    /// where it started from
    /// </summary>
    Vector3 startingPos;

    /// <summary>
    /// initial target to move to
    /// </summary>
    [SerializeField]
    Vector3 destinationPos;

    /// <summary>
    /// True when moving towards the destination
    /// </summary>
    bool shiftToDestination = true;

	// Use this for initialization
	void Start ()
    {
        this.startingPos = this.transform.position;
        StartCoroutine(this.Shift());
	}
	
	IEnumerator Shift()
    {
        while (true) {
            yield return new WaitForSeconds(this.rate);

            Vector3 targetPos = this.destinationPos;
            if (!this.shiftToDestination) {
                targetPos = this.startingPos;
            }

            while(Vector3.Distance(this.transform.position, targetPos) > 0.001f) {
                this.transform.position = Vector3.MoveTowards(
                    this.transform.position,
                    targetPos,
                    this.speed * Time.deltaTime
                );
                yield return new WaitForEndOfFrame();
            }

            this.transform.position = targetPos;
            this.shiftToDestination = !this.shiftToDestination;
        }
    }
}
