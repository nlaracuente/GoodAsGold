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
    /// The spawn point for the move arrow ui that sits behind the player
    /// </summary>
    [SerializeField]
    Transform m_moveArrowSpawnPoint;

    /// <summary>
    /// The world space position to spawn the move arrow ui that sits behind the player
    /// </summary>
    public Vector3 MoveArrowSpawnPoint
    {
        get {
            // Default to the player's posistion so that the arrow is at least
            // close to where it is supposed to be at
            Vector3 spawnPoint = transform.position;
            if(m_moveArrowSpawnPoint != null) {
                spawnPoint = m_moveArrowSpawnPoint.position;
            }
            return spawnPoint;
        }
    }

    /// <summary>
    /// Returns the direction the player is facing
    /// </summary>
    public FacingDirection LookingAtDirection
    {
        get {
            return Utility.GetFacingDirection(transform);
        }
    }

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

        // Process movement/rotations
        if (m_inputManager.InputVector != Vector3.zero) {            
            Vector3 targetPosition = m_inputManager.InputVector;

            // Face direction before moving 
            if (m_playerMover.Rotate(targetPosition)) {
                m_playerMover.Move(targetPosition);
            }

            moveSpeed = m_playerMover.CurrentSpeed;
        }

        m_playerAnimator.UpdateMoveSpeed(moveSpeed);
    }

    /// <summary>
    /// Turns the player to look the given object
    /// </summary>
    /// <param name="targetPos"></param>
    public void LookAtObject(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
