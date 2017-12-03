using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A previous explorer consumed by the cursed and bound to be a 
/// statue for the rest of eternety
/// 
/// Can be pushed/pulled by the player to trigger buttons, etc.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Statue : MonoBehaviour, IMoveable
{
    /// <summary>
    /// How far to cast the ray
    /// </summary>
    [SerializeField]
    float rayDistance = 1f;

    /// <summary>
    /// The layer where the player is on
    /// </summary>
    [SerializeField]
    LayerMask playerLayer;

    /// <summary>
    /// A reference to the rigidbody component
    /// </summary>
    new Rigidbody rigidbody;

    /// <summary>
    /// A reference to the collider component
    /// </summary>
    new Collider collider;

    /// <summary>
    /// Contains all the directions we want to cast a ray
    /// </summary>
    Dictionary<string, Vector3> directions = new Dictionary<string, Vector3>() {
        {"forward", Vector3.forward },
        {"left", Vector3.left },
        {"back", Vector3.back},
        {"right", Vector3.right},
    };

    /// <summary>
    /// Stores the name of the direction in which the player
    /// engaged with this statue
    /// </summary>
    string interactedFrom = "";
    public string InteractedFrom
    {
        get { return this.interactedFrom; }
    }
   
    /// <summary>
    /// Assigns references
    /// </summary>
    void Start()
    {
        this.rigidbody = GetComponent<Rigidbody>();
        this.collider = GetComponent<Collider>();
    }

    /// <summary>
    /// Stores the direction from which the player engaged with this statue
    /// Freeze all other axis excluding the one from which the player interacted from
    /// </summary>
    public void PlayerEngaged()
    {
        Vector3 origin = this.rigidbody.position;

        foreach (KeyValuePair<string, Vector3> dirInfo in this.directions) {
            string dirName = dirInfo.Key;
            Vector3 direction = dirInfo.Value;

            Ray ray = new Ray(origin, direction);
            RaycastHit hitInfo;

            Debug.DrawRay(origin, direction, Color.yellow);

            if (Physics.Raycast(ray, out hitInfo, this.rayDistance, this.playerLayer)) {
                this.interactedFrom = dirName;
                if (dirName == "forward" || dirName == "back") {
                    this.rigidbody.constraints = ~RigidbodyConstraints.FreezePositionZ;
                } else {
                    this.rigidbody.constraints = ~RigidbodyConstraints.FreezePositionX;
                }
                break;
            }
        }
    }

    /// <summary>
    /// Player is no longer interacting with this statue
    /// Freeze all contraints
    /// </summary>
    public void PlayerDisingaged()
    {
        this.interactedFrom = "";
        this.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    /// <summary>
    /// Called by the player to move this statue in the direction given
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void MoveTowardsDirection(Vector3 direction, float speed)
    {
        Vector3 targetPosition = this.rigidbody.position + direction * speed * Time.fixedDeltaTime;
        this.rigidbody.MovePosition(targetPosition);
    }
}
