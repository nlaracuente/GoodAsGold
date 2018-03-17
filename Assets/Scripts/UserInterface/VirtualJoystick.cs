using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Reads player input triggered by moving a virtual joystick or pressing any of the buttons on it
/// Limits the joystick's movement to be within the bounds of the background image
/// </summary>
// [ExecuteInEditMode]
public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    /// <summary>
    /// A reference to the RectTransform of the image that represents the background of the analogue stick
    /// </summary>
    [SerializeField]
    RectTransform m_background;

    /// <summary>
    /// A reference to the RectTransform of the image that represents the analogue stick 
    /// </summary>
    [SerializeField]
    RectTransform m_handle;

    /// <summary>
    /// How much room to leave between the edge of background and the handle
    /// </summary>
    [SerializeField, Range(0f, 1f), Tooltip("How far from the edge of the background the handle can move")]
    float m_padding = 1f;
    
    /// <summary>
    /// The current position of the virtual joystick
    /// </summary>
    Vector2 m_position;

    /// <summary>
    /// Stores the translation of the handle's position into an input request
    /// </summary>
    Vector3 m_inputVector;
    public Vector3 InputVector { get { return new Vector3(m_inputVector.x, 0f, m_inputVector.y); } }

    /// <summary>
    /// A reference to the action button instance
    /// </summary>
    ActionButton m_actionButton;
    public bool IsActionButtonPressed { get { return m_actionButton.IsPressed; } }
    
    /// <summary>
    /// Stores the current joystick position
    /// </summary>
    void Start()
    {
        m_actionButton = FindObjectOfType<ActionButton>();
        m_position = RectTransformUtility.WorldToScreenPoint(new Camera(), m_background.position);
        
        if(m_actionButton == null) {
            Debug.LogErrorFormat("Virtual Joystick Error: Missing Component! ActionButon = {0}", m_actionButton);
        }
    }

    /// <summary>
    /// Positions the handle based on where the touch was made 
    /// Updates the input vector to reflect the new position of the handle
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - m_position;

        m_inputVector = (direction.magnitude > m_background.sizeDelta.x / 2f) ?
                         direction.normalized : 
                         direction / (m_background.sizeDelta.x / 2f);

        m_handle.anchoredPosition = (m_inputVector * m_background.sizeDelta.x / 2f) * m_padding;
    }

    /// <summary>
    /// Checks if the click occurred on the stick while it is at rest to treat it as a "click"
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    /// <summary>
    /// Resets the input vector and the handle
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        m_inputVector = Vector2.zero;
        m_handle.anchoredPosition = Vector2.zero;
    }

    public void OnActionStateChange()
    {

    }
}
