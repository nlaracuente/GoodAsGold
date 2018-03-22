using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player behavior talking to all corresponding component
/// to animate, move, and update player status
/// </summary>
[RequireComponent(typeof(PlayerMover), typeof(PlayerAnimator))]
public class PlayerManager : MonoBehaviour, IButtonInteractible
{
    /// <summary>
    /// A reference to the input manager object
    /// </summary>
    InputManager m_inputManager;

    /// <summary>
    /// A reference to the mesh renderer component
    /// </summary>
    [SerializeField]
    Renderer m_meshRenderer;

    /// <summary>
    /// A reference to the player mover component
    /// </summary>
    PlayerMover m_playerMover;

    /// <summary>
    /// A reference to the player animator component
    /// </summary>
    PlayerAnimator m_playerAnimator;

    /// <summary>
    /// A reference to the virtual joystick object
    /// </summary>
    VirtualJoystick m_joystick;

    /// <summary>
    /// The spawn point for the move arrow ui that sits behind the player
    /// </summary>
    [SerializeField]
    Transform m_moveArrowSpawnPoint;

    /// <summary>
    /// The inder of the gold material as found in the materials array of the renderer component
    /// </summary>
    [SerializeField]
    int m_goldMaterialIndex = 1;

    /// <summary>
    /// How many unit to move to another tile
    /// </summary>
    [SerializeField]
    float m_oneTileUnits = 3f;

    /// <summary>
    /// True while the player is engaging with a moveable object and the animation to lean has been triggered
    /// </summary>
    bool m_isLeaning = false;
    public bool Lean { set { m_isLeaning = value; } }

    /// <summary>
    /// The total coins the player is allowed to collected before losing
    /// </summary>
    [SerializeField, Tooltip("Total coins to collected before gameover")]
    float m_maxCoins = 10;

    /// <summary>
    /// How fast to change the material when cursed/cured
    /// </summary>
    [SerializeField, Tooltip("How fast to turn into gold/recover")]
    float m_alphaChangeDelay = 3f;

    /// <summary>
    /// The LayerMask for detecting moveable objects with raycasting
    /// </summary>
    [SerializeField]
    LayerMask m_moveableLayerMask;

    /// <summary>
    /// Keeps track of how cursed the player is in percentage
    /// </summary>
    float m_currentCursePercent = 0f;
    public float CursePercent
    {
        get { return m_currentCursePercent; }
        set {
            m_currentCursePercent = value;
            float decrement = 1f;

            // Clamp percentage in quater increments
            if (m_currentCursePercent >= 25f && m_currentCursePercent < 50f) {
                decrement = .75f;
            } else if (m_currentCursePercent >= 50f && m_currentCursePercent < 75f) {
                decrement = .50f;
            } else if (m_currentCursePercent >= 75f && m_currentCursePercent < 100f) {
                decrement = .25f;
            } else if (m_currentCursePercent >= 100) {
                decrement = 0;
                // @TODO trigger game over
            }

            ChangeMaterialAlpha();
            m_playerMover.SpeedDecrement = decrement;

            if(m_currentCursePercent >= 100f) {
                StartCoroutine(DeathRoutine());
            }
        }
    }

    /// <summary>
    /// A counter for the total coins collected
    /// </summary>
    float m_coinsCollected = 0;
    public float CoinsCollected
    {
        get { return m_coinsCollected; }
        set {
            m_coinsCollected = value;
            this.CursePercent = m_coinsCollected * 100 / m_maxCoins;
        }
    }

    /// <summary>
    /// The world space position to spawn the move arrow ui that sits behind the player
    /// </summary>
    public Vector3 MoveArrowSpawnPoint
    {
        get {
            // Default to the player's posistion so that the arrow is at least
            // close to where it is supposed to be at
            Vector3 spawnPoint = transform.position;
            if(m_moveArrowSpawnPoint != null) {
                spawnPoint = m_moveArrowSpawnPoint.position;
            }
            return spawnPoint;
        }
    }

    /// <summary>
    /// Returns the direction the player is facing
    /// </summary>
    public FacingDirection LookingAtDirection
    {
        get {
            return Utility.GetFacingDirection(transform);
        }
    }

    /// <summary>
    /// True while the coroutine to change the material's alpha is running
    /// </summary>
    bool m_isChangingAlpha = false;

    /// <summary>
    /// Flag used to trigger game over
    /// Set after the player's death sequence is completed
    /// </summary>
    bool m_isDead = false;
    public bool IsDead { get { return m_isDead; } }

    /// <summary>
    /// Returns the GameObject for the tile the player is currently on
    /// </summary>
    public BaseTile PlayerTile
    {
        get {
            Vector3 position = new Vector3(
                Mathf.Floor(transform.position.x / GameManager.tileXSize),
                0f,
                Mathf.Floor(transform.position.z / GameManager.tileZSize)
            );

            return MapController.instance.GetTileAt(position);
        }
    }

