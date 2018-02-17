﻿using System;
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
    /// Sets the input vector to Vector.zero when true
    /// </summary>
    bool m_inputDisabled = false;
    public bool DisableInput { get { return m_inputDisabled; } set { m_inputDisabled = value; } }

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
    LevelUIManager m_uiManager;

    /// <summary>
    /// Finds the main camera if one was not given
    /// </summary>
    void Awake()
    {
        if(m_camera == null) {
            m_camera = Camera.main;
        }

        if(m_playerTransform == null) {
            m_playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        m_uiManager = FindObjectOfType<LevelUIManager>();
    }

    /// <summary>
    /// Resets the input vector to zero and updates if the player is touching the screen
    /// and their 
    /// </summary>
    void UpdateInputVector(Vector3 inputPosition)
    {
        m_inputVector = OrtographicScreenPointToWorldSpace(inputPosition);
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
    /// Sets the X,Z axis to 1 or -1 if the input is beyond those values
    /// </summary>
    void NormalizeInputVector()
    {
        m_inputVector.x = Mathf.Clamp(m_inputVector.x, -1, 1);
        m_inputVector.y = 0f;
        m_inputVector.z = Mathf.Clamp(m_inputVector.z, -1, 1);
        
        if(m_inputVector.magnitude > 1) {
            m_inputVector.Normalize();
        }
    }

    /// <summary>
    /// Gives priority to IInteractable objects before interpreting the requests an input vector update
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        m_clickable = GetClickableObjectAt(eventData.position);

        if(m_clickable != null && m_clickable.IsClickable()) {
            m_clickable.OnClick();
        } else {
            m_uiManager.HideArrows();
            UpdateInputVector(eventData.position);
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

        UpdateInputVector(eventData.position);
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
