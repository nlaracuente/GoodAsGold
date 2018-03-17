using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves to all the given destination points and can loop the motions or be a one time movement
/// Movements can be triggered in reversed too
/// Note: this does not discriminate when moving and will continue to move regardless of the status of the path
/// </summary>
public class MoveTransform : MonoBehaviour
{
    /// <summary>
    /// The types of loop a motion can be repeated in
    /// </summary>
    enum LoopType
    {
        None,
        Loop,      // Repeat movement from first destination
        PingPong,  // Traverse through the list top to bottom, bottom to top, and repeat
    }

    /// <summary>
    /// How to loop the movement
    /// </summary>
    [SerializeField]
    LoopType m_loopType = LoopType.None;

    /// <summary>
    /// All the destinations this transform will move to
    /// </summary>
    [SerializeField]
    List<Transform> m_destinations = new List<Transform>();

    /// <summary>
    /// Active destination queue
    /// </summary>
    Queue<Transform> m_destinationQueue;

    /// <summary>
    /// How fast to move
    /// </summary>
    [SerializeField, Tooltip("How fast to move")]
    float m_speed = 5f;

    /// <summary>
    /// Delay between movement
    /// </summary>
    [SerializeField, Tooltip("How long to wait before moving towards next destination")]
    float m_delay = .25f;

    /// <summary>
    /// When true triggers movement to start right way
    /// </summary>
    [SerializeField]
    bool m_autoStart = true;

    /// <summary>
    /// When true causes movement to stop at each destination
    /// </summary>
    [SerializeField, Tooltip("Turn on to stop moving at each new destination. Use this when manually invoking each movement")]
    bool m_segmented = false;

    /// <summary>
    /// When true moves through the queue from top to bottom
    /// </summary>
    bool m_isTopToBottom = true;

    /// <summary>
    /// Creates the queue and starts movement if autostart
    /// </summary>
    void Start()
    {
        CreateQueue();

        if (m_autoStart) {
            Move();
        }
    }

    /// <summary>
    /// Creates the queue of destinations based on how the list is being read (Top to Bottom or Bottom to Top)
    /// </summary>
    void CreateQueue()
    {
        // Temp list so that we can manipulate it
        List<Transform> destination = m_destinations;

        // Start from bottom to top
        if (!m_isTopToBottom) {
            destination.Reverse();
        }

        m_destinationQueue = new Queue<Transform>(destination);
    }

    /// <summary>
    /// Attempts to recreates the queue based on the current loop type
    /// when the current queue is done
    /// </summary>
    void ResetQueue()
    {
        // Don't need to
        if (m_destinationQueue.Count > 0 || m_loopType == LoopType.None) {
            return;
        }

        // Reverse how the list is read
        if(m_loopType == LoopType.PingPong) {
            m_isTopToBottom = !m_isTopToBottom;
        }

        CreateQueue();
    }

    /// <summary>
    /// Triggers movement based on the type of movement (loop or segmented)
    /// </summary>
    public void Move()
    {
        // No where else to move to
        if(m_destinationQueue.Count < 1) {
            return;
        }

        if (m_segmented) {
            StartCoroutine(SegmentRoutine());
        } else {
            StartCoroutine(LoopRoutine());
        }
    }

    /// <summary>
    /// Moves this object towards the given destination using interpolation
    /// </summary>
    /// <param name="destination"></param>
    void MoveTowards(Vector3 destination)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            destination, 
            m_speed * Time.deltaTime
        );
    }

    /// <summary>
    /// Moves towards the next destination point
    /// Resets destination points based on loop type
    /// </summary>
    /// <returns></returns>
    IEnumerator SegmentRoutine()
    {
        Vector3 destination = m_destinationQueue.Dequeue().position;        

        while (transform.position != destination) {
            // Keep up with the physics collision
            yield return new WaitForFixedUpdate();
            MoveTowards(destination);
        }

        ResetQueue();
    }

    /// <summary>
    /// Moves through all the destinations and applies the looping logic at the end of the list
    /// </summary>
    /// <returns></returns>
    IEnumerator LoopRoutine()
    {
        // Loop while we have a destination
        while (m_destinationQueue.Count > 0) {
            Vector3 destination = m_destinationQueue.Dequeue().position;

            // Move until we reach the destination
            while (transform.position != destination) {
                // Keep up with the physics collision
                yield return new WaitForFixedUpdate();
                MoveTowards(destination);
            }

            // Wait for the next round of movement
            yield return new WaitForSeconds(m_delay);
            ResetQueue();
        }
    }
}
