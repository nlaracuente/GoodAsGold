using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main camera in a level 
/// Has a focul point and can be rotate -90 or 90 degress
/// </summary>
public class LevelCamera : MonoBehaviour
{
    /// <summary>
    /// A reference to the camera component
    /// </summary>
    [SerializeField]
    Camera mainCamera;

    /// <summary>
    /// Main Level Camera
    /// </summary>
    public Camera MainCamera
    {
        get { return this.mainCamera; }
    }

    /// <summary>
    /// Where to look at and pivot around
    /// </summary>
    [SerializeField]
    Transform targertTransform;

    /// <summary>
    /// Angles in degree to rotate when a direciton is triggered 
    /// </summary>
    [SerializeField]
    float rotationAngle = 90f;

    /// <summary>
    /// How fast to rotate around the target
    /// </summary>
    [SerializeField]
    float rotationSpeed = 10f;

    /// <summary>
    /// The angle to rotate towards
    /// </summary>
    [SerializeField]
    float targetAngle = 0f;

    /// <summary>
    /// True while the camera is still rotating
    /// </summary>
    bool isRotating = false;
	
	// Update is called once per frame
	void Update ()
    {
        float angle = 0;

        // Left
        if (Input.GetKey(KeyCode.Q)) {
            angle = -this.rotationAngle;
    
        // Right
        } else if (Input.GetKey(KeyCode.E)) {
            angle = this.rotationAngle;
        }

        // Update rotation
        if(!this.isRotating && angle != 0) {
            this.targetAngle = angle;
            StartCoroutine(this.SmoothRotate());
        }        
    }

    /// <summary>
    /// Continues to rotate towards the target rotation until it reaches 
    /// a treshold to then snap into place
    /// </summary>
    /// <returns></returns>
    IEnumerator SmoothRotate()
    {
        this.isRotating = true;

        while (this.targetAngle != 0) {
            float speed = this.targetAngle > 0f ? -this.rotationSpeed : this.rotationSpeed;
            this.transform.RotateAround(this.targertTransform.position, Vector3.up, speed);
            this.targetAngle += speed;
            yield return new WaitForEndOfFrame();
        }
        
        this.isRotating = false;
    }
}
