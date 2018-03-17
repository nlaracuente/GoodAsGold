using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
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
}
