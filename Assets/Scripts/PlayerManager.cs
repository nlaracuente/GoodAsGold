using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player behavior talking to all corresponding component
/// to animate, move, and update player status
/// </summary>
[RequireComponent(typeof(PlayerMover), typeof(PlayerAnimator))]
public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// A reference to the input manager object
    /// </summary>
    InputManager m_inputManager;

    /// <summary>
    /// A reference to the player mover component
    /// </summary>
    PlayerMover m_playerMover;

    /// <summary>
    /// A reference to the player animator component
    /// </summary>
    PlayerAnimator m_playerAnimator;   

    /// <summary>
    /// Sets all references
    /// </summary>
    void Awake()
    {
        m_inputManager = FindObjectOfType<InputManager>();
        m_playerMover = GetComponent<PlayerMover>();
        m_playerAnimator = GetComponent<PlayerAnimator>();

        if (m_inputManager == null || m_playerMover == null || m_playerAnimator == null) {
            Debug.LogErrorFormat(
                "PlayerManager Error: A required component is null. " +
                "InputManager = {0}, PlayerMover = {1}, PlayerAnimator = {2}",
                m_playerMover,
                m_inputManager,
                m_playerAnimator
            );
        }
    }

    /// <summary>
    /// Gets an updated player input vector and performs movement/rotations 
    /// as long as the input vector is not zero.
    /// Animation is updated to match the current movements/speed
    /// </summary>
    void Update()
    {
        float moveSpeed = 0;
        m_inputManager.UpdateInputVector();

        // Process movement/rotations
        if (m_inputManager.InputVector != Vector3.zero) {            
            Vector3 targetPosition = m_inputManager.InputVector;
            m_playerMover.Move(targetPosition);
            m_playerMover.Rotate(targetPosition);
            moveSpeed = m_playerMover.CurrentSpeed;
        }

        m_playerAnimator.UpdateMoveSpeed(moveSpeed);
    }
}
