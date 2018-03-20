using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
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
    /// True while the button is pressed
    /// </summary>
    bool m_isPressed = false;
    public bool IsPressed
    {
        get { return m_isPressed; }
        set {
            m_isPressed = value;
            if(OnStateChange != null) {
                OnStateChange(m_isPressed);
            }
        }
    }

    /// <summary>
    /// Notifies listener of a change of state for this button
    /// </summary>
    /// <param name="state"></param>
    public delegate void StateChangeEvent(bool state);
    public event StateChangeEvent OnStateChange;
    
    /// <summary>
    /// Triggers a state change to show the button pressed
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
    }

    /// <summary>
    ///  Triggers a state change to show the button released
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
    }

    /// <summary>
    /// Displays the button and updates the text to show a grab action option
    /// </summary>
    public void ShowGrabAction()
    {
        m_buttonText.text = "Grab";
        m_buttonImage.enabled = true;
    }

    /// <summary>
    /// Disables the button
    /// </summary>
    public void Hide()
    {
        m_buttonText.text = "";
        m_buttonImage.enabled = false;
    }
}
