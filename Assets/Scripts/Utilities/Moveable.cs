using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objects that are moveable by the player
/// These objects can be pushed or pulled
/// </summary>
public class Moveable : MonoBehaviour, IClickable, IButtonInteractible
{
    /// <summary>
    /// A reference to the UI Manager
    /// </summary>
    UIManager m_uiManager;

    /// <summary>
    /// A reference to the playe manager object
    /// </summary>
    PlayerManager m_playerManager;

    /// <summary>
    /// The transform for the parent object
    /// </summary>
    [SerializeField]
    Transform m_parentTransform;
    public Transform ParentTransform { get { return m_parentTransform; } }

    /// <summary>
    /// Set references
    /// </summary>
    void Awake()
    {
        m_uiManager = FindObjectOfType<UIManager>();
        m_playerManager = FindObjectOfType<PlayerManager>();

        if(m_uiManager == null || m_playerManager == null) {
            Debug.LogErrorFormat(
                "PlayerMoveable ERROR: Missing Component. UI Manager = {0}, Player Manager = {1}", 
                m_uiManager,
                m_playerManager
            );
        }
    }

    /// <summary>
    /// Based on the position the player is currently facing
    /// Triggers the uiManager to arrows behind the player and next to this object
    /// in front of the player binding their OnButtonPressed events with the this 
    /// object's OnMoveEvent
    /// </summary>
    public void OnClick()
    {
        // Disable movement
        InputManager.instance.DisableInput = true;

        // Make the player face this object
        m_playerManager.LookAtObject(transform.position);

        // The alignment vector to use when the player is to the left or right of this object
        Vector3 verticalAlignment = new Vector3(
            m_playerManager.transform.position.x,
            m_playerManager.transform.position.y,
            transform.position.z
        );

        // The alignment vector to use when the player is to the above or bellow this object
        Vector3 horizontalAlignment = new Vector3(
            transform.position.x,
            m_playerManager.transform.position.y,
            m_playerManager.transform.position.z
        );

        switch (m_playerManager.LookingAtDirection) {
            case FacingDirection.Up:
            case FacingDirection.Down:
                m_playerManager.transform.position = horizontalAlignment;
                m_uiManager.ShowVerticalMoveArrows();

                // Bind the clicks
                if(m_playerManager.LookingAtDirection == FacingDirection.Up) {
                    m_uiManager.OnDPadArrowOneClicked += Push;
                    m_uiManager.OnDPadArrowTwoClicked += Pull;
                } else {
                    m_uiManager.OnDPadArrowOneClicked += Pull;
                    m_uiManager.OnDPadArrowTwoClicked += Push;
                }
                break;

            // Player is on the right side of this object looking left
            case FacingDirection.Left:
            case FacingDirection.Right:
                m_playerManager.transform.position = verticalAlignment;
                m_uiManager.ShowHorizontalMoveArrows();

                // Bind the clicks
                if (m_playerManager.LookingAtDirection == FacingDirection.Left) {
                    m_uiManager.OnDPadArrowOneClicked += Push;
                    m_uiManager.OnDPadArrowTwoClicked += Pull;
                } else {
                    m_uiManager.OnDPadArrowOneClicked += Pull;
                    m_uiManager.OnDPadArrowTwoClicked += Push;
                }
                break;
        }

        // Trigger the lean animation
        m_playerManager.Lean = true;
    }

    /// <summary>
    /// Removes all bindings with the move arrows
    /// </summary>
    public void OnLoseFocus()
    {
        // Disable movement
        InputManager.instance.DisableInput = false;
        UIManager.instance.HideArrows();

        m_uiManager.OnDPadArrowOneClicked -= Push;
        m_uiManager.OnDPadArrowTwoClicked -= Push;

        m_uiManager.OnDPadArrowOneClicked -= Pull;
        m_uiManager.OnDPadArrowTwoClicked -= Pull;

        // Stop Leaning
        m_playerManager.Lean = false;
    }

    /// <summary>
    /// Returns true when the player is close enough and looking at this object
    /// to interact with it
    /// </summary>
    /// <returns></returns>
    public bool IsClickable()
    {
        bool isClickable = false;

        // Check if the player is close enough to the clickable item and looking at
        // If not, then ignore click, otherwise trigger click
        Vector3 origin = new Vector3(
            m_playerManager.transform.position.x,
            m_playerManager.transform.position.y + 1.5f,
            m_playerManager.transform.position.z
        );

        Ray ray = new Ray(origin, m_playerManager.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f)) {
            isClickable = (hit.collider.gameObject == gameObject);
        }

        return isClickable;
    }

    /// <summary>
    /// Triggers the player to push this object
    /// </summary>
    void Push()
    {
        m_playerManager.PushObject(this);
    }

    /// <summary>
    /// Triggers the player to pull this object
    /// </summary>
    void Pull()
    {
       m_playerManager.PullObject(this);
    }
}
