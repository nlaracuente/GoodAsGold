using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A door can be trigger to open/close revealing or hiding the path behind it
/// </summary>
[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour
{
    /// <summary>
    /// True when the door is opened
    /// </summary>
    [SerializeField]
    bool m_isOpened = false;

    /// <summary>
    /// The name of the animator controller bool for openning/closing the door
    /// </summary>
    [SerializeField]
    string m_isOpenedTag = "IsOpened";

    /// <summary>
    /// A reference to the animator component
    /// </summary>
    Animator m_animator;

    /// <summary>
    /// A reference to any of the button subscriber attached to this door
    /// </summary>
    ButtonSubscriber m_buttonSubscriber;

	/// <summary>
    /// Set references
    /// Registeres to the button subscriber on activate/deactivate
    /// </summary>
	void Awake ()
    {
        m_buttonSubscriber = GetComponent<ButtonSubscriber>();
        m_animator = GetComponent<Animator>();

        if(m_buttonSubscriber == null || m_animator == null) {
            Debug.LogErrorFormat("Door Error: Missing Component! " +
                "Button Subscriber = {0}", 
                m_buttonSubscriber,
                m_animator
            );
        } else {
            m_buttonSubscriber.OnActivate += Open;
            m_buttonSubscriber.OnDeactivate += Close;
        }
	}

    /// <summary>
    /// Updates the animator controller to trigger door opened/close
    /// </summary>
    void Update()
    {
        m_animator.SetBool(m_isOpenedTag, m_isOpened);
    }

    /// <summary>
    /// Changes the status of the door to opened
    /// </summary>
    void Open()
    {
        m_isOpened = true;
    }

    /// <summary>
    /// Changes the status of the door to closed
    /// </summary>
    void Close()
    {
        m_isOpened = false;
    }
}
