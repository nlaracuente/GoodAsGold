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
    float m_currentSpeed = 0f;

    /// <summary>
    /// How fast to push
    /// </summary>
    [SerializeField]
    float m_pushSpeed = 8f;

    /// <summary>
    /// How close to the destination vector when moving is allowed
    /// before considering the movement to be done
    /// </summary>
    [SerializeField]
    float m_destinationProximity = .01f;

    /// <summary>
    /// How fast to rotate
    /// </summary>
    [SerializeField]
    float m_rotationSpeed = 12f;

    /// <summary>
    /// How close to the angle of rotation must the transform be before considering the transform as complete
    /// </summary>
    [SerializeField]
    float m_rotationAngleProximity = .01f;

    /// <summary>
    /// A reference to the character controller
    /// </summary>
    CharacterController m_charController;

    /// <summary>
    /// A reference to the ui manager
    /// </summary>
    LevelUIManager m_uiManager;

    /// <summary>
    /// True while the action to push/pull is running
    /// </summary>
    bool m_isPushingOrPulling = false;

    /// <summary>
    /// True while the player is pushing/pulling an object
    /// </summary>
    public bool IsPushingOrPulling { get { return m_isPushingOrPulling; } }

    /// <summary>
    /// Based on the player's curse percentag, determined by the total coins collected vs maximum allowed,
    /// this value is updated to affect how fast the player can move/turn/push/pull/etc.
    /// </summary>
    float m_speedDecrement = 1f;
    public float SpeedDecrement { get { return m_speedDecrement; } set { m_speedDecrement = Mathf.Clamp01(value); } }

    /// <summary>
    /// Sets references to components
    /// </summary>
    void Awake()
    {
        m_charController = GetComponent<CharacterController>();
        m_uiManager = FindObjectOfType<LevelUIManager>();

        if(m_uiManager == null) {
            Debug.LogErrorFormat("PlayerMover Error: Missing Component! UI Manager = {0}", m_uiManager);
        }
    }

    /// <summary>
    /// Moves the player towards the targetPosition using the character controller
    /// Movement only occurs when the current position is beyond the deadzone
    /// </summary>
    /// <param name="inputVector"></param>
    public void Move(Vector3 inputVector)
    {
        float distanceToTarget = Vector3.Distance(inputVector, transform.position);
        
        // Distance to target is not beyond deadzone
        if (distanceToTarget < m_deadzone) {
            return;
        }

        // Direction to move the player towards
        Vector3 direction = inputVector - transform.position;
        direction.y = 0f;

        // Update speed to match current decrement
        m_currentSpeed = m_moveSpeed * m_speedDecrement;

        m_charController.SimpleMove(direction * m_currentSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Leprs the player's rotation to match the given position
    /// Returns true when the current rotation's angle is close to <see cref="m_rotationAngleProximity"/>
    /// </summary>
    /// <param name="inputVector"></param>
    public bool Rotate(Vector3 inputVector)
    {
        Vector3 direction = inputVector - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            m_rotationSpeed * m_speedDecrement * Time.deltaTime
        );

        return Quaternion.Angle(transform.rotation, targetRotation) <= m_rotationAngleProximity;
    }

    /// <summary>
    /// Handles moving the player and the object it is "pushing or pulling" towards the intended destination
    /// Hides/Shows the ui move arrows
    /// </summary>
    /// <param name="action"></param>
    /// <param name="playerDestination"></param>
    /// <param name="objectTransform"></param>
    /// <returns></returns>
    public IEnumerator PushPullRoutine(Vector3 playerDestination, Vector3 objectDestination, Transform objectTransform)
    {
        m_uiManager.HideArrows();
        m_isPushingOrPulling = true;

        // Decrease the push/pull speed to match curse status
        float moveSpeed = m_pushSpeed * m_speedDecrement;

        while (Vector3.Distance(playerDestination, transform.position) > m_destinationProximity) {

            Vector3 playerPos = Vector3.MoveTowards(transform.position, playerDestination, moveSpeed * Time.deltaTime);
            Vector3 objectPos = Vector3.MoveTowards(objectTransform.position, objectDestination, moveSpeed * Time.deltaTime);

            // Always move the object first to prevent the character controller from colliding with it and stopping
            objectTransform.transform.position = objectPos;
            transform.position = playerPos;
            yield return new WaitForEndOfFrame();
        }

        m_isPushingOrPulling = false;
        m_uiManager.ShowMoveArrow();
    }
}
