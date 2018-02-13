using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player behavior talking to all corresponding component
/// to animate, move, and update player status
/// </summary>
[RequireComponent(typeof(PlayerMover))]
public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// A reference to the player mover component
    /// </summary>
    PlayerMover m_playerMover;

    /// <summary>
    /// A reference to the input manager object
    /// </summary>
    InputManager m_inputManager;

    /// <summary>
    /// Sets all references
    /// </summary>
    void Awake()
    {
        m_playerMover = GetComponent<PlayerMover>();
        m_inputManager = FindObjectOfType<InputManager>();

        if(m_playerMover == null || m_inputManager == null) {
            Debug.LogErrorFormat(
                "PlayerManager Error: A required component is null. PlayerMover{0}, InputManager{1}",
                m_playerMover,
                m_inputManager
            );
        }
    }

    /// <summary>
    /// Gets an updated player input vector and performs movement/rotatations 
    /// as long as the input vector is not zero
    /// </summary>
    void Update()
    {
        m_inputManager.UpdateInputVector();

        if (m_inputManager.InputVector != Vector3.zero) {            
            Vector3 targetPosition = m_inputManager.InputVector;
            m_playerMover.Move(targetPosition);
            m_playerMover.Rotate(targetPosition);
        }
    }
}
