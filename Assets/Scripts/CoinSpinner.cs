using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpinner : MonoBehaviour
{
    /// <summary>
    /// Rotation speed
    /// </summary>
    [SerializeField]
    float rotationSpeed = -120f;

    /// <summary>
    /// Since these are non-physic based collectables
    /// </summary>
    void Update()
    {
        // Rotate
        Vector3 targetRotation = new Vector3(
            0f,
            this.rotationSpeed * Time.deltaTime,
            0f
        );

        this.transform.Rotate(targetRotation);
    }
}
