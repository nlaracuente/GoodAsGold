﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls player behavior
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(MeshRenderer), typeof(Material))]
public class Player : MonoBehaviour, IMoveable
{
    /// <summary>
    /// The fasted the players moves when fully healed
    /// </summary>
    [SerializeField]
    float maxSpeed = 6f;

    /// <summary>
    /// The current speed the player moves at
    /// Defaults to max
    /// </summary>
    float moveSpeed;

    /// <summary>
    /// The fastes the player rotates
    /// </summary>
    [SerializeField]
    float maxRotationSpeed = 10f;

    /// <summary>
    /// The current speed the player rotates at
    /// Defaults to max
    /// </summary>
    float rotationSpeed;

    /// <summary>
    /// The angle at which the player can start moving
    /// </summary>
    [SerializeField]
    float angleDistance = 0.01f;

    /// <summary>
    /// Total pickups the player has to collect 
    /// </summary>
    [SerializeField]
    int maxPickups = 5;

    /// <summary>
    /// A reference to the where the statue are on
    /// </summary>
    [SerializeField]
    LayerMask statueLayer;

    /// <summary>
    /// The raycast distance from the player to where they can interact with an object
    /// </summary>
    [SerializeField]
    float rayDistance = 1f;

    /// <summary>
    /// How much to reduce the player's movement speed
    /// When they collect a cursed pickup
    /// </summary>
    float speedDamper;

    /// <summary>
    /// How much to reduce the player's rotational speed
    /// When they collect a cursed pickup
    /// </summary>
    float rotationDamper;

    /// <summary>
    /// A reference to the rigidbody component
    /// </summary>
    new Rigidbody rigidbody;

    /// <summary>
    /// A reference to the Mesh Renderer component
    /// </summary>
    new MeshRenderer renderer;

    /// <summary>
    /// A reference to the level camera to process movements/rotations based on its position
    /// </summary>
    LevelCamera levelCamera;

    /// <summary>
    /// Saves the player's input
    /// </summary>
    Vector3 inputVector = Vector3.zero;

    /// <summary>
    /// Total pickups the player has collected
    /// </summary>
    [SerializeField]
    int pickups;

    /// <summary>
    /// How fast to change color
    /// </summary>
    [SerializeField]
    float colorChangeDelay = 1.5f;

    /// <summary>
    /// Array of color to change the player into as the curse progresses
    /// Each index reprents a "state" based on total pickups collected
    /// </summary>
    [SerializeField]
    Color[] cursedColors;

    /// <summary>
    /// Returns true while the player is holding the "action" button
    /// which triggers pulling/pushing
    /// </summary>
    public bool IsActionButtonHeld
    {
        get { return Input.GetButton("Jump"); }
    }

    /// <summary>
    /// True while the player is pulling
    /// </summary>
    bool isInteractingWithStatue = false;

    /// <summary>
    /// The current statue the player is interacting with
    /// </summary>
    Statue statue;

