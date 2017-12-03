using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls player behavior
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour, IMoveable
{
    #region Variables
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
    [SerializeField]
    new MeshRenderer renderer;

    /// <summary>
    /// A reference to the animation controller component
    /// </summary>
    [SerializeField]
    Animator animator;

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
    /// A reference to the player camera which is always facing the players
    /// </summary>
    [SerializeField]
    Camera playerCamera;

    /// <summary>
    /// A reference to the level menu
    /// </summary>
    LevelMenu menu;

    /// <summary>
    /// The current statue the player is interacting with
    /// </summary>
    Statue statue;

    /// <summary>
    /// True while the player is leaning on a statue 
    /// This includes pulling/pushing
    /// </summary>
    bool IsInteractingWithStatue
    {
        get { return this.statue != null; }
    }

    /// <summary>
    /// Returns the the current curse percent the player is at
    /// </summary>
    public float CursePercent
    {
        get {
            return ((float)this.pickups / (float)this.maxPickups) * 100;
        }
    }

    /// <summary>
    /// Returns true once the player has reached 100 or more of the curse
    /// </summary>
    public bool IsDead
    {
        get { return this.pickups >= this.maxPickups; }
    }

    /// <summary>
    /// True when the player is disabled
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// True while an action is playing and waiting for the 
    /// animation to notify it is done
    /// </summary>
    bool isActionCompleted = true;
    public bool IsActionCompleted
    {
        get { return this.isActionCompleted; }
        set { this.isActionCompleted = value; }
    }

    /// <summary>
    /// True when the animations are within the frames
    /// where it makes sense to move the player/statue
    /// </summary>
    public bool DoTheAction { get; set; }
    #endregion

    #region Unity Methods
    /// <summary>
    /// Initialize
    /// </summary>
    void Awake ()
    {
        this.rigidbody      = GetComponent<Rigidbody>();
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
        this.menu = FindObjectOfType<LevelMenu>();
        this.levelCamera = FindObjectOfType<LevelCamera>();
    }

    /// <summary>
    /// Store player inputs to trigger rotation/movement
    /// Handles player pull/push action
    /// </summary>
    void Update ()
    {
        // @TODO: replace this with a game manager to determine when to disable the player
        if (this.IsDisabled || this.menu.isMenuOpened) {
            this.inputVector = Vector3.zero;
            return;
        }

        // Player has died, trigger death sequence
        if (this.IsDead) {
            this.PlayerDeath();
        }

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
        if(isActionButton && !this.IsInteractingWithStatue) {

            this.statue = this.GetStatueInfront();

            // Player is facing a statue, let's grab it
            if (this.statue != null) {
                this.statue.PlayerEngaged();
                string dirName = this.statue.InteractedFrom;

                if (dirName == "forward" || dirName == "back") {
                    this.rigidbody.constraints = ~RigidbodyConstraints.FreezePositionZ;
                } else {
                    this.rigidbody.constraints = ~RigidbodyConstraints.FreezePositionX;
                }
                
                this.OnPlayerLean();
            }
        }

        // No longer grabing the statue
        if (!isActionButton && this.IsInteractingWithStatue) {
            this.OnPlayerLeanCancelled();
        }

        // Updates animations
        this.UpdateAnimator();
    }

    /// <summary>
    /// Performs rotations and movements
    /// </summary>
    void FixedUpdate()
    {
        // Vector3.zero means the player is not moving
        // Also, don't do anything while disabled
        if (this.inputVector == Vector3.zero || this.IsDisabled) {
            return;
        }

        if (this.IsInteractingWithStatue) {
            this.InteractWithStatue();
        } else {
            this.RotateAndMove();
        }
    }
    #endregion

    /// <summary>
    /// Returns the statue that is directly infront of the player
    /// </summary>
    /// <returns></returns>
    Statue GetStatueInfront()
    {
        Statue statue = null;
        Vector3 origin = this.rigidbody.position;
        Vector3 direction = this.transform.forward;

        Debug.DrawLine(origin, origin + direction * this.rayDistance, Color.green, .25f);
        Ray ray = new Ray(origin, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, this.rayDistance, this.statueLayer)) {
            statue = hit.collider.GetComponent<Statue>();
        }

        return statue;
    }

    /// <summary>
    /// Updates animator variables to trigger corresponding animations
    /// </summary>
    void UpdateAnimator(string triggerName = "")
    {
        // Don't have a reference to the animator
        if (this.animator == null) {
            return;
        }

        // Set of a trigger
        if (triggerName != "") {
            this.animator.SetTrigger(triggerName);
        }

        // Movement Blend
        if (this.inputVector == Vector3.zero) {
            this.animator.SetFloat("MoveSpeed", 0f);
        } else {
            // We want a number between 0 and 1 for the animation blend
            this.animator.SetFloat("MoveSpeed", this.moveSpeed / this.maxSpeed);
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
    /// When the player moves towards the statue we move the player and allow the physics engine to push the statue
    /// When the player is moving away from then we move the statue and let it push the player
    /// </summary>
    void InteractWithStatue()
    {
        // Wait until the current push/pull action is completed
        if (!this.IsActionCompleted) {
            return;
        }

        Vector3 direction = this.levelCamera.MainCamera.transform.TransformDirection(this.inputVector);
        direction.y = 0f;

        // Ignore Axis based on where the player intected with the block
        string dirName = this.statue.InteractedFrom;
        if(dirName == "forward" || dirName == "back") {
            direction.x = 0f;
        } else {
            direction.z = 0f;
        }

        // True when is moving in the opposite direction
        bool isPulling = false;

        // Pulling up
        if(dirName == "forward" && direction.z > 0) {
            isPulling = true;
        }

        // Pulling Left
        if (dirName == "left" && direction.x < 0) {
            isPulling = true;
        }

        // Pulling Down
        if (dirName == "back" && direction.z < 0) {
            isPulling = true;
        }

        // Pulling Right
        if (dirName == "right" && direction.x > 0) {
            isPulling = true;
        }

        this.StartCoroutine(this.PushOrPullAction(isPulling, direction));
    }

    /// <summary>
    /// While the push or pull animation is playing
    /// Continue to move the player and statue
    /// </summary>
    /// <param name="isPulling"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    IEnumerator PushOrPullAction(bool isPulling, Vector3 direction)
    {
        this.IsDisabled = true;
        this.IsActionCompleted = false;
        AnimatorStateInfo animationInfo = this.animator.GetCurrentAnimatorStateInfo(0);

        // Update Action
        if (isPulling) {
            this.UpdateAnimator("Pull");
        } else {
            this.UpdateAnimator("Push");
        }

        // Wait for the animation to hit the sweet spot to let us know to move
        while (!this.DoTheAction) {
            yield return new WaitForEndOfFrame();
        }

        // Turn it off for future use
        this.DoTheAction = false;

        while (!this.IsActionCompleted && !this.IsDead) {
            // Statue pushes the player
            if (isPulling) {
                this.statue.MoveTowardsDirection(direction, this.moveSpeed);

            // Player pushes the statue
            } else {
                Vector3 targetPosition = this.rigidbody.position + direction * this.moveSpeed * Time.fixedDeltaTime;
                this.rigidbody.MovePosition(targetPosition);
            }

            // Ensure physics calculations are processes
            yield return new WaitForFixedUpdate();
        }

        // Wait for the animator to start playing the animation
        yield return new WaitForEndOfFrame();

        this.IsDisabled = false;
    }

    /// <summary>
    /// Triggers player leaning on statue to push or pull
    /// </summary>
    void OnPlayerLean()
    {
        // Rotate the player to fully face the direction they interacted from
        Vector3 direction = this.levelCamera.MainCamera.transform.TransformDirection(this.inputVector);
        direction.y = 0f;

        if(direction != Vector3.zero) {
            this.rigidbody.MoveRotation(Quaternion.LookRotation(direction, Vector3.up));
        }

        this.inputVector = Vector3.zero;
        this.UpdateAnimator("Lean");
        this.StartCoroutine(this.WaitForAnimationEnd("PlayerLeanAnimation"));
    }
    
    /// <summary>
    /// Triggers player leaning off statue and returning to idle
    /// </summary>
    void OnPlayerLeanCancelled()
    {
        this.inputVector = Vector3.zero;
        this.UpdateAnimator("StopLeaning");
        this.StartCoroutine(this.WaitForAnimationEnd("PlayerStopsLeaningAnimation"));

        this.statue.PlayerDisingaged();
        this.statue = null;
        this.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    /// <summary>
    /// Triggers the curse's effect by slowing down the player's movement and rotation
    /// Changes the the mesh's color to simulate turning into gold
    /// </summary>
    public void CursedItemCollected()
    {
        this.pickups++;

        // Reduce Movement Speed
        this.moveSpeed = Mathf.Max(
            0,
            Mathf.Min(this.maxSpeed, this.moveSpeed - this.speedDamper)
        );

        // Reduce Rotation Speed
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
        float cursedPercent = this.CursePercent;

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
    /// Disables the player while waiting for the animation to finish playing
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForAnimationEnd(string animationName)
    {
        this.IsDisabled = true;
        AnimatorStateInfo animationInfo = this.animator.GetCurrentAnimatorStateInfo(0);

        // When the name changes then we are done playing the animation
        while (animationInfo.IsName(animationName)) {
            yield return new WaitForEndOfFrame();
        }
        
        this.IsDisabled = false;
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

    /// <summary>
    /// Disable the level camera to enable the player camera
    /// Trigger the death animation
    /// Wait until the animation completes to display game over screen
    /// </summary>
    void PlayerDeath()
    {
        this.IsDisabled = true;
        Debug.Log("Death Sequence");
        this.levelCamera.DisableCamera = true;
        this.playerCamera.gameObject.SetActive(true);
        this.UpdateAnimator("Death");
    }

    /// <summary>
    /// Notified by the avatar once the animation for either death or win
    /// is done to open the corresponding meny
    /// </summary>
    public void WinOrDeathAnimationDone()
    {
        if (this.IsDead) {
            this.menu.GameOverMenu();
        } else {
            this.menu.GameWonMenu();
        }
    }
}
