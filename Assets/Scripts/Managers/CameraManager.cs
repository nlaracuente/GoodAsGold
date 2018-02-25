using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages camera movements, zooms, and tracking
/// </summary>
public class CameraManager : MonoBehaviour
{
    /// <summary>
    /// The transform to track
    /// </summary>
    [SerializeField]
    Transform m_target;

    /// <summary>
    /// A refrence to the camera component
    /// </summary>
    [SerializeField]
    Camera m_camera;

    /// <summary>
    /// How fast to track the target
    /// </summary>
    [SerializeField]
    float m_trackingSpeed = 5f;

    /// <summary>
    /// How far away from the target to stay at
    /// </summary>
    [SerializeField]
    Vector3 m_trackingDistance = new Vector3(0f, 10f, 0f);

    /// <summary>
    /// Initialize references
    /// </summary>
    void Awake()
    {
        if(m_target == null) {
            m_target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

        if(m_camera == null) {
            m_camera = Camera.main;
        }
    }

    /// <summary>
    /// Smoothly tracks the target
    /// </summary>
    void LateUpdate()
    {
        Vector3 targetPosition = m_target.position + m_trackingDistance;
        transform.position = Vector3.Lerp(transform.position, targetPosition, m_trackingSpeed * Time.deltaTime);
    }
}
