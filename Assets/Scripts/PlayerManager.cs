﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player behavior talking to all corresponding component
/// to animate, move, and update player status
/// </summary>
[RequireComponent(typeof(PlayerMover), typeof(PlayerAnimator))]
public class PlayerManager : MonoBehaviour, IButtonInteractible
{
    /// <summary>
    /// A reference to the input manager object
    /// </summary>
    InputManager m_inputManager;

    /// <summary>
    /// A reference to the mesh renderer component
    /// </summary>
    [SerializeField]
    Renderer m_meshRenderer;

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
    /// The inder of the gold material as found in the materials array of the renderer component
    /// </summary>
    [SerializeField]
    int m_goldMaterialIndex = 1;

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
    /// The total coins the player is allowed to collected before losing
    /// </summary>
    [SerializeField, Tooltip("Total coins to collected before gameover")]
    float m_maxCoins = 10;

    /// <summary>
    /// How fast to change the material when cursed/cured
    /// </summary>
    [SerializeField, Tooltip("How fast to turn into gold/recover")]
    float m_alphaChangeDelay = 3f;

    /// <summary>
    /// Keeps track of how cursed the player is in percentage
    /// </summary>
    float m_currentCursePercent = 0f;
    public float CursePercent
    {
        get { return m_currentCursePercent; }
        set {
            m_currentCursePercent = value;
            float decrement = 1f;

            // Clamp percentage in quater increments
            if (m_currentCursePercent >= 25f && m_currentCursePercent < 50f) {
                decrement = .75f;
            } else if (m_currentCursePercent >= 50f && m_currentCursePercent < 75f) {
                decrement = .50f;
            } else if (m_currentCursePercent >= 75f && m_currentCursePercent < 100f) {
                decrement = .25f;
            } else if (m_currentCursePercent >= 100) {
                decrement = 0;
                // @TODO trigger game over
            }

            ChangeMaterialAlpha();
            m_playerMover.SpeedDecrement = decrement;

            if(m_currentCursePercent >= 100f) {
                StartCoroutine(DeathRoutine());
            }
        }
    }

    /// <summary>
    /// A counter for the total coins collected
    /// </summary>
    float m_coinsCollected = 0;
    public float CoinsCollected
    {
        get { return m_coinsCollected; }
        set {
            m_coinsCollected = value;
            this.CursePercent = m_coinsCollected * 100 / m_maxCoins;
        }
    }

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
    /// True while the coroutine to change the material's alpha is running
    /// </summary>
    bool m_isChangingAlpha = false;

    /// <summary>
    /// Flag used to trigger game over
    /// Set after the player's death sequence is completed
    /// </summary>
    bool m_isDead = false;
    public bool IsDead { get { return m_isDead; } }

    /// <summary>
    /// Sets all references
    /// </summary>
    void Awake()
    {
        m_inputManager = FindObjectOfType<InputManager>();
        m_playerMover = GetComponent<PlayerMover>();
        m_playerAnimator = GetComponent<PlayerAnimator>();

        if(m_meshRenderer == null) {
            m_meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        if (m_inputManager == null || m_playerMover == null || m_playerAnimator == null) {
            Debug.LogErrorFormat(
                "PlayerManager Error: A required component is null. " +
                "InputManager = {0}, MeshRenderer = {1}, PlayerMover = {2}, PlayerAnimator = {3}",
                m_playerMover,
                m_meshRenderer,
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
        // Level is not done loading
        // Wait unitl this action is completed
        if (m_currentCursePercent == 100f || !GameManager.instance.HasLevelLoaded || GameManager.instance.IsGameOver || m_playerMover.IsPushingOrPulling) {
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
        Vector3 inputVector = m_inputManager.InputVector;

        if(inputVector.magnitude > 1) {
            inputVector.Normalize();
        }

        // Process movement/rotations
        if (inputVector != Vector3.zero) {

            // Face direction before moving 
            if (m_playerMover.Rotate(inputVector)) {
                m_playerMover.Move(inputVector);
            }

            // Because the speed decrement clamps between 0 and 1
            // and it represents how fast the player can move we use it to update the animator
            moveSpeed = m_playerMover.SpeedDecrement;
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
            Collider collider = other.GetComponent<Collider>();
            Transform objectParent = other.transform.parent;

            // Avoid collision with the player, the object being moved, and any trigger collider
            if (!collider.isTrigger && other != gameObject && other != objectTransform.gameObject) {

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

    /// <summary>
    /// Triggers the animation of the player turning gold or being healed
    /// </summary>
    /// <param name="percent"></param>
    void ChangeMaterialAlpha()
    {
        if (!m_isChangingAlpha) {
            StartCoroutine(ChangeMaterialAlphaRoutine());
        }
    }

    /// <summary>
    /// Animates the player turning into gold or being restored
    /// The renderer has a "gold material" with the alpha set to 0
    /// We increase this number to slowly reveal the gold color
    /// </summary>
    /// <param name="percent"></param>
    /// <returns></returns>
    IEnumerator ChangeMaterialAlphaRoutine()
    {
        m_isChangingAlpha = true;

        // Alpha is clamp between 0 and 1 therefore we convert our 100 based number into a double
        float targetAlpha = m_currentCursePercent * .01f;
        Color materialColor = m_meshRenderer.materials[m_goldMaterialIndex].color;

        while (!Mathf.Approximately(materialColor.a, targetAlpha)) {
            materialColor.a = Mathf.Lerp(
                materialColor.a,
                targetAlpha,
                m_alphaChangeDelay * Time.deltaTime
            );
            
            m_meshRenderer.materials[m_goldMaterialIndex].color = materialColor;

            // Curse percent may have changed
            targetAlpha = m_currentCursePercent * .01f;
            yield return new WaitForEndOfFrame();
        }

        m_isChangingAlpha = false;
    }

    /// <summary>
    /// Triggers the death animations
    /// Waits for the animation to complete before marking the player as dead
    /// </summary>
    /// <returns></returns>
    IEnumerator DeathRoutine()
    {
        m_playerAnimator.TriggerDeath();

        // Wait for the animation to trigger
        yield return new WaitForSeconds(1f);

        while (!m_playerAnimator.IsDeathAnimationCompleted()) {
            yield return null;
        }

        m_isDead = true;
    }
}
