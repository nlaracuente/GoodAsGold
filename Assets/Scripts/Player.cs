using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls player behavior
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    /// <summary>
    /// How fast the player moves
    /// </summary>
    [SerializeField]
    float moveSpeed = 3f;

    /// <summary>
    /// How fast the player rotates
    /// </summary>
    [SerializeField]
    float rotationSpeed = 3f;

    /// <summary>
    /// The angle at which the player can start moving
    /// </summary>
    [SerializeField]
    float angleDistance;

    /// <summary>
    /// A reference to the rigidbody component
    /// </summary>
    new Rigidbody rigidbody;

    /// <summary>
    /// A reference to the level camera to process movements/rotations based on its position
    /// </summary>
    LevelCamera levelCamera;

    /// <summary>
    /// Saves the player's input
    /// </summary>
    [SerializeField]
    Vector3 inputVector = Vector3.zero;

    /// <summary>
    /// Initialize
    /// </summary>
    void Awake ()
    {
        this.rigidbody = GetComponent<Rigidbody>();
	}

    /// <summary>
    /// Assigns references to external objects
    /// </summary>
    void Start()
    {
        this.levelCamera = FindObjectOfType<LevelCamera>();
    }

    /// <summary>
    /// Store player inputs to trigger rotation/movement
    /// </summary>
    void Update ()
    {
        this.inputVector = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0f,
            Input.GetAxisRaw("Vertical")
        );

        if(this.inputVector.magnitude > 1) {
            this.inputVector.Normalize();
        }
	}

    /// <summary>
    /// Performs rotations and movements
    /// </summary>
    void FixedUpdate()
    {
        // Vector3.zero means the player is not moving
        if (this.inputVector == Vector3.zero) {
            return;
        }

        // Translate input vector based on camera position
        Vector3 direction = this.levelCamera.MainCamera.transform.TransformDirection(this.inputVector);
        direction.y = 0f;
        
        Quaternion targetRotation = Quaternion.Lerp(
                this.rigidbody.rotation,
                Quaternion.LookRotation(direction, Vector3.up),
                this.rotationSpeed * Time.fixedDeltaTime
        );
                
        this.rigidbody.MoveRotation(targetRotation);

        // Wait until rotation is at a certain angle before allowing movement
        // so that the player does not look awkward trying to moving while facing a different direction
        if(Quaternion.Angle(this.rigidbody.rotation, targetRotation) > this.angleDistance) {
            return;
        }

        Vector3 targetPosition = this.rigidbody.position + direction * this.moveSpeed * Time.fixedDeltaTime;
        this.rigidbody.MovePosition(targetPosition);
    }
}
