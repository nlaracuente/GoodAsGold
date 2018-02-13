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
    /// A reference to the character controller
    /// </summary>
    CharacterController m_controller;

    /// <summary>
    /// A reference to the input manager object
    /// </summary>
    InputManager m_inputManager;

    /// <summary>
    /// Sets references to components
    /// </summary>
    void Awake()
    {
        m_controller = GetComponent<CharacterController>();
        m_inputManager = FindObjectOfType<InputManager>();
    }

    /// <summary>
    /// Triggers movement when input vector is not zero
    /// The input vector represents the point in worldspace to move to
    /// </summary>
	void Update ()
    {
        if (m_inputManager != null && m_inputManager.InputVector != Vector3.zero) {
            Move(m_inputManager.InputVector);
        }
	}

    /// <summary>
    /// Moves the player towards the targetPosition using the character controller
    /// Movement only occurs when the current position is beyond the deadzone
    /// </summary>
    /// <param name="targetPosition"></param>
    void Move(Vector3 targetPosition)
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
        float speed = Mathf.Clamp(distanceToTarget * m_moveSpeed, -m_moveSpeed, m_moveSpeed);

        m_controller.Move(direction * speed * Time.deltaTime);
    }
}
