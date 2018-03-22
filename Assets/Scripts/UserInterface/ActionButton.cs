using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerDownHandler
{
    /// <summary>
    /// These states represent the action the button will invoke when pressed
    /// </summary>
    enum State
    {
        None,
        Grab,
        Release,
    }

    /// <summary>
    /// Current state the button is in
    /// </summary>
    State m_state;
    
    /// <summary>
    /// A reference to the button's text
    /// </summary>
    [SerializeField]
    Text m_buttonText;

    /// <summary>
    /// A reference to the button's image
    /// </summary>
    [SerializeField]
    Image m_buttonImage;

    /// <summary>
    /// A reference to the moveable object that triggered this button to be enabled
    /// </summary>
    Moveable m_moveable;
    public Moveable MoveableObject { get { return m_moveable; } set { m_moveable = value; } }

    public static ActionButton instance;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    ///  Triggers a state change to show the button released
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if(m_moveable == null) {
            return;
        }

        switch (m_state) {
            case State.Grab:
                m_moveable.OnClick();
                ShowReleaseAction(m_moveable);
                break;
            case State.Release:
                m_moveable.OnLoseFocus();
                HideButton();
                break;
        }
    }

    /// <summary>
    /// Displays the button and updates the text to show a grab action option
    /// </summary>
    public void ShowGrabAction(Moveable moveable)
    {
        m_state = State.Grab;
        m_moveable = moveable;
        m_buttonText.text = "Grab";
        m_buttonImage.enabled = true;
    }

    public void ShowReleaseAction(Moveable moveable)
    {
        m_state = State.Release;
        m_moveable = moveable;
        m_buttonText.text = "Release";
        m_buttonImage.enabled = true;
    }

    /// <summary>
    /// Disables the button
    /// </summary>
    public void HideButton()
    {
        m_moveable = null;
        m_state = State.None;
        m_buttonText.text = "";
        m_buttonImage.enabled = false;
    }
}
