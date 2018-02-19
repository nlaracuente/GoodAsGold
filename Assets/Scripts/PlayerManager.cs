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
    /// How many unit to move to another tile
    /// </summary>
    [SerializeField]
    float m_oneTileUnits = 3f;

    /// <summary>
    /// True while the player is engaging with a moveable object and the animation to lean has been triggered
    /// </summary>
    bool m_isLeaning = false;

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
        // Wait unitl this action is completed
        if (m_playerMover.IsPushingOrPulling) {
            return;
        }

        if (m_inputManager.MoveableObject != null && !m_isLeaning) {
            m_isLeaning = true;            
        } else if (m_inputManager.MoveableObject == null && m_isLeaning) {
            m_isLeaning = false;
        } else if(m_playerAnimator.IsInMovementBlend()) {
            RotateAndMove();
        }

        m_playerAnimator.SetLeaningBool(m_isLeaning);
    }

    /// <summary>
    /// Handles applying rotation and movement based on player input
    /// </summary>
    void RotateAndMove()
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

    /// <summary>
    /// Triggers the routine to push an object in the direction the player is moving
    /// </summary>
    /// <param name="objectTransform"></param>
    public void PushObject(Transform objectTransform)
    {
        Vector3 directionVector = Utility.GetDirectionVectorByName(LookingAtDirection);
        Vector3 playerDestination = transform.position + (directionVector * m_oneTileUnits);
        Vector3 objectDestination = objectTransform.position + (directionVector * m_oneTileUnits);

        // Verify that the destination is available (where the object will be)
        if (IsPushPullDestinationAvailable(objectTransform.position, directionVector, objectTransform)) {
            m_playerAnimator.TriggerPushAction();
            StartCoroutine(m_playerMover.PushPullRoutine(playerDestination, objectDestination, objectTransform));
        }
    }

    /// <summary>
    /// Triggers the routine to push an object in the direction the player is moving
    /// </summary>
    /// <param name="objectTransform"></param>
    public void PullObject(Transform objectTransform)
    {
        Vector3 directionVector = Utility.GetDirectionVectorByName(LookingAtDirection);
        Vector3 playerDestination = transform.position - (directionVector * m_oneTileUnits);
        Vector3 objectDestination = objectTransform.position - (directionVector * m_oneTileUnits);

        if (IsPushPullDestinationAvailable(transform.position, -directionVector, objectTransform)) {
            m_playerAnimator.TriggerPullAction();
            StartCoroutine(m_playerMover.PushPullRoutine(playerDestination, objectDestination, objectTransform));
        }        
    }

    /// <summary>
    /// Cast a ray in the direction of the push/pull returning true when nothing blocking it
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    bool IsPushPullDestinationAvailable(Vector3 origin, Vector3 direction, Transform objectTransform)
    {
        bool isAvailable = true;

        // Raise the origin to avoid collision with the floor
        origin.y += .5f;

        Debug.DrawLine(origin, origin + direction * m_oneTileUnits, Color.red, .25f);
        Ray ray = new Ray(origin, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, m_oneTileUnits + .25f);

        foreach (RaycastHit hit in hits) {
            GameObject other = hit.collider.gameObject;
            Transform objectParent = other.transform.parent;

            // Avoid collision with the player and the object being moved
            if (other != gameObject && other != objectTransform.gameObject) {

                // It may be a child of the object we are moving, skip it if it is
                if(objectParent != null && objectParent.gameObject == objectTransform.gameObject) {
                    continue;
                }

                isAvailable = false;
                break;
            }
        }

        return isAvailable;
    }
}
