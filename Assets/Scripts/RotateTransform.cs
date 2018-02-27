using System.Collections;
using UnityEngine;

/// <summary>
/// Continuously rotates the transform this is attached to
/// </summary>
public class RotateTransform : MonoBehaviour
{
    /// <summary>
    /// How fast to rotate
    /// </summary>
    [SerializeField, Tooltip("How fast to rotate")]
    float m_rate = 90f;

    /// <summary>
    /// Triggers the rotation on start
    /// </summary>
    [SerializeField]
    bool m_rotate = true;

    /// <summary>
    /// Triggers the rotation routine
    /// </summary>
    void Start()
    {
        StartRotation();
    }

    /// <summary>
    /// Initiates the rotation routine
    /// </summary>
    public void StartRotation()
    {
        m_rotate = true;
        StartCoroutine("RotateRoutine");
    }

    /// <summary>
    /// Stops the rotation routine
    /// </summary>
    public void StopRotation()
    {
        m_rotate = false;
        StopCoroutine("RotateRoutine");
    }

    /// <summary>
    /// Continuously rotate this object at the given rate
    /// </summary>
    /// <returns></returns>
    IEnumerator RotateRoutine()
    {
        while (m_rotate) {
            transform.Rotate(Vector3.up * m_rate * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
