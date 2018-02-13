using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles receiving, interpreting, and storing user innput
/// </summary>
public class InputManager : MonoBehaviour
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
    }

    /// <summary>
    /// Stores player input
    /// </summary>
    void Update ()
    {
        UpdateInputVector();
        // Debug.LogFormat("Input Vector {0}", m_inputVector);
	}

    /// <summary>
    /// Resets the input vector to zero and updates if the player is touching the screen
    /// </summary>
    public void UpdateInputVector()
    {
        m_inputVector = Vector3.zero;

        // Only update it when the input is not disabled
        if (!m_inputDisabled && Input.touchCount == 1) {
            Vector2 screenPoint = Input.GetTouch(0).position;
            m_inputVector = OrtographicScreenPointToWorldSpace(screenPoint);
        }
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
}