    /// <summary>
    /// Initialize
    /// </summary>
    void Awake ()
    {
        this.rigidbody      = GetComponent<Rigidbody>();
        this.renderer       = GetComponent<MeshRenderer>();
        this.moveSpeed      = this.maxSpeed;
        this.rotationSpeed  = this.maxRotationSpeed;
        this.speedDamper    = this.maxSpeed / this.maxPickups;
        this.rotationDamper = this.maxRotationSpeed / this.maxPickups;
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
    /// Handles player pull/push action
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

        bool isActionButton = Input.GetButton("Jump");

        // Currently not enaged
        // Trigger animation to engage
        if(isActionButton && !this.isInteractingWithStatue) {

            this.statue = this.GetStatueInfront();

            // Player is facing a statue, let's grab it
            if (this.statue != null) {
                this.isInteractingWithStatue = true;
                this.statue.transform.SetParent(this.transform);
                string dirName = this.statue.GetInteractionDirectionName(this.rigidbody);

                if (dirName == "forward" || dirName == "back") {
                    this.rigidbody.constraints = ~RigidbodyConstraints.FreezePositionZ;
                } else {
                    this.rigidbody.constraints = ~RigidbodyConstraints.FreezePositionX;
                }
            }
        }

        // No longer grabing on to the statue
        if (!isActionButton) {
            if(this.statue != null) {
                this.statue.transform.SetParent(null);
                this.isInteractingWithStatue = false;
                this.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }

    /// <summary>
    /// Returns the statue that is directly infront of the player
    /// </summary>
    /// <returns></returns>
    Statue GetStatueInfront()
    {
        Statue statue = null;
        Vector3 origin = this.rigidbody.position;
        Vector3 direction = this.transform.forward;

        Debug.DrawLine(origin, origin + direction * this.rayDistance, Color.blue);
        Ray ray = new Ray(origin, direction);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, this.rayDistance, this.statueLayer)) {
            statue = hit.collider.GetComponent<Statue>();
        }

        return statue;
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

        if (this.isInteractingWithStatue) {
            this.InteractWithStatue();
        } else {
            this.RotateAndMove();
        }
    }

    /// <summary>
    /// Handles rotating and moving the player
    /// This is when the player is not pushing or pulling a statue
    /// </summary>
    void RotateAndMove()
    {
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
        if (Quaternion.Angle(this.rigidbody.rotation, targetRotation) > this.angleDistance) {
            return;
        }

        Vector3 targetPosition = this.rigidbody.position + direction * this.moveSpeed * Time.fixedDeltaTime;
        this.rigidbody.MovePosition(targetPosition);
    }

    /// <summary>
    /// Handles moving the player and the statue the player is currently interacting with
    /// </summary>
    void InteractWithStatue()
    {
        Vector3 direction = this.levelCamera.MainCamera.transform.TransformDirection(this.inputVector);
        direction.y = 0f;

        Vector3 targetPosition = this.rigidbody.position + direction * this.moveSpeed * Time.fixedDeltaTime;
        this.rigidbody.MovePosition(targetPosition);
    }
    
    /// <summary>
    /// Triggers the curse's effect by slowing down the player's movement and rotation
    /// Changes the the mesh's color to simulate turning into gold
    /// </summary>
    public void CursedItemCollected()
    {
        this.pickups++;

        this.moveSpeed = Mathf.Max(
            0,
            Mathf.Min(this.maxSpeed, this.moveSpeed - this.speedDamper)
        );

        this.rotationSpeed = Mathf.Max(
            0,
            Mathf.Min(this.maxRotationSpeed, this.rotationSpeed - this.rotationDamper)
        );

        // Fire off the color changer
        StopCoroutine(this.ChangeColorPerCurse());
        StartCoroutine(this.ChangeColorPerCurse());
    }

    /// <summary>
    /// Gradually changes the color of the player to match their current curse state
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangeColorPerCurse()
    {
        // Default to current
        Color newColor = this.renderer.material.color;

        // Update based on curse percentage
        float cursedPercent = ((float)this.pickups / (float)this.maxPickups) * 100;

        if (cursedPercent >= 25 && cursedPercent < 50) {
            newColor = this.cursedColors[0];
        } else if (cursedPercent >= 50 && cursedPercent < 75) {
            newColor = this.cursedColors[1];
        } else if (cursedPercent >= 75 & cursedPercent < 100) {
            newColor = this.cursedColors[2];
        } else if (cursedPercent >= 100) {
            newColor = this.cursedColors[3];
        }

        while (this.renderer.material.color != newColor) {
            this.renderer.material.color = Color.Lerp(
            this.renderer.material.color,
                newColor,
                this.colorChangeDelay * Time.deltaTime
            );
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Removes the effects of being cursed
    /// Resets speed/rotation back to max
    /// </summary>
    void LiftCurse()
    {
        this.pickups = 0;
        this.moveSpeed = this.maxSpeed;
        this.rotationSpeed = this.maxRotationSpeed;
    }
}