    /// <summary>
    /// Sets all references
    /// </summary>
    void Awake()
    {
        m_inputManager = FindObjectOfType<InputManager>();
        m_playerMover = GetComponent<PlayerMover>();
        m_playerAnimator = GetComponent<PlayerAnimator>();
        m_joystick = FindObjectOfType<VirtualJoystick>();

        if (m_meshRenderer == null) {
            m_meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        if (m_inputManager == null || m_playerMover == null || m_playerAnimator == null) {
            Debug.LogErrorFormat(
                "PlayerManager Error: A required component is null. " +
                "InputManager = {0}, MeshRenderer = {1}, PlayerMover = {2}, PlayerAnimator = {3}",
                m_playerMover,
                m_meshRenderer,
                m_inputManager,
                m_playerAnimator
            );
        }
    }

    /// <summary>
    /// Gets an updated player input vector and performs movement/rotations 
    /// as long as the input vector is not zero.
    /// Animation is updated to match the current movements/speed
    /// </summary>
    void Update()
    {
        // Level is not done loading
        // Wait unitl this action is completed
        if (m_currentCursePercent == 100f || !GameManager.instance.HasLevelLoaded || GameManager.instance.IsGameOver || m_playerMover.IsPushingOrPulling) {
            return;
        }

        if(m_playerAnimator.IsInMovementBlend()) {
            RotateAndMove();
        }

        m_playerAnimator.SetLeaningBool(m_isLeaning);
    }

    /// <summary>
    /// Checks if the player is within range of a moveable object to trigger action button to display as such
    /// </summary>
    void LateUpdate()
    {
        // Already engaging a moveable object therefore we can skip the following
        if (m_isLeaning) {
            return;
        }

        Moveable block = GetMoveableObjectInfrontOfPlayer();

        if(block != null) {
            UIManager.instance.ShowGrabButton(block);
        } else {
            UIManager.instance.HideGrabButton();
        }
    }

    /// <summary>
    /// Cast a ray in the direciton the player is facing to see if there's a moveable object within reach
    /// returning said object is one is found
    /// </summary>
    /// <returns></returns>
    Moveable GetMoveableObjectInfrontOfPlayer()
    {
        Moveable block = null;
        
        Vector3 origin = new Vector3(
            transform.position.x,
            transform.position.y + 1.5f,
            transform.position.z
        );

        Ray ray = new Ray(origin, transform.forward);
        RaycastHit hit;

        Debug.DrawRay(origin, transform.forward, Color.magenta);

        if (Physics.Raycast(ray, out hit, 5f, m_moveableLayerMask)) {
            block = hit.collider.GetComponentInChildren<Moveable>();
        }

        return block;
    }

    /// <summary>
    /// Handles applying rotation and movement based on player input
    /// </summary>
    void RotateAndMove()
    {
        float moveSpeed = 0;

        Vector3 inputVector = m_joystick.InputVector;// m_inputManager.InputVector;

        if(inputVector.magnitude > 1) {
            inputVector.Normalize();
        }

        // Process movement/rotations
        if (inputVector != Vector3.zero) {

            // Face direction before moving 
            if (m_playerMover.Rotate(inputVector)) {
                m_playerMover.Move(inputVector);
            }

            // Because the speed decrement clamps between 0 and 1
            // and it represents how fast the player can move we use it to update the animator
            moveSpeed = m_playerMover.SpeedDecrement;
        }

        m_playerAnimator.UpdateMoveSpeed(moveSpeed);
    }

    /// <summary>
    /// Turns the player to look the given object
    /// </summary>
    /// <param name="targetPos"></param>
    public void LookAtObject(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    /// <summary>
    /// Triggers the routine to push an object in the direction the player is moving
    /// </summary>
    /// <param name="moveableObject"></param>
    public void PushObject(Moveable moveableObject)
    {
        BaseObject tileObject = moveableObject.ParentTransform.GetComponent<BaseObject>();

        Vector3 directionVector = Utility.GetDirectionVectorByName(LookingAtDirection);
        Vector3 playerDestination = moveableObject.ParentTransform.position;
        Vector3 objectDestination = MapController.instance.GetTileAt(tileObject.Index + directionVector).transform.position;

        // The y axis shall remain as it is currently
        playerDestination.y = transform.position.y;
        objectDestination.y = moveableObject.ParentTransform.position.y;

        // Get the tile in the direction our object will be moving
        BaseTile tile = MapController.instance.GetTileAt(tileObject.Index);
        Vector3 targetIndex = tile.Index + directionVector;

        PushPullObject(targetIndex, tile, playerDestination, objectDestination, moveableObject.ParentTransform);
    }

    /// <summary>
    /// Triggers the routine to push an object in the direction the player is moving
    /// </summary>
    /// <param name="moveableObject"></param>
    public void PullObject(Moveable moveableObject)
    {
        BaseObject tile = moveableObject.ParentTransform.GetComponent<BaseObject>();

        Vector3 directionVector = Utility.GetDirectionVectorByName(LookingAtDirection);
        Vector3 playerDestination = MapController.instance.GetTileAt (PlayerTile.Index + directionVector).transform.position;
        Vector3 objectDestination = PlayerTile.transform.position;

        // The y axis shall remain as it is currently
        playerDestination.y = transform.position.y;
        objectDestination.y = moveableObject.ParentTransform.position.y;        

        // Because the map "centers" itself that means that the x,z coordinates on the transform will not
        // match the map's array. We must instead get this value from the tile that the player/object is on
        BaseTile objectTile = MapController.instance.GetTileAt(tile.Index);
        Vector3 targetIndex = PlayerTile.Index + directionVector;

        PushPullObject(targetIndex, objectTile, playerDestination, objectDestination, moveableObject.ParentTransform);
    }

    /// <summary>
    /// TODO: Refactor the Push and Pull methods as 
    /// </summary>
    void PushPullObject(Vector3 targetIndex, BaseTile objectTile, Vector3 playerDestination, Vector3 objectDestination, Transform objectTransform)
    {
        if (IsTargetTileAvailable(targetIndex)) {
            // Update the map to reflect the object's new position
            MapController.instance.UpdateObjectPosition(objectTile.Index, targetIndex);

            m_playerAnimator.TriggerPushAction();
            StartCoroutine(m_playerMover.PushPullRoutine(playerDestination, objectDestination, objectTransform));
        } else {
            Debug.LogFormat("Target Index {0} is not available", targetIndex);
        }
    }

    /// <summary>
    /// Returns true so long as at the destination contains a walkable tile and the tile contains
    /// a walkable object or no object at all
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    bool IsTargetTileAvailable(Vector3 targetPosition)
    {
        bool isAvailable = false;

        targetPosition.Set(
            Mathf.Floor(targetPosition.x),
            0f,
            Mathf.Floor(targetPosition.z)
        );

        BaseTile tile = MapController.instance.GetTileAt(targetPosition);

        // Destination must be on a floor type of the same type as the player is currently on
        if (tile != null && PlayerTile != null && tile.CompareTag(PlayerTile.tag)) {
            // Make sure that any objects on the destination tile are walkable
            BaseObject tileObject = tile.ObjectOnTile;

            if (tileObject == null || (tileObject != null && tileObject.IsWalkable)) {
                isAvailable = true;
            }
        }

        return isAvailable;
    }

    /// <summary>
    /// Cast a ray in the direction of the push/pull returning true when nothing blocking it
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    bool IsPushPullDestinationAvailable(Vector3 origin, Vector3 direction, Transform objectTransform)
    {
        bool isAvailable = true;

        // Raise the origin to avoid collision with the floor
        origin.y += .5f;

        Debug.DrawLine(origin, origin + direction * m_oneTileUnits, Color.red, .25f);
        Ray ray = new Ray(origin, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, m_oneTileUnits + .25f);

        foreach (RaycastHit hit in hits) {
            GameObject other = hit.collider.gameObject;
            Collider collider = other.GetComponent<Collider>();
            Transform objectParent = other.transform.parent;

            // Avoid collision with the player, the object being moved, and any trigger collider
            if (!collider.isTrigger && other != gameObject && other != objectTransform.gameObject) {

                // It may be a child of the object we are moving, skip it if it is
                if(objectParent != null && objectParent.gameObject == objectTransform.gameObject) {
                    continue;
                }

                isAvailable = false;
                break;
            }
        }

        return isAvailable;
    }

    /// <summary>
    /// Triggers the animation of the player turning gold or being healed
    /// </summary>
    /// <param name="percent"></param>
    void ChangeMaterialAlpha()
    {
        if (!m_isChangingAlpha) {
            StartCoroutine(ChangeMaterialAlphaRoutine());
        }
    }

    /// <summary>
    /// Animates the player turning into gold or being restored
    /// The renderer has a "gold material" with the alpha set to 0
    /// We increase this number to slowly reveal the gold color
    /// </summary>
    /// <param name="percent"></param>
    /// <returns></returns>
    IEnumerator ChangeMaterialAlphaRoutine()
    {
        m_isChangingAlpha = true;

        // Alpha is clamp between 0 and 1 therefore we convert our 100 based number into a double
        float targetAlpha = m_currentCursePercent * .01f;
        Color materialColor = m_meshRenderer.materials[m_goldMaterialIndex].color;

        while (!Mathf.Approximately(materialColor.a, targetAlpha)) {
            materialColor.a = Mathf.Lerp(
                materialColor.a,
                targetAlpha,
                m_alphaChangeDelay * Time.deltaTime
            );
            
            m_meshRenderer.materials[m_goldMaterialIndex].color = materialColor;

            // Curse percent may have changed
            targetAlpha = m_currentCursePercent * .01f;
            yield return new WaitForEndOfFrame();
        }

        m_isChangingAlpha = false;
    }

    /// <summary>
    /// Triggers the death animations
    /// Waits for the animation to complete before marking the player as dead
    /// </summary>
    /// <returns></returns>
    IEnumerator DeathRoutine()
    {
        m_playerAnimator.TriggerDeath();

        // Wait for the animation to trigger
        yield return new WaitForSeconds(1f);

        while (!m_playerAnimator.IsDeathAnimationCompleted()) {
            yield return null;
        }

        m_isDead = true;
    }
}
