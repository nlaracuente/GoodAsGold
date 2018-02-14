using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles moving the player based on player input
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    /// <summary>
    /// How much distance must there be between the player and the input to initiate movement
    /// </summary>
    [SerializeField]
    float m_deadzone = .5f;

    /// <summary>
    /// How fast the player moves
    /// </summary>
    [SerializeField]
    float m_moveSpeed = 5f;

    /// <summary>
    /// Keeps track of the current speed the player is moving at
    /// </summary>
    [SerializeField]
    float m_currentSpeed = 0f;
    /// <summary>
    /// Current speed the player is moving at
    /// </summary>
    public float CurrentSpeed { get { return Mathf.Abs(m_currentSpeed); } }

    /// <summary>
    /// A reference to the character controller
    /// </summary>
    CharacterController m_charController;

    /// <summary>
    /// Sets references to components
    /// </summary>
    void Awake()
    {
        m_charController = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Moves the player towards the targetPosition using the character controller
    /// Movement only occurs when the current position is beyond the deadzone
    /// </summary>
    /// <param name="targetPosition"></param>
    public void Move(Vector3 targetPosition)
    {
        float distanceToTarget = Vector3.Distance(targetPosition, transform.position);
        
        // Distance to target is not beyond deadzone
        if (distanceToTarget < m_deadzone) {
            return;
        }

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        // Speed is affected by the distance to the target
        // The greater the distance the greater the speed
        // However, the speed must be capped at the current max speed
        m_currentSpeed = Mathf.Clamp(distanceToTarget * m_moveSpeed, -m_moveSpeed, m_moveSpeed);

        m_charController.Move(direction * m_currentSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Rotates the player model to face the target position
    /// </summary>
    /// <param name="targetPosition"></param>
    public void Rotate(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;
    }
}
