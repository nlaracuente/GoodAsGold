using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles updating the player animator controller to play animations
/// that match the player's current state
/// </summary>
public class PlayerAnimator : MonoBehaviour
{
    /// <summary>
    /// A reference to the animator controller component
    /// </summary>
    [SerializeField]
    Animator m_animator;

    /// <summary>
    /// The name of the movement blend
    /// </summary>
    [SerializeField]
    string m_movementBlendTag = "Movement";

    /// <summary>
    /// The name of the animator float for movement speed
    /// </summary>
    [Tooltip("Animator movement speed parameter name")]
    [SerializeField]
    string m_moveSpeedTag = "MoveSpeed";

    /// <summary>
    /// The name of the animator float for movement speed
    /// </summary>
    [Tooltip("Animator movement speed parameter name")]
    [SerializeField]
    string m_leanBoolTag = "IsLeaning";

    /// <summary>
    /// Sets component references
    /// </summary>
    void Awake()
    {
        // See if it exists in the current object
        if(m_animator == null) {
            m_animator = GetComponent<Animator>();
        }

        // Check child objects
        if (m_animator == null) {
            m_animator = GetComponentInChildren<Animator>();
        }

        // Still Missing? Error
        if (m_animator == null) {
           Debug.LogErrorFormat("PlayerAnimator Error: Missing Animator Component");
        }
    }

    /// <summary>
    /// Returns true when the current animation is part of the Movement animation blend
    /// </summary>
    /// <returns></returns>
    public bool IsInMovementBlend()
    {
        AnimatorStateInfo state = m_animator.GetCurrentAnimatorStateInfo(0);
        return state.IsName(m_movementBlendTag);
    }

    /// <summary>
    /// Sets the movement speed parameter in the animator controller
    /// to show the character idle, walking, or running
    /// </summary>
    /// <param name="speed"></param>
    public void UpdateMoveSpeed(float speed)
    {
        m_animator.SetFloat(m_moveSpeedTag, speed);
    }

    /// <summary>
    /// Sets the state of the isLeaning bool
    /// </summary>
    /// <param name="state"></param>
    public void SetLeaningBool(bool state)
    {
        m_animator.SetBool(m_leanBoolTag, state);
    }
}
