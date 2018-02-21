using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Floor buttons are triggered by the player or any other object that can rest on them
/// Buttons can be active (down) or inactive (up)
/// Buttons are tide to an "action" that they trigger when their state changes
/// </summary>
public class FloorButton : MonoBehaviour
{
    /// <summary>
    /// A reference to the gameobject that represent button's active state
    /// </summary>
    [SerializeField]
    GameObject m_activeButton;

    /// <summary>
    /// A reference to the gameobject that represent button's inactive state
    /// </summary>
    [SerializeField]
    GameObject m_inactiveButton;

    /// <summary>
    /// When true it remains active after being triggered ON
    /// </summary>
    [SerializeField]
    bool m_isSingleUse = false;

    /// <summary>
    /// True when the button is active
    /// Remains true if this is a single use button
    /// </summary>
    bool m_isActive = false;

    /// <summary>
    /// Defaults the button to inactive
    /// </summary>
    void Awake()
    {
        if(m_activeButton == null || m_inactiveButton == null) {
            Debug.LogErrorFormat("Floor Button Error: Missing Component! " +
                "Active Button = {0}, Inactive Button = {1}",
                m_activeButton,
                m_inactiveButton
            );
        }

        Deactivate();
    }

    /// <summary>
    /// Sets the button to active state
    /// Single use buttons disable this component to avoid being interacted with again
    /// </summary>
    void Activate()
    {
        if (m_isSingleUse && m_isActive) {
            return;
        }

        m_isActive = true;
        m_inactiveButton.SetActive(false);
        m_activeButton.SetActive(true);
        
    }

    /// <summary>
    /// Sets the button to an inactive state
    /// </summary>
    void Deactivate()
    {
        // Don't need to do anything
        if (m_isSingleUse) {
            return;
        }

        m_isActive = false;
        m_inactiveButton.SetActive(true);
        m_activeButton.SetActive(false);
    }

    /// <summary>
    /// Triggers the button to activate
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Activate();
        }
    }

    /// <summary>
    /// Triggers the button to deactivate
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) {
            Deactivate();
        }
    }
}
