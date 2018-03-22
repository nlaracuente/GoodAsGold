using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles receiving, interpreting, and storing user input
/// </summary>
public class InputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    /// <summary>
    /// A reference to self
    /// </summary>
    public static InputManager instance;

    /// <summary>
    /// A reference to the main camera to use when converting screen positions to world space
    /// </summary>
    [SerializeField]
    Camera m_camera;

    /// <summary>
    /// A reference to the player's transform
    /// </summary>
    [SerializeField]
    Transform m_playerTransform;

    /// <summary>
    /// Holds the player's direction input vector 
    /// </summary>
    Vector3 m_inputVector = Vector3.zero;
    public Vector3 InputVector { get { return m_inputVector; } }

    /// <summary>
    /// A reference to the virtual hoystick
    /// </summary>
    VirtualJoystick m_joystick;

    /// <summary>
    /// Sets the input vector to Vector.zero when true
    /// </summary>
    bool m_inputDisabled = false;
    public bool DisableInput
    {
        get { return m_inputDisabled; }

        // Disables/Enables the joystick
        set {
            m_inputDisabled = value;
            m_joystick.ShowJoystick = !value; // invert to show/hide
        }
    }

    /// <summary>
    /// A reference to the last IClickable object the player clicked on
    /// </summary>
    IClickable m_clickable;
    public IClickable MoveableObject
    {
        get { return m_clickable; }
    }

    /// <summary>
    /// A reference to the UI Manager
    /// </summary>
    UIManager m_uiManager;

    /// <summary>
    /// Finds the main camera if one was not given
    /// </summary>
    void Awake()
    {
        instance = this;

        if(m_camera == null) {
            m_camera = Camera.main;
        }

        if(m_playerTransform == null) {
            m_playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        
        m_uiManager = FindObjectOfType<UIManager>();
        m_joystick = FindObjectOfType<VirtualJoystick>();
    }

    /// <summary>
    /// Updates the input vector to store the direction and distance from the player avatar
    /// the player is pressing on the screen which represent the direction to move
    /// Values is clamped between -1 and 1 to emulate Input.GetAxis()
    /// </summary>
    void SetInputVector(Vector3 screenPoint)
    {
        // No input detected
        if(screenPoint == Vector3.zero) {
            m_inputVector = screenPoint;
            return;
        }

        // Translate the input
        Vector3 worldSpace = OrtographicScreenPointToWorldSpace(screenPoint);

        // Get the direction of the input ignoring the y axis
        Vector3 direction = worldSpace - m_playerTransform.position;
        direction.y = 0f;

        m_inputVector = direction;
    }

    /// <summary>
    /// Converts screen point into world space for an ortographic camera
    /// The Camera.ScreenToWorldPoint() fails to properly calculate Z 
    /// This corrects the issue by using a Plane.Raycast at the player's position
    /// </summary>
    /// <param name="screenPoint"></param>
    /// <returns></returns>
    Vector3 OrtographicScreenPointToWorldSpace(Vector3 screenPoint)
    {
        Vector3 worldSpace = Vector3.zero;

        float distance = 0;
        Ray ray = m_camera.ScreenPointToRay(screenPoint);
        Plane plane = new Plane(Vector3.up, m_playerTransform.position);

        if (plane.Raycast(ray, out distance)) {
            worldSpace = ray.GetPoint(distance);
        }
        
        return worldSpace;
    }

    /// <summary>
    /// Gives priority to IInteractable objects before interpreting the requests an input vector update
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        IClickable clickable = GetClickableObjectAt(eventData.position);

        if(clickable != null && clickable.IsClickable()) {
            // Lose the reference to the previous clickable
            if (m_clickable != null && m_clickable != clickable) {
                m_clickable.OnLoseFocus();
                m_clickable = null;
            }

            m_clickable = clickable;
            m_clickable.OnClick();

        } else {
            // No longer referencing the clickable object
            if (m_clickable != null) {
                m_clickable.OnLoseFocus();
                m_clickable = null;
            }
            
            SetInputVector(eventData.position);
        }
    }

    /// <summary>
    /// Resets the input vector and unlinks clickable item
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        m_inputVector = Vector3.zero;
    }

    /// <summary>
    /// Updates the input vector when not interacting with a clickable object
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if(m_clickable != null) {
            return;
        }

        SetInputVector(eventData.position);
    }

    /// <summary>
    /// Returns the IClickable object located at screen point if one is found
    /// </summary>
    /// <param name="screenPoint"></param>
    /// <returns></returns>
    IClickable GetClickableObjectAt(Vector2 screenPoint)
    {
        IClickable clickable = null;

        Ray ray = m_camera.ScreenPointToRay(screenPoint);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            clickable = hit.collider.GetComponent<IClickable>();
        }

        return clickable;
    }
}
